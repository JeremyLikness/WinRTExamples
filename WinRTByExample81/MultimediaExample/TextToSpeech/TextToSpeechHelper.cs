using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace MultimediaExample
{
    public class TextToSpeechHelper
    {
        public static async void SpeakContentAsync
            (String content, Boolean isSsml, String voiceId)
        {
            if (String.IsNullOrWhiteSpace(content)) return;

            // Find the voice with the matching Id or just use the default
            var voice = Voices.FirstOrDefault(x => x.Id == voiceId) ??
                                DefaultVoice;

            using (var synthesizer = new SpeechSynthesizer {Voice = voice})
            {
                // NOTE - if Synthesize___ToStreamAsync throws an Access Denied exception, 
                // there is a known problem with some fresh Windows 8.1 installations.  
                // Additional information about the problem and its remedy (clearing 
                // a bad permission entry for a registry value) can be found at http://j.mp/1o2eeJL

                // Get the voice stream for the given text
                var voiceStream = isSsml
                    ? await synthesizer.SynthesizeSsmlToStreamAsync(content)
                    : await synthesizer.SynthesizeTextToStreamAsync(content);

                // Create a new MediaElement and use it to play the voice stream
                var mediaElement = new MediaElement();
                mediaElement.SetSource(voiceStream, voiceStream.ContentType);
                mediaElement.Play();
            }
        }

        public static IEnumerable<VoiceInformation> Voices
        {
            get { return SpeechSynthesizer.AllVoices; }
        }

        public static VoiceInformation DefaultVoice
        {
            get { return SpeechSynthesizer.DefaultVoice; }
        }
    }
}