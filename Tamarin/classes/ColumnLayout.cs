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
	public class ColumnLayout
	{
		//expect top row to be title row
		//look for the expected columns

		public string IdColumn { get; set; }
		public string DescriptionColumn { get; set; }
		public string StatusColumn { get; set; }
		public string CategoryColumn { get; set; }
		public string CreateDateColumn { get; set; }
		public string DoneDateColumn { get; set; }

		public bool ValidLayout {
			get {
				return (!String.IsNullOrEmpty(IdColumn)
					&& !String.IsNullOrEmpty(DescriptionColumn)
					&& !String.IsNullOrEmpty(StatusColumn)
					&& !String.IsNullOrEmpty(CategoryColumn)
					&& !String.IsNullOrEmpty(CreateDateColumn));
			}
		}

		public const string ID_HEADER = "Id";
		public const string DESCRIPTION_HEADER = "Description";
		public const string OLD_DESCRIPTION_HEADER = "Title";
		public const string STATUS_HEADER = "Status";
		public const string CATEGORY_HEADER = "Category";
		public const string CREATE_DATE_HEADER = "Created";
		public const string DONE_DATE_HEADER = "Done";

		public ColumnLayout()
		{
			IdColumn = "A";
			DescriptionColumn = "B";
			StatusColumn = "C";
			CategoryColumn = "D";
			CreateDateColumn = "E";
			DoneDateColumn = "F";
		}

		public ColumnLayout(List<string> headers)
		{
			char columnChar = 'A';
			foreach(string header in headers)
			{
				switch(header)
				{
					case ID_HEADER: IdColumn = columnChar.ToString(); break;
					case OLD_DESCRIPTION_HEADER:
					case DESCRIPTION_HEADER: DescriptionColumn = columnChar.ToString(); break;
					case STATUS_HEADER: StatusColumn = columnChar.ToString(); break;
					case CATEGORY_HEADER: CategoryColumn = columnChar.ToString(); break;
					case CREATE_DATE_HEADER: CreateDateColumn = columnChar.ToString(); break;
					case DONE_DATE_HEADER: DoneDateColumn = columnChar.ToString(); break;
				}
				columnChar++;
			}
		}

		public ColumnLayout(ExcelWorksheet sheet)
		{
			IdColumn = ExcelPackageHelper.GetColumnCharForHeader(sheet, ID_HEADER);
			DescriptionColumn = ExcelPackageHelper.GetColumnCharForHeader(sheet, DESCRIPTION_HEADER);
			if(DescriptionColumn == null)
				DescriptionColumn = ExcelPackageHelper.GetColumnCharForHeader(sheet, OLD_DESCRIPTION_HEADER);
			StatusColumn = ExcelPackageHelper.GetColumnCharForHeader(sheet, STATUS_HEADER);
			CategoryColumn = ExcelPackageHelper.GetColumnCharForHeader(sheet, CATEGORY_HEADER);
			CreateDateColumn = ExcelPackageHelper.GetColumnCharForHeader(sheet, CREATE_DATE_HEADER);
			DoneDateColumn = ExcelPackageHelper.GetColumnCharForHeader(sheet, DONE_DATE_HEADER);
		}

		public static void WriteTaskHeaders(ExcelWorksheet worksheet, bool active)
		{
			List<object> values = new List<object>() {
				ID_HEADER,
				DESCRIPTION_HEADER,
				STATUS_HEADER,
				CATEGORY_HEADER,
				CREATE_DATE_HEADER
			};
			if(!active) values.Add(DONE_DATE_HEADER);

			ExcelPackageHelper.AppendRow(worksheet, values);
			worksheet.Cells["A1:F1"].Style.Font.Bold = true;
		}

		public void WriteTask(ExcelWorksheet worksheet, Task task, bool active)
		{
			List<object> values = new List<object>() {
				task.Id,
				task.Description,
				task.Status,
				task.Category,
				task.CreateDateString
			};
			if(!active) values.Add(task.DoneDateString);

			ExcelPackageHelper.AppendRow(worksheet, values);
		}

		public void WriteTaskHeaders(MSExcel2003XmlFile xmlFile, int tableIndex, bool isActive)
		{
			List<string> headers = GetHeaders(isActive);
			xmlFile.SetColumnWidths(tableIndex, headers.Select(h => GetHeaderWidth(h)).ToList());
			xmlFile.AddHeaderRow(tableIndex, headers);
		}

		private int GetHeaderWidth(string header)
		{
			if(header == ID_HEADER) return 25;
			if(header == DESCRIPTION_HEADER) return 300;
			return 60;
		}

		private List<string> GetHeaders(bool isActive)
		{
			List<string> headers = new List<string> { ID_HEADER, DESCRIPTION_HEADER, STATUS_HEADER, CATEGORY_HEADER, CREATE_DATE_HEADER, DONE_DATE_HEADER };
			if(isActive)
			{
				headers.Remove(DONE_DATE_HEADER);
			}
			return headers;
		}

		public void WriteTask(MSExcel2003XmlFile xmlFile, int tableIndex, Task task, bool isActive)
		{
			List<XmlNode> cellNodes = new List<XmlNode>() {
				xmlFile.GenerateNumberCell(task.Id),
				xmlFile.GenerateParagraphCell(task.Description),
				xmlFile.GenerateTextCell(task.Status),
				xmlFile.GenerateTextCell(task.Category),
				xmlFile.GenerateDateCell(task.CreateDate)
			};
			if(!isActive && task.DoneDate.HasValue)
			{
				cellNodes.Add(xmlFile.GenerateDateCell(task.DoneDate.Value));
			}

			xmlFile.AddRow(tableIndex, cellNodes);
		}

		public string GetHeaderByColumnChar(string columnChar)
		{
			if(columnChar == IdColumn) return ID_HEADER;
			if(columnChar == DescriptionColumn) return DESCRIPTION_HEADER;
			if(columnChar == StatusColumn) return STATUS_HEADER;
			if(columnChar == CategoryColumn) return CATEGORY_HEADER;
			if(columnChar == CreateDateColumn) return CREATE_DATE_HEADER;
			if(columnChar == DoneDateColumn) return DONE_DATE_HEADER;
			return null;
		}
	}
}
