using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VA_Leo.Classes;

namespace VA_Leo.Pages
{
    public partial class Chat
    {
        private static Chat? _chat;
        private static string _textMessage = "";
        private static bool _nullMessages = true;
        private static ScrollViewer _scrollViewer;

        private static readonly ChatManager ChatManager = new();
        
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
            
            ScrollBox.ScrollToEnd();
            _scrollViewer = ScrollBox;
        }

        public class Messages
        {
            public string? Message { get; set; }
            public string? Time { get; set; }
            public int Length { get; set; }
            public string? Aligment { get; set; }
            public string? DateMessage { get; set; }
            public bool IsDateVisible { get; set; }
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

            var ft = new FormattedText(text, 
                CultureInfo.CurrentCulture, 
                0, 
                new Typeface("Montserrat Alternates"), 
                14, 
                Brushes.White, 
                96);
            var length = (int)ft.WidthIncludingTrailingWhitespace + 20;

            if (_chat!.HelloLabel.Visibility == System.Windows.Visibility.Visible)
            {
                _chat.HelloLabel.Visibility = System.Windows.Visibility.Hidden;
            }

            var isDateVisible = true;
            if (Properties.Settings.Default.nowDate == DateTime.Now.ToShortDateString())
            {
                isDateVisible = false;
            }
            else
            {
                Properties.Settings.Default.nowDate = DateTime.Now.ToShortDateString();
                Properties.Settings.Default.Save();
            }

            MainWindow.ChatCollection!.Add(new Messages
            {
                Message = text,
                Time = DateTime.Now.ToShortTimeString(),
                Length = length,
                Aligment = aligment,
                DateMessage = DateTime.Now.ToShortDateString(),
                IsDateVisible = isDateVisible
            });
            
            ChatManager.serializeChat(text, aligment, DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString(), isDateVisible);
            _scrollViewer.ScrollToEnd();
        }
        
        public static void addMessage(string? text, string? aligment, string? time, string? date, bool isDateVisible)
        {
            if (text == string.Empty)
            {
                return;
            }

            if (_nullMessages)
            {
                _nullMessages = false;
            }

            var ft = new FormattedText(text, 
                CultureInfo.CurrentCulture, 
                0, 
                new Typeface("Montserrat Alternates"), 
                14, 
                Brushes.White, 
                96);
            var length = (int)ft.WidthIncludingTrailingWhitespace + 20;

            if (_chat!.HelloLabel.Visibility == System.Windows.Visibility.Visible)
            {
                _chat.HelloLabel.Visibility = System.Windows.Visibility.Hidden;
            }

            MainWindow.ChatCollection!.Add(new Messages
            {
                Message = text,
                Time = time,
                Length = length,
                Aligment = aligment,
                DateMessage = date,
                IsDateVisible = isDateVisible
            });
            
            _scrollViewer.ScrollToEnd();
        }

        private void send(object sender, MouseButtonEventArgs? e)
        {
            Classes.Vosk vosk = new Classes.Vosk();
            Classes.Vosk.recognizedText = TextBox.Text.ToLower();
            vosk.speechRecognized();

            Console.WriteLine($@"[INPUT] Input > {Classes.Vosk.recognizedText}");

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
            switch (e.Key)
            {
                case Key.Enter:
                    send(SendButton, null);
                    break;
                case Key.Up:
                    TextBox.Text = _textMessage;
                    break;
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
