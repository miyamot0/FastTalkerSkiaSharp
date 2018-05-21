/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

[assembly: Xamarin.Forms.Dependency(typeof(FastTalkerSkiaSharp.Droid.Implementations.ImplementationSpeechOutput))]
namespace FastTalkerSkiaSharp.Droid.Implementations
{
#pragma warning disable CS0618
    public class ImplementationSpeechOutput : Java.Lang.Object, FastTalkerSkiaSharp.Interfaces.InterfaceSpeechOutput, Android.Speech.Tts.TextToSpeech.IOnInitListener
#pragma warning restore CS0618 
    {
        Android.Speech.Tts.TextToSpeech speaker;
        string toSpeak;

        public void SpeakText(string text)
        {
            toSpeak = text;

            if (speaker == null)
            {
                Android.Media.AudioManager am = (Android.Media.AudioManager) Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.GetSystemService(Android.Content.Context.AudioService);
                int amStreamMusicMaxVol = am.GetStreamMaxVolume(Android.Media.Stream.Music);
                am.SetStreamVolume(Android.Media.Stream.Music, amStreamMusicMaxVol, 0);
                speaker = new Android.Speech.Tts.TextToSpeech(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, this);
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
        public void OnInit(Android.Speech.Tts.OperationResult status)
        {
            if (status.Equals(Android.Speech.Tts.OperationResult.Success))
            {
                SpeakRoute(toSpeak);
            }
        }

        #endregion

        /// <summary>
        /// Route speech to suitable Api 
        /// </summary>
        /// <param name="text"></param>
        private void SpeakRoute(string text)
        {
            if (speaker.IsSpeaking) return;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
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
        private void ApiUnder20(string text)
        {
            System.Collections.Generic.Dictionary<string, string> map = new System.Collections.Generic.Dictionary<string, string>();
            map.Add(Android.Speech.Tts.TextToSpeech.Engine.KeyParamUtteranceId, "MessageId");
#pragma warning disable CS0618 // Legacy support
            speaker.Speak(text, Android.Speech.Tts.QueueMode.Flush, map);
#pragma warning restore  CS0618
        }

        /// <summary>
        /// More suited TTS call, for lollipop and greater
        /// </summary>
        /// <param name="text"></param>
        private void ApiOver21(string text)
        {
            string utteranceId = this.GetHashCode() + "";
            speaker.Speak(text, Android.Speech.Tts.QueueMode.Flush, null, utteranceId);
        }
    }
}