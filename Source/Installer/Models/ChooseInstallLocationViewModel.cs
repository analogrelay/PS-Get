using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace PsGet.Installer.Models {
    public class ChooseInstallLocationViewModel : INotifyPropertyChanged {
        private ObservableCollection<string> _installPaths;
        private string _selectedPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> InstallPaths {
            get { return _installPaths; }
            set {
                if (_installPaths != value) {
                    _installPaths = value;
                    NotifyPropertyChanged("InstallPaths");
                }
            }
        }

        public string SelectedPath {
            get { return _selectedPath; }
            set {
                if (_selectedPath != value) {
                    _selectedPath = value;
                    NotifyPropertyChanged("SelectedPath");
                    NextCommand.CommandCanExecute = !String.IsNullOrEmpty(_selectedPath);
                }
            }
        }

        public DelegateCommand NextCommand { get; set; }

        public ChooseInstallLocationViewModel() {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                InstallPaths = new ObservableCollection<string>(new [] {
                    @"C:\Foo\Bar",
                    @"D:\Bar",
                    @"E:\Baz"
                });
            }
            else {
                InstallPaths = new ObservableCollection<string>(
                    App.Current.Shell
                               .AddScript("$env:PsModulePath.Split(';')")
                               .Invoke()
                               .Select(o => o.ToString()));
            }

            NextCommand = new DelegateCommand(_ => {
                App.Current.NavigationService.Navigate(new Uri("/InstallingView.xaml", UriKind.RelativeOrAbsolute), SelectedPath);
            }, canExecute: false);
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
