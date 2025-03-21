using Autodesk.DesignScript.Runtime;
using Dynamo.PackageManager;
using Dynamo.PackageManager.UI;
using Dynamo.Utilities;
using Greg.Requests;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dynamo.UI.Views
{
    /// <summary>
    /// Interaction logic for PackageManagerWizard.xaml
    /// </summary>
    public partial class PackageManagerWizard : UserControl
    {
        #region Properties
        private static readonly string fontStylePath = "Dynamo.Wpf.Views.GuidedTour.HtmlPages.Resources.ArtifaktElement-Regular.woff";
        private static readonly string virtualFolderName = "embeddedFonts";
        private static readonly string fontUrl = $"http://{virtualFolderName}/ArtifaktElement-Regular.woff";
        private static readonly string virtualFolderPath = Path.Combine(Path.GetTempPath(), virtualFolderName);
        private static readonly string htmlRelativeFilePath = $"PackageManager\\index.html";

        private bool _onlyLoadOnce = false;
        private bool _disposed = false;

        private PublishPackageViewModel publishPackageViewModel;
        private List<PackageItemRootViewModel> _previousPackageContents = new List<PackageItemRootViewModel>();
        private List<PackageItemRootViewModel> _previousPreviewPackageContents = new List<PackageItemRootViewModel>();

        internal Action<string> RequestAddFileOrFolder;
        internal Action<string> RequestRemoveFileOrFolder;
        internal Action<bool> RequestRetainFolderStructure;
        internal Action<string> RequestUpdatePackageDetails;
        internal Action RequestSubmit;
        internal Action RequestPublish;
        internal Action RequestReset;
        internal Action<string> RequestToggleNodeLibraryOnItem;
        internal Action<string> RequestOpenFolder;
        internal Action<string> RequestUpdateCompatibilityMatrix;
        internal Action RequestLoadMarkdownContent;
        internal Action RequestClearMarkdownContent;
        internal Action<string> RequestLogMessage;

        #endregion



        private PackageUpdateRequest previousPackageDetails;

        public PackageManagerWizard()
        {
            InitializeComponent();



            // Bind event handlers
            RequestAddFileOrFolder = AddFileOrFolder;
            RequestRemoveFileOrFolder = RemoveFileOrFolder;
            RequestRetainFolderStructure = ToggleRetainFolderStructure;
            RequestUpdatePackageDetails = UpdatePackageDetails;
            RequestSubmit = Submit;
            RequestPublish = Publish;
            RequestReset = Reset;
            RequestToggleNodeLibraryOnItem = ToggleNodeLibraryOnItem;
            RequestOpenFolder = OpenFolder;
            RequestUpdateCompatibilityMatrix = UpdateCompatibilityMatrix;
            RequestLoadMarkdownContent = LoadMarkdownContent;
            RequestClearMarkdownContent = ClearMarkdownContent;
            RequestLogMessage = LogMessage;

            DataContextChanged += OnDataContextChanged;


        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_onlyLoadOnce) return;
            _onlyLoadOnce = true;

        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {


        }



        #region View Model PropertyChanged


        #endregion

        #region Upstream API calls





        #endregion

        #region Relay Commands
        internal void AddFileOrFolder(string fileOrFolder)
        {
            if (string.IsNullOrEmpty(fileOrFolder) || publishPackageViewModel == null)
                return;

            if (fileOrFolder.Equals("File", StringComparison.OrdinalIgnoreCase))
            {
                if (publishPackageViewModel.ShowAddFileDialogAndAddCommand?.CanExecute(null) == true)
                {
                    publishPackageViewModel.ShowAddFileDialogAndAddCommand.Execute(null);
                }
            }
            else if (fileOrFolder.Equals("Folder", StringComparison.OrdinalIgnoreCase))
            {
                if (publishPackageViewModel.SelectDirectoryAndAddFilesRecursivelyCommand?.CanExecute(null) == true)
                {
                    publishPackageViewModel.SelectDirectoryAndAddFilesRecursivelyCommand.Execute(null);
                }
            }
        }

        internal void RemoveFileOrFolder(string filePath)
        {
            if (publishPackageViewModel == null) return;

            var itemToRemove = FindItemByFilePath(publishPackageViewModel.PackageContents.ToList(), filePath);

            if (itemToRemove != null)
            {
                if (publishPackageViewModel.RemoveItemCommand?.CanExecute(itemToRemove) == true)
                {
                    publishPackageViewModel.RemoveItemCommand.Execute(itemToRemove);
                }
            }
        }

        internal void ToggleNodeLibraryOnItem(string filePath)
        {
            if (publishPackageViewModel == null) return;

            var itemToToggle = FindItemByFilePath(publishPackageViewModel.PackageContents.ToList(), filePath);

            if (itemToToggle != null)
            {
                itemToToggle.IsNodeLibrary = !itemToToggle.IsNodeLibrary;
            }
        }

        private PackageItemRootViewModel FindItemByFilePath(List<PackageItemRootViewModel> items, string filePath)
        {
            foreach (var item in items)
            {
                switch (item.DependencyType)
                {
                    case DependencyType.Folder when item.DirectoryName == filePath:
                    case DependencyType.File when item.FilePath == filePath:
                    case DependencyType.CustomNodePreview when item.FilePath == filePath:
                    case DependencyType.Assembly when item.FilePath == filePath:
                        return item;
                }

                var foundItem = FindItemByFilePath(item.ChildItems.ToList(), filePath);
                if (foundItem != null)
                {
                    return foundItem;
                }
            }

            return null;
        }

        internal void ToggleRetainFolderStructure(bool flag)
        {
            if (publishPackageViewModel == null) return;

            publishPackageViewModel.RetainFolderStructureOverride = flag;
        }

        internal void UpdatePackageDetails(string jsonPayload)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload) || publishPackageViewModel == null)
                return;

            try
            {
                var packageDetails = JsonSerializer.Deserialize<PackageUpdateRequest>(
                    jsonPayload,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (packageDetails == null) return;

                // Check if the new details are the same as the previous ones
                if (previousPackageDetails != null && previousPackageDetails.Equals(packageDetails))
                    return;

                // Store the new details to prevent redundant updates
                previousPackageDetails = packageDetails;

                // Assign required fields
                publishPackageViewModel.Name = packageDetails.Name;
                publishPackageViewModel.Description = packageDetails.Description;

                // Assign version numbers separately
                publishPackageViewModel.MajorVersion = packageDetails.Major;
                publishPackageViewModel.MinorVersion = packageDetails.Minor;
                publishPackageViewModel.BuildVersion = packageDetails.Patch;

                // Join keywords into a comma-separated string
                publishPackageViewModel.Keywords = packageDetails.Keywords?.Count > 0
                    ? string.Join(", ", packageDetails.Keywords)
                    : string.Empty;

                // Assign optional fields
                publishPackageViewModel.CopyrightHolder = packageDetails.CopyrightHolder ?? string.Empty;
                publishPackageViewModel.CopyrightYear = packageDetails.CopyrightYear ?? string.Empty;
                publishPackageViewModel.License = packageDetails.License ?? string.Empty;
                publishPackageViewModel.RepositoryUrl = packageDetails.RepositoryUrl ?? string.Empty;
                publishPackageViewModel.SiteUrl = packageDetails.SiteUrl ?? string.Empty;
                publishPackageViewModel.Group = packageDetails.Group ?? string.Empty;
                publishPackageViewModel.ReleaseNotesUrl = packageDetails.ReleaseNotesUrl ?? string.Empty;

                LogMessage("Package details updated successfully.");
            }
            catch (Exception ex)
            {
                LogMessage(ex);
            }
        }

        internal void UpdateCompatibilityMatrix(string jsonPayload)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload) || publishPackageViewModel == null)
                return;

            try
            {
                var compatibilityMatrix = JsonSerializer.Deserialize<ICollection<PackageCompatibility>>(
                    jsonPayload,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (compatibilityMatrix == null) return;

                publishPackageViewModel.CompatibilityMatrix = compatibilityMatrix;

                LogMessage("Compatibility matrix updated successfully.");
            }
            catch (Exception ex)
            {
                LogMessage(ex);
            }
        }

        internal void LoadMarkdownContent()
        {
            if (publishPackageViewModel == null)
                return;

            if (publishPackageViewModel.SelectMarkdownDirectoryCommand?.CanExecute(null) == true)
            {
                publishPackageViewModel.SelectMarkdownDirectoryCommand.Execute(null);
            }
        }

        internal void ClearMarkdownContent()
        {
            if (publishPackageViewModel == null)
                return;

            if (publishPackageViewModel.ClearMarkdownDirectoryCommand?.CanExecute(null) == true)
            {
                publishPackageViewModel.ClearMarkdownDirectoryCommand.Execute(null);
            }
        }

        internal void Submit()
        {
            if (publishPackageViewModel == null) return;
            if (publishPackageViewModel.SubmitCommand?.CanExecute(null) == true)
            {
                publishPackageViewModel.SubmitCommand.Execute(null);
            }
        }

        internal void Publish()
        {
            if (publishPackageViewModel == null) return;
            if (publishPackageViewModel.PublishLocallyCommand?.CanExecute(null) == true)
            {
                publishPackageViewModel.PublishLocallyCommand.Execute(null);
            }
        }

        internal void Reset()
        {
            if (publishPackageViewModel == null) return;
            if (publishPackageViewModel.CancelCommand?.CanExecute(null) == true)
            {
                publishPackageViewModel.CancelCommand.Execute(null);
            }
        }

        internal void OpenFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
        }

        #endregion

        #region Utility
        /// <summary>
        ///  Generates the absolute runtime filePath of the html file
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetFileFromRelativePath(string relativePath)
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new ArgumentNullException(nameof(Path.GetDirectoryName));
            var toolPath = Path.Combine(rootPath, relativePath);

            return toolPath;
        }



        internal bool ProcessUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return false;

            Uri fileUri;
            try
            {
                fileUri = new Uri(uri);
            }
            catch (UriFormatException)
            {
                return false;
            }

            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            Logging.Analytics.TrackEvent(Logging.Actions.Open, Logging.Categories.DynamoHomeOperations, "Hyper Link: " + uri);

            return false;
        }

        /// <summary>
        /// This is used before DynamoModel initialization specifically to get user data dir
        /// </summary>
        /// <returns></returns>
        private static string GetUserDirectory()
        {
            var version = AssemblyHelper.GetDynamoVersion();

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(Path.Combine(folder, "Dynamo", "Dynamo Core"),
                            String.Format("{0}.{1}", version.Major, version.Minor));
        }

        /// <summary>
        /// Compare package contents using hashsets for quick comparison
        /// </summary>
        /// <param name="oldContents"></param>
        /// <param name="newContents"></param>
        /// <returns></returns>
        internal static bool ArePackageContentsEqual(List<PackageItemRootViewModel> oldContents, List<PackageItemRootViewModel> newContents)
        {
            if (oldContents.Count != newContents.Count)
                return false;

            // Sort by FilePath to ensure order-independent comparison
            var oldSorted = oldContents.OrderBy(item => item.FilePath).ToList();
            var newSorted = newContents.OrderBy(item => item.FilePath).ToList();

            for (int i = 0; i < oldSorted.Count; i++)
            {
                var oldItem = oldSorted[i];
                var newItem = newSorted[i];

                // Compare file paths directly
                if (oldItem.FilePath != newItem.FilePath)
                    return false;

                // Recursively compare child items
                if (!ArePackageContentsEqual(oldItem.ChildItems.ToList(), newItem.ChildItems.ToList()))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Coverts a list of PackageItemRootViewModel to simple json structure
        /// </summary>
        /// <param name="packageContents"></param>
        /// <returns></returns>
        private object ConvertToJsonFormat(List<PackageItemRootViewModel> packageContents)
        {
            return new
            {
                name = "root",
                children = packageContents.Select(ConvertToNode).ToList(),
                filePath = "",
                isNodeLibrary = false,
                isSelected = false,
                isFolder = true,
            };
        }

        /// <summary>
        /// Recursive function to convert to simple json structrue
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private object ConvertToNode(PackageItemRootViewModel node)
        {
            bool isFolder = node.DependencyType == DependencyType.Folder;

            return new
            {
                name = node.DisplayName,
                children = node.ChildItems?.Select(ConvertToNode).ToList() ?? new List<object>(),
                filePath = isFolder ? node.DirectoryName : node.FilePath,
                isNodeLibrary = node.IsNodeLibrary,
                isSelected = node.IsSelected,
                isFolder = isFolder
            };
        }

        /// <summary>
        /// Error handling and loging
        /// Overloaded method for messages
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            string logMessage = $"Package Manager Wizard INFO: {message}";
            this.publishPackageViewModel?.DynamoViewModel?.Model.Logger.Log(logMessage);
        }

        /// <summary>
        /// Error handling and loging
        /// Overloaded method for exceptions
        /// </summary>
        /// <param name="ex">Exception to log</param>
        private void LogMessage(Exception ex)
        {
            if (ex == null) return;

            string logMessage = $"Package Manager Wizard ERROR: {ex.GetType()}: {ex.Message}\nStack Trace:\n{ex.StackTrace}";
            this.publishPackageViewModel?.DynamoViewModel?.Model.Logger.Log(logMessage);
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers (as per Microsoft documentation)
        /// https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern
        /// </summary>
        /// <param name="disposing">To be called by the finalizer if necessary</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.DataContextChanged -= OnDataContextChanged;


                }

                _disposed = true;
            }
        }
        #endregion
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class ScriptWizardObject
    {
        readonly Action<string> RequestAddFileOrFolder;
        readonly Action<string> RequestRemoveFileOrFolder;
        readonly Action<bool> RequestRetainFolderStructure;
        readonly Action<string> RequestUpdatePackageDetails;
        readonly Action RequestSubmit;
        readonly Action RequestPublish;
        readonly Action RequestReset;
        readonly Action<string> RequestToggleNodeLibraryOnItem;
        readonly Action<string> RequestOpenFolder;
        readonly Action<string> RequestUpdateCompatibilityMatrix;
        readonly Action RequestLoadMarkdownContent;
        readonly Action RequestClearMarkdownContent;
        readonly Action<string> RequestLogMessage;

        public ScriptWizardObject(
            Action<string> requestAddFileOrFolder,
            Action<string> requestRemoveFileOrFolder,
            Action<bool> requestRetainFolderStructure,
            Action<string> requestUpdatePackageDetails,
            Action requestSubmit,
            Action requestPublish,
            Action requestReset,
            Action<string> requestToggleNodeLibraryOnItem,
            Action<string> requestOpenFolder,
            Action<string> requestUpdateCompatibilityMatrix,
            Action requestLoadMarkdownContent,
            Action requestClearMarkdownContent,
            Action<string> requestLogMessage)
        {
            RequestAddFileOrFolder = requestAddFileOrFolder;
            RequestRemoveFileOrFolder = requestRemoveFileOrFolder;
            RequestRetainFolderStructure = requestRetainFolderStructure;
            RequestUpdatePackageDetails = requestUpdatePackageDetails;
            RequestSubmit = requestSubmit;
            RequestPublish = requestPublish;
            RequestReset = requestReset;
            RequestToggleNodeLibraryOnItem = requestToggleNodeLibraryOnItem;
            RequestOpenFolder = requestOpenFolder;
            RequestUpdateCompatibilityMatrix = requestUpdateCompatibilityMatrix;
            RequestLoadMarkdownContent = requestLoadMarkdownContent;
            RequestClearMarkdownContent = requestClearMarkdownContent;
            RequestLogMessage = requestLogMessage;
        }

        [DynamoJSInvokable]
        public void AddFileOrFolder(string fileOrFolder)
        {
            RequestAddFileOrFolder(fileOrFolder);
        }


        [DynamoJSInvokable]
        public void RemoveFileOrFolder(string name)
        {
            RequestRemoveFileOrFolder(name);
        }


        [DynamoJSInvokable]
        public void ToggleRetainFolderStructure(bool flag)
        {
            RequestRetainFolderStructure(flag);
        }

        [DynamoJSInvokable]
        public void UpdatePackageDetails(string jsonPayload)
        {
            RequestUpdatePackageDetails(jsonPayload);
        }

        [DynamoJSInvokable]
        public void UpdateCompatibilityMatrix(string jsonPayload)
        {
            RequestUpdateCompatibilityMatrix(jsonPayload);
        }

        [DynamoJSInvokable]
        public void LoadMarkdownContent()
        {
            RequestLoadMarkdownContent();
        }

        [DynamoJSInvokable]
        public void ClearMarkdownContent()
        {
            RequestClearMarkdownContent();
        }

        [DynamoJSInvokable]
        public void Submit()
        {
            RequestSubmit();
        }

        [DynamoJSInvokable]
        public void Publish()
        {
            RequestPublish();
        }

        [DynamoJSInvokable]
        public void Reset()
        {
            RequestReset();
        }

        [DynamoJSInvokable]
        public void ToggleNodeLibraryOnItem(string name)
        {
            RequestToggleNodeLibraryOnItem(name);
        }

        [DynamoJSInvokable]
        public void OpenFolder(string path)
        {
            RequestOpenFolder(path);
        }

        [DynamoJSInvokable]
        public void LogMessage(string message)
        {
            RequestLogMessage(message);
        }
    }

    public class PackageUpdateRequest : IEquatable<PackageUpdateRequest>
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("major")]
        public string Major { get; set; }

        [JsonPropertyName("minor")]
        public string Minor { get; set; }

        [JsonPropertyName("patch")]
        public string Patch { get; set; }

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = new();

        [JsonPropertyName("release_notes_url")]
        public string ReleaseNotesUrl { get; set; }

        [JsonPropertyName("copyright_holder")]
        public string CopyrightHolder { get; set; }

        [JsonPropertyName("copyright_year")]
        public string CopyrightYear { get; set; }

        [JsonPropertyName("license")]
        public string License { get; set; }

        [JsonPropertyName("repository_url")]
        public string RepositoryUrl { get; set; }

        [JsonPropertyName("site_url")]
        public string SiteUrl { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; }

        public bool Equals(PackageUpdateRequest other)
        {
            if (other == null) return false;

            return Name == other.Name &&
                   Description == other.Description &&
                   Major == other.Major &&
                   Minor == other.Minor &&
                   Patch == other.Patch &&
                   (Keywords == null && other.Keywords == null || (Keywords?.SequenceEqual(other.Keywords) ?? false)) &&
                   ReleaseNotesUrl == other.ReleaseNotesUrl &&
                   CopyrightHolder == other.CopyrightHolder &&
                   CopyrightYear == other.CopyrightYear &&
                   License == other.License &&
                   RepositoryUrl == other.RepositoryUrl &&
                   SiteUrl == other.SiteUrl &&
                   Group == other.Group;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PackageUpdateRequest);
        }

        /// <summary>
        /// Overriding GetHashCode allows us to get a deep equality
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked // Allow integer overflow (safe for hash calculations)
            {
                int hash = 17;
                hash = hash * 31 + (Name?.GetHashCode() ?? 0);
                hash = hash * 31 + (Description?.GetHashCode() ?? 0);
                hash = hash * 31 + (Major?.GetHashCode() ?? 0);
                hash = hash * 31 + (Minor?.GetHashCode() ?? 0);
                hash = hash * 31 + (Patch?.GetHashCode() ?? 0);
                hash = hash * 31 + (ReleaseNotesUrl?.GetHashCode() ?? 0);
                hash = hash * 31 + (CopyrightHolder?.GetHashCode() ?? 0);
                hash = hash * 31 + (CopyrightYear?.GetHashCode() ?? 0);
                hash = hash * 31 + (License?.GetHashCode() ?? 0);
                hash = hash * 31 + (RepositoryUrl?.GetHashCode() ?? 0);
                hash = hash * 31 + (SiteUrl?.GetHashCode() ?? 0);
                hash = hash * 31 + (Group?.GetHashCode() ?? 0);

                // Handle list separately
                if (Keywords != null)
                {
                    foreach (var keyword in Keywords)
                    {
                        hash = hash * 31 + (keyword?.GetHashCode() ?? 0);
                    }
                }

                return hash;
            }
        }

    }

}
