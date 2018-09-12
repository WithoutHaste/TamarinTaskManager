using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerX
{
	public class StatusChangeResult
	{
		public bool ActiveInactiveChanged { get; set; }
		public DateTime? DoneDate { get; set; }

		public string DoneDateString {
			get {
				if(DoneDate == null)
					return "";
				return DoneDate.Value.ToShortDateString();
			}
		}
	}
}
