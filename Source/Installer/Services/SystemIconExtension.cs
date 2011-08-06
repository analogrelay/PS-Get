using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Diagnostics;

namespace PsGet.Installer.Services {
    [MarkupExtensionReturnType(typeof(ImageSource))]
    public class SystemIconExtension : MarkupExtension {
        private ImageSource _value;
        public string IconName { get; set; }

        public SystemIconExtension() {
        }

        public SystemIconExtension(string iconName) {
            IconName = iconName;

        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return _value ?? CreateValue();
        }

        private object CreateValue() {
            if (String.IsNullOrEmpty(IconName)) {
                throw new ArgumentException("No Icon Name was specified");
            }

            PropertyInfo prop = typeof(SystemIcons).GetProperty(
                IconName, 
                BindingFlags.Public | BindingFlags.Static, 
                Type.DefaultBinder, 
                typeof(Icon), 
                Type.EmptyTypes, 
                new ParameterModifier[0]);

            if (prop == null) {
                throw new ArgumentException(
                    String.Format("Icon '{0}' does not exist as a static property on SystemIcons",
                                  IconName));
            }

            Icon ico = prop.GetValue(null, new object[0]) as Icon;
            Debug.Assert(ico != null);

            return _value = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, 
                                                                new Int32Rect(0, 0, ico.Width, ico.Height), 
                                                                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
