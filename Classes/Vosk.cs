using NAudio.Wave;
using Newtonsoft.Json.Linq;
using NLog;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Vosk;
using VA_Leo.Pages;
using System.Windows.Threading;

namespace VA_Leo
{
    public class Vosk
    {
        public static Dispatcher _dispatcher;

        private static VoskRecognizer rec; // Объект распознования VOSK
        private static WaveFileWriter writer; // Объект записи с микрофона
        public static bool busy = false;

        public static bool ready;
        public static string text; // Текст распознанный Vosk
        public static string chatReply; // Ответ в чат
        public static string chatText;
        private static bool active = false; // Статус Wake Word
        private static int num = 1;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Stopwatch wakeTimer = new Stopwatch();
        private static WaveInEvent waveIn = new WaveInEvent();

        public static void main()
        {
            // Инициализация модели
            var model = new Model(".\\vosk");
            rec = new VoskRecognizer(model, 16000f);

            // Инициализация прослушивания
            waveIn.WaveFormat = new WaveFormat(16000, 1);
            waveIn.DataAvailable += WaveInOnDataAvailable;

            // Временный файл записи голоса
            string tmp = Path.GetTempPath();
            tmp += "assistant_leo_audio_rec_temp.wav";
            writer = new WaveFileWriter(tmp, waveIn.WaveFormat);

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher = dispatcher;
        }

        public static void update()
        {

            if (Properties.Settings.Default.isMuted)
            {
                wakeTimer.Stop();
                waveIn.StopRecording();
            }
            else
            {
                try
                {
                    waveIn.StartRecording();
                }
                catch
                {
                    MessageBox.Show("Лео не удалось получить доступ к микрофону. Попробуйте разрешить приложению доступ к " +
                        "микрофону: Параметры Windows -> Конфиденциальность -> Разрешения -> Микрофон.\n\nКод ошибки: 01",
                        "Что-то пошло не так...", MessageBoxButton.OK, MessageBoxImage.Error);
                    MainWindow.close();
                }
            }
        }

        private enum RecycleFlags : uint
        {
            SHERB_NOCONFIRMATION = 0x00000001,
            SHERB_NOPROGRESSUI = 0x00000002,
            SHERB_NOSOUND = 0x00000004
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32", SetLastError = true)]
        internal static extern int GetWindowThreadProcessId([In] IntPtr hwnd, [Out] out int lProcessId);

        private static void WaveInOnDataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);

            Vosk Vosk = new Vosk();

