using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CSharpFilters
{
	/// <summary>
	/// Summary description for Parameter.
	/// </summary>
	public class Parameter : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.TextBox Value;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public int nValue
		{
			get 
			{
				return (Convert.ToInt32(Value.Text, 10));
			}
			set{Value.Text = value.ToString();}
		}

		public Parameter()
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.Value = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// OK
			// 
			this.OK.Location = new System.Drawing.Point(16, 56);
			this.OK.Name = "OK";
			this.OK.TabIndex = 0;
			this.OK.Text = "OK";
			// 
			// Cancel
			// 
			this.Cancel.Location = new System.Drawing.Point(104, 56);
			this.Cancel.Name = "Cancel";
			this.Cancel.TabIndex = 1;
			this.Cancel.Text = "Cancel";
			// 
			// Value
			// 
			this.Value.Location = new System.Drawing.Point(80, 16);
			this.Value.Name = "Value";
			this.Value.TabIndex = 2;
			this.Value.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Value";
			// 
			// Parameter
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(200, 85);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label1,
																		  this.Value,
																		  this.Cancel,
																		  this.OK});
			this.Name = "Parameter";
			this.Text = "Parameter";
			this.Load += new System.EventHandler(this.Parameter_Load);
			this.CenterToParent();
			this.ResumeLayout(false);

		}
		#endregion

		private void Parameter_Load(object sender, System.EventArgs e)
		{
		
		}
	}
}
