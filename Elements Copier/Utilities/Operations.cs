using Autodesk.Revit.UI;

namespace ElementsCopier
{
    public enum Operation
    {
        Default,
        WithSourceElements,
    }

    public static class CopySettings
    {
        public static Operation GetCopySettingsType()
        {
            if (!ElementsData.WithSourceElements)
            {
                return Operation.Default;
            }
            else if (ElementsData.WithSourceElements)
            {
                return Operation.WithSourceElements;
            }
            else
            {
                TaskDialog.Show("Ошибка", "Не удается определить тип исполняемой операции, будет выполнена операция по умолчанию.");
                return Operation.Default;
            }
        }
    }
}