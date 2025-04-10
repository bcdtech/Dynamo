using Dynamo.Configuration;
using Dynamo.Controls;
using Dynamo.Logging;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml.Serialization;


namespace Dynamo.UI.Views
{
    public partial class SplashScreen : Window
    {
        // These are hardcoded string and should only change when npm package structure changed or image path changed
        private static readonly string htmlEmbeddedFile = "Dynamo.Wpf.Packages.SplashScreen.build.index.html";
        private static readonly string jsEmbeddedFile = "Dynamo.Wpf.Packages.SplashScreen.build.index.bundle.js";
        private static readonly string backgroundImage = "Dynamo.Wpf.Views.SplashScreen.WebApp.splashScreenBackground.png";
        private static readonly string imageFileExtension = "png";

        /// <summary>
        /// True if the reason the splash screen was closed was because the user explicitly closed it,
        /// as opposed to the splash screen closing because dynamo was launched.
        /// This is useful for knowing if Dynamo is already started or not.
        /// </summary>
        public bool CloseWasExplicit { get; private set; }

        // Indicates if the SplashScren close button was hit.
        // Used to ensure that OnClosing is called only once.
        private bool IsClosing = false;

        internal enum CloseMode { ByStartingDynamo, ByCloseButton, ByOther };

        internal CloseMode currentCloseMode = CloseMode.ByOther;

        // Timer used for Splash Screen loading
        internal Stopwatch loadingTimer;

        /// <summary>
        /// Total loading time for the Dynamo loading tasks in milliseconds
        /// </summary>
        public long totalLoadingTime;

        /// <summary>
        /// Request to launch Dynamo main window.
        /// </summary>
        public Action<bool> RequestLaunchDynamo;

        internal Action<string> RequestImportSettings;
        internal Func<bool> RequestSignIn;
        internal Func<bool> RequestSignOut;


        /// <summary>
        /// Dynamo View Model reference
        /// </summary>
        internal DynamoViewModel viewModel;

        private DynamoView dynamoView;
        /// <summary>
        /// Dynamo View reference
        /// </summary>
        public DynamoView DynamoView
        {
            get
            {
                return dynamoView;
            }
            set
            {
                dynamoView = value;
                if (dynamoView == null)
                {
                    return;
                }
                viewModel = value.DataContext as DynamoViewModel;
            }
        }
        /// <summary>
        /// This delegate is used in StaticSplashScreenReady events
        /// </summary>
        internal delegate void StaticSplashScreenReadyHandler();

        /// <summary>
        /// This delegate is used in DynamicSplashScreenReady events
        /// </summary>
        public delegate void DynamicSplashScreenReadyHandler();

        /// <summary>
        /// Event to throw for Splash Screen to show Dynamo static screen
        /// </summary>
        internal event StaticSplashScreenReadyHandler StaticSplashScreenReady;

        /// <summary>
        /// Event to throw for Splash Screen to update Dynamo launching tasks
        /// </summary>
        public event DynamicSplashScreenReadyHandler DynamicSplashScreenReady;

        /// <summary>
        /// Request to trigger DynamicSplashScreenReady event
        /// </summary>
        public void OnRequestDynamicSplashScreen()
        {
            DynamicSplashScreenReady?.Invoke();
        }

        /// <summary>
        /// Request to trigger StaticSplashScreenReady event
        /// </summary>
        public void OnRequestStaticSplashScreen()
        {
            StaticSplashScreenReady?.Invoke();
        }

        /// <summary>
        /// Stores the value that indicates if the SignIn Button will be enabled(default) or not
        /// </summary>
        bool enableSignInButton;

