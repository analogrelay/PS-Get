using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace PsGet.Installer {
    /// <summary>
    /// Interaction logic for RootWindow.xaml
    /// </summary>
    public partial class RootWindow : NavigationWindow {
        public RootWindow() {
            InitializeComponent();
            NavigationService.Navigated += new System.Windows.Navigation.NavigatedEventHandler(NavigationService_Navigated);
        }

        void NavigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e) {
            // Pass the extra data along
            Page target = e.Content as Page;
            if (target != null) {
                foreach (var reciever in target.Resources.Values.OfType<INavigationStateReciever>()) {
                    reciever.Recieve(e.ExtraData);
                }
            }
        }
    }
}
