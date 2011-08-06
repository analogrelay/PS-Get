using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using NuGet;

namespace PsGet.Installer.Models {
    public class InstallingViewModel : INotifyPropertyChanged, INavigationStateReciever {
        private string _operation;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Operation {
            get { return _operation; }
            set {
                if (_operation != value) {
                    _operation = value;
                    NotifyPropertyChanged("Operation");
                }
            }
        }

        public string Destination { get; set; }
        
        public InstallingViewModel() {
        }

        public void Recieve(object data) {
            Destination = data.ToString();
            DoInstall();
        }

        private void DoInstall() {
            Operation = "Installing PS-Get to " + Destination;

            PackageManager manager = new PackageManager(
                new ManifestResourcePackageRepository(), 
                new DefaultPackagePathResolver(Destination, useSideBySidePaths: false), 
                new PhysicalFileSystem(Destination));

            manager.PackageInstalling += (_, args) => Operation = String.Format("Installing {0}", args.Package.GetFullName());
            manager.PackageInstalled += (_, args) => Operation = String.Format("Installed {0}", args.Package.GetFullName());

            manager.InstallPackage("PS-Get");
            
            App.Current.NavigationService.Navigate(new Uri("/Completed.xaml", UriKind.RelativeOrAbsolute));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        private void NotifyPropertyChanged(string name) {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }
    }
}
