﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace PsGet.Installer.Models {
    public class ChooseInstallLocationViewModel : INotifyPropertyChanged {
        private ObservableCollection<InstallPath> _installPaths;
        private InstallPath _selectedPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<InstallPath> InstallPaths {
            get { return _installPaths; }
            set {
                if (_installPaths != value) {
                    _installPaths = value;
                    NotifyPropertyChanged("InstallPaths");
                }
            }
        }

        public InstallPath SelectedPath {
            get { return _selectedPath; }
            set {
                if (_selectedPath != value) {
                    _selectedPath = value;
                    NotifyPropertyChanged("SelectedPath");
                    NextCommand.CommandCanExecute = _selectedPath != null;
                }
            }
        }

        public DelegateCommand NextCommand { get; set; }

        public ChooseInstallLocationViewModel() {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                InstallPaths = new ObservableCollection<InstallPath>(new[] {
                    new InstallPath(@"C:\SystemDir", isSystemPathIn64BitOs: true),
                    new InstallPath(@"D:\Bar", isSystemPathIn64BitOs: false),
                    new InstallPath(@"E:\Baz", isSystemPathIn64BitOs: false)
                });
            }
            else {
                InstallPaths = new ObservableCollection<InstallPath>(
                    App.Current.Shell
                               .AddScript("$env:PsModulePath.Split(';')")
                               .Invoke()
                               .Select(o => new InstallPath(o.ToString())));
            }

            NextCommand = new DelegateCommand(_ => {
                App.Current.NavigationService.Navigate(new Uri("/InstallingView.xaml", UriKind.RelativeOrAbsolute), SelectedPath.Path);
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
