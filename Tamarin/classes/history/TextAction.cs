using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
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
}
