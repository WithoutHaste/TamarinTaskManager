using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
{
	public partial class StatusForm : Form
	{
		private System.Windows.Forms.RichTextBox activeTextBox;
		private System.Windows.Forms.Label activeLabel;
		private System.Windows.Forms.Label inactiveLabel;
		private System.Windows.Forms.RichTextBox inactiveTextBox;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.Label helpLabel;
		private System.Windows.Forms.Button importButton;

		public StatusForm(string[] activeStatuses, string[] inactiveStatuses)
		{
			InitializeComponent();

			activeTextBox.Text = String.Join("\n", activeStatuses);
			inactiveTextBox.Text = String.Join("\n", inactiveStatuses);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.activeTextBox = new System.Windows.Forms.RichTextBox();
			this.activeLabel = new System.Windows.Forms.Label();
			this.inactiveLabel = new System.Windows.Forms.Label();
			this.inactiveTextBox = new System.Windows.Forms.RichTextBox();
			this.applyButton = new System.Windows.Forms.Button();
			this.helpLabel = new System.Windows.Forms.Label();
			this.importButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// activeTextBox
			// 
			this.activeTextBox.Location = new System.Drawing.Point(56, 34);
			this.activeTextBox.Name = "activeTextBox";
			this.activeTextBox.Size = new System.Drawing.Size(216, 129);
			this.activeTextBox.TabIndex = 0;
			this.activeTextBox.Text = "";
			// 
			// activeLabel
			// 
			this.activeLabel.AutoSize = true;
			this.activeLabel.Location = new System.Drawing.Point(8, 34);
			this.activeLabel.Name = "activeLabel";
			this.activeLabel.Size = new System.Drawing.Size(37, 13);
			this.activeLabel.TabIndex = 1;
			this.activeLabel.Text = "Active";
			// 
			// inactiveLabel
			// 
			this.inactiveLabel.AutoSize = true;
			this.inactiveLabel.Location = new System.Drawing.Point(5, 170);
			this.inactiveLabel.Name = "inactiveLabel";
			this.inactiveLabel.Size = new System.Drawing.Size(45, 13);
			this.inactiveLabel.TabIndex = 2;
			this.inactiveLabel.Text = "Inactive";
			// 
			// inactiveTextBox
			// 
			this.inactiveTextBox.Location = new System.Drawing.Point(56, 170);
			this.inactiveTextBox.Name = "inactiveTextBox";
			this.inactiveTextBox.Size = new System.Drawing.Size(216, 129);
			this.inactiveTextBox.TabIndex = 3;
			this.inactiveTextBox.Text = "";
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(197, 305);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 4;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// helpLabel
			// 
			this.helpLabel.AutoSize = true;
			this.helpLabel.Location = new System.Drawing.Point(53, 9);
			this.helpLabel.Name = "helpLabel";
			this.helpLabel.Size = new System.Drawing.Size(176, 13);
			this.helpLabel.TabIndex = 5;
			this.helpLabel.Text = "Put an [enter] between each status.";
			// 
			// importButton
			// 
			this.importButton.Location = new System.Drawing.Point(56, 305);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(75, 23);
			this.importButton.TabIndex = 6;
			this.importButton.Text = "Import";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// StatusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 340);
			this.Controls.Add(this.importButton);
			this.Controls.Add(this.helpLabel);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.inactiveTextBox);
			this.Controls.Add(this.inactiveLabel);
			this.Controls.Add(this.activeLabel);
			this.Controls.Add(this.activeTextBox);
			this.Name = "StatusForm";
			this.Text = "Edit Statuses";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public string[] GetActiveStatuses()
		{
			return activeTextBox.Text.Split('\n');
		}

		public string[] GetInactiveStatuses()
		{
			return inactiveTextBox.Text.Split('\n');
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			if(openDialog.ShowDialog() == DialogResult.OK)
			{
				string fullPath = openDialog.FileName;
				if(!File.Exists(fullPath))
				{
					MessageBox.Show(String.Format("File does not exist: {0}", fullPath), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}

				Project fromProject = new Project(fullPath);
				activeTextBox.Text = String.Join("\n", fromProject.ActiveStatuses);
				inactiveTextBox.Text = String.Join("\n", fromProject.InactiveStatuses);
			}
		}
	}
}
