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
		public string PathAndFilename { get; set; }

		public string Name {
			get {
				if (PathAndFilename == null)
					return "new";
				return Path.GetFileNameWithoutExtension(PathAndFilename);
			}
		}

		public bool NotNamed {
			get {
				return (PathAndFilename == null);
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
			PathAndFilename = fullPath;
			FileInfo file = new FileInfo(PathAndFilename);
			ExcelPackage = new ExcelPackage(file);
		}

		public void Save()
		{
			if(ExcelPackage.File == null)
			{
				if(PathAndFilename == null)
					throw new Exception("Filename not set.");
				ExcelPackage.File = new FileInfo(PathAndFilename);
			}
			ExcelPackage.Save();
		}

		public void Dispose()
		{
			ExcelPackage.Dispose();
		}
	}
}
