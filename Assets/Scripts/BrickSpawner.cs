using UnityEngine;

namespace Bricks {
	/// <summary>
	/// game object to spawn new bricks at top of field
	/// </summary>
	public class BrickSpawner : MonoBehaviour {
		[SerializeField] private GameObject[] _bricks;

		/// <summary>
		/// spawn a brick to start the game and subscribe to rest event to spawn new bricks
		/// </summary>
		private void Start() {
			BrickController.NotifyRest += (_) => SpawnBrick();
			SpawnBrick();
		}

		/// <summary>
		/// create a new brick at the spawn point
		/// </summary>
		private void SpawnBrick() =>
			Instantiate(_bricks[Random.Range(0, _bricks.Length)], transform.position, Quaternion.identity);
	}
}
