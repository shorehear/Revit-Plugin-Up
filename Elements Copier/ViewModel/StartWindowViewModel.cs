using System.Windows.Input;
using System.ComponentModel;
using System;

using Autodesk.Revit.UI;

namespace Elements_Copier
{
    enum TypeOfOperation
    {
        SingleSelection = 1,
        GroupSelection = 2
    }
    enum OptionsOfOperation
    {
        NeedRotate = 1,
        NeedUnification = 2,
        NeedRotateAndUnification = 3
    }
    public class StartWindowViewModel : INotifyPropertyChanged
    {
        private TypeOfOperation typeOfOperation = 0;
        private OptionsOfOperation optionsOfOperation = 0;

        public ICommand SingleSelectionCommand { get; }
        public ICommand GroupSelectionCommand { get; }
        public ICommand GetSelectionCommand { get; }

        public event EventHandler RequestClose;

        public StartWindowViewModel() 
        {
            
            SingleSelectionCommand = new RelayCommand(SingleSelection);
            GroupSelectionCommand = new RelayCommand(GroupSelection);
            GetSelectionCommand = new RelayCommand(GetSelection);
        }

        private void SingleSelection(object parameter)
        {
            typeOfOperation = TypeOfOperation.SingleSelection;
        }
        private void GroupSelection(object parameter)
        {
            typeOfOperation = TypeOfOperation.GroupSelection;
        }

        private bool needRotate;
        public bool NeedRotate
        {
            get { return needRotate; }
            set
            {
                if (needRotate != value)
                {
                    needRotate = value;
                    OnPropertyChanged(nameof(NeedRotate));
                    UpdateOptionsOfOperation();
                }
            }
        }

        private bool selectedAndCopiedElements;
        public bool SelectedAndCopiedElements
        {
            get { return selectedAndCopiedElements; }
            set
            {
                if (selectedAndCopiedElements != value)
                {
                    selectedAndCopiedElements = value;
                    OnPropertyChanged(nameof(SelectedAndCopiedElements));
                    UpdateOptionsOfOperation();
                }
            }
        }

        private void UpdateOptionsOfOperation()
        {
            if (NeedRotate && SelectedAndCopiedElements)
            {
                optionsOfOperation = OptionsOfOperation.NeedRotateAndUnification;
            }
            else if (NeedRotate)
            {
                optionsOfOperation = OptionsOfOperation.NeedRotate;
            }
            else if (SelectedAndCopiedElements)
            {
                optionsOfOperation = OptionsOfOperation.NeedUnification;
            }
            else
            {
                optionsOfOperation = 0;
            }
            OnPropertyChanged(nameof(optionsOfOperation));
        }
                
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetSelection(object parameter)
        {
            if (typeOfOperation == 0)
            {
                TaskDialog.Show("Ошибка", "Не была выбрана операция");
                return;
            }

            TaskDialog.Show("Данные перехода", $"Тип операции: {GetTypeOfOperation()}\nТип опции: {GetOptionsOfOperation()}");
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public int GetTypeOfOperation()
        {
            switch (typeOfOperation)
            {
                case TypeOfOperation.SingleSelection:
                    return 1;
                case TypeOfOperation.GroupSelection:
                    return 2;
                default:
                    return 0;
            }
        }

        public int GetOptionsOfOperation()
        {
            switch (optionsOfOperation)
            {
                case OptionsOfOperation.NeedRotate:
                    return 1;
                case OptionsOfOperation.NeedUnification:
                    return 2;
                case OptionsOfOperation.NeedRotateAndUnification:
                    return 3;
                default:
                    return 0;
            }
        }


    }
}
