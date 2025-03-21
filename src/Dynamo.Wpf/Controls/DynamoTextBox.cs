using Dynamo.Controls;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.UI;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Thickness = System.Windows.Thickness;

namespace Dynamo.Nodes
{
    //taken from http://stackoverflow.com/questions/660554/how-to-automatically-select-all-text-on-focus-in-wpf-textbox
    public class ClickSelectTextBox : TextBox
    {
        protected bool selectAllWhenFocused = true;
        protected RoutedEventHandler focusHandler;

        public ClickSelectTextBox()
        {
            focusHandler = new RoutedEventHandler(SelectAllText);
            AddHandler(PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent, focusHandler, true);
            AddHandler(MouseDoubleClickEvent, focusHandler, true);
        }

        private static void SelectivelyIgnoreMouseButton(
            object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = parent as ClickSelectTextBox;
                if (textBox != null && (!textBox.IsKeyboardFocusWithin))
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = textBox.selectAllWhenFocused;
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }
    }

    public class DynamoTextBox : ClickSelectTextBox
    {
        public event Action OnChangeCommitted;

        private static Brush clear = new SolidColorBrush(Color.FromArgb(255, 102, 102, 102));
        private static Brush highlighted = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));

        private NodeViewModel nodeViewModel;
        private NodeViewModel NodeViewModel
        {
            get
            {
                if (nodeViewModel != null) return nodeViewModel;

                var f = WpfUtilities.FindUpVisualTree<NodeView>(this);
                if (f != null) this.nodeViewModel = f.ViewModel;

                return nodeViewModel;
            }
        }

        #region Class Operational Methods

        public DynamoTextBox()
            : this(string.Empty)
        {
        }

        public DynamoTextBox(string initialText)
        {
            //turn off the border
            Background = clear;
            BorderThickness = new Thickness(1);
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 74, 74, 74));
            Foreground = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238));
            CaretBrush = new SolidColorBrush(Color.FromArgb(255, 106, 192, 231));
            GotFocus += OnGotFocus;
            FontSize = 16;
            LostFocus += OnLostFocus;
            LostKeyboardFocus += OnLostFocus;
            Padding = new Thickness(5, 3, 5, 3);
            base.Text = initialText;
            Pending = false;
            Style = (Style)SharedDictionaryManager.DynamoModernDictionary["SZoomFadeTextBox"];
            MinHeight = 29;
            MinWidth = 100;
        }

        public void BindToProperty(Binding binding)
        {
            SetBinding(TextProperty, binding);
            UpdateDataSource(false);
        }

        #endregion

        #region Class Properties

        private bool pending;

        public bool Pending
        {
            get { return pending; }
            set
            {
                FontStyle = value ? FontStyles.Italic : FontStyles.Normal;
                pending = value;
            }
        }

        /// <summary>
        /// This property hides the base "TextBox.Text" property to remove the 
        /// ability to directly set its value (by-passing the undo recording 
        /// completely). For this reason, there is no setter for this property.
        /// </summary>
        new public string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                UpdateDataSource(true);
            }
        }

        #endregion

        #region Class Event Handlers

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            Background = clear;
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            Background = highlighted;
            SelectAll();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Pending = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && NodeViewModel != null)
            {
                NodeViewModel.DynamoViewModel.OnRequestReturnFocusToView();
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            UpdateDataSource(true);
        }

        #endregion

        #region Private Class Helper Methods

        private void UpdateDataSource(bool recordForUndo)
        {
            if (Pending)
            {
                var expr = GetBindingExpression(TextProperty);

                var nvm = NodeViewModel;
                if (expr != null)
                {
                    // There are two ways in which the bound data source can be 
                    // updated: when it is first bound to the target (text box),
                    // and when it is explicitly updated by text box losing its 
                    // focus. In the first way, even though the bound data source 
                    // is updated, its actual value does not get changed. In this 
                    // case there is no need for undo recording. However, it is 
                    // deemed as a user commit when text box loses its focus, in 
                    // which case undo recording has to be done. It is in the 
                    // second case a command is being sent to actually update the 
                    // data source (also record the update for undo).

                    if (false == recordForUndo)
                    {   //when first bound we do not update the
                        //source unless the input text is valid
                        if (expr.ValidateWithoutUpdate())
                        {
                            expr.UpdateSource();
                        }
                    }

                    else if (nvm != null && expr.ValidateWithoutUpdate())
                    {
                        string propName = expr.ParentBinding.Path.Path;
                        nvm.DynamoViewModel.ExecuteCommand(
                            new DynamoModel.UpdateModelValueCommand(
                                nodeViewModel.WorkspaceViewModel.Model.Guid,
                                nvm.NodeModel.GUID, propName, Text));
                    }

                    if (expr.HasValidationError && nvm != null)
                    {
                        nvm.NodeModel.Error(expr.ValidationError.ErrorContent as string);
                    }
                }

                if (OnChangeCommitted != null)
                    OnChangeCommitted();

                Pending = false;
            }
        }

        private NodeModel GetBoundModel(object dataItem)
        {
            // Attempt get to the data-bound model (if there's any).
            NodeModel nodeModel = dataItem as NodeModel;
            if (null == nodeModel)
            {
                NodeViewModel nodeViewModel = dataItem as NodeViewModel;
                if (null != nodeViewModel)
                    nodeModel = nodeViewModel.NodeModel;
            }

            return nodeModel;
        }

        #endregion
    }

    public class StringTextBox : DynamoTextBox
    {

        #region Class Event Handlers

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // This method is overridden so that the base implementation will 
            // not be called (the base class commits changes once <Enter> key
            // is pressed, not something that a multi-line string edit box needs.
        }

        #endregion
    }
}

