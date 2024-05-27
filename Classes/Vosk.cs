using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using VA_Leo.Pages;
using VA_Leo.Properties;
using Vosk;
using Settings = VA_Leo.Pages.Settings;

namespace VA_Leo.Classes
{
    public class Vosk
    {
        private static Dispatcher? _dispatcher;

        private static VoskRecognizer? _rec; // Объект распознавания VOSK
        private static WaveFileWriter? _writer; // Объект записи с микрофона
        private static bool _busy;
        
        public static string? text; // Текст распознанный Vosk
        private static bool _active; // Статус Wake Word
        private static int _num = 1;

        //private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        //TODO: Доделать логгер
        private static readonly Stopwatch WakeTimer = new();
        private static WaveInEvent _waveIn = new();
        
        private enum RecycleFlags : uint;
        
        private readonly MediaPlayer _player = new();

        public static void main()
        {
            // Инициализация модели
            var model = new Model(".\\vosk");
            _rec = new VoskRecognizer(model, 16000f);

            // Инициализация прослушивания
            _waveIn.WaveFormat = new WaveFormat(16000, 1);
            _waveIn.DataAvailable += WaveInOnDataAvailable;

            // Временный файл записи голоса
            string tmp = Path.GetTempPath();
            tmp += "assistant_leo_audio_rec_temp.wav";
            _writer = new WaveFileWriter(tmp, _waveIn.WaveFormat);

            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher = currentDispatcher;
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, RecycleFlags dwFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowThreadProcessId([In] IntPtr hwnd, [Out] out int lProcessId);
        
        public static void update()
        {
            if (Properties.Settings.Default.isMuted)
            {
                WakeTimer.Stop();
                _waveIn.StopRecording();
            }
            else
            {
                try
                {
                    _waveIn.StartRecording();
                }
                catch
                {
                    error1();
                    MainWindow.micAccess = false;
                }
            }
        }

        public static void error1()
        {
            MessageBox.Show(Resources.error1, Resources.MessageBox_errorSing, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            _writer?.Write(e.Buffer, 0, e.BytesRecorded);

            var vosk = new Vosk();

            if (_rec!.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                // Парсинг объекта с текстом
                var pResult = JObject.Parse(_rec.Result());
                text = pResult["text"]!.ToString();
                vosk.speechRecognized(); // Проверка результатов
            }
            else
            {
                // Парсинг объекта с текстом
                var pResult = JObject.Parse(_rec.PartialResult());
                text = pResult["partial"]!.ToString();
                vosk.speechRecognized(); // Проверка результатов
            }
        }

        public void speechRecognized()
        {
            if (text != "")
            {
                Console.WriteLine($@"[VOSK] Recognized > {text}");
            }

            if (WakeTimer.Elapsed.Seconds >= 15 && _active)
            {
                _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)Home.deactivateAnimation);

                playSound(@".\sounds\stop.wav");

                WakeTimer.Stop();
                WakeTimer.Reset();
                _active = false;
            }

            // WAKE WORD
            if (text!.Contains("лео"))
            {
                WakeTimer.Reset();
                WakeTimer.Start();

                if (!_active)
                {
                    playSound(@".\sounds\start.wav");
                    initialMessage("Лео", "Left");

                    _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)Home.activateAnimation);
                }

                _active = true; 
                _rec!.Reset();
            }

            // Спасибо
            if (text == "спасибо" && !_busy && _active)
            {

                _busy = true;
                WakeTimer.Restart();

                initialMessage("Спасибо", "Left");

                playVoice(@".\voices\vsegda_pozyalusta.wav");
                initialMessage("Всегда пожалуйста!", "Right");

                text = "";

                _rec?.Reset();
                _busy = false;
            }

            // Алиса
            if (text == "алиса" && !_busy)
            {

                _busy = true;
                WakeTimer.Restart();

                initialMessage("Алиса", "Left");

                playVoice(@".\voices\neAlica.wav");
                initialMessage("Я не Алиса! Я Лео!", "Right");

                _rec?.Reset();
                _busy = false;
            }

            // Siri
            if (text == "сири" && !_busy)
            {
                _busy = true;
                WakeTimer.Restart();

                initialMessage("Сири", "Left");

                playVoice(@".\voices\neSiri.wav");
                initialMessage("Я не Сири! Я Лео!", "Right");

                _rec?.Reset();
                _busy = false;
            }

            // Маруся
            if (text == "маруся" && !_busy)
            {
                _busy = true;
                WakeTimer.Restart();

                initialMessage("Маруся", "Left");

                playVoice(@".\voices\neMarusa.wav");
                initialMessage("Я не Маруся! Я Лео!", "Right");

                _rec?.Reset();
                _busy = false;
            }

            // Очистка корзины
            if (text == "очисти корзину" && !_busy && _active)
            {
                _busy = true;
                WakeTimer.Restart();

                initialMessage("Очисти корзину", "Left");

                if (Properties.Settings.Default.allowComputerControl)
                {
                    var result = SHEmptyRecycleBin(IntPtr.Zero, null, 0);
                    if (result == 0)
                    {
                        playVoice(@".\voices\bin1.wav");
                        initialMessage("Корзина очищена", "Right");
                    }
                    else
                    {
                        playVoice(@".\voices\bin2.wav");
                        initialMessage("Корзина уже пуста!", "Right");
                    }
                }
                else
                {
                    playVoice(@".\voices\err1.wav");
                    initialMessage("Мне запрещено делать это", "Right");
                }
                _rec?.Reset();
                _busy = false;

            }

