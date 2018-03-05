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

		public DateTime? LastSavedDateTime {
			get;
			set;
		}

		public DateTime? LastEditedDateTime {
			get;
			set;
		}

		public bool EditedSinceLastSave {
			get {
				if(!LastEditedDateTime.HasValue)
					return false;
				if(!LastSavedDateTime.HasValue)
					return true;
				return (LastEditedDateTime.Value > LastSavedDateTime.Value);
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

		public string[] ActiveStatuses {
			get {
				return config.ActiveStatuses;
			}
		}

		public string[] InactiveStatuses {
			get {
				return config.InactiveStatuses;
			}
		}

		public string[] Categories {
			get {
				return config.Categories.ToArray();
			}
			set {
				config.SetCategories(value);
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
			LastSavedDateTime = DateTime.Now;
		}

		public void Save()
		{
			if(excelPackage.File == null)
				throw new Exception("Filename not set.");
			excelPackage.Save();
			LastSavedDateTime = DateTime.Now;
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
			Task previousTask = (row <= 2) ? default(Task) : GetTask(row - 1, active);

			Task task = new Task() {
				Id = config.NextId,
				Status = (previousTask == null) ? config.DefaultActiveStatus : previousTask.Status,
				Category = (previousTask == null) ? config.DefaultCategory : previousTask.Category,
				CreateDate = DateTime.Now
			};

			InsertTask(row, active, task);

			return task;
		}

		public Task GetTask(int row, bool active)
		{
			return GetSheet(active).GetTask(row);
		}

		public string GetTitle(int row, bool active)
		{
			return GetSheet(active).GetTask(row).Title;
		}

		public void InsertTask(int row, bool active, Task task)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).InsertTask(row, task);
		}

		public void RemoveTask(int row, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).RemoveTask(row);
		}

		public Task MoveRow(int fromRow, int toRow, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			TaskSheet sheet = GetSheet(active);
			return sheet.MoveRow(fromRow, toRow);
		}

		public void UpdateTitle(int row, string text, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).UpdateTitle(row, text);
		}

		public string GetStatus(int row, bool active)
		{
			return GetSheet(active).GetStatus(row);
		}

		public StatusChangeResult UpdateStatus(int row, string status, bool active)
		{
			LastEditedDateTime = DateTime.Now;
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

		public string GetCategory(int row, bool active)
		{
			return GetSheet(active).GetCategory(row);
		}

		public void UpdateCategory(int row, string category, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).UpdateCategory(row, category);
		}

		public List<Task> GetTasks(bool active)
		{
			return GetSheet(active).LoadTasks();
		}

		public void SetStatuses(string[] active, string[] inactive)
		{
			LastEditedDateTime = DateTime.Now;

			//check if any task contradicts these settings
			activeSheet.ValidateDoesNotContainStatuses(inactive);
			inactiveSheet.ValidateDoesNotContainStatuses(active);

			//apply settings
			config.SetStatuses(active, inactive);
		}

		private TaskSheet GetSheet(bool active)
		{
			return (active ? activeSheet : inactiveSheet);
		}
	}
}
