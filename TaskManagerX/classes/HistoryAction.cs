using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerX
{
	public class HistoryAction
	{
		public virtual void Undo(TaskTableControl control) { }
		public virtual void Redo(TaskTableControl control) { }
	}

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

	public class ChangeStatusAction : HistoryAction
	{
		public bool PreviousActiveSheet;
		public bool NewActiveSheet;
		public int PreviousRowNumber;
		public int NewRowNumber;
		public string PreviousStatus;
		public string NewStatus;

		public ChangeStatusAction(bool activeSheet, int rowNumber, string status)
		{
			PreviousActiveSheet = activeSheet;
			PreviousRowNumber = rowNumber;
			PreviousStatus = status;
		}

		public void SetNew(bool activeSheet, int rowNumber, string status)
		{
			NewActiveSheet = activeSheet;
			NewRowNumber = rowNumber;
			NewStatus = status;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualChangeTaskStatus(NewActiveSheet, NewRowNumber, PreviousActiveSheet, PreviousRowNumber, PreviousStatus);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualChangeTaskStatus(PreviousActiveSheet, PreviousRowNumber, NewActiveSheet, NewRowNumber, NewStatus);
		}
	}
}
