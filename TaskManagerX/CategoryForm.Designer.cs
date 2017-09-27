namespace TaskManagerX
{
	partial class CategoryForm
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
			this.categoriesTextBox = new System.Windows.Forms.RichTextBox();
			this.applyButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// categoriesTextBox
			// 
			this.categoriesTextBox.Location = new System.Drawing.Point(13, 13);
			this.categoriesTextBox.Name = "categoriesTextBox";
			this.categoriesTextBox.Size = new System.Drawing.Size(259, 207);
			this.categoriesTextBox.TabIndex = 0;
			this.categoriesTextBox.Text = "";
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(103, 226);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 1;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// CategoryForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.categoriesTextBox);
			this.Name = "CategoryForm";
			this.Text = "Edit Categories";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox categoriesTextBox;
		private System.Windows.Forms.Button applyButton;
	}
}