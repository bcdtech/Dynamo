using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    public partial class NodeViewModel
    {
        private ICommand _deleteCommand1;
        private ICommand _setLacingTypeCommand;
        private ICommand _setStateCommand;
        private ICommand _selectCommand;
        private ICommand _showHelpCommand;
        private ICommand _viewCustomNodeWorkspaceCommand;
        private ICommand _validateConnectionsCommand;
        private ICommand _toggleIsVisibleCommand;
        private ICommand _renameCommand;
        private ICommand _setModelSizeCommand;
        private ICommand _gotoWorkspaceCommand;
        private ICommand _createGroupCommand;
        private ICommand _ungroupCommand;
        private ICommand _addToGroupCommand;
        private ICommand _computeRunStateOfTheNodeCommand;
        private ICommand selectConnectedUpstreamCommand;
        private ICommand selectConnectedDownstreamCommand;
        private ICommand selectConnectedDownAndUpstreamCommand;

        [JsonIgnore]
        public ICommand RenameCommand
        {
            get
            {
                if (_renameCommand == null)
                    _renameCommand = new RelayCommand<object>(ShowRename, CanShowRename);
                return _renameCommand;
            }
        }

        [JsonIgnore]
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand1 == null)
                    _deleteCommand1 =
                        new RelayCommand<object>(DeleteNodeAndItsConnectors, CanDeleteNode);

                return _deleteCommand1;
            }
        }

        [JsonIgnore]
        public ICommand SetLacingTypeCommand
        {
            get
            {
                if (_setLacingTypeCommand == null)
                    _setLacingTypeCommand
                        = new RelayCommand<object>(SetLacingType, CanSetLacingType);

                return _setLacingTypeCommand;
            }
        }

        [JsonIgnore]
        public ICommand SetStateCommand
        {
            get
            {
                if (_setStateCommand == null)
                    _setStateCommand =
                        new RelayCommand<object>(SetState, CanSetState);

                return _setStateCommand;
            }
        }

        [JsonIgnore]
        public ICommand SelectCommand
        {
            get
            {
                if (_selectCommand == null)
                    _selectCommand =
                        new RelayCommand<object>(Select, CanSelect);

                return _selectCommand;
            }
        }

        [JsonIgnore]
        public ICommand ShowHelpCommand
        {
            get
            {
                if (_showHelpCommand == null)
                    _showHelpCommand =
                        new RelayCommand<object>(ShowHelp, CanShowHelp);

                return _showHelpCommand;
            }
        }

        [JsonIgnore]
        public ICommand ViewCustomNodeWorkspaceCommand
        {
            get
            {
                if (_viewCustomNodeWorkspaceCommand == null)
                    _viewCustomNodeWorkspaceCommand =
                        new RelayCommand<object>(ViewCustomNodeWorkspace, CanViewCustomNodeWorkspace);

                return _viewCustomNodeWorkspaceCommand;
            }
        }

        [JsonIgnore]
        public ICommand ToggleIsVisibleCommand
        {
            get
            {
                if (_toggleIsVisibleCommand == null)
                    _toggleIsVisibleCommand =
                        new RelayCommand<object>(ToggleIsVisible, CanVisibilityBeToggled);

                return _toggleIsVisibleCommand;
            }
        }

        [JsonIgnore]
        public ICommand SetModelSizeCommand
        {
            get
            {
                if (_setModelSizeCommand == null)
                    _setModelSizeCommand = new RelayCommand<object>(SetModelSize, CanSetModelSize);
                return _setModelSizeCommand;
            }
        }

        [JsonIgnore]
        public ICommand GotoWorkspaceCommand
        {
            get
            {
                if (_gotoWorkspaceCommand == null)
                    _gotoWorkspaceCommand = new RelayCommand<object>(GotoWorkspace, CanGotoWorkspace);
                return _gotoWorkspaceCommand;
            }
        }

        [JsonIgnore]
        public ICommand CreateGroupCommand
        {
            get
            {
                if (_createGroupCommand == null)
                    _createGroupCommand =
                        new RelayCommand<object>(CreateGroup, CanCreateGroup);

                return _createGroupCommand;
            }
        }

        [JsonIgnore]
        public ICommand UngroupCommand
        {
            get
            {
                if (_ungroupCommand == null)
                    _ungroupCommand =
                        new RelayCommand<object>(UngroupNode, CanUngroupNode);

                return _ungroupCommand;
            }
        }

        [JsonIgnore]
        public ICommand AddToGroupCommand
        {
            get
            {
                if (_addToGroupCommand == null)
                    _addToGroupCommand =
                        new RelayCommand<object>(AddToGroup, CanAddToGroup);

                return _addToGroupCommand;
            }
        }

        [JsonIgnore]
        public ICommand ToggleIsFrozenCommand
        {
            get
            {
                if (_computeRunStateOfTheNodeCommand == null)
                {
                    _computeRunStateOfTheNodeCommand = new RelayCommand<object>(ToggleIsFrozen);
                }

                return _computeRunStateOfTheNodeCommand;
            }
        }

        /// <summary>
        /// Command to select connected upstream nodes of the selected nodeModel
        /// </summary>
        [JsonIgnore]
        public ICommand SelectConnectedUpstreamCommand
        {
            get
            {
                if (selectConnectedUpstreamCommand == null)
                {
                    selectConnectedUpstreamCommand = new RelayCommand<object>(SelectUpstreamNeighbours);
                }

                return selectConnectedUpstreamCommand;
            }
        }

        /// <summary>
        /// Command to select connected downstream nodes of the selected nodeModel
        /// </summary>
        [JsonIgnore]
        public ICommand SelectConnectedDownstreamCommand
        {
            get
            {
                if (selectConnectedDownstreamCommand == null)
                {
                    selectConnectedDownstreamCommand = new RelayCommand<object>(SelectDownstreamNeighbours);
                }

                return selectConnectedDownstreamCommand;
            }
        }

        /// <summary>
        /// Command to select connected upstream and downstream nodes of the selected nodeModel
        /// </summary>
        [JsonIgnore]
        public ICommand SelectConnectedUpAndDownstreamCommand
        {
            get
            {
                if (selectConnectedDownAndUpstreamCommand == null)
                {
                    selectConnectedDownAndUpstreamCommand = new RelayCommand<object>(SelectDownstreamAndUpstreamNeighbours);
                }

                return selectConnectedDownAndUpstreamCommand;
            }
        }
    }
}
