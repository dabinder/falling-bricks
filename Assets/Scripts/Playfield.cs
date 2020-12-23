using UnityEngine;

namespace Bricks {
	public static class Playfield {
		private const int WIDTH = 10, HEIGHT = 20;

		/// <summary>
		/// game grid
		/// </summary>
		private static Transform[,] _grid = new Transform[WIDTH, HEIGHT];

		/// <summary>
		/// subscribe to rest event to add brick to grid
		/// </summary>
		static Playfield() => BrickController.NotifyRest += AddToGrid;

		/// <summary>
		/// check whether given vector is inside the playable area
		/// </summary>
		/// <param name="position">vector to verify</param>
		/// <returns>vector is inside the playable area</returns>
		internal static bool IsInsideField(Vector2 position) {
			Vector2Int rounded = position.ToVector2Int();
			return rounded.x >= 0 &&
				rounded.x < WIDTH &&
				rounded.y >= 0;
		}

		/// <summary>
		/// check if the specified grid position is occupied by a block
		/// </summary>
		/// <param name="position">vector to lookup</param>
		/// <returns></returns>
		internal static bool IsOccupied(Vector2 position) {
			Vector2Int rounded = position.ToVector2Int();
			return _grid[rounded.x, rounded.y] != null;
		}

		/// <summary>
		/// record final position of brick in game grid
		/// </summary>
		/// <param name="brick">brick to store in grid</param>
		private static void AddToGrid(Transform brick) {
			foreach (Transform block in brick) {
				Vector2Int rounded = block.position.ToVector2Int();
				Debug.Log(rounded.ToString());
				_grid[rounded.x, rounded.y] = block;
			}
		}
	}
}