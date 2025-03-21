using CommunityToolkit.Mvvm.Input;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Selection;
using Dynamo.Utilities;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    public partial class WorkspaceViewModel
    {
        #region Private Delegate Command Data Members

        private ICommand hideCommand;
        private ICommand setCurrentOffsetCommand;
        private ICommand nodeFromSelectionCommand;
        private ICommand setZoomCommand;
        private ICommand resetFitViewToggleCommand;
        private ICommand findByIdCommand;
        private ICommand focusNodeCommand;
        private ICommand alignSelectedCommand;
        private ICommand setArgumentLacingCommand;
        private ICommand findNodesFromSelectionCommand;
        private ICommand selectAllCommand;
        private ICommand graphAutoLayoutCommand;
        private ICommand showHideAllGeometryPreviewCommand;
        private ICommand showInCanvasSearchCommand;
        private ICommand pasteCommand;
        private ICommand hideAllPopupCommand;
        private ICommand showAllWiresCommand;
        private ICommand hideAllWiresCommand;
        private ICommand unpinAllPreviewBubblesCommand;

        #endregion

        #region Public Delegate Commands

        [JsonIgnore]
        public ICommand CopyCommand
        {
            get { return DynamoViewModel.CopyCommand; }
        }

        [JsonIgnore]
        public ICommand PasteCommand
        {
            get { return pasteCommand ?? (pasteCommand = new RelayCommand<object>(Paste, DynamoViewModel.CanPaste)); }
        }

        [JsonIgnore]
        public ICommand SelectAllCommand
        {
            get
            {
                if (selectAllCommand == null)
                    selectAllCommand = new RelayCommand<object>(SelectAll, CanSelectAll);
                return selectAllCommand;
            }
        }

        [JsonIgnore]
        public ICommand GraphAutoLayoutCommand
        {
            get
            {
                return graphAutoLayoutCommand
                    ?? (graphAutoLayoutCommand =
                        new RelayCommand<object>(DoGraphAutoLayout, CanDoGraphAutoLayout));
            }
        }

        private ICommand _nodeToCodeCommand;
        [JsonIgnore]
        public ICommand NodeToCodeCommand
        {
            get
            {
                if (_nodeToCodeCommand == null)
                {
                    _nodeToCodeCommand = new RelayCommand<object>(NodeToCode, CanNodeToCode);
                }
                return _nodeToCodeCommand;
            }
        }

        [JsonIgnore]
        public ICommand HideCommand
        {
            get
            {
                if (hideCommand == null)
                    hideCommand = new RelayCommand<object>(Hide, CanHide);

                return hideCommand;
            }
        }

        [JsonIgnore]
        public ICommand SetCurrentOffsetCommand
        {
            get
            {
                if (setCurrentOffsetCommand == null)
                    setCurrentOffsetCommand = new RelayCommand<object>(SetCurrentOffset, CanSetCurrentOffset);

                return setCurrentOffsetCommand;
            }
        }

        [JsonIgnore]
        public ICommand NodeFromSelectionCommand
        {
            get
            {
                if (nodeFromSelectionCommand == null)
                    nodeFromSelectionCommand = new RelayCommand<object>(CreateNodeFromSelection, CanCreateNodeFromSelection);

                return nodeFromSelectionCommand;
            }
        }

        [JsonIgnore]
        public ICommand SetZoomCommand
        {
            get
            {
                if (setZoomCommand == null)
                    setZoomCommand = new RelayCommand<object>(SetZoom, CanSetZoom);
                return setZoomCommand;
            }
        }

        [JsonIgnore]
        public ICommand ResetFitViewToggleCommand
        {
            get
            {
                if (resetFitViewToggleCommand == null)
                    resetFitViewToggleCommand = new RelayCommand<object>(ResetFitViewToggle, CanResetFitViewToggle);
                return resetFitViewToggleCommand;
            }
        }

        [JsonIgnore]
        public ICommand FindByIdCommand
        {
            get
            {
                if (findByIdCommand == null)
                    findByIdCommand = new RelayCommand<object>(FindById, CanFindById);

                return findByIdCommand;
            }
        }

        [JsonIgnore]
        public ICommand FocusNodeCommand
        {
            get
            {
                if (focusNodeCommand == null)
                    focusNodeCommand = new RelayCommand<object>(FocusNode, CanFocusNode);

                return focusNodeCommand;
            }
        }

        [JsonIgnore]
        public ICommand AlignSelectedCommand
        {
            get
            {
                if (alignSelectedCommand == null)
                    alignSelectedCommand = new RelayCommand<object>(AlignSelected, CanAlignSelected);

                return alignSelectedCommand;
            }
        }

        [JsonIgnore]
        public ICommand SetArgumentLacingCommand
        {
            get
            {
                if (setArgumentLacingCommand == null)
                {
                    setArgumentLacingCommand = new RelayCommand<object>(
                        SetArgumentLacing, p => HasSelection);
                }

                return setArgumentLacingCommand;
            }
        }

        [JsonIgnore]
        public ICommand FindNodesFromSelectionCommand
        {
            get
            {
                if (findNodesFromSelectionCommand == null)
                    findNodesFromSelectionCommand = new RelayCommand<object>(FindNodesFromSelection, CanFindNodesFromSelection);

                return findNodesFromSelectionCommand;
            }
        }

        [JsonIgnore]
        public ICommand ShowHideAllGeometryPreviewCommand
        {
            get
            {
                if (showHideAllGeometryPreviewCommand == null)
                {
                    showHideAllGeometryPreviewCommand = new RelayCommand<object>(
                        ShowHideAllGeometryPreview);
                }

                return showHideAllGeometryPreviewCommand;
            }
        }


        [JsonIgnore]
        public ICommand ShowInCanvasSearchCommand
        {
            get
            {
                if (showInCanvasSearchCommand == null)
                    showInCanvasSearchCommand = new RelayCommand<object>(OnRequestShowInCanvasSearch);

                return showInCanvasSearchCommand;
            }
        }

        /// <summary>
        /// View Command to hide all popup in special cases
        /// </summary>
        [JsonIgnore]
        public ICommand HideAllPopupCommand
        {
            get
            {
                if (hideAllPopupCommand == null)
                    hideAllPopupCommand = new RelayCommand<object>(OnRequestHideAllPopup);

                return hideAllPopupCommand;
            }
        }

        /// <summary>
        /// View Command to show all connection wires (on current selection), if any are hidden
        /// </summary>
        [JsonIgnore]
        public ICommand ShowAllWiresCommand
        {
            get
            {
                if (showAllWiresCommand == null)
                    showAllWiresCommand = new RelayCommand<object>(ShowAllWires, CanShowAllWires);

                return showAllWiresCommand;
            }
        }

        /// <summary>
        /// View Command to hide all connection wires (on current selection), if any are shown
        /// </summary>
        [JsonIgnore]
        public ICommand HideAllWiresCommand
        {
            get
            {
                if (hideAllWiresCommand == null)
                    hideAllWiresCommand = new RelayCommand<object>(HideAllWires, CanHideAllWires);

                return hideAllWiresCommand;
            }
        }

        /// <summary>
        /// View Command to hide all currently pinned preview bubbles within the workspace
        /// </summary>
        [JsonIgnore]
        public ICommand UnpinAllPreviewBubblesCommand
        {
            get
            {
                if (unpinAllPreviewBubblesCommand == null)
                    unpinAllPreviewBubblesCommand = new RelayCommand<object>(UnpinAllPreviewBubbles, CanUnpinAllPreviewBubbles);
                return unpinAllPreviewBubblesCommand;
            }
        }
        #endregion

        #region Properties for Command Data Binding

        [JsonIgnore]
        public bool CanCopy
        {
            get { return DynamoViewModel.CanCopy(null); }
        }

        [JsonIgnore]
        public bool CanPaste
        {
            get { return DynamoViewModel.CanPaste(null); }
        }

        [JsonIgnore]
        public bool CanCopyOrPaste
        {
            get { return CanCopy || CanPaste; }
        }

        [JsonIgnore]
        public bool AnyNodeVisible
        {
            get
            {
                return DynamoSelection.Instance.Selection.
                    OfType<NodeModel>().Any(n => n.IsVisible);
            }
        }

        [JsonIgnore]
        public bool HasSelection
        {
            get { return DynamoSelection.Instance.Selection.Count > 0; }
        }

        [JsonIgnore]
        public bool CanUpdatePythonEngine => false;

        [JsonIgnore]
        public bool CanUpdateAllPythonEngine
        {
            get { return DynamoViewModel.CanUpdateAllPythonEngine(null); }
        }

        [JsonIgnore]
        public bool IsGeometryOperationEnabled
        {
            get
            {
                if (DynamoSelection.Instance.Selection.Count <= 0)
                    return false; // No selection.

                // Menu options that are specific to geometry (show/hide all 
                // geometry previews, etc.) are only enabled
                // in the home workspace.
                // 
                return (this.Model is HomeWorkspaceModel);
            }
        }

        [JsonIgnore]
        public LacingStrategy SelectionArgumentLacing
        {
            // TODO We may need a better way to do this
            // For now this returns the most common lacing strategy in the collection.
            get
            {
                // We were still hitting this getter when the Selection
                // was empty, and throwing an exception when attempting to
                // sort a null collection. If the Selection is empty, just
                // return First lacing.

                if (!DynamoSelection.Instance.Selection.Any())
                    return LacingStrategy.First;

                return DynamoSelection.Instance.Selection.OfType<NodeModel>()
                    .GroupBy(node => node.ArgumentLacing)
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key).FirstOrDefault();
            }
        }

        #endregion
    }
}
