using Dynamo.Configuration;
using Dynamo.Controls;
using Dynamo.Utilities;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Dynamo.UI.Controls
{
    public class LibraryToolTipPopup : Popup
    {
        private ToolTipWindow tooltip = new ToolTipWindow();
        private readonly LibraryToolTipTimer toolTipTimer;
        private DynamoView mainDynamoWindow;

        public LibraryToolTipPopup()
        {
            this.Placement = PlacementMode.Custom;
            this.AllowsTransparency = true;
            this.CustomPopupPlacementCallback = PlacementCallback;
            this.DataContext = null;
            this.Child = tooltip;
            this.Loaded += LoadMainDynamoWindow;

            toolTipTimer = new LibraryToolTipTimer();
            toolTipTimer.TimerElapsed += (dataContext) =>
            {
                OpenCloseLibraryToolTipPopup(dataContext);
            };
        }

        // We should load main window after Popup has been initialized.
        // If we try to load it before, we will get null.
        private void LoadMainDynamoWindow(object sender, RoutedEventArgs e)
        {
            mainDynamoWindow = WpfUtilities.FindUpVisualTree<DynamoView>(this);
            if (mainDynamoWindow == null)
                return;

            // When Dynamo window goes behind another app, the tool-tip should be hidden right 
            // away. We cannot use CloseLibraryToolTipPopup because it only hides the tool-tip 
            // window after a pause.
            mainDynamoWindow.Deactivated += (Sender, args) =>
            {
                this.DataContext = null;
                IsOpen = false;
                toolTipTimer.Stop();
            };
        }

        public void SetDataContext(object dataContext, bool closeImmediately = false)
        {
            // If Dynamo window is not active, we should not show as well as hide tooltip or do any other staff.
            if (mainDynamoWindow == null || !mainDynamoWindow.IsActive) return;

            if (closeImmediately)
            {
                CloseLibraryToolTipPopup();
                return;
            }

            toolTipTimer.Start(dataContext, 150);
        }

        private void OpenCloseLibraryToolTipPopup(object dataContext)
        {
            IsOpen = dataContext != null || this.IsMouseOver;
            if (dataContext != null)
                this.DataContext = dataContext;

            // This line is needed to change position of Popup.
            // As position changed PlacementCallback is called and
            // Popup placed correctly.            
            HorizontalOffset++;

            // Moving tooltip back.
            HorizontalOffset--;
        }

        private void CloseLibraryToolTipPopup()
        {
            this.DataContext = null;
            IsOpen = false;
            toolTipTimer.Stop();
        }

        private CustomPopupPlacement[] PlacementCallback(Size popup, Size target, Point offset)
        {
            // http://stackoverflow.com/questions/1918877/how-can-i-get-the-dpi-in-wpf
            // MAGN 7397 Library tooltip popup is offset over library items on highres monitors (retina and >96 dpi)
            //Youtrack http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7397
            PresentationSource source = PresentationSource.FromVisual(this);
            double xfactor = 1.0;
            if (source != null)
            {
                xfactor = source.CompositionTarget.TransformToDevice.M11;
            }

            double gap = Configurations.ToolTipTargetGapInPixels;
            var dynamoWindow = WpfUtilities.FindUpVisualTree<DynamoView>(this.PlacementTarget);
            if (dynamoWindow == null)
            {
                SetDataContext(null, true);
                return null;
            }
            Point targetLocation = this.PlacementTarget
                .TransformToAncestor(dynamoWindow)
                .Transform(new Point(0, 0));

            double x = 0;
            // Count width.
            // multiplying by xfactor scales the placement point of the library UI tooltip to the correct location
            //otherwise direct pixel coordinates are off by this factor due to screen dpi.
            var placementTarget = PlacementTarget as FrameworkElement;
            if (placementTarget != null)
            {
                x = (placementTarget.ActualWidth + gap * 2 + targetLocation.X * (-1)) * xfactor;
            }

            // Count height.
            var availableHeight = dynamoWindow.ActualHeight - popup.Height
                - (targetLocation.Y + Configurations.NodeButtonHeight);

            double y = 0;
            if (availableHeight < Configurations.BottomPanelHeight)
                y = availableHeight - (Configurations.BottomPanelHeight + gap * 4);

            return new[]
            {
                new CustomPopupPlacement()
                {
                    Point = new Point(x, y),
                    PrimaryAxis = PopupPrimaryAxis.Horizontal
                }
            };
        }

        private class LibraryToolTipTimer
        {
            private readonly DispatcherTimer dispatcherTimer;

            internal LibraryToolTipTimer()
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += (sender, args) =>
                {
                    // Send the data context to caller.
                    this.OnTimerElapsed(dispatcherTimer.Tag);
                };

            }
            internal void Start(object dataContext, int milliseconds)
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tag = dataContext;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds);
                dispatcherTimer.Start();
            }

            internal void Stop()
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tag = null;
            }

            internal event Action<object> TimerElapsed;
            private void OnTimerElapsed(object dataContext)
            {
                this.Stop();

                var handler = TimerElapsed;
                if (handler != null)
                    handler(dataContext);
            }
        }
    }
}
