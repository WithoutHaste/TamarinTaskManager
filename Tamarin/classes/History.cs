using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerX
{
	public class History
	{
		private bool isActive = true;
		private Stack<HistoryAction> undoStack = new Stack<HistoryAction>();
		private Stack<HistoryAction> redoStack = new Stack<HistoryAction>();

		public bool CanUndo { get { return undoStack.Count > 0; } }
		public bool CanRedo { get { return redoStack.Count > 0; } }

		public History()
		{
			On();
		}

		public void On()
		{
			isActive = true;
		}

		public void Off()
		{
			isActive = false;
		}

		public void Add(HistoryAction action)
		{
			if(!isActive)
				return;

			if(!TryMerge(action))
			{
				undoStack.Push(action);
			}

			redoStack.Clear();
		}

		private bool TryMerge(HistoryAction action)
		{
			if(!(action is IMergable))
				return false;
			if(undoStack.Count == 0)
				return false;
			if(!(undoStack.Peek() is IMergable))
				return false;
			return (undoStack.Peek() as IMergable).Merge(action);
		}

		public HistoryAction Undo()
		{
			if(!CanUndo)
				throw new Exception("Cannot undo.");
			HistoryAction action = undoStack.Pop();
			redoStack.Push(action);
			return action;
		}

		public HistoryAction Redo()
		{
			if(!CanRedo)
				throw new Exception("Cannot redo.");
			HistoryAction action = redoStack.Pop();
			undoStack.Push(action);
			return action;
		}

		public void Clear()
		{
			undoStack.Clear();
			redoStack.Clear();
		}

	}
}
