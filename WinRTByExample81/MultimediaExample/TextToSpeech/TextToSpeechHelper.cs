using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace MultimediaExample
{
    public class TextToSpeechHelper
    {
        public static async void PlayContentAsync(String content, Boolean isSsml, String voiceId)
        {
            content = content ?? String.Empty;

            var selectedVoice = Voices.FirstOrDefault(x => x.Id == voiceId) ??
                                DefaultVoice;

            using (var synthesizer = new SpeechSynthesizer {Voice = selectedVoice})
            {
                var voiceStream = isSsml
                    ? await synthesizer.SynthesizeSsmlToStreamAsync(content)
                    : await synthesizer.SynthesizeTextToStreamAsync(content);
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