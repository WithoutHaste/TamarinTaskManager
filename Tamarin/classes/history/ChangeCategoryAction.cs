using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
	public class ChangeCategoryAction : HistoryAction
	{
		public bool ActiveSheet;
		public int RowNumber;
		public string PreviousCategory;
		public string NewCategory;

		public ChangeCategoryAction(bool activeSheet, int rowNumber, string previousCategory, string newCategory)
		{
			ActiveSheet = activeSheet;
			RowNumber = rowNumber;
			PreviousCategory = previousCategory;
			NewCategory = newCategory;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualChangeTaskCategory(ActiveSheet, RowNumber, PreviousCategory);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualChangeTaskCategory(ActiveSheet, RowNumber, NewCategory);
		}
	}
}
