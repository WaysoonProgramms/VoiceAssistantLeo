using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace VA_Leo
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public static float vVoulme = Properties.Settings.Default.voiceVol;

        public Settings()
        {
            InitializeComponent();

            devModeBox.IsChecked = Properties.Settings.Default.isDevModeTrue;
            minimizeToTrayBox.IsChecked = Properties.Settings.Default.isMinimizeToTrayTrue;

            appStartBox.IsChecked = Properties.Settings.Default.allowProgrammsStart;
            browserStartBox.IsChecked = Properties.Settings.Default.allowBrowserStart;
            usingNetworkBox.IsChecked = Properties.Settings.Default.allowNetworkUsing;
            AIbox.IsChecked = Properties.Settings.Default.allowAI;
            PCbox.IsChecked = Properties.Settings.Default.allowPC;

            voiceVolumeSlider.Value = Properties.Settings.Default.voiceVol;

    }

        private void voiceVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vVoulme = (float)voiceVolumeSlider.Value;

            Properties.Settings.Default.voiceVol = vVoulme;
            Properties.Settings.Default.Save();
        }

        private void devModeBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isDevModeTrue = true;
            Properties.Settings.Default.Save();
        }

        private void devModeBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isDevModeTrue = false;
            Properties.Settings.Default.Save();
        }

        private void minimizeToTrayBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isMinimizeToTrayTrue = true;
            Properties.Settings.Default.Save();
        }

        private void minimizeToTrayBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isMinimizeToTrayTrue = false;
            Properties.Settings.Default.Save();
        }

        private void appStartBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowProgrammsStart = true;
            Properties.Settings.Default.Save();
        }

        private void appStartBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowProgrammsStart = false;
            Properties.Settings.Default.Save();
        }

        private void browserStartBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowBrowserStart = true;
            Properties.Settings.Default.Save();
        }

        private void browserStartBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowBrowserStart = false;
            Properties.Settings.Default.Save();
        }

        private void usingNetworkBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowNetworkUsing = true;
            Properties.Settings.Default.Save();
        }

        private void usingNetworkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowNetworkUsing = false;
            Properties.Settings.Default.Save();
        }

        private void AIbox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowAI = true;
            Properties.Settings.Default.Save();
        }

        private void AIbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowAI = false;
            Properties.Settings.Default.Save();
        }

        private void PCbox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowPC = true;
            Properties.Settings.Default.Save();
        }

        private void PCbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowPC = false;
            Properties.Settings.Default.Save();
        }
    }
}
