using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    public partial class InfoBubbleViewModel : ViewModelBase
    {
        private ICommand updateContentCommand;
        private ICommand updatePositionCommand;
        private ICommand resizeCommand;
        private ICommand showFullContentCommand;
        private ICommand showCondensedContentCommand;
        private ICommand changeInfoBubbleStateCommand;
        private ICommand openDocumentationLinkCommand;
        private ICommand dismissMessageCommand;
        private ICommand undismissMessageCommand;

        public ICommand UpdateContentCommand
        {
            get
            {
                if (updateContentCommand == null)
                    updateContentCommand = new RelayCommand<object>(UpdateInfoBubbleContent, CanUpdateInfoBubbleCommand);
                return updateContentCommand;
            }
        }

        public ICommand UpdatePositionCommand
        {
            get
            {
                if (updatePositionCommand == null)
                    updatePositionCommand = new RelayCommand<object>(UpdatePosition, CanUpdatePosition);
                return updatePositionCommand;
            }
        }
        public ICommand ResizeCommand
        {
            get
            {
                if (resizeCommand == null)
                {
                    resizeCommand = new RelayCommand<object>(Resize, CanResize);
                }
                return resizeCommand;
            }
        }

        // TODO add new command handler
        public ICommand ShowFullContentCommand
        {
            get
            {
                if (showFullContentCommand == null)
                {
                    showFullContentCommand = new RelayCommand<object>(ShowFullContent, CanShowFullContent);
                }
                return showFullContentCommand;
            }
        }

        // TODO add new command handler
        public ICommand ShowCondensedContentCommand
        {
            get
            {
                if (showCondensedContentCommand == null)
                {
                    showCondensedContentCommand = new RelayCommand<object>(ShowCondensedContent, CanShowCondensedContent);
                }
                return showCondensedContentCommand;
            }
        }

        public ICommand ChangeInfoBubbleStateCommand
        {
            get
            {
                if (changeInfoBubbleStateCommand == null)
                {
                    changeInfoBubbleStateCommand = new RelayCommand<object>(ChangeInfoBubbleState, CanChangeInfoBubbleState);
                }
                return changeInfoBubbleStateCommand;
            }
        }

        public ICommand OpenDocumentationLinkCommand
        {
            get
            {
                if (openDocumentationLinkCommand == null)
                {
                    openDocumentationLinkCommand = new RelayCommand<object>(OpenDocumentationLink, CanOpenDocumentationLink);
                }
                return openDocumentationLinkCommand;
            }
        }

        /// <summary>
        /// Fires when the user manually dismisses a message by clicking the little 'X' button next to it.
        /// Users can only dismiss Info Messages and Warnings - not Errors.
        /// </summary>
        public ICommand DismissMessageCommand
        {
            get
            {
                if (dismissMessageCommand == null)
                {
                    dismissMessageCommand = new RelayCommand<object>(DismissMessage);
                }
                return dismissMessageCommand;
            }
        }

        /// <summary>
        /// Fires when the user manually selects a previously-dismissed message from the node's ContextMenu.
        /// This un-dismisses the message and causes it to reappear above the node again.
        /// </summary>
        public ICommand UndismissMessageCommand
        {
            get
            {
                if (undismissMessageCommand == null)
                {
                    undismissMessageCommand = new RelayCommand<object>(UndismissMessage);
                }
                return undismissMessageCommand;
            }
        }
    }
}