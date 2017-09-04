namespace TaskManagerX
{
	partial class TaskTableToolStrip
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.activeRadioButton = new System.Windows.Forms.RadioButton();
			this.inactiveRadioButton = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(466, 22);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// activeRadioButton
			// 
			this.activeRadioButton.AutoSize = true;
			this.activeRadioButton.Checked = true;
			this.activeRadioButton.Location = new System.Drawing.Point(10, 3);
			this.activeRadioButton.Name = "activeRadioButton";
			this.activeRadioButton.Size = new System.Drawing.Size(85, 17);
			this.activeRadioButton.TabIndex = 1;
			this.activeRadioButton.TabStop = true;
			this.activeRadioButton.Text = "Show Active";
			this.activeRadioButton.UseVisualStyleBackColor = true;
			this.activeRadioButton.CheckedChanged += new System.EventHandler(this.activeRadioButton_CheckedChanged);
			// 
			// inactiveRadioButton
			// 
			this.inactiveRadioButton.AutoSize = true;
			this.inactiveRadioButton.Location = new System.Drawing.Point(101, 2);
			this.inactiveRadioButton.Name = "inactiveRadioButton";
			this.inactiveRadioButton.Size = new System.Drawing.Size(93, 17);
			this.inactiveRadioButton.TabIndex = 2;
			this.inactiveRadioButton.Text = "Show Inactive";
			this.inactiveRadioButton.UseVisualStyleBackColor = true;
			this.inactiveRadioButton.CheckedChanged += new System.EventHandler(this.inactiveRadioButton_CheckedChanged);
			// 
			// TaskTableToolStrip
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.inactiveRadioButton);
			this.Controls.Add(this.activeRadioButton);
			this.Controls.Add(this.toolStrip1);
			this.Name = "TaskTableToolStrip";
			this.Size = new System.Drawing.Size(466, 22);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.RadioButton activeRadioButton;
		private System.Windows.Forms.RadioButton inactiveRadioButton;
	}
}
