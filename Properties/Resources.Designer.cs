﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Leo.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VA_Leo.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Режим разработчика.
        /// </summary>
        public static string devModeSing {
            get {
                return ResourceManager.GetString("devModeSing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Лео не удалось получить доступ к микрофону. Попробуйте разрешить приложению доступ к микрофону: Параметры Windows -&gt; Конфиденциальность -&gt; Разрешения -&gt; Микрофон.
        ///
        ///Код ошибки: 01.
        /// </summary>
        public static string error1 {
            get {
                return ResourceManager.GetString("error1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Лео не удалось загрузить последние сообщения.
        ///
        ///Код ошибки: 04.
        /// </summary>
        public static string error4 {
            get {
                return ResourceManager.GetString("error4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Лео не удалось сохранить последние сообщения.
        ///
        ///Код ошибки: 05.
        /// </summary>
        public static string error5 {
            get {
                return ResourceManager.GetString("error5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///
        ///ДОСТУП РАЗРЕШЕН!
        ///Запуск приложения...
        ///.
        /// </summary>
        public static string MainWindow_consoleAuth_accessAllowed {
            get {
                return ResourceManager.GetString("MainWindow_consoleAuth_accessAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неверный ключ авторизаци! ДОСТУП ОГРАНИЧЕН!.
        /// </summary>
        public static string MainWindow_consoleAuth_accessDenided {
            get {
                return ResourceManager.GetString("MainWindow_consoleAuth_accessDenided", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ассистент Лео 0.1 | Режим разработчика
        ///@WaysoonProgramms 2024
        ///
        ///Введите ключ авторизации:.
        /// </summary>
        public static string MainWindow_consoleAuth_welcomeMessage {
            get {
                return ResourceManager.GetString("MainWindow_consoleAuth_welcomeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Количество сообщений достигло 10 000. Это может замедлить работу приложения. Так же, большое количество сообщений увеличивает вес приложения.
        ///
        ///Очистить сообщения?.
        /// </summary>
        public static string message1 {
            get {
                return ResourceManager.GetString("message1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Что-то пошло не так....
        /// </summary>
        public static string MessageBox_errorSign {
            get {
                return ResourceManager.GetString("MessageBox_errorSign", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Обратите внимание!.
        /// </summary>
        public static string MessageBox_messageSign {
            get {
                return ResourceManager.GetString("MessageBox_messageSign", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Этот параметр отвечает за активацию ИИ. Лео сможет отвечать на вопросы с помощью искусственного интеллекта (GPT-4)
        ///
        ///ОБРАТИТЕ ВНИМАНИЕ! Все вычисления ИИ производит на вашем устройстве для этого требуется GPU.
        /// </summary>
        public static string Settings_AIBox_Help {
            get {
                return ResourceManager.GetString("Settings_AIBox_Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Интеграция ИИ.
        /// </summary>
        public static string Settings_AIBox_Sing {
            get {
                return ResourceManager.GetString("Settings_AIBox_Sing", resourceCulture);
            }
        }
    }
}
