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
		//TableLayoutPanel has 0-based row index

		private Project project;

		private bool showActive = true;

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

			ShowTaskSheet(active: showActive);
		}

		public void ShowTaskSheet(bool active)
		{
			showActive = active;

			this.Controls.Clear();
			this.ColumnStyles.Clear();

			InsertTitleRow();

			int row = 1;
			foreach(Task task in project.GetTasks(active: active))
			{
				InsertTaskRowAt(row, task);
				row++;
			}
		}

		public void InsertTitleRow()
		{
			this.Controls.Add(NewButton("+", addTask_Click), 0, 0);
			this.Controls.Add(NewTitleLabel("Row"), 1, 0);
			this.Controls.Add(NewTitleLabel("Id"), 2, 0);
			this.Controls.Add(NewTitleLabel("Title"), 3, 0);
			this.Controls.Add(NewTitleLabel("Status"), 4, 0);
			this.Controls.Add(NewTitleLabel("Category"), 5, 0);
			this.Controls.Add(NewTitleLabel("Created"), 6, 0);
			this.Controls.Add(NewTitleLabel("Done"), 7, 0);

			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (showActive ? 0F : 80F)));

			this.ColumnCount = 8;
			this.RowCount = 1;
		}

		private void InsertTaskRowAt(int rowIndex, Task task)
		{
			this.SuspendLayout(); //avoid screen flickers

			InsertRowAt(rowIndex);

			this.Controls.Add(NewButton("+", addTask_Click), 0, rowIndex);

			Label rowLabel = NewDataLabel("Row", rowIndex.ToString());
			//rowLabel.Cursor = Cursors.Hand;
			//rowLabel.MouseDown += new MouseEventHandler(rowLabel_MouseDown);
			//rowLabel.AllowDrop = true;
			//rowLabel.DragEnter += new DragEventHandler(rowLabel_DragEnter);
			//rowLabel.DragDrop += new DragEventHandler(rowLabel_DragDrop);
			this.Controls.Add(rowLabel, 1, rowIndex);

			this.Controls.Add(NewDataLabel("Id", task.Id.ToString()), 2, rowIndex);

			TextBox titleBox = NewTextBox("TitleTextBox", task.Title);
			titleBox.TextChanged += new EventHandler(titleTextBox_TextChanged);
			this.Controls.Add(titleBox, 3, rowIndex);

			ComboBox statusComboBox = NewComboBox("StatusComboBox", project.Statuses, task.Status);
			statusComboBox.SelectedIndexChanged += new EventHandler(statusComboBox_SelectedIndexChanged);
			this.Controls.Add(statusComboBox, 4, rowIndex);

			ComboBox categoryComboBox = NewComboBox("CategoryComboBox", project.Categories, task.Category);
			categoryComboBox.SelectedIndexChanged += new EventHandler(categoryComboBox_SelectedIndexChanged);
			this.Controls.Add(categoryComboBox, 5, rowIndex);

			this.Controls.Add(NewDataLabel("CreateDate", task.CreateDateString), 6, rowIndex);
			this.Controls.Add(NewDataLabel("DoneDate", task.DoneDateString), 7, rowIndex);

			this.RowCount++;

			this.ResumeLayout();
		}

		private void InsertRowAt(int rowIndex)
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

		private void RemoveRow(int rowIndex)
		{
			for(int col = 0; col <= 7; col++)
			{
				Control control = this.GetControlFromPosition(col, rowIndex);
				this.Controls.Remove(control);
			}
			foreach(Control control in this.Controls)
			{
				int controlRow = this.GetRow(control);
				if(controlRow < rowIndex)
					continue;

				this.SetRow(control, controlRow - 1);

				if(control.Name == "Row")
				{
					(control as Label).Text = (Int32.Parse((control as Label).Text) - 1).ToString();
				}
			}
		}

		//private void MoveRow(int fromRow, int toRow)
		//{
		//	//push down toRow, so insert above it
		//	if(fromRow == toRow)
		//		return;
		//	Task task = project.MoveRow(fromRow + 1, toRow + 1, showActive);
		//	RemoveRow(fromRow);
		//	if(toRow > fromRow)
		//		toRow--;
		//	InsertTaskRowAt(toRow, task);
		//}

		private void addTask_Click(object sender, EventArgs e)
		{
			//add task below current
			int row = this.GetRow(sender as Control);
			InsertTaskRowAt(row + 1, project.InsertNewTask(row + 2, active: showActive));
		}

		//private void rowLabel_MouseDown(object sender, MouseEventArgs e)
		//{
		//	Label rowLabel = (sender as Label);
		//	rowLabel.DoDragDrop(this.GetRow(rowLabel).ToString(), DragDropEffects.Move);
		//}

		//private void rowLabel_DragEnter(object sender, DragEventArgs e)
		//{
		//	if(!e.Data.GetDataPresent(DataFormats.Text))
		//	{
		//		e.Effect = DragDropEffects.None;
		//		return;
		//	}
		//	string data = e.Data.GetData(DataFormats.Text).ToString();
		//	int row;
		//	if(!Int32.TryParse(data, out row))
		//	{
		//		e.Effect = DragDropEffects.None;
		//		return;
		//	}
		//	e.Effect = DragDropEffects.Move;
		//}

		//private void rowLabel_DragDrop(object sender, DragEventArgs e)
		//{
		//	if(!e.Data.GetDataPresent(DataFormats.Text))
		//	{
		//		return;
		//	}
		//	string data = e.Data.GetData(DataFormats.Text).ToString();
		//	int fromRow;
		//	if(!Int32.TryParse(data, out fromRow))
		//	{
		//		return;
		//	}
		//	int toRow = this.GetRow(sender as Label);
		//	MoveRow(fromRow, toRow);
		//}

		private void titleTextBox_TextChanged(object sender, EventArgs e)
		{
			TextBox textBox = (sender as TextBox);
			int row = this.GetRow(textBox);
			project.UpdateTitle(row + 1, textBox.Text, active: showActive);
		}

		private void statusComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (sender as ComboBox);
			int row = this.GetRow(comboBox);

			StatusChangeResult result = project.UpdateStatus(row + 1, comboBox.Text, active: showActive);
			Label dateDoneLabel = (Label)this.GetControlFromPosition(7, row);
			dateDoneLabel.Text = result.DoneDateString;

			if(result.ActiveInactiveChanged)
			{
				RemoveRow(row);
			}
		}

		private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (sender as ComboBox);
			int row = this.GetRow(comboBox);
			project.UpdateCategory(row + 1, comboBox.Text, active: showActive);
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

		private ComboBox NewComboBox(string name, string[] options, string selectedValue = null)
		{
			ComboBox comboBox = new ComboBox();
			comboBox.Font = regularFont;
			comboBox.FormattingEnabled = true;
			comboBox.Name = name;
			foreach(string option in options) //when using datasource, could not set the selected item
			{
				comboBox.Items.Add(option);
			}
			if(!String.IsNullOrEmpty(selectedValue))
			{
				comboBox.SelectedIndex = comboBox.FindString(selectedValue);
			}
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
