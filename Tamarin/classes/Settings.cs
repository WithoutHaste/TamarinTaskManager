using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	public static class Settings
	{
		//for some reason, Bold font is coming out smaller than Regular font of same size - fonts "Microsoft Sans Serif" and "Cambria"

		public static readonly Font TITLE_FONT = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		public static readonly Font REGULAR_FONT = new System.Drawing.Font("Cambria", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

		private static int? titleCharWidth = null;
		private static int? titleCharHeight = null;
		private static void CalcTitleCharSize()
		{
			string text = "ABCDE";
			Size size = TextRenderer.MeasureText(text, TITLE_FONT);
			titleCharWidth = size.Width / text.Length;
			titleCharHeight = size.Height;
		}
		public static int TITLE_CHAR_WIDTH {
			get {
				if(titleCharWidth == null)
					CalcTitleCharSize();
				return titleCharWidth.Value;
			}
		}
		public static int TITLE_CHAR_HEIGHT {
			get {
				if(titleCharHeight == null)
					CalcTitleCharSize();
				return titleCharHeight.Value;
			}
		}

		private static int? regularCharWidth = null;
		private static int? regularCharHeight = null;
		private static void CalcRegularCharSize()
		{
			string text = "ABCDE";
			Size size = TextRenderer.MeasureText(text, REGULAR_FONT);
			regularCharWidth = size.Width / text.Length;
			regularCharHeight = size.Height;
		}
		public static int REGULAR_CHAR_WIDTH {
			get {
				if(regularCharWidth == null)
					CalcRegularCharSize();
				return regularCharWidth.Value;
			}
		}
		public static int REGULAR_CHAR_HEIGHT {
			get {
				if(regularCharHeight == null)
					CalcRegularCharSize();
				return regularCharHeight.Value;
			}
		}

	}
}
