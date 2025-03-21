using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    public partial class NoteViewModel
    {
        private ICommand _selectCommand;
        [JsonIgnore]
        public ICommand SelectCommand
        {
            get
            {
                if (_selectCommand == null)
                    _selectCommand = new RelayCommand<object>(Select, CanSelect);
                return _selectCommand;
            }
        }

        private ICommand _createGroupCommand;
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

        private ICommand _ungroupCommand;
        [JsonIgnore]
        public ICommand UngroupCommand
        {
            get
            {
                if (_ungroupCommand == null)
                    _ungroupCommand =
                        new RelayCommand<object>(UngroupNote, CanUngroupNote);

                return _ungroupCommand;
            }
        }

        private ICommand _addToGroupCommand;
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

        private ICommand pinToNodeCommand;

        /// <summary>
        /// Command to pin the current note to a selected node
        /// </summary>
        [JsonIgnore]
        public ICommand PinToNodeCommand
        {
            get
            {
                if (pinToNodeCommand == null)
                {
                    pinToNodeCommand = new RelayCommand<object>(PinToNode, CanPinToNode);
                }
                return pinToNodeCommand;
            }
        }

        private ICommand unpinFromNodeCommand;
        /// <summary>
        /// Command to unpin the pinned node (sets it to null)
        /// </summary>
        [JsonIgnore]
        public ICommand UnpinFromNodeCommand
        {
            get
            {
                if (unpinFromNodeCommand == null)
                {
                    unpinFromNodeCommand = new RelayCommand<object>(UnpinFromNode, CanUnpinFromNode);
                }
                return unpinFromNodeCommand;
            }
        }
    }
}
