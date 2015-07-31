using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sobel_filtering
{
    public partial class Form_preview : Form
    {
        public Form_preview()
        {
            InitializeComponent();
            //pictureBox1.Controls.Add(hScrollBar1);
            //pictureBox1.Controls.Add(vScrollBar1);
            //VerticalScroll 
        }
        
        public Image img;

        public Image image
            {
                get
                {
                    return this.img;
                }
                set
                {
                    this.img = value;
                    
                }
            }
                    
           
        public void set_image ()
        {
            pictureBox1.Image = img;
        }
        private void button_close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Form_preview_Load(object sender, EventArgs e)
        {
            //pictureBox1.Image = image;
        }

        private void Form1_Resize(Object sender, EventArgs e)
        {
            // If the PictureBox has an image, see if it needs 
            // scrollbars and refresh the image. 
            if (pictureBox1.Image != null)
            {
                this.DisplayScrollBars();
                this.SetScrollBarValues();
                this.Refresh();
            }
        }

        public void DisplayScrollBars()
        {
            // If the image is wider than the PictureBox, show the HScrollBar.
            if (pictureBox1.Width > pictureBox1.Image.Width - this.vScrollBar1.Width)
            {
                hScrollBar1.Visible = false;
            }
            else
            {
                hScrollBar1.Visible = true;
            }

            // If the image is taller than the PictureBox, show the VScrollBar.
            if (pictureBox1.Height >
                pictureBox1.Image.Height - this.hScrollBar1.Height)
            {
                vScrollBar1.Visible = false;
            }
            else
            {
                vScrollBar1.Visible = true;
            }
        }

        private void HandleScroll(Object sender, ScrollEventArgs se)
        {
            /* Create a graphics object and draw a portion 
               of the image in the PictureBox. */
            Graphics g = pictureBox1.CreateGraphics();

            g.DrawImage(pictureBox1.Image,
              new Rectangle(0, 0, pictureBox1.Right - vScrollBar1.Width,
              pictureBox1.Bottom - hScrollBar1.Height),
              new Rectangle(hScrollBar1.Value, vScrollBar1.Value,
              pictureBox1.Right - vScrollBar1.Width,
              pictureBox1.Bottom - hScrollBar1.Height),
              GraphicsUnit.Pixel);

            pictureBox1.Update();
            //pictureBox1.Invalidate();
        }

        public void SetScrollBarValues()
        {
            // Set the Maximum, Minimum, LargeChange and SmallChange properties.
            this.vScrollBar1.Minimum = 0;
            this.hScrollBar1.Minimum = 0;

            // If the offset does not make the Maximum less than zero, set its value. 
            if ((this.pictureBox1.Image.Size.Width - pictureBox1.ClientSize.Width) > 0)
            {
                this.hScrollBar1.Maximum =
                    this.pictureBox1.Image.Size.Width - pictureBox1.ClientSize.Width;
            }
            // If the VScrollBar is visible, adjust the Maximum of the 
            // HSCrollBar to account for the width of the VScrollBar.  
            if (this.vScrollBar1.Visible)
            {
                this.hScrollBar1.Maximum += this.vScrollBar1.Width;
            }
            this.hScrollBar1.LargeChange = this.hScrollBar1.Maximum / 10;
            this.hScrollBar1.SmallChange = this.hScrollBar1.Maximum / 20;

            // Adjust the Maximum value to make the raw Maximum value 
            // attainable by user interaction.
            this.hScrollBar1.Maximum += this.hScrollBar1.LargeChange;

            // If the offset does not make the Maximum less than zero, set its value.    
            if ((this.pictureBox1.Image.Size.Height - pictureBox1.ClientSize.Height) > 0)
            {
                this.vScrollBar1.Maximum =
                    this.pictureBox1.Image.Size.Height - pictureBox1.ClientSize.Height;
            }

            // If the HScrollBar is visible, adjust the Maximum of the 
            // VSCrollBar to account for the width of the HScrollBar.
            if (this.hScrollBar1.Visible)
            {
                this.vScrollBar1.Maximum += this.hScrollBar1.Height;
            }
            this.vScrollBar1.LargeChange = this.vScrollBar1.Maximum / 10;
            this.vScrollBar1.SmallChange = this.vScrollBar1.Maximum / 20;

            // Adjust the Maximum value to make the raw Maximum value 
            // attainable by user interaction.
            this.vScrollBar1.Maximum += this.vScrollBar1.LargeChange;
        }

    }
}