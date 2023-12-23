using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VA_Leo
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void tgBtnEnter(object sender, MouseEventArgs e)
        {
            tgBtnRec.Opacity = 1;
        }

        private void tgBtnLeave(object sender, MouseEventArgs e)
        {
            tgBtnRec.Opacity = 0;
        }

        private void tgBtnClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://t.me/waysoon_official", UseShellExecute = true });
        }

        private void webBtnEnter(object sender, MouseEventArgs e)
        {
            webBtnRec.Opacity = 0.5;
        }

        private void webBtnLeave(object sender, MouseEventArgs e)
        {
            webBtnRec.Opacity = 0;
        }

        private void webBtnClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "http://voiceassistantleo.tilda.ws/", UseShellExecute = true });
        }

        private void gitBtnEnter(object sender, MouseEventArgs e)
        {
            gitBtnRec.Opacity = 0.5;
        }

        private void gitBtnLeave(object sender, MouseEventArgs e)
        {
            gitBtnRec.Opacity = 0;
        }

        private void gitBtnClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/WaysoonProgramms/VoiceAssistantLeo", UseShellExecute = true });
        }
    }
}
