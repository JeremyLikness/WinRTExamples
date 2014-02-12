using System;
using Windows.Storage;

namespace MultimediaExample
{
    public interface IPlaybackWindow
    {
        void SetSource(IStorageFile fileToPlay);

        void Play();
        void Pause();
        Boolean CanPause();
        void Stop();

        TimeSpan CurrentPosition { get; }

        void Offset(TimeSpan timeToOffset);
        void Seek(TimeSpan seekPosition);
        Boolean CanSeek();

        void SetSlowMotion(Boolean isSlowMotion);

        Boolean IsFileTypeSupported(String fileType);

        void AddMarker(String name, TimeSpan markerTime);

        void RemoveMarker(TimeSpan markerTime);

        void ClearMarkers();
    }
}