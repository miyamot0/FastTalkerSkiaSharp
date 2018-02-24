/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   Fast Talker is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, version 3.

   Fast Talker is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
   </copyright>

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
        public void SpeakText(string text)
        {
            AVSpeechSynthesizer speechSynthesizer = new AVSpeechSynthesizer();

            AVSpeechUtterance speechUtterance = new AVSpeechUtterance(text)
            {
                Rate = AVSpeechUtterance.MaximumSpeechRate / 3,
                Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
                Volume = 0.9f,
                PitchMultiplier = 1.0f
            };

            speechSynthesizer.SpeakUtterance(speechUtterance);
        }
    }
}