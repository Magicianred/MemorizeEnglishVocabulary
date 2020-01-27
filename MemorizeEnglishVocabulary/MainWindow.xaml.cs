using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindow()
        {
            Model = new MainWindowModel();
            InitializeComponent();
            GoBackButton.Content    = "<";
            GoForwardButton.Content = ">";

            _vocabularyList.SelectionChanged += _vocabularyList_SelectionChanged;

            _sesliSozlükWebBrowser.FrameLoadEnd += (s, e) => { FocusSelectedItem(); };
            _longManWebBrowser.FrameLoadEnd     += (s, e) => { FocusSelectedItem(); };

            _vocabularyList.KeyDown += _vocabularyList_KeyDown;

            _longManWebBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler_For_Longman();

            _sesliSozlükWebBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler_For_SesliSözlük();

            _googleTranslate.RenderProcessMessageHandler = new RenderProcessMessageHandler_For_Google_Translate();

            _imageBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler_For_Google_Image();

            Loaded += (s, e) => { FireAction(Action.Loaded); };

            Closed += KillAllContainer;
        }
        #endregion

        #region Enums
        enum Action
        {
            Loaded,
            GoBack,
            GoForward,
            DeleteSelectedWord,
            NavigateToWord
        }
        #endregion

        #region Methods
        static void KillAllContainer(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcesses())
            {
                var name = typeof(MainWindow).Assembly.GetName().Name;
                if (process.ProcessName == name)
                {
                    process.Kill();
                }
            }
        }

        void _currentWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Model.SelectedWord = _currentWord.Text;
                NavigateToWord();
            }
        }

        void _vocabularyList_KeyDown(object sender, KeyEventArgs e)
        {
            var listBox = (ListBox) sender;

            if (e.Key == Key.Up)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex = listBox.SelectedIndex - 1;
                }
            }

            if (e.Key == Key.Down)
            {
                if (listBox.SelectedIndex < listBox.Items.Count - 1)
                {
                    listBox.SelectedIndex = listBox.SelectedIndex + 1;
                }
            }
        }

        void _vocabularyList_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                FireAction(Action.DeleteSelectedWord);
            }

            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Copy();
            }
        }

        void _vocabularyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.SelectedWord = _vocabularyList.SelectedItem?.ToString();

            _currentWord.Text = Model.SelectedWord + "";

            NavigateToWord();
        }

        void Copy()
        {
            try
            {
                var sb = new StringBuilder();
                foreach (var row in _vocabularyList.SelectedItems)
                {
                    sb.Append(row);
                    sb.AppendLine();
                }

                sb.Remove(sb.Length - 1, 1); // Just to avoid copying last empty row
                Clipboard.SetText(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void FireAction(Action action)
        {
            var controller = new MainWindowController
            {
                Model = Model
            };

            var methodInfo = controller.GetType().GetMethod(action.ToString());
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            methodInfo.Invoke(controller, null);

            OnPropertyChanged(nameof(Model));

            var renderer = _longManWebBrowser.RenderProcessMessageHandler as RenderProcessMessageHandler_For_Longman;
            if (renderer != null)
            {
                renderer.InitialJsScript = Model.LongManInitialJsScript;
            }
        }

        void FocusSelectedItem()
        {
            Dispatcher.InvokeAsync(() => { _vocabularyList.Focus(); });
        }

        void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            FireAction(Action.GoBack);
        }

        void GoForwardButton_Click(object sender, RoutedEventArgs e)
        {
            FireAction(Action.GoForward);
        }

        void NavigateToWord()
        {
            FireAction(Action.NavigateToWord);
        }
        #endregion

        #region MainWindowModel Model
        MainWindowModel _model;

        public MainWindowModel Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}