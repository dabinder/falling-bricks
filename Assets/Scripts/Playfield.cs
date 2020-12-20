using UnityEngine;

namespace Bricks {
	internal static class Playfield {
		internal const int WIDTH = 10, HEIGHT = 20;

		/// <summary>
		/// game grid
		/// </summary>
		internal static Transform[,] grid { get; } = new Transform[WIDTH, HEIGHT];

		/// <summary>
		/// helper method to ensure vector value is int (may be off due to rotation)
		/// </summary>
		/// <param name="vector">input vector to round</param>
		/// <returns>vector rounded as (int, int)</returns>
		internal static Vector2 RoundVectorToInt(Vector2 vector) {
			return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
		}

		/// <summary>
		/// check whether given vector is inside the playable area
		/// </summary>
		/// <param name="position">vector to verify</param>
		/// <returns>vector is inside the playable area</returns>
		internal static bool IsInsideField(Vector2 position) {
			var rounded = RoundVectorToInt(position);
			return rounded.x >= 0 &&
				rounded.x < WIDTH &&
				rounded.y >= 0;
		}
	}
}
