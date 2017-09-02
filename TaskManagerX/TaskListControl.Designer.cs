namespace TaskManagerX
{
	partial class TaskListControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.titlePanel = new System.Windows.Forms.Panel();
			this.scrollPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// titlePanel
			// 
			this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.titlePanel.Location = new System.Drawing.Point(0, 0);
			this.titlePanel.Name = "titlePanel";
			this.titlePanel.Size = new System.Drawing.Size(100, 35);
			this.titlePanel.TabIndex = 0;
			// 
			// scrollPanel
			// 
			this.scrollPanel.AutoSize = true;
			this.scrollPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.scrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scrollPanel.Location = new System.Drawing.Point(0, 35);
			this.scrollPanel.Name = "scrollPanel";
			this.scrollPanel.Size = new System.Drawing.Size(100, 65);
			this.scrollPanel.TabIndex = 1;
			// 
			// TaskListControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.scrollPanel);
			this.Controls.Add(this.titlePanel);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "TaskListControl";
			this.Size = new System.Drawing.Size(100, 100);
			this.Load += new System.EventHandler(this.TaskListControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel titlePanel;
		private System.Windows.Forms.Panel scrollPanel;
	}
}
