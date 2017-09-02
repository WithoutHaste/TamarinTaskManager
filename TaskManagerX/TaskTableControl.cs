using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TaskManagerX
{
	public class TaskTableControl : TableLayoutPanel
	{
		public TaskTableControl()
		{
			this.Location = new Point(0, 0);
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.Dock = DockStyle.Fill;
			this.BackColor = SystemColors.Control;
			this.AutoScroll = true;

			InitTitleRow();
			InsertTaskRow(1);
			InsertTaskRow(1);
			InsertTaskRow(1);
			InsertTaskRow(3);
			InsertTaskRow(3);
			InsertTaskRow(3);
			InsertTaskRow(3);
			InsertTaskRow(3);
		}

		public void InitTitleRow()
		{
			this.Controls.Add(NewTitleLabel("Row"), 1, 0);
			this.Controls.Add(NewTitleLabel("Id"), 2, 0);
			this.Controls.Add(NewTitleLabel("Title"), 3, 0);
			this.Controls.Add(NewTitleLabel("Status"), 4, 0);
			this.Controls.Add(NewTitleLabel("Category"), 5, 0);
			this.Controls.Add(NewTitleLabel("Created"), 6, 0);
			this.Controls.Add(NewTitleLabel("StatChg"), 7, 0);

			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));

			this.ColumnCount = 8;
			this.RowCount = 1;
		}

		private void InsertTaskRow(int rowIndex)
		{
			InsertRow(rowIndex);
			this.Controls.Add(NewDataLabel("Row", rowIndex.ToString()), 1, rowIndex);
			this.Controls.Add(NewDataLabel("Id", "0"), 2, rowIndex);
			this.Controls.Add(NewTextBox("TitleTextBox"), 3, rowIndex);
			this.Controls.Add(NewComboBox("StatusComboBox", new string[] { "Todo", "Done" }), 4, rowIndex);
			this.Controls.Add(NewComboBox("CategoryComboBox", new string[] { "Task", "Bug" }), 5, rowIndex);
			this.Controls.Add(NewDataLabel("CreateDate", "01/01/2017"), 6, rowIndex);
			this.Controls.Add(NewDataLabel("StatusChangeDate", "12/01/2017"), 7, rowIndex);

			this.RowCount++;
		}

		private void InsertRow(int rowIndex)
		{
			foreach(Control control in this.Controls)
			{
				int controlRow = this.GetRow(control);
				if(controlRow < rowIndex)
					continue;

				this.SetRow(control, controlRow + 1);

				if(control.Name == "Row")
				{
					(control as Label).Text = (Int32.Parse((control as Label).Text) + 1).ToString();
				}
			}
		}

		private Label NewTitleLabel(string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			label.Size = new System.Drawing.Size(21, 24);
			label.Text = text;
			return label;
		}

		private Label NewDataLabel(string name, string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			label.Size = new System.Drawing.Size(21, 24);
			label.Name = name;
			label.Text = text;
			return label;
		}

		private TextBox NewTextBox(string name)
		{
			TextBox textBox = new TextBox();
			textBox.Dock = System.Windows.Forms.DockStyle.Top;
			textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			textBox.Name = name;
			textBox.Size = new System.Drawing.Size(119, 22);
			textBox.TabIndex = 1;
			return textBox;
		}

		private ComboBox NewComboBox(string name, string[] dataSource)
		{
			ComboBox comboBox = new ComboBox();
			comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			comboBox.FormattingEnabled = true;
			comboBox.DataSource = dataSource;
			comboBox.Name = name;
			comboBox.Size = new System.Drawing.Size(94, 24);
			comboBox.TabIndex = 2;
			return comboBox;
		}
	}
}
