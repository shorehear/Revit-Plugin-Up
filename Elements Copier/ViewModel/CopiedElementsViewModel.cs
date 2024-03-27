using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;
using System.Text;
using System.ComponentModel;
using System;
using System.Runtime.CompilerServices;

namespace Elements_Copier
{
    public class CopiedElementsViewModel : INotifyPropertyChanged
    {
        public event EventHandler EndSettings;

        private Document doc;
        private UIDocument uidoc;
        private CopiedElementsData copiedElementsData;
        private string operationsStatus;

        public XYZ CoordinatesOfCopies { get; set; }
        public int AmountOfCopies { get; set; }
        public double DistanceBetweenCopies { get; set; }
                
        public ICommand EndSetCopySettingsCommand { get; }

        public CopiedElementsViewModel(CopiedElementsData copiedElementsData, int optionsOfOperation, Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.copiedElementsData = copiedElementsData;

            operationsStatus = GetTypeOfOperations(optionsOfOperation);
            selectedOptionsText = operationsStatus;

            EndSetCopySettingsCommand = new RelayCommand(EndSetSettings);
            ShowSelectedElementsText();
        }

        private void ShowSelectedElementsText()
        {
            StringBuilder elementsListBuilder = new StringBuilder();
            foreach (var elementId in copiedElementsData.SelectedElements)
            {
                Element element = doc.GetElement(elementId);
                if (element != null)
                {
                    elementsListBuilder.AppendLine($"{element.Name} {element.Id}");
                }
            }
            if (copiedElementsData.SelectedLine != null)
            {
                elementsListBuilder.AppendLine($"\nКоординаты линии копирования:\n{copiedElementsData.SelectedLine.GetEndPoint(0)}, \n{copiedElementsData.SelectedLine.GetEndPoint(1)}.");
            }
            SelectedElementsText = elementsListBuilder.ToString();
        }

        private void EndSetSettings(object parameter)
        {
            string[] coordinates = CoordinatesofCopiesText.Split(',');
            string count = NumberOfCopiesText;
            string distance = distanceBetweenCopiesText;

            TaskDialog.Show("81", $"{count}, {distance}");
            EndSettings?.Invoke(this, EventArgs.Empty);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string GetTypeOfOperations(int optionsOfOperation)
        {
            switch (optionsOfOperation)
            {
                case 0:
                    return "Операция не была выбрана";
                case 1:
                    return "Вращать элементы";
                case 2:
                    return "Выбранные элементы переместить в начало линии вместе с их копиями";
                case 3:
                    return "Вращать копируемые элементы и выбранные элементы переместить в начало линии вместе с их копиями";
                default:
                    return "Ошибка";
            }
        }

        private string selectedElementsText;
        public string SelectedElementsText
        {
            get { return selectedElementsText; }
            set
            {
                if (value != selectedElementsText)
                {
                    selectedElementsText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string coordinatesofCopiesText;
        public string CoordinatesofCopiesText
        {
            get { return coordinatesofCopiesText; }
            set
            {
                if (value != coordinatesofCopiesText)
                {
                    coordinatesofCopiesText = value;
                    OnPropertyChanged(nameof(CoordinatesofCopiesText));
                }
            }
        }

        private string numberOfCopiesText;
        public string NumberOfCopiesText
        {
            get { return numberOfCopiesText; }
            set
            {
                if (value != numberOfCopiesText)
                {
                    numberOfCopiesText = value;
                    OnPropertyChanged(nameof(NumberOfCopiesText));
                }
            }

        }
        private string selectedOptionsText;
        public string SelectedOptionsText
        {
            get { return selectedOptionsText; }
            set
            {
                if (value != selectedOptionsText)
                {
                    selectedOptionsText = value;
                    OnPropertyChanged(nameof(SelectedOptionsText));
                }
            }
        }
        private string distanceBetweenCopiesText;
        public string DistanceBetweenCopiesText
        {
            get { return distanceBetweenCopiesText; }
            set
            {
                distanceBetweenCopiesText = value;
                OnPropertyChanged(nameof(DistanceBetweenCopiesText));
            }
        }
    }
}
