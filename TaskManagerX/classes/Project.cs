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
		private Config config;

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
			config = new Config(excelPackage);
			return excelPackage;
		}

		//todo: ? move all config worksheet operations to class ConfigWorksheet

		private ExcelPackage OpenProject()
		{
			FileInfo file = new FileInfo(FullPath);
			ExcelPackage excelPackage = new ExcelPackage(file);
			config = new Config(excelPackage);
			return excelPackage;
		}

	}
}
