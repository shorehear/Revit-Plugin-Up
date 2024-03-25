using System.Windows.Input;
using System.ComponentModel;
using System;

using Autodesk.Revit.UI;

namespace Elements_Copier
{
    enum TypeOfOperation
    {
        SingleSelection = 1,
        SingleSelectionWithLine = 2,
        GroupSelection = 3,
        GroupSelectionWithLine = 4
    }
    public class StartWindowViewModel : INotifyPropertyChanged
    {
        public ICommand GetSingleSelectionCommand { get; }
        public ICommand GetGroupSelectionCommand { get; }
        private bool singleSelectionChosen = false;
        private bool groupSelectionChosen = false;
        public ICommand GetSelectionCommand { get; }

        public event EventHandler RequestClose;
        private TypeOfOperation typeOfOperation;

        public StartWindowViewModel()
        {
            GetSingleSelectionCommand = new RelayCommand(GetSingleSelection);
            GetGroupSelectionCommand = new RelayCommand(GetGroupSelection);
            GetSelectionCommand = new RelayCommand(GetSelection);
        }

        private bool _includeLineSelection;
        public bool IncludeLineSelection
        {
            get { return _includeLineSelection; }
            set
            {
                if (_includeLineSelection != value)
                {
                    _includeLineSelection = value;
                    OnPropertyChanged(nameof(IncludeLineSelection));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetSingleSelection(object parameter)
        {
            singleSelectionChosen = true;
            typeOfOperation = IncludeLineSelection ? TypeOfOperation.SingleSelectionWithLine : TypeOfOperation.SingleSelection;
        }

        private void GetGroupSelection(object parameter)
        {
            groupSelectionChosen = true;
            typeOfOperation = IncludeLineSelection ? TypeOfOperation.GroupSelectionWithLine : TypeOfOperation.GroupSelection;
        }

        public int GetTypeOfOperation()
        {
            if (typeOfOperation != 0)
            {
                switch (typeOfOperation)
                {
                    case TypeOfOperation.SingleSelection:
                        return 1;
                    case TypeOfOperation.SingleSelectionWithLine:
                        return 2;
                    case TypeOfOperation.GroupSelection:
                        return 3;
                    case TypeOfOperation.GroupSelectionWithLine:
                        return 4;
                    default: return 0;
                }
            }
            else
            {
                throw new InvalidOperationException("Некорректный выбор операции");
            }
        }
        private void GetSelection(object parameter)
        {
            if (!singleSelectionChosen && !groupSelectionChosen)
            {
                TaskDialog.Show("Ошибка", "Не были указаны параметры выбора объектов");
                return;
            }

            TaskDialog.Show("Успешно", "Переходим к окну выбора элементов");
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
