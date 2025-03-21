using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Dynamo.Configuration;

namespace Dynamo.UI.Controls
{
    /// <summary>
    /// This is a class designed to be used as a tool-tip for Library View, 
    /// Input/Output ports, and Node Caption. It replaces the default look 
    /// of the system tool-tip where it has a triangular side that points 
    /// to the corresponding "target" element. This tool-tip also aligns itself 
    /// to the center of its target, both vertically and horizontally depending 
    /// on its attachment side.
    /// </summary>
    /// 
    public class DynamoToolTip : ToolTip
    {
        public static readonly DependencyProperty AttachmentSideProperty =
            DependencyProperty.Register("AttachmentSide",
            typeof(Side), typeof(DynamoToolTip),
            new PropertyMetadata(Side.Left));

        public enum Side
        {
            Left, Top, Right, Bottom
        }

        public DynamoToolTip()
        {
            Placement = PlacementMode.Custom;
            CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PlacementCallback);
        }

        private CustomPopupPlacement[] PlacementCallback(Size popup, Size target, Point offset)
        {
            double x = 0, y = 0;
            double gap = Configurations.ToolTipTargetGapInPixels;
            PopupPrimaryAxis primaryAxis = PopupPrimaryAxis.None;

            switch (AttachmentSide)
            {
                case Side.Left:
                    x = -(popup.Width + gap);
                    y = (target.Height - popup.Height) * 0.5;
                    primaryAxis = PopupPrimaryAxis.Horizontal;
                    break;

                case Side.Right:
                    x = target.Width + gap;
                    y = (target.Height - popup.Height) * 0.5;
                    primaryAxis = PopupPrimaryAxis.Horizontal;
                    break;

                case Side.Top:
                    x = (target.Width - popup.Width) * 0.5;
                    y = -(popup.Height + gap);
                    primaryAxis = PopupPrimaryAxis.Vertical;
                    break;

                case Side.Bottom:
                    x = (target.Width - popup.Width) * 0.5;
                    y = target.Height + gap;
                    primaryAxis = PopupPrimaryAxis.Vertical;
                    break;
            }

            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement()
                {
                    Point = new Point(x, y),
                    PrimaryAxis = primaryAxis
                }
            };
        }

        public Side AttachmentSide
        {
            get { return ((Side)GetValue(AttachmentSideProperty)); }
            set { SetValue(AttachmentSideProperty, value); }
        }
    }
}
