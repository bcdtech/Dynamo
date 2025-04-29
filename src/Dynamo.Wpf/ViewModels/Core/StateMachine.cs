using Dynamo.Graph;
using Dynamo.Graph.Annotations;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Notes;
using Dynamo.Models;
using Dynamo.Selection;
using Dynamo.Utilities;
using Dynamo.Wpf.Utilities;
using Dynamo.Wpf.Extensions;
using System.Windows;
using System.Windows.Input;
using DynCmd = Dynamo.Models.DynamoModel;
using ModifierKeys = System.Windows.Input.ModifierKeys;
using Point = System.Windows.Point;

namespace Dynamo.ViewModels
{
    partial class WorkspaceViewModel
    {
        /// <summary>
        /// The StateMachine class manages states in the WorkspaceViewModel it 
        /// belongs. The class is made nested private class because there are 
        /// things that we don't expose beyond WorkspaceViewModel object, but 
        /// should still be readily accessible by the StateMachine class.
        /// </summary>
        internal class StateMachine
        {
            #region Private Class Data Members

            private enum State
            {
                None,
                WindowSelection,
                DragSetup,
                NodeReposition,
                Connection,
                PanMode,
                OrbitMode
            }

            private bool ignoreMouseClick = false;
            private State currentState = State.None;
            private Point mouseDownPos = new Point();
            private WorkspaceViewModel owningWorkspace = null;

            #endregion

            #region Public Class Properties

            internal bool IsInIdleState
            {
                get { return this.currentState == State.None; }
            }

            internal bool IsSelecting
            {
                get { return this.currentState == State.WindowSelection; }
            }

            internal bool IsDragging
            {
                get
                {
                    return this.currentState == State.DragSetup ||
                        this.currentState == State.NodeReposition;
                }
            }

            internal bool IsConnecting
            {
                get { return this.currentState == State.Connection; }
            }

            internal bool IsPanning
            {
                get { return this.currentState == State.PanMode; }
            }

            internal bool IsOrbiting
            {
                get { return this.currentState == State.OrbitMode; }
            }

            private State CurrentState
            {
                get { return this.currentState; }
            }

            #endregion

            #region Public Class Operational Methods

            internal StateMachine(WorkspaceViewModel owningWorkspace)
            {
                this.owningWorkspace = owningWorkspace;
            }

            /// <summary>
            /// The owning WorkspaceView calls this to cancel the current state
            /// </summary>
            internal void CancelActiveState()
            {
                SetCurrentState(State.None);
            }

            /// <summary>
            /// The owning WorkspaceViewModel calls this method in an attempt to
            /// place the StateMachine into view panning mode. Note that as a 
            /// result of calling this method, the StateMachine may be kicked
            /// out of its existing state.
            /// </summary>
            internal void RequestTogglePanMode()
            {
                // In pan mode, left mouse click shall not be handled
                if (currentState == State.PanMode)
                    SetCurrentState(State.None);

                else // no matter in which state, goes to PanMode directly
                    SetCurrentState(State.PanMode);
            }

            /// <summary>
            /// The owning WorkspaceViewModel calls this method in an attempt to
            /// place the StateMachine into view orbiting mode. Note that as a 
            /// result of calling this method, the StateMachine may be kicked
            /// out of its existing state.
            /// </summary>
            internal void RequestToggleOrbitMode()
            {
                if (currentState == State.OrbitMode)
                    SetCurrentState(State.None);
                else
                    SetCurrentState(State.OrbitMode);
            }

