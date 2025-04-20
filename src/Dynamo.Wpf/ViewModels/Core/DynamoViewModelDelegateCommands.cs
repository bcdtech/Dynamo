using CommunityToolkit.Mvvm.Input;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Wpf.ViewModels;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    partial class DynamoViewModel
    {
        private void InitializeDelegateCommands()
        {
            OpenCommand = new RelayCommand<object>(Open, CanOpen);
            OpenIfSavedCommand = new RelayCommand<object>(OpenIfSaved, CanOpen);
            OpenFromJsonCommand = new RelayCommand<object>(OpenFromJson, CanOpenFromJson);
            OpenFromJsonIfSavedCommand = new RelayCommand<object>(OpenFromJsonIfSaved, CanOpenFromJson);
            OpenRecentCommand = new RelayCommand<object>(OpenRecent, CanOpenRecent);
            SaveCommand = new RelayCommand<object>(Save, CanSave);
            SaveAsCommand = new RelayCommand<object>(SaveAs, CanSaveAs);
            ShowOpenDialogAndOpenResultCommand = new RelayCommand<object>(ShowOpenDialogAndOpenResult, CanShowOpenDialogAndOpenResultCommand);
            ShowOpenDialogAndOpenResultAsyncCommand = new RelayCommand<object>(ShowOpenDialogAndOpenResultAsync, CanShowOpenDialogAndOpenResultCommand);
            ShowOpenTemplateDialogAsyncCommand = new RelayCommand<object>(ShowOpenDialogAndOpenResultAsync, CanShowOpenDialogAndOpenResultCommand);
            ShowOpenTemplateDialogCommand = new RelayCommand<object>(ShowOpenDialogAndOpenResult, CanShowOpenDialogAndOpenResultCommand);
            ShowInsertDialogAndInsertResultCommand = new RelayCommand<object>(ShowInsertDialogAndInsertResult, CanShowInsertDialogAndInsertResultCommand);
            ShowSaveDialogAndSaveResultCommand = new RelayCommand<object>(ShowSaveDialogAndSaveResult, CanShowSaveDialogAndSaveResult);
            ShowSaveDialogIfNeededAndSaveResultCommand = new RelayCommand<object>(ShowSaveDialogIfNeededAndSaveResult, CanShowSaveDialogIfNeededAndSaveResultCommand);
            SaveImageCommand = new RelayCommand<object>(SaveImage, CanSaveImage);
            ValidateWorkSpaceBeforeToExportAsImageCommand = new RelayCommand<object>(ValidateWorkSpaceBeforeToExportAsImage, CanValidateWorkSpaceBeforeToExportAsImage);
            WriteToLogCmd = new RelayCommand<object>(o => model.Logger.Log(o.ToString()), CanWriteToLog);
            PostUiActivationCommand = new RelayCommand<object>(model.PostUIActivation);
            AddNoteCommand = new RelayCommand<object>(AddNote, CanAddNote);
            AddAnnotationCommand = new RelayCommand<object>(AddAnnotation, CanAddAnnotation);
            UngroupAnnotationCommand = new RelayCommand<object>(UngroupAnnotation, CanUngroupAnnotation);
            UngroupModelCommand = new RelayCommand<object>(UngroupModel, CanUngroupModel);
            AddModelsToGroupModelCommand = new RelayCommand<object>(AddModelsToGroup, CanAddModelsToGroup);
            AddGroupToGroupModelCommand = new RelayCommand<object>(AddGroupToGroup, CanAddModelsToGroup);
            AddToSelectionCommand = new RelayCommand<object>(model.AddToSelection, CanAddToSelection);
            ShowNewFunctionDialogCommand = new RelayCommand<object>(ShowNewFunctionDialogAndMakeFunction, CanShowNewFunctionDialogCommand);
            SaveRecordedCommand = new RelayCommand<object>(SaveRecordedCommands, CanSaveRecordedCommands);
            InsertPausePlaybackCommand = new RelayCommand<object>(ExecInsertPausePlaybackCommand, CanInsertPausePlaybackCommand);
            GraphAutoLayoutCommand = new RelayCommand<object>(DoGraphAutoLayout, CanDoGraphAutoLayout);
            GoHomeCommand = new RelayCommand<object>(GoHomeView, CanGoHomeView);
            SelectAllCommand = new RelayCommand<object>(SelectAll, CanSelectAll);
            HomeCommand = new RelayCommand<object>(GoHome, CanGoHome);
            NewHomeWorkspaceCommand = new RelayCommand<object>(MakeNewHomeWorkspace, CanMakeNewHomeWorkspace);
            CloseHomeWorkspaceCommand = new RelayCommand<object>(CloseHomeWorkspace, CanCloseHomeWorkspace);
            GoToWorkspaceCommand = new RelayCommand<object>(GoToWorkspace, CanGoToWorkspace);
            DeleteCommand = new RelayCommand<object>(Delete, CanDelete);
            ExitCommand = new RelayCommand<object>(Exit, CanExit);
            ToggleFullscreenWatchShowingCommand = new RelayCommand<object>(ToggleFullscreenWatchShowing, CanToggleFullscreenWatchShowing);
            ToggleBackgroundGridVisibilityCommand = new RelayCommand<object>(ToggleBackgroundGridVisibility, CanToggleBackgroundGridVisibility);
            UpdateGraphicHelpersScaleCommand = new RelayCommand<object>(UpdateGraphicHelpersScale, CanUpdateGraphicHelpersScale);
            AlignSelectedCommand = new RelayCommand<object>(AlignSelected, CanAlignSelected);
            UndoCommand = new RelayCommand<object>(Undo, CanUndo);
            RedoCommand = new RelayCommand<object>(Redo, CanRedo);
            CopyCommand = new RelayCommand<object>(_ => model.Copy(), CanCopy);
            PasteCommand = new RelayCommand<object>(Paste, CanPaste);
            ToggleConsoleShowingCommand = new RelayCommand<object>(ToggleConsoleShowing, CanToggleConsoleShowing);
            ForceRunExpressionCommand = new RelayCommand<object>(ForceRunExprCmd, RunSettingsViewModel.CanRunExpression);
            MutateTestDelegateCommand = new RelayCommand<object>(MutateTestCmd, RunSettingsViewModel.CanRunExpression);
            DisplayFunctionCommand = new RelayCommand<object>(DisplayFunction, CanDisplayFunction);
            SetConnectorTypeCommand = new RelayCommand<object>(SetConnectorType, CanSetConnectorType);
            ReportABugCommand = new RelayCommand<object>(ReportABug, CanReportABug);
            GoToWikiCommand = new RelayCommand<object>(GoToWiki, CanGoToWiki);
            GoToWebsiteCommand = new RelayCommand<object>(GoToWebsite, CanGoToWebsite);
            GoToSourceCodeCommand = new RelayCommand<object>(GoToSourceCode, CanGoToSourceCode);
            GoToDictionaryCommand = new RelayCommand<object>(GoToDictionary, CanGoToDictionary);
            DisplayStartPageCommand = new RelayCommand<object>(DisplayStartPage, CanDisplayStartPage);
            DisplayInteractiveGuideCommand = new RelayCommand<object>(DisplayStartPage, CanDisplayInteractiveGuide);


            SelectNeighborsCommand = new RelayCommand<object>(SelectNeighbors, CanSelectNeighbors);
            ClearLogCommand = new RelayCommand<object>(ClearLog, CanClearLog);
            PanCommand = new RelayCommand<object>(Pan, CanPan);
            ZoomInCommand = new RelayCommand<object>(ZoomIn, CanZoomIn);
            ZoomOutCommand = new RelayCommand<object>(ZoomOut, CanZoomOut);
            FitViewCommand = new RelayCommand<object>(FitView, CanFitView);
            EscapeCommand = new RelayCommand<object>(Escape, CanEscape);
            ImportLibraryCommand = new RelayCommand<object>(ImportLibrary, CanImportLibrary);
            ShowAboutWindowCommand = new RelayCommand<object>(ShowAboutWindow, CanShowAboutWindow);
            SetNumberFormatCommand = new RelayCommand<object>(SetNumberFormat, CanSetNumberFormat);
            DumpNodeHelpDataCommand = new RelayCommand<object>(DumpNodeHelpData, CanDumpNodeHelpData);
            DumpLibraryToXmlCommand = new RelayCommand<object>(model.DumpLibraryToXml, model.CanDumpLibraryToXml);
            ShowNewPresetsDialogCommand = new RelayCommand<object>(ShowNewPresetStateDialogAndMakePreset, CanShowNewPresetStateDialog);
            NodeFromSelectionCommand = new RelayCommand<object>(CreateNodeFromSelection, CanCreateNodeFromSelection);
            OpenDocumentationLinkCommand = new RelayCommand<object>(OpenDocumentationLink);
            ShowNodeDocumentationCommand = new RelayCommand<object>(ShowNodeDocumentation, CanShowNodeDocumentation);
            UnpinAllPreviewBubblesCommand = new RelayCommand<object>(UnpinAllPreviewBubbles, CanUnpinAllPreviewBubbles);
        }
        public ICommand OpenFromJsonIfSavedCommand { get; set; }
        public ICommand OpenFromJsonCommand { get; set; }
        public ICommand OpenIfSavedCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand ShowOpenDialogAndOpenResultAsyncCommand { get; set; }
        public ICommand ShowOpenDialogAndOpenResultCommand { get; set; }
        public ICommand ShowOpenTemplateDialogCommand { get; set; }
        public ICommand ShowOpenTemplateDialogAsyncCommand { get; set; }
        public ICommand ShowInsertDialogAndInsertResultCommand { get; set; }
        public ICommand WriteToLogCmd { get; set; }
        public ICommand PostUiActivationCommand { get; set; }
        public ICommand AddNoteCommand { get; set; }
        public ICommand AddAnnotationCommand { get; set; }
        public ICommand UngroupAnnotationCommand { get; set; }
        public ICommand UngroupModelCommand { get; set; }
        public ICommand AddModelsToGroupModelCommand { get; set; }
        public ICommand AddGroupToGroupModelCommand { get; set; }
        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand PasteCommand { get; set; }
        public ICommand AddToSelectionCommand { get; set; }
        public ICommand ShowNewFunctionDialogCommand { get; set; }
        public ICommand SaveRecordedCommand { get; set; }
        public ICommand InsertPausePlaybackCommand { get; set; }
        public ICommand GraphAutoLayoutCommand { get; set; }
        public ICommand GoHomeCommand { get; set; }
        public ICommand ShowPackageManagerSearchCommand { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand ShowSaveDialogIfNeededAndSaveResultCommand { get; set; }
        public ICommand ShowSaveDialogAndSaveResultCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand NewHomeWorkspaceCommand { get; set; }
        public ICommand CloseHomeWorkspaceCommand { get; set; }
        public ICommand GoToWorkspaceCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AlignSelectedCommand { get; set; }
        public ICommand UpdateAllPythonEngineCommand { get; set; }
        public ICommand PostUIActivationCommand { get; set; }
        public ICommand ToggleFullscreenWatchShowingCommand { get; set; }
        public ICommand ToggleBackgroundGridVisibilityCommand { get; set; }
        public ICommand UpdateGraphicHelpersScaleCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand SaveImageCommand { get; set; }
        public ICommand Save3DImageCommand { get; set; }
        public ICommand ValidateWorkSpaceBeforeToExportAsImageCommand { get; set; }
        public ICommand ToggleConsoleShowingCommand { get; set; }
        public ICommand ShowPackageManagerCommand { get; set; }
        public ICommand ForceRunExpressionCommand { get; set; }
        public ICommand MutateTestDelegateCommand { get; set; }
        public ICommand DisplayFunctionCommand { get; set; }
        public ICommand SetConnectorTypeCommand { get; set; }
        public ICommand ReportABugCommand { get; set; }
        public ICommand GoToWikiCommand { get; set; }
        public ICommand GoToDictionaryCommand { get; set; }
        public ICommand GoToWebsiteCommand { get; set; }
        public ICommand GoToSourceCodeCommand { get; set; }
        public ICommand DisplayStartPageCommand { get; set; }
        public ICommand DisplayInteractiveGuideCommand { get; set; }
        public ICommand GettingStartedGuideCommand { get; set; }
        public ICommand SelectNeighborsCommand { get; set; }
        public ICommand ClearLogCommand { get; set; }
        public ICommand SubmitCommand { get; set; }
        public ICommand PublishNewPackageCommand { get; set; }
        public ICommand PublishCurrentWorkspaceCommand { get; set; }
        public ICommand PublishSelectedNodesCommand { get; set; }
        public RelayCommand<Function> PublishCustomNodeCommand { get; set; }
        public ICommand PanCommand { get; set; }
        public ICommand ZoomInCommand { get; set; }
        public ICommand ZoomOutCommand { get; set; }
        public ICommand FitViewCommand { get; set; }
        public ICommand EscapeCommand { get; set; }
        public ICommand ExportToSTLCommand { get; set; }
        public ICommand ImportLibraryCommand { get; set; }
        public ICommand ShowAboutWindowCommand { get; set; }
        public ICommand SetNumberFormatCommand { get; set; }
        public ICommand OpenRecentCommand { get; set; }
        public ICommand CheckForLatestRenderCommand { get; set; }
        public ICommand DumpLibraryToXmlCommand { get; set; }
        public ICommand DumpNodeHelpDataCommand { get; set; }
        public ICommand ShowNewPresetsDialogCommand { get; set; }
        public ICommand NodeFromSelectionCommand { get; set; }
        public ICommand OpenDocumentationLinkCommand { get; set; }
        public ICommand ShowNodeDocumentationCommand { get; set; }
        public ICommand UnpinAllPreviewBubblesCommand { get; set; }
    }
}
