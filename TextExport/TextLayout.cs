using System;
using System.Collections.Generic;
using GrapeCity.ActiveReports.Drawing;

namespace TextExport
{
	internal static class TextLayout
	{
		public static IEnumerable<Line> SplitLines(string txt, StringFormatEx format, int width)
		{
			//todo: wrap, line breaking
			return CharWrap(txt, width);
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