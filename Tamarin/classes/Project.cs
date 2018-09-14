using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OfficeOpenXml;

namespace TaskManagerX
{
	public class Project
	{
		public string FullPath { get; set; }

		public string FileExtension {
			get {
				return Path.GetExtension(FullPath);
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

		public bool EditedByOutsideSource {
			get {
				if(FullPath == null || !File.Exists(FullPath))
					return false;
				return (File.GetLastWriteTime(FullPath) > LastSavedDateTime);
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
				LastEditedDateTime = DateTime.Now;
			}
		}

		private TaskSheet activeSheet;
		private TaskSheet inactiveSheet;
		private ConfigSheet config;

		private static string ACTIVE_SHEET_NAME = "Active";
		private static string INACTIVE_SHEET_NAME = "Inactive";

		public Project()
		{
			CreateNewProject();
		}

		public Project(string fullPath)
		{
			FullPath = fullPath;
			OpenProject();
			LastSavedDateTime = DateTime.Now;
		}

		public void Save()
		{
			if(FullPath == null)
				throw new Exception("Filename not set.");

			SaveXML();
			//SaveXLSX();

			LastSavedDateTime = DateTime.Now;
		}

		private void SaveXML()
		{
			if(FileExtension.ToLower() != ".xml")
			{
				FullPath = FullPath.Replace(FileExtension, ".xml");
			}

			XmlDocument xml = new XmlDocument();

			XmlNode versionNode = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
			xml.AppendChild(versionNode);

			XmlDocumentFragment applicationNode = xml.CreateDocumentFragment();
			applicationNode.InnerXml = "<?mso-application progid=\"Excel.Sheet\"?>";
			xml.AppendChild(applicationNode);

			XmlDocumentFragment workbookFragment = xml.CreateDocumentFragment();
			workbookFragment.InnerXml = "<Workbook xmlns:c='urn:schemas-microsoft-com:office:component:spreadsheet' xmlns:html='http://www.w3.org/TR/REC-html40' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='urn:schemas-microsoft-com:office:spreadsheet' xmlns:x2='http://schemas.microsoft.com/office/excel/2003/xml' xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet' xmlns:x='urn:schemas-microsoft-com:office:excel'></Workbook>";
			xml.AppendChild(workbookFragment);

			XmlNode workbookNode = xml.GetElementsByTagName("Workbook")[0];

			//passing workbookNode.NamespaceURI (the root's namespace) into child that do not need any namespace so that an empty xmlns="" is not added to that tag
			XmlNode stylesNode = xml.CreateElement("Styles", workbookNode.NamespaceURI);
			workbookNode.AppendChild(stylesNode);

			string shortDateStyleId = "cellShortDate";
			XmlNode shortDateStyleNode = xml.CreateElement("Style", workbookNode.NamespaceURI);
			XmlAttribute shortDateAttribute = xml.CreateAttribute("ss", "ID", "urn:schemas-microsoft-com:office:spreadsheet");
			shortDateAttribute.Value = shortDateStyleId;
			shortDateStyleNode.Attributes.Append(shortDateAttribute);
			stylesNode.AppendChild(shortDateStyleNode);
			XmlNode numberFormatNode = xml.CreateElement("NumberFormat", workbookNode.NamespaceURI);
			XmlAttribute numberFormatAttribute = xml.CreateAttribute("ss", "Format", "urn:schemas-microsoft-com:office:spreadsheet");
			numberFormatAttribute.Value = "Short Date";
			numberFormatNode.Attributes.Append(numberFormatAttribute);
			shortDateStyleNode.AppendChild(numberFormatNode);

			string headerStyleId = "header";
			XmlNode headerStyleNode = xml.CreateElement("Style", workbookNode.NamespaceURI);
			XmlAttribute headerAttribute = xml.CreateAttribute("ss", "ID", "urn:schemas-microsoft-com:office:spreadsheet");
			headerAttribute.Value = headerStyleId;
			headerStyleNode.Attributes.Append(headerAttribute);
			stylesNode.AppendChild(headerStyleNode);
			XmlNode fontFormatNode = xml.CreateElement("Font", workbookNode.NamespaceURI);
			XmlAttribute fontFormatAttribute = xml.CreateAttribute("ss", "Bold", "urn:schemas-microsoft-com:office:spreadsheet");
			fontFormatAttribute.Value = "1";
			fontFormatNode.Attributes.Append(fontFormatAttribute);
			headerStyleNode.AppendChild(fontFormatNode);

			string paragraphStyleId = "paragraph";
			XmlNode paragraphStyleNode = xml.CreateElement("Style", workbookNode.NamespaceURI);
			XmlAttribute paragraphAttribute = xml.CreateAttribute("ss", "ID", "urn:schemas-microsoft-com:office:spreadsheet");
			paragraphAttribute.Value = paragraphStyleId;
			paragraphStyleNode.Attributes.Append(paragraphAttribute);
			stylesNode.AppendChild(paragraphStyleNode);
			XmlNode alignmentNode = xml.CreateElement("Alignment", workbookNode.NamespaceURI);
			XmlAttribute alignmentAttribute = xml.CreateAttribute("ss", "WrapText", "urn:schemas-microsoft-com:office:spreadsheet");
			alignmentAttribute.Value = "1";
			alignmentNode.Attributes.Append(alignmentAttribute);
			paragraphStyleNode.AppendChild(alignmentNode);

			activeSheet.WriteTo(xml, workbookNode, shortDateStyleId, headerStyleId, paragraphStyleId);
			inactiveSheet.WriteTo(xml, workbookNode, shortDateStyleId, headerStyleId, paragraphStyleId);
			config.WriteTo(xml, workbookNode, headerStyleId);

			xml.Save(FullPath);
		}

		private void SaveXLSX()
		{
			if(FileExtension.ToLower() != ".xlsx")
			{
				FullPath = FullPath.Replace(FileExtension, ".xlsx");
			}

			ExcelPackage newPackage = new ExcelPackage();
			activeSheet.WriteTo(newPackage);
			inactiveSheet.WriteTo(newPackage);
			config.WriteTo(newPackage);
			newPackage.SaveAs(new FileInfo(FullPath));
		}

		private void CreateNewProject()
		{
			config = new ConfigSheet();
			activeSheet = new TaskSheet(isActive: true);
			inactiveSheet = new TaskSheet(isActive: false);
		}

		private void OpenProject()
		{
			switch(FileExtension.ToLower())
			{
				case ".xml":
					OpenProjectXML();
					break;
				case ".xlsx":
					OpenProjectXLSX();
					break;
				default: throw new NotSupportedException("File format not supported: " + FileExtension);
			}
		}

		private void OpenProjectXML()
		{
			//TODO
			if(FileExtension.ToLower() != ".xlsx")
			{
				FullPath = FullPath.Replace(FileExtension, ".xlsx");
			}
			OpenProjectXLSX();
		}

		private void OpenProjectXLSX()
		{
			FileInfo file = new FileInfo(FullPath);
			ExcelPackage excelPackage = new ExcelPackage(file);
			activeSheet = new TaskSheet(excelPackage, ACTIVE_SHEET_NAME, true);
			inactiveSheet = new TaskSheet(excelPackage, INACTIVE_SHEET_NAME, false);
			config = new ConfigSheet(excelPackage);
		}

		public void ReloadProject()
		{
			OpenProject();
			LastSavedDateTime = DateTime.Now;
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
			return GetSheet(active).GetTask(ProjectIndex(row));
		}

		public string GetTitle(int row, bool active)
		{
			return GetSheet(active).GetTask(ProjectIndex(row)).Title;
		}

		public void InsertTask(int row, bool active, Task task)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).InsertTask(ProjectIndex(row), task);
		}

