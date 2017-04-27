using Inedo.BuildMaster.Extensibility.Operations;
using Inedo.IO;

namespace Inedo.BuildMasterExtensions.NUnit3
{
    internal sealed class NUnit3ActionImporter : IActionOperationConverter<NUnit3AppAction, NUnit3Operation>
    {
        public ConvertedOperation<NUnit3Operation> ConvertActionToOperation(NUnit3AppAction action, IActionConverterContext context)
        {
            var configurerPath = (context.Configurer as NUnit3Configurer)?.NUnit3ConsoleExePath;

            return new NUnit3Operation
            {
                TestFile = context.ConvertLegacyExpression(PathEx.Combine(action.OverriddenSourceDirectory ?? string.Empty, action.TestFile)),
                ExePath = context.ConvertLegacyExpression(PathEx.Combine(action.OverriddenSourceDirectory ?? string.Empty, AH.CoalesceString(action.ExePath, configurerPath))),
                AdditionalArguments = AH.NullIf(context.ConvertLegacyExpression(action.AdditionalArguments), string.Empty),
                OutputPath = AH.NullIf(context.ConvertLegacyExpression(PathEx.Combine(action.OverriddenSourceDirectory ?? string.Empty, action.CustomXmlOutputPath)), string.Empty)
            };
        }
    }
}
