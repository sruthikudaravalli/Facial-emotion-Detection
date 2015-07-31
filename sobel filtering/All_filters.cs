using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace sobel_filtering
{
    class All_filters
    {
        public All_filters()
        { 

        }

        public Bitmap gray(Bitmap Im)
        {
            Bitmap b = (Bitmap)Im;
            for (int i = 1; i < b.Height; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width; j++)  // loop for the image pixels width
                {
                    Color col;
                    col = b.GetPixel(j, i);
                    b.SetPixel(j, i, Color.FromArgb((col.R + col.G + col.B) / 3, (col.R + col.G + col.B) / 3, (col.R + col.G + col.B) / 3));

                }
            }
            return (Bitmap)b;
        }

        public Bitmap mean_filter(Bitmap bp)
        {
            Bitmap bp1;
            bp1 = bp;

            int red, blue, green, count;

            for (int i = 0; i < bp.Width; i++)
            {
                for (int j = 0; j < bp.Height; j++)
                {
                    red=0;
                    blue = 0;
                    green=0 ;
                    count = 0;
                    if (i + 1 < bp.Width && j + 1 < bp.Height)
                    {
                        red += bp.GetPixel(i + 1, j + 1).R;
                        blue += bp.GetPixel(i + 1, j + 1).B;
                        green += bp.GetPixel(i + 1, j + 1).G;
                        count++;
                    }
                    if (i + 1 < bp.Width && j - 1 >=0)
                    {
                        red += bp.GetPixel(i + 1, j - 1).R;
                        blue += bp.GetPixel(i + 1, j - 1).B;
                        green += bp.GetPixel(i + 1, j - 1).G;
                        count++;
                    }
                    if (i - 1 >=0 && j + 1 < bp.Height)
                    {
                        red += bp.GetPixel(i - 1, j + 1).R;
                        blue += bp.GetPixel(i - 1, j + 1).B;
                        green += bp.GetPixel(i - 1, j + 1).G;
                        count++;
                    }
                    if (i - 1 >=0 && j - 1 >=0)
                    {
                        red += bp.GetPixel(i - 1, j - 1).R;
                        blue += bp.GetPixel(i - 1, j - 1).B;
                        green += bp.GetPixel(i - 1, j - 1).G;
                        count++;
                    }


                    if (j + 1 < bp.Height)
                    {
                        red += bp.GetPixel(i , j + 1).R;
                        blue += bp.GetPixel(i , j + 1).B;
                        green += bp.GetPixel(i , j + 1).G;
                        count++;
                    }
                    if (i + 1 < bp.Width)
                    {
                        red += bp.GetPixel(i + 1, j ).R;
                        blue += bp.GetPixel(i + 1, j ).B;
                        green += bp.GetPixel(i + 1, j ).G;
                        count++;
                    }
                    if (i - 1 >= 0 )
                    {
                        red += bp.GetPixel(i - 1, j ).R;
                        blue += bp.GetPixel(i - 1, j ).B;
                        green += bp.GetPixel(i - 1, j ).G;
                        count++;
                    }
                    if (j - 1 >= 0)
                    {
                        red += bp.GetPixel(i , j - 1).R;
                        blue += bp.GetPixel(i , j - 1).B;
                        green += bp.GetPixel(i , j - 1).G;
                        count++;
                    }


                    red += bp.GetPixel(i , j ).R;
                    blue += bp.GetPixel(i , j ).B;
                    green += bp.GetPixel(i , j ).G;
                    count++;

                    red = (int)((float)red / (float)count);
                    green = (int)((float)green / (float)count);
                    blue = (int)((float)blue / (float)count);
                    
                    bp1.SetPixel(i, j, Color.FromArgb(red, green, blue));
                }
            }

            Random rand = new Random();
            bp1.Save(@"c:\image\mean\mean_" + rand.Next().ToString() + ".jpg");
            return bp1;
        }


        public Bitmap median_filter(Bitmap bp)
        {
            Bitmap bp1;
            bp1 = bp;

            Array red = Array.CreateInstance(typeof(Int32),9);
            Array blue = Array.CreateInstance(typeof(Int32), 9);
            Array green = Array.CreateInstance(typeof(Int32), 9);
            
            int count;

        
            

            for (int i = 0; i < bp.Width; i++)
            {

                for (int j = 0; j < bp.Height; j++)
                {
                    count = 0;

                    if (i + 1 < bp.Width && j + 1 < bp.Height)
                    {
                        red.SetValue( bp.GetPixel(i + 1, j + 1).R,count);
                        blue.SetValue( bp.GetPixel(i + 1, j + 1).B,count);
                        green.SetValue(bp.GetPixel(i + 1, j + 1).G,count);
                        count++;
                    }
                    if (i + 1 < bp.Width && j - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i + 1, j - 1).R, count);
                        blue.SetValue(bp.GetPixel(i + 1, j - 1).B, count);
                        green.SetValue(bp.GetPixel(i + 1, j - 1).G, count);
                        count++;
                    }
                    if (i - 1 >= 0 && j + 1 < bp.Height)
                    {
                        red.SetValue(bp.GetPixel(i - 1, j + 1).R, count);
                        blue.SetValue(bp.GetPixel(i - 1, j + 1).B, count);
                        green.SetValue(bp.GetPixel(i - 1, j + 1).G, count);
                        count++;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i - 1, j - 1).R, count);
                        blue.SetValue(bp.GetPixel(i - 1, j - 1).B, count);
                        green.SetValue(bp.GetPixel(i - 1, j - 1).G, count);
                        count++;
                    }


                    if (j + 1 < bp.Height)
                    {
                        red.SetValue(bp.GetPixel(i , j + 1).R, count);
                        blue.SetValue(bp.GetPixel(i , j + 1).B, count);
                        green.SetValue(bp.GetPixel(i , j + 1).G, count);
                        count++;
                    }
                    if (i + 1 < bp.Width)
                    {
                        red.SetValue(bp.GetPixel(i + 1, j ).R, count);
                        blue.SetValue(bp.GetPixel(i + 1, j ).B, count);
                        green.SetValue(bp.GetPixel(i + 1, j ).G, count);
                        count++;
                    }
                    if (i - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i - 1, j ).R, count);
                        blue.SetValue(bp.GetPixel(i - 1, j ).B, count);
                        green.SetValue(bp.GetPixel(i - 1, j ).G, count);
                        count++;
                    }
                    if (j - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i , j - 1).R, count);
                        blue.SetValue(bp.GetPixel(i , j - 1).B, count);
                        green.SetValue(bp.GetPixel(i , j - 1).G, count);
                        count++;
                    }


                    red.SetValue(bp.GetPixel(i , j ).R, count);
                    blue.SetValue(bp.GetPixel(i , j ).B, count);
                    green.SetValue(bp.GetPixel(i , j ).G, count);
                    //count++;

                    Array.Sort(red);
                    Array.Sort(green);
                    Array.Sort(blue);
                                       
                    count=count/2;
                    //count = (count + 0) / 2;
                    int red1 = (int)red.GetValue(count);
                    int green1 = (int)green.GetValue(count);
                    int blue1 = (int)blue.GetValue(count);
                    
                    bp1.SetPixel(i, j, Color.FromArgb(red1, green1, blue1));
                    Array.Clear(red,0,9);
                    Array.Clear(blue, 0, 9);
                    Array.Clear(green, 0, 9);
                }
            }

            Random rand = new Random();
            bp1.Save(@"c:\image\median\median_" + rand.Next().ToString() + ".jpg");
            return bp1;
        }

        public Bitmap bilash_filter(Bitmap bp)
        {

            Bitmap bp_original;
            bp_original = (Bitmap)bp.Clone();

            int count,k;

            int[] red = new int[9];
            int[] green = new int[9];
            int[] blue = new int[9];

            int[] count_red = new int[256];
            int[] count_green = new int[256];
            int[] count_blue = new int[256];
                    
                  
            for (int i = 0; i < bp_original.Width; i++)
            {

                for (int j = 0; j < bp_original.Height; j++)
                {
                    count = 0;

                    if (i + 1 < bp_original.Width && j + 1 < bp_original.Height)
                    {
                        red[count] = bp_original.GetPixel(i + 1, j + 1).R;
                        green[count] = bp_original.GetPixel(i + 1, j + 1).G;
                        blue[count] = bp_original.GetPixel(i + 1, j + 1).B;
                        count++;
                    }
                    if (i + 1 < bp_original.Width && j - 1 >= 0)
                    {
                        red[count] = bp_original.GetPixel(i + 1, j - 1).R;
                        green[count] = bp_original.GetPixel(i + 1, j - 1).G;
                        blue[count] = bp_original.GetPixel(i + 1, j - 1).B;
                        count++;
                    }
                    if (i - 1 >= 0 && j + 1 < bp_original.Height)
                    {
                        red[count] = bp_original.GetPixel(i - 1, j + 1).R;
                        green[count] = bp_original.GetPixel(i - 1, j + 1).G;
                        blue[count] = bp_original.GetPixel(i - 1, j + 1).B;
                        count++;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        red[count] = bp_original.GetPixel(i - 1, j - 1).R;
                        green[count] = bp_original.GetPixel(i - 1, j - 1).G;
                        blue[count] = bp_original.GetPixel(i - 1, j - 1).B;
                        count++;
                    }


                    if (j + 1 < bp_original.Height)
                    {
                        red[count] = bp_original.GetPixel(i , j + 1).R;
                        green[count] = bp_original.GetPixel(i , j + 1).G;
                        blue[count] = bp_original.GetPixel(i , j + 1).B;
                        count++;
                    }
                    if (i + 1 < bp_original.Width)
                    {
                        red[count] = bp_original.GetPixel(i + 1, j ).R;
                        green[count] = bp_original.GetPixel(i + 1, j ).G;
                        blue[count] = bp_original.GetPixel(i + 1, j ).B;
                        count++;
                    }
                    if (i - 1 >= 0)
                    {
                        red[count] = bp_original.GetPixel(i - 1, j ).R;
                        green[count] = bp_original.GetPixel(i - 1, j ).G;
                        blue[count] = bp_original.GetPixel(i - 1, j ).B;
                        count++;
                    }
                    if (j - 1 >= 0)
                    {
                        red[count] = bp_original.GetPixel(i , j - 1).R;
                        green[count] = bp_original.GetPixel(i , j - 1).G;
                        blue[count] = bp_original.GetPixel(i , j - 1).B;
                        count++;
                    }


                    red[count] = bp_original.GetPixel(i , j ).R;
                    green[count] = bp_original.GetPixel(i , j ).G;
                    blue[count] = bp_original.GetPixel(i , j ).B;
                    count++;

                    int max_red = 0,max_blue=0,max_green=0;
                    
                    
                    int max_red1 = 0,max_blue1=0,max_green1=0;
                    int flag_red = 0,flag_blue=0,flag_green=0;
                    
                    int min_red = 256,min_blue=256,min_green=256;

                    for (k = 0; k < count; k++)
                    {
                        count_red[red[k]]++;
                        
                        if (count_red[red[k]] > 1)
                            flag_red = 1;

                        count_green[green[k]]++;

                        if (count_green[green[k]] > 1)
                            flag_green = 1;

                        count_blue[blue[k]]++;

                        if (count_blue[blue[k]] > 1)
                            flag_blue = 1;

                                                
                    }

                    int index_r, index_g, index_b;

                    index_r=0;
                    index_g=0;
                    index_b=0;

                    if ((flag_blue + flag_green + flag_red) != 0)
                    {
                        for (k = 0; k < count; k++)
                        {
                            if (max_red < count_red[red[k]])
                            {
                                max_red = count_red[red[k]];
                                max_red1 = red[k];
                                index_r = k;
                                
                            }

                            if (max_green < count_green[green[k]])
                            {
                                max_green = count_green[green[k]];
                                max_green1 = green[k];
                                index_g = k;

                            }
                            if (max_blue < count_blue[blue[k]])
                            {
                                max_blue = count_blue[blue[k]];
                                max_blue1 = blue[k];
                                index_b = k;

                            }

                    
                        }
                    }


                    int max = max_red;

                    if (max < max_green)
                        max = max_green;
                    if (max < max_blue)
                        max = max_blue;

                    
                   
                    if (flag_red == 0 && flag_green == 0 && flag_blue == 0)
                    {
                        max_red1 = bp.GetPixel(i, j).R;
                        max_green1 = bp.GetPixel(i, j).G;
                        max_blue1 = bp.GetPixel(i, j).B;
                    }

                    else
                    {
                        if (max == max_blue)
                        {
                            max_blue1 = blue[index_b];
                            max_green1 = green[index_b];
                            max_red1 = red[index_b];
                        }
                        else if (max == max_red)
                        {
                            max_blue1 = blue[index_r];
                            max_green1 = green[index_r];
                            max_red1 = red[index_r];
                        }
                        else if (max == max_green1)
                        {
                            max_blue1 = blue[index_g];
                            max_green1 = green[index_g];
                            max_red1 = red[index_g];
                        }
                    }
                        
                    bp.SetPixel(i, j, Color.FromArgb(max_red1, max_green1, max_blue1));

                            Array.Clear(count_blue, 0, 256);
                            Array.Clear(count_green, 0, 256);
                            Array.Clear(count_red, 0, 256);
                }
            }

            Random rand = new Random();
            bp.Save(@"c:\image\bilash\bilash_" + rand.Next().ToString() + ".jpg");
            return bp;
        
        }

        public Bitmap bilash_filter2(Bitmap bp) 
        {
            Bitmap bp1;
            bp1 = bp;

            Array red = Array.CreateInstance(typeof(Int32), 9);
            Array blue = Array.CreateInstance(typeof(Int32), 9);
            Array green = Array.CreateInstance(typeof(Int32), 9);

            int count;




            for (int i = 0; i < bp.Width; i++)
            {

                for (int j = 0; j < bp.Height; j++)
                {
                    count = 0;

                    if (i + 1 < bp.Width && j + 1 < bp.Height)
                    {
                        red.SetValue(bp.GetPixel(i + 1, j + 1).R, count);
                        blue.SetValue(bp.GetPixel(i + 1, j + 1).B, count);
                        green.SetValue(bp.GetPixel(i + 1, j + 1).G, count);
                        count++;
                    }
                    if (i + 1 < bp.Width && j - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i + 1, j - 1).R, count);
                        blue.SetValue(bp.GetPixel(i + 1, j - 1).B, count);
                        green.SetValue(bp.GetPixel(i + 1, j - 1).G, count);
                        count++;
                    }
                    if (i - 1 >= 0 && j + 1 < bp.Height)
                    {
                        red.SetValue(bp.GetPixel(i - 1, j + 1).R, count);
                        blue.SetValue(bp.GetPixel(i - 1, j + 1).B, count);
                        green.SetValue(bp.GetPixel(i - 1, j + 1).G, count);
                        count++;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i - 1, j - 1).R, count);
                        blue.SetValue(bp.GetPixel(i - 1, j - 1).B, count);
                        green.SetValue(bp.GetPixel(i - 1, j - 1).G, count);
                        count++;
                    }


                    if (j + 1 < bp.Height)
                    {
                        red.SetValue(bp.GetPixel(i, j + 1).R, count);
                        blue.SetValue(bp.GetPixel(i, j + 1).B, count);
                        green.SetValue(bp.GetPixel(i, j + 1).G, count);
                        count++;
                    }
                    if (i + 1 < bp.Width)
                    {
                        red.SetValue(bp.GetPixel(i + 1, j).R, count);
                        blue.SetValue(bp.GetPixel(i + 1, j).B, count);
                        green.SetValue(bp.GetPixel(i + 1, j).G, count);
                        count++;
                    }
                    if (i - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i - 1, j).R, count);
                        blue.SetValue(bp.GetPixel(i - 1, j).B, count);
                        green.SetValue(bp.GetPixel(i - 1, j).G, count);
                        count++;
                    }
                    if (j - 1 >= 0)
                    {
                        red.SetValue(bp.GetPixel(i, j - 1).R, count);
                        blue.SetValue(bp.GetPixel(i, j - 1).B, count);
                        green.SetValue(bp.GetPixel(i, j - 1).G, count);
                        count++;
                    }


                    red.SetValue(bp.GetPixel(i, j).R, count);
                    blue.SetValue(bp.GetPixel(i, j).B, count);
                    green.SetValue(bp.GetPixel(i, j).G, count);
                    //count++;

                    Array.Sort(red);
                    Array.Sort(green);
                    Array.Sort(blue);

                    count = count / 2;
                    //count = (count + 0) / 2;
                    int red1 = (int)red.GetValue(count) + (int)red.GetValue(count + 1) + (int)red.GetValue(count - 1);
                    red1 /= 3;
                    int green1 = (int)green.GetValue(count) + (int)green.GetValue(count + 1) + (int)green.GetValue(count - 1);
                    green1 /= 3;
                    int blue1 = (int)blue.GetValue(count) + (int)blue.GetValue(count + 1) + (int)blue.GetValue(count - 1);
                    blue1 /= 3;

                    bp1.SetPixel(i, j, Color.FromArgb(red1, green1, blue1));
                    Array.Clear(red, 0, 9);
                    Array.Clear(blue, 0, 9);
                    Array.Clear(green, 0, 9);
                }
            }

            Random rand = new Random();
            bp1.Save(@"c:\image\bilash 2\billi_" + rand.Next().ToString() + ".jpg");
            return bp1;
        
        }

    }
}

