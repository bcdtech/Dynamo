using Dynamo.Graph;
using Dynamo.Graph.Annotations;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Selection;
using Dynamo.Utilities;
using Dynamo.Wpf.Utilities;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    partial class WorkspaceViewModel
    {
        #region events
        public event EventHandler DragSelectionStarted;
        public event EventHandler DragSelectionEnded;
        #endregion

        #region State Machine Related Methods/Data Members

        private readonly StateMachine stateMachine = null;
        private List<DraggedNode> draggedNodes = new List<DraggedNode>();

        // When a new connector is created or a single connector is selected,
        // activeConnectors has array size of 1.
        // In the case of shift + click to reconnect multiple connectors, 
        // all selected connectors are saved in activeConnectors.
        private ConnectorViewModel[] activeConnectors = null;
        private static bool multipleConnections = false;

        // firstStartPort records the first port selected when connections
        // are made using ctrl + click, so that the subsequent new
        // connections are all connected to firstStartPort.
        private PortModel firstStartPort;

        [JsonIgnore]
        // These properties need to be public for data-binding to work.
        public bool IsInIdleState { get { return stateMachine.IsInIdleState; } }

        [JsonIgnore]
        public bool IsSelecting { get { return stateMachine.IsSelecting; } }

        [JsonIgnore]
        public bool IsDragging { get { return stateMachine.IsDragging; } }

        [JsonIgnore]
        public bool IsConnecting { get { return stateMachine.IsConnecting; } }

        [JsonIgnore]
        public bool IsPanning { get { return stateMachine.IsPanning; } }

        [JsonIgnore]
        public bool IsOrbiting { get { return stateMachine.IsOrbiting; } }

        [JsonIgnore]
        internal ConnectorViewModel FirstActiveConnector
        {
            get
            {
                if (null != activeConnectors && activeConnectors.Count() > 0)
                {
                    return activeConnectors[0];
                }
                return null;
            }
        }

        internal bool HandleLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            return stateMachine.HandleLeftButtonDown(sender, e);
        }

        internal bool HandleMouseRelease(object sender, MouseButtonEventArgs e)
        {
            return stateMachine.HandleMouseRelease(sender, e);
        }

        internal bool HandleMouseMove(object sender, MouseEventArgs e)
        {
            return stateMachine.HandleMouseMove(sender, e);
        }

        internal bool HandleMouseMove(object sender, System.Windows.Point mouseCursor)
        {
            return stateMachine.HandleMouseMove(sender, mouseCursor);
        }

        internal bool HandleFocusChanged(object sender, bool focused)
        {
            return stateMachine.HandleFocusChanged(sender, focused);
        }

        internal bool HandlePortClicked(PortViewModel portViewModel)
        {
            return stateMachine.HandlePortClicked(portViewModel);
        }

        internal void RequestTogglePanMode()
        {
            stateMachine.RequestTogglePanMode();
        }

        internal void RequestToggleOrbitMode()
        {
            stateMachine.RequestToggleOrbitMode();
        }

        internal void CancelActiveState()
        {
            stateMachine.CancelActiveState();
        }

        internal void BeginDragSelection(Point2D mouseCursor)
        {
            // This represents the first mouse-move event after the mouse-down
            // event. Note that a mouse-down event can either be followed by a
            // mouse-move event or simply a mouse-up event. That means having 
            // a click does not imply there will be a drag operation. That is 
            // the reason the first mouse-move event is used to signal the 
            // actual drag operation (as oppose to just click-and-release).
            // Here each node in the selection is being recorded for undo right
            // before they get updated by the drag operation.
            // 
            draggedNodes.Clear();
            foreach (ISelectable selectable in DynamoSelection.Instance.Selection)
            {
                ILocatable locatable = selectable as ILocatable;
                if (null == locatable)
                    continue;

                // Annotations always update their position relative to all nested Nodes
                // So there is no need to move the Annotation since it will be updated later anyway (performance improvement)
                if (locatable is AnnotationModel)
                    continue;

                draggedNodes.Add(new DraggedNode(locatable, mouseCursor));
            }

            if (draggedNodes.Count <= 0) // There is nothing to drag.
            {
                throw new InvalidOperationException(Wpf.Properties.Resources.InvalidDraggingOperationMessgae);
            }
        }

        internal void UpdateDraggedSelection(Point2D mouseCursor)
        {
            foreach (DraggedNode draggedNode in draggedNodes)
                draggedNode.Update(mouseCursor);
        }

        internal void EndDragSelection(Point2D mouseCursor)
        {
            RecordSelectionForUndo(mouseCursor);
            UpdateDraggedSelection(mouseCursor); // Final position update.

            draggedNodes.Clear(); // We are no longer dragging anything.
        }

        internal void PasteSelection(Point2D targetPoint)
        {
            stateMachine.PasteSelection(targetPoint);
        }

        internal void BeginConnection(Guid nodeId, int portIndex, PortType portType)
        {
            bool isInPort = portType == PortType.Input;

            if (Model.GetModelInternal(nodeId) is not NodeModel node) return;
            PortModel portModel;
            try
            {
                portModel = isInPort ? node.InPorts[portIndex] : node.OutPorts[portIndex];
            }
            catch (Exception ex)
            {
                this.DynamoViewModel.Model.Logger.Log("Failed to make connection: " + ex.Message);
                return;
            }

            // Test if port already has a connection, if so grab it and begin connecting 
            // to somewhere else (we don't allow the grabbing of the start connector).
            if (portModel.Connectors.Count > 0 && portModel.Connectors[0].Start != portModel)
            {
                // Define the new active connector
                var c = new ConnectorViewModel[] { new ConnectorViewModel(this, portModel.Connectors[0].Start) };
                this.SetActiveConnectors(c);
                firstStartPort = portModel.Connectors[0].Start;
            }
            else
            {
                try
                {
                    // Create an array containing a connector view model to begin drawing
                    var connectors = new ConnectorViewModel[] { new ConnectorViewModel(this, portModel) };
                    this.SetActiveConnectors(connectors);
                    firstStartPort = isInPort ? null : node.OutPorts[portIndex];
                }
                catch (Exception ex)
                {
                    this.DynamoViewModel.Model.Logger.Log(ex.Message);
                }
            }
        }

        internal void BeginShiftReconnections(Guid nodeId, int portIndex, PortType portType)
        {
            if (portType == PortType.Input) return;
            if (!(Model.GetModelInternal(nodeId) is NodeModel node)) return;

            PortModel portModel = node.OutPorts[portIndex];
            if (portModel.Connectors.Count <= 0) return;

            // Try to obtain connectors for selected nodes
            var selected = portModel.Connectors.Where(x => x.End.Owner.IsSelected).Select(y => y.End);

            // If there are no selected nodes, obtain all the associated connectors
            if (selected.Count() <= 0)
            {
                selected = portModel.Connectors.Select(y => y.End);
            }

            var connectorsAr = new ConnectorViewModel[selected.Count()];
            for (int i = 0; i < selected.Count(); i++)
            {
                var c = new ConnectorViewModel(this, selected.ElementAt(i));
                connectorsAr[i] = c;
            }

            this.SetActiveConnectors(connectorsAr);
            return;
        }

        internal void EndConnection(Guid nodeId, int portIndex, PortType portType)
        {
            this.SetActiveConnectors(null);
        }

        internal void BeginCreateConnections(Guid nodeId, int portIndex, PortType portType)
        {
            // Only handle ctrl connections if selected port is an input port
            if (firstStartPort == null || portType == PortType.Output) return;
            this.SetActiveConnectors(null); // End the current connection

            // Then, start a new connection
            if (!(Model.GetModelInternal(nodeId) is NodeModel)) return;
            try
            {
                // Create an array containing a connector view model to begin drawing
                var connectors = new ConnectorViewModel[] { new ConnectorViewModel(this, firstStartPort) };
                this.SetActiveConnectors(connectors);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }
        }

        internal bool CheckActiveConnectorCompatibility(PortViewModel portVM, bool isSnapping = true)
        {
            // Check if required ports exist
            if (FirstActiveConnector == null || portVM == null)
                return false;
            //By default the ports will be in snapping mode. But if the connection is not completed,
            //then on mouse leave, the cursor should be pointed as arcselect instead of arcadd.             
            if (!isSnapping)
            {
                CurrentCursor = CursorLibrary.GetCursor(CursorSet.ArcSelect);
                return false;
            }

            foreach (ConnectorViewModel activeConnector in activeConnectors)
            {
                var srcPortM = activeConnector.ActiveStartPort;
                var desPortM = portVM.PortModel;
                // No self connection
                // No start to start or end or end connection
                if (srcPortM.Owner == desPortM.Owner || srcPortM.PortType == desPortM.PortType)
                {
                    // Change cursor to show not compatible
                    CurrentCursor = CursorLibrary.GetCursor(CursorSet.ArcSelect);
                    return false;
                }
            }
            // If all connections are compatible, change cursor to show compatible port connection
            CurrentCursor = CursorLibrary.GetCursor(CursorSet.ArcAdding);
            return true;
        }

        internal void CancelConnection()
        {
            multipleConnections = false;
            this.SetActiveConnectors(null);
            firstStartPort = null;
        }

        internal void UpdateActiveConnector(System.Windows.Point mouseCursor)
        {
            if (null != this.activeConnectors)
            {
                for (int i = 0; i < this.activeConnectors.Count(); i++)
                {
                    this.activeConnectors[i].Redraw(mouseCursor.AsDynamoType());
                }
            }
        }

        private void SetActiveConnectors(ConnectorViewModel[] connectors) // A replacement for SetActiveConnector(), for handling both single and multiple connectors
        {
            if (null != connectors && connectors.Count() > 0)
            {
                this.activeConnectors = new ConnectorViewModel[connectors.Count()];
                for (int i = 0; i < connectors.Count(); i++)
                {
                    this.WorkspaceElements.Add(connectors[i]);
                    this.activeConnectors[i] = connectors[i];
                }
            }
            else
            {
                if (null != activeConnectors)
                {
                    foreach (ConnectorViewModel a in activeConnectors)
                    {
                        this.WorkspaceElements.Remove(a);
                    }
                }
                this.activeConnectors = null;
            }

            this.OnPropertyChanged("ActiveConnector");
        }

        private void RecordSelectionForUndo(Point2D mousePoint)
        {
            //The models are being stored with its initial position to record only the changed ones.
            List<ModelBase> changedPositionModels = new List<ModelBase>();
            foreach (DraggedNode draggedNode in draggedNodes)
            {
                //Checks if the dragged node has changed its position
                if (draggedNode.HasChangedPosition(mousePoint))
                {
                    ModelBase model = DynamoSelection.Instance.Selection.
                        Where((x) => (x is ModelBase)).Cast<ModelBase>().FirstOrDefault(x => x.GUID == draggedNode.guid);

                    changedPositionModels.Add(model);

                    //The nodes are being reseted to initial position for recording the model purposes 
                    draggedNode.UpdateInitialPosition();
                }
            }

            WorkspaceModel.RecordModelsForModification(changedPositionModels, Model.UndoRecorder);
            DynamoViewModel.RaiseCanExecuteUndoRedo();
        }

        private void OnDragSelectionStarted(object sender, EventArgs e)
        {
            //Debug.WriteLine("Drag started : Visualization paused.");
            DragSelectionStarted?.Invoke(sender, e);
        }

        private void OnDragSelectionEnded(object sender, EventArgs e)
        {
            //Debug.WriteLine("Drag ended : Visualization unpaused.");
            DragSelectionEnded?.Invoke(sender, e);
        }

        #endregion

    }
}
