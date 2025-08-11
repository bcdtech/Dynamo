using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using Dynamo.Utilities;
using System.ComponentModel;

namespace Dynamo.Controls;
public class Ruler : FrameworkElement
{
    #region Fields
    private double SegmentHeight;
    private readonly Pen p = new Pen(Brushes.Black, 1.0);
    private readonly Pen ThinPen = new Pen(Brushes.Black, 0.5);
    private readonly Pen BorderPen = new Pen(Brushes.Gray, 1.0);
    private readonly Pen RedPen = new Pen(Brushes.Red, 2.0);
    #endregion

    #region Properties
    #region Background
    public Brush Background
    {
        get
        {
            return (Brush)GetValue(BackgroundProperty);
        }
        set
        {
            SetValue(BackgroundProperty, value);
        }
    }

    /// <summary>
    /// Identifies the Length dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty =
         DependencyProperty.Register(
              "Background",
              typeof(Brush),
              typeof(Ruler),
              new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Length
    /// <summary>
    /// Gets or sets the length of the ruler. If the <see cref="AutoSize"/> property is set to false (default) this
    /// is a fixed length. Otherwise the length is calculated based on the actual width of the ruler.
    /// </summary>
    public double Length
    {
        get
        {
            if (this.AutoSize)
            {
                return (double)(Unit == Unit.Cm ? ScreenHelper.WidthToCm(this.ActualWidth) : ScreenHelper.WidthToInch(this.ActualWidth)) / this.Zoom;
            }
            else
            {
                return (double)GetValue(LengthProperty);
            }
        }
        set
        {
            SetValue(LengthProperty, value);
        }
    }

    /// <summary>
    /// Identifies the Length dependency property.
    /// </summary>
    public static readonly DependencyProperty LengthProperty =
         DependencyProperty.Register(
              "Length",
              typeof(double),
              typeof(Ruler),
              new FrameworkPropertyMetadata(20D, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region AutoSize
    /// <summary>
    /// Gets or sets the AutoSize behavior of the ruler.
    /// false (default): the lenght of the ruler results from the <see cref="Length"/> property. If the window size is changed, e.g. wider
    ///						than the rulers length, free space is shown at the end of the ruler. No rescaling is done.
    /// true				 : the length of the ruler is always adjusted to its actual width. This ensures that the ruler is shown
    ///						for the actual width of the window.
    /// </summary>
    public bool AutoSize
    {
        get
        {
            return (bool)GetValue(AutoSizeProperty);
        }
        set
        {
            SetValue(AutoSizeProperty, value);
            this.InvalidateVisual();
        }
    }

    /// <summary>
    /// Identifies the AutoSize dependency property.
    /// </summary>
    public static readonly DependencyProperty AutoSizeProperty =
         DependencyProperty.Register(
              "AutoSize",
              typeof(bool),
              typeof(Ruler),
              new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Zoom
    /// <summary>
    /// Gets or sets the zoom factor for the ruler. The default value is 1.0. 
    /// </summary>
    public double Zoom
    {
        get
        {
            return (double)GetValue(ZoomProperty);
        }
        set
        {
            SetValue(ZoomProperty, value);
            this.InvalidateVisual();
        }
    }

    /// <summary>
    /// Identifies the Zoom dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register("Zoom", typeof(double), typeof(Ruler),
        new FrameworkPropertyMetadata((double)1.0,
            FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    #region Chip

    /// <summary>
    /// Chip Dependency Property
    /// </summary>
    public static readonly DependencyProperty ChipProperty =
         DependencyProperty.Register("Chip", typeof(double), typeof(Ruler),
              new FrameworkPropertyMetadata((double)-1000,
                    FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Sets the location of the chip in the units of the ruler.
    /// So, to set the chip to 10 in cm units the chip needs to be set to 10.
    /// Use the <see cref="DipHelper"/> class for conversions.
    /// </summary>
    public double Chip
    {
        get { return (double)GetValue(ChipProperty); }
        set { SetValue(ChipProperty, value); }
    }
    #endregion

    #region CountShift

    /// <summary>
    /// CountShift Dependency Property
    /// </summary>
    public static readonly DependencyProperty CountShiftProperty =
         DependencyProperty.Register("CountShift", typeof(double), typeof(Ruler),
              new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// By default the counting of inches or cm starts at zero, this property allows you to shift
    /// the counting.
    /// </summary>
    public double CountShift
    {
        get { return (double)GetValue(CountShiftProperty); }
        set { SetValue(CountShiftProperty, value); }
    }

    #endregion

    #region Marks

    /// <summary>
    /// Marks Dependency Property
    /// </summary>
    public static readonly DependencyProperty MarksProperty =
         DependencyProperty.Register("Marks", typeof(MarksLocation), typeof(Ruler),
              new FrameworkPropertyMetadata(MarksLocation.Up,
                     FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets where the marks are shown in the ruler.
    /// </summary>
    public MarksLocation Marks
    {
        get { return (MarksLocation)GetValue(MarksProperty); }
        set { SetValue(MarksProperty, value); }
    }

    #endregion

    #region Unit
    /// <summary>
    /// Gets or sets the unit of the ruler.
    /// Default value is Unit.Cm.
    /// </summary>
    public Unit Unit
    {
        get { return (Unit)GetValue(UnitProperty); }
        set { SetValue(UnitProperty, value); }
    }

    /// <summary>
    /// Identifies the Unit dependency property.
    /// </summary>
    public static readonly DependencyProperty UnitProperty =
         DependencyProperty.Register(
              "Unit",
              typeof(Unit),
              typeof(Ruler),
              new FrameworkPropertyMetadata(Unit.Cm, FrameworkPropertyMetadataOptions.AffectsRender));

    #endregion

    #endregion

    #region Constructor
    static Ruler()
    {
        HeightProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(20.0));
    }
    public Ruler()
    {
        SegmentHeight = this.Height - 10;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Participates in rendering operations.
    /// </summary>
    /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var rect = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
        drawingContext.DrawRectangle(Background, null, rect);

        double xDest = (Unit == Unit.Cm ? ScreenHelper.CmToWidth(Length) : ScreenHelper.InchToWidth(Length)) * this.Zoom;
        drawingContext.DrawRectangle(null, BorderPen, new Rect(new Point(0.0, 0.0), new Point(xDest, Height)));
        double chip = Unit == Unit.Cm ? ScreenHelper.CmToWidth(Chip) : ScreenHelper.InchToWidth(Chip);
        drawingContext.DrawLine(RedPen, new Point(chip, 0), new Point(chip, Height));

        //画偏移位置之前的
        if (CountShift < 0)
        {
            for (double dUnit = -CountShift; dUnit > -1; dUnit--)
            {
                double d;
                if (Unit == Unit.Cm)
                {
                    d = ScreenHelper.CmToWidth(dUnit) * this.Zoom;
                    if (dUnit < Length)
                    {
                        for (int i = 1; i <= 9; i++)
                        {
                            if (i != 5)
                            {
                                double dMm = ScreenHelper.CmToWidth(dUnit + 0.1 * i) * this.Zoom;
                                if (Marks == MarksLocation.Up)
                                    drawingContext.DrawLine(ThinPen, new Point(dMm, 0), new Point(dMm, SegmentHeight / 3.0));
                                else
                                    drawingContext.DrawLine(ThinPen, new Point(dMm, Height), new Point(dMm, Height - SegmentHeight / 3.0));
                            }
                        }
                        double dMiddle = ScreenHelper.CmToWidth(dUnit + 0.5) * this.Zoom;
                        if (Marks == MarksLocation.Up)
                            drawingContext.DrawLine(p, new Point(dMiddle, 0), new Point(dMiddle, SegmentHeight * 2.0 / 3.0));
                        else
                            drawingContext.DrawLine(p, new Point(dMiddle, Height), new Point(dMiddle, Height - SegmentHeight * 2.0 / 3.0));
                    }
                }
                else
                {
                    d = ScreenHelper.InchToWidth(dUnit) * this.Zoom;
                    if (dUnit < Length)
                    {
                        if (Marks == MarksLocation.Up)
                        {
                            double dQuarter = ScreenHelper.InchToWidth(dUnit + 0.25) * this.Zoom;
                            drawingContext.DrawLine(ThinPen, new Point(dQuarter, 0),
                                                            new Point(dQuarter, SegmentHeight / 3.0));
                            double dMiddle = ScreenHelper.InchToWidth(dUnit + 0.5) * this.Zoom;
                            drawingContext.DrawLine(p, new Point(dMiddle, 0),
                                                            new Point(dMiddle, SegmentHeight * 2D / 3D));
                            double d3Quarter = ScreenHelper.InchToWidth(dUnit + 0.75) * this.Zoom;
                            drawingContext.DrawLine(ThinPen, new Point(d3Quarter, 0),
                                                            new Point(d3Quarter, SegmentHeight / 3.0));
                        }
                        else
                        {
                            double dQuarter = ScreenHelper.InchToWidth(dUnit + 0.25) * this.Zoom;
                            drawingContext.DrawLine(ThinPen, new Point(dQuarter, Height),
                                                            new Point(dQuarter, Height - SegmentHeight / 3.0));
                            double dMiddle = ScreenHelper.InchToWidth(dUnit + 0.5) * this.Zoom;
                            drawingContext.DrawLine(p, new Point(dMiddle, Height),
                                                            new Point(dMiddle, Height - SegmentHeight * 2D / 3D));
                            double d3Quarter = ScreenHelper.InchToWidth(dUnit + 0.75) * this.Zoom;
                            drawingContext.DrawLine(ThinPen, new Point(d3Quarter, Height),
                                                            new Point(d3Quarter, Height - SegmentHeight / 3.0));
                        }
                    }
                }
                if (Marks == MarksLocation.Up)
                    drawingContext.DrawLine(p, new Point(d, 0), new Point(d, SegmentHeight));
                else
                    drawingContext.DrawLine(p, new Point(d, Height), new Point(d, Height - SegmentHeight));


                if ((dUnit != 0.0) && (dUnit < Length))
                {
                    FormattedText ft = new FormattedText(
                        (Math.Round(dUnit + CountShift, 0)).ToString(CultureInfo.CurrentCulture),
                         CultureInfo.CurrentCulture,
                         FlowDirection.LeftToRight,
                         new Typeface("Arial"),
                         9,
                         Brushes.DimGray);
                    ft.SetFontWeight(FontWeights.Regular);
                    ft.TextAlignment = TextAlignment.Center;

                    if (Marks == MarksLocation.Up)
                        drawingContext.DrawText(ft, new Point(d, Height - ft.Height));
                    else
                        drawingContext.DrawText(ft, new Point(d, Height - SegmentHeight - ft.Height));
                }
            }
        }


        //画偏移位置之后的
        for (double dUnit = -CountShift; dUnit <= Length; dUnit++)
        {
            double d;
            if (Unit == Unit.Cm)
            {
                d = ScreenHelper.CmToWidth(dUnit) * this.Zoom;
                if (dUnit < Length)
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        if (i != 5)
                        {
                            double dMm = ScreenHelper.CmToWidth(dUnit + 0.1 * i) * this.Zoom;
                            if (Marks == MarksLocation.Up)
                                drawingContext.DrawLine(ThinPen, new Point(dMm, 0), new Point(dMm, SegmentHeight / 3.0));
                            else
                                drawingContext.DrawLine(ThinPen, new Point(dMm, Height), new Point(dMm, Height - SegmentHeight / 3.0));
                        }
                    }
                    double dMiddle = ScreenHelper.CmToWidth(dUnit + 0.5) * this.Zoom;
                    if (Marks == MarksLocation.Up)
                        drawingContext.DrawLine(p, new Point(dMiddle, 0), new Point(dMiddle, SegmentHeight * 2.0 / 3.0));
                    else
                        drawingContext.DrawLine(p, new Point(dMiddle, Height), new Point(dMiddle, Height - SegmentHeight * 2.0 / 3.0));
                }
            }
            else
            {
                d = ScreenHelper.InchToWidth(dUnit) * this.Zoom;
                if (dUnit < Length)
                {
                    if (Marks == MarksLocation.Up)
                    {
                        double dQuarter = ScreenHelper.InchToWidth(dUnit + 0.25) * this.Zoom;
                        drawingContext.DrawLine(ThinPen, new Point(dQuarter, 0),
                                                        new Point(dQuarter, SegmentHeight / 3.0));
                        double dMiddle = ScreenHelper.InchToWidth(dUnit + 0.5) * this.Zoom;
                        drawingContext.DrawLine(p, new Point(dMiddle, 0),
                                                        new Point(dMiddle, SegmentHeight * 2D / 3D));
                        double d3Quarter = ScreenHelper.InchToWidth(dUnit + 0.75) * this.Zoom;
                        drawingContext.DrawLine(ThinPen, new Point(d3Quarter, 0),
                                                        new Point(d3Quarter, SegmentHeight / 3.0));
                    }
                    else
                    {
                        double dQuarter = ScreenHelper.InchToWidth(dUnit + 0.25) * this.Zoom;
                        drawingContext.DrawLine(ThinPen, new Point(dQuarter, Height),
                                                        new Point(dQuarter, Height - SegmentHeight / 3.0));
                        double dMiddle = ScreenHelper.InchToWidth(dUnit + 0.5) * this.Zoom;
                        drawingContext.DrawLine(p, new Point(dMiddle, Height),
                                                        new Point(dMiddle, Height - SegmentHeight * 2D / 3D));
                        double d3Quarter = ScreenHelper.InchToWidth(dUnit + 0.75) * this.Zoom;
                        drawingContext.DrawLine(ThinPen, new Point(d3Quarter, Height),
                                                        new Point(d3Quarter, Height - SegmentHeight / 3.0));
                    }
                }
            }
            if (Marks == MarksLocation.Up)
                drawingContext.DrawLine(p, new Point(d, 0), new Point(d, SegmentHeight));
            else
                drawingContext.DrawLine(p, new Point(d, Height), new Point(d, Height - SegmentHeight));


            if ((dUnit != 0.0) && (dUnit < Length))
            {
                FormattedText ft = new FormattedText(
                    (Math.Round(dUnit + CountShift, 0)).ToString(CultureInfo.CurrentCulture),
                     CultureInfo.CurrentCulture,
                     FlowDirection.LeftToRight,
                     new Typeface("Arial"),
                     9,
                     Brushes.DimGray);
                ft.SetFontWeight(FontWeights.Regular);
                ft.TextAlignment = TextAlignment.Center;

                if (Marks == MarksLocation.Up)
                    drawingContext.DrawText(ft, new Point(d, Height - ft.Height));
                else
                    drawingContext.DrawText(ft, new Point(d, Height - SegmentHeight - ft.Height));
            }
        }
    }

    /// <summary>
    /// Measures an instance during the first layout pass prior to arranging it.
    /// </summary>
    /// <param name="availableSize">A maximum Size to not exceed.</param>
    /// <returns>The maximum Size for the instance.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        return base.MeasureOverride(availableSize);
        //Size desiredSize;
        //if (Unit == Unit.Cm)
        //{
        //    desiredSize = new Size(ScreenHelper.CmToWidth(Length), Height);
        //}
        //else
        //{
        //    desiredSize = new Size(ScreenHelper.InchToWidth(Length), Height);
        //}
        //return desiredSize;
    }
    #endregion
}

/// <summary>
/// The unit type of the ruler.
/// </summary>
public enum Unit
{
    /// <summary>
    /// the unit is Centimeter.
    /// </summary>
    Cm,

    /// <summary>
    /// The unit is Inch.
    /// </summary>
    Inch
};

public enum MarksLocation
{
    Up, Down
}
public enum PageUnit
{
    [Description("毫米(暂未实现)")]
    mm,
    [Description("厘米")]
    cm,
    [Description("米(暂未实现)")]
    m,
    [Description("千米(暂未实现)")]
    km,
    [Description("英寸")]
    inch,
    [Description("英尺和英寸(暂未实现)")]
    ftin,
    [Description("英尺(暂未实现)")]
    foot,
    [Description("码(暂未实现)")]
    yard,
    [Description("英里(暂未实现)")]
    mile,
    [Description("点(暂未实现)")]
    tiny,
    [Description("皮卡(暂未实现)")]
    pickup,
    [Description("迪多(暂未实现)")]
    ditto,
    [Description("西塞罗(暂未实现)")]
    cicero,
    [Description("像素(暂未实现)")]
    pixel,
}

