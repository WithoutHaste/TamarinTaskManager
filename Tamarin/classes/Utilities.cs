using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tamarin
{
	public static class Utilities
	{
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
