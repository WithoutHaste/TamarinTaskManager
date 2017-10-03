namespace TaskManagerX
{
	partial class SettingsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.showTaskIdsCheckBox = new System.Windows.Forms.CheckBox();
			this.applyButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// showTaskIdsCheckBox
			// 
			this.showTaskIdsCheckBox.AutoSize = true;
			this.showTaskIdsCheckBox.Location = new System.Drawing.Point(13, 13);
			this.showTaskIdsCheckBox.Name = "showTaskIdsCheckBox";
			this.showTaskIdsCheckBox.Size = new System.Drawing.Size(97, 17);
			this.showTaskIdsCheckBox.TabIndex = 0;
			this.showTaskIdsCheckBox.Text = "Show Task Ids";
			this.showTaskIdsCheckBox.UseVisualStyleBackColor = true;
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(91, 46);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 1;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 76);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.showTaskIdsCheckBox);
			this.Name = "SettingsForm";
			this.Text = "Settings";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox showTaskIdsCheckBox;
		private System.Windows.Forms.Button applyButton;
	}
}