using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OfficeOpenXml;

namespace TaskManagerX
{
	public class Task
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Status { get; set; }
		public string Category { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime? DoneDate { get; set; }

		public string CreateDateString {
			get {
				return CreateDate.ToShortDateString();
			}
		}

		public string DoneDateString {
			get {
				if(DoneDate == null)
					return "";
				return DoneDate.Value.ToShortDateString();
			}
		}

		public Task()
		{
		}

		public Task(XmlNode rowNode, ColumnLayout columnLayout)
		{
			char columnChar = 'A';
			foreach(XmlNode cellNode in rowNode.ChildNodes)
			{
				if(cellNode.LocalName != "Cell") continue;

				string cellValue = ColumnLayout.GetCellValue(cellNode);
				string header = columnLayout.GetHeaderByColumnChar(columnChar.ToString());
				switch(header)
				{
					case ColumnLayout.ID_HEADER: Id = Int32.Parse(cellValue); break;
					case ColumnLayout.TITLE_HEADER: Title = cellValue; break;
					case ColumnLayout.STATUS_HEADER: Status = cellValue; break;
					case ColumnLayout.CATEGORY_HEADER: Category = cellValue; break;
					case ColumnLayout.CREATE_DATE_HEADER: CreateDate = DateTime.Parse(cellValue); break;
					case ColumnLayout.DONE_DATE_HEADER: DoneDate = DateTime.Parse(cellValue); break;
				}

				columnChar++;
			}
		}

		public Task(ExcelWorksheet sheet, ColumnLayout columnLayout, int row)
		{
			Id = Int32.Parse(sheet.Cells[columnLayout.IdColumn + row].Value.ToString());
			Title = ConvertToString(sheet.Cells[columnLayout.TitleColumn + row].Value);
			Status = ConvertToString(sheet.Cells[columnLayout.StatusColumn + row].Value);
			Category = ConvertToString(sheet.Cells[columnLayout.CategoryColumn + row].Value);
			CreateDate = DateTime.Parse(sheet.Cells[columnLayout.CreateDateColumn + row].Value.ToString());
			if(columnLayout.DoneDateColumn != null)
			{
				string doneDate = sheet.Cells[columnLayout.DoneDateColumn + row].Value.ToString();
				if(!String.IsNullOrEmpty(doneDate))
				{
					DoneDate = DateTime.Parse(doneDate);
				}
			}
		}

		private string ConvertToString(object cellContents)
		{
			if(cellContents == null)
				return "";
			return cellContents.ToString();
		}
		
	}
}
