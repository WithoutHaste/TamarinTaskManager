using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerX
{
	public interface IMergable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <returns>TRUE if merge was successful</returns>
		bool Merge(HistoryAction action);
	}
}
