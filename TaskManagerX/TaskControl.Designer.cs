namespace TaskManagerX
{
	partial class TaskControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.idLabel = new System.Windows.Forms.Label();
			this.titleTextBox = new System.Windows.Forms.TextBox();
			this.statusComboBox = new System.Windows.Forms.ComboBox();
			this.categoryComboBox = new System.Windows.Forms.ComboBox();
			this.createDateLabel = new System.Windows.Forms.Label();
			this.statusDateLabel = new System.Windows.Forms.Label();
			this.rowLabel = new System.Windows.Forms.Label();
			this.addButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 8;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tableLayoutPanel1.Controls.Add(this.idLabel, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.titleTextBox, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.statusComboBox, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.categoryComboBox, 5, 0);
			this.tableLayoutPanel1.Controls.Add(this.createDateLabel, 6, 0);
			this.tableLayoutPanel1.Controls.Add(this.statusDateLabel, 7, 0);
			this.tableLayoutPanel1.Controls.Add(this.rowLabel, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.addButton, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 35);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// idLabel
			// 
			this.idLabel.AutoSize = true;
			this.idLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.idLabel.Location = new System.Drawing.Point(73, 0);
			this.idLabel.Name = "idLabel";
			this.idLabel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.idLabel.Size = new System.Drawing.Size(36, 24);
			this.idLabel.TabIndex = 0;
			this.idLabel.Text = "9999";
			// 
			// titleTextBox
			// 
			this.titleTextBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.titleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleTextBox.Location = new System.Drawing.Point(118, 3);
			this.titleTextBox.Name = "titleTextBox";
			this.titleTextBox.Size = new System.Drawing.Size(119, 22);
			this.titleTextBox.TabIndex = 1;
			// 
			// statusComboBox
			// 
			this.statusComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusComboBox.FormattingEnabled = true;
			this.statusComboBox.Location = new System.Drawing.Point(243, 3);
			this.statusComboBox.Name = "statusComboBox";
			this.statusComboBox.Size = new System.Drawing.Size(94, 24);
			this.statusComboBox.TabIndex = 2;
			// 
			// categoryComboBox
			// 
			this.categoryComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.categoryComboBox.FormattingEnabled = true;
			this.categoryComboBox.Location = new System.Drawing.Point(343, 3);
			this.categoryComboBox.Name = "categoryComboBox";
			this.categoryComboBox.Size = new System.Drawing.Size(94, 24);
			this.categoryComboBox.TabIndex = 3;
			// 
			// createDateLabel
			// 
			this.createDateLabel.AutoSize = true;
			this.createDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.createDateLabel.Location = new System.Drawing.Point(443, 0);
			this.createDateLabel.Name = "createDateLabel";
			this.createDateLabel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.createDateLabel.Size = new System.Drawing.Size(72, 24);
			this.createDateLabel.TabIndex = 4;
			this.createDateLabel.Text = "01/01/2017";
			// 
			// statusDateLabel
			// 
			this.statusDateLabel.AutoSize = true;
			this.statusDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusDateLabel.Location = new System.Drawing.Point(523, 0);
			this.statusDateLabel.Name = "statusDateLabel";
			this.statusDateLabel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.statusDateLabel.Size = new System.Drawing.Size(72, 24);
			this.statusDateLabel.TabIndex = 5;
			this.statusDateLabel.Text = "12/01/2017";
			// 
			// rowLabel
			// 
			this.rowLabel.AutoSize = true;
			this.rowLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rowLabel.Location = new System.Drawing.Point(28, 0);
			this.rowLabel.Name = "rowLabel";
			this.rowLabel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.rowLabel.Size = new System.Drawing.Size(36, 24);
			this.rowLabel.TabIndex = 6;
			this.rowLabel.Text = "9999";
			// 
			// addButton
			// 
			this.addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.addButton.Location = new System.Drawing.Point(3, 3);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(19, 23);
			this.addButton.TabIndex = 7;
			this.addButton.Text = "+";
			this.addButton.UseVisualStyleBackColor = true;
			// 
			// TaskControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "TaskControl";
			this.Size = new System.Drawing.Size(600, 35);
			this.Load += new System.EventHandler(this.TaskControl_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label idLabel;
		private System.Windows.Forms.TextBox titleTextBox;
		private System.Windows.Forms.ComboBox statusComboBox;
		private System.Windows.Forms.ComboBox categoryComboBox;
		private System.Windows.Forms.Label createDateLabel;
		private System.Windows.Forms.Label statusDateLabel;
		private System.Windows.Forms.Label rowLabel;
		private System.Windows.Forms.Button addButton;
	}
}
