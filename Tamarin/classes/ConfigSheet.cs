using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OfficeOpenXml;
using WithoutHaste.DataFiles;

namespace Tamarin
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

		private const string SHEET_NAME = "Config";
		private const string STATUS_NAME = "Status";
		private const string ACTIVE_NAME = "Active";
		private const string CATEGORY_NAME = "Category";
		private const string ID_NAME = "Max Id";
		private const int DEFAULT_ID = 0;

		private const string IS_ACTIVE = "Active";
		private const string IS_INACTIVE = "Inactive";

		//---------------------------------------------

		public ConfigSheet()
		{
			Init();
		}

		public ConfigSheet(MSExcel2003XmlFile xmlFile)
		{
			int tableIndex = xmlFile.GetTableIndex(SHEET_NAME);
			if(tableIndex == -1)
			{
				Init();
				return;
			}

			Statuses = new List<Status>();
			List<string> statusValues = xmlFile.GetColumnValues(tableIndex, STATUS_NAME);
			List<string> isActiveValues = xmlFile.GetColumnValues(tableIndex, ACTIVE_NAME);
			if(statusValues == null || isActiveValues == null || statusValues.Count == 0 || isActiveValues.Count == 0)
			{
				SetDefaultStatuses();
			}
			else
			{
				for(int i = 0; i < statusValues.Count; i++)
				{
					if(i >= isActiveValues.Count) break;
					Statuses.Add(new Status(statusValues[i], isActiveValues[i]));
				}
			}

			Categories = xmlFile.GetColumnValues(tableIndex, CATEGORY_NAME);
			if(Categories == null || Categories.Count == 0)
			{
				SetDefaultCategories();
			}

			List<string> idValues = xmlFile.GetColumnValues(tableIndex, ID_NAME);
			if(idValues == null || idValues.Count == 0)
			{
				MaxId = DEFAULT_ID;
			}
			else
			{
				MaxId = Int32.Parse(idValues[0]);
			}
		}

		public ConfigSheet(ExcelPackage excelPackage)
		{
			ExcelWorksheet configSheet = ExcelPackageHelper.GetWorksheet(excelPackage, SHEET_NAME);
			if(configSheet == null)
			{
				AddDefaultConfigSheet(excelPackage);
				return;
			}

			List<object> statuses = ExcelPackageHelper.GetColumnByHeader(configSheet, STATUS_NAME);
			List<object> actives = ExcelPackageHelper.GetColumnByHeader(configSheet, ACTIVE_NAME);
			if(statuses.Count > 0)
			{
				Statuses = new List<Status>();
				for(int i = 0; i < statuses.Count; i++)
				{
					bool isActive = true;
					if(actives.Count > i) isActive = (actives[i].ToString() == IS_ACTIVE);
					Statuses.Add(new Status(statuses[i].ToString(), isActive));
				}
			}
			else
			{
				SetDefaultStatuses();
			}

			Categories = ExcelPackageHelper.GetColumnByHeader(configSheet, CATEGORY_NAME).Cast<string>().ToList();
			if(Categories.Count == 0)
			{
				SetDefaultCategories();
			}

			List<object> ids = ExcelPackageHelper.GetColumnByHeader(configSheet, ID_NAME);
			if(ids.Count > 0)
			{
				MaxId = (int)ids[0];
			}
			else
			{
				MaxId = DEFAULT_ID;
			}

			WriteConfigSheet(configSheet); //standardize format
		}

		//---------------------------------------------

		private void Init()
		{
			SetDefaultStatuses();
			SetDefaultCategories();
			MaxId = DEFAULT_ID;
		}

		//---------------------------------------------

		public void SetCategories(string[] categories)
		{
			List<string> validCategories = categories.Where(x => !String.IsNullOrEmpty(x))
				.Distinct()
				.ToList();
			if(validCategories.Count() == 0)
				throw new Exception("Project must contain at least one category.");

			Categories = validCategories;
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
			ExcelWorksheet configSheet = ExcelPackageHelper.AddWorksheet(excelPackage, SHEET_NAME);
			SetDefaultStatuses();
			SetDefaultCategories();
			MaxId = DEFAULT_ID;
			WriteConfigSheet(configSheet);
		}

		private void WriteConfigSheet(ExcelWorksheet worksheet)
		{
			ExcelPackageHelper.Clear(worksheet);
			ExcelPackageHelper.AppendRow(worksheet, new List<object>() {
				STATUS_NAME, ACTIVE_NAME, "", CATEGORY_NAME, "", ID_NAME
			});
			worksheet.Cells["A1:F1"].Style.Font.Bold = true;

			ExcelPackageHelper.SetColumn(worksheet, "A", Statuses.Select(s => (object)s.Name).ToList(), skipFirstRow: true);
			ExcelPackageHelper.SetColumn(worksheet, "B", Statuses.Select(s => (object)(s.Active ? IS_ACTIVE : IS_INACTIVE)).ToList(), skipFirstRow: true);
			ExcelPackageHelper.SetColumn(worksheet, "D", Categories.Select(c => (object)c).ToList(), skipFirstRow: true);
			ExcelPackageHelper.SetColumn(worksheet, "F", new List<object>() { (object)MaxId }, skipFirstRow: true);
		}

		public void WriteTo(ExcelPackage package)
		{
			ExcelWorksheet worksheet = ExcelPackageHelper.AddWorksheet(package, SHEET_NAME);
			WriteConfigSheet(worksheet);
		}

		public void WriteTo(MSExcel2003XmlFile xmlFile)
		{
			int tableIndex = xmlFile.AddWorksheet(SHEET_NAME);
			xmlFile.AddHeaderRow(tableIndex, new List<string>() {
				STATUS_NAME, ACTIVE_NAME, "", CATEGORY_NAME, "", ID_NAME
			});

			List<XmlNode> statusNodes = Statuses.Select(status => xmlFile.GenerateTextCell(status.Name)).ToList();
			List<XmlNode> activeNodes = Statuses.Select(status => xmlFile.GenerateTextCell((status.Active ? "Active" : "Inactive"))).ToList(); //todo: make constants, but not connected to table names
			List<XmlNode> categoryNodes = Categories.Select(category => xmlFile.GenerateTextCell(category)).ToList();
			xmlFile.AddColumns(tableIndex, new List<List<XmlNode>>() {
				statusNodes,
				activeNodes,
				new List<XmlNode>(),
				categoryNodes,
				new List<XmlNode>(),
				new List<XmlNode>() { xmlFile.GenerateNumberCell(MaxId) }
			});
		}
	}
}

