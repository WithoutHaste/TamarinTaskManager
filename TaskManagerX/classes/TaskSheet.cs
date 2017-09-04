using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace TaskManagerX
{
	public class TaskSheet
	{
		private ExcelWorksheet sheet;
		private ColumnLayout columnLayout;

		public TaskSheet(ExcelPackage excelPackage, string name)
		{
			sheet = excelPackage.Workbook.Worksheets[name];
			if(sheet == null)
			{
				sheet = excelPackage.Workbook.Worksheets.Add(name);
				ColumnLayout.WriteTaskHeaders(sheet);
			}
			columnLayout = new ColumnLayout(sheet); //TODO how to handle if some or all of expected headers are missing
		}

		public void InsertTask(int row, Task task)
		{
			sheet.InsertRow(row, 1);
			sheet.Cells[columnLayout.IdColumn + row].Value = task.Id;
			sheet.Cells[columnLayout.TitleColumn + row].Value = task.Title;
			sheet.Cells[columnLayout.StatusColumn + row].Value = task.Status;
			sheet.Cells[columnLayout.CategoryColumn + row].Value = task.Category;
			sheet.Cells[columnLayout.CreateDateColumn + row].Value = task.CreateDateString;
			sheet.Cells[columnLayout.DoneDateColumn + row].Value = task.DoneDateString;
		}

		public string GetStatus(int row)
		{
			return sheet.Cells[columnLayout.StatusColumn + row].Value.ToString();
		}

		public Task GetTask(int row)
		{
			return new Task(sheet, columnLayout, row);
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
			result.ActiveInactiveChanged = (oldStatusActive != newStatusActive);
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
			ColumnLayout columnLayout = new ColumnLayout(sheet); //todo: are ALL columns required?
			if(!columnLayout.AllColumnsFound)
				return tasks;

			int row = 2;
			while(sheet.Cells["A" + row].Value != null)
			{
				tasks.Add(GetTask(row));
				row++;
			}

			return tasks;
		}
	}
}
