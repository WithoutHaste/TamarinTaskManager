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
	public class Task
	{
		public int Id { get; set; }
		public string Description { get; set; }
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

		public Task(List<string> values, ColumnLayout columnLayout)
		{
			char columnChar = 'A';
			foreach(string value in values)
			{
				string header = columnLayout.GetHeaderByColumnChar(columnChar.ToString());
				switch(header)
				{
					case ColumnLayout.ID_HEADER: Id = Int32.Parse(value); break;
					case ColumnLayout.DESCRIPTION_HEADER: Description = value; break;
					case ColumnLayout.STATUS_HEADER: Status = value; break;
					case ColumnLayout.CATEGORY_HEADER: Category = value; break;
					case ColumnLayout.CREATE_DATE_HEADER: CreateDate = DateTime.Parse(value); break;
					case ColumnLayout.DONE_DATE_HEADER: DoneDate = DateTime.Parse(value); break;
				}
				columnChar++;
			}
		}

		public Task(ExcelWorksheet sheet, ColumnLayout columnLayout, int row)
		{
			Id = Int32.Parse(sheet.Cells[columnLayout.IdColumn + row].Value.ToString());
			Description = ConvertToString(sheet.Cells[columnLayout.DescriptionColumn + row].Value);
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