            private void SetCurrentState(State newState)
            {
                if (newState == this.currentState)
                    return; // No state changes

                // Exiting from current state
                if (State.None != this.currentState)
                {
                    switch (this.currentState)
                    {
                        case State.WindowSelection:
                            CancelWindowSelection();
                            break;
                        case State.Connection:
                            CancelConnection();
                            break;
                    }
                }

                // Entering into a new state
                CursorSet cursorToUse = CursorSet.Pointer;

                switch (newState)
                {
                    case State.WindowSelection:
                        cursorToUse = CursorSet.RectangularSelection;
                        owningWorkspace.IsCursorForced = true;
                        break;

                    case State.Connection:
                        cursorToUse = CursorSet.ArcAdding;
                        owningWorkspace.IsCursorForced = true;
                        break;

                    case State.PanMode:
                        cursorToUse = CursorSet.HandPan;
                        owningWorkspace.IsCursorForced = true;
                        break;

                    case State.OrbitMode:
                        cursorToUse = CursorSet.HandPan;
                        owningWorkspace.IsCursorForced = true;
                        break;

                    case State.None:
                        cursorToUse = CursorSet.Pointer;
                        owningWorkspace.IsCursorForced = false;
                        break;
                }

                var previousState = this.currentState;
                owningWorkspace.CurrentCursor = CursorLibrary.GetCursor(cursorToUse);
                this.currentState = newState; // update state

                if (previousState == State.PanMode || newState == State.PanMode)
                {
                    owningWorkspace.OnPropertyChanged(nameof(IsPanning));
                }
                if (previousState == State.OrbitMode || newState == State.OrbitMode)
                {
                    owningWorkspace.OnPropertyChanged(nameof(IsOrbiting));
                }
            }

            #endregion

            #region User Input Event Handlers

            internal bool HandleLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                if (ignoreMouseClick)
                {
                    ignoreMouseClick = false;
                    return false;
                }
                var originalDataContext = (e.OriginalSource as FrameworkElement)?.DataContext;
                if (originalDataContext is OutPortViewModel or InPortViewModel)
                {
                    return true;
                }
                var eventHandled = false;
                var returnFocus = true;
                if (currentState == State.Connection)
                {
                    // Clicking on the canvas while connecting simply cancels 
                    // the operation and drop the temporary connector.
                    SetCurrentState(State.None);

                    eventHandled = true; // Mouse event handled.
                }
                else if (currentState == State.None)
                {
                    // Record the mouse down position.
                    var element = sender as IInputElement;
                    mouseDownPos = e.GetPosition(element);

                    // Check if there is any Dynamo Element (e.g. node, note, group) being clicked on. If so, 
                    // then the state machine should initiate a drag operation if user keeps dragging the mouse .
                    if (null != GetSelectableFromPoint(mouseDownPos))
                    {
                        InitiateDragSequence();
                        returnFocus = ShouldDraggingReturnFocus();
                    }
                    else
                    {
                        if (e.ClickCount < 2)
                        {
                            // if the Shift key is held down while the left mouse button is pressed, then do not initiate the window selection sequence
                            DynamoSelection.Instance.ClearSelectionDisabled = Keyboard.Modifiers == ModifierKeys.Shift;
                            InitiateWindowSelectionSequence();

                        }
                        else // Double-clicking on canvas.
                        {
                            // Double-clicking on the background grid results in 
                            // a code block node being created, in which case we
                            // should keep the input focus on the code block to 
                            // avoid it being dismissed (with empty content).
                            //
                            // If Shift/Ctrl is pressed, CBN shouldn't be created.
                            // Shift Modifier indicates, that user tries to call InCanvasSearch
                            // by using Shift + DoubleClick.
                            // Ctrl Modifier indicates, that user tries to copy node.
                            if (Keyboard.Modifiers != ModifierKeys.Shift && Keyboard.Modifiers != ModifierKeys.Control)
                            {
                                CreateCodeBlockNode(mouseDownPos);
                            }

                            returnFocus = false;
                        }
                    }

                    eventHandled = true; // Mouse event handled.
                }
                else if (currentState == State.PanMode || currentState == State.OrbitMode)
                {
                    var c = CursorLibrary.GetCursor(CursorSet.HandPanActive);
                    owningWorkspace.CurrentCursor = c;
                }

                if (returnFocus)
                {
                    owningWorkspace.DynamoViewModel.OnRequestReturnFocusToView();
                }

                return eventHandled;
            }

