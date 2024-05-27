using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace VA_Leo.Pages
{
    public partial class About
    {
        public About()
        {
            InitializeComponent();
        }

        private void liclinkMouseEnter(object sender, MouseEventArgs e)
        {
            Liclnik.Visibility = Visibility.Visible;
        }

        private void liclinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
            { FileName = "https://raw.githubusercontent.com/WaysoonProgramms/VoiceAssistantLeo/master/LICENSE", UseShellExecute = true });
        }

        private void liclinkMouseLeave(object sender, MouseEventArgs e)
        {
            Liclnik.Visibility = Visibility.Hidden;
        }

        private void repolinkMouseEnter(object sender, MouseEventArgs e)
        {
            Repolink.Visibility = Visibility.Visible;
        }

        private void repolinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo 
            { FileName = "https://github.com/WaysoonProgramms/VoiceAssistantLeo", UseShellExecute = true });
        }

        private void repolinkMouseLeave(object sender, MouseEventArgs e)
        {
            Repolink.Visibility = Visibility.Hidden;
        }

        private void issueslinkMouseEnter(object sender, MouseEventArgs e)
        {
            Issueslink.Visibility = Visibility.Visible;
        }

        private void issueslinkClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            { FileName = "https://github.com/WaysoonProgramms/VoiceAssistantLeo/issues", UseShellExecute = true });
        }

        private void issueslinkMouseLeave(object sender, MouseEventArgs e)
        {
            Issueslink.Visibility = Visibility.Hidden;
        }
    }
}
