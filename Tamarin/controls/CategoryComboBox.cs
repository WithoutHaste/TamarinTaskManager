using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	public class CategoryComboBox : TamarinComboBox
	{
		public CategoryComboBox(List<string> options, string selectedOption) : base("CategoryComboBox", options, selectedOption)
		{
		}
	}
}