            internal bool HandleMouseRelease(object sender, MouseButtonEventArgs e)
            {
                if (e.ChangedButton != MouseButton.Left)
                    return false; // We only handle left mouse button for now.

                if (this.currentState == State.WindowSelection)
                {
                    SetCurrentState(State.None);
                    return true; // Mouse event handled.
                }
                else if (this.currentState == State.NodeReposition)
                {
                    Point mouseCursor = e.GetPosition(sender as IInputElement);
                    var operation = DynCmd.DragSelectionCommand.Operation.EndDrag;
                    var command = new DynCmd.DragSelectionCommand(mouseCursor.AsDynamoType(), operation);

                    owningWorkspace.DynamoViewModel.ExecuteCommand(command);

                    // Pin hovered Note to Node
                    var dropNode = owningWorkspace.Nodes
                        .Where(x => x.NodeHoveringState)
                        .FirstOrDefault();

                    if (dropNode != null)
                    {
                        // Only allow one note to be pinned
                        var draggedNote = DynamoSelection.Instance.Selection
                            .OfType<NoteModel>()
                            .First();

                        DynamoSelection.Instance.ClearSelection();
                        DynamoSelection.Instance.Selection.AddUnique(dropNode.NodeModel);

                        var note = owningWorkspace.Notes.First(n => n.Model.GUID.Equals(draggedNote.GUID));
                        note.PinToNodeCommand.Execute(null);

                        // Clean up
                        dropNode.NodeHoveringState = false;
                        DynamoSelection.Instance.ClearSelection();
                    }

                    // When mouse is released get any group with NodeHoveringState
                    // set to true (this should only ever be one),
                    // and add all ModelBase items in selection to that group
                    var dropGroup = owningWorkspace.Annotations
                        .Where(x => x.NodeHoveringState)
                        .FirstOrDefault();

                    if (dropGroup != null)
                    {
                        // If groups are being dragged store them here
                        var dragedGroups = DynamoSelection.Instance.Selection
                            .OfType<AnnotationModel>()
                            .ToList();

                        // We do not want to add dragged groups content twice
                        // so we filter it out here.
                        var modelsToAdd = DynamoSelection.Instance.Selection
                            .OfType<ModelBase>()
                            .Except(dragedGroups.SelectMany(x => x.Nodes))
                            .ToList();

                        // AddModelsToGroupModelCommand adds models to the selected group
                        // therefore we add the dropGroup to the selection before calling
                        // the command.
                        DynamoSelection.Instance.Selection.AddUnique(dropGroup.AnnotationModel);

                        // If the dropgroup has nested groups they will be marked as 
                        // IsSelected when adding the dropgroup to the selection.
                        // We need to unselect all nested groups before we try and add
                        // the dragged node this group, if we dont do this we might
                        // add the node to the wrong group.
                        if (dropGroup.AnnotationModel.HasNestedGroups)
                        {
                            dropGroup.Nodes
                                .OfType<AnnotationModel>()
                                .ToList()
                                .ForEach(x => x.Deselect());
                        }

                        foreach (var item in modelsToAdd)
                        {
                            if (item == dropGroup.AnnotationModel) continue;

                            // If the item is a group and the hovered group
                            // is not a nested group, we add it to the dropGroup
                            if (item is AnnotationModel &&
                                !owningWorkspace.Model.Annotations.ContainsModel(dropGroup.AnnotationModel))
                            {
                                owningWorkspace.DynamoViewModel.AddGroupToGroupModelCommand.Execute(dropGroup.AnnotationModel.GUID);
                                continue;
                            }

                            owningWorkspace.DynamoViewModel.AddModelsToGroupModelCommand.Execute(null);
                        }
                        dropGroup.NodeHoveringState = false;
                        //select only those models which were added to the group
                        DynamoSelection.Instance.ClearSelection();
                        DynamoSelection.Instance.Selection.AddRange(modelsToAdd);
                    }

                    SetCurrentState(State.None); // Dragging operation ended.
                }
                else if (this.currentState == State.DragSetup)
                    SetCurrentState(State.None);
                else if (this.currentState == State.PanMode)
                {
                    // Change cursor back to Pan
                    var c = CursorLibrary.GetCursor(CursorSet.HandPan);
                    owningWorkspace.CurrentCursor = c;
                }
                else if (this.currentState == State.OrbitMode)
                {
                    var c = CursorLibrary.GetCursor(CursorSet.HandPan);
                    owningWorkspace.CurrentCursor = c;
                }

                return false; // Mouse event not handled.
            }

