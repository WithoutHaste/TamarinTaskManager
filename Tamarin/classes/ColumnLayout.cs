﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

		public ColumnLayout()
		{
			IdColumn = "A";
			TitleColumn = "B";
			StatusColumn = "C";
			CategoryColumn = "D";
			CreateDateColumn = "E";
			DoneDateColumn = "F";
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

		public void WriteTaskHeaders(XmlDocument xml, XmlNode tableNode, bool active, string rootNamespace, string headerStyleId)
		{
			XmlNode rowNode = xml.CreateElement("Row", rootNamespace);
			tableNode.AppendChild(rowNode);

			List<string> headers = new List<string> { ID_HEADER, TITLE_HEADER, STATUS_HEADER, CATEGORY_HEADER, CREATE_DATE_HEADER, DONE_DATE_HEADER };
			if(active)
			{
				headers.Remove(DONE_DATE_HEADER);
			}
			foreach(string header in headers)
			{
				int width = 60;
				if(header == TITLE_HEADER) width = 300;
				else if(header == ID_HEADER) width = 25;

				XmlNode columnNode = xml.CreateElement("Column", rootNamespace);
				XmlAttribute columnAttribute = xml.CreateAttribute("ss", "Width", "urn:schemas-microsoft-com:office:spreadsheet");
				columnAttribute.Value = width.ToString();
				columnNode.Attributes.Append(columnAttribute);
				tableNode.AppendChild(columnNode);
			}
			foreach(string header in headers)
			{
				rowNode.AppendChild(GenerateStringCell(xml, header, rootNamespace, headerStyleId));
			}
		}

		public void WriteTask(XmlDocument xml, XmlNode tableNode, Task task, bool active, string rootNamespace, string shortDateStyleId, string paragraphStyleId)
		{
			XmlNode rowNode = xml.CreateElement("Row", rootNamespace);
			tableNode.AppendChild(rowNode);

			rowNode.AppendChild(GenerateNumberCell(xml, task.Id, rootNamespace));
			rowNode.AppendChild(GenerateStringCell(xml, task.Title, rootNamespace, paragraphStyleId));
			rowNode.AppendChild(GenerateStringCell(xml, task.Status, rootNamespace));
			rowNode.AppendChild(GenerateStringCell(xml, task.Category, rootNamespace));
			rowNode.AppendChild(GenerateDateCell(xml, task.CreateDate, rootNamespace, shortDateStyleId));
			if(!active && task.DoneDate.HasValue)
			{
				rowNode.AppendChild(GenerateDateCell(xml, task.DoneDate.Value, rootNamespace, shortDateStyleId));
			}
		}

		public static XmlNode GenerateStringCell(XmlDocument xml, string data, string rootNamespace, string styleId = null)
		{
			XmlNode cellNode = xml.CreateElement("Cell", rootNamespace);
			if(styleId != null)
			{
				XmlAttribute cellAttribute = xml.CreateAttribute("ss", "StyleID", "urn:schemas-microsoft-com:office:spreadsheet");
				cellAttribute.Value = styleId;
				cellNode.Attributes.Append(cellAttribute);
			}

			XmlNode dataNode = xml.CreateElement("Data", rootNamespace);
			XmlAttribute typeAttribute = xml.CreateAttribute("ss", "Type", "urn:schemas-microsoft-com:office:spreadsheet");
			typeAttribute.Value = "String";
			dataNode.Attributes.Append(typeAttribute);
			dataNode.InnerText = data;

			cellNode.AppendChild(dataNode);

			return cellNode;
		}

		public static XmlNode GenerateNumberCell(XmlDocument xml, int data, string rootNamespace)
		{
			XmlNode cellNode = xml.CreateElement("Cell", rootNamespace);

			XmlNode dataNode = xml.CreateElement("Data", rootNamespace);
			XmlAttribute typeAttribute = xml.CreateAttribute("ss", "Type", "urn:schemas-microsoft-com:office:spreadsheet");
			typeAttribute.Value = "Number";
			dataNode.Attributes.Append(typeAttribute);
			dataNode.InnerText = data.ToString();

			cellNode.AppendChild(dataNode);

			return cellNode;
		}

		public static XmlNode GenerateDateCell(XmlDocument xml, DateTime data, string rootNamespace, string shortDateStyleId)
		{
			XmlNode cellNode = xml.CreateElement("Cell", rootNamespace);
			XmlAttribute cellAttribute = xml.CreateAttribute("ss", "StyleID", "urn:schemas-microsoft-com:office:spreadsheet");
			cellAttribute.Value = shortDateStyleId;
			cellNode.Attributes.Append(cellAttribute);

			XmlNode dataNode = xml.CreateElement("Data", rootNamespace);
			XmlAttribute typeAttribute = xml.CreateAttribute("ss", "Type", "urn:schemas-microsoft-com:office:spreadsheet");
			typeAttribute.Value = "DateTime";
			dataNode.Attributes.Append(typeAttribute);
			dataNode.InnerText = data.ToString("yyyy-MM-ddT00:00:00.000");

			cellNode.AppendChild(dataNode);

			return cellNode;
		}
	}
}
