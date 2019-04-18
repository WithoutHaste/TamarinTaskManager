using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
	public class DeleteAction : HistoryAction
	{
		public bool ActiveSheet;
		public int RowNumber;
		public int IdNumber;
		public Task Task;

		public DeleteAction(bool activeSheet, int rowNumber, int idNumber, Task task)
		{
			ActiveSheet = activeSheet;
			RowNumber = rowNumber;
			IdNumber = idNumber;
			Task = task;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualAddTask(ActiveSheet, RowNumber, Task);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualDeleteTask(ActiveSheet, RowNumber, IdNumber);
		}
	}
}
