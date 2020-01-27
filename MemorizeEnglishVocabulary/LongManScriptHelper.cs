using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows;

namespace WpfApp2
{
    static class LongManScriptHelper
    {
        static string GetMapAsJsObject(IDictionary<string, string> EnToTrMap)
        {
            var sb = new StringBuilder();
            if (EnToTrMap == null)
            {
                return "{}";
            }

            sb.AppendLine("var EnToTrMap ={};");
            foreach (var pair in EnToTrMap)
            {
                var key = pair.Key;
                if (key.Contains("'"))
                {
                    MessageBox.Show("Problem into embedd js " + key);
                }

                var value = pair.Value.Replace("'", "''").Replace("\"", "''");

                sb.AppendLine("EnToTrMap['" + key + "'] = '" + value + "';");
            }

            return sb.ToString();
        }

        public static string GetScript(string word)
        {
            var enToTrMap = GetMapAsJsObject(EnToTrCache.TryGetForWord(word));

            var PlaybackRate                  = ConfigurationManager.AppSettings.Get("PlaybackRate").ToDecimal();
            var WaitMillisecondBetweenSamples = ConfigurationManager.AppSettings.Get("WaitMillisecondBetweenSamples").ToDecimal();

            string script = @"




var CheckTimeout = 100;
var StartProcess = function(fn)
{
	var isSuccess = fn();
	if(isSuccess)
	{
		return;
	}
	
	setTimeout(fn, 100);
}

var TryToRemoveExtraSamples = function()
{
	var word = $('.pagetitle').html();
	if(word == null )
	{
		return false;
	}
	
	for(var i = 1; i < 150; i++)
    {
        if( i < 2 )
        {
            continue;
        }

        var item = $('#'+word.replace(' ','-')+'__'+i);
        if(item.length === 0)
        {
            break;
        }

        item.remove();
    }
	
	return true;
}

var TryToRemoveExtra_dictentry = function()
{
	var dictentryArr = $('.dictentry');
	if(dictentryArr.length === 0)
	{
		return false;
	}
	if( dictentryArr.length > 1 )
	{
		dictentryArr.last().remove(); 
	} 
	
	return true;
}

var RemoveQueue = [
'.header',
'.footer',
'.responsive_cell2',
'body > div.content > div.responsive_cell6 > div.entry_content > div > span > span.dictionary_intro.span',
'.Tail',
'.assetlink',
'.etym',
'#ad_btmslot',
'.asset > div',
'.HYPHENATION',
'.HOMNUM',
'.PronCodes',
'body > div.content > div.responsive_cell6 > div.entry_content > div > span > span.dictlink > span > span.frequent.Head > span.tooltip.LEVEL',
'body > div.content > div.responsive_cell6 > div.entry_content > div > span > span.dictlink > span > span.frequent.Head > span.FREQ'

];
var RunRemoveQueue = function()
{
	if(RemoveQueue.length === 0)
	{
		return true;
	}
	
	for(var i=0; i < RemoveQueue.length; i++)
	{
		var selector = RemoveQueue[i];
		
		var q = $(selector);
		
		console.log($(selector));
		
		if(q.length > 0)
		{
			$(selector).remove();
			RemoveQueue.splice(i,1);
            
		}
	}
}
var onDomReady = function()
{
	
	StartProcess(TryToRemoveExtraSamples);
	StartProcess(TryToRemoveExtra_dictentry);
	StartProcess(RunRemoveQueue);

    




    

   




   


" + enToTrMap + @"





var currentExampleElement = null;

var SamplePlayCountMap = {};
var SampleMaxPlayCount = 2;

var tryPlay = function(index)
{	
	var exampleElements = $('.EXAMPLE');
	
	var exampleElement = exampleElements.get(index);
	if(exampleElement == null || index > 2)
	{
		return;		
	}
	
	exampleElement = $(exampleElement);


    var enKey   = exampleElement.text().trim();
    var trValue = EnToTrMap[enKey];

    // alert(enKey);
    // alert(EnToTrMap[enKey]);

    if(trValue)
    {
        exampleElement.append('<br>').append(document.createTextNode('*'+trValue+'*'));
    }


	if (currentExampleElement)
    {
        currentExampleElement.css({background:''});
    }

	currentExampleElement = exampleElement;
	
	currentExampleElement.css({background:'yellow'});
	
	
	
    var element = exampleElement.find(' > span');
	

    var src_mp3 = element.attr('data-src-mp3');
    var audio   = new Audio(src_mp3);

    if ( SamplePlayCountMap[index] === undefined )
    {
        SamplePlayCountMap[index] = 1;
    }
    else
    {
        SamplePlayCountMap[index] = SamplePlayCountMap[index] + 1;
    }

    var playCount = SamplePlayCountMap[index];

    audio.playbackRate = " + PlaybackRate + @";
    audio.play();
    audio.addEventListener('ended', function()
	{
        setTimeout( function()
                    {
                        if(playCount >= SampleMaxPlayCount)
                        {
                            index++;
                        }
                        tryPlay(index);

                    }, " + WaitMillisecondBetweenSamples + @" );
    });	
}


var PlayAmericanPronuncition = function(playCount, callback)
{
    var americanPronunciation = $('.amefile');
	if( americanPronunciation.length === 0)
	{
		setTimeout(function(){ PlayAmericanPronuncition(playCount,callback);  },100);
		return;
	}
	
    var americanPronunciationAudio   = new Audio(americanPronunciation.attr('data-src-mp3'));

    americanPronunciationAudio.playbackRate = 0.9;
    americanPronunciationAudio.play();
    americanPronunciationAudio.addEventListener('ended', function()
    {
        if(playCount === 0)
        {
            setTimeout(StartToPlaySamples, 1000);
        }
        else
        {
            setTimeout(function(){  PlayAmericanPronuncition(playCount - 1, callback);   }, 700);            
        }
	   
    });	
}

PlayAmericanPronuncition(3, function(){  setTimeout( StartToPlaySamples,1000);   });

function StartToPlaySamples()
{
    tryPlay(0);
}


};

document.addEventListener('DOMContentLoaded', onDomReady);
// $(onDomReady);


";

            return script;
        }
    }
}