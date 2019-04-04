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
	public class CategoryForm : Form
	{
		private System.Windows.Forms.RichTextBox categoriesTextBox;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.Label helpLabel;
		private System.Windows.Forms.Button importButton;

		public CategoryForm(string[] categories)
		{
			InitializeComponent();

			categoriesTextBox.Text = String.Join("\n", categories);
		}

		private void InitializeComponent()
		{
			this.categoriesTextBox = new System.Windows.Forms.RichTextBox();
			this.applyButton = new System.Windows.Forms.Button();
			this.helpLabel = new System.Windows.Forms.Label();
			this.importButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// categoriesTextBox
			// 
			this.categoriesTextBox.Location = new System.Drawing.Point(13, 35);
			this.categoriesTextBox.Name = "categoriesTextBox";
			this.categoriesTextBox.Size = new System.Drawing.Size(259, 207);
			this.categoriesTextBox.TabIndex = 0;
			this.categoriesTextBox.Text = "";
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(197, 248);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 1;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// helpLabel
			// 
			this.helpLabel.AutoSize = true;
			this.helpLabel.Location = new System.Drawing.Point(12, 9);
			this.helpLabel.Name = "helpLabel";
			this.helpLabel.Size = new System.Drawing.Size(189, 13);
			this.helpLabel.TabIndex = 2;
			this.helpLabel.Text = "Put an [enter] between each category.";
			// 
			// importButton
			// 
			this.importButton.Location = new System.Drawing.Point(13, 248);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(75, 23);
			this.importButton.TabIndex = 3;
			this.importButton.Text = "Import";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// CategoryForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 283);
			this.Controls.Add(this.importButton);
			this.Controls.Add(this.helpLabel);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.categoriesTextBox);
			this.Name = "CategoryForm";
			this.Text = "Edit Categories";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		public string[] GetCategories()
		{
			return categoriesTextBox.Text.Split('\n');
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
				categoriesTextBox.Text = String.Join("\n", fromProject.Categories);
			}
		}
	}
}
