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
		private Project project;

		private static Font titleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		private static Font regularFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

		public TaskTableControl(Project project)
		{
			this.project = project;

			this.Location = new Point(0, 0);
			this.Padding = new Padding(left: 0, top: 0, right: SystemInformation.VerticalScrollBarWidth, bottom: 0); //leave room for vertical scrollbar
			this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
			this.Dock = DockStyle.Fill;
			this.BackColor = SystemColors.Control;
			this.AutoScroll = true;

			InsertTitleRow();
			InsertBlankRow();

			int row = 3;
			foreach(Task task in project.GetActiveTasks())
			{
				InsertTaskRow(row, task);
				row++;
			}
		}

		public void InsertTitleRow()
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

		private void InsertBlankRow()
		{
			this.Controls.Add(NewButton("+", addTask_Click), 0, 1);
			this.RowCount++;
		}

		private void InsertTaskRow(int rowIndex)
		{
			this.SuspendLayout(); //avoid screen flickers

			InsertRow(rowIndex);
			this.Controls.Add(NewButton("+", addTask_Click), 0, rowIndex);
			this.Controls.Add(NewDataLabel("Row", (rowIndex - 1).ToString()), 1, rowIndex);
			this.Controls.Add(NewDataLabel("Id", "0"), 2, rowIndex);
			this.Controls.Add(NewTextBox("TitleTextBox"), 3, rowIndex);
			this.Controls.Add(NewComboBox("StatusComboBox", project.Statuses), 4, rowIndex);
			this.Controls.Add(NewComboBox("CategoryComboBox", project.Categories), 5, rowIndex);
			this.Controls.Add(NewDataLabel("CreateDate", "01/01/2017"), 6, rowIndex);
			this.Controls.Add(NewDataLabel("StatusChangeDate", "12/01/2017"), 7, rowIndex);

			this.RowCount++;

			this.ResumeLayout();
		}

		private void InsertTaskRow(int rowIndex, Task task)
		{
			this.SuspendLayout(); //avoid screen flickers

			InsertRow(rowIndex);
			this.Controls.Add(NewButton("+", addTask_Click), 0, rowIndex);
			this.Controls.Add(NewDataLabel("Row", (rowIndex - 1).ToString()), 1, rowIndex);
			this.Controls.Add(NewDataLabel("Id", task.Id.ToString()), 2, rowIndex);
			this.Controls.Add(NewTextBox("TitleTextBox", task.Title), 3, rowIndex);
			this.Controls.Add(NewComboBox("StatusComboBox", project.Statuses, task.Status), 4, rowIndex);
			this.Controls.Add(NewComboBox("CategoryComboBox", project.Categories, task.Category), 5, rowIndex);
			this.Controls.Add(NewDataLabel("CreateDate", task.CreateDateString), 6, rowIndex);
			this.Controls.Add(NewDataLabel("StatusChangeDate", task.StatusChangeDateString), 7, rowIndex);

			this.RowCount++;

			this.ResumeLayout();
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

		private void addTask_Click(object sender, EventArgs e)
		{
			int row = this.GetRow(sender as Control);
			InsertTaskRow(row + 1, project.InsertTask(row + 1));
		}

		private Label NewTitleLabel(string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Font = titleFont;
			label.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			label.Size = new System.Drawing.Size(21, 24);
			label.Text = text;
			return label;
		}

		private Label NewDataLabel(string name, string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Font = regularFont;
			label.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			label.Size = new System.Drawing.Size(21, 24);
			label.Name = name;
			label.Text = text;
			return label;
		}

		private TextBox NewTextBox(string name, string text = null)
		{
			TextBox textBox = new TextBox();
			textBox.Dock = System.Windows.Forms.DockStyle.Top;
			textBox.Font = regularFont;
			textBox.Name = name;
			textBox.Text = text;
			textBox.Size = new System.Drawing.Size(119, 22);
			textBox.TabIndex = 1;
			return textBox;
		}

		private ComboBox NewComboBox(string name, string[] dataSource, string selectedValue = null)
		{
			ComboBox comboBox = new ComboBox();
			comboBox.Font = regularFont;
			comboBox.FormattingEnabled = true;
			comboBox.DataSource = dataSource;
			comboBox.Name = name;
			if(!String.IsNullOrEmpty(selectedValue))
				comboBox.Text = selectedValue;
			comboBox.Size = new System.Drawing.Size(94, 24);
			comboBox.TabIndex = 2;
			return comboBox;
		}

		private Button NewButton(string text, EventHandler onClickHandler)
		{
			Button button = new Button();
			button.Font = regularFont;
			button.Location = new System.Drawing.Point(3, 3);
			button.Size = new System.Drawing.Size(19, 23);
			button.TabIndex = 7;
			button.Text = text;
			button.UseVisualStyleBackColor = true;
			button.Click += onClickHandler;
			return button;
		}
	}
}
