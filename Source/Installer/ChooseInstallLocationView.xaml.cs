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

namespace PsGet.Installer {
    /// <summary>
    /// Interaction logic for ChooseInstallLocationView.xaml
    /// </summary>
    public partial class ChooseInstallLocationView : Page {
        public ChooseInstallLocationView() {
            this.InitializeComponent();
        }

        private void comboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            InstallPath selected = comboBox.SelectedValue as InstallPath;
            if (selected != null) {
                if (warningPanel != null) {
                    VisualStateManager.GoToElementState(
                        warningPanel,
                        selected.IsSystemPathIn64BitOs ? WarningVisible.Name : WarningInvisible.Name,
                        useTransitions: true);
                }
                if (elevatePanel != null) {
                    VisualStateManager.GoToElementState(
                        elevatePanel,
                        selected.RequiresElevation ? ElevateVisible.Name : ElevateInvisible.Name,
                        useTransitions: true);
                }
            }
            else {
                if (warningPanel != null) {
                    VisualStateManager.GoToElementState(
                        warningPanel,
                        WarningInvisible.Name,
                        useTransitions: true);
                }
                if (warningPanel != null) {
                    VisualStateManager.GoToElementState(
                        elevatePanel,
                        ElevateInvisible.Name,
                        useTransitions: true);
                }
            }
        }
    }
}