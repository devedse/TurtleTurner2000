using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace TutleTurner200.Server
{
    public class StoreAppListener
    {
        private int _port;

        public delegate void ReceivedMessageEventHandler(object sender, ActionEventArgs e);


        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event ReceivedMessageEventHandler ReceivedMessage;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnReceivedMessage(ActionEventArgs e)
        {
            if (ReceivedMessage != null)
                ReceivedMessage(this, e);
        }

        public StoreAppListener(int port)
        {
            _port = port;
        }

        public async Task<bool> Start()
        {
            StreamSocketListener listener = new StreamSocketListener();
            listener.ConnectionReceived += OnConnection;

            // Start listen operation.
            try
            {
                await listener.BindServiceNameAsync(_port.ToString());
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    return false;
                }
            }

            return true;
        }

        private async void OnConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string id = string.Empty;

            DataReader reader = new DataReader(args.Socket.InputStream);
            try
            {
                while (true)
                {
                    // Read first 4 bytes (length of the subsequent string).
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Read the string.
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal the text back to the UI thread.
                    string readedString = reader.ReadString(actualStringLength);
                    

                    string action = readedString.Split(';')[0].Split(':')[1].ToString();
                    id = readedString.Split(';')[1].Split(':')[1].ToString();

                    OnReceivedMessage(new ActionEventArgs(action, id));
                }
            }
            catch (Exception exception)
            {
                OnReceivedMessage(new ActionEventArgs("closed", id));
            }
        }
    }

    public class ActionEventArgs : EventArgs
    {
        public readonly string Action;
        public readonly string Id;

        public ActionEventArgs(string action, string id)
        {
            Action = action;
            Id = id;
        }
    }

}
