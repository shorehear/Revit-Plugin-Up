﻿using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Elements_Copier
{
    public partial class StartWindow : Window
    {
        enum TypeOfOperation
        {
            SingleSelection,
            SingleSelectionWithLine,
            GroupSelection,
            GroupSelectionWithLine
        }

        private Document doc;
        private UIDocument uidoc;

        private readonly StartWindowViewModel _viewModel;
        public StartWindow(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            InitializeComponent();
            _viewModel = new StartWindowViewModel();
            DataContext = _viewModel;
            _viewModel.RequestClose += CloseWindow;
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
            int typeOfOperation = _viewModel.GetTypeOfOperation();
            var SelectionElementsWindow = new SelectionElementsWindow(doc, uidoc, typeOfOperation);
            SelectionElementsWindow.Topmost = true;
            SelectionElementsWindow.Show();
        }
    }
}
