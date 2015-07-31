using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Inversion;
using System.IO;
using System.Data.OleDb;
using System.Drawing.Imaging;
using System.Threading;

using CSharpFilters;

namespace sobel_filtering
{
    public partial class Form1 : Form
    {
        string _dataBasePath = "";
        string save_file, extension;
        string file_name;
        private LogicalOperator processing;
        Bitmap b;
        string crop_image;
        int bLength;
        int xp, xq, yp, yq, spq;
        int lip_wx, lip_wy;
        Bitmap Bit;
        Bitmap undo_picture;//for undoing image
        bool delay_process = true;


        public Form1()
        {
            InitializeComponent();
            processing = new LogicalOperator();
        }

        public Bitmap black_and_white(Image Im)
        {
            Bitmap b = (Bitmap)Im;
            int A, B, C, c;
            int limit = 110;

            for (int i = 1; i < b.Height; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width; j++)  // loop for the image pixels width
                {
                    Color col;
                    col = b.GetPixel(j, i);

                    A = Convert.ToInt32(col.R);
                    B = Convert.ToInt32(col.G);
                    C = Convert.ToInt32(col.B);
                    if (A > limit || B > limit || C > limit)
                        c = 255;
                    else
                        c = 0;

                    if (c == 0)
                        b.SetPixel(j, i, Color.Black);
                    else
                        b.SetPixel(j, i, Color.White);
                }
            }
            return b;
        }

