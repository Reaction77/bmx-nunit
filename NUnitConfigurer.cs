using System.ComponentModel;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Configurers.Extension;

[assembly: ExtensionConfigurer(typeof(Inedo.BuildMasterExtensions.NUnit.NUnitConfigurer))]

namespace Inedo.BuildMasterExtensions.NUnit
{
    public sealed class NUnitConfigurer : ExtensionConfigurerBase
    {
        /// <summary>
        /// Gets or sets the path to nunit-console.exe.
        /// </summary>
        [Persistent]
        [DisplayName("NUnit Console Executable Path")]
        [Description(@"The path to nunit-console.exe, typically: <br /><br />"
            + @"""C:\Program Files (x86)\NUnit 2.X.X\bin\nunit-console.exe")]
        public string NUnitConsoleExePath { get; set; }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
