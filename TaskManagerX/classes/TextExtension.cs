using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerX
{
	public static class TextExtension
	{
		public static string SharedStart(this string a, string b)
		{
			if(String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b))
				return null;

			int i = 0;
			for(i = 0; i < a.Length && i < b.Length; i++)
			{
				if(a[i] != b[i])
					break;
			}

			if(i == 0)
				return null;
			return a.Substring(0, i);
		}

		public static string SharedEnd(this string a, string b)
		{
			if(String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b))
				return null;

			int i = 1;
			for(i = 1; i <= a.Length && i <= b.Length; i++)
			{
				if(a[a.Length - i] != b[b.Length - i])
					break;
			}

			if(i == 1)
				return null;
			i--;
			return a.Substring(a.Length - i, i);
		}

		public static void DifferentMiddles(this string a, string b, out int index, out string aMiddle, out string bMiddle)
		{
			index = 0;
			aMiddle = String.Copy(a);
			bMiddle = String.Copy(b);
			string sharedStart = aMiddle.SharedStart(bMiddle);
			if(sharedStart != null)
			{
				index = sharedStart.Length;
				aMiddle = aMiddle.Remove(0, sharedStart.Length);
				bMiddle = bMiddle.Remove(0, sharedStart.Length);
			}
			string sharedEnd = aMiddle.SharedEnd(bMiddle);
			if(sharedEnd != null)
			{
				aMiddle = aMiddle.Remove(aMiddle.Length - sharedEnd.Length, sharedEnd.Length);
				bMiddle = bMiddle.Remove(bMiddle.Length - sharedEnd.Length, sharedEnd.Length);
			}
		}

		public static bool AddedOneChar(this string a, string b, out int index)
		{
			index = -1;
			if(a.Length != b.Length - 1)
				return false;
			string aMiddle;
			string bMiddle;
			a.DifferentMiddles(b, out index, out aMiddle, out bMiddle);
			if(aMiddle.Length != 0)
				return false;
			if(bMiddle.Length != 1)
				return false;
			return true;
		}

		public static bool DeletedOneChar(this string a, string b, out int index)
		{
			index = -1;
			if(a.Length - 1 != b.Length)
				return false;
			string aMiddle;
			string bMiddle;
			a.DifferentMiddles(b, out index, out aMiddle, out bMiddle);
			if(aMiddle.Length != 1)
				return false;
			if(bMiddle.Length != 0)
				return false;
			return true;
		}

		public static void ReplacedChars(this string a, string b, out int selectionStartIndex, out int selectionLength, out int replaceEndIndex)
		{
			string aMiddle;
			string bMiddle;
			a.DifferentMiddles(b, out selectionStartIndex, out aMiddle, out bMiddle);
			selectionLength = aMiddle.Length;
			replaceEndIndex = selectionStartIndex + bMiddle.Length;
		}
	}
}
