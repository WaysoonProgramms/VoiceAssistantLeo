using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Leo.PageModels;
using Leo.Properties;

namespace Leo.Classes
{
    public class MessageData
    {
        public string? Text { get; init; }
        public string? Time { get; init; }
        public string? Date { get; init; }
        public string? Aligment { get; init; }
        public bool IsDateVisible { get; init; }
        public string? Id { get; init; }
    }
    
    public class ChatManager
    {
        private readonly string _path = ".\\data.json";
        private static readonly Logger Logger = new();
        
        public ChatManager()
        {
            try
            {
                FileStream file = File.Open(_path, FileMode.Open);
                file.Close();
            }
            catch
            {
                FileStream file = File.Create(_path);
                file.Close();
                Properties.Settings.Default.messagesId = 0;
                Properties.Settings.Default.nowDate = "01.01.01";
                Properties.Settings.Default.Save();
                MainWindow.ChatCollection = new ObservableCollection<Chat.Messages>();
                Chat.NullMessages = true;
            }
        }

        public async void serializeChat(string? text, string? aligment, string? time, string? date, bool isDateVisible, int id)
        {
            await using StreamWriter writer = new StreamWriter(_path, true);
            MessageData messageData = new MessageData()
            {
                Text = text,
                Time = time,
                Date = date,
                Aligment = aligment,
                IsDateVisible = isDateVisible,
                Id = id.ToString()
            };
            string json = JsonConvert.SerializeObject(messageData, Formatting.Indented);
            
            await writer.WriteLineAsync(json);
        }

        public async void deserializeChat()
        {
            try
            {
                using StreamReader reader = new StreamReader(_path);
                while (true)
                {
                    string line = "";
                    for (int i = 0; i <= 7; i++)
                    {
                        line += await reader.ReadLineAsync();
                    }
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    MessageData? md = JsonConvert.DeserializeObject<MessageData>(line);
                    if (md?.Id == "10000")
                    {
                        Logger.message("Chat messages have reached 10,000 and need clearing");
                        MessageBoxResult messageBoxResult = MessageBox.Show(Resources.message1, Resources.MessageBox_messageSign, MessageBoxButton.YesNo,
                            MessageBoxImage.Information);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            reader.Close();
                            clearChat();
                            break;
                        }
                    }
                    else
                    {
                        Chat.addMessage(md?.Text, md?.Aligment, md?.Time, md?.Date, md!.IsDateVisible);
                    }
                }
            }
            catch
            {
                Logger.error("Leo failed to load recent messages");
                MessageBox.Show(Resources.error4, Resources.MessageBox_errorSign, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clearChat()
        {
            Properties.Settings.Default.messagesId = 0;
            Properties.Settings.Default.nowDate = "01.01.01";
            Properties.Settings.Default.Save();
            File.Delete(_path);
            FileStream file = File.Create(_path);
            file.Close();
            MainWindow.ChatCollection = new ObservableCollection<Chat.Messages>();
            Chat.NullMessages = true;
        }
    }
}