            internal bool HandleMouseMove(object sender, Point mouseCursor)
            {
                if (this.currentState == State.Connection)
                {
                    // If we are currently connecting and there is an active 
                    // connector, redraw it to match the new mouse coordinates.
                    owningWorkspace.UpdateActiveConnector(mouseCursor);
                }
                else if (this.currentState == State.WindowSelection)
                {
                    // When the mouse is held down, reposition the drag selection box.
                    double x = Math.Min(mouseDownPos.X, mouseCursor.X);
                    double y = Math.Min(mouseDownPos.Y, mouseCursor.Y);
                    double width = Math.Abs(mouseDownPos.X - mouseCursor.X);
                    double height = Math.Abs(mouseCursor.Y - mouseDownPos.Y);

                    // We perform cross selection (i.e. select a node whenever 
                    // it touches the selection box as opposed to only select 
                    // it when it is entirely within the selection box) when 
                    // mouse moves in the opposite direction (i.e. the current 
                    // mouse position is smaller than the point mouse-down 
                    // happened).
                    // 
                    bool isCrossSelection = mouseCursor.X < mouseDownPos.X;

                    SelectionBoxUpdateArgs args = null;
                    args = new SelectionBoxUpdateArgs(x, y, width, height);
                    args.SetSelectionMode(isCrossSelection);
                    this.owningWorkspace.RequestSelectionBoxUpdate(this, args);

                    var rect = new Dynamo.Utilities.Rect2D(x, y, width, height);

                    var command = new DynCmd.SelectInRegionCommand(rect, isCrossSelection);

                    owningWorkspace.DynamoViewModel.ExecuteCommand(command);

                }
                else if (this.currentState == State.DragSetup)
                {
                    // There are something in the selection, but none is ILocatable.
                    if (!DynamoSelection.Instance.Selection.Any((x) => (x is ILocatable)))
                    {
                        SetCurrentState(State.None);
                        return false;
                    }

                    if (Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        // Record and begin the drag operation for selected nodes.
                        var operation = DynCmd.DragSelectionCommand.Operation.BeginDrag;
                        var command = new DynCmd.DragSelectionCommand(
                            mouseCursor.AsDynamoType(),
                            operation);
                        owningWorkspace.DynamoViewModel.ExecuteCommand(command);

                        SetCurrentState(State.NodeReposition);
                        return true;
                    }

                }
                else if (this.currentState == State.NodeReposition)
                {
                    // Update the dragged nodes (note: this isn't recorded).
                    owningWorkspace.UpdateDraggedSelection(mouseCursor.AsDynamoType());

                    // Perform drag&drop for notes
                    // terminate early if this was successful 
                    if (PerformDropNotes(mouseCursor)) return false;

                    var draggedGroups = DynamoSelection.Instance.Selection.OfType<AnnotationModel>();

                    // Terminate early if a Note is being hovered over a Node
                    if (owningWorkspace.Nodes.Any(x => x.NodeHoveringState)) return false;

                    // Here we check if the mouse cursor is inside any Annotation groups
                    var dropGroups = owningWorkspace.Annotations
                        .Where(x =>
                        !draggedGroups.Select(a => a.GUID).Contains(x.AnnotationModel.GUID) &&
                        x.IsExpanded &&
                        x.AnnotationModel.Rect.Contains(mouseCursor.X, mouseCursor.Y));

                    // In scenarios where there are nested groups, the above will return both
                    // the nested group and the parent group, as the mouse coursor will be inside
                    // both of there rects. In these cases we want to get group that is nested
                    // inside the parent group.
                    var dropGroup = dropGroups
                        .FirstOrDefault(x => !x.AnnotationModel.HasNestedGroups) ?? dropGroups.FirstOrDefault();


                    // If the dropGroup is null or any of the selected items is already in the dropGroup,
                    // we disable the drop border by setting NodeHoveringState to false
                    var draggedModels = DynamoSelection.Instance.Selection
                        .OfType<ModelBase>()
                        .Except(draggedGroups.SelectMany(x => x.Nodes));

                    if (dropGroup is null ||
                        draggedModels
                        .Any(x => owningWorkspace.Model.Annotations.ContainsModel(x)))
                    {
                        owningWorkspace.Annotations
                            .Where(x => x.NodeHoveringState)
                            .ToList()
                            .ForEach(x => x.NodeHoveringState = false);
                    }

                    // If we are dragging groups over a group that is already nested
                    // we return as we cant have more than one nested layer
                    else if (draggedGroups.Any() &&
                        owningWorkspace.Model.Annotations.ContainsModel(dropGroup.AnnotationModel) ||
                        draggedGroups.Any(x => x.HasNestedGroups))
                    {
                        return false; // Mouse event not handled.
                    }

                    // If the dropGroups NodeHoveringState is set to false
                    // we need to set it to true for the drop border to be displayed.
                    else if (!dropGroup.NodeHoveringState)
                    {
                        // make sure there are no other group
                        // set to NodeHoveringState before setting
                        // the current group.
                        // If we dont do this there are scenarios where
                        // two groups are very close and a node is dragged
                        // quickly between the two where the hovering state
                        // is not reset.
                        owningWorkspace.Annotations
                            .Where(x => x.NodeHoveringState)
                            .ToList()
                            .ForEach(x => x.NodeHoveringState = false);

                        // If the dropGroup belongs to another group
                        // we need to check if the parent group is collapsed
                        // if it is we dont want to be able to add new
                        // models to the drop group.
                        var parentGroup = owningWorkspace.Annotations
                            .Where(x => x.AnnotationModel.ContainsModel(dropGroup.AnnotationModel))
                            .FirstOrDefault();
                        if (parentGroup != null && !parentGroup.IsExpanded) return false;

                        dropGroup.NodeHoveringState = true;
                    }

                }

                return false; // Mouse event not handled.
            }

