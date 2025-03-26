using System.Windows;

namespace Dynamo.UI
{


    public static class SharedDictionaryManager
    {
        private static ResourceDictionary _dynamoModernDictionary;
        private static ResourceDictionary _dataTemplatesDictionary;
        private static ResourceDictionary _dynamoColorsAndBrushesDictionary;
        private static ResourceDictionary _dynamoConvertersDictionary;
        private static ResourceDictionary _dynamoTextDictionary;
        private static ResourceDictionary _menuStyleDictionary;
        private static ResourceDictionary _toolbarStyleDictionary;
        private static ResourceDictionary _connectorsDictionary;
        private static ResourceDictionary _portsDictionary;
        private static ResourceDictionary _sidebarGridDictionary;
        private static ResourceDictionary outPortsDictionary;
        private static ResourceDictionary inPortsDictionary;
        private static ResourceDictionary _liveChartDictionary;

        public static string ThemesDirectory
        {
            get
            {
                return @"pack://application:,,,/Themes/Modern/";
            }
        }
        public static ResourceDictionary Resoures { get; internal set; } = null!;

        //public static Uri DynamoModernDictionaryUri
        //{
        //    get { return new Uri(ThemesDirectory + "Generic.xaml"); }
        //}

        //public static Uri DataTemplatesDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "DataTemplates.xaml")); }
        //}

        //public static Uri DynamoColorsAndBrushesDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "DynamoColorsAndBrushes.xaml")); }
        //}

        //public static Uri DynamoConvertersDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "DynamoConverters.xaml")); }
        //}

        //public static Uri DynamoTextDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "DynamoText.xaml")); }
        //}

        //public static Uri MenuStyleDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "MenuStyleDictionary.xaml")); }
        //}

        //public static Uri ToolbarStyleDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "ToolbarStyleDictionary.xaml")); }
        //}

        //public static Uri ConnectorsDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "Connectors.xaml")); }
        //}

        //public static Uri PortsDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "Ports.xaml")); }
        //}

        //public static Uri OutPortsDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "OutPorts.xaml")); }
        //}

        //public static Uri InPortsDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "InPorts.xaml")); }
        //}

        //public static Uri SidebarGridDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "SidebarGridStyleDictionary.xaml")); }
        //}

        //public static Uri LiveChartsDictionaryUri
        //{
        //    get { return new Uri(Path.Combine(ThemesDirectory, "LiveChartsStyle.xaml")); }
        //}



        public static ResourceDictionary DynamoModernDictionary => Resoures;





        //public static ResourceDictionary DynamoColorsAndBrushesDictionary
        //{
        //    get
        //    {
        //        return _dynamoColorsAndBrushesDictionary ??
        //               (_dynamoColorsAndBrushesDictionary = new ResourceDictionary() { Source = DynamoColorsAndBrushesDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary DynamoConvertersDictionary
        //{
        //    get
        //    {
        //        return _dynamoConvertersDictionary ??
        //               (_dynamoConvertersDictionary = new ResourceDictionary() { Source = DynamoConvertersDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary DynamoTextDictionary
        //{
        //    get
        //    {
        //        return _dynamoTextDictionary ??
        //               (_dynamoTextDictionary = new ResourceDictionary() { Source = DynamoTextDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary MenuStyleDictionary
        //{
        //    get
        //    {
        //        return _menuStyleDictionary ??
        //               (_menuStyleDictionary = new ResourceDictionary() { Source = MenuStyleDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary ToolbarStyleDictionary
        //{
        //    get
        //    {
        //        return _toolbarStyleDictionary ??
        //               (_toolbarStyleDictionary = new ResourceDictionary() { Source = ToolbarStyleDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary ConnectorsDictionary
        //{
        //    get
        //    {
        //        return _connectorsDictionary ??
        //               (_connectorsDictionary = new ResourceDictionary() { Source = ConnectorsDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary OutPortsDictionary
        //{
        //    get
        //    {
        //        return outPortsDictionary ?? (outPortsDictionary = new ResourceDictionary() { Source = OutPortsDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary InPortsDictionary
        //{
        //    get
        //    {
        //        return inPortsDictionary ?? (inPortsDictionary = new ResourceDictionary() { Source = InPortsDictionaryUri });
        //    }
        //}

        //public static ResourceDictionary SidebarGrid
        //{
        //    get
        //    {
        //        return _sidebarGridDictionary ?? (_sidebarGridDictionary = new ResourceDictionary() { Source = SidebarGridDictionaryUri });
        //    }
        //}
    }
}
