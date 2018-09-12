using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManagerX;

namespace UnitTests
{
	[TestClass]
	public class TextExtensionTests_SharedStart
	{
		[TestMethod]
		public void SharedStart_NullA_ReturnNull()
		{
			string a = null;
			string b = "not null";
			string result = a.SharedStart(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedStart_NullB_ReturnNull()
		{
			string a = "not null";
			string b = null;
			string result = a.SharedStart(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedStart_EmptyA_ReturnNull()
		{
			string a = "";
			string b = "not null";
			string result = a.SharedStart(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedStart_EmptyB_ReturnNull()
		{
			string a = "not null";
			string b = "";
			string result = a.SharedStart(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedStart_NothingShared_ReturnNull()
		{
			string a = "abc";
			string b = "def";
			string result = a.SharedStart(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedStart_OneShared_ReturnStart()
		{
			string shared = "T";
			string a = shared + "abc";
			string b = shared + "def";
			string result = a.SharedStart(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedStart_TwoShared_ReturnStart()
		{
			string shared = "TH";
			string a = shared + "abc";
			string b = shared + "def";
			string result = a.SharedStart(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedStart_AllShared_ReturnStart()
		{
			string shared = "THasdkj8349 ASDHK";
			string a = shared;
			string b = shared;
			string result = a.SharedStart(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedStart_AllAShared_ReturnStart()
		{
			string shared = "THsd7 sjs";
			string a = shared;
			string b = shared + "def";
			string result = a.SharedStart(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedStart_AllBShared_ReturnStart()
		{
			string shared = "THsd7 sjs";
			string a = shared + "abc";
			string b = shared;
			string result = a.SharedStart(b);
			Assert.AreEqual(shared, result);
		}
	}

	[TestClass]
	public class TextExtensionTests_SharedEnd
	{
		[TestMethod]
		public void SharedEnd_NullA_ReturnNull()
		{
			string a = null;
			string b = "not null";
			string result = a.SharedEnd(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedEnd_NullB_ReturnNull()
		{
			string a = "not null";
			string b = null;
			string result = a.SharedEnd(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedEnd_EmptyA_ReturnNull()
		{
			string a = "";
			string b = "not null";
			string result = a.SharedEnd(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedEnd_EmptyB_ReturnNull()
		{
			string a = "not null";
			string b = "";
			string result = a.SharedEnd(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedEnd_NothingShared_ReturnNull()
		{
			string a = "abc";
			string b = "def";
			string result = a.SharedEnd(b);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void SharedEnd_OneShared_ReturnStart()
		{
			string shared = "T";
			string a = "abc" + shared;
			string b = "def" + shared;
			string result = a.SharedEnd(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedEnd_TwoShared_ReturnStart()
		{
			string shared = "TH";
			string a = "abc" + shared;
			string b = "def" + shared;
			string result = a.SharedEnd(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedEnd_AllShared_ReturnStart()
		{
			string shared = "THasdkj8349 ASDHK";
			string a = shared;
			string b = shared;
			string result = a.SharedEnd(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedEnd_AllAShared_ReturnStart()
		{
			string shared = "THsd7 sjs";
			string a = shared;
			string b = "def" + shared;
			string result = a.SharedEnd(b);
			Assert.AreEqual(shared, result);
		}

		[TestMethod]
		public void SharedEnd_AllBShared_ReturnStart()
		{
			string shared = "THsd7 sjs";
			string a = "abc" + shared;
			string b = shared;
			string result = a.SharedEnd(b);
			Assert.AreEqual(shared, result);
		}
	}

	[TestClass]
	public class TextExtensionTests_AddedOneChar
	{
		[TestMethod]
		public void AddedOneChar_BShorter_False()
		{
			string a = "abc";
			string b = "ab";
			int index;
			bool result = a.AddedOneChar(b, out index);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void AddedOneChar_BSameLength_False()
		{
			string a = "abc";
			string b = "abc";
			int index;
			bool result = a.AddedOneChar(b, out index);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void AddedOneChar_BTwoLonger_False()
		{
			string a = "abc";
			string b = "abcde";
			int index;
			bool result = a.AddedOneChar(b, out index);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void AddedOneChar_AtStart_True()
		{
			string a = "abc";
			string b = "Tabc";
			int index;
			bool result = a.AddedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(b.IndexOf("T"), index);
		}

		[TestMethod]
		public void AddedOneChar_AtOne_True()
		{
			string a = "abc";
			string b = "aTbc";
			int index;
			bool result = a.AddedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(b.IndexOf("T"), index);
		}

		[TestMethod]
		public void AddedOneChar_AtNegOne_True()
		{
			string a = "abc";
			string b = "abTc";
			int index;
			bool result = a.AddedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(b.IndexOf("T"), index);
		}

		[TestMethod]
		public void AddedOneChar_AtEnd_True()
		{
			string a = "abc";
			string b = "abcT";
			int index;
			bool result = a.AddedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(b.IndexOf("T"), index);
		}
	}

	[TestClass]
	public class TextExtensionTests_DeletedOneChar
	{
		[TestMethod]
		public void DeletedOneChar_BLonger_False()
		{
			string a = "ab";
			string b = "abc";
			int index;
			bool result = a.DeletedOneChar(b, out index);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void DeletedOneChar_BSameLength_False()
		{
			string a = "abc";
			string b = "abc";
			int index;
			bool result = a.DeletedOneChar(b, out index);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void DeletedOneChar_BTwoShorter_False()
		{
			string a = "abcde";
			string b = "abc";
			int index;
			bool result = a.DeletedOneChar(b, out index);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void DeletedOneChar_AtStart_True()
		{
			string a = "Tabc";
			string b = "abc";
			int index;
			bool result = a.DeletedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(a.IndexOf("T"), index);
		}

		[TestMethod]
		public void DeletedOneChar_AtOne_True()
		{
			string a = "aTbc";
			string b = "abc";
			int index;
			bool result = a.DeletedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(a.IndexOf("T"), index);
		}

		[TestMethod]
		public void DeletedOneChar_AtNegOne_True()
		{
			string a = "abTc";
			string b = "abc";
			int index;
			bool result = a.DeletedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(a.IndexOf("T"), index);
		}

		[TestMethod]
		public void DeletedOneChar_AtEnd_True()
		{
			string a = "abcT";
			string b = "abc";
			int index;
			bool result = a.DeletedOneChar(b, out index);
			Assert.IsTrue(result);
			Assert.AreEqual(a.IndexOf("T"), index);
		}
	}

	[TestClass]
	public class TextExtensionTests_ReplacedChars
	{
		[TestMethod]
		public void ReplacedChars_All_SameLength()
		{
			string a = "abc";
			string b = "def";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(0, selectionStartIndex);
			Assert.AreEqual(a.Length, selectionLength);
			Assert.AreEqual(b.Length, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_All_ALonger()
		{
			string a = "abccc";
			string b = "def";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(0, selectionStartIndex);
			Assert.AreEqual(a.Length, selectionLength);
			Assert.AreEqual(b.Length, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_All_BLonger()
		{
			string a = "abc";
			string b = "defff";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(0, selectionStartIndex);
			Assert.AreEqual(a.Length, selectionLength);
			Assert.AreEqual(b.Length, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_None()
		{
			string a = "abc";
			string b = "abc";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(3, selectionStartIndex);
			Assert.AreEqual(0, selectionLength);
			Assert.AreEqual(3, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_Beginning()
		{
			string a = "abcdef";
			string b = "TTdef";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(0, selectionStartIndex);
			Assert.AreEqual(3, selectionLength);
			Assert.AreEqual(2, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_End()
		{
			string a = "abcdef";
			string b = "abcTT";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(3, selectionStartIndex);
			Assert.AreEqual(3, selectionLength);
			Assert.AreEqual(5, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_Insert()
		{
			string a = "abcdef";
			string b = "abcTTTTTdef";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(3, selectionStartIndex);
			Assert.AreEqual(0, selectionLength);
			Assert.AreEqual(8, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_InsertLarger()
		{
			string a = "abcdef";
			string b = "abcTTTTTf";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(3, selectionStartIndex);
			Assert.AreEqual(2, selectionLength);
			Assert.AreEqual(8, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_InsertSmaller()
		{
			string a = "abcdef";
			string b = "abcTf";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(3, selectionStartIndex);
			Assert.AreEqual(2, selectionLength);
			Assert.AreEqual(4, replaceEndIndex);
		}

		[TestMethod]
		public void ReplacedChars_Delete()
		{
			string a = "abcdef";
			string b = "adef";
			int selectionStartIndex;
			int selectionLength;
			int replaceEndIndex;
			a.ReplacedChars(b, out selectionStartIndex, out selectionLength, out replaceEndIndex);
			Assert.AreEqual(1, selectionStartIndex);
			Assert.AreEqual(2, selectionLength);
			Assert.AreEqual(1, replaceEndIndex);
		}
	}
}
