using System;

namespace Misc
{
	public static class StringExtensions
	{
		public static char GetSymbolAt(this string target, int symbolIndex)
		{
			if (target.Length < symbolIndex)
				throw new IndexOutOfRangeException($"Индекса {symbolIndex} в строке длинной {target.Length} не существует!");

			return target[symbolIndex];
		}
	}
}