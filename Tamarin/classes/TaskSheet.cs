using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using OfficeOpenXml;
using WithoutHaste.DataFormats;

namespace Tamarin
{
	public class TaskSheet
	{
		private ColumnLayout columnLayout;
		private bool isActive;

		public List<Task> Tasks { get; set; }

		public TaskSheet(bool isActive)
		{
			this.isActive = isActive;
			columnLayout = new ColumnLayout();
			Tasks = new List<Task>();
		}

		public TaskSheet(XmlNode tableNode, bool isActive)
		{
			this.isActive = isActive;
			Tasks = new List<Task>();

			columnLayout = new ColumnLayout(tableNode);
			foreach(XmlNode rowNode in MSExcel2003XmlFormat.GetRows(tableNode, skipFirstRow: true))
			{
				Tasks.Add(new Task(rowNode, columnLayout));
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

		public void WriteTo(XmlDocument xmlDocument, XmlNode workbookNode)
		{
			XmlNode tableNode = MSExcel2003XmlFormat.AddWorksheetAndTableToWorkbook(xmlDocument, workbookNode, (isActive ? "Active" : "Inactive")); //todo: make constants
			columnLayout.WriteTaskHeaders(xmlDocument, tableNode, isActive);
			foreach(Task task in Tasks)
			{
				columnLayout.WriteTask(xmlDocument, tableNode, task, isActive);
			}
		}
	}
}
