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
        private CopiedElementsData copiedElementsData;

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

        public XYZ CoordinatesOfCopies { get; set; }
        public int AmountOfCopies { get; set; }
        public double DistanceBetweenCopies { get; set; }

        public ICommand EndSetCopySettingsCommand { get; }

        public CopiedElementsViewModel(CopiedElementsData copiedElementsData, Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.copiedElementsData = copiedElementsData;

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
            //обработать заданные эелементы
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
