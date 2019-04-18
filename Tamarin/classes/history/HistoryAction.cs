using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
	public class HistoryAction
	{
		public virtual void Undo(TaskTableControl control) { }
		public virtual void Redo(TaskTableControl control) { }
	}
}
