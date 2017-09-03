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
		private string fullPath;
		private Config config;

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
			ColumnLayout.WriteTaskHeaders(excelPackage.Workbook.Worksheets.Add(ACTIVE_SHEET_NAME));
			ColumnLayout.WriteTaskHeaders(excelPackage.Workbook.Worksheets.Add(INACTIVE_SHEET_NAME));
			config = new Config(excelPackage);
			return excelPackage;
		}

		private ExcelPackage OpenProject()
		{
			FileInfo file = new FileInfo(FullPath);
			ExcelPackage excelPackage = new ExcelPackage(file);
			if(excelPackage.Workbook.Worksheets[ACTIVE_SHEET_NAME] == null)
				ColumnLayout.WriteTaskHeaders(excelPackage.Workbook.Worksheets.Add(ACTIVE_SHEET_NAME));
			if(excelPackage.Workbook.Worksheets[INACTIVE_SHEET_NAME] == null)
				ColumnLayout.WriteTaskHeaders(excelPackage.Workbook.Worksheets.Add(INACTIVE_SHEET_NAME));
			config = new Config(excelPackage);
			return excelPackage;
		}

		public Task InsertTask(int row)
		{
			Task task = new Task() {
				Id = config.NextId,
				Status = config.DefaultActiveStatus,
				Category = config.DefaultCategory,
				CreateDate = DateTime.Now
			};
			ExcelWorksheet activeSheet = excelPackage.Workbook.Worksheets[ACTIVE_SHEET_NAME];
			activeSheet.InsertRow(row, 1);

			ColumnLayout columnLayout = new ColumnLayout(activeSheet);
			activeSheet.Cells[columnLayout.IdColumn + row].Value = task.Id;
			activeSheet.Cells[columnLayout.TitleColumn + row].Value = task.Title;
			activeSheet.Cells[columnLayout.StatusColumn + row].Value = task.Status;
			activeSheet.Cells[columnLayout.CategoryColumn + row].Value = task.Category;
			activeSheet.Cells[columnLayout.CreateDateColumn + row].Value = task.CreateDateString;
			activeSheet.Cells[columnLayout.StatusChangeDateColumn + row].Value = task.StatusChangeDateString;

			return task;
		}

		public void UpdateTitle(int row, string text)
		{
			ExcelWorksheet activeSheet = excelPackage.Workbook.Worksheets[ACTIVE_SHEET_NAME];
			ColumnLayout columnLayout = new ColumnLayout(activeSheet);
			activeSheet.Cells[columnLayout.TitleColumn + row].Value = text;
		}

		public void UpdateStatus(int row, string status)
		{
			ExcelWorksheet activeSheet = excelPackage.Workbook.Worksheets[ACTIVE_SHEET_NAME];
			ColumnLayout columnLayout = new ColumnLayout(activeSheet);
			activeSheet.Cells[columnLayout.StatusColumn + row].Value = status; //todo if inactive, move to other sheet
		}

		public void UpdateCategory(int row, string category)
		{
			ExcelWorksheet activeSheet = excelPackage.Workbook.Worksheets[ACTIVE_SHEET_NAME];
			ColumnLayout columnLayout = new ColumnLayout(activeSheet);
			activeSheet.Cells[columnLayout.CategoryColumn + row].Value = category;
		}

		public List<Task> GetActiveTasks()
		{
			List<Task> tasks = new List<Task>();
			ExcelWorksheet activeSheet = excelPackage.Workbook.Worksheets[ACTIVE_SHEET_NAME];
			if(activeSheet != null)
			{
				tasks.AddRange(LoadTasks(activeSheet));
			}
			return tasks;
		}

		public List<Task> GetInactiveTasks()
		{
			List<Task> tasks = new List<Task>();
			ExcelWorksheet inactiveSheet = excelPackage.Workbook.Worksheets[INACTIVE_SHEET_NAME];
			if(inactiveSheet != null)
			{
				tasks.AddRange(LoadTasks(inactiveSheet));
			}
			return tasks;
		}

		private List<Task> LoadTasks(ExcelWorksheet sheet)
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
