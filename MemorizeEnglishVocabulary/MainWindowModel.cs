using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp2
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        #region Public Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Public Properties
        public string       DataSourcePath { get; set; }
        public List<string> Lines          { get; set; }
        public string       SelectedWord   { get; set; }
        public string GoogleTranslateAddress { get; set; }
        public string AudioDictionaryAddress { get; set; }
        public string LongManWebBrowserAddress { get; set; }
        public string ImageBrowserAddress { get; set; }
        public string LongManInitialJsScript { get; set; }

        public string Sample1En { get; set; }
        public string Sample2En { get; set; }
        public string Sample3En { get; set; }

        public string Sample1Tr { get; set; }
        public string Sample2Tr { get; set; }
        public string Sample3Tr { get; set; }

        #endregion

        #region Methods
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}