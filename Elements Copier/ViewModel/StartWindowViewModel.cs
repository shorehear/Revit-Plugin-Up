using System.Windows.Input;
using System.ComponentModel;
using System;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace Elements_Copier
{
    enum TypeOfOperation
    {
        SingleSelection,
        GroupSelection
    }
    enum OptionsOfOperation
    {
        NeedRotate,
        NeedUnification,
        NeedRotateAndUnification
    }
    public class StartWindowViewModel : INotifyPropertyChanged
    {
        private TypeOfOperation? typeOfOperation;
        private bool singleSelectionChosen;
        private bool groupSelectionChosen;
        private OptionsOfOperation? optionsOfOperation;

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
            singleSelectionChosen = true;
            typeOfOperation = TypeOfOperation.SingleSelection;
        }
        private void GroupSelection(object parameter)
        {
            groupSelectionChosen = true;
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
                optionsOfOperation = null;
            }
            OnPropertyChanged(nameof(optionsOfOperation));
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
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetSelection(object parameter)
        {
            if (!singleSelectionChosen && !groupSelectionChosen)
            {
                TaskDialog.Show("Ошибка", "Не были указаны параметры выбора объектов");
                return;
            }
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
                    throw new InvalidOperationException("Некорректный выбор операции");
            }
        } //для вызова из селекта

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
                    throw new InvalidOperationException("Некорректный выбор опции операции");
            }
        } //для вызова из копира


    }
}
