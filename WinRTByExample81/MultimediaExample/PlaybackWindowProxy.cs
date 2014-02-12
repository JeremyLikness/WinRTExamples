using System;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MultimediaExample
{
    public class PlaybackWindowProxy : IPlaybackWindow
    {
        private MediaElement _playbackWindow;

        public void Initialize(MediaElement playbackWindow)
        {
            if (playbackWindow == null) throw new ArgumentNullException("playbackWindow");
            _playbackWindow = playbackWindow;
        }

        public async void SetSource(IStorageFile fileToPlay)
        {
            if (fileToPlay != null)
            {
                var stream = await fileToPlay.OpenReadAsync();
                _playbackWindow.SetSource(stream, fileToPlay.ContentType);
                //_playbackWindow.SetMediaStreamSource();
                //_playbackWindow.SetSource(IRandomAccessStream, mimeType);
                //_playbackWindow.Source = Uri
            }
        }

        public void Play()
        {
            _playbackWindow.Play();
        }

        public void Pause()
        {
            if (CanPause()) _playbackWindow.Pause();
        }

        public Boolean CanPause()
        {
            return _playbackWindow.CanPause;
        }

        public void Stop()
        {
            _playbackWindow.Stop();
        }

        public TimeSpan CurrentPosition
        {
            get { return _playbackWindow.Position; }
        }

        public void Offset(TimeSpan timeToOffset)
        {
            //PlaybackWindow.SeekCompleted

            // Make sure that seek is an option
            if (!_playbackWindow.CanSeek) return;

            // Pause any current playback
            Pause();

            // Determine the new position
            var newPos = _playbackWindow.Position + timeToOffset;

            Seek(newPos);
        }

        public void Seek(TimeSpan seekPosition)
        {
            if (!_playbackWindow.CanSeek) return;

            // Pause any current playback
            Pause();

            SetPosition(seekPosition);
        }

        private void SetPosition(TimeSpan position)
        {
            // Make sure the new position is "in bounds"
            if (position < TimeSpan.FromMilliseconds(0))
            {
                position = TimeSpan.FromMilliseconds(0);
            }

            // Note that NaturalDuration is "Automatic" until after MediaOpened event is raised
            var duration = _playbackWindow.NaturalDuration;
            if (duration.HasTimeSpan)
            {
                if (position > duration.TimeSpan) position = duration.TimeSpan;
            }


            _playbackWindow.Position = position;
        }

        public Boolean CanSeek()
        {
            return _playbackWindow.CanSeek;
        }

        public void SetSlowMotion(Boolean isSlowMotion)
        {
            Pause();

            var playbackRate = isSlowMotion ? 0.5 : 1.0;
            _playbackWindow.DefaultPlaybackRate = playbackRate;

            //PlaybackWindow.PlaybackRate
            //PlaybackWindow.RateChanged
        }

        public Boolean IsFileTypeSupported(String fileType)
        {
            return _playbackWindow.CanPlayType(fileType) != MediaCanPlayResponse.NotSupported;
        }

        public void AddMarker(String name, TimeSpan markerTime)
        {
            var marker = new TimelineMarker { Text = name, Time = markerTime };
            _playbackWindow.Markers.Add(marker);
        }

        public void RemoveMarker(TimeSpan markerTime)
        {
            var matchingMarker = _playbackWindow.Markers.FirstOrDefault(x => x.Time == markerTime);
            if (matchingMarker == null) return;
            _playbackWindow.Markers.Remove(matchingMarker);
        }

        public void ClearMarkers()
        {
            _playbackWindow.Markers.Clear();
        }
    }
}