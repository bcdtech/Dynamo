using Dynamo.Graph;
using Dynamo.Utilities;
using System.Diagnostics;

namespace Dynamo.ViewModels
{
    partial class WorkspaceViewModel
    {
        /// <summary>
        /// Each instance of this class represents a node that is being dragged.
        /// It keeps the offset of a node from the mouse cursor when a click 
        /// event occurs, and it updates the node position based on the internal
        /// offset values, and the updated mouse cursor position.
        /// </summary>
        public class DraggedNode
        {
            private readonly double deltaX = 0;
            private readonly double deltaY = 0;
            readonly ILocatable locatable = null;
            internal Guid guid;
            private readonly double initialPositionX = 0;
            private readonly double initialPositionY = 0;

            /// <summary>
            /// Construct a DraggedNode for a given ILocatable object.
            /// </summary>
            /// <param name="locatable">The ILocatable (usually a node) that is 
            /// associated with this DraggedNode object. During an update, the 
            /// position of ILocatable will be updated based on the specified 
            /// mouse position and the internal delta values.</param>
            /// <param name="mouseCursor">The mouse cursor at the point this 
            /// DraggedNode object is constructed. This is used to determine the 
            /// offset of the ILocatable from the mouse cursor.</param>
            /// 
            public DraggedNode(ILocatable locatable, Point2D mouseCursor)
            {
                this.locatable = locatable;
                deltaX = mouseCursor.X - locatable.X;
                deltaY = mouseCursor.Y - locatable.Y;
                initialPositionX = locatable.X;
                initialPositionY = locatable.Y;

                var modelBase = locatable as ModelBase;
                guid = modelBase.GUID;
            }

            public void Update(Point2D mouseCursor)
            {
                // Make sure the nodes do not go beyond the region.
                double x = mouseCursor.X - deltaX;
                double y = mouseCursor.Y - deltaY;
                Debug.WriteLine(mouseCursor);
                Debug.WriteLine($"{x}----------{y}");

                locatable.X = x;
                locatable.Y = y;
                locatable.ReportPosition();
            }

            internal bool HasChangedPosition(Point2D mousePoint)
            {
                //This boolean is for cases when the model has already changed its position before ending the drag action
                bool hasAlreadyChanged = !(initialPositionX == locatable.X && initialPositionY == locatable.Y);

                double x = mousePoint.X - deltaX;
                double y = mousePoint.Y - deltaY;

                //This is  boolean is to check if the model will change its position after recording its properties in the undo stack
                bool willChange = initialPositionX != x || initialPositionY != y;

                return hasAlreadyChanged || willChange;
            }

            public void UpdateInitialPosition()
            {
                locatable.X = initialPositionX;
                locatable.Y = initialPositionY;
            }
        }

    }
}
