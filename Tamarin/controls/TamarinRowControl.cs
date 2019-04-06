using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	/// <summary>
	/// Either a header row or a task row.
	/// </summary>
	public abstract class TamarinRowControl : Panel
	{
		public event EventHandler AddRowBelow;

		/// <summary>Table-controlled row index. First data row is row 1.</summary>
		protected int rowIndex;
		public int RowIndex { get { return rowIndex; } }

		public static readonly int PLUS_COLUMN_INDEX = 0;
		public static readonly int ROW_COLUMN_INDEX = 1;
		public static readonly int ID_COLUMN_INDEX = 2;
		public static readonly int TITLE_COLUMN_INDEX = 3;
		public static readonly int STATUS_COLUMN_INDEX = 4;
		public static readonly int CATEGORY_COLUMN_INDEX = 5;
		public static readonly int CREATED_COLUMN_INDEX = 6;
		public static readonly int DONE_COLUMN_INDEX = 7;
		public static readonly int DELETE_COLUMN_INDEX = 8;
		public static readonly int HEADER_ROW_INDEX = 0;

		public TamarinRowControl(int rowIndex)
		{
			this.rowIndex = rowIndex;
		}

		public virtual void SetRowIndex(int rowIndex)
		{
			this.rowIndex = rowIndex;
		}

		public void Add_Click(object sender, EventArgs e)
		{
			if(AddRowBelow == null) return;
			AddRowBelow.Invoke(this, new EventArgs());
		}

		public void OnColumnWidthsChanged(object sender, EventArgs e)
		{
			//TODO
		}

		public void OnShowColumn(object sender, ShowColumnEventArgs e)
		{
			//TODO show or hide id column
		}

		protected void ArrangeControlsLeftToRight()
		{
			Control previousControl = null;
			foreach(Control control in this.Controls)
			{
				if(previousControl != null)
				{
					PlaceRightOf(previousControl, control);
				}
				previousControl = control;
			}
		}

		private void PlaceRightOf(Control left, Control right)
		{
			right.Location = new Point(left.Right, left.Top);
		}

		protected Button NewButton(string text, EventHandler onClickHandler)
		{
			Button button = new Button();
			button.Font = Settings.REGULAR_FONT;
			button.Location = new System.Drawing.Point(3, 3);
			button.AutoSize = true;
			button.TabStop = false;
			button.Text = text;
			button.UseVisualStyleBackColor = true;
			button.Margin = new Padding(0);
			button.Click += onClickHandler;
			return button;
		}

	}
}
