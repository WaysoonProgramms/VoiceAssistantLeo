using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Diagnostics;
using System.Windows;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace VA_Leo.Pages
{
    public partial class Chat : Page
    {
        
        public Chat()
        {
            InitializeComponent();
            TextBox.Text = textMessage;
            chatList.ItemsSource = MainWindow.Message;
        }

        public class Messages
        {
            public string Message { get; set; }
            public string Time { get; set; }
            public int Length { get; set; }
            public string Aligment { get; set; }
        }

        public static void update()
        {
            
        }

        public static string textMessage = "";

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

            addMessage(TextBox.Text, "Left");

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
