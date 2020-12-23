using UnityEngine;

namespace Bricks {
	/// <summary>
	/// extension methods
	/// </summary>
	internal static class Extensions {
		/// <summary>
		/// convert float vector to int vector
		/// </summary>
		/// <param name="vector">vector to convert</param>
		/// <returns>vector with x, y values rounded to integers</returns>
		internal static Vector2Int ToVector2Int(this Vector2 vector) =>
			new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));

		/// <summary>
		/// convert float vector to int vector, discard z coordinate
		/// </summary>
		/// <param name="vector">vector to convert</param>
		/// <returns>vector with x, y values rounded to integers</returns>
		internal static Vector2Int ToVector2Int(this Vector3 vector) =>
			new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
	}
}
