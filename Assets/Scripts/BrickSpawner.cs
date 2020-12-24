using UnityEngine;

namespace Bricks {
	/// <summary>
	/// game object to spawn new bricks at top of field
	/// </summary>
	public class BrickSpawner : MonoBehaviour {
		[SerializeField] private GameObject[] bricks;

		/// <summary>
		/// create a new brick at the spawn point
		/// </summary>
		public void SpawnBrick(Playfield playfield) {
			BrickController controller = Instantiate(bricks[Random.Range(0, bricks.Length)], transform.position, Quaternion.identity)
				.GetComponent<BrickController>();
			controller.Playfield = playfield;
		}
	}
}
