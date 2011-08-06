using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace PsGet.Installer.Services {
    [MarkupExtensionReturnType(typeof(ImageSource))]
    public class EmbeddedResourceMarkupExtension : MarkupExtension {
        public string ResourceName { get; set; }

        public EmbeddedResourceMarkupExtension() { }

        public EmbeddedResourceMarkupExtension(string resourceName) {
            ResourceName = resourceName;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return BitmapFrame.Create(
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(ResourceName));
        }
    }
}
