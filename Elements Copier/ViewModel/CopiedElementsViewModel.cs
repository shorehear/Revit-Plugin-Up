using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;
using System.Text;

namespace Elements_Copier
{
    public class CopiedElementsViewModel
    {
        private Document doc;
        private UIDocument uidoc;
        public SelectedElementsData SelectedElementsData { get; }
        public CopiedElements copiedElements;
        public ICommand EndSetCopySettingsCommand { get; }

        public CopiedElementsViewModel(SelectedElementsData SelectedElementsData)
        {
            this.SelectedElementsData = SelectedElementsData;
            copiedElements = new CopiedElements(SelectedElementsData);
            doc = SelectedElementsData.doc;
            uidoc = SelectedElementsData.uidoc;

            EndSetCopySettingsCommand = new RelayCommand(EndSetting);
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
        private void EndSetting(object parameter)
        {
            string selectedElementsInfo = GetSelectedElementsString();
            TaskDialog.Show("Выбранные элементы", selectedElementsInfo);

        }
    }


}
