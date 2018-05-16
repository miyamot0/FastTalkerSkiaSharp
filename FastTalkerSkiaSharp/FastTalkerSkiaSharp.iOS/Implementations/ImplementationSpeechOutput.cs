/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using AVFoundation;
using FastTalkerSkiaSharp.Interfaces;
using FastTalkerSkiaSharp.iOS.Implementations;

[assembly: Xamarin.Forms.Dependency(typeof(ImplementationSpeechOutput))]
namespace FastTalkerSkiaSharp.iOS.Implementations
{
    public class ImplementationSpeechOutput : InterfaceSpeechOutput
    {
        private AVSpeechSynthesizer speechSynthesizer;

        private AVSpeechUtterance speechUtterance;

        private bool isThisCurrentlySpeaking = false;

        public ImplementationSpeechOutput() { }

        public void SpeakText(string text)
        {
            if (speechSynthesizer == null)
            {
                speechSynthesizer = new AVSpeechSynthesizer();
            }

            if (!isThisCurrentlySpeaking)
            {
                isThisCurrentlySpeaking = true;

                speechUtterance = new AVSpeechUtterance(text)
                {
                    Rate = AVSpeechUtterance.MaximumSpeechRate / 3,
                    Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
                    Volume = 0.9f,
                    PitchMultiplier = 1.0f
                };
                speechSynthesizer.DidFinishSpeechUtterance += FinishedSpeaking;
                speechSynthesizer.DidCancelSpeechUtterance += FinishedSpeaking;
                speechSynthesizer.SpeakUtterance(speechUtterance);
            }
        }

        /// <summary>
        /// Prevent relentless chains of text from being emitted
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void FinishedSpeaking(object sender, AVSpeechSynthesizerUteranceEventArgs e)
        {
            isThisCurrentlySpeaking = false;
        }
    }
}