            /// <summary>
            /// Handles the drag &amp; drop for Notes over Nodes
            /// </summary>
            /// <param name="mouseCursor">The current location of the mouse cursor</param>
            /// <returns></returns>
            private bool PerformDropNotes(Point mouseCursor)
            {
                // If the selected element is not a Note
                // we don't need to keep going
                var draggedNotes = DynamoSelection.Instance.Selection.OfType<NoteModel>();
                if (!draggedNotes.Any())
                {
                    return false;
                }

                var draggedNodes = DynamoSelection.Instance.Selection.OfType<NodeModel>();

                // Here we check if the mouse cursor is inside any Nodes
                var dropNodes = owningWorkspace.Nodes
                    .Where(x =>
                    !draggedNodes.Select(a => a.GUID).Contains(x.NodeModel.GUID) &&
                    x.NodeModel.Rect.Contains(mouseCursor.X, mouseCursor.Y));

                // Only select one Node, in case they are overlapping
                var dropNode = dropNodes.FirstOrDefault();

                if (dropNode is null)
                {
                    // Reset the workspace from hovered nodes
                    owningWorkspace.Nodes
                        .Where(x => x.NodeHoveringState)
                        .ToList()
                        .ForEach(x => x.NodeHoveringState = false);
                }
                else if (!dropNode.NodeHoveringState)
                {
                    // also skip nodes that already have pinned note to them
                    var nodeAlreadyPinned = owningWorkspace.Notes
                        .Where(n => n.PinnedNode != null)
                        .Any(n => n.PinnedNode.NodeModel.GUID == dropNode.NodeModel.GUID);

                    if (nodeAlreadyPinned) return false;

                    // make sure there are no other node
                    // set to NodeHoveringState before setting
                    // the current node.
                    // If we dont do this there are scenarios where
                    // two nodes are very close and a note is dragged
                    // quickly between the two where the hovering state
                    // is not reset.
                    owningWorkspace.Nodes
                        .Where(x => x.NodeHoveringState)
                        .ToList()
                        .ForEach(x => x.NodeHoveringState = false);

                    // also make sure no Annotation groups are marked as hovered
                    // in case the Note is being dropped over a Node inside a Group
                    owningWorkspace.Annotations
                        .Where(x => x.NodeHoveringState)
                        .ToList()
                        .ForEach(x => x.NodeHoveringState = false);

                    dropNode.NodeHoveringState = true;

                    return true;   // Mouse event has not been handled
                }

                return false;
            }

            internal bool HandleMouseMove(object sender, MouseEventArgs e)
            {
                IInputElement element = sender as IInputElement;
                Point mouseCursor = e.GetPosition(element);
                return HandleMouseMove(sender, mouseCursor);
            }

            internal bool HandleFocusChanged(object sender, bool focused)
            {
                CancelActiveState();
                return true; // Handled.
            }

