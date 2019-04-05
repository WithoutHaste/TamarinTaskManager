using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Tamarin
{
	public static class Utilities
	{
		public static void PassMouseWheelToParent(object sender, MouseEventArgs e)
		{
			Control parent = (sender as Control).Parent;
			System.Reflection.MethodInfo onMouseWheel = parent.GetType().GetMethod("OnMouseWheel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			onMouseWheel.Invoke(parent, new object[] { e });
		}

		public static string GetSemanticVersion(Assembly assembly)
		{
			Version version = GetVersion(assembly);
			return GetSemanticVersion(version);
		}

		/// <summary>
		/// Returns the version of the specified assembly.
		/// </summary>
		public static Version GetVersion(Assembly assembly)
		{
			return assembly.GetName().Version;
		}

		public static string GetSemanticVersion(Version version)
		{
			return String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
		}
	}
}