        /// <summary>
        /// Splash Screen Constructor. 
        /// <paramref name="enableSignInButton"/> Indicates if the SignIn Button will be enabled(default) or not.
        /// </summary>
        public SplashScreen(bool enableSignInButton = true)
        {
            InitializeComponent();

            loadingTimer = new Stopwatch();
            loadingTimer.Start();


            DynamoModel.RequestUpdateLoadBarStatus += DynamoModel_RequestUpdateLoadBarStatus;
            DynamoModel.LanguageDetected += DynamoModel_LanguageDetected;
            StaticSplashScreenReady += OnStaticScreenReady;
            RequestLaunchDynamo = LaunchDynamo;
            RequestImportSettings = ImportSettings;

            this.enableSignInButton = enableSignInButton;
            currentCloseMode = CloseMode.ByOther;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // If we have multiple OnClosing events (ex Clicking the close button multiple times)
            // we need to only process the first one. THe rest should be canceled so that we can avoid timing issues with the order of windows messages
            // Ex  WM_CLOSE => webview2.Visibility.Set => waits for windows message =>  WM_DESTROY =>
            // webview2.Dispose => webview2.Visible.Set receives windows message => crash because object got disposed. 
            if (!IsClosing)
            {
                //Means that the SplashScreen was closed by other way for example by using the Windows Task Bar
                if (currentCloseMode == CloseMode.ByOther)
                {
                    CloseWasExplicit = true;
                }
                // First call to OnClosing
                IsClosing = true;
            }
            else
            {
                // Cancel the Close action for all subsequent calls
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void DynamoModel_LanguageDetected()
        {

        }



        /// <summary>
        /// Import setting file from chosen path
        /// </summary>
        /// <param name="fileContent"></param>
        private void ImportSettings(string fileContent)
        {
            bool isImported = viewModel.PreferencesViewModel.importSettingsContent(fileContent);
            if (isImported)
            {
                SetImportStatus(ImportStatus.success);
            }
            else
            {
                SetImportStatus(ImportStatus.error);
            }
            Analytics.TrackEvent(Actions.Import, Categories.SplashScreenOperations, isImported.ToString());
        }


        /// <summary>
        /// Handler to launch Dynamo View
        /// </summary>
        /// <param name="isCheckboxChecked"></param>
        private void LaunchDynamo(bool isCheckboxChecked)
        {
            CloseWasExplicit = false;
            if (viewModel != null)
            {
                viewModel.PreferenceSettings.EnableStaticSplashScreen = !isCheckboxChecked;
            }
            currentCloseMode = CloseMode.ByStartingDynamo;
            Close();

        }



        /// <summary>
        /// Once main window is initialized, Dynamic Splash screen should finish loading
        /// </summary>
        private void OnStaticScreenReady()
        {
            // Stop the timer in any case
            loadingTimer.Stop();

            //When a xml preferences settings file is located at C:\ProgramData\Dynamo will be read and deserialized so the settings can be set correctly.
            LoadPreferencesFileAtStartup();

            // If user is launching Dynamo for the first time or chose to always show splash screen, display it. Otherwise, display Dynamo view directly.
            if (viewModel.PreferenceSettings.IsFirstRun || viewModel.PreferenceSettings.EnableStaticSplashScreen)
            {

                SetLoadingDone();
            }
            else
            {
                RequestLaunchDynamo.Invoke(true);
            }
        }

        private void DynamoModel_RequestUpdateLoadBarStatus(SplashScreenLoadEventArgs args)
        {
            SetBarProperties(Dynamo.Utilities.AssemblyHelper.GetDynamoVersion().ToString(),
                    args.LoadDescription, args.BarSize);
        }

        /// <summary>
        /// This is used before DynamoModel initialization specifically to get user data dir
        /// </summary>
        /// <returns></returns>
        private string GetUserDirectory()
        {
            var version = AssemblyHelper.GetDynamoVersion();

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(Path.Combine(folder, Configurations.DynamoAsString, "Dynamo Core"),
                            String.Format("{0}.{1}", version.Major, version.Minor));
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);




        }

        internal async void SetBarProperties(string version, string loadingDescription, float barSize)
        {
            var elapsedTime = loadingTimer.ElapsedMilliseconds;
            totalLoadingTime += elapsedTime;
            loadingTimer = Stopwatch.StartNew();

        }

        internal async void SetLoadingDone()
        {

        }

        /// <summary>
        /// Set the import status on splash screen.
        /// </summary>
        /// <param name="importStatus"></param>
        internal async void SetImportStatus(ImportStatus importStatus)
        {
            string importSettingsTitle = Dynamo.Wpf.Properties.Resources.SplashScreenImportSettings;
            string errorDescription = string.Empty;

            switch (importStatus)
            {
                case ImportStatus.none:
                    errorDescription = Dynamo.Wpf.Properties.Resources.ImportPreferencesInfo;
                    break;
                case ImportStatus.error:
                    errorDescription = Dynamo.Wpf.Properties.Resources.SplashScreenImportSettingsFailDescription;
                    break;
                default:
                    errorDescription = Dynamo.Wpf.Properties.Resources.ImportPreferencesInfo;
                    break;
            }


        }




        /// <summary>
        /// At Dynamo startup process load the preferences settings file located in C:\ProgramData\Dynamo
        /// </summary>
        internal void LoadPreferencesFileAtStartup()
        {
            if (viewModel.PreferenceSettings.IsFirstRun == true)
            {
                //Move the current location two levels up
                var programDataDir = Directory.GetParent(Directory.GetParent(viewModel.Model.PathManager.CommonDataDirectory).ToString()).ToString();
                var listOfXmlFiles = Directory.GetFiles(programDataDir, "*.xml");
                string PreferencesSettingFilePath = string.Empty;

                //Find the first xml file name from the list that can be Deserialized to PreferenceSettings
                foreach (var xmlFile in listOfXmlFiles)
                {
                    if (IsValidPreferencesFile(xmlFile))
                    {
                        PreferencesSettingFilePath = xmlFile;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(PreferencesSettingFilePath) && File.Exists(PreferencesSettingFilePath))
                {
                    var content = File.ReadAllText(PreferencesSettingFilePath);
                    ImportSettings(content);
                }
            }
        }

        /// <summary>
        /// Try to Deserialize to PreferenceSettings the file content passed as parameter
        /// </summary>
        /// <param name="filePath">Full path to the xml file to be deserialized</param>
        /// <returns>true if the file content was deserialized successfully otherwise returns false</returns>
        private static bool IsValidPreferencesFile(string filePath)
        {
            string content = string.Empty;

            if (File.Exists(filePath))
            {
                content = File.ReadAllText(filePath);
            }

            if (string.IsNullOrEmpty(content))
                return false;

            try
            {
                PreferenceSettings settings = null;
                var serializer = new XmlSerializer(typeof(PreferenceSettings));
                using (TextReader reader = new StringReader(content))
                {
                    settings = serializer.Deserialize(reader) as PreferenceSettings;
                }
                return settings != null;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DynamoModel.RequestUpdateLoadBarStatus -= DynamoModel_RequestUpdateLoadBarStatus;
            DynamoModel.LanguageDetected -= DynamoModel_LanguageDetected;
            StaticSplashScreenReady -= OnStaticScreenReady;



            GC.SuppressFinalize(this);
        }
    }

    enum ImportStatus
    {
        none = 1,
        error,
        success
    }
}
