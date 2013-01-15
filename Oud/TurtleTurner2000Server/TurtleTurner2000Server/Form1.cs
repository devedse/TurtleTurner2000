using DeveConnecteuze.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TutleTurner200.Server;

namespace TurtleTurner2000Server
{
    public partial class Form1 : Form
    {
        private DeveServer deveServer;
        private StoreAppListenerRouter salc;


        private Random r = new Random();

        private Dictionary<DeveConnection, Clientje> allClientjes = new Dictionary<DeveConnection, Clientje>();
        private Dictionary<DeveConnection, ControlClientje> controlClientjes = new Dictionary<DeveConnection, ControlClientje>();
        private Dictionary<DeveConnection, ScreenClientje> screenClientjes = new Dictionary<DeveConnection, ScreenClientje>();

        int curNumberOfScreen = 0;

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = timer1.Interval.ToString();
            deveServer = new DeveServer(1337);
            deveServer.Start();

            salc = new StoreAppListenerRouter(this);


            Task.Run(() => Runner());
        }


        public void Runner()
        {
            while (true)
            {
                DeveIncomingMessage inc;
                if ((inc = deveServer.ReadMessage()) != null)
                {
                    switch (inc.MessageType)
                    {
                        case DeveMessageType.KeepAlive:
                            break;
                        case DeveMessageType.Data:

                            int hetTypeMessage = inc.ReadInt32();
                            if (hetTypeMessage == 0) //Een join message
                            {
                                int hetTypeDevice = inc.ReadInt32();
                                if (hetTypeDevice == 0) //Een screen client
                                {
                                    DebugMSG("Het is een screen client :D:D:D");

                                    ScreenClientje screenClientje = new ScreenClientje(inc.Sender, curNumberOfScreen);
                                    screenClientjes.Add(inc.Sender, screenClientje);
                                    allClientjes.Add(inc.Sender, screenClientje);
                                    curNumberOfScreen++;

                                    SendNewAreas();
                                }
                                else if (hetTypeDevice == 1) //Android
                                {
                                    DebugMSG("Het is een Android :)");

                                    ControlClientje controlClientje = new ControlClientje(inc.Sender);
                                    controlClientjes.Add(inc.Sender, controlClientje);
                                    allClientjes.Add(inc.Sender, controlClientje);

                                    DeveOutgoingMessage outje = new DeveOutgoingMessage();
                                    outje.WriteInt32(1); //Add beestje bij alle screens
                                    outje.WriteString(controlClientje.guid);

                                    SendToScreens(outje);
                                }
                            }
                            else if (hetTypeMessage == 1) //Een message met button klik stuff
                            {

                                ControlClientje curControlClient = controlClientjes[inc.Sender];

                                String direction = inc.ReadString();

                                DebugMSG("Got message with: " + direction + " from: " + inc.Sender);

                                if (direction == "left")
                                {
                                    curControlClient.posx -= 50;
                                }
                                else if (direction == "right")
                                {
                                    curControlClient.posx += 50;
                                }
                                else if (direction == "up")
                                {
                                    curControlClient.posy -= 50;
                                }
                                else if (direction == "down")
                                {
                                    curControlClient.posy += 50;
                                }

                                DebugMSG("X: " + curControlClient.posx + " Y: " + curControlClient.posy);

                                DeveOutgoingMessage outje = new DeveOutgoingMessage();
                                outje.WriteInt32(3);
                                outje.WriteString(curControlClient.guid);
                                outje.WriteInt32(curControlClient.posx);
                                outje.WriteInt32(curControlClient.posy);

                                SendToScreens(outje);
                            }

                            break;
                        case DeveMessageType.StatusChanged:
                            byte newStatus = inc.ReadByte();
                            NetworkStatus ns = (NetworkStatus)newStatus;
                            switch (ns)
                            {
                                case NetworkStatus.Connected:

                                    DebugMSG("Er connect iets :O");
                                    break;
                                case NetworkStatus.Disconnected:

                                    if (controlClientjes.ContainsKey(inc.Sender))
                                    {
                                        ControlClientje controlClientje = controlClientjes[inc.Sender];
                                        DeveOutgoingMessage outje = new DeveOutgoingMessage();
                                        outje.WriteInt32(2);
                                        outje.WriteString(controlClientje.guid);
                                        SendToScreens(outje);

                                    }

                                    RemoveFromAllClientLists(inc.Sender);


                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }

        public void SendToScreens(DeveOutgoingMessage outje)
        {
            foreach (var screenClientje in screenClientjes.Values)
            {
                screenClientje.deveConnection.Send(outje);
            }
        }

        public void RemoveFromAllClientLists(DeveConnection deveConnection)
        {
            if (allClientjes.ContainsKey(deveConnection))
            {
                allClientjes.Remove(deveConnection);
            }
            if (controlClientjes.ContainsKey(deveConnection))
            {
                controlClientjes.Remove(deveConnection);
            }
            if (screenClientjes.ContainsKey(deveConnection))
            {
                screenClientjes.Remove(deveConnection);
            }
        }

        public void DebugMSG(String msg)
        {
            this.Invoke(new Action(() =>
            {
                listBox1.Items.Insert(0, msg);
            }));
        }

        public void SendNewAreas()
        {
            Graphics g = panel1.CreateGraphics();
            g.FillRectangle(Brushes.CornflowerBlue, new Rectangle(0, 0, 1000000, 1000000));


            List<ScreenClientje> screenClients = screenClientjes.Values.ToList();
            List<ScreenClientje> sortedScreenClients = screenClients.OrderBy((x) => x.numberOfScreen).ToList();



            Rectangle totSize = new Rectangle();
            totSize.X = 0;
            totSize.Y = 0;
            totSize.Width = sortedScreenClients.Count * 1920;
            totSize.Height = sortedScreenClients.Count * 1080;

            for (int i = 0; i < sortedScreenClients.Count; i++)
            {
                Clientje curClient = sortedScreenClients[i];

                Rectangle curSize = new Rectangle();
                curSize.X = i * 1920;
                curSize.Y = 0;
                curSize.Width = 1920;
                curSize.Height = 1080;

                DeveOutgoingMessage outje = new DeveOutgoingMessage();
                outje.WriteInt32(0); //Identifier

                outje.WriteInt32(totSize.X);
                outje.WriteInt32(totSize.Y);
                outje.WriteInt32(totSize.Width);
                outje.WriteInt32(totSize.Height);

                outje.WriteInt32(curSize.X);
                outje.WriteInt32(curSize.Y);
                outje.WriteInt32(curSize.Width);
                outje.WriteInt32(curSize.Height);

                drawFatRect(Pens.Red, g, curSize, 5, i.ToString());


                curClient.deveConnection.Send(outje);
            }
        }

        private void drawFatRect(Pen pen, Graphics g, Rectangle r, int fadity, String stringinside)
        {
            int delendoor = 10;

            for (int i = 0; i < fadity; i++)
            {
                g.DrawRectangle(pen, r.X / delendoor + i, r.Y / delendoor + i, r.Width / delendoor - 2 * i, r.Height / delendoor - 2 * i);
            }
            Font drawFont = new Font("Arial", 16);

            g.DrawString(stringinside, drawFont, Brushes.White, new Point(r.X / delendoor + 20, r.Y / delendoor + 20));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SendRandomSquirtle();
        }

        //public void SendRandomSquirtle()
        //{
        //    DeveOutgoingMessage outje = new DeveOutgoingMessage();

        //    outje.WriteInt32(1);


        //    int rrr = r.Next(4);

        //    if (rrr == 0) //Links
        //    {
        //        outje.WriteInt32(0);
        //        outje.WriteInt32(r.Next(0, 1080));
        //        outje.WriteInt32(r.Next(100, 1000));
        //        outje.WriteInt32(r.Next(-1000, 1000));
        //    }
        //    else if (rrr == 1) //Rechts
        //    {
        //        outje.WriteInt32(clientjes.Count * 1920);
        //        outje.WriteInt32(r.Next(0, 1080));
        //        outje.WriteInt32(r.Next(-1000, -100));
        //        outje.WriteInt32(r.Next(-1000, 1000));
        //    }
        //    else if (rrr == 2) //Boven
        //    {
        //        outje.WriteInt32(r.Next(0, clientjes.Count * 1920));
        //        outje.WriteInt32(0);
        //        outje.WriteInt32(r.Next(-1000, 1000));
        //        outje.WriteInt32(r.Next(100, 1000));
        //    }
        //    else if (rrr == 3) //Onder
        //    {
        //        outje.WriteInt32(r.Next(0, clientjes.Count * 1920));
        //        outje.WriteInt32(1080);
        //        outje.WriteInt32(r.Next(-1000, 1000));
        //        outje.WriteInt32(r.Next(-1000, -100));
        //    }

        //    foreach (Clientje clientje in clientjes)
        //    {
        //        clientje.deveConnection.Send(outje);
        //    }


        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = screenClientjes.Count.ToString();
            label4.Text = controlClientjes.Count.ToString();
            label6.Text = salc.clients.Count.ToString();
            label8.Text = allClientjes.Count.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = Math.Max(int.Parse(textBox1.Text), 10);
            }
            catch (Exception eeee)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();

            Rectangle r = new Rectangle(0, 0, 100 * 10, 100 * 10);
            Rectangle r2 = new Rectangle(50 * 10, 50 * 10, 100 * 10, 100 * 10);

            drawFatRect(Pens.Red, g, r, 2, "r1");
            drawFatRect(Pens.Blue, g, r2, 2, "r2");

            Rectangle runion = Rectangle.Union(r, r2);

            drawFatRect(Pens.Green, g, runion, 2, "");


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //DeveOutgoingMessage outje = new DeveOutgoingMessage();
            //outje.WriteInt32(2);
            //foreach (var clientje in clientjes)
            //{
            //    clientje.deveConnection.Send(outje);
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendNewAreas();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //DeveOutgoingMessage outje = new DeveOutgoingMessage();

            //outje.WriteInt32(1);

            //outje.WriteInt32(0);
            //outje.WriteInt32(500);
            //outje.WriteInt32(1000);
            //outje.WriteInt32(0);

            //foreach (Clientje clientje in clientjes)
            //{
            //    clientje.deveConnection.Send(outje);
            //}
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
