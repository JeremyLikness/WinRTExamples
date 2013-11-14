namespace ProximityExample.Data
{
    using System;
    using System.Threading.Tasks;

    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    public class PeerSocket : IDisposable
    {
        private StreamSocket socket;

        private DataWriter writer;

        private DataReader reader;

        public event EventHandler<string> MessageRaisedEvent = delegate { };
        public event EventHandler<string> ErrorRaisedEvent = delegate { };

        public PeerSocket(StreamSocket socket)
        {
            this.socket = socket;
            reader = new DataReader(socket.InputStream);
            writer = new DataWriter(socket.OutputStream);
        }

        public async void ReadLoop()
        {
            try
            {
                // Read first 4 bytes (length of the subsequent string).
                var sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                if (sizeFieldCount != sizeof(uint))
                {
                    // The underlying socket was closed before we were able to read the whole data.
                    ErrorRaisedEvent(this, "The socket is no longer available.");
                }

                // Read the string.
                var stringLength = reader.ReadUInt32();
                var actualStringLength = await reader.LoadAsync(stringLength);
                if (stringLength != actualStringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data.
                    ErrorRaisedEvent(this, "Unable to read the data from the socket.");
                }

                var data = reader.ReadString(actualStringLength);
                MessageRaisedEvent(this, data);
                this.ReadLoop();
            }
            catch (Exception ex)
            {
                ErrorRaisedEvent(this, ex.Message);
            }
        }

        public async Task<bool> WriteMessage(string message)
        {
            try
            {
                var size = this.writer.MeasureString(message);
                this.writer.WriteUInt32(size);
                this.writer.WriteString(message);
                var sizeWritten = await this.writer.StoreAsync();
                if (sizeWritten >= size)
                {
                    return true;
                }

                ErrorRaisedEvent(this, "Error writing message");
                return false;
            }
            catch (Exception ex)
            {
                ErrorRaisedEvent(this, ex.Message);
                return false;
            }
        }

        public void Dispose()
        {
            if (this.reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (this.writer != null)
            {
                writer.Dispose();
                writer = null;
            }

            if (this.socket == null)
            {
                return;
            }

            this.socket.Dispose();
            this.socket = null;
        }
    }
}