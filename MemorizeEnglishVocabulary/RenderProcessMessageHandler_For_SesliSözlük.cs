using CefSharp;

namespace WpfApp2
{
    public class RenderProcessMessageHandler_For_SesliSözlük : IRenderProcessMessageHandler
    {
        // Wait for the underlying `Javascript Context` to be created, this is only called for the main frame.
        // If the page has no javascript, no context will be created.
        void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            const string script = @"

document.addEventListener('DOMContentLoaded',function(){


$('#wrap > div.high.o9').remove();
$('#wrap > div > div:nth-child(1)').remove();
$('#wrap > div > div.row.mTop20').remove();

$('#ss-section-left > div > div > div.panel.panel-default.clear-bottom.trans-box-shadow > div.panel-body.sozluk > dl > dt:nth-child(1)').remove();

$('#ss-section-left > div > div > div.panel.panel-default.clear-bottom.trans-box-shadow > div.panel-body.sozluk > dl > dd').each(function(index,element){

	if(index > 5 || $(element).select(' p > q').length === 0 )
	{
		$(element).remove();
	}
	
	var sample = $(element).find('p > q');
	
	if(sample === undefined || sample.length === 0 )
	{
		$(element).remove();
	}
	

});


$('#ss-section-left > div:nth-child(1) > div > div.panel.panel-default.clear-bottom.trans-box-shadow > div.panel-body.sozluk > dl > h3').remove();


$('#ss-section-left > div:nth-child(1) > div > div.panel.panel-default.clear-bottom.trans-box-shadow > div.panel-body.sozluk > dl > dt').remove();



$(function(){
$('#ss-section-left > div:nth-child(2)').remove();
});

$(function(){
$('#ss-section-left > div:nth-child(2)').remove();


});

$(function(){
	$('#ss-section-left > div:nth-child(2)').remove();   
});

$(function(){
	$('#ss-section-right').remove();   
});

$('#footer').remove();  

$(function(){
	$('#ss-section-left > div > div > div.panel.panel-default.clear-bottom.trans-box-shadow > div.panel-heading.sesli-red-bg').remove();   
});

});



";

            frame.ExecuteJavaScriptAsync(script);


            browser.SetZoomLevel(1.2);
        }

        public void OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node)
        {

        }

        public void OnUncaughtException(IWebBrowser browserControl, IBrowser browser, IFrame frame, JavascriptException exception)
        {

        }
    }
}