using Inedo.BuildMaster.Extensibility.Operations;
using Inedo.IO;

namespace Inedo.BuildMasterExtensions.NUnit
{
    internal sealed class NUnitActionImporter : IActionOperationConverter<NUnitAppAction, NUnitOperation>
    {
        public ConvertedOperation<NUnitOperation> ConvertActionToOperation(NUnitAppAction action, IActionConverterContext context)
        {
            var configurerPath = (context.Configurer as NUnitConfigurer)?.NUnitConsoleExePath;

            return new NUnitOperation
            {
                TestFile = context.ConvertLegacyExpression(PathEx.Combine(action.OverriddenSourceDirectory ?? string.Empty, action.TestFile)),
                ExePath = context.ConvertLegacyExpression(PathEx.Combine(action.OverriddenSourceDirectory ?? string.Empty, AH.CoalesceString(action.ExePath, configurerPath))),
                AdditionalArguments = AH.NullIf(context.ConvertLegacyExpression(action.AdditionalArguments), string.Empty),
                OutputPath = AH.NullIf(context.ConvertLegacyExpression(PathEx.Combine(action.OverriddenSourceDirectory ?? string.Empty, action.CustomXmlOutputPath)), string.Empty)
            };
        }
    }
}
