using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace VA_Leo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                animation.Completed += closeAnimationComplete;
                Storyboard.SetTargetName(animation, this.Name);
                Storyboard.SetTargetProperty(animation, new PropertyPath(MainWindow.OpacityProperty));
                storyboardFade.Children.Add(animation);

                storyboardFade.Begin(this);
            }
        }

        private void closeAnimationComplete(object sender, EventArgs e)
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

        private void removeMarkers()
        {
            HomeBtnMarker.Opacity = 0;
            SettingsBtnMarker.Opacity = 0;
            ChatBtnMarker.Opacity = 0;
        }

        private void HomeBtnMouseEnter(object sender, MouseEventArgs e)
        {
            if (HomeBtnMarker.Opacity == 0)
            {
                HomeBtnMarker.Opacity = 0.2;
            }
        }

        private void HomeBtnMouseLeave(object sender, MouseEventArgs e)
        {
            if (HomeBtnMarker.Opacity == 0.2)
            {
                HomeBtnMarker.Opacity = 0;
            }
        }

        private void SettingsBtnMouseEnter(object sender, MouseEventArgs e)
        {
            if (SettingsBtnMarker.Opacity == 0)
            {
                SettingsBtnMarker.Opacity = 0.2;
            }
        }

        private void SettingsBtnMouseLeave(object sender, MouseEventArgs e)
        {
            if (SettingsBtnMarker.Opacity == 0.2)
            {
                SettingsBtnMarker.Opacity = 0;
            }
        }

        private void ChatBtnMouseEnter(object sender, MouseEventArgs e)
        {
            if (ChatBtnMarker.Opacity == 0)
            {
                ChatBtnMarker.Opacity = 0.2;
            }
        }

        private void ChatBtnMouseLeave(object sender, MouseEventArgs e)
        {
            if (ChatBtnMarker.Opacity == 0.2)
            {
                ChatBtnMarker.Opacity = 0;
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
    }
}