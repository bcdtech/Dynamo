using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
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
            var model = StartupUtils.MakeModel(false, "en-US", true, "", new HostAnalyticsInfo { });
            var path = Path.Combine(Environment.CurrentDirectory, "SampleNodesZeroTouch.dll");
            var assembly = System.Reflection.Assembly.LoadFile(path);
            model.LoadNodeLibrary(assembly, true);
            //var cmdLineArgs = StartupUtils.CommandLineArguments.Parse([]);

            //var model = StartupUtils.MakeCLIModel(cmdLineArgs);
            var resource = "Satest.UI.Assets.LayoutSpecs.json";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            var layoutSpecification = LayoutSpecification.FromJSONStream(stream);
            var startConfiguration = new DynamoViewModel.StartConfiguration()
            {
                DynamoModel = model,
                ShowLogin = false,
                NodeLibraryLayoutSpecification = layoutSpecification

            };

            var viewModel = DynamoViewModel.Start(startConfiguration);

            var dynamoView = new DynamoView(viewModel);
            //var libraryView = dynamoView.sidebarGrid.Children[0];
            //dynamoView.sidebarGrid.Children.Remove(libraryView);
            //sideGrid.Children.Add(libraryView);
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
