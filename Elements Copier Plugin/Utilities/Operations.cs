using Autodesk.Revit.UI;

namespace Plugin
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

                case "MissingLineAndPoint":
                    TaskDialog.Show("Выберите линию и точку", "Не выбрана линия размещения объектов, не выбрана точка в области, " +
                         "\nотносительно которой совершится копирование. \nПожалуйста, определите недостающие объекты.");
                    return "Для начала копирования ожидается \nвыбор точки и линии.";

                case "MissingLine":
                    TaskDialog.Show("Выберите линию", "Не выбрана линия размещения объектов. \nПожалуйста, определите недостающий объект.");
                    return "Для начала копирования ожидается выбор линии.";

                case "MissingPoint":
                    TaskDialog.Show("Выберите точку", "Не выбрана точка в области, относительно которой совершится копирование. \nПожалуйста, определите недостающие объекты");
                    return "Для начала копирования ожидается выбор точки.";

                default:
                    return string.Empty;
            }
        }
    }

    public enum PositionOperations
    {
        IHavePoint,
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


            if (ElementsData.SelectedPoint != null && !ElementsData.SelectedAndCopiedElements)
            {
                positionOperations = PositionOperations.IHavePoint;
                moveOperations = MoveOperations.MoveOnlyCopiedElements;
            }
            else if (ElementsData.SelectedPoint != null && ElementsData.SelectedAndCopiedElements)
            {
                positionOperations = PositionOperations.IHavePoint;
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