
namespace Elements_Copier
{
    public class CopiedElementsViewModel
    {
        public SelectedElementsData SelectedElementsData { get; }
        public CopiedElements copiedElements;
        public CopiedElementsViewModel(SelectedElementsData SelectedElementsData)
        {
            this.SelectedElementsData = SelectedElementsData;
            copiedElements = new CopiedElements(SelectedElementsData);
        }
    }
}
