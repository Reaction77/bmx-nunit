using Inedo.BuildMasterExtensions.NUnit3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inedo.BuildMasterExtensions.NUnit3.Xml.Parsing
{
    public class Parser
    {
        public TestRun ParseTestResultXml(XDocument xdoc)
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
            testRun.testsuite = ParseTestSuiteElement(testRun, testSuiteElement);

            return testRun;
        }

        private string ParseCommandLineNode(XNode commandLineNode)
        {
            return ((System.Xml.Linq.XElement)commandLineNode).Value;
        }

        private TestSuite ParseTestSuiteElement(TestRun testRun, XElement testSuiteElement)
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
                testSuite.testsuite = ParseTestSuiteElement(testRun, childTestSuiteElement);

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

                testRun.TestFixtures.Add(testSuite);
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
    }
}
