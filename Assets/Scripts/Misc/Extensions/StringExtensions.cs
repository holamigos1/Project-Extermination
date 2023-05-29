using System;
using System.Runtime.CompilerServices;

namespace Misc.Extensions
{
	public static class StringExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char GetSymbolAt(this string target, int symbolIndex)
		{
			if (target.Length < symbolIndex)
				throw new IndexOutOfRangeException($"Индекса {symbolIndex} в строке длинной {target.Length} не существует!");

			return target[symbolIndex];
		}
	}
}