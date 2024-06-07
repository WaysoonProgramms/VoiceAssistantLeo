using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VA_Leo.Classes;

namespace VA_Leo.Pages
{
    public partial class Settings
    {
        public static float vVoulme = Properties.Settings.Default.voiceVol;
        public static float sVoulme = Properties.Settings.Default.soundVol;

        private readonly MediaPlayer _player = new();
        private readonly Logger _logger = new();

        public Settings()
        {
            InitializeComponent();
            
            // Функции
            DevModeBox.IsChecked = Properties.Settings.Default.isDevModeTrue;
            MinimizeToTrayBox.IsChecked = Properties.Settings.Default.isMinimizeToTrayTrue;
            AutoRunBox.IsChecked = Properties.Settings.Default.isAutoRun;
            OpacityBox.IsChecked = Properties.Settings.Default.allowOpacity;
            
            // Правила
            AppStartBox.IsChecked = Properties.Settings.Default.allowProgrammsStart;
            BrowserStartBox.IsChecked = Properties.Settings.Default.allowBrowserStart;
            UsingNetworkBox.IsChecked = Properties.Settings.Default.allowNetworkUsing;
            AIbox.IsChecked = Properties.Settings.Default.allowAI;
            ComputerControlbox.IsChecked = Properties.Settings.Default.allowComputerControl;

            // Звук
            VoiceVolumeSlider.Value = Properties.Settings.Default.voiceVol;
            SoundVolumeSlider.Value = Properties.Settings.Default.soundVol;

        }
        
        [DllImport("shell32.dll")]
        private static extern bool CreateShortcut(string shortcutFilePath, string targetFilePath);

        private void voiceVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vVoulme = (float)VoiceVolumeSlider.Value;

            Properties.Settings.Default.voiceVol = vVoulme;
            Properties.Settings.Default.Save();
        }

        private void voiceVolumeTest(object sender, MouseEventArgs e)
        {
            _player.Open(new Uri(@".\voices\test.wav", UriKind.Relative));
            _player.Volume = Settings.vVoulme / 100.0f;
            _player.Play();
        }

        private void soundVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sVoulme = (float)SoundVolumeSlider.Value;

            Properties.Settings.Default.soundVol = sVoulme;
            Properties.Settings.Default.Save();
        }

        private void soundVolumeTest(object sender, MouseEventArgs e)
        {
            _player.Open(new Uri(@".\sounds\start.wav", UriKind.Relative));
            _player.Volume = Settings.sVoulme / 100.0f;
            _player.Play();
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

        private void aiBoxChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowAI = true;
            Properties.Settings.Default.Save();
        }

        private void aiBoxUnchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.allowAI = false;
            Properties.Settings.Default.Save();
        }

        private void aiBoxHelp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(Properties.Resources.Settings_AIBox_Help,
                    Properties.Resources.Settings_AIBox_Sing, MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void addToAutoRun(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoRun = true;
            Properties.Settings.Default.Save();

            try
            {
                string appdt = @"%AppData%\Microsoft\Windows\Start Menu\Programs\Startup";
                Environment.ExpandEnvironmentVariables(appdt);

                string shortcutFilePath = "Ассистент Лео.lnk";
                string targetFilePath = Environment.CurrentDirectory + @"\Ассистент Лео.exe";

                CreateShortcut(shortcutFilePath, targetFilePath);
            }
            catch
            {
                MessageBox.Show("Не удалось создать запись в реестре" +
                "\n\nКод ошибки: 02",
                    "Что-то пошло не так...", MessageBoxButton.OK, MessageBoxImage.Error);
                
                _logger.error("Leo was unable to add himself to autostart.");
            }
        }

        private void removeToAutoRun(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoRun = false;
            Properties.Settings.Default.Save();

            try
            {
                
            }
            catch
            {
                MessageBox.Show("Не удалось изменить/удалить запись в реестре" +
                "\n\nКод ошибки: 03",
                    "Что-то пошло не так...", MessageBoxButton.OK, MessageBoxImage.Error);

                _logger.error("Leo was unable to remove himself from startup.");
            }
        }
    }
}
