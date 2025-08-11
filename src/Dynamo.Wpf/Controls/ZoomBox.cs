using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Dynamo.Utilities;

namespace Dynamo.Controls;
public class ZoomBox : Control
{
    private Thumb zoomThumb;
    private Canvas zoomCanvas;
    private Slider zoomSlider;

    #region DPs

    #region ScrollViewer
    public ScrollViewer ScrollViewer
    {
        get
        {
            return (ScrollViewer)GetValue(ScrollViewerProperty);
        }
        set
        {
            SetValue(ScrollViewerProperty, value);
        }
    }

    public static readonly DependencyProperty ScrollViewerProperty =
        DependencyProperty.Register(nameof(ScrollViewer), typeof(ScrollViewer), typeof(ZoomBox));
    #endregion

    #region FrameworkElement


    public static readonly DependencyProperty DesignerCanvasProperty =
        DependencyProperty.Register(nameof(DesignerCanvas), typeof(FrameworkElement), typeof(ZoomBox),
            new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback(OnDesignerCanvasChanged)));

    public FrameworkElement DesignerCanvas
    {
        get
        {
            return (FrameworkElement)GetValue(DesignerCanvasProperty);
        }
        set
        {
            SetValue(DesignerCanvasProperty, value);
        }
    }

    private static void OnDesignerCanvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox target = (ZoomBox)d;
        FrameworkElement oldDesignerCanvas = (FrameworkElement)e.OldValue;
        FrameworkElement newDesignerCanvas = (FrameworkElement)e.NewValue;
        target.OnDesignerCanvasChanged(oldDesignerCanvas, newDesignerCanvas);
    }

    protected virtual void OnDesignerCanvasChanged(FrameworkElement oldDesignerCanvas, FrameworkElement newDesignerCanvas)
    {
        if (oldDesignerCanvas != null)
        {
            oldDesignerCanvas.LayoutUpdated -= this.DesignerCanvas_LayoutUpdated;
            oldDesignerCanvas.MouseDown -= this.DesignerCanvas_MouseRightButtonDown;
            oldDesignerCanvas.MouseUp -= this.DesignerCanvas_MouseRightButtonUp;
            oldDesignerCanvas.MouseMove -= this.DesignerCanvas_MouseMove;
        }

        if (newDesignerCanvas != null)
        {
            newDesignerCanvas.LayoutUpdated += this.DesignerCanvas_LayoutUpdated;
            newDesignerCanvas.MouseRightButtonDown += this.DesignerCanvas_MouseRightButtonDown;
            newDesignerCanvas.MouseRightButtonUp += this.DesignerCanvas_MouseRightButtonUp;
            newDesignerCanvas.MouseMove += this.DesignerCanvas_MouseMove;
        }
    }
    #endregion

    public static readonly DependencyProperty OffSetProperty =
       DependencyProperty.Register(nameof(OffSet), typeof(bool), typeof(ZoomBox), new UIPropertyMetadata(false));
    public bool OffSet
    {
        get
        {
            return (bool)GetValue(OffSetProperty);
        }
        set
        {
            SetValue(OffSetProperty, value);
        }
    }

    public static readonly DependencyProperty ZoomValueProperty =
        DependencyProperty.Register(nameof(ZoomValue), typeof(double), typeof(ZoomBox), new UIPropertyMetadata(1d));
    public double ZoomValue
    {
        get
        {
            return (double)GetValue(ZoomValueProperty);
        }
        set
        {
            SetValue(ZoomValueProperty, value);
        }
    }

    public static readonly DependencyProperty MaximumZoomValueProperty =
       DependencyProperty.Register(nameof(MaximumZoomValue), typeof(double), typeof(ZoomBox), new UIPropertyMetadata(3d));
    public double MaximumZoomValue
    {
        get
        {
            return (double)GetValue(MaximumZoomValueProperty);
        }
        set
        {
            SetValue(MaximumZoomValueProperty, value);
        }
    }

    public static readonly DependencyProperty MinimumZoomValueProperty =
       DependencyProperty.Register(nameof(MinimumZoomValue), typeof(double), typeof(ZoomBox), new UIPropertyMetadata(0.5d));
    public double MinimumZoomValue
    {
        get
        {
            return (double)GetValue(MinimumZoomValueProperty);
        }
        set
        {
            SetValue(MinimumZoomValueProperty, value);
        }
    }

    public static readonly DependencyProperty FitViewModelProperty =
       DependencyProperty.Register(nameof(FitViewModel), typeof(FitViewModel), typeof(ZoomBox),
           new FrameworkPropertyMetadata(null,
               new PropertyChangedCallback(OnFitViewModelChanged)));

    public FitViewModel FitViewModel
    {
        get
        {
            return (FitViewModel)GetValue(FitViewModelProperty);
        }
        set
        {
            SetValue(FitViewModelProperty, value);
        }
    }

    private static void OnFitViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ZoomBox target = (ZoomBox)d;
        var fitviewmodel = e.NewValue as FitViewModel;
        if (fitviewmodel != null)
        {
            target.OnFitViewModelChanged(fitviewmodel);
        }
    }

    private void OnFitViewModelChanged(FitViewModel fitViewModel)
    {
        if (IsLoaded && fitViewModel != null)
        {
            if (fitViewModel.FitMode == FitMode.None)
            {

            }
            else if (fitViewModel.FitMode == FitMode.FitWidth)
            {
                ZoomValue = (this.ScrollViewer.ViewportWidth * fitViewModel.PaddingRate) / fitViewModel.BoundingRect.Width;
            }
            else if (fitViewModel.FitMode == FitMode.FitHeight)
            {
                ZoomValue = (this.ScrollViewer.ViewportHeight * fitViewModel.PaddingRate) / fitViewModel.BoundingRect.Height;
            }
            else if (fitViewModel.FitMode == FitMode.FitAuto)
            {
                ZoomValue = Math.Min(
                    (this.ScrollViewer.ViewportWidth * fitViewModel.PaddingRate) / fitViewModel.BoundingRect.Width,
                    (this.ScrollViewer.ViewportHeight * fitViewModel.PaddingRate) / fitViewModel.BoundingRect.Height
                    );
            }

            double xOffset, yOffset;
            xOffset = fitViewModel.BoundingRect.Left * ZoomValue - (this.ScrollViewer.ViewportWidth - fitViewModel.BoundingRect.Width * ZoomValue) / 2;
            yOffset = fitViewModel.BoundingRect.Top * ZoomValue - (this.ScrollViewer.ViewportHeight - fitViewModel.BoundingRect.Height * ZoomValue) / 2;
            if (OffSet)
            {
                Vector vector = System.Windows.Media.VisualTreeHelper.GetOffset(DesignerCanvas);
                xOffset += vector.X;
                yOffset += vector.Y;
            }
            this.ScrollViewer.ScrollToHorizontalOffset(xOffset);
            this.ScrollViewer.ScrollToVerticalOffset(yOffset);
        }
    }
    #endregion

    public ZoomBox()
    {
        this.Loaded += ZoomBox_Loaded;
    }

    private void ZoomBox_Loaded(object sender, RoutedEventArgs e)
    {
        OnFitViewModelChanged(FitViewModel);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.ScrollViewer = VisualHelper.TryFindParent<ScrollViewer>(this.DesignerCanvas);
        if (this.ScrollViewer == null)
            return;

        this.zoomThumb = Template.FindName("PART_ZoomThumb", this) as Thumb;
        if (this.zoomThumb == null)
            throw new Exception("PART_ZoomThumb template is missing!");

        this.zoomCanvas = Template.FindName("PART_ZoomCanvas", this) as Canvas;
        if (this.zoomCanvas == null)
            throw new Exception("PART_ZoomCanvas template is missing!");

        this.zoomSlider = Template.FindName("PART_ZoomSlider", this) as Slider;
        if (this.zoomSlider == null)
            throw new Exception("PART_ZoomSlider template is missing!");

        this.zoomThumb.DragDelta += new DragDeltaEventHandler(this.Thumb_DragDelta);
        this.zoomSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.ZoomSlider_ValueChanged);


    }

    private static object thisLock = new Object();
    private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        double scale = e.NewValue / e.OldValue;
        double halfViewportHeight = this.ScrollViewer.ViewportHeight / 2;
        double newVerticalOffset = ((this.ScrollViewer.VerticalOffset + halfViewportHeight) * scale - halfViewportHeight);
        double halfViewportWidth = this.ScrollViewer.ViewportWidth / 2;
        double newHorizontalOffset = ((this.ScrollViewer.HorizontalOffset + halfViewportWidth) * scale - halfViewportWidth);
        this.ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
        this.ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        double scale, xOffset, yOffset;
        this.InvalidateScale(out scale, out xOffset, out yOffset);
        this.ScrollViewer.ScrollToHorizontalOffset(this.ScrollViewer.HorizontalOffset + e.HorizontalChange / scale);
        this.ScrollViewer.ScrollToVerticalOffset(this.ScrollViewer.VerticalOffset + e.VerticalChange / scale);
    }

    private void DesignerCanvas_LayoutUpdated(object sender, EventArgs e)
    {
        try
        {

            double scale, xOffset, yOffset;
            this.InvalidateScale(out scale, out xOffset, out yOffset);
            this.zoomThumb.Width = (this.ScrollViewer.ViewportWidth) * scale;
            this.zoomThumb.Height = (this.ScrollViewer.ViewportHeight) * scale;
            Canvas.SetLeft(this.zoomThumb, xOffset + this.ScrollViewer.HorizontalOffset * scale);
            Canvas.SetTop(this.zoomThumb, yOffset + this.ScrollViewer.VerticalOffset * scale);
        }
        catch { }
    }

    /// <summary>
    /// 用于记录鼠标按下的点
    /// </summary>
    private Point _clickPoint = new Point(0, 0);

    private void DesignerCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        _clickPoint = e.GetPosition((FrameworkElement)sender);
        DesignerCanvas.Cursor = Cursors.Hand;
    }

    private void DesignerCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        DesignerCanvas.Cursor = Cursors.Arrow;
    }

    private void DesignerCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.RightButton == MouseButtonState.Pressed)
        {
            FrameworkElement cabSender = (FrameworkElement)sender;
            double x;
            double y;
            Point p = e.MouseDevice.GetPosition(cabSender);

            x = _clickPoint.X - p.X;
            y = _clickPoint.Y - p.Y;

            ScrollViewer?.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + x);
            ScrollViewer?.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + y);
        }
    }

    private void InvalidateScale(out double scale, out double xOffset, out double yOffset)
    {
        if (OffSet)
        {
            Vector vector = System.Windows.Media.VisualTreeHelper.GetOffset(DesignerCanvas);
            double w = DesignerCanvas.ActualWidth + vector.X * 2;
            double h = DesignerCanvas.ActualHeight + vector.Y * 2;

            // zoom canvas size
            double x = this.zoomCanvas.ActualWidth;
            double y = this.zoomCanvas.ActualHeight;
            double scaleX = x / w;
            double scaleY = y / h;
            scale = (scaleX < scaleY) ? scaleX : scaleY;
            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;
        }
        else
        {
            double w = DesignerCanvas.ActualWidth;
            double h = DesignerCanvas.ActualHeight;

            // zoom canvas size
            double x = this.zoomCanvas.ActualWidth;
            double y = this.zoomCanvas.ActualHeight;
            double scaleX = x / w;
            double scaleY = y / h;
            scale = (scaleX < scaleY) ? scaleX : scaleY;
            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;
        }

    }
}

public class FitViewModel
{
    public Rect BoundingRect
    {
        get; set;
    }

    public FitMode FitMode
    {
        get; set;
    }

    public double PaddingRate
    {
        get; set;
    } = 0.9;
}

public enum FitMode
{
    None,
    FitAuto,
    FitWidth,
    FitHeight,
}

