using Dynamo.ViewModels;
using Dynamo.Wpf.ViewModels.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NotificationObject = Dynamo.Core.NotificationObject;

namespace Dynamo.UI.Controls
{
    /// <summary>
    /// An object which provides the data for the shortcut toolbar.
    /// </summary>
    public partial class ShortcutToolbar : UserControl, IDisposable
    {
        private readonly ObservableCollection<ShortcutBarItem> shortcutBarItems;
        private readonly ObservableCollection<ShortcutBarItem> shortcutBarRightSideItems;

        /// <summary>
        /// A collection of <see cref="ShortcutBarItem"/>.
        /// </summary>
        public ObservableCollection<ShortcutBarItem> ShortcutBarItems
        {
            get { return shortcutBarItems; }
        }

        /// <summary>
        /// A collection of <see cref="ShortcutBarItems"/> for the right hand side of the shortcut bar.
        /// </summary>
        public ObservableCollection<ShortcutBarItem> ShortcutBarRightSideItems
        {
            get { return shortcutBarRightSideItems; }
        }
        public DynamoViewModel DynamoViewModel { get; private set; }

        /// <summary>
        /// Construct a ShortcutToolbar.
        /// </summary>
        /// <param name="dynamoViewModel"></param>
        public ShortcutToolbar(DynamoViewModel dynamoViewModel)
        {
            DynamoViewModel = dynamoViewModel;
            shortcutBarItems = new ObservableCollection<ShortcutBarItem>();
            shortcutBarRightSideItems = new ObservableCollection<ShortcutBarItem>();

            InitializeComponent();

            var shortcutToolbar = new ShortcutToolbarViewModel(dynamoViewModel);
            DataContext = shortcutToolbar;
            var newScriptButton = new ShortcutBarItem
            {
                ShortcutToolTip = Wpf.Properties.Resources.DynamoViewToolbarNewButtonTooltip,
                ShortcutCommand = dynamoViewModel.NewHomeWorkspaceCommand,
                ShortcutCommandParameter = null,
                ImgNormalSource = "/Dynamo.Wpf;component/Assets/Images/new_normal.png",
                ImgDisabledSource = "/Dynamo.Wpf;component/Assets/Images/new_disabled.png",
                ImgHoverSource = "/Dynamo.Wpf;component/Assets/Images/new_normal.png",
                Name = "New"
            };

            var openScriptButton = new ShortcutBarItem
            {
                ShortcutToolTip = Wpf.Properties.Resources.DynamoViewToolbarOpenButtonTooltip,
                ShortcutCommand = dynamoViewModel.ShowOpenDialogAndOpenResultCommand,
                ShortcutCommandParameter = null,
                ImgNormalSource = "/Dynamo.Wpf;component/Assets/Images/open_normal.png",
                ImgDisabledSource = "/Dynamo.Wpf;component/Assets/Images/open_disabled.png",
                ImgHoverSource = "/Dynamo.Wpf;component/Assets/Images/open_normal.png",
                Name = "Open"
            };

            var saveButton = new ShortcutBarItem
            {
                ShortcutToolTip = Wpf.Properties.Resources.DynamoViewToolbarSaveButtonTooltip,
                ShortcutCommand = dynamoViewModel.ShowSaveDialogIfNeededAndSaveResultCommand,
                ShortcutCommandParameter = null,
                ImgNormalSource = "/Dynamo.Wpf;component/Assets/Images/save_normal.png",
                ImgDisabledSource = "/Dynamo.Wpf;component/Assets/Images/save_disabled.png",
                ImgHoverSource = "/Dynamo.Wpf;component/Assets/Images/save_normal.png",
                Name = "Save"
            };

            var undoButton = new ShortcutBarItem
            {
                ShortcutToolTip = Wpf.Properties.Resources.DynamoViewToolbarUndoButtonTooltip,
                ShortcutCommand = dynamoViewModel.UndoCommand,
                ShortcutCommandParameter = null,
                ImgNormalSource = "/Dynamo.Wpf;component/Assets/Images/undo_normal.png",
                ImgDisabledSource = "/Dynamo.Wpf;component/Assets/Images/undo_disabled.png",
                ImgHoverSource = "/Dynamo.Wpf;component/Assets/Images/undo_normal.png",
                Name = "Undo"
            };

            var redoButton = new ShortcutBarItem
            {
                ShortcutToolTip = Wpf.Properties.Resources.DynamoViewToolbarRedoButtonTooltip,
                ShortcutCommand = dynamoViewModel.RedoCommand,
                ShortcutCommandParameter = null,
                ImgNormalSource = "/Dynamo.Wpf;component/Assets/Images/redo_normal.png",
                ImgDisabledSource = "/Dynamo.Wpf;component/Assets/Images/redo_disabled.png",
                ImgHoverSource = "/Dynamo.Wpf;component/Assets/Images/redo_normal.png",
                Name = "Redo"
            };

            ShortcutBarItems.Add(newScriptButton);
            ShortcutBarItems.Add(openScriptButton);
            ShortcutBarItems.Add(saveButton);
            ShortcutBarItems.Add(undoButton);
            ShortcutBarItems.Add(redoButton);

            this.Loaded += ShortcutToolbar_Loaded;
        }

