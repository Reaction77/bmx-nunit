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
using System.Collections.Generic;
using System.Linq;
using Inedo.BuildMasterExtensions.NUnit3.Model;
using Inedo.BuildMasterExtensions.NUnit3.Xml.Parsing;

namespace Inedo.BuildMasterExtensions.NUnit3
{
    [Tag(Tags.UnitTests)]
    [ScriptAlias("Execute-NUnit3")]
    [DisplayName("Execute NUnit3 Tests")]
    [Description("Runs NUnit3 unit tests on a specified project, assembly, or NUnit file.")]
    [ScriptNamespace("NUnit3", PreferUnqualified = true)]
    public sealed class NUnit3Operation : ExecuteOperation
    {
        [Required]
        [ScriptAlias("TestFile")]
        [DisplayName("Test file")]
        [Description("The file nunit will test against (could be dll, proj, nunit or any other file NUnit supports).")]
        public string TestFile { get; set; }

        [Required]
        [ScriptAlias("NUnitExePath")]
        [DisplayName("NUnit path")]
        [Description("The path to the nunit test runner executable.")]
        public string ExePath { get; set; }

        [ScriptAlias("Arguments")]
        [DisplayName("Additional arguments")]
        [Description("Raw command line arguments passed to the nunit test runner.")]
        public string AdditionalArguments { get; set; }

        [Required]
        [ScriptAlias("DeleteOutput")]
        [DisplayName("Delete output")]
        [Description("Indicates if test result output should be preserved on disk.")]
        public bool DeleteOutput { get; set; }

        [ScriptAlias("OutputDirectory")]
        [DisplayName("Output directory")]
        [Description("The directory to generate the XML test results.")]
        public string OutputPath { get; set; }

        [ScriptAlias("OutputResultsFileName")]
        [DisplayName("Results output file name")]
        [Description("Name of the results file (without file extension).")]
        public string OutputName { get; set; }

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

            string outputPath;
            if (String.IsNullOrEmpty(this.OutputPath))
                outputPath = context.WorkingDirectory;
            else
                outputPath = context.ResolvePath(this.OutputPath);

            this.LogDebug("Output directory: " + outputPath);

            string outputFileName = "TestResult.xml";
            if (!String.IsNullOrEmpty(this.OutputName))
            {
                outputFileName = this.OutputName + ".xml";
            }

            this.LogDebug("Output filename: " + outputFileName);

            var testResultsXmlFilePath = fileOps.CombinePath(outputPath, outputFileName);
            this.LogDebug("Output file: " + testResultsXmlFilePath);

            string args = GetCommandLineArguments(testFilePath, outputPath, this.AdditionalArguments);
            this.LogDebug("Command line arguments: " + args);

            try
            {
                this.LogDebug("Run tests");
                await this.ExecuteCommandLineAsync(
                    context,
                    new RemoteProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = args,
                        WorkingDirectory = context.WorkingDirectory
                    }
                );


                this.LogDebug("Read file: " + testResultsXmlFilePath);
                XDocument xdoc = ReadTestResultsFile(fileOps, testResultsXmlFilePath);
                TestRun testRun;

                try
                {
                    var parser = new Parser();
                    testRun = parser.ParseTestResultXml(xdoc);
                }
                catch (Exception ex)
                {
                    this.LogError(ex.Message);
                    this.LogError(ex.StackTrace);
                    return;
                }

                this.LogDebug($"Record results");

