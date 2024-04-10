using Autodesk.Revit.UI;

namespace ElementsCopier
{
    public enum Operation
    {
        Default,
        NeedRotate,
        WithSourceElements,
        NeedRotateWithSourceElements
    }

    public static class CopySettings
    {
        public static Operation GetCopySettingsType()
        {
            if (!ElementsData.NeedRotate && !ElementsData.WithSourceElements)
            {
                return Operation.Default;
            }
            else if(ElementsData.NeedRotate && !ElementsData.WithSourceElements)
            {
                return Operation.NeedRotate;
            }
            else if(!ElementsData.NeedRotate && ElementsData.WithSourceElements)
            {
                return Operation.WithSourceElements;
            }
            else if(ElementsData.NeedRotate && ElementsData.WithSourceElements)
            {
                return Operation.NeedRotateWithSourceElements;
            }
            else
            {
                TaskDialog.Show("Ошибка", "Не удается определить тип исполняемой операции, будет выполнена операция по умолчанию.");
                return Operation.Default;
            }
        }
    }
}