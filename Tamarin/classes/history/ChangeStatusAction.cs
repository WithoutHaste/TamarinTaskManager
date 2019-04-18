using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
	public class ChangeStatusAction : HistoryAction
	{
		public bool PreviousActiveSheet;
		public bool NewActiveSheet;
		public int PreviousRowNumber;
		public int NewRowNumber;
		public string PreviousStatus;
		public string NewStatus;
		public DateTime? PreviousDoneDate;
		public DateTime? NewDoneDate;

		public ChangeStatusAction(bool activeSheet, int rowNumber, string status, DateTime? doneDate)
		{
			PreviousActiveSheet = activeSheet;
			PreviousRowNumber = rowNumber;
			PreviousStatus = status;
			PreviousDoneDate = doneDate;
		}

		public void SetNewValues(bool activeSheet, int rowNumber, string status, DateTime? doneDate)
		{
			NewActiveSheet = activeSheet;
			NewRowNumber = rowNumber;
			NewStatus = status;
			NewDoneDate = doneDate;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualChangeTaskStatus(NewActiveSheet, NewRowNumber, PreviousActiveSheet, PreviousRowNumber, PreviousStatus, PreviousDoneDate);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualChangeTaskStatus(PreviousActiveSheet, PreviousRowNumber, NewActiveSheet, NewRowNumber, NewStatus, NewDoneDate);
		}
	}
}