            internal bool HandlePortClicked(PortViewModel portViewModel)
            {
                // We only entertain port clicking when the current state is idle, 
                // or when it is already having a connector being established.
                if (currentState != State.None && (currentState != State.Connection))
                    return false;

                var portModel = portViewModel.PortModel;

                // When the connect command is triggered, set portDisconnectedByConnectCommand flag based on the port connectors.
                // If the current port has any connectors, then it will be disconnected. Otherwise a new connection will be made. 
                portViewModel.inputPortDisconnectedByConnectCommand = portViewModel.PortType == PortType.Input && portModel.Connectors.Count > 0;

                var workspaceViewModel = owningWorkspace.DynamoViewModel.CurrentSpaceViewModel;

                if (this.currentState != State.Connection) // Not in a connection attempt...
                {
                    Guid nodeId = portModel.Owner.GUID;
                    int portIndex = portModel.Index;

                    var mode = DynamoModel.MakeConnectionCommand.Mode.Begin;
                    if (Keyboard.Modifiers == ModifierKeys.Shift) //if shift key is down, handle multiple wire reconnections
                    {
                        multipleConnections = true;
                        mode = DynamoModel.MakeConnectionCommand.Mode.BeginShiftReconnections;
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        // If the control key is held down, check if there is a need to duplicate connections
                        mode = DynamoModel.MakeConnectionCommand.Mode.BeginDuplicateConnection;
                    }

                    var command = new DynamoModel.MakeConnectionCommand(nodeId, portIndex, portModel.PortType, mode);
                    owningWorkspace.DynamoViewModel.ExecuteCommand(command);

                    if (null != owningWorkspace.FirstActiveConnector)
                    {
                        this.currentState = State.Connection;
                        owningWorkspace.CurrentCursor = CursorLibrary.GetCursor(CursorSet.ArcSelect);
                        owningWorkspace.IsCursorForced = false;
                    }
                }
                else  // Attempt to complete the connection
                {
                    // Check if connection is valid
                    if (owningWorkspace.CheckActiveConnectorCompatibility(portViewModel))
                    {
                        Guid nodeId = portModel.Owner.GUID;
                        int portIndex = portModel.Index;

                        var mode = DynamoModel.MakeConnectionCommand.Mode.End;
                        this.currentState = State.None;
                        owningWorkspace.CurrentCursor = null;

                        if (multipleConnections)
                        {
                            mode = DynamoModel.MakeConnectionCommand.Mode.EndShiftReconnections;
                        }
                        else if (Keyboard.Modifiers == ModifierKeys.Control) // If the control key is held down
                        {
                            mode = DynamoModel.MakeConnectionCommand.Mode.BeginCreateConnections;
                            this.currentState = State.Connection; // Start a new connection
                            owningWorkspace.CurrentCursor = CursorLibrary.GetCursor(CursorSet.ArcSelect); // Reassign the cursor
                        }
                        var command = new DynamoModel.MakeConnectionCommand(nodeId,
                            portIndex, portModel.PortType, mode);
                        owningWorkspace.DynamoViewModel.ExecuteCommand(command);
                        multipleConnections = false;
                        owningWorkspace.IsCursorForced = false;
                    }
                }

                return true;
            }

            internal void PasteSelection(Point2D targetPoint)
            {
                var model = owningWorkspace.DynamoViewModel.Model;
                var oldClipboardData = model.ClipBoard.ToList();

                model.Copy();
                // Prevents Paste from being called when only ConnectorPins are selected.
                if (!model.ClipBoard.All(m => m is ConnectorPinModel))
                {
                    model.Paste(targetPoint, false);
                    owningWorkspace.DynamoViewModel.UndoCommand.RaiseCanExecuteChanged();
                    owningWorkspace.DynamoViewModel.RedoCommand.RaiseCanExecuteChanged();
                }

                model.ClipBoard.Clear();
                model.ClipBoard.AddRange(oldClipboardData);
                SetCurrentState(State.None);
            }

            #endregion

            #region Cancel State Methods

            private void CancelConnection()
            {
                var command = new DynCmd.MakeConnectionCommand(Guid.Empty, -1,
                        PortType.Input, DynCmd.MakeConnectionCommand.Mode.Cancel);

                owningWorkspace.DynamoViewModel.ExecuteCommand(command);
            }

            private void CancelWindowSelection()
            {
                if (owningWorkspace != null)
                {
                    // visualization unpause
                    owningWorkspace.OnDragSelectionEnded(this, EventArgs.Empty);
                    SelectionBoxUpdateArgs args = new SelectionBoxUpdateArgs(Visibility.Collapsed);
                    this.owningWorkspace.RequestSelectionBoxUpdate(this, args);
                }
            }

            #endregion

