using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VA_Leo.Pages
{
    public partial class Settings : Page
    {
        public static float vVoulme = Properties.Settings.Default.voiceVol;
        public static float sVoulme = Properties.Settings.Default.soundVol;

        private readonly MediaPlayer player = new MediaPlayer();

        public Settings()
        {
            InitializeComponent();

            devModeBox.IsChecked = Properties.Settings.Default.isDevModeTrue;
            minimizeToTrayBox.IsChecked = Properties.Settings.Default.isMinimizeToTrayTrue;
            autoRunBox.IsChecked = Properties.Settings.Default.isAutoRun;
            opacityBox.IsChecked = Properties.Settings.Default.allowOpacity;

            appStartBox.IsChecked = Properties.Settings.Default.allowProgrammsStart;
            browserStartBox.IsChecked = Properties.Settings.Default.allowBrowserStart;
            usingNetworkBox.IsChecked = Properties.Settings.Default.allowNetworkUsing;
            AIbox.IsChecked = Properties.Settings.Default.allowAI;
            computerControlbox.IsChecked = Properties.Settings.Default.allowComputerControl;

            voiceVolumeSlider.Value = Properties.Settings.Default.voiceVol;
            soundVolumeSlider.Value = Properties.Settings.Default.soundVol;

    }

        private void voiceVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vVoulme = (float)voiceVolumeSlider.Value;

            Properties.Settings.Default.voiceVol = vVoulme;
            Properties.Settings.Default.Save();
        }

        private void soundVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sVoulme = (float)soundVolumeSlider.Value;

            Properties.Settings.Default.soundVol = sVoulme;
            Properties.Settings.Default.Save();
        }

        private void devModeBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isDevModeTrue = true;
            Properties.Settings.Default.Save();
        }

        private void devModeBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isDevModeTrue = false;
            Properties.Settings.Default.Save();
        }

        private void minimizeToTrayBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isMinimizeToTrayTrue = true;
            Properties.Settings.Default.Save();
        }

        private void minimizeToTrayBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isMinimizeToTrayTrue = false;
            Properties.Settings.Default.Save();
        }

        private void appStartBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowProgrammsStart = true;
            Properties.Settings.Default.Save();
        }

        private void appStartBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowProgrammsStart = false;
            Properties.Settings.Default.Save();
        }

        private void browserStartBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowBrowserStart = true;
            Properties.Settings.Default.Save();
        }

        private void browserStartBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowBrowserStart = false;
            Properties.Settings.Default.Save();
        }

        private void usingNetworkBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowNetworkUsing = true;
            Properties.Settings.Default.Save();
        }

        private void usingNetworkBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowNetworkUsing = false;
            Properties.Settings.Default.Save();
        }

        private void AIboxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowAI = true;
            Properties.Settings.Default.Save();
        }

        private void AIboxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowAI = false;
            Properties.Settings.Default.Save();
        }

        private void AIBoxHelp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Этот параметр отвечает за активацию ИИ. Лео сможет отвечать на вопросы с помощью искусственного интеллекта (GPT-4)" +
                "\n\nОБРАТИТЕ ВНИМАНИЕ! Все вычисления ИИ производит на вашем устройстве для этого требуется GPU",
                    "Интеграция ИИ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void computerControlBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowComputerControl = true;
            Properties.Settings.Default.Save();
        }

        private void computerControlBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowComputerControl = false;
            Properties.Settings.Default.Save();
        }

        private void opacityBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowOpacity = true;
            Properties.Settings.Default.Save();
        }

        private void opacityBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowOpacity = false;
            Properties.Settings.Default.Save();
        }

        private RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

        private void addToAutoRun(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoRun = true;
            Properties.Settings.Default.Save();

            try
            {
                string dir = Environment.CurrentDirectory;
                dir += @"\Ассистент Лео.exe";
                reg.SetValue("LeoAssistan", dir);
            }
            catch
            {
                MessageBox.Show("Не удалось создать запись в реестре" +
                "\n\nКод ошибки: 02",
                    "Что-то пошло не так...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void removeToAutoRun(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoRun = false;
            Properties.Settings.Default.Save();

            try
            {
                reg.DeleteValue("LeoAssistan");
            }
            catch
            {
                MessageBox.Show("Не удалось изменить/удалить запись в реестре" +
                "\n\nКод ошибки: 03",
                    "Что-то пошло не так...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
