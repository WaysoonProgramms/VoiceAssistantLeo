using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;


namespace VA_Leo
{
    public partial class Chat : Page
    {

        public Chat()
        {
            InitializeComponent();
            TextBox.Text = message;
        }

        public static string message = "";

        private void sendButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Vosk vosk = new Vosk();
            Vosk.txt = TextBox.Text.ToLower();
            vosk.SpeechRecognized(1);

            Console.WriteLine($"[INPUT] Введено > {Vosk.txt}");

            TextBox.Text = string.Empty;
        }

        private void SendBtnMouseEnter(object sender, MouseEventArgs e)
        {
            SendButtonBackgound.Opacity = 0.2;
        }

        private void SendBtnMouseLeave(object sender, MouseEventArgs e)
        {
            SendButtonBackgound.Opacity = 0;
        }

        private void Chat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendButton_MouseDown(SendButton, null);
            }

            if (e.Key == Key.Up)
            {
                TextBox.Text = message;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBox.Text != "")
            {
                message = TextBox.Text;
            }
            
        }
    }
}
