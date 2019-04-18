using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
	public class MoveAction : HistoryAction
	{
		public bool ActiveSheet;
		public int PreviousRowNumber;
		public int NewRowNumber;

		public MoveAction(bool activeSheet, int previousRowNumber, int newRowNumber)
		{
			ActiveSheet = activeSheet;
			PreviousRowNumber = previousRowNumber;
			NewRowNumber = newRowNumber;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualMoveTask(ActiveSheet, NewRowNumber, PreviousRowNumber);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualMoveTask(ActiveSheet, PreviousRowNumber, NewRowNumber);
		}
	}
}
