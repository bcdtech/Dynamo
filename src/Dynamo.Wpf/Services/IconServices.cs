using Dynamo.Engine;
using Dynamo.Interfaces;
using Dynamo.Utilities;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Media;

namespace Dynamo.Wpf.Services
{
    public class IconServices
    {
        private readonly IPathManager pathManager;
        private Dictionary<Assembly, IconWarehouse> warehouses =
            new Dictionary<Assembly, IconWarehouse>();

        internal IconServices(IPathManager pathManager)
        {
            this.pathManager = pathManager;
        }

        internal IconWarehouse GetForAssembly(string assemblyPath, bool useAdditionalPaths = true)
        {
            var libraryCustomization = LibraryCustomizationServices.GetForAssembly(assemblyPath, pathManager, useAdditionalPaths);
            if (libraryCustomization == null)
            {
                libraryCustomization = new LibraryCustomization(LoadFrom(assemblyPath), null);
            }

            var assembly = libraryCustomization.ResourceAssembly;
            if (assembly == null)
                return null;

            if (!warehouses.ContainsKey(assembly))
                warehouses[assembly] = new IconWarehouse(assembly, assemblyPath.EndsWith(".dll") ? null : assemblyPath);

            return warehouses[assembly];
        }
        Assembly LoadFrom(string assemblyPath)
        {

            try
            {
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                else
                {
                    return Assembly.GetExecutingAssembly();
                }
            }
            catch
            {
                return Assembly.GetExecutingAssembly();
            }
        }
    }

    public class IconWarehouse
    {
        private Dictionary<string, ImageSource> cachedIcons =
            new Dictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase);

        private readonly string assemblyName;
        private readonly Assembly resourceAssembly;

        private const string imagesSuffix = "Images";
        string? resourceCatagory = null;
        string resourceBaseName = "";
        internal IconWarehouse(Assembly resAssembly, string? resourceCatagory = null)
        {
            if (resAssembly != null)
            {
                resourceAssembly = resAssembly;

                // "Name" can be "Some.Assembly.Name.customization" with multiple dots, 
                // we are interested in removal of the "customization" part and the middle dots.
                var temp = resAssembly.GetName().Name.Split('.');
                if (temp.Length > 1)
                {
                    assemblyName = String.Join("", temp.Take(temp.Length - 1));

                }
                else
                {
                    assemblyName = temp[0];
                }
            }
            this.resourceCatagory = resourceCatagory;
            resourceBaseName = $"{assemblyName}{resourceCatagory}{imagesSuffix}";

        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        internal ImageSource LoadIconInternal(string iconKey)
        {
            ImageSource icon;
            if (cachedIcons.TryGetValue(iconKey, out icon))
                return icon;

            if (resourceAssembly == null)
            {
                cachedIcons.Add(iconKey, null);
                return null;
            }

            ResourceManager rm = new ResourceManager(resourceBaseName, resourceAssembly);

            object iconResource = null;
            try
            {
                iconResource = rm.GetObject(iconKey);
            }
            catch (Exception ex)
            {
                return null;
            }
            var bitmap = iconResource as Bitmap;
            if (bitmap == null)
            {
                cachedIcons.Add(iconKey, null);
                return null;
            }

            var bitmapSource = ResourceUtilities.ConvertToImageSource(bitmap);

            cachedIcons.Add(iconKey, bitmapSource);
            return bitmapSource;
        }
    }
}
