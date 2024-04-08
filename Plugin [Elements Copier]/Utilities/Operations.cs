using Autodesk.Revit.UI;

namespace ElementsCopier
{
    public static class StatusType
    {
        public static string GetStatusMessage(string type)
        {
            switch (type)
            {
                case "Default":
                    return "Ожидание выбора области объектов...";

                case "ObjectsSelected":
                    return "Элементы выбраны.";

                case "MissingLineAndCopyPoint":
                    TaskDialog.Show("Линия или точка", "Не выбран объект, относительно которого совершится копирование. Пожалуйста, определите точку копирования или линию.");
                    return "Для начала копирования ожидается \nвыбор точки или линии.";

                case "MissingPoint":
                    TaskDialog.Show("Выберите точку", "Не выбрана точка в области, относительно которой совершится копирование. Пожалуйста, определите недостающие объекты.");
                    return "Для начала копирования ожидается \nвыбор точки.";

                case "SelectPoint":
                    return "Выберите точку в области для копирования.";
                case "SelectedPoint":
                    return "Выбрана точка области. \nЧтобы добавить еще элементов \nнажмите 'Добавить'.";

                case "SelectLine":
                    return "Ожидается выбор линии.";
                case "SelectedLine":
                    return "Выбрана линия копирования.\nЧтобы добавить еще элементов \nнажмите 'Добавить'.";

                case "SelectCopyPoint":
                    return "Ожидается выбор точки копирования.";
                case "SelectedCopyPoint":
                    return "Выбрана точка копирования. \nЧтобы добавить еще элементов \nнажмите 'Добавить'.";

                case "UserCanseledOperation":
                    return "Пользователь отменил операцию.";
                case "UserCanseledSelection":
                    return "Пользователь отменил выбор объектов.\nЧтобы возобновить выбор \nнажмите 'Добавить'.";

                case "ZeroCountElements":
                    return "Не выбрано количество копий элементов. \nПожалуйста, определите \nнедостающий параметр.";
                case "ZeroDistanceBetweenElements":
                    TaskDialog.Show("Предупреждение", "Не задана дистанция между элементами. Если количество копий больше одной, наслоении копий друг на друга возможны ошибки.");
                    return string.Empty;

                default:
                    return string.Empty;
            }
        }
    }

    public enum PositionOperations
    {
        IHaveCopyPoint,
        IHaveLine,
        IHaveLineAndNeedRotation,
        Error
    }

    public enum MoveOperations
    {
        MoveOnlyCopiedElements,
        MoveCopiedAndSelecedElements,
        Error
    }

    public static class OperationType
    {
        public static (PositionOperations, MoveOperations) GetOperationType()
        {
            PositionOperations positionOperations;
            MoveOperations moveOperations;

            if (ElementsData.SelectedPoint == null)
            {
                TaskDialog.Show("Ошибка", "Не выбрана точка области. \nПожалуйста, определите точку, относительно которой необходтимо совершить копирование.");
                positionOperations = PositionOperations.Error;
                moveOperations = MoveOperations.Error;
            }
            else if (ElementsData.SelectedCopyPoint != null && !ElementsData.SelectedAndCopiedElements)
            {
                positionOperations = PositionOperations.IHaveCopyPoint;
                moveOperations = MoveOperations.MoveOnlyCopiedElements;
            }
            else if (ElementsData.SelectedCopyPoint != null && ElementsData.SelectedAndCopiedElements)
            {
                positionOperations = PositionOperations.IHaveCopyPoint;
                moveOperations = MoveOperations.MoveCopiedAndSelecedElements;
            }
            else if (ElementsData.SelectedLine != null && !ElementsData.SelectedAndCopiedElements)
            {
                positionOperations = PositionOperations.IHaveLine;
                moveOperations = MoveOperations.MoveOnlyCopiedElements;
            }
            else if (ElementsData.SelectedLine != null && ElementsData.SelectedAndCopiedElements)
            {
                positionOperations = PositionOperations.IHaveLine;
                moveOperations = MoveOperations.MoveCopiedAndSelecedElements;
            }
            else
            {
                TaskDialog.Show("Ошибка", "Некорректный тип операции");
                positionOperations = PositionOperations.Error;
                moveOperations = MoveOperations.Error;
            }
            return (positionOperations, moveOperations);
        }
    }
}