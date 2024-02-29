using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VA_Leo
{
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();
        }

        private void liclinkMouseEnter(object sender, MouseEventArgs e)
        {
            liclnik.Visibility = Visibility.Visible;
        }

        private void liclinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
            { FileName = "https://raw.githubusercontent.com/WaysoonProgramms/VoiceAssistantLeo/master/LICENSE", UseShellExecute = true });
        }

        private void liclinkMouseLeave(object sender, MouseEventArgs e)
        {
            liclnik.Visibility = Visibility.Hidden;
        }

        private void repolinkMouseEnter(object sender, MouseEventArgs e)
        {
            repolink.Visibility = Visibility.Visible;
        }

        private void repolinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
            { FileName = "https://github.com/WaysoonProgramms/VoiceAssistantLeo", UseShellExecute = true });
        }

        private void repolinkMouseLeave(object sender, MouseEventArgs e)
        {
            repolink.Visibility = Visibility.Hidden;
        }
    }
}
