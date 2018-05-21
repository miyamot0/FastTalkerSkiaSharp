/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

[assembly: Xamarin.Forms.Dependency(typeof(FastTalkerSkiaSharp.iOS.Implementations.ImplementationSpeechOutput))]
namespace FastTalkerSkiaSharp.iOS.Implementations
{
    public class ImplementationSpeechOutput : FastTalkerSkiaSharp.Interfaces.InterfaceSpeechOutput
    {
        AVFoundation.AVSpeechSynthesizer speechSynthesizer;
        AVFoundation.AVSpeechUtterance speechUtterance;

        public void SpeakText(string text)
        {
            if (speechSynthesizer == null)
            {
                speechSynthesizer = new AVFoundation.AVSpeechSynthesizer();
            }

            if (!speechSynthesizer.Speaking)
            {
                speechUtterance = new AVFoundation.AVSpeechUtterance(text)
                {
                    Rate = AVFoundation.AVSpeechUtterance.MaximumSpeechRate / 3,
                    Voice = AVFoundation.AVSpeechSynthesisVoice.FromLanguage("en-US"),
                    Volume = 0.9f,
                    PitchMultiplier = 1.0f
                };

                speechSynthesizer.SpeakUtterance(speechUtterance);
            }
        }
    }
}