using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarin
{
	public class Status
	{
		public string Name { get; set; }
		public bool Active { get; set; }

		public Status(string name, bool active)
		{
			Name = name;
			Active = active;
		}

		public Status(string name, string active)
		{
			Name = name;
			Active = (active == "Active");
		}
	}
}
