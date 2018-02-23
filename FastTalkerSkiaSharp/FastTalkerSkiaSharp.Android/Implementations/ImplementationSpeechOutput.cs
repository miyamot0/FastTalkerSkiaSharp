﻿using System;
using System.Collections.Generic;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Speech.Tts;
using FastTalkerSkiaSharp.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(FastTalkerSkiaSharp.Droid.Implementations.ImplementationSpeechOutput))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
    public class ImplementationSpeechOutput : Java.Lang.Object, InterfaceSpeechOutput, TextToSpeech.IOnInitListener
    {
        TextToSpeech speaker;
        string toSpeak;

        public ImplementationSpeechOutput() { }

        [Obsolete("Message")]
        public void SpeakText(string text)
        {
            var ctx = Forms.Context;
            toSpeak = text;
            if (speaker == null)
            {
                AudioManager am = (AudioManager)ctx.GetSystemService(Context.AudioService);
                int amStreamMusicMaxVol = am.GetStreamMaxVolume(Android.Media.Stream.Music);
                am.SetStreamVolume(Stream.Music, amStreamMusicMaxVol, 0);
                speaker = new TextToSpeech(ctx, this);
            }
            else
            {
                SpeakRoute(toSpeak);
            }
        }

        #region IOnInitListener implementation

        /// <summary>
        /// After init, echo out speech
        /// </summary>
        /// <param name="status"></param>
        [Obsolete("Message")]
        public void OnInit(OperationResult status)
        {
            if (status.Equals(OperationResult.Success))
            {
                SpeakRoute(toSpeak);
            }
        }

        #endregion

        /// <summary>
        /// Route speech to suitable Api 
        /// </summary>
        /// <param name="text"></param>
        [Obsolete("Message")]
        private void SpeakRoute(string text)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                ApiOver21(text);
            }
            else
            {
                ApiUnder20(text);
            }
        }

        /// <summary>
        /// Obsolete method call for TTS, necessary for legacy devices
        /// </summary>
        /// <param name="text"></param>
        [Obsolete("Message")]
        private void ApiUnder20(string text)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(TextToSpeech.Engine.KeyParamUtteranceId, "MessageId");
            speaker.Speak(text, QueueMode.Flush, map);
        }

        /// <summary>
        /// More suited TTS call, for lollipop and greater
        /// </summary>
        /// <param name="text"></param>
        private void ApiOver21(string text)
        {
            string utteranceId = this.GetHashCode() + "";
            speaker.Speak(text, QueueMode.Flush, null, utteranceId);
        }
    }
}