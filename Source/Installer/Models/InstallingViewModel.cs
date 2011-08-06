using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using NuGet;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using PsGet.Installer.Services;

namespace PsGet.Installer.Models {
    public class InstallingViewModel : INotifyPropertyChanged, INavigationStateReciever {
        private string _operation;
        private SynchronizationContext _context;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Operation {
            get {
                return _operation;}
            set {
                if (_operation != value) {
                    _operation = value;
                    NotifyPropertyChanged("Operation");
                }}
        }

        public string Destination { get; set; }

        public InstallingViewModel()
            : this(new DispatcherSynchronizationContext()) {
        }

        public InstallingViewModel(SynchronizationContext context) {
            _context = context;
        }

        public void Recieve(object data) {
            Destination = data.ToString();
            Task.Factory.StartNew(DoInstall).ContinueWith(t => {
                if(t.IsFaulted) {
                    _context.Post(() => { throw t.Exception; });
                }
            });
        }

        private void DoInstall() {
            _context.Post(() => 
                Operation = "Installing PS-Get to " + Destination);

            PackageManager manager = new PackageManager(
                new ManifestResourcePackageRepository(),
                new DefaultPackagePathResolver(Destination, 
                                               useSideBySidePaths: false),
                new PhysicalFileSystem(Destination));

            manager.PackageInstalling +=
                (_, args) =>
                    _context.Post(() => 
                        Operation = String.Format("Installing {0}", 
                                                  args.Package.GetFullName()));
            manager.PackageInstalled +=
                (_, args) =>
                    _context.Post(() => 
                        Operation = String.Format("Installed {0}", 
                                                  args.Package.GetFullName()));

            manager.InstallPackage("PS-Get");

            _context.Post(() =>
                App.Current
                   .NavigationService
                   .Navigate(new Uri("/Completed.xaml",
                                     UriKind.RelativeOrAbsolute)));
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
