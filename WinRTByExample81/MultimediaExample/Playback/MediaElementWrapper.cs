using System;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MultimediaExample
{
    public class MediaElementWrapper : IMediaElementWrapper
    {
        #region Fields

        private readonly MediaElement _mediaElement; 

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaElementWrapper"/> class.
        /// </summary>
        /// <param name="mediaElement">The media element.</param>
        /// <exception cref="System.ArgumentNullException">mediaElement</exception>
        public MediaElementWrapper(MediaElement mediaElement)
        {
            if (mediaElement == null) throw new ArgumentNullException("mediaElement");
            _mediaElement = mediaElement;
        } 

        #endregion

        #region IMediaElementWrapper Implementation

        public void SetIncludedSource()
        {
        }

        /// <summary>
        /// Sets the given file as the source media to play.
        /// </summary>
        /// <param name="fileToPlay">The file to play.</param>
        public async void SetSource(IStorageFile fileToPlay)
        {
            if (fileToPlay != null)
            {
                var stream = await fileToPlay.OpenReadAsync();
                _mediaElement.SetSource(stream, fileToPlay.ContentType);

                // Other "Set Source" alternatives inlcude:

                // Setting the source to a URI:
                //_mediaElement.Source = new Uri("ms-appx:///Assets/Sample.wmv");

                // Using Set Media Stream Source:
                //var mediaStreamSource =
                //    new MediaStreamSource(new VideoStreamDescriptor(VideoEncodingProperties.CreateH264()),
                //        new AudioStreamDescriptor(AudioEncodingProperties.CreateMp3(48000, 2, 32)));
                //mediaStreamSource.SampleRequested += (sender, args) =>
                //                                     {
                //                                         if (args.Request.StreamDescriptor is AudioStreamDescriptor)
                //                                         {
                //                                             var sample = new MediaStreamSample();
                //                                             sample.Buffer.
                //                                         }

                //                                     }
                //_mediaElement.SetMediaStreamSource(mediaStreamSource);
            }
        }

        /// <summary>
        /// Instructs the <see cref="Windows.UI.Xaml.Controls.MediaElement" /> to start playback.
        /// </summary>
        public void Play()
        {
            _mediaElement.Play();
        }

        /// <summary>
        /// Instructs the <see cref="Windows.UI.Xaml.Controls.MediaElement" /> to pause playback.
        /// </summary>
        public void Pause()
        {
            if (_mediaElement.CanPause) _mediaElement.Pause();
        }

        /// <summary>
        /// Determines whether the <see cref="Windows.UI.Xaml.Controls.MediaElement" /> can pause.
        /// </summary>
        /// <returns></returns>
        public Boolean CanPause()
        {
            return _mediaElement.CanPause;
        }

        /// <summary>
        /// Instructs the <see cref="Windows.UI.Xaml.Controls.MediaElement" /> to stop (and reset) playback.
        /// </summary>
        public void Stop()
        {
            _mediaElement.Stop();
        }

        /// <summary>
        /// Gets the current position of the the <see cref="Windows.UI.Xaml.Controls.MediaElement" />.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        public TimeSpan CurrentPosition
        {
            get { return _mediaElement.Position; }
        }

        /// <summary>
        /// Offsets the current playback position of the <see cref="Windows.UI.Xaml.Controls.MediaElement" /> by the specified offset.
        /// </summary>
        /// <param name="timeToOffset">The amount time to offset.</param>
        public void Offset(TimeSpan timeToOffset)
        {
            //PlaybackWindow.SeekCompleted

            // Pause any current playback
            Pause();

            // Determine the new position
            var newPos = _mediaElement.Position + timeToOffset;

            Seek(newPos);
        }

        /// <summary>
        /// Seeks the current playback position of the the <see cref="Windows.UI.Xaml.Controls.MediaElement" /> to the specified position.
        /// </summary>
        /// <param name="seekPosition">The seek position.</param>
        public void Seek(TimeSpan seekPosition)
        {
            // Pause any current playback
            Pause();

            SetPosition(seekPosition);
        }

        /// <summary>
        /// Determines whether the <see cref="Windows.UI.Xaml.Controls.MediaElement" /> can seek.
        /// </summary>
        /// <returns></returns>
        public Boolean CanSeek()
        {
            return _mediaElement.CanSeek;
        }

        /// <summary>
        /// Sets or clears whether playback for the current <see cref="Windows.UI.Xaml.Controls.MediaElement" /> is in slow motion.
        /// </summary>
        /// <param name="isSlowMotion">if set to <c>true</c> [is slow motion].</param>
        public void SetSlowMotion(Boolean isSlowMotion)
        {
            Pause();

            var playbackRate = isSlowMotion ? 0.5 : 1.0;
            _mediaElement.DefaultPlaybackRate = playbackRate;
        }

        /// <summary>
        /// Determines whether the given file type is supported by the current <see cref="Windows.UI.Xaml.Controls.MediaElement" />.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <returns></returns>
        public Boolean IsFileTypeSupported(String fileType)
        {
            return _mediaElement.CanPlayType(fileType) != MediaCanPlayResponse.NotSupported;
        }

        /// <summary>
        /// Adds a marker to the current source in the <see cref="Windows.UI.Xaml.Controls.MediaElement" />.
        /// </summary>
        /// <param name="name">The marker name.</param>
        /// <param name="markerTime">The marker time.</param>
        public void AddMarker(String name, TimeSpan markerTime)
        {
            var marker = new TimelineMarker { Text = name, Time = markerTime };
            _mediaElement.Markers.Add(marker);
        }

        /// <summary>
        /// Removes the marker at the given time from the current source in the <see cref="Windows.UI.Xaml.Controls.MediaElement" />.
        /// </summary>
        /// <param name="markerTime">The marker time.</param>
        public void RemoveMarker(TimeSpan markerTime)
        {
            var matchingMarker = _mediaElement.Markers.FirstOrDefault(x => x.Time == markerTime);
            if (matchingMarker == null) return;
            _mediaElement.Markers.Remove(matchingMarker);
        }

        #endregion

        #region Helper Methods

        private void SetPosition(TimeSpan position)
        {
            // Make sure that seek is an option
            if (!_mediaElement.CanSeek) return;

            // Make sure the new position is "in bounds"
            if (position < TimeSpan.FromMilliseconds(0))
            {
                position = TimeSpan.FromMilliseconds(0);
            }

            // Note that NaturalDuration returns "Automatic" until after 
            // the MediaOpened event has been  raised
            var duration = _mediaElement.NaturalDuration;
            if (duration.HasTimeSpan)
            {
                if (position > duration.TimeSpan) position = duration.TimeSpan;
            }

            _mediaElement.Position = position;
        } 

        #endregion
    }
}