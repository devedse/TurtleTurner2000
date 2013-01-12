using DeveConnecteuze.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TutleTurner200.Server;

namespace TurtleTurner2000Server
{
    public class StoreAppListenerRouter
    {
        private Form1 form;
        private StoreAppListener kenServer;

        public Dictionary<String, DeveClient> clients = new Dictionary<string, DeveClient>();

        public StoreAppListenerRouter(Form1 form)
        {
            this.form = form;

            kenServer = new StoreAppListener(1338);
            kenServer.ReceivedMessage += kenServer_ReceivedMessage;
            kenServer.Start();

        }


        void kenServer_ReceivedMessage(object sender, ActionEventArgs e)
        {
            form.DebugMSG(e.Id + ": " + e.Action);
            switch (e.Action)
            {
                case "connected":
                    DeveClient deveClient = new DeveClient("localhost", 1337);
                    deveClient.Start();


                    DeveOutgoingMessage outje = new DeveOutgoingMessage();
                    outje.WriteInt32(0); //Join message
                    outje.WriteInt32(1); //Android
                    deveClient.Send(outje);

                    clients.Add(e.Id, deveClient);

                    break;
                case "closed":
                    clients[e.Id].Stop();

                    clients.Remove(e.Id);
                    //deveServer.messages.Enqueue(inc);

                    break;
                default:
                    DeveClient deveClientNow = clients[e.Id];

                    DeveOutgoingMessage outje2 = new DeveOutgoingMessage();
                    outje2.WriteInt32(1); //Identifier for command message
                    outje2.WriteString(e.Action);
                    deveClientNow.Send(outje2);

                    break;
            }
        }
    }
}
