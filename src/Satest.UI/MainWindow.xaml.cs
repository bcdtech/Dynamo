using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.ViewModels;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Satest.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public List<int> Items { get; set; } = Enumerable.Range(0, 100).ToList();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //var assembly = Assembly.LoadFrom("DSCoreNodes.dll");
            //var rm = new ResourceManager("DSCoreNodesImages", assembly);
            //var img = rm.GetObject("Color.Small");
            //Assembly[] nodeAssemblies = [Assembly.LoadFrom("CoreNodeModels.dll"), Assembly.LoadFrom("CoreNodeModelsWpf.dll")];
            var model = StartupUtils.MakeModel(false, "", true, "", new HostAnalyticsInfo { });
            var path =Path.Combine(Environment.CurrentDirectory, "SampleNodesZeroTouch.dll");
            var assembly = System.Reflection.Assembly.LoadFile(path);
            model.LoadNodeLibrary(assembly, true);
            //var cmdLineArgs = StartupUtils.CommandLineArguments.Parse([]);

            //var model = StartupUtils.MakeCLIModel(cmdLineArgs);

            var startConfiguration = new DynamoViewModel.StartConfiguration()
            {
                DynamoModel = model,
                ShowLogin = false,
                

            };

            var viewModel = DynamoViewModel.Start(startConfiguration);

            var dynamoView = new DynamoView(viewModel);
            dynamoView.HorizontalAlignment = HorizontalAlignment.Stretch;
            dynamoView.VerticalAlignment = VerticalAlignment.Stretch;
            mainGrid.Children.Add(dynamoView);
        }

        private void mainGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
