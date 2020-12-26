using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bricks {
	/// <summary>
	/// helper methods to manage the playing field
	/// </summary>
	public class Playfield : MonoBehaviour {
		private const int WIDTH = 10, HEIGHT = 20;

		[SerializeField] private GameObject brickSpawnerObject;

		[SerializeField] private UnityEvent OnFieldFull;
		[SerializeField] private UnityEvent<GameObject> OnSpawn;
		[SerializeField] private UnityEvent<int> OnClearLines;
		
		private readonly Transform[,] _grid = new Transform[WIDTH, HEIGHT];
		private BrickSpawner _brickSpawner;

		/// <summary>
		/// subscribe to rest event to add brick to grid
		/// </summary>
		private void OnEnable() {
			BrickController.NotifyRest += HandleRest;
			_brickSpawner = brickSpawnerObject.GetComponent<BrickSpawner>();
			_brickSpawner.SpawnBrick(this);
			OnSpawn.Invoke(_brickSpawner.Next);
		}

		/// <summary>
		/// avoid errant event calls after this playfield's corresponding game object is disabled or destroyed
		/// </summary>
		private void OnDisable() {
			BrickController.NotifyRest -= HandleRest;
		}

		/// <summary>
		/// check whether given vector is inside the playable area
		/// allow vectors above field in case of newly spawned piece
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
		/// also look for special case where vector is above the top of the field
		/// since this will occur when a piece is first spawned
		/// </summary>
		/// <param name="position">vector to lookup</param>
		/// <returns></returns>
		internal bool IsOccupied(Vector2 position) {
			Vector2Int rounded = position.ToVector2Int();
			return rounded.y < HEIGHT &&
				_grid[rounded.x, rounded.y] != null;
		}

		/// <summary>
		/// record final position of brick in game grid, check for completed lines, and spawn a new brick
		/// 
		/// </summary>
		/// <param name="brick">brick to store in grid</param>
		private void HandleRest(Transform brick) {
			if (AddToGrid(brick)) {
				int lines = ClearLines();
				if (lines > 0) {
					OnClearLines.Invoke(lines);
				}
				_brickSpawner.SpawnBrick(this);
				OnSpawn.Invoke(_brickSpawner.Next);
			} else {
				OnFieldFull?.Invoke();
			}
		}

		/// <summary>
		/// record final position of brick in game grid if in valid position
		/// </summary>
		/// <param name="brick">brick to store in grid</param>
		/// <returns>brick was in a valid position and added to the grid</returns>
		private bool AddToGrid(Transform brick) {
			foreach (Transform block in brick) {
				Vector2Int rounded = block.position.ToVector2Int();
				if (rounded.y >= HEIGHT) {
					//brick couldn't fully enter the playing field
					return false;
				}
				_grid[rounded.x, rounded.y] = block;
			}

			return true;
		}

		/// <summary>
		/// clear all completed lines from playing field
		/// </summary>
		/// <returns>number of cleared lines</returns>
		private int ClearLines() {
			int lines = 0;
			var bricks = new Dictionary<GameObject, int>();

			for (int y = 0; y < HEIGHT; y++) {
				if (IsLine(y)) {
					ClearLine(y, ref bricks);
					lines++;

					//in the miraculous event the top row is cleared, this is an edge case
					if (y < HEIGHT - 1) {
						DropRow(y + 1);
					}

					//everything has shifted down by one
					y--;
				}
			}

			//destroy any empty bricks
			foreach (GameObject brick in bricks.Keys) {
				if (brick.transform.childCount - bricks[brick] <= 0) {
					Destroy(brick);
				}
			}

			return lines;
		}

		/// <summary>
		/// check if specified row contains a complete line of blocks
		/// </summary>
		/// <param name="y">row number to check</param>
		/// <returns>row contains a complete line</returns>
		private bool IsLine(int y) {
			for (int x = 0; x < WIDTH; x++) {
				if (_grid[x, y] == null) {
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// delete all blocks in specified row
		/// </summary>
		/// <param name="y">row to clear</param>
		private void ClearLine(int y, ref Dictionary<GameObject, int> bricks) {
			for (int x = 0; x < WIDTH; x++) {
				GameObject block = _grid[x, y].gameObject,
					brick = block.transform.parent.gameObject;
				Destroy(block);
				_grid[x, y] = null;

				if (bricks.ContainsKey(brick)) {
					bricks[brick]++;
				} else {
					bricks[brick] = 1;
				}
			}
		}

		/// <summary>
		/// drop all blocks in specified row by one
		/// </summary>
		/// <param name="y">row to drop</param>
		private void DropRow(int y) {
			for (int x = 0; x < WIDTH; x++) {
				if (_grid[x, y] != null) {
					//move stored row
					_grid[x, y - 1] = _grid[x, y];
					_grid[x, y] = null;

					//move block
					_grid[x, y - 1].position -= new Vector3(0, 1, 0);
				}
			}

			//continue with each subsequent row
			if (y < HEIGHT - 1) {
				DropRow(y + 1);
			}
		}
	}
}