                RecordTestResults(context, testRun);


            }
            finally
            {
                if (this.DeleteOutput)
                {
                    this.LogDebug($"Deleting temp output file ({testResultsXmlFilePath})...");
                    try
                    {
                        fileOps.DeleteFile(testResultsXmlFilePath);
                    }
                    catch
                    {
                        this.LogWarning($"Could not delete {testResultsXmlFilePath}.");
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


        private void RecordTestResults(IOperationExecutionContext context, TestRun testRun)
        {
            var allTestCases = testRun.TestFixtures.SelectMany(tf => tf.testcase).ToList();
            int failures = 0;

            using (var db = new DB.Context())
            {
                foreach (var testCase in allTestCases)
                {
                    // skip tests that weren't actually run
                    if (TestCaseWasSkipped(testCase))
                    {
                        this.LogInformation($"NUnit test: {testCase.name} (skipped)");
                        continue;
                    }

                    var result = GetTestCaseResult(testCase);
                    if (result == Domains.TestStatusCodes.Failed)
                        failures++;

                    this.LogInformation($"NUnit test: {testCase.name}, Result: {Domains.TestStatusCodes.GetName(result)}, Test length: ~{Math.Round(testCase.duration)} seconds");

                    db.BuildTestResults_RecordTestResult(
                        Execution_Id: context.ExecutionId,
                        Group_Name: AH.NullIf(this.GroupName, String.Empty) ?? "NUnit",
                        Test_Name: testCase.name,
                        TestStatus_Code: result,
                        TestResult_Text: result == Domains.TestStatusCodes.Passed ? result : testCase.fullname,
                        TestStarted_Date: ParseTime(testCase.starttime),
                        TestEnded_Date: ParseTime(testCase.endtime)
                    );

                }
            }

            if (failures > 0)
                this.LogError($"{0} test failures were reported.");
        }

        private XDocument ReadTestResultsFile(IFileOperationsExecuter fileOps, string testResultsXmlFile)
        {
            XDocument xdoc;
            using (var stream = fileOps.OpenFile(testResultsXmlFile, FileMode.Open, FileAccess.Read))
            {
                xdoc = XDocument.Load(stream);
                this.LogDebug("File read");
            }
            return xdoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFilePath">The console program must always have an assembly or project specified. 
        /// Assemblies are specified by file name or path, which may be absolute or relative. Relative paths are interpreted based on the current directory.
        /// 
        /// In addition to assemblies, you may specify any project type that is understood by NUnit. Out of the box, this includes various Visual Studio
        /// project types as well as NUnit(.nunit) projects.</param>
        /// <param name="outputPath">PATH of the directory to use for output files.</param>
        /// <returns>Arguments ready to pass to the NUnit test runner</returns>
        private string GetCommandLineArguments(string inputFilePath, string outputPath, string additionalArguments)
        {
            var args = $"\"{inputFilePath}\" --work:\"{outputPath}\"";

            if (!String.IsNullOrEmpty(additionalArguments))
            {
                args += " " + this.AdditionalArguments;
            }

            return args;
        }

        private DateTime? ParseTime(string time)
        {
            string datePart = null;
            string timePart = null;
            var parts = time?.Split(' ');
            if (parts != null)
            {
                datePart = parts[0];
                timePart = parts[1];
            }

            if (string.IsNullOrWhiteSpace(datePart))
            {
                DateTime result;
                if (DateTime.TryParse(timePart, out result))
                    return result.ToUniversalTime();
            }

            if (!string.IsNullOrWhiteSpace(datePart) && !string.IsNullOrWhiteSpace(timePart))
            {
                var dateParts = datePart.Split('-');
                var timeParts = timePart.Split(':');

                return new DateTime(
                    year: int.Parse(dateParts[0]),
                    month: int.Parse(dateParts[1]),
                    day: int.Parse(dateParts[2]),
                    hour: int.Parse(timeParts[0]),
                    minute: int.Parse(timeParts[1]),
                    second: int.Parse(timeParts[2])
                ).ToUniversalTime();
            }

            this.LogWarning("Unable to parse start time; using current time instead.");
            return DateTime.UtcNow;
        }

        private bool TestCaseWasSkipped(TestCase testCase)
        {
            return String.Equals((string)testCase.result, "Skipped", StringComparison.OrdinalIgnoreCase);
        }

        private string GetTestCaseResult(TestCase testCase)
        {
            return AH.Switch<string, string>((string)testCase.result, StringComparer.OrdinalIgnoreCase)
                                .Case("Passed", Domains.TestStatusCodes.Passed)
                                .Case("Inconclusive", Domains.TestStatusCodes.Inconclusive)
                                .Default(Domains.TestStatusCodes.Failed)
                                .End();
        }
    }

}