            if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                // Парсинг объекта с текстом
                var p_result = JObject.Parse(rec.Result());
                text = p_result["text"].ToString();
                Vosk.speechRecognized(); // Проверка результатов
            }
            else
            {
                // Парсинг объекта с текстом
                var p_result = JObject.Parse(rec.PartialResult());
                text = p_result["partial"].ToString();
                Vosk.speechRecognized(); // Проверка результатов
            }
        }

        private readonly MediaPlayer player = new MediaPlayer();

        public void speechRecognized()
        {
            if (text != "")
            {
                Console.WriteLine($"[VOSK] Распознано > {text}");
            }

            if (wakeTimer.Elapsed.Seconds >= 15 && active)
            {
                playSound(@".\sounds\stop.wav");

                wakeTimer.Stop();
                wakeTimer.Reset();
                active = false;
            }

            // WAKE WORD
            if (text.Contains("лео"))
            {
                wakeTimer.Reset();
                wakeTimer.Start();

                if (!active)
                {
                    playSound(@".\sounds\start.wav");
                    initialMessage("Лео", "Left");
                }

                active = true; 
                rec.Reset();
            }

            // Спасибо
            if (text == "спасибо" && !busy && active)
            {

                busy = true;
                wakeTimer.Restart();

                initialMessage("Спасибо", "Left");

                playSound(@".\voices\vsegda_pozyalusta.wav");
                initialMessage("Всегда пожалуйста!", "Right");

                text = "";

                rec.Reset();
                busy = false;
            }

            // Алиса
            if (text == "алиса" && !busy)
            {

                busy = true;
                wakeTimer.Restart();

                initialMessage("Алиса", "Left");

                playSound(@".\voices\neAlica.wav");
                initialMessage("Я не Алиса! Я Лео!", "Right");

                rec.Reset();
                busy = false;
            }

            // Siri
            if (text == "сири" && !busy)
            {
                busy = true;
                wakeTimer.Restart();

                initialMessage("Сири", "Left");

                playSound(@".\voices\neSiri.wav");
                initialMessage("Я не Сири! Я Лео!", "Right");

                rec.Reset();
                busy = false;
            }

            // Маруся
            if (text == "маруся" && !busy)
            {
                busy = true;
                wakeTimer.Restart();

                initialMessage("Маруся", "Left");

                playSound(@".\voices\neMarusa.wav");
                initialMessage("Я не Маруся! Я Лео!", "Right");

                rec.Reset();
                busy = false;
            }

            // Очистка корзины
            if (text == "очисти корзину" && !busy && active)
            {
                busy = true;
                wakeTimer.Restart();

                initialMessage("Очисти корзину", "Left");

                if (Properties.Settings.Default.allowComputerControl)
                {
                    var result = SHEmptyRecycleBin(IntPtr.Zero, null, 0);
                    if (result == 0)
                    {
                        playSound(@".\voices\bin1.wav");
                        initialMessage("Корзина очищена", "Right");
                    }
                    else
                    {
                        playSound(@".\voices\bin2.wav");
                        initialMessage("Корзина уже пуста!", "Right");
                    }
                }
                else
                {
                    playSound(@".\voices\err1.wav");
                    initialMessage("Мне запрещено делать это", "Right");
                }
                rec.Reset();
                busy = false;

            }

            // Закрытие процесса в фокусе
            if (text == "закрой" && !busy && active)
            {
                busy = true;
                wakeTimer.Restart();

                initialMessage("Закрой", "Left");

                if (Properties.Settings.Default.allowComputerControl)
                {
                    playSound(@".\voices\good.wav");
                    initialMessage("Хорошо", "Right");

                    IntPtr hWnd = GetForegroundWindow();
                    int pid;
                    GetWindowThreadProcessId(hWnd, out pid);

                    Process proc = Process.GetProcessById(pid);
                    proc.Kill();
                }
                else
                {
                    playSound(@".\voices\err1.wav");
                    initialMessage("Мне запрещено делать это", "Right");
                }
                rec.Reset();
                busy = false;
            }

            // Музыка
            if (text == "открой яндекс музыку" && !busy && active)
            {
                string appdt = "%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Яндекс Музыка.lnk";
                appdt = Environment.ExpandEnvironmentVariables(appdt);

                initialMessage("Открой Яндекс Музыку", "Left");

                startProgramm(appdt,
                    $".\\voices\\music.wav",
                    @".\voices\err3.wav",
                    1,
                    "Открываю! [Yandex Music]");
            }

            // Запуск ТГ
            if ((text == "открой телеграмм" || text == "открой телеграм" || text == "открой телегу") && !busy && active)
            {
                string appdt = "%APPDATA%\\Telegram Desktop\\Telegram.exe";
                appdt = Environment.ExpandEnvironmentVariables(appdt);

                initialMessage("Открой телеграм", "Left");

                startProgramm(appdt,
                    $".\\voices\\open{num}.wav",
                    @".\voices\err3.wav",
                    4,
                    "Открываю! [Telegram Desktop]");
            }

            if ((text == "открой консоль") && !busy && active)
            {
                initialMessage("Открой консоль", "Left");

                startProgramm("cmd.exe",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err2.wav",
                    3,
                    "Открываю! [Consloe]");
            }

            if (text == "открой вконтакте" && !busy && active)
            {
                initialMessage("Открой ВКонтакте", "Left");

                openWebsite("https://vk.com",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err1.wav",
                    4,
                    "Открываю! [vk.com]");
            }

            if (text == "открой ютуб" && !busy && active)
            {
                initialMessage("Открой YouTube", "Left");

                openWebsite("https://youtube.com",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err1.wav",
                    3,
                    "Открываю! [youtube.com]");
            }

            if ((text == "запусти майнкрафт" || text == "открой майн") && !busy && active)
            {
                initialMessage("Запусти Minecraft", "Left");

                startProgramm(@"C:\XboxGames\Minecraft Launcher\Content\Minecraft.exe",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err2.wav",
                    3,
                    "Открываю! [Minecraft Launcher]");
            }

            if ((text == "открой почту" || text == "зайди на почту") && !busy && active)
            {
                initialMessage("Открой почту", "Left");

                openWebsite("https://mail.google.com",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err1.wav",
                    3,
                    "Открываю! [mail.google.com]");
            }

        }

        public void startProgramm(string target, string media, string error, int rndInt, string mesText)
        {
            busy = true;
            wakeTimer.Restart();

            if (Properties.Settings.Default.allowProgrammsStart)
            {

                try
                {

                    Random rnd = new Random();
                    num = rnd.Next(1, rndInt);

                    playSound(media);
                    initialMessage(mesText, "Right");

                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = target;
                    p.Start();
                    text = "";

                }
                catch (System.ComponentModel.Win32Exception)
                {
                    playSound(error);
                    initialMessage("Приложение не найдено на вашем устройстве!", "Right");

                    text = "";
                }
            }
            else
            {
                playSound(@".\voices\err1.wav");
                initialMessage("Мне запрещено делать это", "Right");
            }

            rec.Reset();
            busy = false;
        }

        public void openWebsite(string url, string media, string error, int rndInt, string mesText)
        {
            busy = true;
            wakeTimer.Restart();

            Random rnd = new Random();
            int num = rnd.Next(1, rndInt);

            if (Properties.Settings.Default.allowBrowserStart)
            {
                playSound(media);
                initialMessage(mesText, "Right");

                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                text = "";

            }
            else
            {
                playSound(error);
                initialMessage("Мне запрещено делать это", "Right");
            }

            rec.Reset();
            busy = false;
        }

        public void playSound(string file)
        {
            player.Open(new Uri(file, UriKind.Relative));
            player.Volume = Settings.vVoulme / 100.0f;
            player.Play();
        }

        public void initialMessage(string text, string aligment)
        {
            _dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Chat.addMessage(text, aligment);
            });
        }
    }
}