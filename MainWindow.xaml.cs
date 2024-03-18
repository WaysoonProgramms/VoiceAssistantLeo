﻿using System.Diagnostics;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace VA_Leo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.isDevModeTrue)
            {
                AllocConsole();
                consoleAuth();
            }

            getHome(this, null);
            Vosk.main();

            if (Properties.Settings.Default.isMuted == true)
            {
                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/microphone.png"));
            }
            else
            {
                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/mute.png"));
            }
        }

        WindowState prevState;

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        private void consoleAuth()
        {
            Console.WriteLine("Ассистент Лео 0.1 | Режим разработчика\n@WaysoonProgramms 2024\n\nВведите ключ авторизации:");

            List<char> passChar = new List<char>();
            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                    break;
                else
                {
                    Console.Write("*");
                    passChar.Add(cki.KeyChar);
                }
            }
            string passStr = null;
            foreach (char c in passChar)
                passStr += c;

            if (passStr == "1234")
            {
                Console.WriteLine("\n\nДОСТУП РАЗРЕШЕН!\nЗапуск приложения...\n");
            } else
            {
                MessageBox.Show("Неверный ключ авторизации! ДОСТУП ОГРАНИЧЕН!",
                   "Режим разработчика", MessageBoxButton.OK, MessageBoxImage.Error);
                FreeConsole();
                Properties.Settings.Default.isDevModeTrue = false;
                Properties.Settings.Default.Save();
            }
        }

        public static void close()
        {
            close();
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = prevState;
        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) 
        {
            this.Opacity = 0.8;
            this.DragMove();
            this.Opacity = 1;
        }

        private void closeWindow(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.isMinimizeToTrayTrue == true)
            {
                Hide();
            }
            else
            {
                DoubleAnimation animation;
                Storyboard storyboardFade = new Storyboard();

                animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.3)));
                animation.Completed += closeApplication;
                Storyboard.SetTargetName(animation, this.Name);
                Storyboard.SetTargetProperty(animation, new PropertyPath(MainWindow.OpacityProperty));
                storyboardFade.Children.Add(animation);

                storyboardFade.Begin(this);
            }
        }

        public void closeApplication(object sender, EventArgs e)
        {
            Close();
        }

        public void mute(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.isMuted == true)
            {
                Properties.Settings.Default.isMuted = false;
                Properties.Settings.Default.Save();

                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/mute.png"));
            } else
            {
                Properties.Settings.Default.isMuted = true;
                Properties.Settings.Default.Save();

                Mute.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Assets/images/microphone.png"));
            }
        }

        private void minimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void getHome(object sender, MouseButtonEventArgs e)
        {
            removeMarkers();
            MainFrame.Content = new Home();
            HomeBtnMarker.Opacity = 1;
        }
        private void getSettings(object sender, MouseButtonEventArgs e)
        {
            removeMarkers();
            MainFrame.Content = new Settings();
            SettingsBtnMarker.Opacity = 1;
        }
        private void getChat(object sender, MouseButtonEventArgs e)
        {
            removeMarkers();
            MainFrame.Content = new Chat();
            ChatBtnMarker.Opacity = 1;
        }
        private void getAbout(object sender, MouseButtonEventArgs e)
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

        private void HomeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            if (HomeBtnFillMarker.Opacity == 0)
            {
                HomeBtnFillMarker.Opacity = 0.1;
            }
        }

        private void HomeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            if (HomeBtnFillMarker.Opacity == 0.1)
            {
                HomeBtnFillMarker.Opacity = 0;
            }
        }

        private void SettingsBtnMouseEnter(object sender, MouseEventArgs e)
        {
            if (SettingsBtnFillMarker.Opacity == 0)
            {
                SettingsBtnFillMarker.Opacity = 0.1;
            }
        }

        private void SettingsBtnMouseLeave(object sender, MouseEventArgs e)
        {
            if (SettingsBtnFillMarker.Opacity == 0.1)
            {
                SettingsBtnFillMarker.Opacity = 0;
            }
        }

        private void ChatBtnMouseEnter(object sender, MouseEventArgs e)
        {
            if (ChatBtnFillMarker.Opacity == 0)
            {
                ChatBtnFillMarker.Opacity = 0.1;
            }
        }

        private void ChatBtnMouseLeave(object sender, MouseEventArgs e)
        {
            if (ChatBtnFillMarker.Opacity == 0.1)
            {
                ChatBtnFillMarker.Opacity = 0;
            }
        }

        private void AboutBtnMouseEnter(object sender, MouseEventArgs e)
        {
            if (AboutBtnFillMarker.Opacity == 0)
            {
                AboutBtnFillMarker.Opacity = 0.1;
            }
        }

        private void AboutBtnMouseLeave(object sender, MouseEventArgs e)
        {
            if (AboutBtnFillMarker.Opacity == 0.1)
            {
                AboutBtnFillMarker.Opacity = 0;
            }
        }

        private void CloseBtnMouseEnter(object sender, MouseEventArgs e)
        {
            CloseBackgound.Opacity = 1;
        }

        private void CloseBtnMouseLeave(object sender, MouseEventArgs e)
        {
            CloseBackgound.Opacity = 0;
        }

        private void MinimizeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            MinimizeBackgound.Opacity = 0.2;
        }

        private void MinimizeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            MinimizeBackgound.Opacity = 0;
        }

        private void MuteBtnMouseEnter(object sender, MouseEventArgs e)
        {
            MuteBackgound.Opacity = 0.2;
        }

        private void MuteBtnMouseLeave(object sender, MouseEventArgs e)
        {
            MuteBackgound.Opacity = 0;
        }

        private void CopyrightMouseEnter(object sender, MouseEventArgs e)
        {
            CopyrigtLine.Opacity = 1;
        }

        private void CopyrightMouseLeave(object sender, MouseEventArgs e)
        {
            CopyrigtLine.Opacity = 0;
        }

        private void hotKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                mute(null, null);
            }
        }
    }
}