            #region Private Class Helper Method

            private ISelectable GetSelectableFromPoint(Point point)
            {
                foreach (var selectable in DynamoSelection.Instance.Selection)
                {
                    if (!(selectable is ILocatable locatable) || (!locatable.Rect.Contains(point.AsDynamoType())))
                        continue;

                    return selectable;
                }

                return null;
            }

            private void InitiateDragSequence()
            {
                // The state machine must be in idle state.
                if (this.currentState != State.None)
                    throw new InvalidOperationException();

                // Before setting the drag state on node or note,
                // Alt + left click triggers removal of group node or note belongs to
                if (Keyboard.IsKeyDown(Key.LeftAlt) && !DynamoSelection.Instance.Selection.OfType<AnnotationModel>().Any())
                {
                    foreach (var model in DynamoSelection.Instance.Selection.OfType<ModelBase>())
                    {
                        var parentGroup = owningWorkspace.Annotations
                            .Where(x => x.AnnotationModel.ContainsModel(model))
                            .FirstOrDefault();
                        if (parentGroup != null)
                        {
                            owningWorkspace.DynamoViewModel.UngroupModelCommand.Execute(null);
                        }
                    }
                }

                // Before setting the drag state on group
                // Alt + left click triggers removal of group from parent group
                if (Keyboard.IsKeyDown(Key.LeftAlt) && DynamoSelection.Instance.Selection.OfType<AnnotationModel>().Any())
                {
                    var model = DynamoSelection.Instance.Selection.OfType<AnnotationModel>().FirstOrDefault();
                    {
                        var parentGroup = owningWorkspace.Annotations
                           .Where(x => x.AnnotationModel.ContainsModel(model))
                           .FirstOrDefault();
                        if (parentGroup != null)
                        {
                            // Only trigger when parent group exist
                            owningWorkspace.Annotations.Where(x => x.AnnotationModel.GUID == model.GUID).FirstOrDefault().RemoveGroupFromGroupCommand.Execute(null);
                        }
                        else
                        {
                        }
                    }
                }

                SetCurrentState(State.DragSetup);
            }

            private bool ShouldDraggingReturnFocus()
            {
                if (currentState != State.DragSetup)
                    throw new InvalidOperationException();

                // If there is only one node being dragged, and that node is a 
                // code block node, then do not set the focus back to search. 
                // This is to prevent code block editor losing its focus, causing 
                // the newly created code block to be deleted (when user is just 
                // trying to reposition the newly created node before editing its 
                // contents. User reported issue can be found here:
                // 
                //      https://github.com/DynamoDS/Dynamo/issues/1447
                // 
                if (DynamoSelection.Instance.Selection.Count == 1)
                {
                    var selectedNode = DynamoSelection.Instance.Selection[0];
                    if (selectedNode is CodeBlockNodeModel)
                        return false;
                }

                return true; // Return focus to search box.
            }

            private void InitiateWindowSelectionSequence()
            {
                // Visualization pause
                owningWorkspace.OnDragSelectionStarted(this, EventArgs.Empty);

                // The state machine must be in idle state.
                if (this.currentState != State.None)
                    throw new InvalidOperationException();

                // Clear existing selection set.
                var selectNothing = new DynCmd.SelectModelCommand(Guid.Empty, ModifierKeys.None.AsDynamoType());

                owningWorkspace.DynamoViewModel.ExecuteCommand(selectNothing);

                // Update the selection box and make it visible 
                // but with an initial dimension of zero.
                SelectionBoxUpdateArgs args = new SelectionBoxUpdateArgs(mouseDownPos.X, mouseDownPos.Y, 0, 0);
                args.SetVisibility(Visibility.Visible);

                this.owningWorkspace.OnRequestSelectionBoxUpdate(this, args);

                SetCurrentState(State.WindowSelection);
            }

            private void CreateCodeBlockNode(Point cursor)
            {
                // create node
                var node = new CodeBlockNodeModel(owningWorkspace.DynamoViewModel.Model.LibraryServices);

                owningWorkspace.DynamoViewModel.ExecuteCommand(new DynCmd.CreateNodeCommand(node, cursor.X, cursor.Y, false, true));

                //correct node position
                node.X = (int)mouseDownPos.X - 92;
                node.Y = (int)mouseDownPos.Y - 31;
            }

            #endregion
        }

    }
}
