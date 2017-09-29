using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;

namespace TaskManagerX
{
	public class TaskSheet
	{
		private ExcelWorksheet sheet;
		private ColumnLayout columnLayout;
		private bool isActive;

		public TaskSheet(ExcelPackage excelPackage, string name, bool active)
		{
			sheet = excelPackage.Workbook.Worksheets[name];
			if(sheet == null)
			{
				sheet = excelPackage.Workbook.Worksheets.Add(name);
				ColumnLayout.WriteTaskHeaders(sheet, active);
			}
			columnLayout = new ColumnLayout(sheet); //TODO how to handle if some or all of expected headers are missing
			isActive = active;
		}

		public void InsertTask(int row, Task task)
		{
			sheet.InsertRow(row, 1);
			sheet.Cells[columnLayout.IdColumn + row].Value = task.Id;
			sheet.Cells[columnLayout.TitleColumn + row].Value = task.Title;
			sheet.Cells[columnLayout.StatusColumn + row].Value = task.Status;
			sheet.Cells[columnLayout.CategoryColumn + row].Value = task.Category;
			sheet.Cells[columnLayout.CreateDateColumn + row].Value = task.CreateDateString;
			if(columnLayout.DoneDateColumn != null)
			{
				sheet.Cells[columnLayout.DoneDateColumn + row].Value = task.DoneDateString;
			}
		}

		public string GetStatus(int row)
		{
			return sheet.Cells[columnLayout.StatusColumn + row].Value.ToString();
		}

		public Task GetTask(int row)
		{
			return new Task(sheet, columnLayout, row);
		}

		public Task MoveRow(int fromRow, int toRow)
		{
			Task task = GetTask(fromRow);
			RemoveTask(fromRow);
			if(toRow > fromRow)
				toRow--;
			InsertTask(toRow + 1, task);
			return task;
		}

		public void UpdateTitle(int row, string text)
		{
			sheet.Cells[columnLayout.TitleColumn + row].Value = text;
		}

		public StatusChangeResult UpdateStatus(int row, string status, string[] activeStatuses)
		{
			bool oldStatusActive = activeStatuses.Contains(sheet.Cells[columnLayout.StatusColumn + row].Value.ToString());
			bool newStatusActive = activeStatuses.Contains(status);

			StatusChangeResult result = new StatusChangeResult();
			result.ActiveInactiveChanged = (newStatusActive != isActive);
			result.DoneDate = (newStatusActive ? default(DateTime?) : DateTime.Now);

			sheet.Cells[columnLayout.StatusColumn + row].Value = status;
			sheet.Cells[columnLayout.DoneDateColumn + row].Value = result.DoneDateString;

			return result;
		}

		public void UpdateCategory(int row, string category)
		{
			sheet.Cells[columnLayout.CategoryColumn + row].Value = category;
		}

		public void RemoveTask(int row)
		{
			sheet.DeleteRow(row);
		}

		public List<Task> LoadTasks()
		{
			List<Task> tasks = new List<Task>();
			ColumnLayout columnLayout = new ColumnLayout(sheet);
			if(!columnLayout.ValidLayout)
			{
				MessageBox.Show("Worksheet layout not recognized. Cannot load tasks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); //todo this is model level, should just throw exception to higher view level for message display
				return tasks;
			}

			int row = 2;
			while(sheet.Cells["A" + row].Value != null)
			{
				tasks.Add(GetTask(row));
				row++;
			}

			return tasks;
		}

		public void ValidateDoesNotContainStatuses(string[] invalidStatuses)
		{
			int row = 2;
			while(sheet.Cells[columnLayout.StatusColumn + row].Value != null)
			{
				string taskStatus = sheet.Cells[columnLayout.StatusColumn + row].Value.ToString();
				if(invalidStatuses.Contains(taskStatus))
				{
					int taskId = Int32.Parse(sheet.Cells[columnLayout.IdColumn + row].Value.ToString());
					throw new Exception(String.Format("Status {0} cannot be both Active and Inactive: see task id {1}.", taskStatus, taskId));
				}
				row++;
			}
		}
	}
}
