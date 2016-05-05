using System.ComponentModel;
using Inedo.BuildMaster.Extensibility.Configurers.Extension;
using Inedo.Serialization;

[assembly: ExtensionConfigurer(typeof(Inedo.BuildMasterExtensions.NUnit.NUnitConfigurer))]

namespace Inedo.BuildMasterExtensions.NUnit
{
    public sealed class NUnitConfigurer : ExtensionConfigurerBase
    {
        [Persistent]
        [DisplayName("NUnit Console Executable Path")]
        [Description(@"The path to nunit-console.exe, typically: <br /><br />"
            + @"""C:\Program Files (x86)\NUnit 2.X.X\bin\nunit-console.exe")]
        public string NUnitConsoleExePath { get; set; }
    }
}
