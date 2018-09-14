using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OfficeOpenXml;
using WithoutHaste.DataFormats;

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

		public ColumnLayout(XmlNode tableNode)
		{
			List<string> headers = MSExcel2003XmlFormat.GetHeaders(tableNode);
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
			//TODO how to handle if some or all of expected headers are missing
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
				if((header == DESCRIPTION_HEADER || header == OLD_DESCRIPTION_HEADER) && DescriptionColumn == null)
				{
					DescriptionColumn = col.ToString();
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
			worksheet.Cells["B1"].Value = DESCRIPTION_HEADER;
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
			worksheet.Cells[DescriptionColumn + row].Value = task.Description;
			worksheet.Cells[StatusColumn + row].Value = task.Status;
			worksheet.Cells[CategoryColumn + row].Value = task.Category;
			worksheet.Cells[CreateDateColumn + row].Value = task.CreateDateString;
			if(!active)
			{
				worksheet.Cells[DoneDateColumn + row].Value = task.DoneDateString;
			}
		}

		public void WriteTaskHeaders(XmlDocument xmlDocument, XmlNode tableNode, bool isActive)
		{
			List<string> headers = GetHeaders(isActive);
			MSExcel2003XmlFormat.AddColumnWidths(xmlDocument, tableNode, headers.Select(h => GetHeaderWidth(h)).ToList());
			MSExcel2003XmlFormat.AddHeaderRowToTable(xmlDocument, tableNode, headers);
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

		public void WriteTask(XmlDocument xmlDocument, XmlNode tableNode, Task task, bool isActive)
		{
			List<XmlNode> cellNodes = new List<XmlNode>() {
				MSExcel2003XmlFormat.GenerateNumberCell(xmlDocument, task.Id),
				MSExcel2003XmlFormat.GenerateParagraphCell(xmlDocument, task.Description),
				MSExcel2003XmlFormat.GenerateTextCell(xmlDocument, task.Status),
				MSExcel2003XmlFormat.GenerateTextCell(xmlDocument, task.Category),
				MSExcel2003XmlFormat.GenerateDateCell(xmlDocument, task.CreateDate)
			};
			if(!isActive && task.DoneDate.HasValue)
			{
				cellNodes.Add(MSExcel2003XmlFormat.GenerateDateCell(xmlDocument, task.DoneDate.Value));
			}

			MSExcel2003XmlFormat.AddRowToTable(xmlDocument, tableNode, cellNodes);
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
