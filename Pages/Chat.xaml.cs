using System.Windows.Controls;
using System.Windows.Input;


namespace VA_Leo.Pages
{
    public partial class Chat : Page
    {

        public Chat()
        {
            InitializeComponent();
            TextBox.Text = message;
        }

        public static string message = "";

        private void sendButtonClick(object sender, MouseButtonEventArgs e)
        {
            Vosk vosk = new Vosk();
            Vosk.text = TextBox.Text.ToLower();
            vosk.speechRecognized(1);

            Console.WriteLine($"[INPUT] Введено > {Vosk.text}");

            TextBox.Text = string.Empty;
        }

        private void sendBtnMouseEnter(object sender, MouseEventArgs e)
        {
            SendButtonBackgound.Opacity = 0.2;
        }

        private void sendBtnMouseLeave(object sender, MouseEventArgs e)
        {
            SendButtonBackgound.Opacity = 0;
        }

        private void hotKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendButtonClick(SendButton, null);
            }

            if (e.Key == Key.Up)
            {
                TextBox.Text = message;
            }
        }

        private void messageBuffer(object sender, TextChangedEventArgs e)
        {
            if (TextBox.Text != "")
            {
                message = TextBox.Text;
            }
            
        }
    }
}
