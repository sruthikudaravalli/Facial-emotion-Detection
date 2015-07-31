using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CSharpFilters
{
	/// <summary>
	/// Summary description for Convolution.
	/// </summary>
	public class Convolution : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button Cancel;

		private ConvMatrix matrix = new ConvMatrix();
		private System.Windows.Forms.TextBox TL;
		private System.Windows.Forms.TextBox TR;
		private System.Windows.Forms.TextBox TM;
		private System.Windows.Forms.TextBox Pixel;
		private System.Windows.Forms.TextBox MR;
		private System.Windows.Forms.TextBox ML;
		private System.Windows.Forms.TextBox BM;
		private System.Windows.Forms.TextBox BR;
		private System.Windows.Forms.TextBox BL;
		private System.Windows.Forms.TextBox Factor;
		private System.Windows.Forms.TextBox Offset;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Convolution()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public ConvMatrix Matrix
		{
			get 
			{
				ConvMatrix mat = new ConvMatrix();
				mat.TopLeft = Convert.ToInt32(TL.Text);
				mat.TopMid = Convert.ToInt32(TM.Text);
				mat.TopRight = Convert.ToInt32(TR.Text);
				mat.MidLeft = Convert.ToInt32(ML.Text);
				mat.MidRight = Convert.ToInt32(MR.Text);
				mat.BottomLeft = Convert.ToInt32(BL.Text);
				mat.BottomMid = Convert.ToInt32(BM.Text);
				mat.BottomRight = Convert.ToInt32(BR.Text);
				mat.Pixel = Convert.ToInt32(Pixel.Text);
				mat.Factor = Convert.ToInt32(Factor.Text);
				mat.Offset = Convert.ToInt32(Offset.Text);
				return mat;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TL = new System.Windows.Forms.TextBox();
			this.TR = new System.Windows.Forms.TextBox();
			this.TM = new System.Windows.Forms.TextBox();
			this.Pixel = new System.Windows.Forms.TextBox();
			this.MR = new System.Windows.Forms.TextBox();
			this.ML = new System.Windows.Forms.TextBox();
			this.BM = new System.Windows.Forms.TextBox();
			this.BR = new System.Windows.Forms.TextBox();
			this.BL = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.Factor = new System.Windows.Forms.TextBox();
			this.Offset = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.OK = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TL
			// 
			this.TL.Location = new System.Drawing.Point(24, 24);
			this.TL.Name = "TL";
			this.TL.Size = new System.Drawing.Size(24, 20);
			this.TL.TabIndex = 0;
			this.TL.Text = "0";
			// 
			// TR
			// 
			this.TR.Location = new System.Drawing.Point(104, 24);
			this.TR.Name = "TR";
			this.TR.Size = new System.Drawing.Size(24, 20);
			this.TR.TabIndex = 1;
			this.TR.Text = "0";
			// 
			// TM
			// 
			this.TM.Location = new System.Drawing.Point(64, 24);
			this.TM.Name = "TM";
			this.TM.Size = new System.Drawing.Size(24, 20);
			this.TM.TabIndex = 2;
			this.TM.Text = "0";
			// 
			// Pixel
			// 
			this.Pixel.Location = new System.Drawing.Point(64, 56);
			this.Pixel.Name = "Pixel";
			this.Pixel.Size = new System.Drawing.Size(24, 20);
			this.Pixel.TabIndex = 5;
			this.Pixel.Text = "1";
			// 
			// MR
			// 
			this.MR.Location = new System.Drawing.Point(104, 56);
			this.MR.Name = "MR";
			this.MR.Size = new System.Drawing.Size(24, 20);
			this.MR.TabIndex = 4;
			this.MR.Text = "0";
			// 
			// ML
			// 
			this.ML.Location = new System.Drawing.Point(24, 56);
			this.ML.Name = "ML";
			this.ML.Size = new System.Drawing.Size(24, 20);
			this.ML.TabIndex = 3;
			this.ML.Text = "0";
			// 
			// BM
			// 
			this.BM.Location = new System.Drawing.Point(64, 88);
			this.BM.Name = "BM";
			this.BM.Size = new System.Drawing.Size(24, 20);
			this.BM.TabIndex = 8;
			this.BM.Text = "0";
			// 
			// BR
			// 
			this.BR.Location = new System.Drawing.Point(104, 88);
			this.BR.Name = "BR";
			this.BR.Size = new System.Drawing.Size(24, 20);
			this.BR.TabIndex = 7;
			this.BR.Text = "0";
			// 
			// BL
			// 
			this.BL.Location = new System.Drawing.Point(24, 88);
			this.BL.Name = "BL";
			this.BL.Size = new System.Drawing.Size(24, 20);
			this.BL.TabIndex = 6;
			this.BL.Text = "0";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(144, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(8, 23);
			this.label1.TabIndex = 9;
			this.label1.Text = "/";
			// 
			// Factor
			// 
			this.Factor.Location = new System.Drawing.Point(168, 88);
			this.Factor.Name = "Factor";
			this.Factor.Size = new System.Drawing.Size(24, 20);
			this.Factor.TabIndex = 10;
			this.Factor.Text = "1";
			// 
			// Offset
			// 
			this.Offset.Location = new System.Drawing.Point(232, 88);
			this.Offset.Name = "Offset";
			this.Offset.Size = new System.Drawing.Size(24, 20);
			this.Offset.TabIndex = 11;
			this.Offset.Text = "0";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(208, 96);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(8, 23);
			this.label2.TabIndex = 12;
			this.label2.Text = "+";
			// 
			// OK
			// 
			this.OK.Location = new System.Drawing.Point(48, 144);
			this.OK.Name = "OK";
			this.OK.TabIndex = 13;
			this.OK.Text = "OK";
			// 
			// Cancel
			// 
			this.Cancel.Location = new System.Drawing.Point(168, 144);
			this.Cancel.Name = "Cancel";
			this.Cancel.TabIndex = 14;
			this.Cancel.Text = "Cancel";
			// 
			// Convolution
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(280, 181);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.Cancel,
																		  this.OK,
																		  this.label2,
																		  this.Offset,
																		  this.Factor,
																		  this.label1,
																		  this.BM,
																		  this.BR,
																		  this.BL,
																		  this.Pixel,
																		  this.MR,
																		  this.ML,
																		  this.TM,
																		  this.TR,
																		  this.TL});
			this.Name = "Convolution";
			this.Text = "Convolution";
			this.CenterToParent();
			this.ResumeLayout(false);

		}
		#endregion

	}
}
