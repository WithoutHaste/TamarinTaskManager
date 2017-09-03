using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		public DateTime? StatusChangeDate { get; set; }

		public string CreateDateString {
			get {
				return CreateDate.ToShortDateString();
			}
		}

		public string StatusChangeDateString {
			get {
				if(StatusChangeDate == null)
					return "";
				return StatusChangeDate.Value.ToShortDateString();
			}
		}

		public Task()
		{
		}

		public Task(ExcelWorksheet sheet, ColumnLayout columnLayout, int row)
		{
			Id = Int32.Parse(sheet.Cells[columnLayout.IdColumn + row].Value.ToString());
			Title = sheet.Cells[columnLayout.TitleColumn + row].Value.ToString();
			Status = sheet.Cells[columnLayout.StatusColumn + row].Value.ToString();
			Category = sheet.Cells[columnLayout.CategoryColumn + row].Value.ToString();
			CreateDate = DateTime.Parse(sheet.Cells[columnLayout.CreateDateColumn + row].Value.ToString());
			string statusChangeDate = sheet.Cells[columnLayout.StatusChangeDateColumn + row].Value.ToString();
			if(!String.IsNullOrEmpty(statusChangeDate))
			{
				StatusChangeDate = DateTime.Parse(statusChangeDate);
			}
		}
	}
}
