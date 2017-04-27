using System.ComponentModel;
using Inedo.BuildMaster.Extensibility.Configurers.Extension;
using Inedo.Serialization;

[assembly: ExtensionConfigurer(typeof(Inedo.BuildMasterExtensions.NUnit3.NUnit3Configurer))]

namespace Inedo.BuildMasterExtensions.NUnit3
{
    public sealed class NUnit3Configurer : ExtensionConfigurerBase
    {
        [Persistent]
        [DisplayName("NUnit3 Console Executable Path")]
        [Description(@"The path to nunit3-console.exe")]
        public string NUnit3ConsoleExePath { get; set; }
    }
}
