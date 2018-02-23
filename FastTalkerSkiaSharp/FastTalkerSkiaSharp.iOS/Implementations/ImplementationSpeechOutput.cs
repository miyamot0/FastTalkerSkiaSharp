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