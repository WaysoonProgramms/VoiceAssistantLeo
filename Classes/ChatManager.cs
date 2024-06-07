using System.IO;
using System.Windows;
using Newtonsoft.Json;
using VA_Leo.Pages;
using VA_Leo.Properties;

namespace VA_Leo.Classes
{
    public class MessageData
    {
        public string? Text { get; init; }
        public string? Time { get; init; }
        public string? Date { get; init; }
        public string? Aligment { get; init; }
        public bool IsDateVisible { get; init; }
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
            }
        }

        public async void serializeChat(string? text, string? aligment, string? time, string? date, bool isDateVisible)
        {
            await using StreamWriter writer = new StreamWriter(_path, true);
            MessageData messageData = new MessageData()
            {
                Text = text,
                Time = time,
                Date = date,
                Aligment = aligment,
                IsDateVisible = isDateVisible
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
                    for (int i = 0; i <= 6; i++)
                    {
                        line += await reader.ReadLineAsync();
                    }
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    MessageData? md = JsonConvert.DeserializeObject<MessageData>(line);
                    Chat.addMessage(md?.Text, md?.Aligment, md?.Time, md?.Date, md!.IsDateVisible);
                }
            }
            catch
            {
                Logger.error("Leo failed to load recent messages");
                MessageBox.Show(Resources.error4, Resources.MessageBox_errorSing, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}