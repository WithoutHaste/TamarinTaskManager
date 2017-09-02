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
		//todo: update logic so these are get-only or only accessed through methods
		public List<Status> Statuses { get; set; }
		public List<string> Categories { get; set; }
		public int MaxId { get; set; }

		public string FullPath {
			get {
				return fullPath;
			}
			set {
				if(String.IsNullOrEmpty(value))
					throw new Exception("Cannot save to empty path.");
				if(ExcelPackage != null)
				{
					ExcelPackage.File = new FileInfo(value);
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

		private ExcelPackage ExcelPackage;
		private string fullPath;

		public Project()
		{
			ExcelPackage = CreateNewProject();
		}

		public Project(string fullPath)
		{
			FullPath = fullPath;
			ExcelPackage = OpenProject();
		}

		public void Save()
		{
			if(ExcelPackage.File == null)
				throw new Exception("Filename not set.");
			ExcelPackage.Save();
		}

		public void Dispose()
		{
			ExcelPackage.Dispose();
		}

		private ExcelPackage CreateNewProject()
		{
			ExcelPackage excelPackage = new ExcelPackage();
			AddDefaultConfigSheet(excelPackage);
			return excelPackage;
		}

		//todo: ? move all config worksheet operations to class ConfigWorksheet

		private ExcelPackage OpenProject()
		{
			FileInfo file = new FileInfo(FullPath);
			ExcelPackage excelPackage = new ExcelPackage(file);

			ExcelWorksheet configSheet = excelPackage.Workbook.Worksheets["Config"];
			if(configSheet == null)
			{
				AddDefaultConfigSheet(excelPackage);
			}
			else
			{
				//Config sheet should not be touched by users, so can expect the exact correct format
				if(configSheet.Cells["A1"].Value.ToString() == "Status" && configSheet.Cells["B1"].Value.ToString() == "Active")
				{
					Statuses = new List<Status>();
					int row = 2;
					while(configSheet.GetValue(Row: row, Column: 1) != null)
					{
						Statuses.Add(new Status(configSheet.GetValue(row, 1).ToString(), configSheet.GetValue(row, 2).ToString()));
						row++;
					}
				}
				else
				{
					SetDefaultStatuses();
				}

				if(configSheet.Cells["D1"].Value.ToString() == "Category")
				{
					Categories = new List<string>();
					int row = 2;
					while(configSheet.GetValue(Row: row, Column: 4) != null)
					{
						Categories.Add(configSheet.GetValue(row, 4).ToString());
						row++;
					}
				}
				else
				{
					SetDefaultCategories();
				}

				if(configSheet.Cells["F1"].Value.ToString() == "Max Id" && configSheet.Cells["F2"].Value.ToString() != null) //todo: move all string literals to constants
				{
					MaxId = Int32.Parse(configSheet.Cells["F2"].Value.ToString());
				}
				else
				{
					MaxId = 0;
				}

				WriteConfigSheet(configSheet);
			}

			return excelPackage;
		}

		private void SetDefaultStatuses()
		{
			Statuses = new List<Status>() {
				new Status("Todo", active:true),
				new Status("Done", active:false)
			};
		}

		private void SetDefaultCategories()
		{
			Categories = new List<string>() { "Task", "Bug" };
		}

		private void AddDefaultConfigSheet(ExcelPackage excelPackage)
		{
			ExcelWorksheet configSheet = excelPackage.Workbook.Worksheets.Add("Config");
			SetDefaultStatuses();
			SetDefaultCategories();
			MaxId = 0;
			WriteConfigSheet(configSheet);
		}

		private void WriteConfigSheet(ExcelWorksheet configSheet)
		{
			ClearWorksheet(configSheet);

			configSheet.Cells["A1"].Value = "Status";
			configSheet.Cells["B1"].Value = "Active";
			configSheet.Cells["D1"].Value = "Category";
			configSheet.Cells["F1"].Value = "Max Id";
			configSheet.Cells["A1:F1"].Style.Font.Bold = true;

			int row = 2;
			foreach(Status status in Statuses)
			{
				configSheet.Cells["A" + row].Value = status.Name;
				configSheet.Cells["B" + row].Value = (status.Active ? "Active" : "Inactive");
				row++;
			}

			row = 2;
			foreach(string category in Categories)
			{
				configSheet.Cells["D" + row].Value = category;
				row++;
			}

			configSheet.Cells["F2"].Value = MaxId;
		}

		private void ClearWorksheet(ExcelWorksheet sheet)
		{
			while(sheet.Cells["A1"].Value != null)
			{
				sheet.DeleteRow(rowFrom: 1, rows: 100, shiftOtherRowsUp: true);
			}
		}
	}
}
