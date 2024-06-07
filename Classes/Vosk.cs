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

        private static VoskRecognizer? _recognizer; // Объект распознавания VOSK
        private static WaveFileWriter? _writer; // Объект записи с микрофона
        private static bool _busy;
        
        public static string? recognizedText;
        private static bool _wakeWordStatus;
        private static int _num = 1;
        
        private static readonly Logger Logger = new();
        private static readonly Stopwatch WakeTimer = new();
        private static readonly WaveInEvent WaveIn = new();
        
        private enum RecycleFlags : uint;
        
        private readonly MediaPlayer _player = new();

        public static void main()
        {
            // Инициализация модели
            var model = new Model(".\\vosk");
            _recognizer = new VoskRecognizer(model, 16000f);

            // Инициализация записи
            WaveIn.WaveFormat = new WaveFormat(16000, 1);
            WaveIn.DataAvailable += WaveInOnDataAvailable;

            // Временный файл записи голоса
            string tmp = Path.GetTempPath();
            tmp += "assistant_leo_audio_rec_temp.wav";
            _writer = new WaveFileWriter(tmp, WaveIn.WaveFormat);

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
                WaveIn.StopRecording();
            }
            else
            {
                try
                {
                    WaveIn.StartRecording();
                }
                catch
                {
                    error1();
                    
                    Logger.warn("Leo couldn't start voice recognition. Microphone access is not allowed.");
                    
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

            if (_recognizer!.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                // Парсинг объекта с текстом
                var pResult = JObject.Parse(_recognizer.Result());
                recognizedText = pResult["text"]!.ToString();
                vosk.speechRecognized(); // Проверка результатов
            }
            else
            {
                // Парсинг объекта с текстом
                var pResult = JObject.Parse(_recognizer.PartialResult());
                recognizedText = pResult["partial"]!.ToString();
                vosk.speechRecognized(); // Проверка результатов
            }
        }

        public void speechRecognized()
        {
            if (recognizedText != "")
            {
                Console.WriteLine($@"[VOSK] Recognized > {recognizedText}");
            }

            if (WakeTimer.Elapsed.Seconds >= 15 && _wakeWordStatus)
            {
                playSound(@".\sounds\stop.wav");

                _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)Home.deactivateAnimation);
                
                Logger.message("Assistant deactivated");
                
                WakeTimer.Stop();
                WakeTimer.Reset();
                _wakeWordStatus = false;
            }

            // WAKE WORD
            if (recognizedText!.Contains("лео"))
            {
                WakeTimer.Reset();
                WakeTimer.Start();

                if (!_wakeWordStatus)
                {
                    playSound(@".\sounds\start.wav");
                    initialMessage("Лео", "Left");

                    _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)Home.activateAnimation);
                }

                _wakeWordStatus = true; 
                _recognizer!.Reset();

                Logger.message("Assistant activated");
            }

            // Спасибо
            if (recognizedText == "спасибо" && !_busy && _wakeWordStatus)
            {

                _busy = true;
                WakeTimer.Restart();

                initialMessage("Спасибо", "Left");

                playVoice(@".\voices\vsegda_pozyalusta.wav");
                initialMessage("Всегда пожалуйста!", "Right");
                
                Logger.message($"Vosk recognized the phrase - {recognizedText}");
                
                recognizedText = "";

                _recognizer?.Reset();
                _busy = false;
            }

            // Алиса
            if (recognizedText == "алиса" && !_busy)
            {

                _busy = true;
                WakeTimer.Restart();

                initialMessage("Алиса", "Left");

                playVoice(@".\voices\neAlica.wav");
                initialMessage("Я не Алиса! Я Лео!", "Right");

                _recognizer?.Reset();
                _busy = false;
            }

            // Siri
            if (recognizedText == "сири" && !_busy)
            {
                _busy = true;
                WakeTimer.Restart();

                initialMessage("Siri", "Left");
                

                playVoice(@".\voices\neSiri.wav");
                initialMessage("Я не Siri! Я Лео!", "Right");

                _recognizer?.Reset();
                _busy = false;
            }

            // Маруся
            if (recognizedText == "маруся" && !_busy)
            {
                _busy = true;
                WakeTimer.Restart();

                initialMessage("Маруся", "Left");

                playVoice(@".\voices\neMarusa.wav");
                initialMessage("Я не Маруся! Я Лео!", "Right");

                _recognizer?.Reset();
                _busy = false;
            }

            // Очистка корзины
            if (recognizedText == "очисти корзину" && !_busy && _wakeWordStatus)
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
                _recognizer?.Reset();
                _busy = false;

            }

            // Закрытие процесса в фокусе
            if (recognizedText == "закрой" && !_busy && _wakeWordStatus)
            {
                _busy = true;
                WakeTimer.Restart();

                initialMessage("Закрой", "Left");

                if (Properties.Settings.Default.allowComputerControl)
                {
                    playVoice(@".\voices\good.wav");
                    initialMessage("Хорошо", "Right");

                    IntPtr hWnd = GetForegroundWindow();
                    GetWindowThreadProcessId(hWnd, out var processId);

                    Process proc = Process.GetProcessById(processId);
                    proc.Kill();
                }
                else
                {
                    playVoice(@".\voices\err1.wav");
                    initialMessage("Мне запрещено делать это", "Right");
                }
                _recognizer?.Reset();
                _busy = false;
            }

            // Музыка
            if (recognizedText == "открой яндекс музыку" && !_busy && _wakeWordStatus)
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
            if ((recognizedText == "открой телеграмм" || recognizedText == "открой телеграм" || recognizedText == "открой телегу") && !_busy && _wakeWordStatus)
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

            if ((recognizedText == "открой консоль") && !_busy && _wakeWordStatus)
            {
                initialMessage("Открой консоль", "Left");

                startProgramm("cmd.exe",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err2.wav",
                    3,
                    "Открываю! [Consloe]");
            }

            if (recognizedText == "открой вконтакте" && !_busy && _wakeWordStatus)
            {
                initialMessage("Открой ВКонтакте", "Left");

                openWebsite("https://vk.com",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err1.wav",
                    "Открываю! [vk.com]");
            }

            if (recognizedText == "открой ютуб" && !_busy && _wakeWordStatus)
            {
                initialMessage("Открой YouTube", "Left");

                openWebsite("https://youtube.com",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err1.wav",
                    "Открываю! [youtube.com]");
            }

            if ((recognizedText == "запусти майнкрафт" || recognizedText == "открой майн") && !_busy && _wakeWordStatus)
            {
                initialMessage("Запусти Minecraft", "Left");

                startProgramm(@"C:\XboxGames\Minecraft Launcher\Content\Minecraft.exe",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err2.wav",
                    3,
                    "Открываю! [Minecraft Launcher]");
            }

            if ((recognizedText == "открой почту" || recognizedText == "зайди на почту") && !_busy && _wakeWordStatus)
            {
                initialMessage("Открой почту", "Left");

                openWebsite("https://mail.google.com",
                    $".\\voices\\open{_num}.wav",
                    @".\voices\err1.wav",
                    "Открываю! [mail.google.com]");
            }

            if (recognizedText == "поставь на паузу" && !_busy && _wakeWordStatus)
            {
                _busy = true;
                WakeTimer.Restart();
                
                initialMessage("Поставь на паузу", "Left");

                //TODO: Пауза SMTC
                
                _recognizer?.Reset();
                _busy = false;
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
                    recognizedText = "";
                    _num = 1;

                }
                catch (System.ComponentModel.Win32Exception)
                {
                    playVoice(error);
                    initialMessage("Приложение не найдено на вашем устройстве!", "Right");
                    
                    Logger.error("Leo was unable to open the program. The program was not found on the device.");
                    
                    recognizedText = "";
                }
            }
            else
            {
                playVoice(@".\voices\err1.wav");
                initialMessage("Мне запрещено делать это", "Right");
            }

            _recognizer?.Reset();
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
                recognizedText = "";

            }
            else
            {
                playVoice(error);
                initialMessage("Мне запрещено делать это", "Right");
            }

            _recognizer?.Reset();
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