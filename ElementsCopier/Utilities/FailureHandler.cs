using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ElementsCopier
{
    public class FailureHandler : IFailuresPreprocessor
    {
        public PluginLogger logger;
        private SelectionElementsViewModel viewModel;
        public string errorMessage;

        public FailureHandler(PluginLogger logger, SelectionElementsViewModel viewModel)
        {
            this.viewModel = viewModel;
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
                else if (errorMessage == "Проем не образует выреза в основе.")
                {
                    logger.LogWarning(errorMessage);
                    viewModel.Status = "Проем не образует выреза в основе.\n Пожалуйста, удалите \n'Вырезание проема' из коллекции.";
                    viewModel.ClearData();
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
