using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Leo.Classes;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.PageModels
{
    public partial class Settings
    {
        public static float VoiceVolume = Properties.Settings.Default.voiceVol;
        public static float SoundVolume = Properties.Settings.Default.soundVol;

        private readonly MediaPlayer _player = new();
        private readonly Logger _logger = new();
        private readonly MessageBox _messageBox = new();

        public Settings()
        {
            InitializeComponent();
            
            // Инициализация настроек:
            // Функции
            DevModeBox.IsChecked = Properties.Settings.Default.isDevModeTrue;
            MinimizeToTrayBox.IsChecked = Properties.Settings.Default.isMinimizeToTrayTrue;
            AutoRunBox.IsChecked = Properties.Settings.Default.isAutoRun;
            OpacityBox.IsChecked = Properties.Settings.Default.allowOpacity;
            
            // Правила
            AppStartBox.IsChecked = Properties.Settings.Default.allowProgrammsStart;
            BrowserStartBox.IsChecked = Properties.Settings.Default.allowBrowserStart;
            UsingNetworkBox.IsChecked = Properties.Settings.Default.allowNetworkUsing;
            AiBox.IsChecked = Properties.Settings.Default.allowAI;
            ComputerControlBox.IsChecked = Properties.Settings.Default.allowComputerControl;

            // Звук
            VoiceVolumeSlider.Value = Properties.Settings.Default.voiceVol;
            SoundVolumeSlider.Value = Properties.Settings.Default.soundVol;

        }
        
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateShortcut(string shortcutFilePath, string targetFilePath);

        private void voiceVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VoiceVolume = (float)VoiceVolumeSlider.Value;

            Properties.Settings.Default.voiceVol = VoiceVolume;
            Properties.Settings.Default.Save();
        }

        private void voiceVolumeTest(object sender, MouseEventArgs e)
        {
            _player.Open(new Uri(@".\voices\test.wav", UriKind.Relative));
            _player.Volume = Settings.VoiceVolume / 100.0f;
            _player.Play();
        }

        private void soundVolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SoundVolume = (float)SoundVolumeSlider.Value;

            Properties.Settings.Default.soundVol = SoundVolume;
            Properties.Settings.Default.Save();
        }

        private void soundVolumeTest(object sender, MouseEventArgs e)
        {
            _player.Open(new Uri(@".\sounds\start.wav", UriKind.Relative));
            _player.Volume = Settings.SoundVolume / 100.0f;
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
            _messageBox.showMessage(Properties.Resources.MessageBox_errorSign,
                    Properties.Resources.Settings_AIBox_Help, MessageBox.MessageBoxType.Info, MessageBox.MessageBoxButtons.Ok);
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
            var message = "Не удалось создать запись в реестре\n\nКод ошибки: 02";
            var label = "Что-то пошло не так...";
            
            try
            {
                const string path = @"%AppData%\Microsoft\Windows\Start Menu\Programs\Startup";
                Environment.ExpandEnvironmentVariables(path);

                const string shortcutFilePath = "Ассистент Лео.lnk";
                var targetFilePath = Environment.CurrentDirectory + @"\Ассистент Лео.exe";

                CreateShortcut(shortcutFilePath, targetFilePath);
            }
            catch
            { 
                
                _messageBox.showMessage(message, label, MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
                
                _logger.error("Leo was unable to add himself to autostart.");
            }
        }

        private void removeToAutoRun(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoRun = false;
            Properties.Settings.Default.Save();
            var message = "Не удалось изменить/удалить запись в реестре\n\nКод ошибки: 03";
            var label = "Что-то пошло не так...";
            
            try
            {
                
            }
            catch
            {
                _messageBox.showMessage(message, label, MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);

                _logger.error("Leo was unable to remove himself from startup.");
            }
        }
    }
}
