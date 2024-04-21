using System;

namespace ElementsCopier
{
    public class PluginLogger
    {
        private readonly SelectionElementsViewModel viewModel;
        public PluginLogger(SelectionElementsViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public void Log(string message)
        {
            viewModel.LogText += message + Environment.NewLine;
        }

        public void LogInformation(string message)
        {

            string formattedMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] INFO: {message}";
            Log(formattedMessage);
        }

        public void LogWarning(string message)
        {
            string formattedMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] WARNING: {message}";
            Log(formattedMessage);
        }

        public void LogError(string message)
        {
            string formattedMessage = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] ERROR: {message}";
            Log(formattedMessage);
        }
    }
}