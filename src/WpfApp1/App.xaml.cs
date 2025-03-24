using Dynamo.Applications;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.ViewModels;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace WpfApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [DllImport("msvcrt.dll")]
    public static extern int _putenv(string env);
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += (sender, args) =>
        {
            //args.Handled = true;
            MessageBox.Show(args.Exception.Message);
        };
        base.OnStartup(e);
        _putenv(GetLocale());

        var model = StartupUtils.MakeModel(false, "", false, string.Empty, new HostAnalyticsInfo { });
        var startConfiguration = new DynamoViewModel.StartConfiguration()
        {
            DynamoModel = model,
            ShowLogin = false
        };
        var viewModel = DynamoViewModel.Start(startConfiguration);

        var dynamoView = new DynamoView(viewModel);
        dynamoView.Show();

    }
    public static string GetLocale()
    {
        var supportedLocale = new HashSet<string>(Dynamo.Configuration.Configurations.SupportedLocaleDic.Values);
        string libgLocale = CultureInfo.InstalledUICulture.ToString();

        // If locale is not supported by Dynamo, default to en-US.
        if (!supportedLocale.Any(s => s.Equals(libgLocale, StringComparison.InvariantCultureIgnoreCase)))
        {
            libgLocale = "en-US";
        }
        // Change the locale that LibG depends on.
        StringBuilder sb = new StringBuilder("LANGUAGE=");
        sb.Append(libgLocale.Replace("-", "_"));
        return sb.ToString();
    }
}

