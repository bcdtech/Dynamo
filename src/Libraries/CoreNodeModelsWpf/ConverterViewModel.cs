using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CoreNodeModels;
using Dynamo.Controls;
using Dynamo.Core;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.ViewModels;
using DynamoConversions;

namespace CoreNodeModelsWpf
{
    public class ConverterViewModel : NotificationObject
    {
        private readonly DynamoConvert dynamoConvertModel;
        public ICommand ToggleButtonClick { get; set; }
        private readonly NodeViewModel nodeViewModel;
        private readonly NodeModel nodeModel;

        public ConversionMetricUnit SelectedMetricConversion
        {
            get { return dynamoConvertModel.SelectedMetricConversion; }
            set
            {
                dynamoConvertModel.SelectedMetricConversion = value;                                
            }
        }

        public ConversionUnit SelectedFromConversion
        {
            get { return dynamoConvertModel.SelectedFromConversion; }
            set
            {
                dynamoConvertModel.SelectedFromConversion = value;                             
            }
        }

        public ConversionUnit SelectedToConversion
        {
            get { return dynamoConvertModel.SelectedToConversion; }
            set
            {
                dynamoConvertModel.SelectedToConversion = value;                            
            }
        }

        public List<ConversionUnit> SelectedFromConversionSource
        {
            get { return dynamoConvertModel.SelectedFromConversionSource; }
            set
            {
                dynamoConvertModel.SelectedFromConversionSource = value;               
            }
        }

        public List<ConversionUnit> SelectedToConversionSource
        {
            get { return dynamoConvertModel.SelectedToConversionSource; }
            set
            {
                dynamoConvertModel.SelectedToConversionSource = value;             
            }
        }

        public bool IsSelectionFromBoxEnabled
        {
            get { return dynamoConvertModel.IsSelectionFromBoxEnabled; }
            set
            {
                dynamoConvertModel.IsSelectionFromBoxEnabled = value;                
            }
        }

        public string SelectionFromBoxToolTip
        {
            get { return dynamoConvertModel.SelectionFromBoxToolTip; }
            set
            {
                dynamoConvertModel.SelectionFromBoxToolTip = value;                
            }
        }

        public ConverterViewModel(DynamoConvert model,NodeView nodeView)
        {
            dynamoConvertModel = model;           
            nodeViewModel = nodeView.ViewModel;
            nodeModel = nodeView.ViewModel.NodeModel;
            model.PropertyChanged +=model_PropertyChanged;
            ToggleButtonClick = new RelayCommand<object>(OnToggleButtonClick, CanToggleButton);         
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedMetricConversion":
                    OnPropertyChanged("SelectedMetricConversion");
                    break;
                case "SelectedFromConversionSource":
                    OnPropertyChanged("SelectedFromConversionSource");
                    break;
                case "SelectedToConversionSource":
                    OnPropertyChanged("SelectedToConversionSource");
                    break;
                case "SelectedFromConversion":
                    OnPropertyChanged("SelectedFromConversion");
                    break;
                case "SelectedToConversion":                    
                    OnPropertyChanged("SelectedToConversion");
                    break;
                case "IsSelectionFromBoxEnabled":
                    OnPropertyChanged("IsSelectionFromBoxEnabled");
                    break;
                case "SelectionFromBoxToolTip":
                    OnPropertyChanged("SelectionFromBoxToolTip");
                    break;
            }
        }

        /// <summary>
        /// Called when Toggle button is clicked.
        /// Switches the combo box values
        /// </summary>
        /// <param name="obj">The sender.</param>
        private void OnToggleButtonClick(object obj)
        {
            var undoRecorder = nodeViewModel.WorkspaceViewModel.Model.UndoRecorder;
            WorkspaceModel.RecordModelForModification(nodeModel, undoRecorder);   
            dynamoConvertModel.ToggleDropdownValues();
            nodeViewModel.WorkspaceViewModel.HasUnsavedChanges = true;             
        }

        private bool CanToggleButton(object obj)
        {
            return true;
        }
    }
}
