using Dynamo.UI;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using System.Diagnostics;
using System.Windows;
namespace Dynamo.Controls
{
    /// <summary>
    /// Interaction logic for InPortView.xaml
    /// </summary>
    public partial class InPortView : IViewModelView<InPortViewModel>
    {
        public InPortView()
        {
            InitializeComponent();
            Loaded += InPortView_Loaded;
        }

        private void InPortView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCenter();
        }


        public bool  IsCondensed
        {
            get { return (bool )GetValue(IsCondensedProperty); }
            set { SetValue(IsCondensedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCondensed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCondensedProperty =
            DependencyProperty.Register("IsCondensed", typeof(bool ), typeof(InPortView), new PropertyMetadata(false));


        public InPortViewModel ViewModel => this.DataContext as InPortViewModel;
        public void UpdateCenter()
        {
            var center = new Point(0, MainGrid.ActualHeight / 2);

            Debug.WriteLine($"orginal position:{center}");
            var container = this.FindUpVisualTree<DesignerCanvas>();
            var transform = MainGrid.TransformToAncestor(container);
            ViewModel.Center = transform.Transform(center);
            Debug.WriteLine($"transformed position:{ViewModel.Center}");

        }
    }
}
