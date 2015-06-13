using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MikesLibrary
{
    public class EZBoid : EZPictureBox
    {
        private static List<EZBoid> allEZBoids = new List<EZBoid>();
        private static Timer allEZBoidsTicker = null;

        public List<Object> metadata = new List<Object>();

        public delegate void onTimerTickEvent(EZBoid boid);
        public onTimerTickEvent onTimerTick;
        private static List<EZBoid> boidsSchedualedToBeDisposed = new List<EZBoid>();

        private void SetupEZBoid()
        {
            allEZBoids.Add(this);
            if (allEZBoidsTicker == null)
            {
                allEZBoidsTicker = new Timer();
                allEZBoidsTicker.Enabled = true;
                allEZBoidsTicker.Interval = 10;
                allEZBoidsTicker.Tick += new EventHandler((object sender, EventArgs e) => 
                {
                    foreach (EZBoid deadBoid in boidsSchedualedToBeDisposed)
                    {
                        allEZBoids.Remove(deadBoid);
                        deadBoid.Dispose(false);
                    }

                    List<Control> parents = new List<Control>();

                    foreach (EZBoid boid in allEZBoids)
                        if (!parents.Contains(getMostAncesterousParent(boid)))
                            parents.Add(getMostAncesterousParent(boid));

                    foreach (Control c in parents)
                        EZDrawingControl.SuspendDrawing(c);

                    for (int i = 0; i < allEZBoids.Count; i++) //cant use a foreach here, it disallows the creation of new boids in the onTimerClick method. 
                        allEZBoids[i].onTimerTick(allEZBoids[i]);

                    foreach (Control c in parents)
                        EZDrawingControl.ResumeDrawing(c);
                });
            }
        }

        public void Dispose()
        {
            boidsSchedualedToBeDisposed.Add(this);
        }

        private Control getMostAncesterousParent(Control c)
        {
            while (c.Parent != null)
                c = c.Parent;
            return c;
        }
       
        public EZBoid()
            :base()
        {
            SetupEZBoid();
        }

        public EZBoid(Control parent, int x, int y, int width, int height, bool refreshParent = true)
            :base(parent,x,y,width,height,refreshParent)
        {
            SetupEZBoid();
        }

        public EZBoid(Control parent, int x, int y, int width, int height, int imageWidth, int imageHeight, Color imageColor)
            :base(parent,x,y,width,height,imageWidth,imageHeight,imageColor)
        {
            SetupEZBoid();
        }

        public void setTickerInterval(int interval)
        {
            allEZBoidsTicker.Interval = interval;
        }

    }
}
