using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementsCopier
{
    public class FailureHandler : IFailuresPreprocessor
    {
        public PluginLogger logger;
        public string errorMessage;

        public FailureHandler(PluginLogger logger)
        {
            this.logger = logger;
        }

        public FailureProcessingResult PreprocessFailures(
          FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor failureMessageAccessor in failureMessages)
            {
                try
                {
                    errorMessage = failureMessageAccessor.GetDescriptionText();
                }
                catch
                {
                    errorMessage = "Неизвестная ошибка";
                }

                FailureSeverity failureSeverity = failureMessageAccessor.GetSeverity();

                if (failureSeverity == FailureSeverity.Warning)
                {
                    logger.LogWarning(errorMessage);
                    failuresAccessor.DeleteWarning(failureMessageAccessor);
                }
                else
                {
                    logger.LogError(errorMessage);
                    return FailureProcessingResult.ProceedWithRollBack;
                }

            }
            return FailureProcessingResult.Continue;
        }
    }
}
