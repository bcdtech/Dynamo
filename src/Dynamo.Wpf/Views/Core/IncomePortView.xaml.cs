using Dynamo.Utilities;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dynamo.Controls
{
    /// <summary>
    /// Interaction logic for IncomePortView.xaml
    /// </summary>
    public partial class IncomePortView : UserControl
    {
        public IncomePortView()
        {
            InitializeComponent();
            Loaded += OutPortView_Loaded;
        }
        private void OutPortView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCenter();
        }
        public InPortViewModel ViewModel => this.DataContext as InPortViewModel;
        public void UpdateCenter()
        {
            var center = new Point(MainGrid.ActualWidth/2, 0);
            var container = this.FindUpVisualTree<DesignerCanvas>();
            var transform = MainGrid.TransformToAncestor(container);
            ViewModel.Center = transform.Transform(center);
        }
    }
}
