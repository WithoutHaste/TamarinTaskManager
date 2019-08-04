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
		/// <summary>Add a row below this one.</summary>
		public event EventHandler AddRowBelow;

		protected List<int> hiddenColumnIndexes = new List<int>();

		/// <summary>Table-controlled row index. First data row is row 1.</summary>
		protected int rowIndex;
		public int RowIndex { get { return rowIndex; } }

		public const int PLUS_COLUMN_INDEX = 0;
		public const int ROW_COLUMN_INDEX = 1;
		public const int ID_COLUMN_INDEX = 2;
		public const int TITLE_COLUMN_INDEX = 3;
		public const int STATUS_COLUMN_INDEX = 4;
		public const int CATEGORY_COLUMN_INDEX = 5;
		public const int CREATED_COLUMN_INDEX = 6;
		public const int DONE_COLUMN_INDEX = 7;
		public const int DELETE_COLUMN_INDEX = 8;
		public const int HEADER_ROW_INDEX = 0;

		public TamarinRowControl(int rowIndex, bool showColumnsForActive)
		{
			this.rowIndex = rowIndex;
			if(showColumnsForActive)
			{
				hiddenColumnIndexes.Add(DONE_COLUMN_INDEX);
			}
			this.Width = 300; //default width to start layout with - big enough for all controls to have room
			this.Height = (int)(Settings.TITLE_CHAR_HEIGHT * 2);
		}

		public void SyncHiddenColumns(TamarinRowControl other)
		{
			this.hiddenColumnIndexes.AddRange(other.hiddenColumnIndexes);
			SetupColumns(this.Controls);
		}

		public virtual void SetRowIndex(int rowIndex)
		{
			this.rowIndex = rowIndex;
		}

		#region Event Handlers

		public void OnAddButtonClick(object sender, EventArgs e)
		{
			InvokeAddRowBelow();
		}

		public void OnColumnWidthsChanged(object sender, EventArgs e)
		{
			//TODO
		}

		public void OnShowColumn(object sender, ShowColumnEventArgs e)
		{
			if(e.Show)
			{
				hiddenColumnIndexes.RemoveAll(element => element == e.ColumnIndex);
			}
			else
			{
				hiddenColumnIndexes.Add(e.ColumnIndex);
			}
			SetupColumns(this.Controls);
		}

		#endregion

		#region Invoke Events

		protected void InvokeAddRowBelow()
		{
			if(AddRowBelow == null) return;
			AddRowBelow.Invoke(this, new EventArgs());
		}

		#endregion

		protected void SetupColumns(ControlCollection controls)
		{
			Control add = controls[PLUS_COLUMN_INDEX];
			Control index = controls[ROW_COLUMN_INDEX];
			Control id = controls[ID_COLUMN_INDEX];
			Control title = controls[TITLE_COLUMN_INDEX];
			Control status = controls[STATUS_COLUMN_INDEX];
			Control category = controls[CATEGORY_COLUMN_INDEX];
			Control created = controls[CREATED_COLUMN_INDEX];
			Control finished = controls[DONE_COLUMN_INDEX];
			Control delete = controls[DELETE_COLUMN_INDEX];

			add.Location = new Point(0, 0);
			add.Size = new Size(Settings.TITLE_CHAR_WIDTH * 2, this.Height);
			add.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			index.Size = new Size(Settings.TITLE_CHAR_WIDTH * 4, this.Height);
			PlaceRightOf(add, index);
			PlaceCenterVerticalOn(add, index);
			index.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			int idWidth = (hiddenColumnIndexes.Contains(ID_COLUMN_INDEX)) ? 0 : Settings.TITLE_CHAR_WIDTH * 3;
			id.Size = new Size(idWidth, this.Height);
			PlaceRightOf(index, id);
			id.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			delete.Size = new Size(Settings.TITLE_CHAR_WIDTH * 2, this.Height);
			delete.Location = new Point(this.Width - delete.Width, 0);
			delete.Anchor = AnchorStyles.Right | AnchorStyles.Top;

			int finishedWidth = (hiddenColumnIndexes.Contains(DONE_COLUMN_INDEX)) ? 0 : Settings.TITLE_CHAR_WIDTH * 10;
			finished.Size = new Size(finishedWidth, this.Height);
			PlaceLeftOf(finished, delete);
			PlaceCenterVerticalOn(add, finished);
			finished.Anchor = AnchorStyles.Right | AnchorStyles.Top;

			created.Size = new Size(Settings.TITLE_CHAR_WIDTH * 10, this.Height);
			PlaceLeftOf(created, finished);
			PlaceCenterVerticalOn(add, created);
			created.Anchor = AnchorStyles.Right | AnchorStyles.Top;

			category.Size = new Size(Settings.TITLE_CHAR_WIDTH * 8, this.Height);
			PlaceLeftOf(category, created);
			PlaceCenterVerticalOn(add, category);
			category.Anchor = AnchorStyles.Right | AnchorStyles.Top;

			status.Size = new Size(Settings.TITLE_CHAR_WIDTH * 8, this.Height);
			PlaceLeftOf(status, category);
			PlaceCenterVerticalOn(add, status);
			status.Anchor = AnchorStyles.Right | AnchorStyles.Top;

			int titleMargin = 4;
			int remainingWidth = this.Width - add.Width - index.Width - id.Width - delete.Width - finished.Width - created.Width - category.Width - status.Width - (titleMargin * 2);

			title.Size = new Size(remainingWidth, this.Height);
			PlaceRightOf(id, title, titleMargin);
			title.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			var x = title.Font;
			var y = id.Font;
		}

		protected void PlaceRightOf(Control left, Control right, int margin = 0)
		{
			right.Location = new Point(left.Right + margin, left.Top);
		}

		protected void PlaceLeftOf(Control left, Control right)
		{
			left.Location = new Point(right.Left - left.Width, right.Top);
		}

		protected void PlaceCenterVerticalOn(Control reference, Control control)
		{
			int heightDifference = reference.Height - control.Height;
			control.Location = new Point(control.Left, reference.Top + (heightDifference / 2));
		}

		protected Button NewButton(string text, EventHandler onClickHandler)
		{
			Button button = new Button();
			button.Font = Settings.REGULAR_FONT;
			button.TabStop = false;
			button.Text = text;
			button.UseVisualStyleBackColor = true;
			button.Margin = new Padding(0);
			button.Click += onClickHandler;
			button.Invalidate();
			return button;
		}

	}
}
