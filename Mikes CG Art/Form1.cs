using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MikesLibrary;
using System.Threading;


namespace Mikes_CG_Art
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myFirstEZPictureBox = new EZPictureBox(this, 5, 5, 1000, 600, 1000*3, 600*3, Color.Gray);
            myFirstEZPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        EZPictureBox myFirstEZPictureBox;

        private void button1_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                EZBoid boid = new EZBoid(myFirstEZPictureBox, r.Next(100), r.Next(100), 10, 10, 10, 10, Color.Black);
                boid.metadata.Add(r.Next(1, 10));
                boid.metadata.Add(r.Next(1, 10));
                boid.onTimerTick += new EZBoid.onTimerTickEvent((EZBoid b) => 
                {
                    int velX = (int)b.metadata[0];
                    int velY = (int)b.metadata[1];
                    int newX = b.Location.X + velX;
                    int newY = b.Location.Y + velY;

                    if (newX < b.Parent.Left)
                    {
                        newX = 2 * b.Parent.Left - newX;
                        velX *= -1;
                    }
                    if (newX + b.Width >= b.Parent.Width + b.Parent.Left)
                    {
                        newX += b.Parent.Left + b.Parent.Width - newX - b.Width;
                        velX *= -1;
                    }

                    if (newY < b.Parent.Top)
                    {
                        newY = 2 * b.Parent.Top - newY;
                        velY *= -1;
                    }
                    if (newY + b.Height >= b.Parent.Height + b.Parent.Top)
                    {
                        newY += b.Parent.Top + b.Parent.Height - newY - b.Height;
                        velY *= -1;
                    }

                    b.metadata[0] = velX;
                    b.metadata[1] = velY;

                    EZPictureBox tempBox = new EZPictureBox(b.Parent, newX, newY, 5, 5, 1, 1, Color.Yellow);
                    ((EZPictureBox)b.Parent).engraveEZPictureBox(tempBox);
                    ((EZPictureBox)b.Parent).Refresh();
                    //((EZPictureBox)b.Parent).Controls.Add(new EZPictureBox(b.Parent, newX, newY, 3, 3, 3, 3, Color.Yellow));
                    tempBox.Dispose();

                    b.MoveTo(newX, newY);
                    b.Refresh();
                });
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        Random r = new Random();

        private void button2_Click(object sender, EventArgs e)
        {
            
            Graphics graphics = myFirstEZPictureBox.getGraphics();
            EZBoid boid = new EZBoid(myFirstEZPictureBox, 50, 50, 5, 5);
            boid.metadata.Add(80.0f);//brush width
            boid.metadata.Add(1.8f * 0.0174532925f);//clockwise rotation (in radians)
            boid.metadata.Add(0.0f);//direction
            boid.metadata.Add(0.0f);//x
            boid.metadata.Add(50.0f);//y
            boid.metadata.Add(0.005f);//size decrease per tick %
            boid.metadata.Add(1.5f);  //the (size decrease per tick %) multiplier for off branches (each branch gets smaller faster)
            int activeBoidsCount = 1;
            boid.onTimerTick += new EZBoid.onTimerTickEvent((EZBoid b) =>
                {
                    if (r.Next(15) == 10 && activeBoidsCount < 800)
                    {
                        EZBoid deamonSpawn = new EZBoid(myFirstEZPictureBox, 50, 50, 5, 5);
                        foreach (Object o in b.metadata)
                            deamonSpawn.metadata.Add(o); //beware of reference passing, instead of value cloning
                        deamonSpawn.metadata[5] = (float)b.metadata[5] * (float)b.metadata[6];
                        b.metadata[6] = (float)b.metadata[6] * 1.05f;
                        float deamonRotation = (float)deamonSpawn.metadata[2];
                        deamonRotation -= 90;
                        deamonSpawn.metadata[2] = deamonRotation;
                        deamonSpawn.onTimerTick += new EZBoid.onTimerTickEvent((EZBoid b2) =>
                            {
                                b2.onTimerTick = b.onTimerTick;//sneaky sneaky ;)
                            });
                        activeBoidsCount++;
                    }

                    double newX = (float)b.metadata[3] + Math.Cos((float)b.metadata[2]) * ((float)b.metadata[0] / 2);
                    double newY = (float)b.metadata[4] + Math.Sin((float)b.metadata[2]) * ((float)b.metadata[0] / 2);
                    b.metadata[2] = (float)b.metadata[1] + (float)b.metadata[2];

                    //Pen pen = new Pen(Color.Azure);
                    Pen pen = new Pen(Color.FromArgb((int)(255.0f * ((float)boid.metadata[0] / 80.0f)), (int)(105.0f * ((float)boid.metadata[0] / 80.0f)) + 20, (int)(155.0f * ((float)boid.metadata[0] / 80.0f)) + 30));
                    pen.Width = (float)b.metadata[0] / 2f;
                    graphics.DrawLine(pen, (float)b.metadata[3], (float)b.metadata[4], (float)newX, (float)newY);

                    b.MoveTo((int) newX, (int) newY);
                    b.metadata[3] = (float)newX;
                    b.metadata[4] = (float)newY;
                    b.metadata[0] = (float)b.metadata[0] * (1f - (float)b.metadata[5]);

                    if ((float)b.metadata[0] < 1)
                    {
                        activeBoidsCount--;
                        b.Dispose();
                    }

                    this.Text = activeBoidsCount + "";
                });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            myFirstEZPictureBox.ExportImage(@"Z:\Users\s2122_000\Desktop\");
        }
    }
}
