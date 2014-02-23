namespace MultimediaExample
{
    public class DesignFileMarkerViewModel : FileMarkerViewModel
    {
        public DesignFileMarkerViewModel()
            : base(new MultimediaViewModel(null), new FileMarker())
        {
        }
    }
}