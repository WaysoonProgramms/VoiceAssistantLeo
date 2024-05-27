using System.Windows.Controls;
using System.Windows.Input;

namespace VA_Leo.Pages
{
    public partial class Chat
    {
        private static Chat? _chat;
        private static string _textMessage = "";
        private static bool _nullMessages = true;
        
        public Chat()
        {
            InitializeComponent();
            TextBox.Text = _textMessage;
            ChatList.ItemsSource = MainWindow.ChatCollection;
            _chat = this;

            if (!_nullMessages)
            {
                HelloLabel.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public class Messages
        {
            public string? Message { get; set; }
            public string? Time { get; set; }
            public int Length { get; set; }
            public string? Aligment { get; set; }
        }

        public static void addMessage(string text, string aligment)
        {
            if (text == string.Empty)
            {
                return;
            }

            if (_nullMessages)
            {
                _nullMessages = false;
            }

            int length;

            if (text.Length < 5)
            {
                length = text.Length + 50;
            }
            else
            {
                length = text.Length * 8 + 20;
            }

            if (_chat!.HelloLabel.Visibility == System.Windows.Visibility.Visible)
            {
                _chat.HelloLabel.Visibility = System.Windows.Visibility.Hidden;
            }
            
            MainWindow.ChatCollection!.Add(new Messages
            {
                Message = text,
                Time = DateTime.Now.ToShortTimeString(),
                Length = length,
                Aligment = aligment
            });
        }

        private void send(object sender, MouseButtonEventArgs? e)
        {
            Classes.Vosk vosk = new Classes.Vosk();
            Classes.Vosk.text = TextBox.Text.ToLower();
            vosk.speechRecognized();

            Console.WriteLine($@"[INPUT] Input > {Classes.Vosk.text}");

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
                send(SendButton, null);
            }

            if (e.Key == Key.Up)
            {
                TextBox.Text = _textMessage;
            }
        }

        private void messageBuffer(object sender, TextChangedEventArgs e)
        {
            if (TextBox.Text != "")
            {
                _textMessage = TextBox.Text;
            }
        }
    }
}
