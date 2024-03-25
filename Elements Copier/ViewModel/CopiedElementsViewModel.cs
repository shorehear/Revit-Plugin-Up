using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Elements_Copier
{
    public class CopiedElementsViewModel : INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;
        public SelectedElementsData SelectedElementsData { get; }
        public CopiedElements copiedElements;
        public ICommand EndSetCopySettingsCommand { get; }

        private string _selectedElementsText;
        public string SelectedElementsText
        {
            get { return _selectedElementsText; }
            set
            {
                if (_selectedElementsText != value)
                {
                    _selectedElementsText = value;
                    OnPropertyChanged();
                }
            }
        }
        public CopiedElementsViewModel(SelectedElementsData SelectedElementsData)
        {
            this.SelectedElementsData = SelectedElementsData;

            copiedElements = new CopiedElements(SelectedElementsData);
            doc = SelectedElementsData.doc;
            uidoc = SelectedElementsData.uidoc;
            EndSetCopySettingsCommand = new RelayCommand(EndSetting);

            SelectedElementsText = GetSelectedElementsString();
        }
        public string GetSelectedElementsString()
        {
            StringBuilder elementsListBuilder = new StringBuilder();
            foreach (var elementId in copiedElements.SelectedElements)
            {
                Element element = doc.GetElement(elementId);
                if (element != null)
                {
                    elementsListBuilder.AppendLine($"{element.Name} {element.Id}");
                }
            }
            if (copiedElements.SelectedLine != null)
            {
                elementsListBuilder.AppendLine("\nЛиния направления выбрана");
            }
            return elementsListBuilder.ToString();
        }
        public string CoordinatesTextBoxText { get; set; }
        public string NumberOfCopiesTextBoxText { get; set; }
        public string DistanceBetweenCopiesTextBoxText { get; set; }

        private void EndSetting(object parameter)
        {
            string[] coordinates = CoordinatesTextBoxText.Split(',');
            XYZ coordinatesPoint = new XYZ(double.Parse(coordinates[0]), double.Parse(coordinates[1]), double.Parse(coordinates[2]));
            copiedElements.CoordinatesToCopy = coordinatesPoint;
            int numberOfCopies = int.Parse(NumberOfCopiesTextBoxText);
            copiedElements.AmountOfCopies = numberOfCopies;
            double distanceBetweenCopies = double.Parse(DistanceBetweenCopiesTextBoxText);
            copiedElements.DistanceBetweenElements = distanceBetweenCopies;
            string selectedElementsInfo = GetSelectedElementsString();
            TaskDialog.Show("Выбранные элементы", selectedElementsInfo);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
