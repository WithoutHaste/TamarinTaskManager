using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerX
{
	public static class IndexConverter
	{
		//TableLayoutPanel has 0-based row index

		//ExcelWorksheet has 1-based row index

		public static int TableLayoutPanelToExcelWorksheet(int index)
		{
			return index + 1;
		}

		public static int ExcelWorksheetToTableLayoutPanel(int index)
		{
			return index - 1;
		}
	}
}
