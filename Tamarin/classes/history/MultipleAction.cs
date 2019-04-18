using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
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
