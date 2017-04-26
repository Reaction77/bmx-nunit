using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inedo.BuildMasterExtensions.NUnit.Model
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("test-run", Namespace = "", IsNullable = false)]
    public partial class TestRun
    {

        private string commandlineField;

        private TestSuite testsuiteField;

        private int idField;

        private int testcasecountField;

        private string resultField;

        private int totalField;

        private int passedField;

        private int failedField;

        private int inconclusiveField;

        private int skippedField;

        private int assertsField;

        private string engineversionField;

        private string clrversionField;

        private string starttimeField;

        private string endtimeField;

        private decimal durationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("command-line")]
        public string commandline
        {
            get
            {
                return this.commandlineField;
            }
            set
            {
                this.commandlineField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("test-suite")]
        public TestSuite testsuite
        {
            get
            {
                return this.testsuiteField;
            }
            set
            {
                this.testsuiteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int testcasecount
        {
            get
            {
                return this.testcasecountField;
            }
            set
            {
                this.testcasecountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int passed
        {
            get
            {
                return this.passedField;
            }
            set
            {
                this.passedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int failed
        {
            get
            {
                return this.failedField;
            }
            set
            {
                this.failedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int inconclusive
        {
            get
            {
                return this.inconclusiveField;
            }
            set
            {
                this.inconclusiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int skipped
        {
            get
            {
                return this.skippedField;
            }
            set
            {
                this.skippedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int asserts
        {
            get
            {
                return this.assertsField;
            }
            set
            {
                this.assertsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("engine-version")]
        public string engineversion
        {
            get
            {
                return this.engineversionField;
            }
            set
            {
                this.engineversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("clr-version")]
        public string clrversion
        {
            get
            {
                return this.clrversionField;
            }
            set
            {
                this.clrversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("start-time")]
        public string starttime
        {
            get
            {
                return this.starttimeField;
            }
            set
            {
                this.starttimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("end-time")]
        public string endtime
        {
            get
            {
                return this.endtimeField;
            }
            set
            {
                this.endtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal duration
        {
            get
            {
                return this.durationField;
            }
            set
            {
                this.durationField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TestSuite
    {

        private Environment environmentField;

        private Setting[] settingsField;

        private Property[] propertiesField;

        private TestCase[] testcaseField;

        private TestSuite testsuiteField;

        private string typeField;

        private string idField;

        private string nameField;

        private string fullnameField;

        private string runstateField;

        private int testcasecountField;

        private string resultField;

        private string starttimeField;

        private string endtimeField;

        private decimal durationField;

        private int totalField;

        private int passedField;

        private int failedField;

        private int warningsField;

        private int inconclusiveField;

        private int skippedField;

        private int assertsField;

        /// <remarks/>
        public Environment environment
        {
            get
            {
                return this.environmentField;
            }
            set
            {
                this.environmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("setting", IsNullable = false)]
        public Setting[] settings
        {
            get
            {
                return this.settingsField;
            }
            set
            {
                this.settingsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("property", IsNullable = false)]
        public Property[] properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("test-case")]
        public TestCase[] testcase
        {
            get
            {
                return this.testcaseField;
            }
            set
            {
                this.testcaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("test-suite")]
        public TestSuite testsuite
        {
            get
            {
                return this.testsuiteField;
            }
            set
            {
                this.testsuiteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fullname
        {
            get
            {
                return this.fullnameField;
            }
            set
            {
                this.fullnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string runstate
        {
            get
            {
                return this.runstateField;
            }
            set
            {
                this.runstateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int testcasecount
        {
            get
            {
                return this.testcasecountField;
            }
            set
            {
                this.testcasecountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("start-time")]
        public string starttime
        {
            get
            {
                return this.starttimeField;
            }
            set
            {
                this.starttimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("end-time")]
        public string endtime
        {
            get
            {
                return this.endtimeField;
            }
            set
            {
                this.endtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal duration
        {
            get
            {
                return this.durationField;
            }
            set
            {
                this.durationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int passed
        {
            get
            {
                return this.passedField;
            }
            set
            {
                this.passedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int failed
        {
            get
            {
                return this.failedField;
            }
            set
            {
                this.failedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int warnings
        {
            get
            {
                return this.warningsField;
            }
            set
            {
                this.warningsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int inconclusive
        {
            get
            {
                return this.inconclusiveField;
            }
            set
            {
                this.inconclusiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int skipped
        {
            get
            {
                return this.skippedField;
            }
            set
            {
                this.skippedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int asserts
        {
            get
            {
                return this.assertsField;
            }
            set
            {
                this.assertsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Environment
    {

        private string frameworkversionField;

        private string clrversionField;

        private string osversionField;

        private string platformField;

        private string cwdField;

        private string machinenameField;

        private string userField;

        private string userdomainField;

        private string cultureField;

        private string uicultureField;

        private string osarchitectureField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("framework-version")]
        public string frameworkversion
        {
            get
            {
                return this.frameworkversionField;
            }
            set
            {
                this.frameworkversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("clr-version")]
        public string clrversion
        {
            get
            {
                return this.clrversionField;
            }
            set
            {
                this.clrversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("os-version")]
        public string osversion
        {
            get
            {
                return this.osversionField;
            }
            set
            {
                this.osversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string platform
        {
            get
            {
                return this.platformField;
            }
            set
            {
                this.platformField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cwd
        {
            get
            {
                return this.cwdField;
            }
            set
            {
                this.cwdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("machine-name")]
        public string machinename
        {
            get
            {
                return this.machinenameField;
            }
            set
            {
                this.machinenameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string user
        {
            get
            {
                return this.userField;
            }
            set
            {
                this.userField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("user-domain")]
        public string userdomain
        {
            get
            {
                return this.userdomainField;
            }
            set
            {
                this.userdomainField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string culture
        {
            get
            {
                return this.cultureField;
            }
            set
            {
                this.cultureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string uiculture
        {
            get
            {
                return this.uicultureField;
            }
            set
            {
                this.uicultureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("os-architecture")]
        public string osarchitecture
        {
            get
            {
                return this.osarchitectureField;
            }
            set
            {
                this.osarchitectureField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Setting
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Property
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class testrunTestsuiteTestsuiteTestsuiteTestsuiteTestsuiteTestsuiteTestsuiteTestsuite
    {

        private TestCase[] testcaseField;

        private string typeField;

        private string idField;

        private string nameField;

        private string fullnameField;

        private string classnameField;

        private string runstateField;

        private int testcasecountField;

        private string resultField;

        private string starttimeField;

        private string endtimeField;

        private decimal durationField;

        private int totalField;

        private int passedField;

        private int failedField;

        private int warningsField;

        private int inconclusiveField;

        private int skippedField;

        private int assertsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("test-case")]
        public TestCase[] testcase
        {
            get
            {
                return this.testcaseField;
            }
            set
            {
                this.testcaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fullname
        {
            get
            {
                return this.fullnameField;
            }
            set
            {
                this.fullnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string classname
        {
            get
            {
                return this.classnameField;
            }
            set
            {
                this.classnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string runstate
        {
            get
            {
                return this.runstateField;
            }
            set
            {
                this.runstateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int testcasecount
        {
            get
            {
                return this.testcasecountField;
            }
            set
            {
                this.testcasecountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("start-time")]
        public string starttime
        {
            get
            {
                return this.starttimeField;
            }
            set
            {
                this.starttimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("end-time")]
        public string endtime
        {
            get
            {
                return this.endtimeField;
            }
            set
            {
                this.endtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal duration
        {
            get
            {
                return this.durationField;
            }
            set
            {
                this.durationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int passed
        {
            get
            {
                return this.passedField;
            }
            set
            {
                this.passedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int failed
        {
            get
            {
                return this.failedField;
            }
            set
            {
                this.failedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int warnings
        {
            get
            {
                return this.warningsField;
            }
            set
            {
                this.warningsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int inconclusive
        {
            get
            {
                return this.inconclusiveField;
            }
            set
            {
                this.inconclusiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int skipped
        {
            get
            {
                return this.skippedField;
            }
            set
            {
                this.skippedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int asserts
        {
            get
            {
                return this.assertsField;
            }
            set
            {
                this.assertsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TestCase
    {

        private string idField;

        private string nameField;

        private string fullnameField;

        private string methodnameField;

        private string classnameField;

        private string runstateField;

        private uint seedField;

        private string resultField;

        private string starttimeField;

        private string endtimeField;

        private decimal durationField;

        private int assertsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fullname
        {
            get
            {
                return this.fullnameField;
            }
            set
            {
                this.fullnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string methodname
        {
            get
            {
                return this.methodnameField;
            }
            set
            {
                this.methodnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string classname
        {
            get
            {
                return this.classnameField;
            }
            set
            {
                this.classnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string runstate
        {
            get
            {
                return this.runstateField;
            }
            set
            {
                this.runstateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint seed
        {
            get
            {
                return this.seedField;
            }
            set
            {
                this.seedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("start-time")]
        public string starttime
        {
            get
            {
                return this.starttimeField;
            }
            set
            {
                this.starttimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("end-time")]
        public string endtime
        {
            get
            {
                return this.endtimeField;
            }
            set
            {
                this.endtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal duration
        {
            get
            {
                return this.durationField;
            }
            set
            {
                this.durationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int asserts
        {
            get
            {
                return this.assertsField;
            }
            set
            {
                this.assertsField = value;
            }
        }
    }


}
