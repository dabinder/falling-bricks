using UnityEngine;

namespace Bricks {
	/// <summary>
	/// game object to spawn new bricks at top of field
	/// </summary>
	public class BrickSpawner : MonoBehaviour {
		[SerializeField] private GameObject[] bricks;

		/// <summary>
		/// next brick to spawn
		/// </summary>
		internal GameObject Next { get; private set; }

		/// <summary>
		/// current game level (affects brick drop speed)
		/// </summary>
		internal int Level { get; set; }

		/// <summary>
		/// queue up a brick to start the game and subscribe to level changes (impacting brick speed)
		/// </summary>
		private void Awake() {
			UpdateNext();
		}

		/// <summary>
		/// queue up a brick at random
		/// </summary>
		/// <returns>next brick to spawn</returns>
		private void UpdateNext() =>
			Next = bricks[Random.Range(0, bricks.Length)];

		/// <summary>
		/// create a new brick at the spawn point
		/// </summary>
		internal void SpawnBrick() {
			BrickController controller = Instantiate(Next, transform.position, Quaternion.identity)
				.GetComponent<BrickController>();
			controller.Level = Level;
			controller.enabled = true;
			UpdateNext();
		}
	}
}
