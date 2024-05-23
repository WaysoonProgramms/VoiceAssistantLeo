using System.Windows.Controls;
using System.Windows.Input;

namespace VA_Leo.Pages
{
    public partial class Chat : Page
    {

        public static Chat cht;
        
        public Chat()
        {
            InitializeComponent();
            TextBox.Text = textMessage;
            chatList.ItemsSource = MainWindow.Message;
            cht = this;
        }

        public class Messages
        {
            public string? Message { get; set; }
            public string? Time { get; set; }
            public int Length { get; set; }
            public string? Aligment { get; set; }
        }

        public static string textMessage = "";

        private void Update()
        {
            helloLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        public static void addMessage(string text, string aligment)
        {
            if (text == "")
            {
                return;
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

            if (cht.helloLabel.Visibility == System.Windows.Visibility.Visible)
            {
                cht.helloLabel.Visibility = System.Windows.Visibility.Hidden;
            }
            
            MainWindow.Message.Add(new Messages
            {
                Message = text,
                Time = DateTime.Now.ToShortTimeString(),
                Length = length,
                Aligment = aligment
            });
        }

        public void send(object sender, MouseButtonEventArgs e)
        {
            Vosk vosk = new Vosk();
            Vosk.text = TextBox.Text.ToLower();
            vosk.speechRecognized();

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
                send(SendButton, null);
            }

            if (e.Key == Key.Up)
            {
                TextBox.Text = textMessage;
            }
        }

        private void messageBuffer(object sender, TextChangedEventArgs e)
        {
            if (TextBox.Text != "")
            {
                textMessage = TextBox.Text;
            }
        }
    }
}
