using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    public partial class PortViewModel
    {
        private ICommand connectCommand;
        private ICommand autoCompleteCommand;
        private ICommand nodeClusterAutoCompleteCommand;
        private ICommand portMouseEnterCommand;
        private ICommand portMouseLeaveCommand;
        private ICommand portMouseLeftButtonCommand;
        private ICommand portMouseLeftButtonOnLevelCommand;
        private ICommand portMouseLeftUseLevelCommand;
        private ICommand nodePortContextMenuCommand;

        public ICommand ConnectCommand
        {
            get
            {
                if (connectCommand == null)
                    connectCommand = new RelayCommand<object>(Connect, CanConnect);

                return connectCommand;
            }
        }

        /// <summary>
        /// Command to trigger Node Auto Complete from node port interaction
        /// </summary>
        public ICommand NodeAutoCompleteCommand
        {
            get
            {
                if (autoCompleteCommand == null)
                    autoCompleteCommand ??= new RelayCommand<object>(NodeViewModel.WorkspaceViewModel.DynamoViewModel.IsDNAClusterPlacementEnabled ? AutoCompleteCluster : AutoComplete, CanAutoComplete);

                return autoCompleteCommand;
            }
        }

        /// <summary>
        /// Command to open an Port's Context Menu popup
        /// </summary>
        public ICommand NodePortContextMenuCommand
        {
            get
            {
                if (nodePortContextMenuCommand == null)
                {
                    nodePortContextMenuCommand = new RelayCommand<object>(NodePortContextMenu);
                }
                return nodePortContextMenuCommand;
            }
        }

        public ICommand MouseEnterCommand
        {
            get
            {
                if (portMouseEnterCommand == null)
                    portMouseEnterCommand = new RelayCommand<object>(OnRectangleMouseEnter, CanConnect);

                return portMouseEnterCommand;
            }
        }

        public ICommand MouseLeaveCommand
        {
            get
            {
                if (portMouseLeaveCommand == null)
                    portMouseLeaveCommand = new RelayCommand<object>(OnRectangleMouseLeave, CanConnect);

                return portMouseLeaveCommand;
            }
        }

        public ICommand MouseLeftButtonDownCommand
        {
            get
            {
                if (portMouseLeftButtonCommand == null)
                    portMouseLeftButtonCommand = new RelayCommand<object>(OnRectangleMouseLeftButtonDown, CanConnect);

                return portMouseLeftButtonCommand;
            }
        }

        public ICommand MouseLeftButtonDownOnLevelCommand
        {
            get
            {
                if (portMouseLeftButtonOnLevelCommand == null)
                    portMouseLeftButtonOnLevelCommand = new RelayCommand<object>(OnMouseLeftButtonDownOnLevel, CanConnect);

                return portMouseLeftButtonOnLevelCommand;
            }
        }

        public ICommand MouseLeftUseLevelCommand
        {
            get
            {
                if (portMouseLeftUseLevelCommand == null)
                    portMouseLeftUseLevelCommand = new RelayCommand<object>(OnMouseLeftUseLevel, CanConnect);

                return portMouseLeftUseLevelCommand;
            }
        }
    }
}
