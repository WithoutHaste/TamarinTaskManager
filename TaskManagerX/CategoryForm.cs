using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManagerX
{
	public partial class CategoryForm : Form
	{
		public CategoryForm(string[] categories)
		{
			InitializeComponent();

			categoriesTextBox.Text = String.Join("\n", categories);
		}

		public string[] GetCategories()
		{
			return categoriesTextBox.Text.Split('\n');
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}
	}
}
