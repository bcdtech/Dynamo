using Dynamo.Applications;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.ViewModels;
using System.Windows;

namespace WpfApp1
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new Application();
            var model = StartupUtils.MakeModel(false, "", true, string.Empty, new HostAnalyticsInfo { });
            var startConfiguration = new DynamoViewModel.StartConfiguration()
            {
                DynamoModel = model,
                ShowLogin = false
            };
            var viewModel = DynamoViewModel.Start(startConfiguration);

            var dynamoView = new DynamoView(viewModel);
            dynamoView.Show();
            app.Run();
        }
    }
}
