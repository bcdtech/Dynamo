using Dynamo.Configuration;
using Dynamo.Interfaces;
using Dynamo.ViewModels;
using Dynamo.Visualization;

namespace Dynamo.Wpf.ViewModels
{
    public class RenderPackageFactoryViewModel : ViewModelBase
    {
        private readonly IRenderPackageFactory factory;

        public IRenderPackageFactory Factory
        {
            get { return factory; }
        }

        public bool ShowEdges
        {
            get { return factory.TessellationParameters.ShowEdges; }
            set
            {
                if (factory.TessellationParameters.ShowEdges == value) return;
                factory.TessellationParameters.ShowEdges = value;
                RaisePropertyChanged("ShowEdges");
            }
        }

        public bool UseRenderInstancing
        {
            get { return factory.TessellationParameters.UseRenderInstancing; }
            set
            {
                if (factory.TessellationParameters.UseRenderInstancing == value) return;
                factory.TessellationParameters.UseRenderInstancing = value;
                RaisePropertyChanged("UseRenderInstancing");
            }
        }

        public int MaxTessellationDivisions
        {
            get { return factory.TessellationParameters.MaxTessellationDivisions; }
            set
            {
                if (factory.TessellationParameters.MaxTessellationDivisions == value) return;

                factory.TessellationParameters.MaxTessellationDivisions = value;
                if (value >= 8 && value <= 12)
                    factory.TessellationParameters.Tolerance = 0;
                else
                    factory.TessellationParameters.Tolerance = -1;

                RaisePropertyChanged("MaxTessellationDivisions");
            }
        }

        // TODO: Simplify this constructor once IPreferences and IRenderPrecisionPreference have been consolidated in 3.0 (DYN-1699)
        public RenderPackageFactoryViewModel(IPreferences preferenceSettings)
        {
            var ps = preferenceSettings as PreferenceSettings;


        }
    }
}
