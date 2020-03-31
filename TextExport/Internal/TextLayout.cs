using System;
using System.Collections.Generic;
using GrapeCity.ActiveReports.Drawing;

namespace TextExport
{
	internal static class TextLayout
	{
		public static IEnumerable<Line> SplitLines(string txt, StringFormatEx format, int width)
		{
			switch (format.WrapMode)
			{
				case WrapMode.NoWrap:
					return CharWrap(txt, int.MaxValue);
				case WrapMode.CharWrap:
					return CharWrap(txt, width);
				case WrapMode.WordWrap:
					return WordWrap(txt, width);
				default:
					throw new ArgumentOutOfRangeException("format.WrapMode");
			}
		}

		internal static IEnumerable<Line> CharWrap(string txt, int width)
		{
			if(width == 0)
				throw new ArgumentOutOfRangeException(nameof(width), "Can not be 0");
			
			if(string.IsNullOrEmpty(txt))
				yield break;

			int startIndex = 0;
			for (var curIndex = startIndex; curIndex < txt.Length; curIndex++)
			{
				var c = txt[curIndex];
				var length = curIndex - startIndex;
				
				if (c == '\n')
				{
					yield return new Line(startIndex, length);
					startIndex = curIndex + 1;
				}
				else if(c == '\r')
				{
					yield return new Line(startIndex, length);
					startIndex = curIndex + 1;
					if (startIndex < txt.Length && txt[startIndex] == '\n')
					{
						startIndex++;
						curIndex++;
					}
				}
				else if(curIndex-startIndex >= width)
				{
					yield return new Line(startIndex, length);
					startIndex = curIndex;
				}
			}

			yield return new Line(startIndex, txt.Length - startIndex);
		}
		
		internal static IEnumerable<Line> WordWrap(string txt, int width)
		{
			if(width == 0)
				throw new ArgumentOutOfRangeException(nameof(width), "Can not be 0");
			
			if(string.IsNullOrEmpty(txt))
				yield break;

			int startIndex = 0;
			int breakIndex = 0;
			for (var curIndex = startIndex; curIndex < txt.Length; curIndex++)
			{
				var c = txt[curIndex];
				var length = curIndex - startIndex;

				//todo: punctuation wrap
				if (char.IsWhiteSpace(c))
				{
					breakIndex = curIndex;
				}
				
				if (c == '\n')
				{
					yield return new Line(startIndex, length);
					startIndex = curIndex + 1;
				}
				else if(c == '\r')
				{
					yield return new Line(startIndex, length);
					startIndex = curIndex + 1;
					if (startIndex < txt.Length && txt[startIndex] == '\n')
					{
						startIndex++;
						curIndex++;
					}
				}
				else if(curIndex-startIndex >= width && breakIndex > startIndex)
				{
					yield return new Line(startIndex, breakIndex-startIndex);
					startIndex = breakIndex;
					//Remove whitespaces at the line start.
					while (startIndex < txt.Length && char.IsWhiteSpace(txt[startIndex]))
						startIndex++;
				}
			}

			yield return new Line(startIndex, txt.Length - startIndex);
		}
	}

	struct Line
	{
		public Line(int startIndex, int length)
		{
			StartIndex = startIndex;
			Length = length;
		}
		
		public int StartIndex;
		public int Length;
	}
}