using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OfficeOpenXml;
using WithoutHaste.DataFiles.Excel;

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

		private string ConvertToString(object cellContents)
		{
			if(cellContents == null)
				return "";
			return cellContents.ToString();
		}
		
	}
}
