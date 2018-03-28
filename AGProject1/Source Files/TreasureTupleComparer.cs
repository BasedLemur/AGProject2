using System;
using System.Collections.Generic;

namespace AGProject1 {
	/// <summary>
	/// Comparer functor used when finding the closest treasure to the NPAgent.
	/// This just compares the float part of the tuple that contains the distance (float)
	/// and the corresponding treasure.
	/// </summary>
	class TreasureTupleComparer : IComparer<Tuple<float, Treasure>> {
		public int Compare(Tuple<float, Treasure> x, Tuple<float, Treasure> y) {
			return x.Item1 < y.Item1 ? -1 : x.Item1 > y.Item1 ? 1 : 0;
		}
	}
}
