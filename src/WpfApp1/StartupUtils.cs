using Dynamo.Configuration;
using Dynamo.Core;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.Scheduler;

public static class StartupUtils
{
    //TODO internal?
    /// <summary>
    /// Raised when loading of the ASM binaries fails. A failure message is passed as a parameter.
    /// </summary>
    public static event Action<string> ASMPreloadFailure;



    /// <summary>
    /// Attempts to load the geometry library binaries using the location params.
    /// </summary>
    /// <param name="geometryFactoryPath">libG ProtoInterface path</param>
    /// <param name="preloaderLocation">libG folder path</param>



    private static DynamoModel PrepareModel(
        string cliLocale,
        string asmPath,
        bool noNetworkMode,
        HostAnalyticsInfo analyticsInfo,
        bool cliMode = true,
        string userDataFolder = "",
        string commonDataFolder = "",
        bool serviceMode = false)
    {
        var normalizedCLILocale = string.IsNullOrEmpty(cliLocale) ? null : cliLocale;
        IPathResolver pathResolver = CreatePathResolver(false, string.Empty, string.Empty, string.Empty);
        PathManager.Instance.AssignHostPathAndIPathResolver(string.Empty, pathResolver);
        DynamoModel.SetUICulture(normalizedCLILocale ?? PreferenceSettings.Instance.Locale);
        DynamoModel.OnDetectLanguage();

        // Preload ASM and display corresponding message on splash screen
        var model = StartDynamoWithDefaultConfig(
            CLImode: cliMode,
            userDataFolder: userDataFolder,
            commonDataFolder: commonDataFolder,
            geometryFactoryPath: "",
            preloaderLocation: "",
            noNetworkMode: noNetworkMode,
            info: analyticsInfo,
            isServiceMode: serviceMode,
            cliLocale: normalizedCLILocale
        );
        model.IsASMLoaded = false;
        return model;
    }



    /// <summary>
    /// Use this overload to construct a DynamoModel when the location of ASM to use is known and host name is known.
    /// </summary>
    /// <param name="CLImode">CLI mode starts the model in test mode and uses a separate path resolver.</param>
    /// <param name="asmPath">Path to directory containing geometry library binaries</param>
    /// <param name="hostName">Dynamo variation identified by host.</param>
    /// <returns></returns>
    public static DynamoModel MakeModel(bool CLImode, string asmPath = "", string hostName = "")
    {
        var model = PrepareModel(
            cliLocale: string.Empty,
            asmPath: asmPath,
            noNetworkMode: false,
            analyticsInfo: new HostAnalyticsInfo() { HostName = hostName },
            cliMode: CLImode);
        return model;
    }

    /// <summary>
    /// Use this overload to construct a DynamoModel when the location of ASM to use is known and host analytics info is known.
    /// </summary>
    /// <param name="CLImode">CLI mode starts the model in test mode and uses a separate path resolver.</param>
    /// <param name="noNetworkMode">Option to initialize Dynamo in no-network mode</param>
    /// <param name="asmPath">Path to directory containing geometry library binaries</param>
    /// <param name="info">Host analytics info specifying Dynamo launching host related information.</param>
    /// <returns></returns>
    public static DynamoModel MakeModel(bool CLImode, bool noNetworkMode, string asmPath = "", HostAnalyticsInfo info = new HostAnalyticsInfo())
    {
        var model = PrepareModel(
            cliLocale: string.Empty,
            asmPath: asmPath,
            noNetworkMode: noNetworkMode,
            analyticsInfo: info,
            cliMode: CLImode);
        return model;
    }

    /// <summary>
    /// Use this overload to construct a DynamoModel when the location of ASM to use is known and host analytics info is known.
    /// </summary>
    /// <param name="CLImode">CLI mode starts the model in test mode and uses a separate path resolver.</param>
    /// <param name="CLIlocale">CLI argument to force dynamo locale</param>
    /// <param name="noNetworkMode">Option to initialize Dynamo in no-network mode</param>
    /// <param name="asmPath">Path to directory containing geometry library binaries</param>
    /// <param name="info">Host analytics info specifying Dynamo launching host related information.</param>
    /// <returns></returns>
    public static DynamoModel MakeModel(bool CLImode, string CLIlocale, bool noNetworkMode, string asmPath = "", HostAnalyticsInfo info = new HostAnalyticsInfo())
    {
        var model = PrepareModel(
            cliLocale: CLIlocale,
            asmPath: asmPath,
            noNetworkMode: noNetworkMode,
            analyticsInfo: info,
            cliMode: CLImode);
        return model;
    }

    /// <summary>
    /// It returns an IPathResolver based on the mode and some locations
    /// </summary>
    /// <param name="CLImode">CLI mode starts the model in test mode and uses a seperate path resolver.</param>
    /// <param name="preloaderLocation">Path to be used by PathResolver for preLoaderLocation</param>
    /// <param name="userDataFolder">Path to be used by PathResolver for UserDataFolder</param>
    /// <param name="commonDataFolder">Path to be used by PathResolver for CommonDataFolder</param>
    /// <returns></returns>
    private static IPathResolver CreatePathResolver(bool CLImode, string preloaderLocation, string userDataFolder, string commonDataFolder)
    {
        IPathResolver pathResolver = new SandboxPathResolver(preloaderLocation) as IPathResolver;
        return pathResolver;
    }



    private static DynamoModel StartDynamoWithDefaultConfig(bool CLImode,
        string userDataFolder,
        string commonDataFolder,
        string geometryFactoryPath,
        string preloaderLocation,
        bool noNetworkMode,
        HostAnalyticsInfo info = new HostAnalyticsInfo(),
        bool isServiceMode = false,
        string cliLocale = null)
    {

        var config = new DynamoModel.DefaultStartConfiguration
        {
            GeometryFactoryPath = geometryFactoryPath,
            ProcessMode = CLImode ? TaskProcessMode.Synchronous : TaskProcessMode.Asynchronous,
            HostAnalyticsInfo = info,
            CLIMode = CLImode,
            StartInTestMode = CLImode,
            PathResolver = CreatePathResolver(CLImode, preloaderLocation, userDataFolder, commonDataFolder),
            IsServiceMode = isServiceMode,
            Preferences = PreferenceSettings.Instance,
            NoNetworkMode = noNetworkMode,
            CLILocale = cliLocale,
            //Breaks all Lucene calls. TI enable this would require a lot of refactoring around Lucene usage in Dynamo.
            //IsHeadless = CLImode
        };
        var model = DynamoModel.Start(config);
        return model;
    }



    /// <summary>
    /// The white list of dependencies to be ignored.
    /// </summary>
    private static readonly String[] assemblyNamesToIgnore = { "Newtonsoft.Json", "RevitAPI.dll", "RevitAPIUI.dll" };


}
