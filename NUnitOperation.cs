using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Inedo.Agents;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Data;
using Inedo.BuildMaster.Extensibility;
using Inedo.BuildMaster.Extensibility.Operations;
using Inedo.Diagnostics;
using Inedo.Documentation;

namespace Inedo.BuildMasterExtensions.NUnit
{
    [Tag(Tags.UnitTests)]
    [ScriptAlias("Execute-NUnit")]
    [DisplayName("Execute NUnit Tests")]
    [Description("Runs NUnit unit tests on a specified project, assembly, or NUnit file.")]
    [ScriptNamespace("NUnit", PreferUnqualified = true)]
    public sealed class NUnitOperation : ExecuteOperation
    {
        [Required]
        [ScriptAlias("TestFile")]
        [DisplayName("Test file")]
        [Description("The file nunit will test against (could be dll, proj, or config file based on test runner).")]
        public string TestFile { get; set; }
        [Required]
        [ScriptAlias("NUnitExePath")]
        [DisplayName("nunit path")]
        [Description("The path to the nunit test runner executable.")]
        public string ExePath { get; set; }
        [ScriptAlias("Arguments")]
        [DisplayName("Additional arguments")]
        [Description("Raw command line arguments passed to the nunit test runner.")]
        public string AdditionalArguments { get; set; }
        [ScriptAlias("OutputDirectory")]
        [DisplayName("Output directory")]
        [Description("The directory to generate the XML test results.")]
        public string CustomXmlOutputPath { get; set; }

        [Category("Advanced")]
        [ScriptAlias("Group")]
        [DisplayName("Group name")]
        [Description("When multiple sets of tests are performed, unique group names will categorize them in the UI.")]
        [PlaceholderText("NUnit")]
        public string GroupName { get; set; }

        public override async Task ExecuteAsync(IOperationExecutionContext context)
        {
            var fileOps = context.Agent.GetService<IFileOperationsExecuter>();
            var testFilePath = context.ResolvePath(this.TestFile);
            this.LogDebug("Test file: " + testFilePath);

            var exePath = context.ResolvePath(this.ExePath);
            this.LogDebug("Exe path: " + exePath);

            if (!fileOps.FileExists(testFilePath))
            {
                this.LogError($"Test file {testFilePath} does not exist.");
                return;
            }

            if (!fileOps.FileExists(exePath))
            {
                this.LogError($"NUnit runner not found at {exePath}.");
                return;
            }

            string outputFilePath;
            if (string.IsNullOrEmpty(this.CustomXmlOutputPath))
                outputFilePath = fileOps.CombinePath(context.WorkingDirectory, Guid.NewGuid().ToString("N") + ".xml");
            else
                outputFilePath = context.ResolvePath(this.CustomXmlOutputPath);

            this.LogDebug("Output file: " + outputFilePath);

            var args = $"\"{testFilePath}\" /xml:\"{outputFilePath}\"";
            if (!string.IsNullOrEmpty(this.AdditionalArguments))
            {
                this.LogDebug("Additional arguments: " + this.AdditionalArguments);
                args += " " + this.AdditionalArguments;
            }

            try
            {
                await this.ExecuteCommandLineAsync(
                    context,
                    new RemoteProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = args,
                        WorkingDirectory = context.WorkingDirectory
                    }
                );

                XDocument xdoc;
                using (var stream = fileOps.OpenFile(outputFilePath, FileMode.Open, FileAccess.Read))
                {
                    xdoc = XDocument.Load(stream);
                }

                var testResultsElement = xdoc.Element("test-results");

                var startTime = this.TryParseStartTime((string)testResultsElement.Attribute("date"), (string)testResultsElement.Attribute("time")) ?? DateTime.UtcNow;

                using (var db = new DB.Context())
                {
                    foreach (var testCaseElement in xdoc.Descendants("test-case"))
                    {
                        var testName = (string)testCaseElement.Attribute("name");

                        // skip tests that weren't actually run
                        if (string.Equals((string)testCaseElement.Attribute("executed"), "False", StringComparison.OrdinalIgnoreCase))
                        {
                            this.LogInformation($"NUnit test: {testName} (skipped)");
                            continue;
                        }

                        var result = AH.Switch<string, string>((string)testCaseElement.Attribute("success"), StringComparer.OrdinalIgnoreCase)
                            .Case("True", Domains.TestStatusCodes.Passed)
                            .Case("Inconclusive", Domains.TestStatusCodes.Inconclusive)
                            .Default(Domains.TestStatusCodes.Failed)
                            .End();

                        var testDuration = this.TryParseTestTime((string)testCaseElement.Attribute("time"));

                        this.LogInformation($"NUnit test: {testName}, Result: {Domains.TestStatusCodes.GetName(result)}, Test length: {testDuration}");

                        db.BuildTestResults_RecordTestResult(
                            Execution_Id: context.ExecutionId,
                            Group_Name: AH.NullIf(this.GroupName, string.Empty) ?? "NUnit",
                            Test_Name: testName,
                            TestStatus_Code: result,
                            TestResult_Text: testCaseElement.ToString(),
                            TestStarted_Date: startTime,
                            TestEnded_Date: startTime + testDuration
                        );

                        startTime += testDuration;
                    }
                }
            }
            finally
            {
                if (string.IsNullOrEmpty(this.CustomXmlOutputPath))
                {
                    this.LogDebug($"Deleting temp output file ({outputFilePath})...");
                    try
                    {
                        fileOps.DeleteFile(outputFilePath);
                    }
                    catch
                    {
                        this.LogWarning($"Could not delete {outputFilePath}.");
                    }
                }
            }
        }

        protected override ExtendedRichDescription GetDescription(IOperationConfiguration config)
        {
            var longActionDescription = new RichDescription();
            if (!string.IsNullOrWhiteSpace(config[nameof(this.AdditionalArguments)]))
            {
                longActionDescription.AppendContent(
                    "with additional arguments: ",
                    new Hilite(config[nameof(this.AdditionalArguments)])
                );
            }

            return new ExtendedRichDescription(
                new RichDescription(
                    "Run NUnit on ",
                    new DirectoryHilite(config[nameof(this.TestFile)])
                ),
                longActionDescription
            );
        }

        private DateTime? TryParseStartTime(string date, string time)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(date))
                {
                    DateTime result;
                    if (DateTime.TryParse(time, out result))
                        return result.ToUniversalTime();
                }

                if (!string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(time))
                {
                    var dateParts = date.Split('-');
                    var timeParts = time.Split(':');

                    return new DateTime(
                        year: int.Parse(dateParts[0]),
                        month: int.Parse(dateParts[1]),
                        day: int.Parse(dateParts[2]),
                        hour: int.Parse(timeParts[0]),
                        minute: int.Parse(timeParts[1]),
                        second: int.Parse(timeParts[2])
                    ).ToUniversalTime();
                }
            }
            catch
            {
            }

            this.LogWarning("Unable to parse start time; using current time instead.");
            return null;
        }
        private TimeSpan TryParseTestTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                return TimeSpan.Zero;

            var mungedTime = time.Replace(',', '.');
            double doubleTime;
            bool parsed = double.TryParse(
                mungedTime,
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture,
                out doubleTime
            );

            if (!parsed)
                this.LogWarning($"Could not parse {time} as a time in seconds.");

            return TimeSpan.FromSeconds(doubleTime);
        }
    }
}
