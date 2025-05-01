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

        public InPortViewModel ViewModel => this.DataContext as InPortViewModel;
        public void UpdateCenter()
        {
            //var center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            var center = new Point(0, this.ActualHeight / 2);

            Debug.WriteLine($"orginal position:{center}");
            var container = this.FindUpVisualTree<DragCanvas>();
            var transform = this.TransformToAncestor(container);
            ViewModel.Center = transform.Transform(center);
            Debug.WriteLine($"transformed position:{ViewModel.Center}");

        }
    }
}
