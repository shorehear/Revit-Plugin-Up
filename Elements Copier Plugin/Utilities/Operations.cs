//using Autodesk.Revit.DB;

//namespace Plugin
//{
//    public enum PositionOperations
//    {
//        IHavePoint,
//        IHaveLine,
//        IHaveLineAndNeedRotation
//    }

//    public enum MoveOperations
//    {
//        MoveOnlyCopiedElements,
//        MoveCopiedAndSelecedElements
//    }

//    public static class OperationType
//    {
//        public static (PositionOperations, MoveOperations) GetOperationType()
//        {
//            PositionOperations positionOperations;
//            MoveOperations moveOperations;

//            //if (ElementsData.SelectedPoint != null && !ElementsData.SelectedAndCopiedElements)
//            //{
//            //    positionOperations = PositionOperations.IHavePoint;
//            //    moveOperations = MoveOperations.MoveOnlyCopiedElements;
//            //}
//            //else if (ElementsData.SelectedPoint != null && ElementsData.SelectedAndCopiedElements)
//            //{
//            //    positionOperations = PositionOperations.IHavePoint;
//            //    moveOperations = MoveOperations.MoveCopiedAndSelecedElements;
//            //}
//            //else if (ElementsData.SelectedLine != null && !ElementsData.SelectedAndCopiedElements)
//            //{
//            //    positionOperations = PositionOperations.IHaveLine;
//            //    moveOperations = MoveOperations.MoveOnlyCopiedElements;
//            //}
//            //else if (ElementsData.SelectedLine != null && ElementsData.SelectedAndCopiedElements)
//            //{
//            //    positionOperations = PositionOperations.IHaveLine;
//            //    moveOperations = MoveOperations.MoveCopiedAndSelecedElements;
//            //}
//            //else
//            //{
//            //    TaskDialog.Show("Ошибка", "Некорректный тип операции");
//            //}

//            //return (positionOperations, moveOperations);
//        }
//    }

//}