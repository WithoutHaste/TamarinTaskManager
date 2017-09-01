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
		public string FullPath { get; set; }

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

		public Project()
		{
			ExcelPackage = new ExcelPackage();
			ExcelPackage.Workbook.Worksheets.Add("Config");
		}

		public Project(string fullPath)
		{
			FullPath = fullPath;
			FileInfo file = new FileInfo(FullPath);
			ExcelPackage = new ExcelPackage(file);
		}

		public void Save()
		{
			if(ExcelPackage.File == null)
			{
				if(FullPath == null)
					throw new Exception("Filename not set.");
				ExcelPackage.File = new FileInfo(FullPath);
			}
			ExcelPackage.Save();
		}

		public void Dispose()
		{
			ExcelPackage.Dispose();
		}
	}
}
