using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks {
	/// <summary>
	/// game object to spawn new bricks at top of field
	/// </summary>
	public class BrickSpawner : MonoBehaviour {
		[SerializeField] private GameObject[] bricks;

		/// <summary>
		/// next brick to spawn
		/// </summary>
		public GameObject Next { get; private set; }

		private int _level;

		/// <summary>
		/// queue up a brick to start the game
		/// </summary>
		private void Awake() {
			UpdateNext();
			GameManager.NotifyLevel += UpdateLevel;
		}

		/// <summary>
		/// queue up a brick at random
		/// </summary>
		/// <returns>next brick to spawn</returns>
		private void UpdateNext() =>
			Next = bricks[Random.Range(0, bricks.Length)];

		/// <summary>
		/// set level for spawning future bricks
		/// </summary>
		/// <param name="level">new level</param>
		private void UpdateLevel(int level) {
			_level = level;
		}

		/// <summary>
		/// create a new brick at the spawn point
		/// </summary>
		public void SpawnBrick(Playfield playfield) {
			GameObject brick = Instantiate(Next, transform.position, Quaternion.identity);
			BrickController controller = brick.GetComponent<BrickController>();
			controller.Playfield = playfield;
			controller.Level = _level;
			controller.enabled = true;
			brick.GetComponent<PlayerInput>().enabled = true;
			UpdateNext();
		}
	}
}