            // Закрытие процесса в фокусе
            if (text == "закрой" && !_busy && _active)
            {
                _busy = true;
                WakeTimer.Restart();

                initialMessage("Закрой", "Left");

                if (Properties.Settings.Default.allowComputerControl)
                {
                    playVoice(@".\voices\good.wav");
                    initialMessage("Хорошо", "Right");

                    IntPtr hWnd = GetForegroundWindow();
                    int processId;
                    GetWindowThreadProcessId(hWnd, out processId);

                    Process proc = Process.GetProcessById(processId);
                    proc.Kill();
                }
                else
                {
                    playVoice(@".\voices\err1.wav");
                    initialMessage("Мне запрещено делать это", "Right");
                }
                _rec?.Reset();
                _busy = false;
            }

            // Музыка
            if (text == "открой яндекс музыку" && !_busy && _active)
            {
                string appdt = @"%LOCAL%\Programs\YandexMusic\Яндекс Музыка.exe";
                appdt = Environment.ExpandEnvironmentVariables(appdt);

                initialMessage(appdt, "Right");

                initialMessage("Открой Яндекс Музыку", "Left");

                startProgramm(appdt,
                    $".\\voices\\music.wav",
                    @".\voices\err3.wav",
                    1,
                    "Открываю! [Yandex Music]");
            }

            // Запуск ТГ
            if ((text == "открой телеграмм" || text == "открой телеграм" || text == "открой телегу") && !_busy && _active)
            {
                string appdt = "%APPDATA%\\Telegram Desktop\\Telegram.exe";
                appdt = Environment.ExpandEnvironmentVariables(appdt);

                initialMessage("Открой телеграм", "Left");

                startProgramm(appdt,
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err3.wav",
                    4,
                    "Открываю! [Telegram Desktop]");
            }

            if ((text == "открой консоль") && !_busy && _active)
            {
                initialMessage("Открой консоль", "Left");

                startProgramm("cmd.exe",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err2.wav",
                    3,
                    "Открываю! [Consloe]");
            }

            if (text == "открой вконтакте" && !_busy && _active)
            {
                initialMessage("Открой ВКонтакте", "Left");

                openWebsite("https://vk.com",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err1.wav",
                    "Открываю! [vk.com]");
            }

            if (text == "открой ютуб" && !_busy && _active)
            {
                initialMessage("Открой YouTube", "Left");

                openWebsite("https://youtube.com",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err1.wav",
                    "Открываю! [youtube.com]");
            }

            if ((text == "запусти майнкрафт" || text == "открой майн") && !_busy && _active)
            {
                initialMessage("Запусти Minecraft", "Left");

                startProgramm(@"C:\XboxGames\Minecraft Launcher\Content\Minecraft.exe",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err2.wav",
                    3,
                    "Открываю! [Minecraft Launcher]");
            }

            if ((text == "открой почту" || text == "зайди на почту") && !_busy && _active)
            {
                initialMessage("Открой почту", "Left");

                openWebsite("https://mail.google.com",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err1.wav",
                    "Открываю! [mail.google.com]");
            }

        }
        
        private void startProgramm(string target, string media, string error, int rndInt, string mesText)
        {
            _busy = true;
            WakeTimer.Restart();

            if (Properties.Settings.Default.allowProgrammsStart)
            {

                try
                {

                    Random rnd = new Random();
                    _num = rnd.Next(1, rndInt);

                    playVoice(media);
                    initialMessage(mesText, "Right");

                    var p = new Process();
                    p.StartInfo.FileName = target;
                    p.Start();
                    text = "";

                }
                catch (System.ComponentModel.Win32Exception)
                {
                    playVoice(error);
                    initialMessage("Приложение не найдено на вашем устройстве!", "Right");

                    text = "";
                }
            }
            else
            {
                playVoice(@".\voices\err1.wav");
                initialMessage("Мне запрещено делать это", "Right");
            }

            _rec?.Reset();
            _busy = false;
        }

        private void openWebsite(string url, string media, string error, string mesText)
        {
            _busy = true;
            WakeTimer.Restart();

            if (Properties.Settings.Default.allowBrowserStart)
            {
                playVoice(media);
                initialMessage(mesText, "Right");

                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                text = "";

            }
            else
            {
                playVoice(error);
                initialMessage("Мне запрещено делать это", "Right");
            }

            _rec?.Reset();
            _busy = false;
        }

        private void playSound(string file)
        {
            _player.Open(new Uri(file, UriKind.Relative));
            _player.Volume = Settings.sVoulme / 100.0f;
            _player.Play();
        }

        private void playVoice(string file)
        {
            _player.Open(new Uri(file, UriKind.Relative));
            _player.Volume = Settings.vVoulme / 100.0f;
            _player.Play();
        }

        private void initialMessage(string message, string aligment)
        {
            _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Chat.addMessage(message, aligment);
            });
        }
    }
}