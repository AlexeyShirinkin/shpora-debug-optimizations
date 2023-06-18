using System;
using System.Collections.Generic;
using System.Linq;

namespace JPEG;

internal static class EnumerableExtensions
{
	public static T MinOrDefault<T>(this IEnumerable<T> enumerable, Func<T, int> selector) =>
		enumerable.OrderBy(selector).FirstOrDefault();

	public static IEnumerable<T> Without<T>(this IEnumerable<T> enumerable, params T[] elements) =>
		enumerable.Where(x => !elements.Contains(x));

	public static IEnumerable<T> ToEnumerable<T>(this T element)
	{
		yield return element;
	}
}