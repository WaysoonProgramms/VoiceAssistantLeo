﻿using Microsoft.VisualBasic;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Vosk;

namespace VA_Leo
{
    class Vosk
    {
        public static VoskRecognizer rec;
        static WaveFileWriter writer;
        public static bool started = false;

        public static string txt; // Текст распознанный Vosk
        public static bool end = false; // Окончание обработки фразы и произношения
        public static bool active = false; // Статус Wake Word
        public static string awaitTime = ""; // Время активации
        public static int awaitRang = 0; // Кол-во запросов в не активном режиме
        public static int num = 1;

        public static Logger logger = LogManager.GetCurrentClassLogger();

        public static void main()
        {
            // Инициализация модели
            var model = new Model(".\\vosk\\small-model");
            rec = new VoskRecognizer(model, 16000f);

            // Инициализация прослушивания
            WaveInEvent waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(16000, 1);
            waveIn.DataAvailable += WaveInOnDataAvailable;
            waveIn.StartRecording();

            // Временный файл записи голоса
            writer = new WaveFileWriter(@"C:\Users\User\AppData\Local\Temp\VoiceAssistantRecord.wav", waveIn.WaveFormat);
        }

        enum RecycleFlags : uint
        {
            SHERB_NOCONFIRMATION = 0x00000001,
            SHERB_NOPROGRESSUI = 0x00000002,
            SHERB_NOSOUND = 0x00000004
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);

        private static void WaveInOnDataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);

            Vosk Vosk = new Vosk();

            if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                // Парсинг объекта с текстом
                var p_result = JObject.Parse(rec.Result());
                Vosk.txt = p_result["text"].ToString();
                Vosk.SpeechRecognized(); // Проверка результатов

            }
            else
            {
                // Парсинг объекта с текстом
                var p_result = JObject.Parse(rec.PartialResult());
                Vosk.txt = p_result["partial"].ToString();
                Vosk.SpeechRecognized(); // Проверка результатов
            }
        }

        private MediaPlayer player = new MediaPlayer();

        public void SpeechRecognized()
        {
            if (Properties.Settings.Default.isMuted)
            {
                return;
            }

            // WAKE WORD
            if (txt == "лео" && !Vosk.started)
            {
                Vosk.started = true;

                // Воспроизведение ответа
                player.Open(new Uri($".\\sounds\\start.wav", UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();

                Vosk.rec.Reset();
                Vosk.started = false;
            }

            // Спасибо
            if (txt == "спасибо" && !Vosk.started)
            {
                Vosk.started = true;

                player.Open(new Uri(@".\voices\vsegda_pozyalusta.wav", UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();

                txt = "";

                Vosk.rec.Reset();
                Vosk.started = false;
            }

            // Алиса
            if (txt == "алиса" && !Vosk.started)
            {
                Vosk.started = true;

                // Воспроизведение ответа
                player.Open(new Uri($".\\voices\\neAlica.wav", UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();

                Vosk.rec.Reset();
                Vosk.started = false;
            }

            // Siri
            if (txt == "сири" && !Vosk.started)
            {
                Vosk.started = true;

                // Воспроизведение ответа
                player.Open(new Uri($".\\voices\\neSiri.wav", UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();

                Vosk.rec.Reset();
                Vosk.started = false;
            }

            // Маруся
            if (txt == "маруся" && !Vosk.started)
            {
                Vosk.started = true;

                // Воспроизведение ответа
                player.Open(new Uri($".\\voices\\neMarusa.wav", UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();

                Vosk.rec.Reset();
                Vosk.started = false;
            }

            // Очистка корзины
            if (txt == "очисти корзину" && !Vosk.started)
            {
                Vosk.started = true;

                if (Properties.Settings.Default.allowPC)
                {
                    // Очистка
                    uint result = SHEmptyRecycleBin(IntPtr.Zero, null, 0);
                    if (result == 0)
                    {
                        player.Open(new Uri($".\\voices\\bin1.wav", UriKind.Relative));
                        player.Volume = Settings.vVoulme / 100.0f;
                        player.Play();
                    }
                    else
                    {
                        player.Open(new Uri($".\\voices\\bin2.wav", UriKind.Relative));
                        player.Volume = Settings.vVoulme / 100.0f;
                        player.Play();
                    }
                }
                else
                {
                    // Воспроизведение ошибки
                    player.Open(new Uri($".\\voices\\err1.wav", UriKind.Relative));
                    player.Volume = Settings.vVoulme / 100.0f;
                    player.Play();
                }
                Vosk.rec.Reset();
                Vosk.started = false;

            }

            // Запуск ТГ
            if ((txt == "открой телеграмм") && !Vosk.started)
            {
                startProgramm(@"C:\Users\User\AppData\Roaming\Telegram Desktop\Telegram.exe",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err3.wav",
                    4);
            }

            if ((txt == "открой консоль") && !Vosk.started)
            {
                startProgramm("cmd.exe",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err2.wav",
                    3);
            }

            if (txt == "открой вконтакте" && !Vosk.started)
            {
                openWebsite("https://vk.com",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err1.wav",
                    4);
            }

            if (txt == "открой ютуб" && !Vosk.started)
            {
                openWebsite("https://youtube.com",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err1.wav",
                    3);
            }

            if ((txt == "запусти майнкрафт" || txt == "открой майн") && !Vosk.started)
            {
                startProgramm(@"C:\XboxGames\Minecraft Launcher\Content\Minecraft.exe",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err2.wav",
                    3);
            }

            if ((txt == "открой почту") && !Vosk.started)
            {
                openWebsite("https://mail.google.com",
                    $".\\voices\\open{num}.wav",
                    @".\voices\err1.wav",
                    3);
            }
        }

        public void startProgramm(string target, string media, string error, int rndInt)
        {
            Vosk.started = true;

            if (Properties.Settings.Default.allowProgrammsStart)
            {

                try
                {

                    // Рандомайзер
                    Random rnd = new Random();
                    num = rnd.Next(1, rndInt);

                    // Воспроизведение ответа
                    player.Open(new Uri(media, UriKind.Relative));
                    player.Volume = Settings.vVoulme / 100.0f;
                    player.Play();

                    // Запуск
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = target;
                    p.Start();
                    txt = "";

                    logger.Info($"Ассистент выполнил комманду - запуск приложения >>> {target}");

                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Воспроизведение ошибки
                    player.Open(new Uri(error, UriKind.Relative));
                    player.Volume = Settings.vVoulme / 100.0f;
                    player.Play();

                    txt = "";

                    logger.Error("При попытке выполнить комманду - запуск приложения - произошла ошибка (Win32Exception)");
                }
            }
            else
            {
                // Воспроизведение ошибки
                player.Open(new Uri(@".\voices\err1.wav", UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();
            }

            Vosk.rec.Reset();
            Vosk.started = false;
        }

        public void openWebsite(string url, string media, string error, int rndInt)
        {
            Vosk.started = true;

            // Рандомайзер
            Random rnd = new Random();
            int num = rnd.Next(1, rndInt);

            if (Properties.Settings.Default.allowBrowserStart)
            {
                // Воспроизведение ответа
                player.Open(new Uri(media, UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();

                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                txt = "";

                logger.Info($"Ассистент выполнил комманду - открытие ссылки >>> {url}");
            }
            else
            {
                // Воспроизведение ошибки
                player.Open(new Uri(error, UriKind.Relative));
                player.Volume = Settings.vVoulme / 100.0f;
                player.Play();
            }

            Vosk.rec.Reset();
            Vosk.started = false;
        }
    }
}