        private void ShortcutToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            IsSaveButtonEnabled = false;
            IsExportMenuEnabled = false;
            DynamoViewModel.OnRequestShorcutToolbarLoaded(RightMenu.ActualWidth);
        }

        public void Dispose()
        {

            this.Loaded -= ShortcutToolbar_Loaded;
        }



        private void exportMenu_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            this.HeaderText.FontFamily = Resources["ArtifaktElementRegular"] as FontFamily;
            this.Icon.Source = new BitmapImage(new System.Uri(@"pack://application:,,,/Dynamo.Wpf;component/Assets/Images/image-icon.png"));
        }

        private void exportMenu_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.HeaderText.FontFamily = Resources["ArtifaktElementRegular"] as FontFamily;
            this.Icon.Source = new BitmapImage(new System.Uri(@"pack://application:,,,/Dynamo.Wpf;component/Assets/Images/image-icon-default.png"));
        }





        public List<Control> AllChildren(DependencyObject parent)
        {
            var _list = new List<Control> { };
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _child = VisualTreeHelper.GetChild(parent, i);
                if (_child is Control)
                {
                    _list.Add(_child as Control);
                    _list.AddRange(AllChildren(_child));
                }
            }
            return _list;
        }

        private Button GetButton(string shortcutName)
        {
            if (this.shortcutBarItems.Count > 1)
            {
                try
                {
                    int buttonIndex = ShortcutBarItems.ToList().FindIndex(item => item.Name.ToUpper() == shortcutName);
                    var _container = ShortcutItemsControl.ItemContainerGenerator.ContainerFromIndex(buttonIndex);
                    var _children = AllChildren(_container);
                    var _control = (Button)_children.First();
                    return _control;
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        internal bool IsNewButtonEnabled
        {
            set
            {
                Button saveButton = GetButton("NEW");
                if (saveButton != null)
                {
                    saveButton.IsEnabled = value;
                    saveButton.Opacity = value ? 1 : 0.5;
                }
            }
        }

        internal bool IsOpenButtonEnabled
        {
            set
            {
                Button saveButton = GetButton("OPEN");
                if (saveButton != null)
                {
                    saveButton.IsEnabled = value;
                    saveButton.Opacity = value ? 1 : 0.5;
                }
            }
        }

        internal bool IsSaveButtonEnabled
        {
            set
            {
                Button saveButton = GetButton("SAVE");
                if (saveButton != null)
                {
                    saveButton.IsEnabled = value;
                    saveButton.Opacity = value ? 1 : 0.5;
                }
            }
        }

        internal bool IsLoginMenuEnabled
        {
            set
            {

            }
        }

        internal bool IsExportMenuEnabled
        {
            set
            {
                this.exportMenu.IsEnabled = value;
                this.exportMenu.Opacity = value ? 1 : 0.5;
            }
        }

        internal bool IsNotificationCenterEnabled
        {
            get; set;
        }
    }

    /// <summary>
    /// An object which provides the data for an item in the shortcut toolbar.
    /// </summary>
    public partial class ShortcutBarItem : NotificationObject
    {
        protected string shortcutToolTip;

        /// <summary>
        /// A parameter sent to the ShortcutCommand.
        /// </summary>
        public string ShortcutCommandParameter { get; set; }

        /// <summary>
        /// The Command that will be executed by this shortcut item.
        /// </summary>
        public ICommand ShortcutCommand { get; set; }

        /// <summary>
        /// The path to the image for the disabled state of this shortcut item.
        /// </summary>
        public string ImgDisabledSource { get; set; }

        /// <summary>
        /// The path to the image for the hover state of this shortcut item.
        /// </summary>
        public string ImgHoverSource { get; set; }

        /// <summary>
        /// The path to the image for the normal state of this shortcut item.
        /// </summary>
        public string ImgNormalSource { get; set; }

        /// <summary>
        /// The tooltip for this shortcut item.
        /// </summary>
        public virtual string ShortcutToolTip
        {
            get { return shortcutToolTip; }
            set { shortcutToolTip = value; }
        }

        /// <summary>
        /// The Name of the shortcut
        /// </summary>
        public string Name { get; set; }
    }

    internal partial class ImageExportShortcutBarItem : ShortcutBarItem
    {
        private readonly DynamoViewModel vm;

        public override string ShortcutToolTip
        {
            get
            {
                return Wpf.Properties.Resources.DynamoViewToolbarExportButtonTooltip;
            }
            set
            {
                shortcutToolTip = value;
            }
        }

        public ImageExportShortcutBarItem(DynamoViewModel viewModel)
        {
            vm = viewModel;
        }

        private void BackgroundPreviewViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CanNavigateBackground") return;
            OnPropertyChanged("ShortcutToolTip");
        }
    }
}
