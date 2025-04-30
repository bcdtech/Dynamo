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
        }

        public OutPortViewModel ViewModel => this.DataContext as OutPortViewModel;
        public void UpdateCenter()
        {
            var center = new Point(this.ActualWidth, this.ActualHeight / 2);

            Debug.WriteLine($"orginal position:{center}");
            var container = this.FindUpVisualTree<DragCanvas>();
            var transform = this.TransformToAncestor(container);
            ViewModel.Center = transform.Transform(center);
            Debug.WriteLine($"transformed position:{ViewModel.Center}");

        }
    }
}
