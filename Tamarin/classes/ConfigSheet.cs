using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace TaskManagerX
{
	public class ConfigSheet
	{
		public List<Status> Statuses { get; set; }
		public List<string> Categories { get; set; }
		public int MaxId { get; set; }

		public string[] ActiveStatuses {
			get {
				return Statuses.Where(x => x.Active).Select(x => x.Name).ToArray();
			}
		}

		public string[] InactiveStatuses {
			get {
				return Statuses.Where(x => !x.Active).Select(x => x.Name).ToArray();
			}
		}

		public int NextId {
			get {
				MaxId++;
				configSheet.Cells["F2"].Value = MaxId;
				return MaxId;
			}
		}

		public string DefaultActiveStatus {
			get {
				return Statuses.First(x => x.Active).Name;
			}
		}

		public string DefaultCategory {
			get {
				return Categories.First();
			}
		}

		private ExcelWorksheet configSheet;

		private static string SHEET_NAME = "Config";
		private static string STATUS_NAME = "Status";
		private static string ACTIVE_NAME = "Active";
		private static string CATEGORY_NAME = "Category";
		private static string ID_NAME = "Max Id";
		private static int DEFAULT_ID = 0;

		public ConfigSheet(ExcelPackage excelPackage)
		{
			configSheet = excelPackage.Workbook.Worksheets[SHEET_NAME];
			if(configSheet == null)
			{
				AddDefaultConfigSheet(excelPackage);
				return;
			}

			//Config sheet should not be touched by users, so can expect the exact correct format
			if(configSheet.Cells["A1"].Value.ToString() == STATUS_NAME && configSheet.Cells["B1"].Value.ToString() == ACTIVE_NAME)
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

			if(configSheet.Cells["D1"].Value.ToString() == CATEGORY_NAME)
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

			if(configSheet.Cells["F1"].Value.ToString() == ID_NAME && configSheet.Cells["F2"].Value.ToString() != null)
			{
				MaxId = Int32.Parse(configSheet.Cells["F2"].Value.ToString());
			}
			else
			{
				MaxId = DEFAULT_ID;
			}

			WriteConfigSheet(configSheet);
		}

		public void SetCategories(string[] categories)
		{
			List<string> validCategories = categories.Where(x => !String.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			if(validCategories.Count() == 0)
				throw new Exception("Project must contain at least one category.");

			Categories = validCategories;
			WriteConfigSheet(configSheet);
		}

		public void SetStatuses(string[] active, string[] inactive)
		{
			List<string> validActive = active.Where(x => !String.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			if(validActive.Count == 0)
				throw new Exception("Project must contain at least one Active status.");

			List<string> validInactive = inactive.Where(x => !String.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			if(validInactive.Count == 0)
				throw new Exception("Project must contain at least one Inactive status.");

			string[] intersection = validActive.Intersect(validInactive).ToArray();
			if(intersection.Length > 0)
				throw new Exception(String.Format("Status(es) cannot be both Active and Inactive: {0}", String.Join(", ", intersection)));

			Statuses.Clear();
			Statuses.AddRange(validActive.Select(x => new Status(x, true)));
			Statuses.AddRange(validInactive.Select(x => new Status(x, false)));
			WriteConfigSheet(configSheet);
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
			Categories = new List<string>() {
				"Task",
				"Bug"
			};
		}

		private void AddDefaultConfigSheet(ExcelPackage excelPackage)
		{
			configSheet = excelPackage.Workbook.Worksheets.Add(SHEET_NAME);
			SetDefaultStatuses();
			SetDefaultCategories();
			MaxId = DEFAULT_ID;
			WriteConfigSheet(configSheet);
		}

		private void WriteConfigSheet(ExcelWorksheet configSheet)
		{
			ClearWorksheet(configSheet);

			configSheet.Cells["A1"].Value = STATUS_NAME;
			configSheet.Cells["B1"].Value = ACTIVE_NAME;
			configSheet.Cells["D1"].Value = CATEGORY_NAME;
			configSheet.Cells["F1"].Value = ID_NAME;
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

		public void WriteTo(ExcelPackage package)
		{
			package.Workbook.Worksheets.Add(SHEET_NAME);
			ExcelWorksheet worksheet = package.Workbook.Worksheets.Last();
			WriteConfigSheet(worksheet);
		}
	}
}

