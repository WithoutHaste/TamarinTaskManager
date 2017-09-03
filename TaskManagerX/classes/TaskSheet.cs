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
			sheet.Cells[columnLayout.StatusChangeDateColumn + row].Value = task.StatusChangeDateString;
		}

		public void UpdateTitle(int row, string text)
		{
			sheet.Cells[columnLayout.TitleColumn + row].Value = text;
		}

		public void UpdateStatus(int row, string status)
		{
			sheet.Cells[columnLayout.StatusChangeDateColumn + row].Value = DateTime.Now.ToShortDateString();
			sheet.Cells[columnLayout.StatusColumn + row].Value = status;
		}

		public void UpdateCategory(int row, string category)
		{
			sheet.Cells[columnLayout.CategoryColumn + row].Value = category;
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
				tasks.Add(new Task(sheet, columnLayout, row));
				row++;
			}

			return tasks;
		}
	}
}
