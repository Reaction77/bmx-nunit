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
using Inedo.BuildMasterExtensions.NUnit.Model;
using System.Linq;

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

            var testResultsXmlFile = fileOps.CombinePath(outputPath, outputFileName);
            this.LogDebug("Output file: " + testResultsXmlFile);

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


                this.LogDebug("Read file: " + testResultsXmlFile);
                XDocument xdoc = ReadTestResultsFile(fileOps, testResultsXmlFile);

                try
                {
                    Model.TestRun testRun = ParseTestResultXml(xdoc);
                }
                catch (Exception ex)
                {
                    this.LogError(ex.Message);
                    this.LogError(ex.StackTrace);
                    return;
                }

                this.LogDebug($"Record results");


                string resultsNodeName = "test-run";

                var testResultsElement = xdoc.Element(resultsNodeName);

                var startTime = this.TryParseStartTime(testResultsElement);
                var failures = 0;

                using (var db = new DB.Context())
                {
                    foreach (var testCaseElement in xdoc.Descendants("test-case"))
                    {
                        var testName = (string)testCaseElement.Attribute("name");

                        // skip tests that weren't actually run
                        if (TestCaseWasSkipped(testCaseElement))
                        {
                            this.LogInformation($"NUnit test: {testName} (skipped)");
                            continue;
                        }

                        var result = GetTestCaseResult(testCaseElement);
                        if (result == Domains.TestStatusCodes.Failed)
                            failures++;

                        var testDuration = this.TryParseTestTime((string)testCaseElement.Attribute("time"));

                        this.LogInformation($"NUnit test: {testName}, Result: {Domains.TestStatusCodes.GetName(result)}, Test length: {testDuration}");

                        db.BuildTestResults_RecordTestResult(
                            Execution_Id: context.ExecutionId,
                            Group_Name: AH.NullIf(this.GroupName, string.Empty) ?? "NUnit",
                            Test_Name: testName,
                            TestStatus_Code: result,
                            TestResult_Text: result == Domains.TestStatusCodes.Passed ? result : testCaseElement.ToString(),
                            TestStarted_Date: startTime,
                            TestEnded_Date: startTime + testDuration
                        );

                        startTime += testDuration;
                    }
                }

                if (failures > 0)
                    this.LogError($"{0} test failures were reported.");
            }
            finally
            {
                if (string.IsNullOrEmpty(this.OutputPath))
                {
                    this.LogDebug($"Deleting temp output file ({testResultsXmlFile})...");
                    try
                    {
                        fileOps.DeleteFile(testResultsXmlFile);
                    }
                    catch
                    {
                        this.LogWarning($"Could not delete {testResultsXmlFile}.");
                    }
                }
            }
        }

        private TestRun ParseTestResultXml(XDocument xdoc)
        {
            return xdoc.Descendants("test-run").Select(e => ParseTestRunElement(e)).FirstOrDefault();
        }

        private TestRun ParseTestRunElement(XElement e)
        {
            var testRun = new TestRun()
            {
                id = Convert.ToInt32(e.Attribute("id").Value),
                testcasecount = Convert.ToInt32(e.Attribute("testcasecount").Value),
                result = e.Attribute("result").Value,
                total = Convert.ToInt32(e.Attribute("total").Value),
                passed = Convert.ToInt32(e.Attribute("passed").Value),
                failed = Convert.ToInt32(e.Attribute("failed").Value),
                inconclusive = Convert.ToInt32(e.Attribute("inconclusive").Value),
                skipped = Convert.ToInt32(e.Attribute("skipped").Value),
                asserts = Convert.ToInt32(e.Attribute("asserts").Value),
                engineversion = e.Attribute("engine-version").Value,
                clrversion = e.Attribute("clr-version").Value,
                starttime = e.Attribute("start-time").Value,
                endtime = e.Attribute("end-time").Value,
                duration = Convert.ToDecimal(e.Attribute("duration").Value)
            };

            //TODO: refactor this check of XmlNodeType and Element name
            var commandLineNode = e.DescendantNodes().FirstOrDefault(n => n.NodeType == System.Xml.XmlNodeType.Element && ((System.Xml.Linq.XElement)n).Name == "command-line");
            testRun.commandline = ParseCommandLineNode(commandLineNode);

            var testSuiteElement = e.Descendants("test-suite").FirstOrDefault();
            testRun.testsuite = ParseTestSuiteElement(testSuiteElement);

            return testRun;
        }

        private string ParseCommandLineNode(XNode commandLineNode)
        {
            return ((System.Xml.Linq.XElement)commandLineNode).Value;
        }

        private TestSuite ParseTestSuiteElement(XElement testSuiteElement)
        {
            var testSuite = new TestSuite()
            {
                type = testSuiteElement.Attribute("type").Value,
                id = testSuiteElement.Attribute("id").Value,
                name = testSuiteElement.Attribute("name").Value,
                fullname = testSuiteElement.Attribute("fullname").Value,
                runstate = testSuiteElement.Attribute("runstate").Value,
                testcasecount = Convert.ToInt32(testSuiteElement.Attribute("testcasecount").Value),
                result = testSuiteElement.Attribute("result").Value,
                starttime = testSuiteElement.Attribute("start-time").Value,
                endtime = testSuiteElement.Attribute("end-time").Value,
                duration = Convert.ToDecimal(testSuiteElement.Attribute("duration").Value),
                total = Convert.ToInt32(testSuiteElement.Attribute("total").Value),
                passed = Convert.ToInt32(testSuiteElement.Attribute("passed").Value),
                failed = Convert.ToInt32(testSuiteElement.Attribute("failed").Value),
                warnings = Convert.ToInt32(testSuiteElement.Attribute("warnings").Value),
                inconclusive = Convert.ToInt32(testSuiteElement.Attribute("inconclusive").Value),
                skipped = Convert.ToInt32(testSuiteElement.Attribute("skipped").Value)
            };

            if (testSuite.type == "Assembly")
            { 
                var environmentElement = testSuiteElement.DescendantNodes().FirstOrDefault(n => n.NodeType == System.Xml.XmlNodeType.Element && ((System.Xml.Linq.XElement)n).Name == "enivronment");
                //TODO: Parse environment

                var settingsElement = testSuiteElement.DescendantNodes().FirstOrDefault(n => n.NodeType == System.Xml.XmlNodeType.Element && ((System.Xml.Linq.XElement)n).Name == "settings");
                //TODO: Parse settings

                var propertiesElement = testSuiteElement.DescendantNodes().FirstOrDefault(n => n.NodeType == System.Xml.XmlNodeType.Element && ((System.Xml.Linq.XElement)n).Name == "properties");
                //TODO: Parse properties
            }

            if (testSuite.type == "TestSuite")
            {
                var childTestSuiteElement = testSuiteElement.Descendants("test-suite").FirstOrDefault();
                testSuite.testsuite = ParseTestSuiteElement(childTestSuiteElement);
            }


            if (testSuite.type == "TestFixture")
            {
                var testCases = new List<TestCase>();
                testSuiteElement.Descendants("test-case")
                                .ToList()
                                .ForEach(tce =>
                                {
                                    testCases.Add(ParseTestCaseElement(tce));
                                });

                testSuite.testcase = testCases.ToArray();
            }

            return testSuite;
        }

        private TestCase ParseTestCaseElement(XElement testCaseElement)
        {
            var testCase = new TestCase()
            {
                id = testCaseElement.Attribute("id").Value,
                name = testCaseElement.Attribute("name").Value,
                fullname = testCaseElement.Attribute("fullname").Value,
                methodname = testCaseElement.Attribute("methodname").Value,
                classname = testCaseElement.Attribute("classname").Value,
                runstate = testCaseElement.Attribute("runstate").Value,
                seed = Convert.ToUInt32(testCaseElement.Attribute("start-time").Value),
                result = testCaseElement.Attribute("end-time").Value,
                starttime = testCaseElement.Attribute("duration").Value,
                endtime = testCaseElement.Attribute("total").Value,
                duration = Convert.ToDecimal(testCaseElement.Attribute("passed").Value),
                asserts = Convert.ToInt32(testCaseElement.Attribute("failed").Value)
            };

            return testCase;
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
                //this.LogDebug("Additional arguments: " + this.AdditionalArguments);
                args += " " + this.AdditionalArguments;
            }

            return args;
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

        private DateTime? TryParseStartTime(XElement rootNode)
        {
            string date = null;
            string time = null;
            string startTime = (string)rootNode.Attribute("start-time");
            var startTimeParts = startTime?.Split(' ');
            if (startTimeParts != null)
            {
                date = startTimeParts[0];
                time = startTimeParts[1];
            }

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
            return DateTime.UtcNow;
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

        private bool TestCaseWasSkipped(XElement testCaseElement)
        {
            return string.Equals((string)testCaseElement.Attribute("result"), "Skipped", StringComparison.OrdinalIgnoreCase);
        }

        private string GetTestCaseResult(XElement testCaseElement)
        {

            return AH.Switch<string, string>((string)testCaseElement.Attribute("result"), StringComparer.OrdinalIgnoreCase)
                                .Case("Passed", Domains.TestStatusCodes.Passed)
                                .Case("Inconclusive", Domains.TestStatusCodes.Inconclusive)
                                .Default(Domains.TestStatusCodes.Failed)
                                .End();
        }
    }
}
