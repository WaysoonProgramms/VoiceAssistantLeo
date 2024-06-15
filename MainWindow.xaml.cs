using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Leo.Classes;
using Leo.PageModels;
using static Leo.PageModels.Chat;
using Control = System.Windows.Forms.Control;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Leo
{
    public partial class MainWindow
    {
        public static ObservableCollection<Messages>? ChatCollection { get; set; }
        public static bool MicAccess = true;
        //private readonly MediaPlayer _player = new();

        private static readonly ChatManager ChatManager = new();
        
        public MainWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.isDevModeTrue)
            {
                AllocConsole();
                consoleAuth();
            }

            getChat(this, null); // Сдлеано для инициализации коллекции с сообщениями
            getHome(this, null);

            Classes.Vosk.main();

            if (Properties.Settings.Default.isMuted)
            {
                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/mute.png"));
                TrayIconMuteBtn.Header = "Вкл. микрофон";
            }
            else
            {
                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/microphone.png"));
                TrayIconMuteBtn.Header = "Выкл. микрофон";
            }

            Classes.Vosk.update();
            
            ChatCollection = new ObservableCollection<Messages>();
            
            ChatManager.deserializeChat();
        }

        [DllImport("Kernel32")]
        private static extern void AllocConsole();

        [DllImport("Kernel32")]
        private static extern void FreeConsole();

        private static void consoleAuth()
        {
            Console.WriteLine(Properties.Resources.MainWindow_consoleAuth_welcomeMessage);

            var passChar = new List<char>();
            while (true)
            {
                var cki = Console.ReadKey(true);
                if (cki.Key != ConsoleKey.Enter)
                {
                    Console.Write(@"*");
                    passChar.Add(cki.KeyChar);
                }
                else
                {
                    break;
                }
            }
            var passStr = passChar.Aggregate("", (current, c) => current + c);

            if (passStr == "1234")
            {
                Console.WriteLine(Properties.Resources.MainWindow_consoleAuth_accessAllowed);
            } else
            {
                MessageBox.Show(Properties.Resources.MainWindow_consoleAuth_accessDenided,
                    Properties.Resources.devModeSing, MessageBoxButton.OK, MessageBoxImage.Error);
                FreeConsole();
                Properties.Settings.Default.isDevModeTrue = false;
                Properties.Settings.Default.Save();
            }
        }

        private void trayIconClick(object sender, RoutedEventArgs e)
        {
            if (Equals(sender, TrayIconChatBtn))
            {
                getChat(TrayIconChatBtn, null);
            }
            if (Equals(sender, TrayIconSettingsBtn))
            {
                getSettings(TrayIconSettingsBtn, null);
            }


            Show();
            WindowState = WindowState.Normal;

            opacityAnimation(Name, 0, 1, 0.3, 2);
        }

        private void trayIconMute(object sender, RoutedEventArgs e)
        {
            if (MicAccess == false)
            {
                Classes.Vosk.error1();
                return;
            }

            if (Properties.Settings.Default.isMuted)
            {
                Properties.Settings.Default.isMuted = false;
                Properties.Settings.Default.Save();

                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/microphone.png"));
                TrayIconMuteBtn.Header = "Выкл. микрофон";
            }
            else
            {
                Properties.Settings.Default.isMuted = true;
                Properties.Settings.Default.Save();

                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/mute.png"));
                TrayIconMuteBtn.Header = "Вкл. микрофон";
            }

            Classes.Vosk.update();
        }

        private void trayIconClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void movingWindow(object sender, MouseButtonEventArgs e) 
        {
            DragMove();     
        }

        private void opacityAnimation(string target, double at, double to, double time, int operation)
        {
            if (Properties.Settings.Default.allowOpacity)
            {
                var storyboardFade = new Storyboard();

                var animation = new DoubleAnimation(at, to, new Duration(TimeSpan.FromSeconds(time)));
                
                switch (operation)
                {
                    case 0:
                        animation.Completed += closeApplication;
                        break;
                    case 1:
                        animation.Completed += hideApplication;
                        break;
                }

                Storyboard.SetTargetName(animation, target);
                Storyboard.SetTargetProperty(animation, new PropertyPath(MainWindow.OpacityProperty));
                storyboardFade.FillBehavior = FillBehavior.Stop;
                storyboardFade.Children.Add(animation);

                storyboardFade.Begin(this);
            } 
            else
            {
                switch (operation)
                {
                    case 0:
                        Close();
                        break;
                    case 1:
                        Hide();
                        break;
                }
            }
        }

        private void closeApplication(object? sender, EventArgs e)
        {
            TaskbarIcon.Visibility = Visibility.Hidden;
            Close();
        }

        private void hideApplication(object? sender, EventArgs e)
        {
            Hide();
        }

        private void closeWindow(object sender, MouseButtonEventArgs e)
        {
            opacityAnimation(this.Name, 1, 0, 0.3, Properties.Settings.Default.isMinimizeToTrayTrue ? 1 : 0);
        }

        private void mute(object sender, MouseButtonEventArgs? e)
        {
            if (MicAccess == false)
            {
                Classes.Vosk.error1();
                return;
            }

            if (Properties.Settings.Default.isMuted)
            {
                Properties.Settings.Default.isMuted = false;
                Properties.Settings.Default.Save();

                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/microphone.png"));
            } else
            {
                Properties.Settings.Default.isMuted = true;
                Properties.Settings.Default.Save();

                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/mute.png"));
            }

            Classes.Vosk.update();
        }

        private void minimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void getHome(object sender, MouseButtonEventArgs? e)
        {
            removeMarkers();
            MainFrame.Content = new Home();
            HomeBtnMarker.Opacity = 1;
        }
        private void getSettings(object sender, MouseButtonEventArgs? e)
        {
            removeMarkers();
            MainFrame.Content = new Settings();
            SettingsBtnMarker.Opacity = 1;
        }
        private void getChat(object sender, MouseButtonEventArgs? e)
        { 
            removeMarkers();
            MainFrame.Content = new Chat();
            ChatBtnMarker.Opacity = 1;
        }
        private void getAbout(object sender, MouseButtonEventArgs? e)
        {
            removeMarkers();
            MainFrame.Content = new About();
            AboutBtnMarker.Opacity = 1;
        }
        private void copyrightLink(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
            { FileName = "https://raw.githubusercontent.com/WaysoonProgramms/VoiceAssistantLeo/master/LICENSE", UseShellExecute = true });
        }

        private void removeMarkers()
        {
            HomeBtnMarker.Opacity = 0;
            SettingsBtnMarker.Opacity = 0;
            ChatBtnMarker.Opacity = 0;
            AboutBtnMarker.Opacity = 0;
        }

        private void homeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(HomeBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            HomeBtnFillMarker.Opacity = 0.1;
        }

        private void homeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(HomeBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            HomeBtnFillMarker.Opacity = 0;
        }

        private void settingsBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(SettingsBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            SettingsBtnFillMarker.Opacity = 0.1;
        }

        private void settingsBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(SettingsBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            SettingsBtnFillMarker.Opacity = 0;
        }

        private void chatBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(ChatBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            ChatBtnFillMarker.Opacity = 0.1;
        }

        private void chatBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(ChatBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            ChatBtnFillMarker.Opacity = 0;
        }

        private void aboutBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(AboutBtnFillMarker.Name, 0, 0.1, 0.1, 2);
            AboutBtnFillMarker.Opacity = 0.1;
        }

        private void aboutBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(AboutBtnFillMarker.Name, 0.1, 0, 0.1, 2);
            AboutBtnFillMarker.Opacity = 0;
        }

        private void closeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(CloseBackgound.Name, 0, 1, 0.1, 2);
            CloseBackgound.Opacity = 1;
        }

        private void closeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(CloseBackgound.Name, 1, 0, 0.1, 2);
            CloseBackgound.Opacity = 0;
        }

        private void minimizeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(MinimizeBackgound.Name, 0, 0.2, 0.1, 2);
            MinimizeBackgound.Opacity = 0.2;
        }

        private void minimizeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(MinimizeBackgound.Name, 0.2, 0, 0.1, 2);
            MinimizeBackgound.Opacity = 0;
        }

        private void muteBtnMouseEnter(object sender, MouseEventArgs e)
        {
            opacityAnimation(MuteBackgound.Name, 0, 0.2, 0.1, 2);
            MuteBackgound.Opacity = 0.2;
        }

        private void muteBtnMouseLeave(object sender, MouseEventArgs e)
        {
            opacityAnimation(MuteBackgound.Name, 0.2, 0, 0.1, 2);
            MuteBackgound.Opacity = 0;
        }

        private void copyrightMouseEnter(object sender, MouseEventArgs e)
        {
            CopyrightLine.Opacity = 1;
        }

        private void copyrightMouseLeave(object sender, MouseEventArgs e)
        {
            CopyrightLine.Opacity = 0;
        }

        private void hotKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                mute(this, null);
            }

            if (e.Key == Key.L && Control.ModifierKeys == Keys.Control)
            {
                Process.Start("explorer.exe", ".\\logs");
            }
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            opacityAnimation(Name, 0, 1, 0.3, 2);
        }

        // private void playVoice(string path)
        // {
        //     _player.Open(new Uri(path, UriKind.Relative));
        //     _player.Volume = Settings.sVoulme / 100.0f;
        //     _player.Play();
        // }
    }
}