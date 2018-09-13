using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace TaskManagerX
{
	public class ColumnLayout
	{
		//expect top row to be title row
		//look for the expected columns

		public string IdColumn { get; set; }
		public string TitleColumn { get; set; }
		public string StatusColumn { get; set; }
		public string CategoryColumn { get; set; }
		public string CreateDateColumn { get; set; }
		public string DoneDateColumn { get; set; }

		public bool ValidLayout {
			get {
				return (!String.IsNullOrEmpty(IdColumn)
					&& !String.IsNullOrEmpty(TitleColumn)
					&& !String.IsNullOrEmpty(StatusColumn)
					&& !String.IsNullOrEmpty(CategoryColumn)
					&& !String.IsNullOrEmpty(CreateDateColumn));
			}
		}

		public static string ID_HEADER = "Id";
		public static string TITLE_HEADER = "Title";
		public static string STATUS_HEADER = "Status";
		public static string CATEGORY_HEADER = "Category";
		public static string CREATE_DATE_HEADER = "Created";
		public static string DONE_DATE_HEADER = "Done";

		public ColumnLayout(ExcelWorksheet sheet)
		{
			for(char col = 'A'; col <= 'Z'; col++)
			{
				if(sheet.Cells[col + "1"].Value == null)
					continue;
				string header = sheet.Cells[col + "1"].Value.ToString();
				if(header == ID_HEADER && IdColumn == null)
				{
					IdColumn = col.ToString();
					continue;
				}
				if(header == TITLE_HEADER && TitleColumn == null)
				{
					TitleColumn = col.ToString();
					continue;
				}
				if(header == STATUS_HEADER && StatusColumn == null)
				{
					StatusColumn = col.ToString();
					continue;
				}
				if(header == CATEGORY_HEADER && CategoryColumn == null)
				{
					CategoryColumn = col.ToString();
					continue;
				}
				if(header == CREATE_DATE_HEADER && CreateDateColumn == null)
				{
					CreateDateColumn = col.ToString();
					continue;
				}
				if(header == DONE_DATE_HEADER && DoneDateColumn == null)
				{
					DoneDateColumn = col.ToString();
					continue;
				}
			}
		}

		public static void WriteTaskHeaders(ExcelWorksheet worksheet, bool active)
		{
			worksheet.Cells["A1"].Value = ID_HEADER;
			worksheet.Cells["B1"].Value = TITLE_HEADER;
			worksheet.Cells["C1"].Value = STATUS_HEADER;
			worksheet.Cells["D1"].Value = CATEGORY_HEADER;
			worksheet.Cells["E1"].Value = CREATE_DATE_HEADER;
			if(!active)
			{
				worksheet.Cells["F1"].Value = DONE_DATE_HEADER;
			}
			worksheet.Cells["A1:F1"].Style.Font.Bold = true;
		}

		public void WriteTask(ExcelWorksheet worksheet, Task task, int row, bool active)
		{
			worksheet.Cells[IdColumn + row].Value = task.Id;
			worksheet.Cells[TitleColumn + row].Value = task.Title;
			worksheet.Cells[StatusColumn + row].Value = task.Status;
			worksheet.Cells[CategoryColumn + row].Value = task.Category;
			worksheet.Cells[CreateDateColumn + row].Value = task.CreateDateString;
			if(!active)
			{
				worksheet.Cells[DoneDateColumn + row].Value = task.DoneDateString;
			}
		}

	}
}
