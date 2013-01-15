using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DeveConnecteuze.Network;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;

namespace TurtleTurner2000AndroidServer
{
	[Activity (Label = "TurtleTurner2000AndroidServer", MainLauncher = true)]
	public class Activity1 : Activity
	{
		int count = 1;
		int count2 = 1;

		private List<Clientje> clientjes = new List<Clientje>();
		private DeveServer deveServer;

		private Random r = new Random();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it


			deveServer = new DeveServer(1337);
			deveServer.Start();

			Thread tr = new Thread(Runner);
			tr.IsBackground = true;
			tr.Start();

			Button button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
				new Thread(()=>{
					SendRandomSquirtle();
				}){IsBackground = true}.Start();

			};

			Button button2 = FindViewById<Button> (Resource.Id.myButton2);
			button2.Click += delegate {
				button2.Text = string.Format ("{0} clicks!", count2++);
				new Thread(()=>{
					for (int i = 0; i < 1000; i++)
					{
						SendRandomSquirtle();
						
					}
				}){IsBackground = true}.Start();
			
			
			};

			Button button3 = FindViewById<Button> (Resource.Id.myButton3);
			button3.Click += delegate {
				//button.Text = string.Format ("{0} clicks!", count2++);
				new Thread(()=>{
					for (int i = 0; i < 100; i++)
					{
						SendRandomSquirtle();
						Thread.Sleep(25);
					}
				}){IsBackground = true}.Start();
			
				
			};

			Button button4 = FindViewById<Button> (Resource.Id.myButton4);
			button4.Click += delegate {
				//button.Text = string.Format ("{0} clicks!", count2++);
				new Thread(()=>{
					for (int i = 0; i < 100; i++)
					{
						button5_Click(null,null);
						Thread.Sleep(25);
					}
				}){IsBackground = true}.Start();
		
				
			};

			Button button5 = FindViewById<Button> (Resource.Id.myButton5);
			button5.Click += delegate {
				//button.Text = string.Format ("{0} clicks!", count2++);
				new Thread(()=>{
					for (int i = 0; i < 100; i++)
					{
						button3_Click(null,null);
						Thread.Sleep(25);
					}
				}){IsBackground = true}.Start();
			
				
			};

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
							//Clientje toremove = clientjes.Where((x) => x.deveConnection == inc.Sender).FirstOrDefault();
							int startje = 0;
							while (startje < clientjes.Count)
							{
								Clientje curur = clientjes[startje];
								if (curur.deveConnection == inc.Sender)
								{
									clientjes.RemoveAt(startje);
								}
								else
								{
									startje++;
								}

							}

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
				
				//drawFatRect(Pens.Red, g, curSize, 5, i.ToString());
				
				
				curClient.deveConnection.Send(outje);
			}
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
		

		private void button3_Click(object sender, EventArgs e)
		{
			DeveOutgoingMessage outje = new DeveOutgoingMessage();
			outje.WriteInt32(2);
			foreach (var clientje in clientjes)
			{
				clientje.deveConnection.Send(outje);
			}
		}
		
		private void button4_Click(object sender, EventArgs e)
		{
			SendNewAreas();
		}
		
		private void button5_Click(object sender, EventArgs e)
		{
			DeveOutgoingMessage outje = new DeveOutgoingMessage();
			
			outje.WriteInt32(1);
			
			outje.WriteInt32(0);
			outje.WriteInt32(500);
			outje.WriteInt32(1000);
			outje.WriteInt32(0);
			
			foreach (Clientje clientje in clientjes)
			{
				clientje.deveConnection.Send(outje);
			}
		}
	}
}


