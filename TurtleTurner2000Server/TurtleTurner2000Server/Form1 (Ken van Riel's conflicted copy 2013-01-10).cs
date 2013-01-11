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

namespace TurtleTurner2000Server
{
    public partial class Form1 : Form
    {
        private DeveServer deveServer;
        private Random r = new Random();

        private List<Clientje> clientjes = new List<Clientje>();

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = timer1.Interval.ToString();
            deveServer = new DeveServer(1337);
            deveServer.Start();

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
                            break;
                        case DeveMessageType.StatusChanged:
                            byte newStatus = inc.ReadByte();
                            NetworkStatus ns = (NetworkStatus)newStatus;
                            switch (ns)
                            {
                                case NetworkStatus.Connected:
                                    clientjes.Add(new Clientje(inc.Sender));
                                    SendNewAreas();
                                    break;
                                case NetworkStatus.Disconnected:
                                    Clientje toremove = clientjes.Where((x) => x.deveConnection == inc.Sender).FirstOrDefault();
                                    clientjes.Remove(toremove);
                                    SendNewAreas();
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

        public void SendNewAreas()
        {
            Graphics g = panel1.CreateGraphics();
            g.FillRectangle(Brushes.CornflowerBlue, new Rectangle(0, 0, 1000000, 1000000));

            Rectangle totSize = new Rectangle();
            totSize.X = 0;
            totSize.Y = 0;
            totSize.Width = clientjes.Count * 1920;
            totSize.Height = clientjes.Count * 1080;

            for (int i = 0; i < clientjes.Count; i++)
            {
                Clientje curClient = clientjes[i];

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
            SendRandomSquirtle();
        }

        public void SendRandomSquirtle()
        {
            DeveOutgoingMessage outje = new DeveOutgoingMessage();

            outje.WriteInt32(1);


            int rrr = r.Next(4);

            if (rrr == 0) //Links
            {
                outje.WriteInt32(0);
                outje.WriteInt32(r.Next(0, 1080));
                outje.WriteInt32(r.Next(100, 1000));
                outje.WriteInt32(r.Next(-1000, 1000));
            }
            else if (rrr == 1) //Rechts
            {
                outje.WriteInt32(clientjes.Count * 1920);
                outje.WriteInt32(r.Next(0, 1080));
                outje.WriteInt32(r.Next(-1000, -100));
                outje.WriteInt32(r.Next(-1000, 1000));
            }
            else if (rrr == 2) //Boven
            {
                outje.WriteInt32(r.Next(0, clientjes.Count * 1920));
                outje.WriteInt32(0);
                outje.WriteInt32(r.Next(-1000, 1000));
                outje.WriteInt32(r.Next(100, 1000));
            }
            else if (rrr == 3) //Onder
            {
                outje.WriteInt32(r.Next(0, clientjes.Count * 1920));
                outje.WriteInt32(1080);
                outje.WriteInt32(r.Next(-1000, 1000));
                outje.WriteInt32(r.Next(-1000, -100));
            }

            foreach (Clientje clientje in clientjes)
            {
                clientje.deveConnection.Send(outje);
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendRandomSquirtle();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = Math.Max(int.Parse(textBox1.Text), 50);
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
    }
}
