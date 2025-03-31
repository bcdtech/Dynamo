using Dynamo.Applications;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.ViewModels;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var model = StartupUtils.MakeModel(false, "", true, string.Empty, new HostAnalyticsInfo { });
            var startConfiguration = new DynamoViewModel.StartConfiguration()
            {
                DynamoModel = model,
                ShowLogin = false
            };
            var viewModel = DynamoViewModel.Start(startConfiguration);

            var dynamoView = new DynamoView(viewModel);
            dynamoView.HorizontalAlignment = HorizontalAlignment.Stretch;
            dynamoView.VerticalAlignment = VerticalAlignment.Stretch;
            mainGrid.Children.Add(dynamoView);
        }
    }
}
