using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace DeveConnecteuze.Network
{
    public class DeveClientStreamSocket : DevePeerStreamSocket
    {
        private DeveConnectionStreamSocket connection;
        //private Boolean shouldShutdown = false;

        private String iporhost;
        private int port;

        public DeveClientStreamSocket(String iporhost, int port)
        {
            this.iporhost = iporhost;
            this.port = port;
        }

        public override void Start()
        {
            TcpClient client = new TcpClient();
            client.Connect(iporhost, port);
            connection = new DeveConnectionStreamSocket(client, this);
            connection.Start();
        }

        public override void Stop()
        {
            connection.Stop();
        }

        public void Send(DeveOutgoingMessageStreamSocket devOut)
        {
            connection.Send(devOut);
        }
    }
}
