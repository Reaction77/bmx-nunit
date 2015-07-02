using System.Linq;
using System.Web.UI.WebControls;
using Inedo.BuildMaster.Data;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.NUnit
{
    internal sealed class NUnitActionEditor : ActionEditorBase
    {
        private SourceControlFileFolderPicker txtExePath;
        private ValidatingTextBox txtTestFile, txtGroupName;
        private DropDownList ddlFrameworkVersion;
        private ValidatingTextBox txtAdditionalArguments;
        private ValidatingTextBox txtCustomXmlOutputPath;
        private CheckBox chkTreatInconclusiveTestsAsFailure;

        public override bool DisplaySourceDirectory
        {
            get { return true; }
        }

        protected override void CreateChildControls()
        {
            Tables.Deployables_Extended deployable = null;
            if (this.DeployableId > 0)
            {
                deployable = StoredProcs.Applications_GetDeployable(this.DeployableId)
                   .Execute()
                   .FirstOrDefault();
            }

            this.txtExePath = new SourceControlFileFolderPicker
            {
                DisplayMode = SourceControlBrowser.DisplayModes.FoldersAndFiles,
                ServerId = this.ServerId,
                DefaultText = "default for selected configuration"
            };

            this.txtGroupName = new ValidatingTextBox
            {
                Text = deployable != null ? deployable.Deployable_Name : string.Empty,
                Required = true
            };

            this.txtTestFile = new ValidatingTextBox
            {
                Required = true
            };

            this.ddlFrameworkVersion = new DropDownList
            {
                Items =
                {
                    new ListItem("2.0.50727", "2.0.50727"),
                    new ListItem("4.0.30319", "4.0.30319"),
                    new ListItem("default", string.Empty)
                }
            };

            this.ddlFrameworkVersion.SelectedValue = string.Empty;

            this.txtAdditionalArguments = new ValidatingTextBox
            {
                DefaultText = "none"
            };

            this.txtCustomXmlOutputPath = new ValidatingTextBox
            {
                DefaultText = "managed by BuildMaster"
            };

            this.chkTreatInconclusiveTestsAsFailure = new CheckBox
            {
                Text = "Treat inconclusive tests as failures",
                Checked = true
            };

            this.Controls.Add(
                new SlimFormField("Unit test group:", this.txtGroupName),
                new SlimFormField("NUnit-console.exe path:", this.txtExePath)
                {
                    HelpText = "The path to (and including) nunit-console.exe if using a different version of NUnit than the one specified "
                             + "in the NUnit extension configuration."
                },
                new SlimFormField("CLR version:", this.ddlFrameworkVersion),
                new SlimFormField("Test file:", this.txtTestFile)
                {
                    HelpText = "This should normally be a .dll or project file."
                },
                new SlimFormField("XML output path:", this.txtCustomXmlOutputPath),
                new SlimFormField("Additional NUnit arguments:", this.txtAdditionalArguments),
                new SlimFormField("Options:", this.chkTreatInconclusiveTestsAsFailure)
            );
        }

        public override void BindToForm(ActionBase extension)
        {
            var nunitAction = (NUnitAppAction)extension;

            this.txtExePath.Text = nunitAction.ExePath;
            this.txtTestFile.Text = nunitAction.TestFile;
            this.txtGroupName.Text = nunitAction.GroupName;
            this.ddlFrameworkVersion.SelectedValue = nunitAction.FrameworkVersion ?? "";
            this.txtAdditionalArguments.Text = nunitAction.AdditionalArguments;
            this.txtCustomXmlOutputPath.Text = nunitAction.CustomXmlOutputPath;
            this.chkTreatInconclusiveTestsAsFailure.Checked = nunitAction.TreatInconclusiveAsFailure;
        }

        public override ActionBase CreateFromForm()
        {
            return new NUnitAppAction
            {
                ExePath = this.txtExePath.Text,
                TestFile = this.txtTestFile.Text,
                GroupName = this.txtGroupName.Text,
                FrameworkVersion = this.ddlFrameworkVersion.SelectedValue,
                AdditionalArguments = this.txtAdditionalArguments.Text,
                CustomXmlOutputPath = this.txtCustomXmlOutputPath.Text,
                TreatInconclusiveAsFailure = this.chkTreatInconclusiveTestsAsFailure.Checked
            };
        }
    }
}
