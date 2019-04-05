using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tamarin
{
	/// <summary>
	/// From https://stackoverflow.com/questions/8900099/tablelayoutpanel-responds-very-slowly-to-events
	/// The goal is to speed up display of panel when the controls change or the panel size changes.
	/// </summary>
	public class CoTableLayoutPanel : TableLayoutPanel
	{
		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.CacheText, true);
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= NativeMethods.WS_EX_COMPOSITED;
				return cp;
			}
		}

		public void BeginUpdate()
		{
			NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
		}

		public void EndUpdate()
		{
			NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
			Parent.Invalidate(true);
		}

		#region Custom

		private int SuspendLayoutCount = 0;

		protected void RequestSuspendLayout([CallerMemberName] string name="")
		{
			this.SuspendLayout();
			SuspendLayoutCount++;
			Console.WriteLine("Suspend layout: {0}: count {1}", name, SuspendLayoutCount);
		}

		protected void RequestResumeLayout([CallerMemberName] string name="")
		{
			SuspendLayoutCount--;
			//Console.WriteLine("Maybe Resume layout: {0}: count {1}", name, SuspendLayoutCount);
			//if(SuspendLayoutCount <= 0)
			//{
				Console.WriteLine("Resume layout: {0}: count {1}", name, SuspendLayoutCount);
				//Console.WriteLine("Panel width: {0}", this.Width);
				this.ResumeLayout();
			//}
		}

		#endregion
	}

	public static class NativeMethods
	{
		public static int WM_SETREDRAW = 0x000B; //uint WM_SETREDRAW
		public static int WS_EX_COMPOSITED = 0x02000000;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam); //UInt32 Msg
	}
}
