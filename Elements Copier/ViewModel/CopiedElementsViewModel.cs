using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Threading.Tasks;



namespace Elements_Copier
{
    public class CopiedElementsViewModel : INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;
        private CopiedElementsData copiedElementsData;


        private XYZ coordinatesOfCopies;
        private int amountOfCopies;
        private double distanceBetweenCopies;

        

        public ICommand EndSetCopySettingsCommand { get; }

        public CopiedElementsViewModel(CopiedElementsData copiedElementsData, Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.copiedElementsData = copiedElementsData;

            string elementsList = "Элементы:\n";
            foreach (var element in copiedElementsData.SelectedElements)
            {
                elementsList += element.IntegerValue + "\n";
            }
            TaskDialog.Show("COPIED ELEMENTS DATA", elementsList);
            if (copiedElementsData.SelectedLine != null)
            {
                TaskDialog.Show("Статус линии", "Линия успешно выбрана!");
            }

            EndSetCopySettingsCommand = new RelayCommand(EndSetSettings);
            UpdateSelectedElementsTextAsync();
        }

        private string selectedElementsText;
        public string SelectedElementsText
        {
            get { return selectedElementsText; }
            set 
            {
                selectedElementsText = value; 
                OnPropertyChanged(); 
           
            }
        }

        private async void UpdateSelectedElementsTextAsync() // вывод выбранных элементов в окно
        {
            await Task.Delay(10);
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
                elementsListBuilder.AppendLine("\nЛиния направления выбрана");
            }
            SelectedElementsText = elementsListBuilder.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EndSetSettings (object parameter)
        {
            //обработать заданные эелементы
        }
    }
}
