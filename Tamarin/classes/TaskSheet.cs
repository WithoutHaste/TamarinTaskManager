using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using OfficeOpenXml;
using WithoutHaste.DataFiles;

namespace Tamarin
{
	public class TaskSheet
	{
		private const string ACTIVE_SHEET_NAME = "Active";
		private const string INACTIVE_SHEET_NAME = "Inactive";

		private ColumnLayout columnLayout;
		private bool isActive;

		public List<Task> Tasks { get; set; }

		public TaskSheet(bool isActive)
		{
			Init(isActive);
		}

		public TaskSheet(MSExcel2003XmlFile xmlFile, string worksheetTitle, bool isActive)
		{
			Init(isActive);

			int tableIndex = xmlFile.GetTableIndex(worksheetTitle);
			if(tableIndex == -1) return;

			columnLayout = new ColumnLayout(xmlFile.GetRowValues(tableIndex, 0));
			for(int rowIndex = 1; rowIndex < xmlFile.GetRowCount(tableIndex); rowIndex++)
			{
				Tasks.Add(new Task(xmlFile.GetRowValues(tableIndex, rowIndex), columnLayout));
			}
		}

		public TaskSheet(ExcelPackage excelPackage, string name, bool isActive)
		{
			this.isActive = isActive;
			ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[name];
			columnLayout = (sheet == null) ? new ColumnLayout() : new ColumnLayout(sheet);
			if(!columnLayout.ValidLayout)
			{
				MessageBox.Show("Worksheet layout not recognized. Cannot load tasks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); //todo this is model level, should just throw exception to higher view level for message display
				return;
			}
			Tasks = LoadTasks(sheet);
		}

		//---------------------------------------------------

		private void Init(bool isActive)
		{
			this.isActive = isActive;
			columnLayout = new ColumnLayout();
			Tasks = new List<Task>();
		}

		//-----------------------------------------------------

		public void InsertTask(int row, Task task)
		{
			Tasks.Insert(row, task);
		}

		public string GetStatus(int row)
		{
			return Tasks[row].Status;
		}

		public string GetCategory(int row)
		{
			return Tasks[row].Category;
		}

		public Task GetTask(int row)
		{
			return Tasks[row];
		}

		public Task MoveRow(int fromRow, int toRow)
		{
			Task task = Tasks[fromRow];
			Tasks.Remove(task);
			//going down, push toRow up - already done by removing task
			//going up, push toRow down
			InsertTask(toRow, task);
			return task;
		}

		public void UpdateTitle(int row, string text)
		{
			Tasks[row].Description = text;
		}

		public StatusChangeResult UpdateStatus(int row, string status, string[] activeStatuses)
		{
			Task task = Tasks[row];
			bool oldStatusActive = activeStatuses.Contains(task.Status);
			bool newStatusActive = activeStatuses.Contains(status);

			StatusChangeResult result = new StatusChangeResult();
			result.ActiveInactiveChanged = (newStatusActive != isActive);
			result.DoneDate = (newStatusActive ? default(DateTime?) : DateTime.Now);

			task.Status = status;
			task.DoneDate = result.DoneDate;

			return result;
		}

		public void UpdateCategory(int row, string category)
		{
			Tasks[row].Category = category;
		}

		public void RemoveTask(int row)
		{
			Tasks.RemoveAt(row);
		}

		private List<Task> LoadTasks(ExcelWorksheet worksheet)
		{
			List<Task> tasks = new List<Task>();
			int row = 2;
			while(worksheet.Cells["A" + row].Value != null)
			{
				tasks.Add(new Task(worksheet, columnLayout, row));
				row++;
			}
			return tasks;
		}

		public void ValidateDoesNotContainStatuses(string[] invalidStatuses)
		{
			foreach(Task task in Tasks)
			{
				if(invalidStatuses.Contains(task.Status))
				{
					throw new Exception(String.Format("Status {0} cannot be both Active and Inactive: see task id {1}.", task.Status, task.Id));
				}
			}
		}

		public void WriteTo(ExcelPackage package)
		{
			string worksheetName = (isActive ? "Active" : "Inactive");
			package.Workbook.Worksheets.Add(worksheetName);
			ExcelWorksheet worksheet = package.Workbook.Worksheets.Last();
			ColumnLayout.WriteTaskHeaders(worksheet, isActive);

			int row = 2;
			worksheet.InsertRow(row, Tasks.Count);
			foreach(Task task in Tasks)
			{
				columnLayout.WriteTask(worksheet, task, row, isActive);
				row++;
			}
		}

		public void WriteTo(MSExcel2003XmlFile xmlFile)
		{
			int tableIndex = xmlFile.AddWorksheet(GetSheetName());
			columnLayout.WriteTaskHeaders(xmlFile, tableIndex, isActive);
			foreach(Task task in Tasks)
			{
				columnLayout.WriteTask(xmlFile, tableIndex, task, isActive);
			}
		}

		private string GetSheetName()
		{
			return isActive ? ACTIVE_SHEET_NAME : INACTIVE_SHEET_NAME;
		}
	}
}
