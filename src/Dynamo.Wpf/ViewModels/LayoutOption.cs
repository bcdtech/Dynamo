using Dynamo.Controls;
using Dynamo.Utilities;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace Dynamo.ViewModels
{
    public class LayoutOption : ViewModelBase
    {
        private PageSizeType _pageSizeType = PageSizeType.A4;
        public PageSizeType PageSizeType
        {
            get
            {
                return _pageSizeType;
            }
            set
            {
                SetProperty(ref _pageSizeType, value);
                OnPropertyChanged(nameof(PageSize));
            }
        }

        private Size _pageSize = new Size(1000, 600);
        public Size PageSize
        {
            get
            {
                if (PageSizeOrientation == PageSizeOrientation.Vertical)
                {
                    return GetPageSize();
                }
                else
                {
                    return new Size(GetPageSize().Height, GetPageSize().Width);
                }
            }
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    OnPropertyChanged(nameof(PhysicalPageSize));
                }
            }
        }

        public Size PhysicalPageSize
        {
            get
            {
                return new Size(ScreenHelper.WidthToMm(PageSize.Width), ScreenHelper.WidthToMm(PageSize.Height));
            }
            set
            {
                PageSize = new Size(ScreenHelper.MmToWidth(value.Width), ScreenHelper.MmToWidth(value.Height));
            }
        }

        public Size GetPageSize()
        {
            Size size = _pageSize;
            switch (PageSizeType)
            {
                case PageSizeType.A3: size = new Size(ScreenHelper.MmToWidth(297), ScreenHelper.MmToWidth(420)); break;
                case PageSizeType.A4: size = new Size(ScreenHelper.MmToWidth(210), ScreenHelper.MmToWidth(297)); break;
                case PageSizeType.A5: size = new Size(ScreenHelper.MmToWidth(148), ScreenHelper.MmToWidth(210)); break;
                case PageSizeType.B4: size = new Size(ScreenHelper.MmToWidth(257), ScreenHelper.MmToWidth(364)); break;
                case PageSizeType.B5: size = new Size(ScreenHelper.MmToWidth(176), ScreenHelper.MmToWidth(250)); break;
                case PageSizeType.DLEnvelope: size = new Size(ScreenHelper.MmToWidth(110), ScreenHelper.MmToWidth(220)); break;
                case PageSizeType.C5Envelope: size = new Size(ScreenHelper.MmToWidth(162), ScreenHelper.MmToWidth(229)); break;
                case PageSizeType.Quarto: size = new Size(ScreenHelper.MmToWidth(215), ScreenHelper.MmToWidth(275)); break;
                case PageSizeType.C6Quarto: size = new Size(ScreenHelper.MmToWidth(114), ScreenHelper.MmToWidth(162)); break;
                case PageSizeType.B5Quarto: size = new Size(ScreenHelper.MmToWidth(176), ScreenHelper.MmToWidth(250)); break;
                case PageSizeType.ItalyQuarto: size = new Size(ScreenHelper.MmToWidth(110), ScreenHelper.MmToWidth(230)); break;
                case PageSizeType.A4small: size = new Size(ScreenHelper.MmToWidth(210), ScreenHelper.MmToWidth(297)); break;
                case PageSizeType.GermanStdFanfold: size = new Size(ScreenHelper.MmToWidth(215.9), ScreenHelper.MmToWidth(304.8)); break;
                case PageSizeType.GermanLegalFanfold: size = new Size(ScreenHelper.MmToWidth(203.2), ScreenHelper.MmToWidth(330.2)); break;
                case PageSizeType.PRC16K: size = new Size(ScreenHelper.MmToWidth(146), ScreenHelper.MmToWidth(215)); break;
                case PageSizeType.PRC32K: size = new Size(ScreenHelper.MmToWidth(97), ScreenHelper.MmToWidth(151)); break;
                case PageSizeType.Letter: size = new Size(ScreenHelper.MmToWidth(215.9), ScreenHelper.MmToWidth(279.4)); break;
                case PageSizeType.Folio: size = new Size(ScreenHelper.MmToWidth(215.9), ScreenHelper.MmToWidth(330.2)); break;
                case PageSizeType.Legal: size = new Size(ScreenHelper.MmToWidth(215.9), ScreenHelper.MmToWidth(355.6)); break;
                case PageSizeType.Executive: size = new Size(ScreenHelper.MmToWidth(184.15), ScreenHelper.MmToWidth(266.7)); break;
                case PageSizeType.Statement: size = new Size(ScreenHelper.MmToWidth(139.7), ScreenHelper.MmToWidth(215.9)); break;
                case PageSizeType.Envelope: size = new Size(ScreenHelper.MmToWidth(104.77), ScreenHelper.MmToWidth(241.3)); break;
                case PageSizeType.MonarchEnvelope: size = new Size(ScreenHelper.MmToWidth(98.425), ScreenHelper.MmToWidth(190.5)); break;
                case PageSizeType.Tabloid: size = new Size(ScreenHelper.MmToWidth(279.4), ScreenHelper.MmToWidth(431.8)); break;
                case PageSizeType.LetterSmall: size = new Size(ScreenHelper.MmToWidth(215.9), ScreenHelper.MmToWidth(279.4)); break;
                case PageSizeType.CSheet: size = new Size(ScreenHelper.MmToWidth(431.8), ScreenHelper.MmToWidth(558.8)); break;
                case PageSizeType.DSheet: size = new Size(ScreenHelper.MmToWidth(558.8), ScreenHelper.MmToWidth(863.6)); break;
                case PageSizeType.ESheet: size = new Size(ScreenHelper.MmToWidth(863.6), ScreenHelper.MmToWidth(1117.6)); break;
            }

            return new Size(size.Width, size.Height);
        }

        private PageSizeOrientation _pageSizeOrientation;
        public PageSizeOrientation PageSizeOrientation
        {
            get
            {
                return _pageSizeOrientation;
            }
            set
            {
                SetProperty(ref _pageSizeOrientation, value);
                OnPropertyChanged(nameof(PageSize));
            }
        }

        private PageUnit _pageUnit = PageUnit.cm;
        [Browsable(false)]
        public PageUnit PageUnit
        {
            get
            {
                return _pageUnit;
            }
            set
            {
                if (value != PageUnit.cm && value != PageUnit.inch)
                {
                    return;
                }
                SetProperty(ref _pageUnit, value);
            }
        }

        private Size _gridCellSize = new Size(100, 100);
        public Size GridCellSize
        {
            get
            {
                return _gridCellSize;
            }
            set
            {
                SetProperty(ref _gridCellSize, value);
            }
        }

        public double GridCellWidth
        {
            get
            {
                return _gridCellSize.Width;
            }
            set
            {
                _gridCellSize.Width = value;
                OnPropertyChanged(nameof(PhysicalGridCellWidth));
                OnPropertyChanged(nameof(GridCellSize));
            }
        }

        public double GridCellHeight
        {
            get
            {
                return _gridCellSize.Height;
            }
            set
            {
                _gridCellSize.Height = value;
                OnPropertyChanged(nameof(PhysicalGridCellHeight));
                OnPropertyChanged(nameof(GridCellSize));
            }
        }

        public Size PhysicalGridCellSize
        {
            get
            {
                return new Size(ScreenHelper.WidthToMm(GridCellSize.Width), ScreenHelper.WidthToMm(GridCellSize.Height));
            }
            set
            {
                GridCellSize = new Size(ScreenHelper.MmToWidth(value.Width), ScreenHelper.MmToWidth(value.Height));
            }
        }

        public double PhysicalGridCellWidth
        {
            get
            {
                return ScreenHelper.WidthToMm(GridCellWidth);
            }
            set
            {
                GridCellWidth = ScreenHelper.MmToWidth(value);
            }
        }

        public double PhysicalGridCellHeight
        {
            get
            {
                return ScreenHelper.WidthToMm(GridCellHeight);
            }
            set
            {
                GridCellHeight = ScreenHelper.MmToWidth(value);
            }
        }

        private Color _pageBackground = Colors.White;
        public Color PageBackground
        {
            get
            {
                return _pageBackground;
            }
            set
            {
                SetProperty(ref _pageBackground, value);
            }
        }

        private bool _showGrid = true;
        public bool ShowGrid
        {
            get
            {
                return _showGrid;
            }
            set
            {
                SetProperty(ref _showGrid, value);
            }
        }

        private Color _gridColor = Colors.LightGray;
        public Color GridColor
        {
            get
            {
                return _gridColor;
            }
            set
            {
                SetProperty(ref _gridColor, value);
            }
        }

        private Size _gridMarginSize = new Size(28, 28);
        public Size GridMarginSize
        {
            get
            {
                return _gridMarginSize;
            }
            set
            {
                SetProperty(ref _gridMarginSize, value);
            }
        }

        public double GridMarginWidth
        {
            get
            {
                return _gridMarginSize.Width;
            }
            set
            {
                _gridMarginSize.Width = value;
                OnPropertyChanged(nameof(GridMarginSize));
            }
        }

        public double GridMarginHeight
        {
            get
            {
                return _gridMarginSize.Height;
            }
            set
            {
                _gridMarginSize.Height = value;
                OnPropertyChanged(nameof(GridMarginSize));
            }
        }

        public Size PhysicalGridMarginSize
        {
            get
            {
                return new Size(ScreenHelper.WidthToMm(GridMarginSize.Width), ScreenHelper.WidthToMm(GridMarginSize.Height));
            }
            set
            {
                GridMarginSize = new Size(ScreenHelper.MmToWidth(value.Width), ScreenHelper.MmToWidth(value.Height));
            }
        }

        public double PhysicalGridMarginWidth
        {
            get
            {
                return ScreenHelper.WidthToMm(GridMarginWidth);
            }
            set
            {
                GridMarginWidth = ScreenHelper.MmToWidth(value);
            }
        }

        public double PhysicalGridMarginHeight
        {
            get
            {
                return ScreenHelper.WidthToMm(GridMarginHeight);
            }
            set
            {
                GridMarginHeight = ScreenHelper.MmToWidth(value);
            }
        }

        private CellHorizontalAlignment _cellHorizontalAlignment;
        [Browsable(false)]
        public CellHorizontalAlignment CellHorizontalAlignment
        {
            get
            {
                return _cellHorizontalAlignment;
            }
            set
            {
                SetProperty(ref _cellHorizontalAlignment, value);
            }
        }

        private CellVerticalAlignment _cellVerticalAlignment;
        [Browsable(false)]
        public CellVerticalAlignment CellVerticalAlignment
        {
            get
            {
                return _cellVerticalAlignment;
            }
            set
            {
                SetProperty(ref _cellVerticalAlignment, value);
            }
        }

        public bool AllowDrop
        {
            get; set;
        } = true;

        public bool ClipToBounds
        {
            get; set;
        }
        public bool AutoGrowth
        {
            get; set;
        } = true;

        public CornerRadius CornerRadius
        {
            get; set;
        } = new CornerRadius();
    }
}
