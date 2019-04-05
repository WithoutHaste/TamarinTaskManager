using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	public class StatusComboBox : TamarinComboBox
	{
		public StatusComboBox(List<string> options, string selectedOption) : base("StatusComboBox", options, selectedOption)
		{
		}
	}
}
