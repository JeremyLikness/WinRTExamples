using System;
using Windows.Storage;

namespace MultimediaExample
{
    /// <summary>
    /// Wrapper to encapsulate the fuctionality of the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> control being used.
    /// </summary>
    public interface IMediaElementWrapper
    {
        /// <summary>
        /// Sets the given file as the source media to play.
        /// </summary>
        /// <param name="fileToPlay">The file to play.</param>
        void SetSource(IStorageFile fileToPlay);

        /// <summary>
        /// Instructs the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> to start playback.
        /// </summary>
        void Play();

        /// <summary>
        /// Instructs the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> to pause playback.
        /// </summary>
        void Pause();

        /// <summary>
        /// Determines whether the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> can pause.
        /// </summary>
        /// <returns></returns>
        Boolean CanPause();

        /// <summary>
        /// Instructs the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> to stop (and reset) playback.
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets the current position of the the <see cref="Windows.UI.Xaml.Controls.MediaElement"/>.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        TimeSpan CurrentPosition { get; }

        /// <summary>
        /// Offsets the current playback position of the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> by the specified offset.
        /// </summary>
        /// <param name="timeToOffset">The amount time to offset.</param>
        void Offset(TimeSpan timeToOffset);

        /// <summary>
        /// Seeks the current playback position of the the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> to the specified position.
        /// </summary>
        /// <param name="seekPosition">The seek position.</param>
        void Seek(TimeSpan seekPosition);

        /// <summary>
        /// Determines whether the <see cref="Windows.UI.Xaml.Controls.MediaElement"/> can seek.
        /// </summary>
        /// <returns></returns>
        Boolean CanSeek();

        /// <summary>
        /// Sets or clears whether playback for the current <see cref="Windows.UI.Xaml.Controls.MediaElement"/> is in slow motion.
        /// </summary>
        /// <param name="isSlowMotion">if set to <c>true</c> [is slow motion].</param>
        void SetSlowMotion(Boolean isSlowMotion);

        /// <summary>
        /// Determines whether the given file type is supported by the current <see cref="Windows.UI.Xaml.Controls.MediaElement"/>.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <returns></returns>
        Boolean IsFileTypeSupported(String fileType);

        /// <summary>
        /// Adds a marker to the current source in the <see cref="Windows.UI.Xaml.Controls.MediaElement"/>.
        /// </summary>
        /// <param name="name">The marker name.</param>
        /// <param name="markerTime">The marker time.</param>
        void AddMarker(String name, TimeSpan markerTime);

        /// <summary>
        /// Removes the marker at the given time from the current source in the <see cref="Windows.UI.Xaml.Controls.MediaElement"/>.
        /// </summary>
        /// <param name="markerTime">The marker time.</param>
        void RemoveMarker(TimeSpan markerTime);

        /// <summary>
        /// Clears all of the markers from the current source in the <see cref="Windows.UI.Xaml.Controls.MediaElement"/>.
        /// </summary>
        void ClearAllMarkers();
    }
}