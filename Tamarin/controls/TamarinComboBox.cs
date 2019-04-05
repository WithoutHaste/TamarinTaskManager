using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	public abstract class TamarinComboBox : ComboBox
	{
		public TamarinComboBox(string controlName, List<string> options, string selectedOption)
		{
			this.Font = Settings.REGULAR_FONT;
			this.FormattingEnabled = true;
			this.Name = controlName;
			this.DropDownStyle = ComboBoxStyle.DropDownList; // so that it doesn't get a blue highlight
			this.Size = new System.Drawing.Size(94, 24);
			this.Margin = new Padding(0);
			this.TabStop = false;

			UpdateOptions(options, selectedOption);

			this.MouseWheel += new MouseEventHandler(Utilities.PassMouseWheelToParent);
		}

		public void UpdateOptions(List<string> options)
		{
			UpdateOptions(options, this.SelectedItem.ToString());
		}

		private void UpdateOptions(List<string> options, string selectedOption)
		{
			this.Items.Clear();

			if(!String.IsNullOrEmpty(selectedOption) && !options.Contains(selectedOption))
				options.Add(selectedOption);
			foreach(string option in options) //when using datasource, could not set the selected item
			{
				this.Items.Add(option);
			}

			if(!String.IsNullOrEmpty(selectedOption))
			{
				this.SelectedIndex = this.FindStringExact(selectedOption);
			}
		}
	}
}
