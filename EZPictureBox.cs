using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MikesLibrary
{
    public class EZPictureBox : PictureBox
    {
        private static int EZPictureBox_Count = 0;
        private Graphics graphics;
        
        public EZPictureBox()
            :base()
        {
            
        }

        /// <summary>
        /// Creates the EZPictureBox and adds it to the specified parent form/control.
        /// </summary>
        /// <param name="parent">The form/control the EZPictureBox is being added to. It will automatically be refreshed once added.</param>
        /// <param name="refreshParent">Should the parent be automatically refreshed after adding the EZPictureBox. Default = true.</param>
        public EZPictureBox(Control parent, int x, int y, int width, int height, bool refreshParent = true)
            :base()
        {
            this.Location = new System.Drawing.Point(x, y);
            this.Name = "EZPictureBox" + ++EZPictureBox_Count;
            this.Size = new System.Drawing.Size(width, height);

            this.graphics = this.CreateGraphics();

            this.Parent = parent;
            parent.Controls.Add(this);
            if (refreshParent)
                parent.Refresh();
        }

        public EZPictureBox(Control parent, int x, int y, int width, int height, int imageWidth, int imageHeight, Color imageColor)
            :this(parent,x,y,width,height,false)
        {
            this.Image = new Bitmap(imageWidth, imageHeight);
            Pen pen = new Pen(imageColor);
            this.getGraphics().FillRectangle(pen.Brush, 0, 0, imageWidth, imageHeight);
        }

        public void setPixel(int x, int y, Color color)
        {
            ((Bitmap)Image).SetPixel(x, y, color);
        }

        public Color getPixel(int x, int y)
        {
            return ((Bitmap)Image).GetPixel(x, y);
        }

        public Graphics getGraphics()
        {
            return Graphics.FromImage(this.Image);
        }

        /// <summary>
        /// Takes a EZPictureBox and draws it (pixel by pixel) onto the canvas of this EZPictureBox. Uses same positioning system/relativeness for the engravement as this EZPictureBox.
        /// </summary>
        public void engraveEZPictureBox(EZPictureBox engravement)
        {
            String info = String.Format("Engravement({0}x,{1}y)({2}w,{3}h) | this({4}x,{5}y)({6}w,{7}h)", engravement.Location.X, engravement.Location.Y, engravement.Width, engravement.Height, Location.X, Location.Y, Width, Height);
            if (engravement.Location.X < this.Location.X)
                throw new Exception("Out of bounds. Engravement.X < this.X : " + info);
            if (engravement.Location.X + engravement.Width >= this.Location.X + this.Width)
                throw new Exception("Out of bounds. Engravement right side exceeds this.right side : " + info);
            if (engravement.Location.Y < this.Location.Y)
                throw new Exception("Out of bounds. Engravement.Y < this.Y : " + info);
            if (engravement.Location.Y + engravement.Height >= this.Location.Y + this.Height)
                throw new Exception("Out of bounds. Engravement bottom side exceeds this.bottom side : " + info);
            //NOTE TO SELF: UPDATE THIS CODE TO BETTER TAKE INTO ACOUNT PICTUREBOXES OF SAME SIZE BUT DIFFERENT IMAGE SIZES
            for (int x = 0; x < engravement.Width; x++)
                for (int y = 0; y < engravement.Height; y++)
                    setPixel(x + engravement.Left, y + engravement.Top, engravement.getPixel((int)(x / (engravement.Width / (engravement.Image.Width * 1.0))), (int)(y / (engravement.Height / (engravement.Image.Height * 1.0)))));
        }

        public void Refresh()
        {
            this.Parent.Refresh();
        }

        public void Move(int offsetX, int offsetY)
        {
            this.Location = new Point(this.Location.X + offsetX, this.Location.Y + offsetY);
        }

        public void MoveTo(int newX, int newY)
        {
            this.Location = new Point(newX, newY);
        }

        public void ExportImage(String location, String name = "autoGenerate")
        {
            if (location[location.Length - 1] != '\\' && location != "")
                location = location + '\\';
            if (name == "autoGenerate")
            {
                int curID = 1;
                while (File.Exists(location + "img " + curID + ".jpg"))
                    curID++;
                name = "img " + curID;
            }
            if (!name.ToLower().Contains(".jpg"))
                name = name + ".jpg";
            Image.Save(@location + name);
        }
    }
}
