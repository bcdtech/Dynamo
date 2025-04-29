using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Logging;
using Dynamo.Models;
using Dynamo.Properties;
using Dynamo.Search.SearchElements;
using Dynamo.Utilities;
using Dynamo.Wpf.ViewModels;
using ProtoCore.AST.AssociativeAST;
using ProtoCore.Mirror;
using ProtoCore.Utils;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Dynamo.ViewModels
{
    /// <summary>
    /// Search View Model for Node AutoComplete Search Bar
    /// </summary>
    public class NodeAutoCompleteSearchViewModel : SearchViewModel
    {
        internal PortViewModel PortViewModel { get; set; }
        private List<NodeSearchElementViewModel> searchElementsCache;
        private string autocompleteMLMessage;
        private string autocompleteMLTitle;
        private bool displayAutocompleteMLStaticPage;
        private bool displayLowConfidence;
        private const string nodeAutocompleteMLEndpoint = "MLNodeAutocomplete";
        private const string nodeClusterAutocompleteMLEndpoint = "MLNodeClusterAutocomplete";



        /// <summary>
        /// The Node AutoComplete ML service version, this could be empty if user has not used ML way
        /// </summary>
        internal string ServiceVersion { get; set; }

        /// <summary>
        /// Cache of default node suggestions, use it in case where
        /// a. our algorithm does not return sufficient results
        /// b. the results returned by our algorithm will not be useful for user
        /// </summary>
        internal IEnumerable<NodeSearchElementViewModel> DefaultResults { get; set; }

        /// <summary>
        /// For checking if the ML method is selected
        /// </summary>
        public bool IsDisplayingMLRecommendation
        {
            get
            {
                return dynamoViewModel.PreferenceSettings.DefaultNodeAutocompleteSuggestion == Models.NodeAutocompleteSuggestion.MLRecommendation;
            }
        }

        /// <summary>
        /// If MLAutocompleteTOU is approved
        /// </summary>
        public bool IsMLAutocompleteTOUApproved
        {
            get
            {
                return dynamoViewModel.PreferenceSettings.IsMLAutocompleteTOUApproved;
            }
        }

        /// <summary>
        /// If true, autocomplete method options are hidden from UI 
        /// </summary>
        public bool HideAutocompleteMethodOptions
        {
            get
            {
                return dynamoViewModel.PreferenceSettings.HideAutocompleteMethodOptions;
            }
        }


        /// <summary>
        /// The No Recommendations or Low Confidence Title
        /// </summary>
        public string AutocompleteMLTitle
        {
            get { return autocompleteMLTitle; }
            set
            {
                autocompleteMLTitle = value;
                OnPropertyChanged(nameof(AutocompleteMLTitle));
            }
        }

        /// <summary>
        /// The No Recommendations or Low Confidence message
        /// </summary>
        public string AutocompleteMLMessage
        {
            get { return autocompleteMLMessage; }
            set
            {
                autocompleteMLMessage = value;
                OnPropertyChanged(nameof(AutocompleteMLMessage));
            }
        }

        /// <summary>
        /// Indicates the No recommendations / Low confidence message should be displayed (image and texts)
        /// </summary>
        public bool DisplayAutocompleteMLStaticPage
        {
            get { return displayAutocompleteMLStaticPage; }
            set
            {
                displayAutocompleteMLStaticPage = value;
                OnPropertyChanged(nameof(DisplayAutocompleteMLStaticPage));
            }
        }

        /// <summary>
        /// Indicates if display the Low confidence option and Tooltip
        /// </summary>
        public bool DisplayLowConfidence
        {
            get { return displayLowConfidence; }
            set
            {
                displayLowConfidence = value;
                OnPropertyChanged(nameof(DisplayLowConfidence));
            }
        }

        internal event Action<NodeModel> ParentNodeRemoved;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dynamoViewModel">Dynamo ViewModel</param>
        internal NodeAutoCompleteSearchViewModel(DynamoViewModel dynamoViewModel) : base(dynamoViewModel)
        {
            // Off load some time consuming operation here
            DefaultResults = dynamoViewModel.DefaultAutocompleteCandidates.Values;
            ServiceVersion = string.Empty;
        }

        /// <summary>
        /// Reset Node AutoComplete search view state
        /// </summary>
        internal void ResetAutoCompleteSearchViewState()
        {
            DisplayAutocompleteMLStaticPage = false;
            DisplayLowConfidence = dynamoViewModel.PreferenceSettings.HideNodesBelowSpecificConfidenceLevel && dynamoViewModel.PreferenceSettings.DefaultNodeAutocompleteSuggestion == NodeAutocompleteSuggestion.MLRecommendation;
            AutocompleteMLMessage = string.Empty;
            AutocompleteMLTitle = string.Empty;
            FilteredResults = new List<NodeSearchElementViewModel>();
            FilteredHighConfidenceResults = new List<NodeSearchElementViewModel>();
            FilteredLowConfidenceResults = new List<NodeSearchElementViewModel>();
            searchElementsCache = new List<NodeSearchElementViewModel>();
        }





        /// <summary>
        /// Compare to low confidence threadhold defined by user can origanize the results into high and low confidence sections.
        /// </summary>
        /// <param name="results"></param>
        internal void OrganizeConfidenceSection(List<NodeSearchElementViewModel> results)
        {
            foreach (var result in results)
            {
                if (result.AutoCompletionNodeMachineLearningInfo.ConfidenceScore >= dynamoViewModel.PreferenceSettings.MLRecommendationConfidenceLevel)
                {
                    FilteredHighConfidenceResults = FilteredHighConfidenceResults.Append(result);
                }
                else
                {
                    FilteredLowConfidenceResults = FilteredLowConfidenceResults.Append(result);
                }
            }

            // Show low confidence section if there are some results under threshold and feature enabled
            DisplayLowConfidence = FilteredLowConfidenceResults.Any() && dynamoViewModel.PreferenceSettings.HideNodesBelowSpecificConfidenceLevel;

            if (!FilteredHighConfidenceResults.Any())
            {
                DisplayAutocompleteMLStaticPage = true;
                AutocompleteMLTitle = Resources.AutocompleteLowConfidenceTitle;
                AutocompleteMLMessage = Resources.AutocompleteLowConfidenceMessage;
                return;
            }

            // By default, show only the results which are above the threshold
            FilteredResults = dynamoViewModel.PreferenceSettings.HideNodesBelowSpecificConfidenceLevel ? FilteredHighConfidenceResults : results;
        }





        /// <summary>
        /// Show the low confidence ML results.
        /// </summary>
        internal void ShowLowConfidenceResults()
        {
            DisplayLowConfidence = false;
            DisplayAutocompleteMLStaticPage = false;
            IEnumerable<NodeSearchElementViewModel> allResults = FilteredHighConfidenceResults.Concat(FilteredLowConfidenceResults);
            FilteredResults = allResults;
            // Save the filtered results for search.
            searchElementsCache = FilteredResults.ToList();
        }

        // Full name and assembly name 
        internal NodeModelTypeId GetInfoFromTypeId(string typeId)
        {
            if (typeId.Contains(','))
            {
                var type = typeId.Split(',');
                return new NodeModelTypeId(type[0].Trim(), type[1].Trim());
            }

            return new NodeModelTypeId(typeId);
        }

        // Remove the digits at the end of the portname for variable input node
        private string ParseVariableInputPortName(string portName)
        {
            string pattern = @"\d+$";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(portName, string.Empty);
        }

        // Get the host name from the enum list.
        internal HostNames GetHostNameEnum(string HostName)
        {
            switch (HostName)
            {
                case string name when name.IndexOf("Revit", StringComparison.OrdinalIgnoreCase) >= 0:
                    return HostNames.Revit;
                case string name when name.IndexOf("Civil", StringComparison.OrdinalIgnoreCase) >= 0:
                    return HostNames.Civil3d;
                case string name when name.IndexOf("Alias", StringComparison.OrdinalIgnoreCase) >= 0:
                    return HostNames.Alias;
                case string name when name.IndexOf("FormIt", StringComparison.OrdinalIgnoreCase) >= 0:
                    return HostNames.FormIt;
                case string name when name.IndexOf("Steel", StringComparison.OrdinalIgnoreCase) >= 0:
                    return HostNames.AdvanceSteel;
                case string name when name.IndexOf("RSA", StringComparison.OrdinalIgnoreCase) >= 0:
                    return HostNames.RSA;
                default:
                    return HostNames.None;
            }
        }

        /// <summary>
        /// Key function to populate node autocomplete results to display
        /// </summary>
        internal void PopulateAutoCompleteCandidates()
        {
            if (PortViewModel == null) return;

            dynamoViewModel.CurrentSpaceViewModel.Model.NodeRemoved += NodeViewModel_Removed;
            ResetAutoCompleteSearchViewState();

            //Tracking Analytics when raising Node Autocomplete with the Object Types option selected.
            Analytics.TrackEvent(
                Actions.Show,
                Categories.NodeAutoCompleteOperations,
                nameof(NodeAutocompleteSuggestion.ObjectType));
            // Only call GetMatchingSearchElements() for object type match comparison
            var objectTypeMatchingElements = GetMatchingSearchElements().ToList();
            // If node match searchElements found, use default suggestions. 
            // These default suggestions will be populated based on the port type.
            if (!objectTypeMatchingElements.Any())
            {
                PopulateDefaultAutoCompleteCandidates();
            }
            else
            {
                FilteredResults = GetViewModelForNodeSearchElements(objectTypeMatchingElements);
            }

            // Save the filtered results for search.
            searchElementsCache = FilteredResults.ToList();
        }

        internal void PopulateDefaultAutoCompleteCandidates()
        {
            if (PortViewModel.PortModel.PortType == PortType.Input)
            {
                switch (PortViewModel.PortModel.GetInputPortType())
                {
                    case "int":
                        FilteredResults = DefaultResults.Where(e => e.Name == "Number Slider" || e.Name == "Integer Slider").ToList();
                        break;
                    case "double":
                        FilteredResults = DefaultResults.Where(e => e.Name == "Number Slider" || e.Name == "Integer Slider").ToList();
                        break;
                    case "string":
                        FilteredResults = DefaultResults.Where(e => e.Name == "String").ToList();
                        break;
                    case "bool":
                        FilteredResults = DefaultResults.Where(e => e.Name == "Boolean").ToList();
                        break;
                    default:
                        FilteredResults = DefaultResults.Where(e => e.Name == "String" || e.Name == "Number Slider" || e.Name == "Integer Slider" || e.Name == "Number" || e.Name == "Boolean");
                        break;
                }
            }
            else
            {
                FilteredResults = DefaultResults.Where(e => e.Name == "Watch" || e.Name == "Watch 3D" || e.Name == "Python Script").ToList();
            }
        }

        internal void OnNodeAutoCompleteWindowClosed()
        {
            dynamoViewModel.CurrentSpaceViewModel.Model.NodeRemoved -= NodeViewModel_Removed;
        }

        internal void NodeViewModel_Removed(NodeModel node)
        {
            ParentNodeRemoved?.Invoke(node);
        }

        /// <summary>
        /// Returns a IEnumberable of NodeSearchElementViewModel for respective NodeSearchElements.
        /// </summary>
        private IEnumerable<NodeSearchElementViewModel> GetViewModelForNodeSearchElements(List<NodeSearchElement> searchElementsCache)
        {
            return searchElementsCache.Select(e =>
            {
                var vm = new NodeSearchElementViewModel(e, this);
                vm.RequestBitmapSource += SearchViewModelRequestBitmapSource;
                return vm;
            });
        }

        /// <summary>
        /// Returns a NodeSearchElementViewModel for a NodeSearchElement
        /// </summary>
        private NodeSearchElementViewModel GetViewModelForNodeSearchElement(NodeSearchElement nodeSearchElement)
        {
            if (nodeSearchElement != null)
            {
                var vm = new NodeSearchElementViewModel(nodeSearchElement, this);
                vm.RequestBitmapSource += SearchViewModelRequestBitmapSource;
                return vm;
            }
            return null;
        }


        /// <summary>
        ///     Performs a search using the given string as query and subset, if provided.
        /// </summary>
        /// <returns> Returns a list with a maximum MaxNumSearchResults elements.</returns>
        /// <param name="search"> The search query </param>
        internal IEnumerable<NodeSearchElementViewModel> SearchNodeAutocomplete(string search)
        {

            return null;
        }

        /// <summary>
        /// Filters the matching node search elements based on user input in the search field. 
        /// </summary>
        internal void SearchAutoCompleteCandidates(string input)
        {
            if (PortViewModel == null) return;

            if (searchElementsCache.Any())
            {
                if (string.IsNullOrEmpty(input))
                {
                    FilteredResults = searchElementsCache;
                }
                else
                {


                    var luceneResults = SearchNodeAutocomplete(input);
                    var foundNodesModels = luceneResults.Select(x => x.Model);
                    var foundNodes = foundNodesModels.Select(MakeNodeSearchElementVM);

                    var filteredSearchElements = new List<NodeSearchElementViewModel>();

                    foreach (var node in foundNodes)
                    {
                        var matchingElement = searchElementsCache.FirstOrDefault(x => x.FullName.Equals(node.FullName));

                        if (matchingElement != null)
                        {
                            node.AutoCompletionNodeMachineLearningInfo = new AutoCompletionNodeMachineLearningInfo(matchingElement.AutoCompletionNodeMachineLearningInfo.DisplayIcon,
                                                                                                                   matchingElement.AutoCompletionNodeMachineLearningInfo.IsByRecommendation,
                                                                                                                   matchingElement.AutoCompletionNodeMachineLearningInfo.ConfidenceScore);
                            filteredSearchElements.Add(node);
                        }
                    }
                    FilteredResults = new List<NodeSearchElementViewModel>(filteredSearchElements).OrderBy(x => x.Name).ThenBy(x => x.Description);

                }
            }
        }

        /// <summary>
        /// Returns a collection of node search elements for nodes
        /// that output a type compatible with the port type if it's an input port.
        /// These search elements can belong to either zero touch, NodeModel or Builtin nodes.
        /// This method returns an empty collection if the input port type cannot be inferred or
        /// there are no matching nodes found for the type. Currently the match does not include
        /// rank information. For example Curve[] is matched as Curve. The results include types
        /// that would be valid using normal class inheritance rules.
        /// The resulting compatible search elements can be made to appear in the node autocomplete search dialog.
        /// </summary>
        /// <returns>collection of node search elements</returns>
        internal IEnumerable<NodeSearchElement> GetMatchingSearchElements()
        {
            var elements = new List<NodeSearchElement>();

            var portType = String.Empty;

            if (PortViewModel.PortModel.PortType == PortType.Input)
            {
                portType = PortViewModel.PortModel.GetInputPortType();
            }
            else if (PortViewModel.PortModel.PortType == PortType.Output)
            {
                portType = PortViewModel.PortModel.GetOutPortType();
                //if the custom node output name contains spaces, try using the first word.
                if (PortViewModel.PortModel.Owner is Graph.Nodes.CustomNodes.Function && portType.Any(char.IsWhiteSpace))
                {
                    portType = string.Concat(portType.TrimStart().TakeWhile(char.IsLetterOrDigit));
                }
            }

            //List of input types that are skipped temporarily, and will display list of default suggestions instead.
            var skippedInputTypes = new List<string>() { "var", "object", "string", "bool", "int", "double" };

            if (portType == null)
            {
                return elements;
            }

            var core = dynamoViewModel.Model.LibraryServices.LibraryManagementCore;

            //if inputPortType is an array, use just the typename
            ParseResult parseResult = null;
            try
            {
                parseResult = ParserUtils.ParseWithCore($"dummyName:{portType};", core);
            }
            catch (ProtoCore.BuildHaltException)
            { }

            var ast = parseResult?.CodeBlockNode.Children().FirstOrDefault() as IdentifierNode;
            //if parsing the type failed, revert to original string.
            portType = ast != null ? ast.datatype.Name : portType;
            //check if the input port return type is in the skipped input types list
            if (skippedInputTypes.Any(s => s == portType))
            {
                return elements;
            }

            //gather all ztsearchelements or nodemodel nodes that are visible in search and filter using inputPortType and zt return type name.
            var ztSearchElements = Model.Entries.OfType<ZeroTouchSearchElement>().Where(x => x.IsVisibleInSearch);
            var nodeModelSearchElements = Model.Entries.OfType<NodeModelSearchElement>().Where(x => x.IsVisibleInSearch);

            if (PortViewModel.PortModel.PortType == PortType.Input)
            {
                foreach (var ztSearchElement in ztSearchElements)
                {
                    //for now, remove rank from descriptors    
                    var returnTypeName = ztSearchElement.Descriptor.ReturnType.Name;

                    var descriptor = ztSearchElement.Descriptor;
                    if ((returnTypeName == portType) || DerivesFrom(portType, returnTypeName, core))
                    {
                        elements.Add(ztSearchElement);
                    }
                }

                // NodeModel nodes, match any output return type to inputport type name
                foreach (var element in nodeModelSearchElements)
                {
                    if (element.OutputParameters.Any(op => op == portType))
                    {
                        elements.Add(element);
                    }
                }
            }
            else if (PortViewModel.PortModel.PortType == PortType.Output)
            {
                foreach (var ztSearchElement in ztSearchElements)
                {
                    foreach (var inputParameter in ztSearchElement.Descriptor.Parameters.Select((value, index) => new { value, index }))
                    {
                        var ZTparamName = inputParameter.value.Type.Name ?? inputParameter.value.Type.ToString();
                        if (ZTparamName == portType || DerivesFrom(ZTparamName, portType, core))
                        {
                            ztSearchElement.AutoCompletionNodeElementInfo.PortToConnect = ztSearchElement.Descriptor.Type == FunctionType.InstanceMethod ? inputParameter.index + 1 : inputParameter.index;
                            elements.Add(ztSearchElement);
                            break;
                        }
                    }
                }

                // NodeModel nodes, match any output return type to inputport type name
                foreach (var element in nodeModelSearchElements)
                {
                    foreach (var inputParameter in element.InputParameters)
                    {
                        if (inputParameter.Item2 == portType)
                        {
                            elements.Add(element);
                        }
                    }
                }
            }

            var comparer = new NodeSearchElementComparer(portType, core);

            //first sort by type distance to input port type
            elements.Sort(comparer);
            //then sort by node library group (create, action, or query node)
            //this results in a list of elements with 3 major groups(create,action,query), each group is sub sorted into types and then sorted by name.
            return elements.OrderBy(x => x.Group).ThenBy(x => x.OutputParameters.FirstOrDefault().ToString()).ThenBy(x => x.Name);
        }

        /// <summary>
        /// Does typeb derive from typea
        /// </summary>
        /// <param name="typea"></param>
        /// <param name="typeb"></param>
        /// <param name="core"></param>
        /// <returns></returns>
        private bool DerivesFrom(string typea, string typeb, ProtoCore.Core core)
        {
            try
            {
                //TODO mirrors can be cached until new types are imported...
                var mirror1 = new ClassMirror(typea, core);
                var mirror2 = new ClassMirror(typeb, core);

                //TODO as we do this check we can cache the type distance...
                if (mirror2.GetClassHierarchy().Any(x => x.ClassName == mirror1.ClassName))
                {
                    //this is a derived type
                    return true;
                }
                return false;
            }
            catch
            {
                Debug.WriteLine($"failed to create class mirror for either {typea} or {typeb} during node autocomplete operation ");
                return false;
            }
        }

        /// <summary>
        /// Compares NodeSearchElements based on their typeDistance from a given type 'typeNameToCompareTo'
        /// </summary>
        internal class NodeSearchElementComparer : IComparer<NodeSearchElement>
        {
            private string typeNameToCompareTo;
            private ProtoCore.Core core;

            internal NodeSearchElementComparer(string typeNameToCompareTo, ProtoCore.Core core)
            {
                this.typeNameToCompareTo = typeNameToCompareTo;
                this.core = core;
            }

            public int Compare(NodeSearchElement x, NodeSearchElement y)
            {
                return CompareNodeSearchElementByTypeDistance(x, y, typeNameToCompareTo, core);
            }


            /// <summary>
            /// Compares two nodeSearchElements - general rules of the sort as follows:
            /// If all return types of the two searchElements are the same, they are equal.
            /// If any return type of both searchElements is an exact match for our input type, they are equal.
            /// If a searchElement is null, it is larger than the other element.
            /// If a single searchElement's return type list contains an exact match it is smaller.
            /// If the minimum type distance between a searchElements return types and our inputType than this searchElement is smaller. (closer)
            /// If the minimim type distances are the same the searchElements are equal.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="typeNameToCompareTo"></param>
            /// <param name="core"></param>
            /// <returns></returns>
            private int CompareNodeSearchElementByTypeDistance(NodeSearchElement x, NodeSearchElement y, string typeNameToCompareTo, ProtoCore.Core core)
            {
                /*
                 * -1	x precedes y.
                 0 x and y are equal
                 1	x is after y.
                 */
                var none = new string[] { Resources.NoneString };
                var xTypeNames = x.OutputParameters;
                var yTypeNames = y.OutputParameters;

                //if either element is a ztsearchElement then just use the type name (this strips off the rank)
                if (x is ZeroTouchSearchElement xzt)
                {
                    xTypeNames = new string[] { xzt.Descriptor.ReturnType.Name };
                }
                // for non ZT nodes, we don't have concrete return types, so we need to parse the typename. 
                else
                {
                    //if inputPortType is an array, lets just use the class name to match 
                    xTypeNames = xTypeNames.Select(type => (ParserUtils.ParseWithCore($"dummyName:{type};", core).
                    CodeBlockNode.Children().ElementAt(0) as TypedIdentifierNode).datatype.Name);
                }
                if (y is ZeroTouchSearchElement yzt)
                {
                    yTypeNames = new string[] { yzt.Descriptor.ReturnType.Name };
                }
                // for non ZT nodes, we don't have concrete return types, so we need to parse the typename. 
                else
                {
                    yTypeNames = yTypeNames.Select(type => (ParserUtils.ParseWithCore($"dummyName:{type};", core).
                    CodeBlockNode.Children().ElementAt(0) as TypedIdentifierNode).datatype.Name);
                }

                if (xTypeNames.SequenceEqual(yTypeNames))
                {
                    return 0;
                }

                // x and y are equal because both typeLists contain an exact match
                if (xTypeNames.Any(xType => xType == typeNameToCompareTo) && (yTypeNames.Any(yType => yType == typeNameToCompareTo)))
                {
                    return 0;
                }

                // null is further away, so x is at the end of list.
                if (xTypeNames.SequenceEqual(none))
                {
                    return 1;
                }
                // null is further away, so y is at the end of the list.
                if (yTypeNames.SequenceEqual(none))
                {
                    return -1;
                }

                // x precedes y because it contains an exact match
                if (xTypeNames.Any(xType => xType == typeNameToCompareTo))
                {
                    return -1;
                }
                //  y precedes x because it contains an exact match
                if (yTypeNames.Any(yType => yType == typeNameToCompareTo))
                {
                    return 1;
                }

                var xminDistance = xTypeNames.Select(name => GetTypeDistance(typeNameToCompareTo, name, core)).Min();
                var yminDistance = yTypeNames.Select(name => GetTypeDistance(typeNameToCompareTo, name, core)).Min();

                //if distance of x to currentSelectedType is greater than y distance
                //then x is further away
                if (xminDistance > yminDistance)
                {
                    return 1;
                }
                if (xminDistance == yminDistance)
                {
                    return 0;
                }
                // distance2 < distance 1
                // x precedes y
                return -1;
            }

            /// <summary>
            /// Return the type distance between two type names. 
            /// </summary>
            /// <param name="typea"></param>
            /// <param name="typeb"></param>
            /// <param name="core"></param>
            /// <returns>Will return int.MaxValue if no match can be found.
            /// Otherwise will return the distance between two types in class hierarchy.
            /// Will throw an exception if either type name is undefined.
            ///</returns>
            private static int GetTypeDistance(string typea, string typeb, ProtoCore.Core core)
            {
                //TODO - cache? Turn into params?
                var mirror1 = new ClassMirror(typea, core);
                var mirror2 = new ClassMirror(typeb, core);

                if (mirror1.ClassNodeID == mirror2.ClassNodeID)
                {
                    return 0;
                }

                var heirarchy = mirror2.GetClassHierarchy();
                var dist = 0;
                while (dist < heirarchy.Count())
                {
                    if (heirarchy.ElementAt(dist).ClassName == mirror1.ClassName)
                    {
                        return dist + 1;
                    }
                    dist++;
                }
                //if we can't find a match then dist should indicate that.
                return int.MaxValue;
            }

        }
    }
}
