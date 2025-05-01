using Dynamo.UI;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using System.Diagnostics;
using System.Windows;

namespace Dynamo.Controls
{
    /// <summary>
    /// Interaction logic for OutPortView.xaml
    /// </summary>
    public partial class OutPortView : IViewModelView<OutPortViewModel>
    {
        public OutPortView()
        {
            InitializeComponent();
            Loaded += OutPortView_Loaded    ;
        }

        private void OutPortView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCenter();
        }

        public OutPortViewModel ViewModel => this.DataContext as OutPortViewModel;
        public void UpdateCenter()
        {
            var center = new Point(MainGrid.ActualWidth, MainGrid.ActualHeight / 2);

            Debug.WriteLine($"orginal position:{center}");
            var container = this.FindUpVisualTree<DragCanvas>();
            var transform = MainGrid.TransformToAncestor(container);
            ViewModel.Center = transform.Transform(center);
            Debug.WriteLine($"transformed position:{ViewModel.Center}");

        }
    }
}