		public void RemoveTask(int row, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).RemoveTask(ProjectIndex(row));
		}

		public Task MoveRow(int fromRow, int toRow, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			TaskSheet sheet = GetSheet(active);
			return sheet.MoveRow(ProjectIndex(fromRow), ProjectIndex(toRow));
		}

		public void UpdateTitle(int row, string text, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).UpdateTitle(ProjectIndex(row), text);
		}

		public string GetStatus(int row, bool active)
		{
			return GetSheet(active).GetStatus(ProjectIndex(row));
		}

		public StatusChangeResult UpdateStatus(int row, string status, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			TaskSheet selectedSheet = GetSheet(active);
			TaskSheet otherSheet = GetSheet(!active);

			StatusChangeResult result = selectedSheet.UpdateStatus(ProjectIndex(row), status, config.ActiveStatuses);

			if(result.ActiveInactiveChanged)
			{
				Task task = selectedSheet.GetTask(ProjectIndex(row));
				task.DoneDate = DateTime.Now;
				otherSheet.InsertTask(0, task);
				selectedSheet.RemoveTask(ProjectIndex(row));
			}

			return result;
		}

		public string GetCategory(int row, bool active)
		{
			return GetSheet(active).GetCategory(ProjectIndex(row));
		}

		public void UpdateCategory(int row, string category, bool active)
		{
			LastEditedDateTime = DateTime.Now;
			GetSheet(active).UpdateCategory(ProjectIndex(row), category);
		}

		public List<Task> GetTasks(bool active)
		{
			return GetSheet(active).Tasks;
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

		private int ProjectIndex(int tableIndex)
		{
			return tableIndex - 1;
		}
	}
}
