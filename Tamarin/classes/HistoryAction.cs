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

	public class AddAction : HistoryAction
	{
		public bool ActiveSheet;
		public int RowNumber;

		public AddAction(bool activeSheet, int rowNumber)
		{
			ActiveSheet = activeSheet;
			RowNumber = rowNumber;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualDeleteTask(ActiveSheet, RowNumber);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualAddTask(ActiveSheet, RowNumber);
		}
	}

	public class DeleteAction : HistoryAction
	{
		public bool ActiveSheet;
		public int RowNumber;
		public Task Task;

		public DeleteAction(bool activeSheet, int rowNumber, Task task)
		{
			ActiveSheet = activeSheet;
			RowNumber = rowNumber;
			Task = task;
		}

		public override void Undo(TaskTableControl control)
		{
			control.ManualAddTask(ActiveSheet, RowNumber, Task);
		}

		public override void Redo(TaskTableControl control)
		{
			control.ManualDeleteTask(ActiveSheet, RowNumber);
		}
	}

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

	public class TextAction : HistoryAction, IMergable
	{
		public bool ActiveSheet;
		public int RowNumber;
		public string PreviousText;
		public string NewText;

		private bool isTyping = false;
		private bool isDeleting = false;

		private int typingAtIndex = -1;
		private int deletingAtIndex = -1;
		private int deleteStartedAtIndex = -1;

		private int selectionStartIndex = -1;
		private int selectionLength = -1;
		private int replaceEndIndex = -1;

		public TextAction(bool activeSheet, int rowNumber, string previousText, string newText)
		{
			ActiveSheet = activeSheet;
			RowNumber = rowNumber;
			PreviousText = previousText;
			NewText = newText;

			if(previousText.AddedOneChar(newText, out typingAtIndex))
			{
				isTyping = true;
			}
			else if(previousText.DeletedOneChar(newText, out deletingAtIndex))
			{
				isDeleting = true;
				deleteStartedAtIndex = deletingAtIndex;
			}
			else
			{
				previousText.ReplacedChars(newText, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			}
		}

		public bool Merge(HistoryAction action)
		{
			if(!(action is TextAction))
				return false;
			TextAction newAction = (action as TextAction);
			if(this.ActiveSheet != newAction.ActiveSheet)
				return false;
			if(this.RowNumber != newAction.RowNumber)
				return false;

			if(this.isTyping && newAction.isTyping 
				&& this.typingAtIndex + 1 == newAction.typingAtIndex //typing continues from same position
				&& this.NewText[typingAtIndex] != ' ') //break action at space
			{
				this.NewText = newAction.NewText;
				this.typingAtIndex = newAction.typingAtIndex;
				return true;
			}

			if(this.isDeleting && newAction.isDeleting
				&& this.deletingAtIndex - 1 == newAction.deletingAtIndex //deleting continues from same position
				&& this.PreviousText[deletingAtIndex] != ' ') //break action at space
			{
				this.NewText = newAction.NewText;
				this.deletingAtIndex = newAction.deletingAtIndex;
				return true;
			}

			return false;
		}

		public override void Undo(TaskTableControl control)
		{
			int caret = -1;
			int selectionLength = 0;

			if(isTyping) caret = typingAtIndex;
			else if(isDeleting) caret = deleteStartedAtIndex + 1;
			else
			{
				caret = selectionStartIndex;
				selectionLength = this.selectionLength;
			}

			control.ManualTextChange(ActiveSheet, RowNumber, PreviousText, caret, selectionLength);
		}

		public override void Redo(TaskTableControl control)
		{
			int caret = -1;
			int selectionLength = 0;

			if(isTyping) caret = typingAtIndex + 1;
			else if(isDeleting) caret = deletingAtIndex;
			else
			{
				caret = replaceEndIndex;
				selectionLength = 0;
			}

			control.ManualTextChange(ActiveSheet, RowNumber, NewText, caret, selectionLength);
		}
	}

	public class MultipleAction : HistoryAction
	{
		public List<HistoryAction> Actions;

		public MultipleAction()
		{
			Actions = new List<HistoryAction>();
		}

		public void AddAction(DeleteAction action)
		{
			Actions.Add(action);
		}

		public override void Undo(TaskTableControl control)
		{
			for(int i = Actions.Count - 1; i >= 0; i--)
			{
				Actions[i].Undo(control);
			}
		}

		public override void Redo(TaskTableControl control)
		{
			for(int i = 0; i < Actions.Count; i++)
			{
				Actions[i].Redo(control);
			}
		}
	}
}
