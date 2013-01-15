using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirtyMapGenerator
{
    public partial class Form1 : Form
    {
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<List<String>> lijstje = new List<List<string>>();

            int width = 200;
            int height = 150;

            for (int y = 0; y < height; y++)
            {
                List<String> xlist = new List<string>();
                lijstje.Add(xlist);
                for (int x = 0; x < width; x++)
                {
                    //Border
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        xlist.Add("1");
                    }
                    else
                    {
                        xlist.Add("0");
                    }
                }
            }



            //Random platformpjes

            int amount = 10;

            for (int i = 0; i < amount; i++)
            {
                int xstart = random.Next(10, width - 10);
                int yheight = random.Next(3, height - 10);

                int length = random.Next(2, 10);

                for (int x = xstart; x < xstart + length; x++)
                {
                    lijstje[yheight][x] = "1";
                }
            }

            WriteToFile(lijstje);

        }

        public void WriteToFile(List<List<String>> map)
        {
            using (TextWriter writer = new StreamWriter("map.txt"))
            {
                foreach (var xlist in map)
                {
                    StringBuilder build = new StringBuilder();

                    foreach (String str in xlist)
                    {
                        build.Append(str);
                    }

                    writer.WriteLine(build.ToString());
                }
            }



        }
    }
}
