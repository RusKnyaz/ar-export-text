using System.Linq;
using NUnit.Framework;
using TextExport;

namespace TextExportTests
{
	[TestFixture]
	public class TextLayoutTests
	{
		[TestCase("ABC", ExpectedResult = "ABC")]
		[TestCase("ABCDEFGH", ExpectedResult = "ABCDE\nFGH")]
		[TestCase("AB\r\nCD\n123456", ExpectedResult = "AB\nCD\n12345\n6")]
		[TestCase("a\r\n\r\nb", ExpectedResult = "a\n\nb")]
		public string CharWrapTests(string text) => 
			string.Join("\n", TextLayout.CharWrap(text, 5).Select(x => text.Substring(x.StartIndex, x.Length)));
		
		[TestCase("ABCDEFG", ExpectedResult = "ABCDEFG")]
		[TestCase("ABCD EFG", ExpectedResult = "ABCD\nEFG")]
		[TestCase("ABCDE FG", ExpectedResult = "ABCDE\nFG")]
		public string WordWrapTests(string text) => 
			string.Join("\n", TextLayout.WordWrap(text, 5).Select(x => text.Substring(x.StartIndex, x.Length)));
		
	}
}