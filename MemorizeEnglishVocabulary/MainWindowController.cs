using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using BOA.Common.Helpers;
using BOA.LanguageTranslations.Longman;

namespace WpfApp2
{
    static class EnToTrCache
    {
        #region Public Methods
        public static void StartToCache(IReadOnlyCollection<string> words)
        {
            if (ConfigHelper.GetBooleanFromAppSetting("EnToTrCache.UseSameThread"))
            {
                StartToInitializeCache(new List<string>(words));
                return;
            }

            var thread = new Thread(() =>
            {
                try
                {
                    StartToInitializeCache(new List<string>(words));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static IDictionary<string, string> TryGetForWord(string word)
        {
            var filePath = GetFilePath(word);
            if (File.Exists(filePath))
            {
                return JsonHelper.Deserialize<Dictionary<string, string>>(File.ReadAllText(filePath));
            }

            return null;
        }
        static bool IsAlreadyCached(string word)
        { 
			var filePath = GetFilePath(word);
            
            return File.Exists(filePath);
		
		}
        #endregion

        #region Methods
        internal static void StartToInitializeCache(IReadOnlyCollection<string> words)
        {
            try
            {
                foreach (var word in words)
                {
                    if(IsAlreadyCached(word))
                    {
                        continue;
                    }
				
                    var filePath   = GetFilePath(word);
                    var dictionary = new Dictionary<string, string>();

                    var wordInfo = Translator.GetWordInfo(word);
                    foreach (var entry in wordInfo.Dictentries)
                    {
                        foreach (var usageInfo in entry.Usages)
                        {
                            foreach (var example in usageInfo.Examples)
                            {
                                if (dictionary.Count <= 3)
                                {
                                    dictionary.SetValue(example.Text.Trim(), example.TextTR.Trim());
                                }
                            }
                        }
                    }

                    FileHelper.WriteAllText(filePath, JsonHelper.Serialize(dictionary));
                }
            }
            catch (Exception e)
            {
                Log.Push(e);
            }
            
        }

        static string GetFilePath(string word)
        {
            return MainWindowController.WorkingDirectory + "Cache" + Path.DirectorySeparatorChar + word + ".json";
        }
        #endregion
    }

    class MainWindowController
    {
        #region Constants
        public const string WorkingDirectory = @"D:\EnglishVocabulary\";
        #endregion

        #region Public Properties
        public MainWindowModel Model { get; set; }
        #endregion

        #region Properties
        static string CommandLineArgumentAsFilePath
        {
            get { return Environment.GetCommandLineArgs().FirstOrDefault(x => x.EndsWith(".txt")); }
        }
        #endregion

        #region Public Methods
        public void DeleteSelectedWord()
        {
            Model.Lines.Remove(Model.SelectedWord);
            SaveDataSource();
            Clean();
        }

        public void GoBack()
        {
            FocusToNewFile(-1);
        }

        public void GoForward()
        {
            FocusToNewFile(1);
        }

        public void Loaded()
        {
            Model.DataSourcePath = CommandLineArgumentAsFilePath ?? WorkingDirectory + "1.txt";
            Clean();
        }

        public void NavigateToWord()
        {
            if (string.IsNullOrWhiteSpace(Model.SelectedWord))
            {
                return;
            }

            Model.GoogleTranslateAddress = "https://translate.google.com/#en/tr/" + Model.SelectedWord;

            Model.AudioDictionaryAddress = "https://www.seslisozluk.net/en/what-is-the-meaning-of-" + Model.SelectedWord + "/";

            Model.LongManWebBrowserAddress = "https://www.ldoceonline.com/dictionary/" + Model.SelectedWord;

            Model.ImageBrowserAddress = "www.google.com.tr/search?q=" + Model.SelectedWord + "&safe=off&source=lnms&tbm=isch";


            Model.LongManInitialJsScript = LongManScriptHelper.GetScript(Model.SelectedWord);

            //if (Model.SelectedWordEnToTrCache != null)
            //{
            //    foreach (var key in Model.SelectedWordEnToTrCache.Keys)
            //    {
            //        Model.Sample1En = key;
            //        Model.Sample1Tr = Model.SelectedWordEnToTrCache[key];
            //    }
            //}

        }
        #endregion

        #region Methods
        void Clean()
        {
            ReadDataSource();
        }

        void FocusToNewFile(int precision)
        {
            var fileName = Path.GetFileNameWithoutExtension(Model.DataSourcePath);
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var directoryName = Path.GetDirectoryName(Model.DataSourcePath) + Path.DirectorySeparatorChar;
            var newFile       = int.Parse(fileName) + precision + ".txt";

            var exists = File.Exists(directoryName + newFile);
            if (exists)
            {
                Model.DataSourcePath = directoryName + newFile;
                ReadDataSource();
            }
        }

        void ReadDataSource()
        {
            Model.Lines        = File.ReadAllLines(Model.DataSourcePath).Where(line => string.IsNullOrWhiteSpace(line) == false).ToList();
            Model.SelectedWord = Model.Lines.FirstOrDefault();

            EnToTrCache.StartToCache(Model.Lines);
        }

        void SaveDataSource()
        {
            File.WriteAllLines(Model.DataSourcePath, Model.Lines);
        }
        #endregion
    }
}