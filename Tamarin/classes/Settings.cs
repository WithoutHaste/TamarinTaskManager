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
		public static readonly string SYMBOL_MULTIPLY = "\u00D7";
		public static readonly string SYMBOL_ADD = "+";

		//for some reason, Bold font is coming out smaller than Regular font of same size - fonts "Microsoft Sans Serif" and "Cambria"

		public static readonly Font TITLE_FONT = new System.Drawing.Font("Cambria", 13F, System.Drawing.FontStyle.Bold); //, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		public static readonly Font REGULAR_FONT = new System.Drawing.Font("Cambria", 14F, System.Drawing.FontStyle.Regular); //, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		public static readonly Font REGULAR_LABEL_FONT = new System.Drawing.Font("Cambria", 10F, System.Drawing.FontStyle.Regular); //, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

		private static int? titleCharWidth = null;
		private static int? titleCharHeight = null;
		private static void CalcTitleCharSize()
		{
			string text = "ABCDE";
			SizeF size = MeasureTextSize(TITLE_FONT, "ABCDE");
			titleCharWidth = (int)Math.Ceiling(size.Width / text.Length);
			titleCharHeight = (int)Math.Ceiling(size.Height);
	Console.WriteLine("title font size: " + size);
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
			SizeF size = MeasureTextSize(REGULAR_FONT, "ABCDE");
			regularCharWidth = (int)Math.Ceiling(size.Width / text.Length);
			regularCharHeight = (int)Math.Ceiling(size.Height);
	Console.WriteLine("regular font size: " + size);
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

		/// <remarks>
		/// If done within a Control object, using <c>this.CreateGraphics()</c> here somehow interrupts the sizing of the control, causing a layout error.
		/// </remarks>
		/// <remarks>
		/// Using <c>TextRenderer.MeasureText(text, font);</c> somehow gives randomly different answers.
		/// Like, I'll get height 22 for ages, then get height 33 in an unpredictable way.
		/// </remarks>
		private static SizeF MeasureTextSize(Font font, string text)
		{
			using(Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
			{
				SizeF size = graphics.MeasureString(text, font);
				float scaling = getScalingFactor();
				return new SizeF(size.Width * scaling, size.Height * scaling);
			}
		}

		#region GET SCALING

		/// <summary>
		/// Returns the amount of scaling Windows is forcing on your fonts.
		/// </summary>
		/// <remarks>
		/// https://stackoverflow.com/questions/5977445/how-to-get-windows-display-settings
		/// </remarks>
		/// <returns>1.25 means 125% scaling</returns>
		public static float getScalingFactor()
		{
			Graphics g = Graphics.FromHwnd(IntPtr.Zero);
			IntPtr desktop = g.GetHdc();
			int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
			int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

			float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

			return ScreenScalingFactor;
		}

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		public enum DeviceCap
		{
			VERTRES = 10,
			DESKTOPVERTRES = 117,

			// http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
		}

		#endregion
	}
}


