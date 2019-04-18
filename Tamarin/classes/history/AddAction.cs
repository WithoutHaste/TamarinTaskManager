using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
	public class AddAction : HistoryAction
	{
		public bool ActiveSheet;
		public int RowNumber;
		public int IdNumber;

		public AddAction(bool activeSheet, int rowNumber, int idNumber)
		{
			ActiveSheet = activeSheet;
			RowNumber = rowNumber;
			IdNumber = idNumber;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualDeleteTask(ActiveSheet, RowNumber, IdNumber);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualAddTask(ActiveSheet, RowNumber);
		}
	}
}
