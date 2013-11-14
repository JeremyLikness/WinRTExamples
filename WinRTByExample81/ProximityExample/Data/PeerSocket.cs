namespace ProximityExample.Data
{
    using System;
    using System.IO;
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
            var bytesToRead = await this.reader.LoadAsync(sizeof(UInt32));
            if (bytesToRead > 0)
            {
                var length = this.reader.ReadUInt32();
                bytesToRead = await this.reader.LoadAsync(length);
                if (bytesToRead > 0)
                {
                    var message = this.reader.ReadString(length);
                    MessageRaisedEvent(this, message);
                    this.ReadLoop();
                }
            }
            ErrorRaisedEvent(this, "Nothing to read.");
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