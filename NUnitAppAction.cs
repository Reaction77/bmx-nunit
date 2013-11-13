using System;
using System.IO;
using System.Xml;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Extensibility.Actions.Testing;
using Inedo.BuildMaster.Extensibility.Agents;
using Inedo.BuildMaster.Web;

namespace Inedo.BuildMasterExtensions.NUnit
{
    /// <summary>
    /// Action that runs NUnit unit tests on a specified project, assembly, or NUnit file.
    /// </summary>
    [ActionProperties(
        "Execute NUnit Tests",
        "Runs NUnit unit tests on a specified project, assembly, or NUnit file.",
        "Testing")]
    [CustomEditor(typeof(NUnitActionEditor))]
    public sealed class NUnitAppAction : UnitTestActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitAppAction"/> class.
        /// </summary>
        public NUnitAppAction()
        {
        }

        /// <summary>
        /// Gets or sets the test runner exe path
        /// </summary>
        [Persistent]
        public string ExePath { get; set; }

        /// <summary>
        /// Gets or sets the file nunit will test against (could be dll, proj, or config file based on test runner)
        /// </summary>
        [Persistent]
        public string TestFile { get; set; }

        /// <summary>
        /// Gets or sets the .NET Framework version to run against.
        /// </summary>
        [Persistent]
        public string FrameworkVersion { get; set; }

        /// <summary>
        /// Gets or sets the additional arguments.
        /// </summary>
        [Persistent]
        public string AdditionalArguments { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <remarks>
        /// This should return a user-friendly string describing what the Action does
        /// and the state of its important persistent properties.
        /// </remarks>
        public override string ToString()
        {
            return string.Format("Run NUnit Unit Tests on {0}{1}", this.TestFile, Util.ConcatNE(" with the additional arguments: ", this.AdditionalArguments));
        }

        /// <summary>
        /// Runs a unit test against the target specified in the action.
        /// After the test is run, use the <see cref="M:RecordResult" /> method
        /// to save the test results to the database.
        /// </summary>
        protected override void RunTests()
        {
            var doc = new XmlDocument();

            var agent = this.Context.Agent;
            {
                var fileOps = agent.GetService<IFileOperationsExecuter>();
                var nunitPath = fileOps.GetWorkingDirectory(this.Context.ApplicationId, this.Context.DeployableId ?? 0, this.ExePath);

                var tmpFileName = fileOps.CombinePath(this.Context.TempDirectory, Guid.NewGuid().ToString() + ".xml");

                this.ExecuteCommandLine(
                    nunitPath,
                    string.Format("\"{0}\" /xml:\"{1}\" {2}", this.TestFile, tmpFileName, this.AdditionalArguments),
                    this.Context.SourceDirectory
                );

                using (var stream = new MemoryStream(fileOps.ReadFileBytes(tmpFileName), false))
                {
                    doc.Load(stream);
                }
            }

            var testStart = DateTime.Parse(doc.SelectSingleNode("//test-results").Attributes["time"].Value);

            var nodeList = doc.SelectNodes("//test-case");

            foreach (XmlNode node in nodeList)
            {
                string testName = node.Attributes["name"].Value;

                // skip tests that weren't actually run
                if (string.Equals(node.Attributes["executed"].Value, "false", StringComparison.OrdinalIgnoreCase))
                {
                    LogInformation(String.Format("NUnit Test: {0} (skipped)", testName));
                    continue;
                }
                
                bool nodeResult = node.Attributes["success"].Value.Equals("True", StringComparison.OrdinalIgnoreCase);

                double testLength = double.Parse(node.Attributes["time"].Value);

                this.LogInformation(string.Format("NUnit Test: {0}, Result: {1}, Test Length: {2} secs",
                    testName,
                    nodeResult,
                    testLength));

                this.RecordResult(
                    testName,
                    nodeResult,
                    node.OuterXml,
                    testStart,
                    testStart.AddSeconds(testLength)
                );

                testStart = testStart.AddSeconds(testLength);
            }
        }
    }
}
