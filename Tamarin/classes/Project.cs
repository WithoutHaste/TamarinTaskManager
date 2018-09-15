﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OfficeOpenXml;
using WithoutHaste.DataFiles;

namespace Tamarin
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
		private static string CONFIG_SHEET_NAME = "Config";

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
			LastSavedDateTime = DateTime.Now;
		}

		private void SaveXML()
		{
			if(FileExtension.ToLower() != ".xml")
			{
				FullPath = FullPath.Replace(FileExtension, ".xml");
			}

			XmlDocument xmlDocument = new XmlDocument();
			XmlNode workbookNode = MSExcel2003XmlFormat.GenerateDefaultWorkbook(xmlDocument);

			activeSheet.WriteTo(xmlDocument, workbookNode);
			inactiveSheet.WriteTo(xmlDocument, workbookNode);
			config.WriteTo(xmlDocument, workbookNode);

			xmlDocument.Save(FullPath);
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
			XmlDocument xml = new XmlDocument();
			xml.Load(FullPath);
			XmlNodeList workbookNodes = xml.GetElementsByTagName("Workbook"); //c# note: only checks immediate children
			if(workbookNodes.Count != 1)
				throw new Exception("Incorrect XML Format: expects exactly one Workbook.");

			XmlNode workbookNode = workbookNodes[0];
			foreach(XmlNode worksheetNode in workbookNode.ChildNodes)
			{
				if(worksheetNode.LocalName != "Worksheet") continue;

				//c# notes: add Cast<T> to LINQ notes - casts each element to type T
				XmlNode tableNode = worksheetNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.LocalName == "Table");
				if(tableNode == null) continue;

				if(worksheetNode.Attributes["ss:Name"].Value == ACTIVE_SHEET_NAME)
				{
					activeSheet = new TaskSheet(tableNode, isActive: true);
				}
				else if(worksheetNode.Attributes["ss:Name"].Value == INACTIVE_SHEET_NAME)
				{
					inactiveSheet = new TaskSheet(tableNode, isActive: false);
				}
				else if(worksheetNode.Attributes["ss:Name"].Value == CONFIG_SHEET_NAME)
				{
					config = new ConfigSheet(tableNode);
				}
			}
			if(activeSheet == null) activeSheet = new TaskSheet(isActive: true);
			if(inactiveSheet == null) inactiveSheet = new TaskSheet(isActive: false);
			if(config == null) config = new ConfigSheet();
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
			return GetSheet(active).GetTask(ProjectIndex(row)).Description;
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