        public Bitmap white(Image Im)
        {
            Bitmap b = (Bitmap)Im;
            int A, B, C, c;
            int limit = 110;

            for (int i = 1; i < b.Height; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width; j++)  // loop for the image pixels width
                {
                    Color col;
                    col = b.GetPixel(j, i);


                    c = Convert.ToInt32(col.R) + Convert.ToInt32(col.G) + Convert.ToInt32(col.B);
                    c /= 3;

                    if (c > 135)
                        c = 0;
                    else
                        c = 255;

                    if (c == 0)
                        b.SetPixel(j, i, Color.Black);
                    else
                        b.SetPixel(j, i, Color.White);

                }
            }
            return b;
        }

        public Image black_white(Image Im)
        {
            Bitmap b = (Bitmap)Im;
            for (int i = 1; i < b.Height; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width; j++)  // loop for the image pixels width
                {
                    Color col;
                    col = b.GetPixel(j, i);

                    int c;
                    c = Convert.ToInt32(col.R) + Convert.ToInt32(col.G) + Convert.ToInt32(col.B);
                    c /= 3;
                    if (c > 128)
                        c = 255;
                    else
                        c = 0;

                    if (c == 0)
                        b.SetPixel(j, i, Color.Black);
                    else
                        b.SetPixel(j, i, Color.GhostWhite);

                }
            }
            return (Image)b;
        }

        public Image gray(Image Im)
        {
            Bitmap b = (Bitmap)Im.Clone();
            for (int i = 0; i < b.Height; i++)   // loop for the image pixels height
            {
                for (int j = 0; j < b.Width; j++)  // loop for the image pixels width
                {
                    Color col;
                    col = b.GetPixel(j, i);
                    b.SetPixel(j, i, Color.FromArgb((col.R + col.G + col.B) / 3, (col.R + col.G + col.B) / 3, (col.R + col.G + col.B) / 3));

                }
            }
            return (Image)b;
        }



        public Image sobel(Image im)
        {
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };   //  The matrix Gx
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };  //  The matrix Gy
            b = (Bitmap)im;
            Bitmap b1 = new Bitmap(im);
            for (int i = 1; i < b.Height - 1; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width - 1; j++) // loop for image pixels width    
                {
                    float new_x = 0, new_y = 0;
                    float c;
                    for (int hw = -1; hw < 2; hw++)  //loop for cov matrix
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            c = (b.GetPixel(j + wi, i + hw).B + b.GetPixel(j + wi, i + hw).R + b.GetPixel(j + wi, i + hw).G) / 3;
                            new_x += gx[hw + 1, wi + 1] * c;
                            new_y += gy[hw + 1, wi + 1] * c;
                        }
                    }


                    if (new_x * new_x + new_y * new_y > 128 * 128)
                        b1.SetPixel(j, i, Color.Black);
                    else
                        b1.SetPixel(j, i, Color.White);
                }
            }
            return (Image)b1;
        }

        public Image prewitt(Image im)
        {

            int[,] gx = new int[,] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };   //  The matrix Gx
            int[,] gy = new int[,] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };  //  The matrix Gy
            b = (Bitmap)im;
            Bitmap b1 = new Bitmap(im);

            for (int i = 1; i < b.Height - 1; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width - 1; j++) // loop for image pixels width    
                {
                    float new_x = 0, new_y = 0;
                    float c;
                    for (int hw = -1; hw < 2; hw++)  //loop for cov matrix
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            c = (b.GetPixel(j + wi, i + hw).B + b.GetPixel(j + wi, i + hw).R + b.GetPixel(j + wi, i + hw).G) / 3;

                            new_x += gx[hw + 1, wi + 1] * c;
                            new_y += gy[hw + 1, wi + 1] * c;
                        }
                    }
                    if (new_x * new_x + new_y * new_y > 128 * 100)
                        b1.SetPixel(j, i, Color.Black);
                    else
                        b1.SetPixel(j, i, Color.White);
                }
            }
            return (Image)b1;
        }

        public Image laplace(Image im)
        {
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };   //  The matrix Gx
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };  //  The matrix Gy

           
            b = (Bitmap)im;
            Bitmap b1 = new Bitmap(im);
            for (int i = 1; i < b.Height - 1; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width - 1; j++) // loop for image pixels width    
                {
                    float new_x = 0, new_y = 0;
                    float c;
                    for (int hw = -1; hw < 2; hw++)  //loop for cov matrix
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            c = (b.GetPixel(j + wi, i + hw).B + b.GetPixel(j + wi, i + hw).R + b.GetPixel(j + wi, i + hw).G) / 3;

                            new_x += gx[hw + 1, wi + 1] * c;
                            new_y += gy[hw + 1, wi + 1] * c;
                        }
                    }
                    if (new_x * new_x + new_y * new_y > 128 * 45)
                        b1.SetPixel(j, i, Color.Black);
                    else
                        b1.SetPixel(j, i, Color.White);
                }
            }
            return (Image)b1;
        }

        public Image mirror_image(Image im)
        {
            Bitmap b = (Bitmap)im.Clone();

            Color c1, c2;

            for (int i = 0; i < b.Height - 1; i++)
            {
                for (int j = 0; j < b.Width / 2 - 1; j++)
                {
                    c1 = b.GetPixel(j, i);
                    c2 = b.GetPixel(b.Width - j - 1, i);

                    b.SetPixel(j, i, c2);
                    b.SetPixel(b.Width - j - 1, i, c1);

                }
            }

            return (Image)b;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear_all();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                

                pictureBox_Main.Image = Image.FromFile(file_name);
                pictureBox1.Image = Image.FromFile(file_name);
                contrast_function();
                label1.Text = "Original Image";
                performAllSteps();   
            }

        }


        private void contrast_function()
        {
            double nContrast = 30;
            double pixel = 0, contrast = (100.0 + nContrast) / 100.0;

            contrast *= contrast;

            int red, green, blue;
            Bitmap b = new Bitmap(pictureBox1.Image);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        pixel = red / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[2] = (byte)pixel;

                        pixel = green / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[1] = (byte)pixel;

                        pixel = blue / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[0] = (byte)pixel;

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
            pictureBox1.Image = (Image)b;

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            file_name = openFileDialog1.FileName;
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = gray((Image)pictureBox1.Image.Clone());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = black_white((Image)pictureBox1.Image.Clone());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = mirror_image(pictureBox1.Image);
 }

        int cr_start = 140, cr_end = 170, cb_start = 105, cb_end = 150;
        private void YCbCr_Click(object sender, EventArgs e)
        {
            double c = 0, cb = 0, cr = 255;
            Bitmap bb = (Bitmap)pictureBox1.Image.Clone();
            Bitmap bb1 = new Bitmap(pictureBox1.Image.Size.Width, pictureBox1.Image.Size.Height);
            int min_x = bb.Width + 5;
            int max_x = 0;
            int max_y = 0;
            int min_y = bb.Height + 5;
            
            cr_start = 140;
            cr_end = 170;
            cb_start = 105;
            cb_end = 150;

            for (int i = 1; i < bb.Width - 1; i++)
                for (int j = 1; j < bb.Height - 1; j++)
                {
                    c = 0.257 * Convert.ToDouble(bb.GetPixel(i, j).R) + 0.504 * bb.GetPixel(i, j).G + 0.098 * bb.GetPixel(i, j).B + 16;
                    cb = 0.148 * Convert.ToDouble(bb.GetPixel(i, j).R) - 0.291 * Convert.ToDouble(bb.GetPixel(i, j).G) + 0.439 * Convert.ToDouble(bb.GetPixel(i, j).B) + 128;
                    cr = 0.439 * Convert.ToDouble(bb.GetPixel(i, j).R) - 0.368 * Convert.ToDouble(bb.GetPixel(i, j).G) - 0.071 * Convert.ToDouble(bb.GetPixel(i, j).B) + 128;


                    
                    if ((cr > cr_start && cr < cr_end) && (cb > cb_start && cb < cb_end))
                    {

                        bb1.SetPixel(i, j, Color.Black);
                    }
                    else bb1.SetPixel(i, j, Color.White);
                }


            pictureBox2.Image = (Bitmap)bb1;
            MessageBox.Show("Finished....");

        }

        bool mouse_down = false;
        bool flag = false;
        Point first, last;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_down = true;
            flag = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_down = false;
            flag = false;

            first.X *= (int)Math.Round((double)pictureBox1.Image.Width / (double)pictureBox1.Width);
            last.X *= (int)Math.Round((double)pictureBox1.Image.Width / (double)pictureBox1.Width);

            first.Y *= (int)Math.Round((double)pictureBox1.Image.Height / (double)pictureBox1.Height);
            last.Y *= (int)Math.Round((double)pictureBox1.Image.Height / (double)pictureBox1.Height);
            Graphics.FromImage(pictureBox1.Image).DrawRectangle(new Pen(Color.Red, 3), first.X, first.Y, (last.X - first.X), (last.Y - first.Y));
            pictureBox1.Invalidate();

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_down)
            {
                if (flag)
                {
                    first = new Point(e.X, e.Y);
                    flag = false;
                }
                else
                {
                    last = new Point(e.X, e.Y);
                }


            }


        }

        private void button_crop_Click(object sender, EventArgs e)
        {
            Bitmap myBitmap = new Bitmap((last.X - first.X), (last.Y - first.Y));
            Bitmap target = new Bitmap(pictureBox1.Image);
            int m = 0, n = 0;
            for (int i = first.X + 3; i < last.X - 2; i++)
            {
                for (int j = first.Y + 3; j < last.Y - 2; j++)
                {
                    myBitmap.SetPixel(m, n++, target.GetPixel(i, j));
                }
                n = 0;
                m++;
            }

            if (!Directory.Exists(@"c:\image_xx_pp_tt\crop\"))
                Directory.CreateDirectory(@"c:\image_xx_pp_tt\crop\");

            Random rand = new Random();
            crop_image = @"c:\image_xx_pp_tt\crop\crop_" + rand.Next().ToString() + ".jpg";
            myBitmap.Save(crop_image);
            pictureBox2.Image = Image.FromFile(crop_image);
        }

        private void button_contrast_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = (Bitmap)prewitt(pictureBox1.Image);

        }

        private void button_laplace_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = (Bitmap)laplace(pictureBox1.Image);
        }


        bool[][] visited;


        int[][] big;
        int[] count;
        //Bitmap Bit;




        Queue<int> queue_i;
        Queue<int> queue_j;
        int first_i;
        int first_j;

        private int find_first_neighbour(int x, int y)
        {
            int w = Bit.Width;
            int h = Bit.Height;

            if (x - 1 >= 0)
                if (!visited[x - 1][y] && Bit.GetPixel(x - 1, y).B == 0 && Bit.GetPixel(x - 1, y).R == 0 && Bit.GetPixel(x - 1, y).G == 0 && big[x - 1][y] == 0)
                {
                    first_i = x - 1;
                    first_j = y;
                    return 1;
                }
            if (y - 1 >= 0)
                if (!visited[x][y - 1] && Bit.GetPixel(x, y - 1).B == 0 && Bit.GetPixel(x, y - 1).R == 0 && Bit.GetPixel(x, y - 1).G == 0 && big[x][y - 1] == 0)
                {
                    first_i = x;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w)
                if (!visited[x + 1][y] && Bit.GetPixel(x + 1, y).B == 0 && Bit.GetPixel(x + 1, y).R == 0 && Bit.GetPixel(x + 1, y).G == 0 && big[x + 1][y] == 0)
                {
                    first_i = x + 1;
                    first_j = y;
                    return 1;
                }
            if (y + 1 < h)
                if (!visited[x][y + 1] && Bit.GetPixel(x, y + 1).B == 0 && Bit.GetPixel(x, y + 1).R == 0 && Bit.GetPixel(x, y + 1).G == 0 && big[x][y + 1] == 0)
                {
                    first_i = x;
                    first_j = y + 1;
                    return 1;
                }


            if (x - 1 >= 0 && y - 1 >= 0)
                if (!visited[x - 1][y - 1] && Bit.GetPixel(x - 1, y - 1).B == 0 && Bit.GetPixel(x - 1, y - 1).R == 0 && Bit.GetPixel(x - 1, y - 1).G == 0 && big[x - 1][y - 1] == 0)
                {
                    first_i = x - 1;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w && y - 1 >= 0)
                if (!visited[x + 1][y - 1] && Bit.GetPixel(x + 1, y - 1).B == 0 && Bit.GetPixel(x + 1, y - 1).R == 0 && Bit.GetPixel(x + 1, y - 1).G == 0 && big[x + 1][y - 1] == 0)
                {
                    first_i = x + 1;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w && y + 1 < h)
                if (!visited[x + 1][y + 1] && Bit.GetPixel(x + 1, y + 1).B == 0 && Bit.GetPixel(x + 1, y + 1).R == 0 && Bit.GetPixel(x + 1, y + 1).G == 0 && big[x + 1][y + 1] == 0)
                {
                    first_i = x + 1;
                    first_j = y + 1;
                    return 1;
                }
            if (x - 1 >= 0 && y + 1 < h)
                if (!visited[x - 1][y + 1] && Bit.GetPixel(x - 1, y + 1).B == 0 && Bit.GetPixel(x - 1, y + 1).R == 0 && Bit.GetPixel(x - 1, y + 1).G == 0 && big[x - 1][y + 1] == 0)
                {
                    first_i = x - 1;
                    first_j = y + 1;
                    return 1;
                }

            return -1;

        }
        int count_region;
        int[] countt;

        private void BFS(int i, int j)
        {

            visited[i][j] = true;
            queue_i.Enqueue(i);
            queue_j.Enqueue(j);

            int w;
            while (queue_i.Count != 0)
            {
                i = queue_i.Dequeue(); //deque
                j = queue_j.Dequeue(); // deque

                big[i][j] = count_region;//assaigning tag for connected region
                countt[count_region]++;
                w = find_first_neighbour(i, j); //find first neighbour...if no neighbour return -1

                while (w != -1) //visit all 8 neighbours
                {
                    if (!visited[first_i][first_j])//unvisited nodes
                    {
                        visited[first_i][first_j] = true;

                        queue_i.Enqueue(first_i);//enque
                        queue_j.Enqueue(first_j);//enque

                    }

                    w = find_first_neighbour(i, j);// again find first neighbour...if no neighbour return -1
                }

            }

        }
        private void button_connected_Click(object sender, EventArgs e)
        {
            connected_area();
        }

        private void connected_area()
        {


            Bit = (Bitmap)pictureBox2.Image.Clone();
            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0;
            count_region = 1;
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))
                    {
                        countt[count_region] = 0;
                       
                        BFS(i, j);
                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                           
                        }
                        count_region++;

                    }
                }
            }


            Bitmap bmp = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);

            int min_x = Bit.Width;
            int max_x = 0;
            int max_y = 0;
            int min_y = Bit.Height;

            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)
                    {
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));

                        #region calculating max min x and y of shorted image frame
                        if (min_x >= i)
                            min_x = i;
                        if (max_x < i)
                            max_x = i;
                        if (min_y >= j)
                            min_y = j;
                        if (max_y < j)
                            max_y = j;
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, Color.White);

                }
            }

            


            int w, h, t;
            double a, p;
            int flagforidentification = 0;

            if (max_x - min_x >= 30 && max_y - min_x >= 30)
            {
                min_x = min_x - 30;
                min_y = min_y - 30;
                max_x = max_x + 30;
                max_y = max_y + 30;
                flagforidentification = 1;
            }

            if (min_x < 0)
                min_x = 0;

            if (min_y < 0)
                min_y = 0;

            if (max_x > Bit.Width)
                max_x = Bit.Width;

            if (max_y > Bit.Height)
                max_y = Bit.Height;
            a = max_x - min_x;
            p = a * 0.12;
            t = Convert.ToInt16(p);
            if (flagforidentification == 1)
            {
                max_x -= t;
                min_x += t;
                min_y += t;
            }
            a = max_x - min_x;
            w = Convert.ToInt16(a);
            h = Convert.ToInt16(w * 1.5);
            // MessageBox.Show(w.ToString());

            if (h + min_y > max_y)
                h = max_y - min_y;


            Bitmap bbbb = new Bitmap(w, h);
            Bitmap pic1 = (Bitmap)pictureBox1.Image.Clone();
            pictureBox2.Image = Image.FromFile(file_name);
            Bitmap fre = (Bitmap)pictureBox2.Image;
            for (int i = min_x; i < max_x; i++)
            {
                for (int j = min_y; j < min_y + h; j++)
                {
                    //if(bmp.GetPixel(i,j).B==0)
                    bbbb.SetPixel(i - min_x, j - min_y, fre.GetPixel(i, j));
                    ////// bbbb.SetPixel(i - min_x, j - min_y, pic1.GetPixel(i, j));
                    pic1.SetPixel(i, j, Color.Black);
                    //else
                    //bbbb.SetPixel(i - min_x, j - min_y, Color.White);
                }
            }

            //bbbb = shape((Bitmap)bbbb.Clone());

            pictureBox2.Image = (Bitmap)bbbb;
            pictureBox2.Invalidate();
            pictureBox1.Image = (Image)pic1;
            queue_i.Clear();
            queue_j.Clear();
        }

        public Bitmap shape(Bitmap bit)
        {
            /////Bit = new Bitmap(pictureBox2.Image);
            int i, j, w, h;
            w = bit.Width;
            h = bit.Height;
            int[] a = new int[w];

            for (j = 0; j < w; j++)
            {
                a[j] = 0;
                for (i = 0; i < h; i++)
                    if (bit.GetPixel(j, i).R == 0 && bit.GetPixel(j, i).G == 0 && bit.GetPixel(j, i).B == 0)
                        a[j]++;

            }

            int max = 0, max_i = 0;

            for (i = 0; i < w; i++)
                if (a[i] > max)
                {
                    max = a[i];
                    max_i = i;
                }
            max = 0;
            int t = -1;
            for (i = 0; i < max_i; i++)
            {
                if (max / 4 >= a[i])
                    t = i;
                if (a[i] > max)
                    max = a[i];
            }
            if (t != -1)
            {
                for (j = t; j >= 0; j--)
                    for (i = 0; i < h; i++)
                        bit.SetPixel(j, i, Color.White);

            }

            max = 0;
            t = -1;
            for (i = w - 1; i > max_i; i--)
            {
                if (max / 4 >= a[i])
                    t = i;
                if (a[i] > max)
                    max = a[i];
            }
            if (t != -1)
            {
                for (j = t; j < w; j++)
                    for (i = 0; i < h; i++)
                        bit.SetPixel(j, i, Color.White);

            }

            return bit;
            //////pictureBox2.Image=(Image)Bit;

        }

        public Bitmap connected_eye(Bitmap bit)
        {
            Bit = bit;
            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0;
            count_region = 1;
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))
                    {
                        countt[count_region] = 0;
                        //MessageBox.Show(i+" "+j+" ");
                        BFS(i, j);
                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                            //MessageBox.Show(max+" ");
                        }
                        count_region++;

                    }
                }
            }


            Bitmap bmp = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);

            int min_x = Bit.Width;
            int max_x = 0;
            int max_y = 0;
            int min_y = Bit.Height;

            //MessageBox.Show(max_bit.ToString());
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)
                    {
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));

                        #region calculating max min x and y of shorted image frame
                        if (min_x >= i)
                            min_x = i;
                        if (max_x < i)
                            max_x = i;
                        if (min_y >= j)
                            min_y = j;
                        if (max_y < j)
                            max_y = j;
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, Color.White);

                }
            }

            Bitmap bbbb = new Bitmap(max_x - min_x, max_y - min_y);
            Bitmap pic1 = (Bitmap)pictureBox1.Image.Clone();

            for (int i = min_x; i < max_x; i++)
            {
                for (int j = min_y; j < max_y; j++)
                {
                    //if(bmp.GetPixel(i,j).B==0)
                    bbbb.SetPixel(i - min_x, j - min_y, pic1.GetPixel(i, j));
                    //else
                    //bbbb.SetPixel(i - min_x, j - min_y, Color.White);

                }
            }

            //pictureBox2.Image = (Bitmap)bbbb;
            //pictureBox2.Invalidate();
            queue_i.Clear();
            queue_j.Clear();

            //MessageBox.Show("afdasdf");
            return bbbb;

        }

        public Bitmap connected_lips(Bitmap bit)
        {
            Bit = bit;
            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0;
            count_region = 1;
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))
                    {
                        countt[count_region] = 0;
                        //MessageBox.Show(i+" "+j+" ");
                        BFS(i, j);
                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                            //MessageBox.Show(max+" ");
                        }
                        count_region++;

                    }
                }
            }


            Bitmap bmp = new Bitmap(Bit.Width, Bit.Height);



            //MessageBox.Show(max_bit.ToString());
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));
                    else
                        bmp.SetPixel(i, j, Color.White);

                }
            }


            queue_i.Clear();
            queue_j.Clear();

            //MessageBox.Show("afdasdf");
            return bmp;

        }
        public Bitmap skew_filter(Bitmap bmp)
        {
            Bitmap bm = new Bitmap(bmp.Width, bmp.Height);

            Array red = Array.CreateInstance(typeof(Int32), 9);
            Array blue = Array.CreateInstance(typeof(Int32), 9);
            Array green = Array.CreateInstance(typeof(Int32), 9);

            int index = 0;
            int red_tot = 0;
            int blue_tot = 0;
            int green_tot = 0;
            int min_red, min_blue, min_green;

            bool flag_red, flag_blue, flag_green;

            for (int i = 1; i < bmp.Width - 1; i++)
            {
                for (int j = 1; j < bmp.Height - 1; j++)
                {
                    index = 0; red_tot = green_tot = blue_tot = 0;

                    for (int k = -1; k < 2; k++)
                    {
                        for (int m = -1; m < 2; m++)
                        {

                            red.SetValue(bmp.GetPixel(i + k, j + m).R, index);
                            blue.SetValue(bmp.GetPixel(i + k, j + m).B, index);
                            green.SetValue(bmp.GetPixel(i + k, j + m).G, index++);

                            red_tot += bmp.GetPixel(i + k, j + m).R;
                            green_tot += bmp.GetPixel(i + k, j + m).G;
                            blue_tot += bmp.GetPixel(i + k, j + m).B;
                        }
                    }

                    Array.Sort(red);
                    Array.Sort(blue);
                    Array.Sort(green);

                    red_tot /= 9;
                    green_tot /= 9;
                    blue_tot /= 9;

                    min_blue = min_green = min_red = 256;
                    flag_red = flag_blue = flag_green = true;

                    for (int k = 2; k < 7; k++)
                    {
                        if (flag_red)
                        {
                            if (Convert.ToInt16(red.GetValue(k)) == red_tot)
                            { min_red = red_tot; flag_red = false; }
                            else if (Convert.ToInt16(red.GetValue(k)) > red_tot)
                            {
                                flag_red = false;
                                min_red = Convert.ToInt16(red.GetValue(k - 1));// 
                                min_blue = Convert.ToInt16(blue.GetValue(k - 1));
                                min_green = Convert.ToInt16(green.GetValue(k - 1));
                                break;
                            }
                        }
                        if (flag_blue)
                        {
                            if (Convert.ToInt16(blue.GetValue(k)) == blue_tot)
                            {
                                min_blue = blue_tot;
                                flag_blue = false;


                            }
                            else if (Convert.ToInt16(red.GetValue(k)) > blue_tot)
                            {
                                min_red = Convert.ToInt16(red.GetValue(k - 1));
                                min_blue = Convert.ToInt16(blue.GetValue(k - 1));//
                                min_green = Convert.ToInt16(green.GetValue(k - 1));
                                flag_blue = false;
                            }
                            break;
                        }
                        if (flag_green)
                        {
                            if (Convert.ToInt16(green.GetValue(k)) == green_tot)
                            {
                                min_green = green_tot; flag_green = false;
                            }
                            else if (Convert.ToInt16(green.GetValue(k)) > green_tot)
                            {

                                min_red = Convert.ToInt16(red.GetValue(k - 1));
                                min_blue = Convert.ToInt16(blue.GetValue(k - 1));
                                min_green = Convert.ToInt16(green.GetValue(k - 1));//
                                flag_green = false;
                                break;
                            }

                        }
                    }

                    if (min_blue == 256)
                        min_blue = (int)blue.GetValue(4);
                    if (min_green == 256)
                        min_green = (int)green.GetValue(4);
                    if (min_red == 256)
                        min_red = (int)red.GetValue(4);

                    bm.SetPixel(i, j, Color.FromArgb(min_red, min_green, min_blue));

                }
            }

            bm.Save(@"c:\im.jpg");
            return bm;
        }
        private void button_skew_Click(object sender, EventArgs e)
        {
            //pictureBox2.Image = skew_filter((Bitmap)pictureBox1.Image);
            All_filters filters = new All_filters();
            pictureBox2.Image = filters.median_filter((Bitmap)pictureBox1.Image.Clone());
            pictureBox2.Invalidate();
        }



        private void button_erosion_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = dilation((Bitmap)pictureBox2.Image.Clone());
            pictureBox2.Invalidate();
        }

        private Bitmap dilation(Bitmap bmp)
        {
            processing.setImage(new Bitmap(pictureBox2.Image));

            byte[,] sele = new byte[3, 3];


            sele[0, 0] = (byte)1;
            sele[0, 1] = (byte)1;
            sele[0, 2] = (byte)1;
            sele[1, 0] = (byte)1;
            sele[1, 1] = (byte)1;
            sele[1, 2] = (byte)1;
            sele[2, 0] = (byte)1;
            sele[2, 1] = (byte)1;
            sele[2, 2] = (byte)1;

            processing.gray_erosion(sele);
            //pictureBox2.Image = new Bitmap((Bitmap)processing.getImage());

            //this.Invalidate();
            return (Bitmap)processing.getImage();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = (Bitmap)gray(pictureBox2.Image);
            pictureBox2.Invalidate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = erosion((Bitmap)pictureBox2.Image.Clone());
            pictureBox2.Invalidate();

        }

        private Bitmap erosion(Bitmap bm)
        {
            processing.setImage(bm);
            byte[,] sele = new byte[3, 3];

            sele[0, 0] = (byte)1;
            sele[0, 1] = (byte)1;
            sele[0, 2] = (byte)1;
            sele[1, 0] = (byte)1;
            sele[1, 1] = (byte)1;
            sele[1, 2] = (byte)1;
            sele[2, 0] = (byte)1;
            sele[2, 1] = (byte)1;
            sele[2, 2] = (byte)1;

            processing.gray_Dilation(sele);

            return (Bitmap)processing.getImage();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = (Bitmap)sobel(pictureBox2.Image);
            pictureBox2.Invalidate();
        }

       
        private void skin_color_segmentation()
        {
            Bitmap bm = (Bitmap)pictureBox1.Image;
            Bitmap bmp = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);

            double tot_pixel = bm.Height * bm.Width;
            tot_pixel /= 100;
            tot_pixel *= 10;
            int min_x = bm.Width + 5;// Convert.ToInt16(tot_pixel);
            int max_x = 0;
            int max_y = 0;
            int min_y = bm.Height + 5;// Convert.ToInt16(tot_pixel);

            Color color = new Color();
            double g, r, avg = 0;
            double f_upper, f_lower, w, l;
            bool R1, R2, R3, R4, s;
            double c, cr, cb;
            R1 = R3 = R4 = s = false;
            R2 = true;
            cr_start = 140;
            cr_end = 170;
            cb_start = 105;
            cb_end = 150;

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    r = Convert.ToDouble(color.R) / Convert.ToDouble(color.R + color.G + color.B);
                    g = Convert.ToDouble(color.G) / Convert.ToDouble(color.R + color.G + color.B);

                    f_upper = -1.3767 * r * r + 1.0743 * r + 0.1452;
                    f_lower = -0.776 * r * r + 0.5601 * r + 0.1766;

                    if (g > f_lower && g < f_upper)
                        R1 = true;
                    else
                        R1 = false;

                    w = Math.Pow((r - 0.33), 2) + Math.Pow((g - 0.33), 2);
                    //avg += w;
                    //MessageBox.Show(w.ToString());
                    if (w <= 0.0004)
                    {
                        R2 = true;
                        avg++;
                    }
                    else
                        R2 = false;

                    if (color.R > color.G && color.G > color.B)
                        R3 = true;
                    else
                        R3 = false;

                    if ((color.R - color.G) >= 45)
                        R4 = true;
                    else
                        R4 = false;

                    if (R3 && R4)//&& R1 && !R2)//if(R1 && R2)//
                        s = true;
                    else
                        s = false;

                    c = 0.257 * Convert.ToDouble(color.R) + 0.504 * color.G + 0.098 * color.B + 16;
                    cb = 0.148 * Convert.ToDouble(color.R) - 0.291 * Convert.ToDouble(color.G) + 0.439 * Convert.ToDouble(color.B) + 128;
                    cr = 0.439 * Convert.ToDouble(color.R) - 0.368 * Convert.ToDouble(color.G) - 0.071 * Convert.ToDouble(color.B) + 128;


                    if (s)// || (cr > cr_start && cr < cr_end) && (cb > cb_start && cb < cb_end))//nice result for good image                                     
                    {

                        bmp.SetPixel(i, j, Color.Black);
                        R1 = R3 = R4 = s = false;
                        R2 = true;
                        //bmp.SetPixel(i, j, color);
                        #region finding face rectangle
                        /*
                         * finding the minimum co-ordinate and maximum co-ordinate xy
                         * of the image between the Cb and Cr threshold value region
                         */

                        if (i < bm.Width / 2 && i < min_x)
                        {
                            min_x = i;
                        }
                        if ((i >= bm.Width / 2 && i < bm.Width) && i > max_x)
                        {
                            max_x = i;
                        }

                        if (j < bm.Height / 2 && j < min_y)
                        {
                            min_y = j;
                        }
                        if ((j >= bm.Height / 2 && i < bm.Height) && j > max_y)
                        {
                            max_y = j;
                        }
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, Color.White);
                }
            }

            pictureBox2.Image = (Bitmap)bmp;
            pictureBox2.Invalidate();
            //MessageBox.Show("End");

        }

        private void button_pc2_to_pc1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = (Bitmap)pictureBox2.Image.Clone();

        }

        private void button_default_Click(object sender, EventArgs e)
        {
            
        }

        private Bitmap find_edge(Image imm)
        {
            Bitmap b;
            b = (Bitmap)imm;
            int n, m;
            n = b.Height;
            m = b.Width;
            int cc = 0, total = 0;
            Bitmap B1 = new Bitmap(m, n);
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    B1.SetPixel(i, j, Color.White);
                }
            

            for (int i = 1; i < m - 1; i++)
                for (int j = 1; j < n - 1; j++)
                {


                    total = Math.Abs(b.GetPixel(i + 1, j + 1).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i + 1, j + 1).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i + 1, j + 1).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }

                    total = Math.Abs(b.GetPixel(i, j + 1).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i, j + 1).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i, j + 1).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }

                    total = Math.Abs(b.GetPixel(i + 1, j).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i + 1, j).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i + 1, j).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }
                    total = Math.Abs(b.GetPixel(i - 1, j - 1).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i - 1, j - 1).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i - 1, j - 1).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }
                    total = Math.Abs(b.GetPixel(i, j - 1).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i, j - 1).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i, j - 1).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }

                    total = Math.Abs(b.GetPixel(i - 1, j).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i - 1, j).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i - 1, j).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }

                    total = Math.Abs(b.GetPixel(i - 1, j + 1).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i - 1, j + 1).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i - 1, j + 1).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }

                    total = Math.Abs(b.GetPixel(i + 1, j - 1).B - b.GetPixel(i, j).B) + Math.Abs(b.GetPixel(i + 1, j - 1).R - b.GetPixel(i, j).R) + Math.Abs(b.GetPixel(i + 1, j - 1).G - b.GetPixel(i, j).G);
                    if (total > 90)
                    { B1.SetPixel(i, j, Color.Black); continue; }

                }

            return B1;
        }

        private void button_edge_Click(object sender, EventArgs e)
        {

            pictureBox2.Image = (Bitmap)black_and_white((Image)pictureBox1.Image.Clone());

        }

        public Image face(Image Im)
        {
            Bitmap b = (Bitmap)Im;
            int i, j, k, w, w_f = 0;
            w = b.Width / 2;
            for (i = 0; i < b.Height; i++)
            {
                if (b.GetPixel(w, i).R != 0 && b.GetPixel(w, i).R != 0 && b.GetPixel(w, i).R != 0 && w_f == 1)
                {
                    break;
                }
                if (w_f == 0)
                    if (b.GetPixel(w, i).R == 0 && b.GetPixel(w, i).R == 0 && b.GetPixel(w, i).R == 0)
                        w_f = 1;
            }
           
            int copal_start = i, x_st = 5, x_en = 5, tet;
            int preco = 0, nowco = 0;
            for (i = copal_start + 2; i < copal_start + (b.Height - copal_start) / 4; i++)
            {
                w_f = 0;
                nowco = 0;
                for (j = w; j < b.Width; j++)
                {
                    if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0))
                        break;
                    nowco++;
                }
                for (k = w - 1; k >= 0; k--)
                {
                    if ((b.GetPixel(k, i).R == 0 && b.GetPixel(k, i).R == 0 && b.GetPixel(k, i).R == 0))
                        break;
                    nowco++;
                }
                

                if (preco >= 15)
                    if (preco / 2 > nowco)
                    {
                        tet = 0;
                        for (j = x_st + 1; j < x_en; j++)
                        {
                            if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0))
                                continue;
                            tet++;
                        }

                        ///if ((preco*2) / 3 >= tet)
                        if ((preco * 4) / 5 >= tet)
                            break;
                    }
                if (preco < nowco)
                {
                    preco = nowco;
                    x_st = k;
                    x_en = j;
                    spq = i;
                }
            }
            //textBox1.Text += x_st;
            //textBox1.Text += " , " + x_en;
            MessageBox.Show(x_st + " " + x_en);
            
            int t;
            t = x_en - x_st;
            t = Convert.ToInt16(Convert.ToDouble(t) * 1.5);
            /*
             for (j = 0; j < b.Width; j++)
                     b.SetPixel(j,copal_start+t, Color.Red);
         */
            Bitmap B1 = new Bitmap(x_en - x_st + 1, t + 1);


            /*
                    for (i = x_st; i < x_en; i++)
                        for (j = copal_start; j < copal_start+t; j++)
                            B1.SetPixel(i - (x_st ), j - (copal_start), b.GetPixel(i, j));
            */

            xp = x_st;
            xq = x_en;
            yp = copal_start;
            yq = t + copal_start;
            spq = spq - copal_start;
            return (Image)B1;
        }

        public void conntected()
        {


            Bit = (Bitmap)pictureBox2.Image.Clone();
            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0, max_bit1 = 0, max_2 = 0;
            int point_y_max = 0, point_y = 0, one = 0;
            count_region = 1;
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))
                    {
                        countt[count_region] = 0;
                        //MessageBox.Show(i+" "+j+" ");
                        BFS(i, j);
                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;

                        }

                        count_region++;

                    }
                }
            }


            Bitmap bmp = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);

            int min_x = Bit.Width;
            int max_x = 0;
            int max_y = 0;
            int min_y = Bit.Height;


            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)// || big[i][j] == max_bit1)
                    {
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));

                        #region calculating max min x and y of shorted image frame
                        if (min_x >= i)
                            min_x = i;
                        if (max_x < i)
                            max_x = i;
                        if (min_y >= j)
                            min_y = j;
                        if (max_y < j)
                            max_y = j;
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, Color.White);

                }
            }


            int w = max_x - min_x;
            int h = max_y - min_y;
            int fac = h / 10;

            Bitmap pic1 = (Bitmap)pictureBox1.Image.Clone();

            if (h + fac > pic1.Height)
                fac = 0;

            Bitmap bbbb = new Bitmap(w, h + fac);

            for (int i = min_x; i < max_x; i++)
            {
                for (int j = min_y; j < max_y + fac; j++)
                {
                    bbbb.SetPixel(i - min_x, j - min_y, pic1.GetPixel(i, j));

                }
            }

            pictureBox2.Image = (Bitmap)bbbb;
            pictureBox2.Invalidate();
            queue_i.Clear();
            queue_j.Clear();
        }
        public void conntected1()
        {

            Bitmap original = (Bitmap)pictureBox1.Image.Clone();

            Bit = (Bitmap)pictureBox2.Image.Clone();
            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0, max_bit1 = 0, max_2 = 0;
            int point_y_max = 0, point_y = 0, one = 0, c;
            count_region = 1;
            Color col;

            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    col = original.GetPixel(i, j);
                    c = col.R + col.G + col.B;
                    c /= 3;

                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))// && c>135)
                    {
                        countt[count_region] = 0;
                        //MessageBox.Show(i+" "+j+" ");
                        BFS(i, j);
                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                            point_y = j;
                            if (point_y_max < point_y)
                            {
                                point_y_max = point_y;
                                one = 1;
                            }
                        }
                        else if (max_2 < countt[count_region])
                        {
                            max_2 = countt[count_region];
                            max_bit1 = count_region;
                            point_y = j;

                            if (point_y_max < point_y)
                            {
                                point_y_max = point_y;
                                one = 2;
                            }

                        }



                        count_region++;

                    }
                }
            }


            Bitmap bmp = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);

            int min_x = Bit.Width;
            int max_x = 0;
            int max_y = 0;
            int min_y = Bit.Height;

            //MessageBox.Show(one.ToString());
            if (one == 2)
                max_bit = max_bit1;

            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)
                    {
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));

                        #region calculating max min x and y of shorted image frame
                        if (min_x >= i)
                            min_x = i;
                        if (max_x < i)
                            max_x = i;
                        if (min_y >= j)
                            min_y = j;
                        if (max_y < j)
                            max_y = j;
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, Color.White);

                }
            }


          

            int fac = max_x - min_x;
            fac /= 10;

            if (max_x + fac < bmp.Width)
                max_x += fac;
            if (min_x - fac > 0)
                min_x -= fac;

            int w = max_x - min_x;
            int h = max_y - min_y;

            Bitmap bbbb = new Bitmap(w, h);
            Bitmap pic1 = (Bitmap)pictureBox1.Image.Clone();

            for (int i = min_x; i < max_x; i++)
            {
                for (int j = min_y; j < max_y; j++)
                {
                    bbbb.SetPixel(i - min_x, j - min_y, pic1.GetPixel(i, j));

                }
            }


            pictureBox2.Image = (Bitmap)bbbb;
            pictureBox2.Invalidate();
            queue_i.Clear();
            queue_j.Clear();
        }

        public Bitmap shape1(Bitmap Bit)
        {
            Bit = (Bitmap)Bit.Clone();
            int i, j, w, h;
            w = Bit.Width;
            h = Bit.Height;
            int[] a = new int[h];

            for (i = 0; i < h; i++)
            {
                a[i] = 0;
                for (j = 0; j < w; j++)
                    if (Bit.GetPixel(j, i).R == 0 && Bit.GetPixel(j, i).G == 0 && Bit.GetPixel(j, i).B == 0)
                        a[i]++;

            }

            int max = 0, max_i = 0;

            for (i = 0; i < h; i++)
                if (a[i] > max)
                {
                    max = a[i];
                    max_i = i;
                }
            max = 0;
            int t = -1;
            for (i = 0; i < max_i; i++)
            {
                if (max / 2 >= a[i])
                    t = i;
                if (a[i] > max)
                    max = a[i];
            }
            if (t != -1)
            {
                for (i = t; i >= 0; i--)
                    for (j = 0; j < w; j++)
                        Bit.SetPixel(j, i, Color.White);

            }

            max = 0;
            t = -1;
            for (i = h - 1; i > max_i; i--)
            {
                if (max / 2 >= a[i])
                    t = i;
                if (a[i] > max)
                    max = a[i];
            }
            if (t != -1)
            {
                for (i = t; i < h; i++)
                    for (j = 0; j < w; j++)
                        Bit.SetPixel(j, i, Color.White);

            }

            //////pictureBox2.Image=(Image)Bit;

            return Bit;

        }
        public void conntected2()
        {


            Bit = (Bitmap)pictureBox2.Image.Clone();
            Bitmap original = (Bitmap)pictureBox1.Image.Clone();

            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0, max_bit1 = 0, max_2 = 0, c;
            int point_y_max = 0, point_y = 0, one = 0;
            //int[] save_j = new int[capacity];
            //int [] save_i = new int[capacity];
            //int[] keep_index = new int[capacity];
            count_region = 1;
            Color col;

            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    col = original.GetPixel(i, j);
                    c = col.R + col.G + col.B;
                    c /= 3;

                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))//&& c>135)
                    {
                        countt[count_region] = 0;
                     
                        BFS(i, j);

                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                            point_y = j;
                            if (point_y_max < point_y)
                            {
                                point_y_max = point_y;
                                one = 1;
                            }
                        }
                        else if (max_2 < countt[count_region])
                        {
                            max_2 = countt[count_region];
                            max_bit1 = count_region;
                            point_y = j;

                            if (point_y_max < point_y)
                            {
                                point_y_max = point_y;
                                one = 2;
                            }
                            //MessageBox.Show("as"); 
                        }



                        count_region++;

                    }
                }
            }

           

            Bitmap bmp = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);

            int min_x = Bit.Width;
            int max_x = 0;
            int max_y = 0;
            int min_y = Bit.Height;

            if (one == 2)
                max_bit = max_bit1;

            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)
                    {
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));

                        #region calculating max min x and y of shorted image frame
                        if (min_x >= i)
                            min_x = i;
                        if (max_x < i)
                            max_x = i;
                        if (min_y >= j)
                            min_y = j;
                        if (max_y < j)
                            max_y = j;
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, Color.White);

                }
            }



            int fac = max_x - min_x;
            fac /= 10;

            if (max_x + fac < bmp.Width)
                max_x += fac;
            if (min_x - fac > 0)
                min_x -= fac;

            int w = max_x - min_x;
            int h = max_y - min_y;

            Bitmap bbbb = new Bitmap(w, h);
            Bitmap pic1 = (Bitmap)pictureBox1.Image.Clone();

            for (int i = min_x; i < max_x; i++)
            {
                for (int j = min_y; j < max_y; j++)
                {
                    bbbb.SetPixel(i - min_x, j - min_y, pic1.GetPixel(i, j));

                }
            }


            pictureBox2.Image = (Bitmap)bbbb;

            pictureBox2.Invalidate();
            queue_i.Clear();
            queue_j.Clear();
        }
        public void conntected_black()
        {


            Bit = (Bitmap)pictureBox2.Image.Clone();
            Bitmap original = (Bitmap)pictureBox1.Image.Clone();

            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0, max_bit1 = 0, max_2 = 0, c;
            int point_y_max = 0, point_y = 0, one = 0;

            count_region = 1;
            Color col;

            for (int j = 0; j < 10; j++)
            {
                for (int i = Bit.Width - 10; i < Bit.Width; i++)
                {
                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))//&& c>135)
                    {
                        countt[count_region] = 0;
                        BFS(i, j);

                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                        }


                        count_region++;

                    }
                }
            }





            if (max > 0)
                for (int i = 0; i < Bit.Width; i++)
                {
                    for (int j = 0; j < Bit.Height; j++)
                    {
                        if (big[i][j] == max_bit)
                        {
                            Bit.SetPixel(i, j, Color.White);
                            big[i][j] = 0;

                        }


                    }
                }


            max = 0;
            max_bit = 0;
            count_region = 1;
            queue_i.Clear();
            queue_j.Clear();
            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))//&& c>135)
                    {
                        countt[count_region] = 0;
                        BFS(i, j);

                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                        }


                        count_region++;

                    }
                }
            }




            if (max > 0)
                for (int i = 0; i < Bit.Width; i++)
                {
                    for (int j = 0; j < Bit.Height; j++)
                    {
                        if (big[i][j] == max_bit)
                        {
                            Bit.SetPixel(i, j, Color.White);
                            big[i][j] = 0;
                        }


                    }
                }

            /*********************************************/

            max = 0;
            max_bit = 0;
            count_region = 1;
            queue_i.Clear();
            queue_j.Clear();
            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion


            for (int i = 0; i < Bit.Width; i++)
            {

                if (!visited[i][Bit.Height - 1] && (Bit.GetPixel(i, Bit.Height - 1).R == 0 && Bit.GetPixel(i, Bit.Height - 1).G == 0 && Bit.GetPixel(i, Bit.Height - 1).B == 0))//&& c>135)
                {
                    countt[count_region] = 0;
                    BFS(i, Bit.Height - 1);

                    if (max < countt[count_region])
                    {
                        max = countt[count_region];
                        max_bit = count_region;
                    }


                    count_region++;

                }
            }




            if (max > 0)
                for (int i = 0; i < Bit.Width; i++)
                {
                    for (int j = 0; j < Bit.Height; j++)
                    {
                        if (big[i][j] == max_bit)
                        {
                            Bit.SetPixel(i, j, Color.White);
                            big[i][j] = 0;
                        }



                    }
                }

            max = 0;
            max_bit = 0;
            count_region = 1;
            queue_i.Clear();
            queue_j.Clear();
            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion


            for (int j = 0; j < Bit.Width; j++)
            {

                if (!visited[j][0] && (Bit.GetPixel(j, 0).R == 0 && Bit.GetPixel(j, 0).G == 0 && Bit.GetPixel(j, 0).B == 0))//&& c>135)
                {
                    countt[count_region] = 0;
                    BFS(j, 0);

                    if (max < countt[count_region])
                    {
                        max = countt[count_region];
                        max_bit = count_region;
                    }


                    count_region++;

                }
            }




            if (max > 0)
                for (int i = 0; i < Bit.Width; i++)
                {
                    for (int j = 0; j < Bit.Height; j++)
                    {
                        if (big[i][j] == max_bit)
                        {
                            Bit.SetPixel(i, j, Color.White);
                            big[i][j] = 0;
                        }


                    }
                }


            pictureBox2.Image = (Bitmap)Bit;

            pictureBox2.Invalidate();
            queue_i.Clear();
            queue_j.Clear();
        }

        Bitmap lips_bezier_candidate;
        public void conntected_lips()
        {


            Bit = (Bitmap)pictureBox2.Image.Clone();
            Bitmap original = (Bitmap)pictureBox1.Image.Clone();

            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0, max_bit1 = 0, max_2 = 0, c;
            int point_y_max = 0, point_y = 0, one = 0;
            count_region = 1;
            Color col;

            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    col = original.GetPixel(i, j);

                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))//&& c>135)
                    {
                        countt[count_region] = 0;
                        //MessageBox.Show(i+" "+j+" ");
                        BFS(i, j);
                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                            point_y = j;
                            if (point_y_max < point_y)
                            {
                                point_y_max = point_y;
                                one = 1;
                            }
                        }
                        else if (max_2 < countt[count_region])
                        {
                            max_2 = countt[count_region];
                            max_bit1 = count_region;
                            point_y = j;

                            if (point_y_max < point_y)
                            {
                                point_y_max = point_y;
                                one = 2;
                            }

                        }



                        count_region++;

                    }
                }
            }


            Bitmap bmp = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height);

            int min_x = Bit.Width;
            int max_x = 0;
            int max_y = 0;
            int min_y = Bit.Height;

            if (one == 2)
                max_bit = max_bit1;

            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)
                    {
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));

                        #region calculating max min x and y of shorted image frame
                        if (min_x >= i)
                            min_x = i;
                        if (max_x < i)
                            max_x = i;
                        if (min_y >= j)
                            min_y = j;
                        if (max_y < j)
                            max_y = j;
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, Color.White);

                }
            }



            int fac = max_x - min_x;
            fac /= 8;

            if (max_x + fac < bmp.Width)
                max_x += fac;
            if (min_x - fac > 0)
                min_x -= fac;

            int fac1 = max_y - min_y;
            fac1 /= 8;

            if (max_y + fac1 < bmp.Height)
                max_y += fac1;
            if (min_y - 2 * fac1 > 0)
                min_y -= 2 * fac1;

            int w = max_x - min_x;
            int h = max_y - min_y;

            Bitmap bbbb = new Bitmap(w, h);
            lips_bezier_candidate = new Bitmap(w, h);
            Bitmap pic1 = (Bitmap)pictureBox1.Image.Clone();

            for (int i = min_x; i < max_x; i++)
            {
                for (int j = min_y; j < max_y; j++)
                {
                    bbbb.SetPixel(i - min_x, j - min_y, pic1.GetPixel(i, j));
                    lips_bezier_candidate.SetPixel(i - min_x, j - min_y, Color.Black);
                }
            }


            pictureBox2.Image = (Bitmap)bbbb;

            pictureBox2.Invalidate();
            queue_i.Clear();
            queue_j.Clear();
        }

        private void button_eye_Click(object sender, EventArgs e)
        {

            //pictureBox1.Image = (Bitmap)pictureBox3.Image.Clone();
            //pictureBox1.Invalidate();
            anti_face_color();
            conntected_black();
            conntected();

        }

        private void button_histogram_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = (Bitmap)equilizeHist(pictureBox1.Image);
        }

        public float[] equilize(int[] histogram, long numPixel)
        {
            float[] hist = new float[256];

            hist[0] = histogram[0] * histogram.Length / numPixel;
            long prev = histogram[0];
            string str = "";
            str += (int)hist[0] + "\n";

            for (int i = 1; i < hist.Length; i++)
            {
                prev += histogram[i];
                hist[i] = prev * histogram.Length / numPixel;
                str += (int)hist[i] + "   _" + i + "\t";
            }

            //	MessageBox.Show( str );
            return hist;


        }

        public Image equilizeHist(Image im)
        {

            int[] histogram = new int[256];
            int color12, i, w, h, mean, j;

            //Bitmap b = new Bitmap(pictureBox1.Image);
            Bitmap b = (Bitmap)im.Clone();
            w = b.Width;
            h = b.Height;

            for (color12 = 0; color12 < 3; color12++)
            {
                for (i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;

                for (i = 0; i < w; i++)
                    for (j = 0; j < h; j++)
                    {
                        if (color12 == 0)
                            mean = b.GetPixel(i, j).R;
                        else if (color12 == 1)
                            mean = b.GetPixel(i, j).G;
                        else
                            mean = b.GetPixel(i, j).B;
                        histogram[mean]++;
                    }
                float[] LUT = equilize(histogram, b.Width * b.Height);
                int index1, index2, index3;
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        index1 = b.GetPixel(i, j).R;
                        index2 = b.GetPixel(i, j).G;
                        index3 = b.GetPixel(i, j).B;
                        if (color12 == 0)
                        {
                            byte nValue = (byte)LUT[index1];
                            if (LUT[index1] > 255)
                                nValue = 255;
                            index1 = nValue;
                        }
                        else if (color12 == 1)
                        {
                            byte nValue = (byte)LUT[index2];
                            if (LUT[index2] > 255)
                                nValue = 255;
                            index2 = nValue;
                        }
                        else
                        {
                            byte nValue = (byte)LUT[index3];
                            if (LUT[index3] > 255)
                                nValue = 255;
                            index3 = nValue;
                        }
                        b.SetPixel(i, j, Color.FromArgb(index1, index2, index3));
                    }

                }




            }


            return (Image)b;
        }



        private void lip_location(Bitmap b)
        {
            //////Bitmap b = new Bitmap(pictureBox2.Image);
            int w = b.Width;
            int h = b.Height;
            int ys1 = h, ye1 = h - 1, ys2 = h, ye2 = h - 1;
            int i, j, k;
            int mid = 0, max = 0;

            for (i = 0; i < h; i++)
            {
                b.SetPixel(0, i, Color.White);
                b.SetPixel(1, i, Color.White);
                b.SetPixel(w - 1, i, Color.White);
                b.SetPixel(w - 2, i, Color.White);
            }


            for (i = w / 4; i < w - (w / 4); i++)
            {
                for (j = spq; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                        break;

                if (max < (j - spq))
                {
                    max = j - spq;
                    mid = i;
                }
            }
            int tp1 = mid - 5, tp2 = mid + 5, mp1 = h - 1, mp2 = h - 1;
            for (i = w / 8; i < w - (w / 8); i++)
            {
                for (j = spq; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                        break;
                if (i <= mid)
                {
                    if (j - 1 < ys1)
                        ys1 = j - 1;
                    if (i >= mid / 2)
                        if (mp1 > j)
                        { tp1 = i; mp1 = j; }
                }
                else
                {
                    if (j - 1 < ys2)
                        ys2 = j - 1;
                    if (i <= mid + (w - mid) / 2)
                        if (mp2 > j)
                        { tp2 = i; mp2 = j; }
                }
            }

            int black = 0;

            for (i = mid / 2; i >= mid / 4; i--)
            {
                black = 0;
                for (j = ys1; j <= ys1 + (h - ys1) / 4; j++)
                {
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {

                        if (black != 0 && black != j - 1)
                        {

                            for (k = black; k <= j; k++)
                                b.SetPixel(i, k, Color.Black);

                        }
                        black = j;
                    }



                }


            }

            for (i = mid + (w - mid) / 4; i <= mid + 3 * (w - mid) / 4; i++)
            {
                black = 0;
                for (j = ys2; j <= ys2 + (h - ys2) / 4; j++)
                {
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {

                        if (black != 0 && black != j - 1)
                        {

                            for (k = black; k <= j; k++)
                                b.SetPixel(i, k, Color.Black);

                        }
                        black = j;
                    }



                }


            }

            ///// pictureBox2.Image = (Image)b;
            int df = 0;
            for (i = ys1 + 1; i < h; i++)
            {
                df = 0;
                for (j = mid; j >= mid / 8; j--)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).B == 0 && b.GetPixel(j, i).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 0)
                {
                    ye1 = i;
                    break;
                }
            }


            for (i = ys2 + 1; i < h; i++)
            {
                df = 0;
                for (j = mid + 1; j <= w - (mid / 8); j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).B == 0 && b.GetPixel(j, i).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 0)
                {
                    ye2 = i;
                    break;
                }
            }

            if ((ye2 - ys2) >= 3 * (ye1 - ys1) / 2)
            {
                ye2 = ys2 + (ye1 - ys1);
            }
            if ((ye1 - ys1) >= 3 * (ye2 - ys2) / 2)
            {
                ye1 = ys1 + (ye2 - ys2);
            }

            int xe1, xe2;
            xe1 = mid;
            xe2 = mid;
            df = 0;
            for (i = mid - 1; i >= 0; i--)
            {
                for (j = ys1; j < ye1; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 1)
                    break;
                xe1 = i;
            }

            df = 0;
            for (i = mid + 1; i < w; i++)
            {
                for (j = ys2; j < ye2; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 1)
                    break;
                xe2 = i;
            }

            int lip_h1 = 0, ac = 0;
            int po1, po2;
            po2 = (((w - xe2) / 2) + xe2);
            po1 = (xe1 / 2);
            int pro, k1;

            for (i = ye1 + 1; i < h - 5; i++)
            {
                int cc = 0;
                for (j = po1; j <= po2; j++)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).B == 0 && b.GetPixel(j, i).G == 0)
                        cc++;
                    /////  b.SetPixel(j,i,Color.Red);
                }
                k1 = (cc * 100) / (po2 - po1);
                if (k1 / 10 > ac / 10)
                {
                    lip_h1 = i;
                    ac = k1;

                }
                if (ac >= 80)
                    break;

            }


            int u1, u2, u3;
            u1 = xe1 / 4;
            u2 = w - (w - xe2) / 4;
            u3 = ys1;
            if (u3 > ys2)
                u3 = ys2;

            if (ye1 > ye2)
                u3 += 5 * ye2 / 6;
            else
                u3 += 5 * ye1 / 6;

            if (u3 >= h)
                u3 = h - 1;

            Bitmap B3 = new Bitmap(u2 - u1, h - u3);
            for (i = u1; i < u2; i++)
                for (j = u3; j < h; j++)
                    B3.SetPixel(i - u1, j - u3, b.GetPixel(i, j));







        }
        private void button_lips_Click(object sender, EventArgs e)
        {

            lip_location((Bitmap)pictureBox1.Image.Clone());

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            _dataBasePath = Directory.GetCurrentDirectory() + @"\db1.mdb";
          
        }

       

        private void button_save_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(save_file);
        }
      

        private void flood_fill(int x, int y, Bitmap bm, Bitmap bmp)
        {
            Color color = new Color();
            int w = bm.Width;
            int h = bm.Height;
            int diff, limit = 20;

            color = bm.GetPixel(x, y);

            visited[x][y] = true;

            if (x - 1 >= 0)
                if (!visited[x - 1][y])
                {
                    diff = Math.Abs(bm.GetPixel(x - 1, y).B - color.B) + Math.Abs(bm.GetPixel(x - 1, y).R - color.R) + Math.Abs(bm.GetPixel(x - 1, y).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x - 1, y, Color.Red);
                        flood_fill(x - 1, y, bm, bmp);
                    }
                }
            if (y - 1 >= 0)
                if (!visited[x][y - 1])
                {
                    diff = Math.Abs(bm.GetPixel(x, y - 1).B - color.B) + Math.Abs(bm.GetPixel(x, y - 1).R - color.R) + Math.Abs(bm.GetPixel(x, y - 1).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x, y - 1, Color.Red);
                        flood_fill(x, y - 1, bm, bmp);
                    }
                }
            if (x + 1 < w)
                if (!visited[x + 1][y])
                {
                    diff = Math.Abs(bm.GetPixel(x + 1, y).B - color.B) + Math.Abs(bm.GetPixel(x + 1, y).R - color.R) + Math.Abs(bm.GetPixel(x + 1, y).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x + 1, y, Color.Red);
                        flood_fill(x + 1, y, bm, bmp);
                    }
                }
            if (y + 1 < h)
                if (!visited[x][y + 1])
                {
                    diff = Math.Abs(bm.GetPixel(x, y + 1).B - color.B) + Math.Abs(bm.GetPixel(x, y + 1).R - color.R) + Math.Abs(bm.GetPixel(x, y + 1).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x, y + 1, Color.Red);
                        flood_fill(x, y + 1, bm, bmp);
                    }
                }


            if (x - 1 >= 0 && y - 1 >= 0)
                if (!visited[x - 1][y - 1])
                {
                    diff = Math.Abs(bm.GetPixel(x - 1, y - 1).B - color.B) + Math.Abs(bm.GetPixel(x - 1, y - 1).R - color.R) + Math.Abs(bm.GetPixel(x - 1, y - 1).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x - 1, y - 1, Color.Red);
                        flood_fill(x - 1, y - 1, bm, bmp);
                    }
                }
            if (x + 1 < w && y - 1 >= 0)
                if (!visited[x + 1][y - 1])
                {
                    diff = Math.Abs(bm.GetPixel(x + 1, y - 1).B - color.B) + Math.Abs(bm.GetPixel(x + 1, y - 1).R - color.R) + Math.Abs(bm.GetPixel(x + 1, y - 1).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x + 1, y - 1, Color.Red);
                        flood_fill(x + 1, y - 1, bm, bmp);
                    }
                }
            if (x + 1 < w && y + 1 < h)
                if (!visited[x + 1][y + 1])
                {
                    diff = Math.Abs(bm.GetPixel(x + 1, y + 1).B - color.B) + Math.Abs(bm.GetPixel(x + 1, y + 1).R - color.R) + Math.Abs(bm.GetPixel(x + 1, y + 1).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x + 1, y + 1, Color.Red);
                        flood_fill(x + 1, y + 1, bm, bmp);
                    }
                }
            if (x - 1 >= 0 && y + 1 < h)
                if (!visited[x - 1][y + 1])
                {
                    diff = Math.Abs(bm.GetPixel(x - 1, y + 1).B - color.B) + Math.Abs(bm.GetPixel(x - 1, y + 1).R - color.R) + Math.Abs(bm.GetPixel(x - 1, y + 1).G - color.G);
                    if (diff < limit)
                    {
                        bmp.SetPixel(x - 1, y + 1, Color.Red);
                        flood_fill(x - 1, y + 1, bm, bmp);
                    }
                }


        }
        private void button_bezier_lips_click(object sender, EventArgs e)
        {


            lips_bezier();

        }

        private void lips_bezier()
        {
            Bitmap ba = new Bitmap(bezier((Bitmap)lips_bezier_candidate.Clone()));
            //////  pictureBox_lips_bezier.Image = (Image)ba;
        }

     

        private void button_test_Click(object sender, EventArgs e)
        {
           

            pictureBox2.Image = white((Image)pictureBox1.Image.Clone());
        }

      

        private void button12_Click(object sender, EventArgs e)
        {
            anti_face_color();
            //conntected2();
        }

        private void anti_face_color_lips()
        {
            Bitmap bm = (Bitmap)pictureBox1.Image;
            Bitmap bmp = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);

            double tot_pixel = bm.Height * bm.Width;
            tot_pixel /= 100;
            tot_pixel *= 10;
            int min_x = bm.Width + 5;// Convert.ToInt16(tot_pixel);
            int max_x = 0;
            int max_y = 0;
            int min_y = bm.Height + 5;// Convert.ToInt16(tot_pixel);


            Color color = new Color();
            double g, r, avg = 0;
            double f_upper, f_lower, w, l;
            bool R1, R2, R3, R4, s;
            double cr, cb;
            R1 = R3 = R4 = s = false;
            R2 = true;
            int c;
          
            cr_start = 140;
            cr_end = 170;
            cb_start = 105;
            cb_end = 150;

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    r = Convert.ToDouble(color.R) / Convert.ToDouble(color.R + color.G + color.B);
                    g = Convert.ToDouble(color.G) / Convert.ToDouble(color.R + color.G + color.B);

                    cb = 0.148 * Convert.ToDouble(color.R) - 0.291 * Convert.ToDouble(color.G) + 0.439 * Convert.ToDouble(color.B) + 128;
                    cr = 0.439 * Convert.ToDouble(color.R) - 0.368 * Convert.ToDouble(color.G) - 0.071 * Convert.ToDouble(color.B) + 128;


                    f_upper = -1.3767 * r * r + 1.0743 * r + 0.1452;
                    f_lower = -0.776 * r * r + 0.5601 * r + 0.1766;

                    if (g > f_lower && g < f_upper)
                        R1 = true;
                    else
                        R1 = false;

                    w = Math.Pow((r - 0.33), 2) + Math.Pow((g - 0.33), 2);
                    //avg += w;
                    //MessageBox.Show(w.ToString());
                    if (w <= 0.0004)
                    {
                        R2 = true;
                        avg++;
                    }
                    else
                        R2 = false;

                    if (color.R > color.G && color.G > color.B)
                        R3 = true;
                    else
                        R3 = false;

                    if ((color.R - color.G) >= 45)
                        R4 = true;
                    else
                        R4 = false;

                    if (R3 && R4)// &&R1)//
                        s = true;
                    else
                        s = false;


                    c = color.R + color.G + color.B;
                    c /= 3;

                    //if ((cr > 140 && cr < 160) && (cb > 105 && cb < 140))actual according to papper
                    if (s)// || c>128)// && c<135)//|| (cr > cr_start && cr < cr_end) && (cb > cb_start && cb < cb_end))//nice result for good image                                     
                    {

                        bmp.SetPixel(i, j, Color.White);
                        R1 = R3 = R4 = s = false;
                        R2 = true;


                    }
                    else
                        bmp.SetPixel(i, j, Color.Black);




                }

            }



            pictureBox2.Image = (Bitmap)bmp;

            pictureBox2.Invalidate();


        }
        private void anti_face_color()
        {
            Bitmap bm = (Bitmap)pictureBox1.Image;
            Bitmap bmp = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);

            double tot_pixel = bm.Height * bm.Width;
            tot_pixel /= 100;
            tot_pixel *= 10;
            int min_x = bm.Width + 5;// Convert.ToInt16(tot_pixel);
            int max_x = 0;
            int max_y = 0;
            int min_y = bm.Height + 5;// Convert.ToInt16(tot_pixel);


            Color color = new Color();
            double g, r, avg = 0;
            double f_upper, f_lower, w, l;
            bool R1, R2, R3, R4, s;
            double cr, cb;
            R1 = R3 = R4 = s = false;
            R2 = true;
            int c;
        
            cr_start = 140;
            cr_end = 170;
            cb_start = 105;
            cb_end = 150;

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    r = Convert.ToDouble(color.R) / Convert.ToDouble(color.R + color.G + color.B);
                    g = Convert.ToDouble(color.G) / Convert.ToDouble(color.R + color.G + color.B);

                    cb = 0.148 * Convert.ToDouble(color.R) - 0.291 * Convert.ToDouble(color.G) + 0.439 * Convert.ToDouble(color.B) + 128;
                    cr = 0.439 * Convert.ToDouble(color.R) - 0.368 * Convert.ToDouble(color.G) - 0.071 * Convert.ToDouble(color.B) + 128;


                    f_upper = -1.3767 * r * r + 1.0743 * r + 0.1452;
                    f_lower = -0.776 * r * r + 0.5601 * r + 0.1766;

                    if (g > f_lower && g < f_upper)
                        R1 = true;
                    else
                        R1 = false;

                    w = Math.Pow((r - 0.33), 2) + Math.Pow((g - 0.33), 2);
                    //avg += w;
                    //MessageBox.Show(w.ToString());
                    if (w <= 0.0004)
                    {
                        R2 = true;
                        avg++;
                    }
                    else
                        R2 = false;

                    if (color.R > color.G && color.G > color.B)
                        R3 = true;
                    else
                        R3 = false;

                    if ((color.R - color.G) >= 45)
                        R4 = true;
                    else
                        R4 = false;

                    if (R3 && R4)// &&R1)//
                        s = true;
                    else
                        s = false;


                    c = color.R + color.G + color.B;
                    c /= 3;

                    //if ((cr > 140 && cr < 160) && (cb > 105 && cb < 140))actual according to papper
                    if (s)// || c>128)// && c<135)//|| (cr > cr_start && cr < cr_end) && (cb > cb_start && cb < cb_end))//nice result for good image                                     
                    {

                        bmp.SetPixel(i, j, Color.White);
                        R1 = R3 = R4 = s = false;
                        R2 = true;


                    }
                    else
                        bmp.SetPixel(i, j, Color.Black);




                }

            }


            bmp = erosion(bmp);
            pictureBox2.Image = (Bitmap)bmp;
            bmp = erosion(bmp);
            pictureBox2.Image = (Bitmap)bmp;

            bmp = dilation(bmp);
            pictureBox2.Image = (Bitmap)bmp;
            bmp = dilation(bmp);
            pictureBox2.Image = (Bitmap)bmp;

            pictureBox2.Invalidate();


        }

        private void button_extract_eye2_Click(object sender, EventArgs e)
        {
            anti_face_color();
            conntected1();
        }
        private void button_eye_extrct4_Click(object sender, EventArgs e)
        {
            my_eye();
        }

        private void my_eye()
        {
            anti_face_color();
            conntected_black();
            conntected2();
        }

        Image pic_box_image;

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (pic_box_image != null)
                {
                    Form_preview newform = new Form_preview();
                    newform.image = pic_box_image;
                    newform.set_image();
                    newform.ShowDialog();
                }
            }
            catch
            {

            }

        }
        PictureBox pic_box;
        private void pictureBox_MouseClick(object sender, EventArgs e)
        {
            try
            {

                pic_box = new PictureBox();
                pic_box = sender as PictureBox;
                if (pic_box.Image != null)
                    pic_box_image = (Image)pic_box.Image.Clone();
            }
            catch
            {
            }
        }

        public Bitmap left_eye(Bitmap b)
        {

            b = black_and_white(b);
            int i, j, max, x, h, w, y;
            max = 0;
            x = 0;

            h = b.Height;
            w = b.Width;
            y = h - 1;
            for (i = 2; i < 2 * h / 3; i++)
            {
                for (j = 0; j < w; j++)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        break;
                }
                if (j == w)
                    j = w - 1;
                if (max <= j && j != 0)
                {
                    max = j;
                    x = i;
                }
                if (i >= h / 2 && j < w / 3)
                    break;
            }

            for (i = h - 1; i >= h / 2; i--)
            {
                int count = 0;
                for (j = w / 4; j <= 3 * w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        count++;
                if (count * 2 >= w / 2)
                    break;
            }
            if (i != h - 1)
                y = i + 1;

            int le_1 = 0, ri_1 = w - 1, ttt = 0;
            ttt = 0;
            for (j = 0; j < w; j++)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    le_1 = j;
                    break;
                }
            }
            ttt = 0;
            for (j = w - 1; j >= 0; j--)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    ri_1 = j;
                    break;
                }
            }

            ttt = 0;
            for (i = x; i < y; i++)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    x = i;
                    break;
                }
            }

            ttt = 0;
            for (i = y; i >= x; i--)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    y = i;
                    break;
                }
            }


            b = new Bitmap(pictureBox1.Image);
            Bitmap BB = new Bitmap(w - le_1, h - x);

            for (i = le_1; i < w; i++)
                for (j = x; j < h; j++)
                    BB.SetPixel(i - le_1, j - x, b.GetPixel(i, j));

            return BB;

        }
        private Bitmap right_eye(Bitmap b)
        {
            b = black_and_white(b);
            int i, j, max, x, h, w, y;
            max = 0;
            x = 0;

            h = b.Height;
            w = b.Width;
            y = h - 1;
            for (i = 2; i < 2 * h / 3; i++)
            {
                for (j = w - 1; j >= 0; j--)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        break;
                }
                j = w - j;
                if (j == w)
                    j = w - 1;
                if (max <= j && j != 0)
                {
                    max = j;
                    x = i;
                }
                if (i >= h / 2 && j < w / 3)
                    break;
            }

            for (i = h - 1; i >= h / 2; i--)
            {
                int count = 0;
                for (j = w / 4; j <= 3 * w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        count++;
                if (count * 2 >= w / 2)
                    break;
            }
            if (i != h - 1)
                y = i + 1;

            int le_1 = 0, ri_1 = w - 1, ttt = 0;
            ttt = 0;
            for (j = 0; j < w; j++)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    le_1 = j;
                    break;
                }
            }
            ttt = 0;
            for (j = w - 1; j >= 0; j--)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    ri_1 = j;
                    break;
                }
            }


            ttt = 0;
            for (i = x; i < y; i++)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    x = i;
                    break;
                }
            }

            ttt = 0;
            for (i = y; i >= x; i--)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    y = i;
                    break;
                }
            }




            b = new Bitmap(pictureBox1.Image);
            if (x >= y)
                x = y - 2;

            le_1 -= 3;
            if (le_1 < 0)
                le_1 = 0;
            x -= 3;
            if (x < 0)
                x = 0;

            le_1 -= 3;
            if (le_1 < 0)
                le_1 = 0;
            x -= 3;
            if (x < 0)
                x = 0;

            Bitmap BB = new Bitmap(w - le_1, h - x);

            for (i = le_1; i < w; i++)
                for (j = x; j < h; j++)
                    BB.SetPixel(i - le_1, j - x, b.GetPixel(i, j));

            return BB;
        }

        private int find_first_neighbour_eye(int x, int y)
        {
            int w = Bit.Width;
            int h = Bit.Height;

            if (x - 1 >= 0)
                if (!visited[x - 1][y] && similar(x, y, x - 1, y) == 1)
                {
                    first_i = x - 1;
                    first_j = y;
                    return 1;
                }
            if (y - 1 >= 0)
                if (!visited[x][y - 1] && similar(x, y, x, y - 1) == 1)
                {
                    first_i = x;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w)
                if (!visited[x + 1][y] && similar(x, y, x + 1, y) == 1)
                {
                    first_i = x + 1;
                    first_j = y;
                    return 1;
                }
            if (y + 1 < h)
                if (!visited[x][y + 1] && similar(x, y, x, y + 1) == 1)
                {
                    first_i = x;
                    first_j = y + 1;
                    return 1;
                }


            if (x - 1 >= 0 && y - 1 >= 0)
                if (!visited[x - 1][y - 1] && similar(x, y, x - 1, y - 1) == 1)
                {
                    first_i = x - 1;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w && y - 1 >= 0)
                if (!visited[x + 1][y - 1] && similar(x, y, x + 1, y - 1) == 1)
                {
                    first_i = x + 1;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w && y + 1 < h)
                if (!visited[x + 1][y + 1] && similar(x, y, x + 1, y + 1) == 1)
                {
                    first_i = x + 1;
                    first_j = y + 1;
                    return 1;
                }
            if (x - 1 >= 0 && y + 1 < h)
                if (!visited[x - 1][y + 1] && similar(x, y, x - 1, y + 1) == 1)
                {
                    first_i = x - 1;
                    first_j = y + 1;
                    return 1;
                }

            return -1;

        }



        private void BFS_eye(int i, int j)
        {

            visited[i][j] = true;
            queue_i.Enqueue(i);
            queue_j.Enqueue(j);

            int w;
            while (queue_i.Count != 0)
            {
                i = queue_i.Dequeue(); //deque
                j = queue_j.Dequeue(); // deque


                w = find_first_neighbour_eye(i, j); //find first neighbour...if no neighbour return -1

                while (w != -1) //visit all 8 neighbours
                {
                    if (!visited[first_i][first_j])//unvisited nodes
                    {
                        visited[first_i][first_j] = true;

                        queue_i.Enqueue(first_i);//enque
                        queue_j.Enqueue(first_j);//enque

                    }

                    w = find_first_neighbour_eye(i, j);// again find first neighbour...if no neighbour return -1
                }

            }

        }

        public Bitmap skin_color(Bitmap b)
        {
            /******************/
            Bit = b;
            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);


            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion


            for (int i = 0; i < Bit.Width; i++)
                if (!visited[i][0])
                    BFS_eye(i, 0);
            for (int i = 0; i < Bit.Width; i++)
                if (!visited[i][Bit.Height - 1])
                    BFS_eye(i, Bit.Height - 1);
            for (int i = 0; i < Bit.Height; i++)
                if (!visited[0][i])
                    BFS_eye(0, i);
            for (int i = 0; i < Bit.Height; i++)
                if (!visited[Bit.Width - 1][i])
                    BFS_eye(Bit.Width - 1, i);



            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (visited[i][j])
                        Bit.SetPixel(i, j, Color.White);
                    else
                        Bit.SetPixel(i, j, Color.Black);
                }
            }


            return Bit;

        }



        public int similar(int x, int y, int x1, int y1)
        {
            int R, G, B;
            R = Math.Abs(Bit.GetPixel(x, y).R - Bit.GetPixel(x1, y1).R);
            G = Math.Abs(Bit.GetPixel(x, y).G - Bit.GetPixel(x1, y1).G);
            B = Math.Abs(Bit.GetPixel(x, y).B - Bit.GetPixel(x1, y1).B);
            if (R < 10 && G < 10 && B < 10)
                return 1;
            return 0;
        }


        private void button_eye1_Click(object sender, EventArgs e)
        {

            right_eye2();

        }

        private void right_eye2()
        {
            ///// pictureBox1.Image = (Bitmap)pictureBox4.Image.Clone();
            Bitmap b = new Bitmap((Image)pictureBox1.Image.Clone());
            Bitmap ba = new Bitmap(right_eye(b));
            pictureBox2.Image = (Image)ba;
            pictureBox1.Image = (Image)ba.Clone();

            ba = new Bitmap(skin_color(ba));
            pictureBox2.Image = (Image)ba;


            b = new Bitmap((Image)pictureBox2.Image.Clone());
            ba = new Bitmap(connected_eye(b));
            pictureBox2.Image = (Image)ba;
            ///// pictureBox_right_eye.Image = (Image)ba.Clone();

            /*Bitmap bs = new Bitmap(pictureBox2.Image.Clone());
            Bitmap bg = bezier_eye(bs);
            pictureBox2.Image = (Image)bg;*/

        }

        private void button1_eye2_Click(object sender, EventArgs e)
        {
            left_eye2();
        }

        private void left_eye2()
        {
            ////// pictureBox1.Image = (Bitmap)pictureBox3.Image.Clone();
            Bitmap b = new Bitmap((Image)pictureBox1.Image.Clone());
            Bitmap ba = new Bitmap(left_eye(b));
            pictureBox2.Image = (Image)ba;
            pictureBox1.Image = (Image)ba.Clone();

            ba = new Bitmap(skin_color(ba));
            pictureBox2.Image = (Image)ba;


            b = new Bitmap((Image)pictureBox2.Image.Clone());
            ba = new Bitmap(connected_eye(b));
            pictureBox2.Image = (Image)ba;
            //////   pictureBox_left_eye.Image = (Image)ba.Clone();
        }

        private void button1_to_2_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = (Bitmap)pictureBox1.Image;
        }

        void slope(long x1, long y1, long x2, long y2)
        {
            long x3, y3, i, p, q;
            double x, y, dx, dy, b, m;

            x = x1;
            y = y1;
            dx = x2 - x1;
            dy = y2 - y1;

            if (dx >= dy && dx != 0.0)
            {
                if (x1 > x2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                    y3 = y1;
                    y1 = y2;
                    y2 = y3;
                    x = x1;
                    y = y1;
                    dx = x2 - x1;
                    dy = y2 - y1;
                }
                m = dy / dx;
                p = Convert.ToInt16(x);
                q = Convert.ToInt16(y);
                big[p][q] = 1;
                while (x1 < x2)
                {
                    b = y - m * x;
                    x++;
                    y = m * x + b;
                    p = Convert.ToInt16(x);
                    q = Convert.ToInt16(y);
                    big[p][q] = 1;
                    x1++;
                }
            }
            else if (dy != 0.0 && dx != 0.0)
            {
                if (y1 > y2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                    y3 = y1;
                    y1 = y2;
                    y2 = y3;
                    x = x1;
                    y = y1;
                    dx = x2 - x1;
                    dy = y2 - y1;
                }
                m = dx / dy;
                p = Convert.ToInt16(x);
                q = Convert.ToInt16(y);
                big[p][q] = 1;
                while (y1 < y2)
                {
                    b = y - (x / m);
                    y++;
                    x = m * (y - b);
                    p = Convert.ToInt16(x);
                    q = Convert.ToInt16(y);
                    big[p][q] = 1;
                    y1++;
                }
            }
            else
            {
                if (y1 > y2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                    y3 = y1;
                    y1 = y2;
                    y2 = y3;
                    x = x1;
                    y = y1;
                    dx = x2 - x1;
                    dy = y2 - y1;
                }
                while (y1 <= y2)
                {
                    p = Convert.ToInt16(x);
                    q = Convert.ToInt16(y);
                    big[p][q] = 1;
                    y++;
                    y1++;
                }
            }

        }
        double[] b1;
        double[] p1;

        public Bitmap bezier(Bitmap b)
        {
            int n, m;
            n = b.Height;
            m = b.Width;
            big = new int[m][];


            int i, j = 0, flag = 0;

            for (i = 0; i < m; i++)
            {
                big[i] = new int[n];
                for (j = 0; j < n; j++)
                    big[i][j] = 0;
            }
            int count = 0;
            b1 = new double[1000];
            p1 = new double[1000];
            int x1 = 0, y1 = 0, xn = 0, yn = 0, xm1 = 0, ym1 = 0, xm2 = 0, ym2 = 0, xm3 = 0, ym3 = 0, xm4 = 0, ym4 = 0;
            int yz1 = -1, yz2 = -1;

            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        if (yz1 == -1)
                        { yz1 = j; yz2 = j; }
                        else
                            yz2 = j;
                    }
                if (yz1 != -1)
                {
                    x1 = i;
                    y1 = (yz1 + yz2) / 2;
                    break;
                }
            }

            yz1 = -1; yz2 = -1;
            for (i = m - 1; i >= 0; i--)
            {
                for (j = 0; j < n; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        if (yz1 == -1)
                        { yz1 = j; yz2 = j; }
                        else
                            yz2 = j;
                    }
                if (yz1 != -1)
                {
                    xn = i;
                    yn = (yz1 + yz2) / 2;
                    break;
                }
            }

            //////////Uper Lip///////////////
            ///////left/////////////////////
            int Q, R, T, start_x, p;
            double pi = 22 / 7;
            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p - T;
                    if (R >= m || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm1 = x1 + (xn - x1) / 3;
            R = xm1 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym1 = T;
            if (ym1 < 0)
                ym1 = 0;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p - T;
                    if (R < 0 || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm2 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm2;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym2 = T;
            if (ym2 < 0)
                ym2 = 0;
            /////////right////////////////
            //////////Uper Lip///////////////
            //////////Lower Lip///////////////
            ///////left/////////////////////

            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p + T;
                    if (R >= m || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm3 = x1 + (xn - x1) / 3;
            R = xm3 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym3 = T;
            if (ym3 > n)
                ym3 = n - 1;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p + T;
                    if (R < 0 || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm4 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm4;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym4 = T;
            if (ym4 > n)
                ym4 = n - 1;

            for (i = 0; i < n; i++)
                if (b.GetPixel(xm1, i).R == 0 && b.GetPixel(xm1, i).G == 0 && b.GetPixel(xm1, i).B == 0)
                { ym1 = i; break; }
            for (i = 0; i < n; i++)
                if (b.GetPixel(xm2, i).R == 0 && b.GetPixel(xm2, i).G == 0 && b.GetPixel(xm2, i).B == 0)
                { ym2 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm3, i).R == 0 && b.GetPixel(xm3, i).G == 0 && b.GetPixel(xm3, i).B == 0)
                { ym3 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm4, i).R == 0 && b.GetPixel(xm4, i).G == 0 && b.GetPixel(xm4, i).B == 0)
                { ym4 = i; break; }

            b1[2 * count] = x1;
            b1[2 * count + 1] = y1;
            count++;
            b1[2 * count] = xm1;
            b1[2 * count + 1] = ym1;
            count++;
            b1[2 * count] = xm2;
            b1[2 * count + 1] = ym2;
            count++;
            b1[2 * count] = xn;
            b1[2 * count + 1] = yn;
            count++;

            bLength = count * 2;
            Bezier2D(10);
            int x, y;
            int xs, ys, xe, ye;

            xs = Convert.ToInt16(p1[0]);
            ys = Convert.ToInt16(p1[1]);
            xe = Convert.ToInt16(p1[16]);
            ye = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < xs)
                { xs = x; ys = y; }
                if (x1 < xs)
                { xs = x1; ys = y1; }
                if (x > xe)
                { xe = x; ye = y; }
                if (x1 > xe)
                { xe = x1; ye = y1; }
            }


            b1[2 * 1 + 1] = ym3;
            b1[2 * 2 + 1] = ym4;
            bLength = count * 2;
            Bezier2D(10);
            int x1s, y1s, x1e, y1e;

            x1s = Convert.ToInt16(p1[0]);
            y1s = Convert.ToInt16(p1[1]);
            x1e = Convert.ToInt16(p1[16]);
            y1e = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < x1s)
                { x1s = x; y1s = y; }
                if (x1 < x1s)
                { x1s = x1; y1s = y1; }
                if (x > x1e)
                { x1e = x; y1e = y; }
                if (x1 > x1e)
                { x1e = x1; y1e = y1; }
            }


            slope(xs, ys, x1s, y1s);
            slope(xe, ye, x1e, y1e);

            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                {
                    if (big[i][j] == 1)
                        b.SetPixel(i, j, Color.Black);
                    else
                        b.SetPixel(i, j, Color.White);
                }


            return b;


        }

        public Bitmap bezier_eye(Bitmap b)
        {

            int n, m;
            n = b.Height;
            m = b.Width;
            big = new int[m][];


            int i, j = 0, flag = 0;

            for (i = 0; i < m; i++)
            {
                big[i] = new int[n];
                for (j = 0; j < n; j++)
                    big[i][j] = 0;
            }
            int count = 0;
            b1 = new double[1000];
            p1 = new double[1000];
            int x1 = 0, y1 = 0, xn = 0, yn = 0, xm1 = 0, ym1 = 0, xm2 = 0, ym2 = 0, xm3 = 0, ym3 = 0, xm4 = 0, ym4 = 0;
            int yz1 = -1, yz2 = -1;

            for (i = 0; i < m; i++)
            {
                for (j = n - 1; j >= 0; j--)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        x1 = i;
                        y1 = j;
                        yz1 = 1;
                        break;
                    }
                if (yz1 != -1)
                {
                    break;
                }
            }

            yz1 = -1; yz2 = -1;
            for (i = m - 1; i >= 0; i--)
            {
                for (j = n - 1; j >= 0; j--)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        xn = i;
                        yn = j;
                        yz1 = 1;
                    }
                if (yz1 != -1)
                {
                    break;
                }
            }

            //////////Uper Lip///////////////
            ///////left/////////////////////
            int Q, R, T, start_x, p;
            double pi = 22 / 7;
            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p - T;
                    if (R >= m || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm1 = x1 + (xn - x1) / 3;
            R = xm1 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym1 = T;
            if (ym1 < 0)
                ym1 = 0;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p - T;
                    if (R < 0 || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm2 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm2;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym2 = T;
            if (ym2 < 0)
                ym2 = 0;
            /////////right////////////////
            //////////Uper Lip///////////////
            //////////Lower Lip///////////////
            ///////left/////////////////////

            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p + T;
                    if (R >= m || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm3 = x1 + (xn - x1) / 3;
            R = xm3 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym3 = T;
            if (ym3 > n)
                ym3 = n - 1;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p + T;
                    if (R < 0 || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm4 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm4;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym4 = T;
            if (ym4 > n)
                ym4 = n - 1;

            for (i = 0; i < n; i++)
                if (b.GetPixel(xm1, i).R == 0 && b.GetPixel(xm1, i).G == 0 && b.GetPixel(xm1, i).B == 0)
                { ym1 = i; break; }
            for (i = 0; i < n; i++)
                if (b.GetPixel(xm2, i).R == 0 && b.GetPixel(xm2, i).G == 0 && b.GetPixel(xm2, i).B == 0)
                { ym2 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm3, i).R == 0 && b.GetPixel(xm3, i).G == 0 && b.GetPixel(xm3, i).B == 0)
                { ym3 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm4, i).R == 0 && b.GetPixel(xm4, i).G == 0 && b.GetPixel(xm4, i).B == 0)
                { ym4 = i; break; }

            b1[2 * count] = x1;
            b1[2 * count + 1] = y1;
            count++;
            b1[2 * count] = xm1;
            b1[2 * count + 1] = ym1;
            count++;
            b1[2 * count] = xm2;
            b1[2 * count + 1] = ym2;
            count++;
            b1[2 * count] = xn;
            b1[2 * count + 1] = yn;
            count++;

            bLength = count * 2;
            Bezier2D(10);
            int x, y;
            int xs, ys, xe, ye;

            xs = Convert.ToInt16(p1[0]);
            ys = Convert.ToInt16(p1[1]);
            xe = Convert.ToInt16(p1[16]);
            ye = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < xs)
                { xs = x; ys = y; }
                if (x1 < xs)
                { xs = x1; ys = y1; }
                if (x > xe)
                { xe = x; ye = y; }
                if (x1 > xe)
                { xe = x1; ye = y1; }
            }


            b1[2 * 1 + 1] = ym3;
            b1[2 * 2 + 1] = ym4;
            bLength = count * 2;
            Bezier2D(10);
            int x1s, y1s, x1e, y1e;

            x1s = Convert.ToInt16(p1[0]);
            y1s = Convert.ToInt16(p1[1]);
            x1e = Convert.ToInt16(p1[16]);
            y1e = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < x1s)
                { x1s = x; y1s = y; }
                if (x1 < x1s)
                { x1s = x1; y1s = y1; }
                if (x > x1e)
                { x1e = x; y1e = y; }
                if (x1 > x1e)
                { x1e = x1; y1e = y1; }
            }


            slope(xs, ys, x1s, y1s);
            slope(xe, ye, x1e, y1e);
            Bitmap bp = new Bitmap(m, n);
            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                {
                    if (big[i][j] == 1)
                        bp.SetPixel(i, j, Color.Black);
                    else
                        bp.SetPixel(i, j, Color.White);
                }

            return bp;

        }
        void Bezier2D(int cpts)
        {
            ////   int npts = (b.Length) / 2;
            int npts = (bLength) / 2;
            int icount, jcount;
            double step, t;

            // Calculate points on curve

            icount = 0;
            t = 0;
            step = (double)1.0 / (cpts - 1);

            for (int i1 = 0; i1 != cpts; i1++)
            {
                if ((1.0 - t) < 5e-6)
                    t = 1.0;

                jcount = 0;
                p1[icount] = 0.0;
                p1[icount + 1] = 0.0;
                for (int i = 0; i != npts; i++)
                {
                    double basis = Bernstein(npts - 1, i, t);
                    p1[icount] += basis * b1[jcount];
                    p1[icount + 1] += basis * b1[jcount + 1];
                    jcount = jcount + 2;
                }

                icount += 2;
                t += step;
            }
            /////	    for (i1 = 0; i1 != cpts; i1++)
            ///////	    printf("Point %ld : %lf %lf\n",i1,p[2*i1],p[2*i1+1]);
        }
        /* function to calculate the factorial */

        double factrl(int n)
        {
            int ntop = 6;
            double[] a = new double[100];
            ///////[1.0,1.0,2.0,6.0,24.0,120.0,720.0] /* fill in the first few values */
            int j1;
            a[0] = 1;
            for (int i = 1; i <= 6; i++)
                a[i] = a[i - 1] * i;
           
            { 
                int j;
                if (ntop < n)
                    for (j = 7; j <= n; j++)
                        a[j] = a[j - 1] * j;
            }
            return a[n]; /* returns the value n! as a floating point number */
        }

        /* function to calculate the factorial function for Bernstein basis */

        double Ni(int n, int i)
        {
            double ni;
            ni = factrl(n) / (factrl(i) * factrl(n - i));
            return ni;
        }

        /* function to calculate the Bernstein basis */

        double Bernstein(int n, int i, double t)
        {
            double basis;
            double ti; /* this is t^i */
            double tni; /* this is (1 - t)^i */

            /* handle the special cases to avoid domain problem with pow */

            if (t == 0.0 && i == 0) ti = 1.0; else ti = Math.Pow(t, i);
            if (n == i && t == 1.0) tni = 1.0; else tni = Math.Pow((1 - t), (n - i));
            basis = Ni(n, i) * ti * tni; /* calculate Bernstein basis function */
            return basis;
        }
      


        int[] AI;
        public void Point_detection(Bitmap b)
        {
            int w, h, i, j, k, l;
            w = b.Width;
            h = b.Height;
            k = -1;
            for (i = 0; i < w; i++)
            {
                for (j = 0; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    { k = i; break; }
                if (k != -1)
                    break;
            }
            l = -1;
            for (i = w - 1; i >= 0; i--)
            {
                for (j = 0; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    { l = i; break; }
                if (l != -1)
                    break;
            }
            AI[0] = l - k;

            k = -1;
            for (j = 0; j < h; j++)
            {
                for (i = 0; i < w; i++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    { k = j; break; }
                if (k != -1)
                    break;
            }
            l = -1;
            for (j = h - 1; j >= 0; j--)
            {
                for (i = w - 1; i >= 0; i--)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    { l = j; break; }
                if (l != -1)
                    break;
            }
            AI[1] = l - k;

        }

        private string take_decion()
        {
          
            return "Ambiguous";

        }
       

        private void button_sharpen_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap m_Bitmap = (Bitmap)pictureBox1.Image;
                undo_picture = m_Bitmap;
                if (BitmapFilter.Sharpen(m_Bitmap, 11))
                    this.Invalidate();
                pictureBox1.Invalidate();
            }
            catch
            {

            }
        }

        private void sharpenImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap m_Bitmap = (Bitmap)pic_box.Image;
                undo_picture = m_Bitmap;
                Bitmap mm_bit = (Bitmap)m_Bitmap.Clone();
                if (BitmapFilter.Sharpen(mm_bit, 11))
                    this.Invalidate();
                pic_box.Image = (Bitmap)mm_bit;
                pic_box.Invalidate();
            }
            catch
            {

            }
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Parameter dlg = new Parameter();
                dlg.nValue = 0;

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    Bitmap m_Bitmap = (Bitmap)pic_box.Image;
                    undo_picture = m_Bitmap;
                    Bitmap mm_bit = (Bitmap)m_Bitmap.Clone();
                    if (BitmapFilter.Contrast(mm_bit, (sbyte)dlg.nValue))
                        this.Invalidate();

                    pic_box.Image = (Bitmap)mm_bit;
                    pic_box.Invalidate();
                }
            }
            catch
            {

            }
        }

        private void restoreImageToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                pic_box.Image = (Image)undo_picture.Clone();
                pic_box.Invalidate();
            }
            catch
            {
            }

        }

        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Parameter dlg = new Parameter();
                dlg.nValue = 0;

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    Bitmap m_Bitmap = (Bitmap)pic_box.Image;
                    undo_picture = m_Bitmap;
                    Bitmap mm_bit = (Bitmap)m_Bitmap.Clone();
                    if (BitmapFilter.Brightness(mm_bit, (sbyte)dlg.nValue))
                        this.Invalidate();

                    pic_box.Image = (Bitmap)mm_bit;
                    pic_box.Invalidate();

                }
            }
            catch
            {

            }
        }

        private void SaveImagetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                pic_box.Image.Save(save_file);
        }

        public Bitmap big_conect(Bitmap b)
        {

            int start = 1, i, j;
            Bit = new Bitmap(b);

            int hight = Bit.Height;
            int weight = Bit.Width;
            ///hight -= 5;
            ///weight -= 5;

            big = new int[weight + 5][];
            count = new int[1000];

            for (i = 0; i < weight + 5; i++)
            {
                big[i] = new int[hight + 5];
                for (j = 0; j < hight + 5; j++)
                    big[i][j] = 0;
            }
            int st = 0;
            for (i = 0; i < weight; i++)
                for (j = 0; j < hight; j++)
                    if (big[i][j] == 0 && Bit.GetPixel(i, j).B == 0 && Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0)
                    {
                        count[start] = 1;
                        st = 1;
                        big[i][j] = start;
                        conect(i, j, weight, hight);
                        start++;
                    }

            int max = 0, value = 1;
            for (i = 1; i < start; i++)
                if (count[i] > max)
                {
                    max = count[i];
                    value = i;
                }

            for (i = 0; i < weight; i++)
                for (j = 0; j < hight; j++)
                {
                    if (big[i][j] == value)
                        Bit.SetPixel(i, j, Color.Black);
                    else
                        Bit.SetPixel(i, j, Color.White);
                }
            /////label1.Text = "  " + max+"  ";
            /////pictureBox2.Image = (Image)Bit;
            Bit = shape(Bit);

            return Bit;

        }

        public void conect(int x, int y, int w, int h)
        {
            int[] bfs_x = new int[w * h];
            int[] bfs_y = new int[w * h];
            int n, m;
            bfs_x[0] = x;
            bfs_y[0] = y;
            n = 0;
            m = 1;

            while (n != m)
            {
                x = bfs_x[n];
                y = bfs_y[n];

                if (x - 1 >= 0)
                    if (Bit.GetPixel(x - 1, y).B == 0 && Bit.GetPixel(x - 1, y).R == 0 && Bit.GetPixel(x - 1, y).G == 0 && big[x - 1][y] == 0)
                    {
                        count[big[x][y]]++;
                        big[x - 1][y] = big[x][y];
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y;
                        m++;
                        ////conect(x - 1, y, w, h);
                    }
                if (y - 1 >= 0)
                    if (Bit.GetPixel(x, y - 1).B == 0 && Bit.GetPixel(x, y - 1).R == 0 && Bit.GetPixel(x, y - 1).G == 0 && big[x][y - 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x][y - 1] = big[x][y];
                        bfs_x[m] = x;
                        bfs_y[m] = y - 1;
                        m++;
                        ////conect(x, y - 1, w, h);
                    }
                if (x + 1 < w)
                    if (Bit.GetPixel(x + 1, y).B == 0 && Bit.GetPixel(x + 1, y).R == 0 && Bit.GetPixel(x + 1, y).G == 0 && big[x + 1][y] == 0)
                    {
                        count[big[x][y]]++;
                        big[x + 1][y] = big[x][y];
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y;
                        m++;
                        ////conect(x + 1, y, w, h);
                    }
                if (y + 1 < h)
                    if (Bit.GetPixel(x, y + 1).B == 0 && Bit.GetPixel(x, y + 1).R == 0 && Bit.GetPixel(x, y + 1).G == 0 && big[x][y + 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x][y + 1] = big[x][y];
                        bfs_x[m] = x;
                        bfs_y[m] = y + 1;
                        m++;
                        /////conect(x, y + 1, w, h);
                    }


                if (x - 1 >= 0 && y - 1 >= 0)
                    if (Bit.GetPixel(x - 1, y - 1).B == 0 && Bit.GetPixel(x - 1, y - 1).R == 0 && Bit.GetPixel(x - 1, y - 1).G == 0 && big[x - 1][y - 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x - 1][y - 1] = big[x][y];
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y - 1;
                        m++;
                        /////conect(x - 1, y - 1, w, h);
                    }
                if (x + 1 < w && y - 1 >= 0)
                    if (Bit.GetPixel(x + 1, y - 1).B == 0 && Bit.GetPixel(x + 1, y - 1).R == 0 && Bit.GetPixel(x + 1, y - 1).G == 0 && big[x + 1][y - 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x + 1][y - 1] = big[x][y];
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y - 1;
                        m++;
                        /////conect(x + 1, y - 1, w, h);
                    }
                if (x + 1 < w && y + 1 < h)
                    if (Bit.GetPixel(x + 1, y + 1).B == 0 && Bit.GetPixel(x + 1, y + 1).R == 0 && Bit.GetPixel(x + 1, y + 1).G == 0 && big[x + 1][y + 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x + 1][y + 1] = big[x][y];
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y + 1;
                        m++;
                        ///// conect(x + 1, y + 1, w, h);
                    }
                if (x - 1 >= 0 && y + 1 < h)
                    if (Bit.GetPixel(x - 1, y + 1).B == 0 && Bit.GetPixel(x - 1, y + 1).R == 0 && Bit.GetPixel(x - 1, y + 1).G == 0 && big[x - 1][y + 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x - 1][y + 1] = big[x][y];
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y + 1;
                        m++;
                        ////conect(x - 1, y + 1, w, h);
                    }
                n++;
            }

        }

        private void button_auto_Click(object sender, EventArgs e)
        {

            string location = @"c:\babu\";

            if (!Directory.Exists(@"C:\experiment\face\"))
                Directory.CreateDirectory(@"C:\experiment\face\");

            if (!Directory.Exists(@"C:\experiment\left eye0\"))
                Directory.CreateDirectory(@"C:\experiment\left eye0\");

            if (!Directory.Exists(@"C:\experiment\right eye0\"))
                Directory.CreateDirectory(@"C:\experiment\right eye0\");

            if (!Directory.Exists(@"C:\experiment\left eye bezier0\"))
                Directory.CreateDirectory(@"C:\experiment\left eye bezier0\");

            if (!Directory.Exists(@"C:\experiment\right eye bezier0\"))
                Directory.CreateDirectory(@"C:\experiment\right eye bezier0\");

            if (!Directory.Exists(@"C:\experiment\lips0\"))
                Directory.CreateDirectory(@"C:\experiment\lips0\");

            if (!Directory.Exists(@"C:\experiment\lips bezier0\"))
                Directory.CreateDirectory(@"C:\experiment\lips bezier0\");

            //@"C:\experiment\lips bezier0\"
            int ttt = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(location);
                FileInfo[] files = di.GetFiles();

                foreach (FileInfo file in files)
                {

                    pictureBox1.Image = (Bitmap)Image.FromFile(file.FullName);
                    skin_color_segmentation();
                    connected_area();
                    pictureBox2.Image.Save(@"C:\experiment\face\" + file.Name);
                   
                    my_eye();
                    
                    my_eye();
                    
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }

        }

        private void button_lips_me_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = (Image)pictureBox1.Image.Clone();
            anti_face_color_lips();

            pictureBox2.Image = dilation((Bitmap)pictureBox2.Image.Clone());
            pictureBox2.Image = dilation((Bitmap)pictureBox2.Image.Clone());
            pictureBox2.Image = dilation((Bitmap)pictureBox2.Image.Clone());
            pictureBox2.Image = erosion((Bitmap)pictureBox2.Image.Clone());
            pictureBox2.Image = erosion((Bitmap)pictureBox2.Image.Clone());

            conntected_lips();

            pictureBox1.Image = (Image)pictureBox2.Image.Clone();
            
        }

        private void button7_Click_1(object sender, EventArgs e)
        {

            lips_bezier_draw();

        }

        private void lips_bezier_draw()
        {
            Bitmap b = new Bitmap((Image)pictureBox1.Image.Clone());
            Bitmap ba = new Bitmap(skin_color(b));
            pictureBox1.Image = (Image)ba;

            b = new Bitmap(pictureBox1.Image);
            ba = new Bitmap(big_conect(b));
            pictureBox1.Image = (Image)ba;

            b = new Bitmap(pictureBox1.Image);
            ba = new Bitmap(bezier(b));
            pictureBox1.Image = (Image)ba;

         
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button_robot_control_Click(object sender, EventArgs e)
        {
           
            string FILE_NAME = @"c:\input.txt";

           

            TextWriter tw = new StreamWriter(FILE_NAME);

        }

        private void numericUpDown_cr_start_ValueChanged(object sender, EventArgs e)
        {

        }

        private void detect_face()
        {
            Bitmap b = new Bitmap(pictureBox2.Image);

            long newHeight = b.Height;
            long newWeight = b.Width;
            double Ratio = (Convert.ToDouble(newHeight) / Convert.ToDouble(newWeight));

            if ((Ratio >= 1.0 && Ratio <= 2.0) && newHeight >= 50 && newWeight >= 50)
            {
                Form2 a = new Form2();
                a.pic(b);
                a.DataBasePath(_dataBasePath);
                //a.Show();

                panel_container.Controls.Clear();



                a.TopLevel = false;
                a.FormBorderStyle = FormBorderStyle.None;
                panel_container.Controls.Add(a);
                a.Visible = true;
            }
            else
            {
                MessageBox.Show("This is not a Human Face or this image is too small for fine an emotion.\n Please try again.");
                if (newHeight < 50 || newWeight < 50)
                {
                    MessageBox.Show("There may be no face in the image.\nSo, please select another image.");
                    // button_connected.Enabled = false;
                    // button_skincolor.Enabled = false;
                    //button2.Enabled = false;
                }
            }

        }

        

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            save_file = saveFileDialog1.FileName;
            Bitmap b = new Bitmap(pictureBox2.Image);
            b.Save(save_file);

        }

        private void clear_all()
        {
            panel_container.Controls.Clear();
            pictureBox1.Image = null;
            pictureBox2.Image = null;

            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
        }

        private void delay_timer(int ms)
        {
            Thread.Sleep(ms);
        }
        private void performAllSteps(){

            panel_container.Controls.Clear();
            label2.Text = "Identifying the skin color...";
            Application.DoEvents();
          //  delay_timer(1000);
           // button_skincolor.PerformClick();
            skin_color_segmentation();
            label2.Text = "Skin color identified.";
            label3.Text = "Identifying high impact area...";

            Application.DoEvents();
            //delay_timer(1000);
            //button_connected.PerformClick();

            connected_area();
            label2.Text = "High impact area.";
            label3.Text = "Identified the face.";

            Application.DoEvents();
            //button2.PerformClick();
            detect_face();

            Application.DoEvents();

            
            if (panel_container.Controls.Find("button9",true).Length > 0)
            {
             //Binary Image   
                ((Button)panel_container.Controls.Find("button9", true)[0]).PerformClick();
                Application.DoEvents();
                //Face detection
                if (panel_container.Controls.Find("button10", true).Length > 0)
                {

                    ((Button)panel_container.Controls.Find("button10", true)[0]).PerformClick();
                    Application.DoEvents();
                }
                //Seperate Eye & Lips
                if (panel_container.Controls.Find("button11", true).Length > 0)
                {

                    ((Button)panel_container.Controls.Find("button11", true)[0]).PerformClick();
                    Application.DoEvents();
                }

                //Left Eye
                if (panel_container.Controls.Find("button31", true).Length > 0)
                {

                    ((Button)panel_container.Controls.Find("button31", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button31", true)[0]).PerformClick();
                    
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button31", true)[0]).PerformClick();
                    
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button31", true)[0]).PerformClick();
                    Application.DoEvents();
                }
                //Right Eye
                if (panel_container.Controls.Find("button32", true).Length > 0)
                {

                    ((Button)panel_container.Controls.Find("button32", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button32", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button32", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button32", true)[0]).PerformClick();
                    Application.DoEvents();
                }
                //Seperate Eye & Lips
                if (panel_container.Controls.Find("button33", true).Length > 0)
                {

                    ((Button)panel_container.Controls.Find("button33", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button33", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button33", true)[0]).PerformClick();
                    Application.DoEvents();
                    ((Button)panel_container.Controls.Find("button33", true)[0]).PerformClick();
                    Application.DoEvents();
                }
                ((Label)panel_container.Controls.Find("label1", true)[0]).Text = "Eyes and Lips.";
                Application.DoEvents();
                //Emotion
                if (panel_container.Controls.Find("button21", true).Length > 0)
                {

                    ((Button)panel_container.Controls.Find("button21", true)[0]).PerformClick();
                    Application.DoEvents();
                }
                
            }



        }

        private void panel_container_Paint(object sender, PaintEventArgs e)
        {

        }

        

        

    }
}