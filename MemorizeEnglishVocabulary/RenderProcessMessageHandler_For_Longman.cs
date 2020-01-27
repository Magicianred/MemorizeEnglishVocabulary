using CefSharp;

namespace WpfApp2
{
    public class RenderProcessMessageHandler_For_Longman : IRenderProcessMessageHandler
    {
        #region Public Properties
        public string InitialJsScript { get; set; }
        #endregion

        #region Public Methods
        public void OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }

        public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node)
        {
        }

        public void OnUncaughtException(IWebBrowser browserControl, IBrowser browser, IFrame frame, JavascriptException exception)
        {
        }
        #endregion

        #region Explicit Interface Methods
        // Wait for the underlying `Javascript Context` to be created, this is only called for the main frame.
        // If the page has no javascript, no context will be created.
        void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            frame.ExecuteJavaScriptAsync(InitialJsScript);
        }
        #endregion
    }
}