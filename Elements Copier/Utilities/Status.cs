
namespace ElementsCopier
{
    public static class StatusType
    {
        public static string GetStatusMessage(string type)
        {
            switch (type)
            {
                case "WaitingForSelection":
                    return "Ожидание выбора области объектов...";
                case "GetElements":
                    return "Выбраны элементы. Чтобы добавить\nеще нажмите 'Добавить'.\n\n " +
                        "Чтобы удалить элемент из выбранного\n списка нажмите на строчку с ним.";

                case "SetPoint":
                    return "Выберите точку области.";
                case "GetPoint":
                    return "Выбрана точка области.";

                case "SetLine":
                    return "Выберите линию копирования.";
                case "GetLine":
                    return "Выбрана линия копирования.";

                case "DistanceContains":
                    return "Пожалуйста, введите \nнецелое число через запятую.";

                case "CanselOperation":
                    return "Для добавления элементов \nнажмите 'Добавить'.";

                case "NoElementsSelected":
                    return "Не выбранны элементы для копирования. \nУкажите недостающие параметры.";
                case "NoPointSelected":
                    return "Не выбрана точка области. \nУкажите недостающие параметры.";
                case "NoLineSelected":
                    return "Не выбрана линия копирования. \nУкажите недостающие параметры.";
                case "NoCountCopies":
                    return "Не задано количество копий. \nУкажите недостающие параметры.";

                default:
                    return string.Empty;
            }
        }
    }
}