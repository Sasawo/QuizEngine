using System;
using System.Collections.Generic;

public static class Extensions
{
	public static List<T> Shuffle<T>(this List<T> list)
	{
		Random rng = new();

		for (int i = 0; i < list.Count; ++i)
		{
			int index = rng.Next(0, list.Count);
			T temp = list[i];
			list[i] = list[index];
			list[index] = temp;
		}

		return list;
	}
}