using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsGet.Installer.Models;

namespace PsGet.Installer
{
    /// <summary>
    /// Interaction logic for ChooseInstallLocationView.xaml
    /// </summary>
    public partial class ChooseInstallLocationView : Page
    {
        public ChooseInstallLocationView()
        {
            this.InitializeComponent();

        }

        private void comboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dockPanel != null) {
                InstallPath selected = comboBox.SelectedValue as InstallPath;
                if (selected != null) {
                    if (selected.IsSystemPathIn64BitOs) {
                        VisualStateManager.GoToElementState(LayoutRoot,
                                                     "Visible",
                                                     useTransitions: true);
                    }
                    else {
                        VisualStateManager.GoToElementState(LayoutRoot,
                                                     "Invisible",
                                                     useTransitions: true);
                    }
                }
                else {
                    VisualStateManager.GoToElementState(LayoutRoot,
                                                 "Invisible",
                                                 useTransitions: true);
                }
            }
        }
    }
}