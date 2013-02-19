namespace TemplatesExample
{
    /// <summary>
    /// An instance of a message which includes the text and a type
    /// </summary>
    public class MessageInstance
    {
        /// <summary>
        /// Gets or sets the type of the message
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// Gets or sets the text of the message
        /// </summary>
        public string Message { get; set; }
    }
}
