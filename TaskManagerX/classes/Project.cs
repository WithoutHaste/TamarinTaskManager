using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace TaskManagerX
{
	public class Project : IDisposable
	{
		//ExcelWorksheet has 1-based row index

		public string FullPath {
			get {
				return fullPath;
			}
			set {
				if(String.IsNullOrEmpty(value))
					throw new Exception("Cannot save to empty path.");
				if(excelPackage != null)
				{
					excelPackage.File = new FileInfo(value);
				}
				fullPath = value;
			}
		}

		public string Name {
			get {
				if (FullPath == null)
					return "new";
				return Path.GetFileNameWithoutExtension(FullPath);
			}
		}

		public bool NotNamed {
			get {
				return (FullPath == null);
			}
		}

		public string[] Statuses {
			get {
				return config.Statuses.Select(x => x.Name).ToArray();
			}
		}

		public string[] Categories {
			get {
				return config.Categories.ToArray();
			}
		}

		private ExcelPackage excelPackage;
		private TaskSheet activeSheet;
		private TaskSheet inactiveSheet;
		private string fullPath;
		private ConfigSheet config;

		private static string ACTIVE_SHEET_NAME = "Active";
		private static string INACTIVE_SHEET_NAME = "Inactive";

		public Project()
		{
			excelPackage = CreateNewProject();
		}

		public Project(string fullPath)
		{
			FullPath = fullPath;
			excelPackage = OpenProject();
		}

		public void Save()
		{
			if(excelPackage.File == null)
				throw new Exception("Filename not set.");
			excelPackage.Save();
		}

		public void Dispose()
		{
			excelPackage.Dispose();
		}

		private ExcelPackage CreateNewProject()
		{
			ExcelPackage excelPackage = new ExcelPackage();
			activeSheet = new TaskSheet(excelPackage, ACTIVE_SHEET_NAME, true);
			inactiveSheet = new TaskSheet(excelPackage, INACTIVE_SHEET_NAME, false);
			config = new ConfigSheet(excelPackage);
			return excelPackage;
		}

		private ExcelPackage OpenProject()
		{
			FileInfo file = new FileInfo(FullPath);
			ExcelPackage excelPackage = new ExcelPackage(file);
			activeSheet = new TaskSheet(excelPackage, ACTIVE_SHEET_NAME, true);
			inactiveSheet = new TaskSheet(excelPackage, INACTIVE_SHEET_NAME, false);
			config = new ConfigSheet(excelPackage);
			return excelPackage;
		}

		public Task InsertNewTask(int row, bool active)
		{
			Task task = new Task() {
				Id = config.NextId,
				Status = config.DefaultActiveStatus,
				Category = config.DefaultCategory,
				CreateDate = DateTime.Now
			};

			GetSheet(active).InsertTask(row, task);

			return task;
		}

		public Task MoveRow(int fromRow, int toRow, bool active)
		{
			TaskSheet sheet = GetSheet(active);
			return sheet.MoveRow(fromRow, toRow);
		}

		public void UpdateTitle(int row, string text, bool active)
		{
			GetSheet(active).UpdateTitle(row, text);
		}

		public StatusChangeResult UpdateStatus(int row, string status, bool active)
		{
			TaskSheet selectedSheet = GetSheet(active);
			TaskSheet otherSheet = GetSheet(!active);

			StatusChangeResult result = selectedSheet.UpdateStatus(row, status, config.ActiveStatuses);

			if(result.ActiveInactiveChanged)
			{
				Task task = selectedSheet.GetTask(row);
				task.DoneDate = DateTime.Now;
				otherSheet.InsertTask(2, task);
				selectedSheet.RemoveTask(row);
			}

			return result;
		}

		public void UpdateCategory(int row, string category, bool active)
		{
			GetSheet(active).UpdateCategory(row, category);
		}

		public List<Task> GetTasks(bool active)
		{
			return GetSheet(active).LoadTasks();
		}

		private TaskSheet GetSheet(bool active)
		{
			return (active ? activeSheet : inactiveSheet);
		}
	}
}
