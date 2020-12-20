using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks.Controllers {
	public class BrickController : MonoBehaviour {
		[SerializeField] private Vector3 rotationPoint;

		/// <summary>
		/// move active piece left or right
		/// </summary>
		/// <param name="movementValue">negative (left) or positive (right) value</param>
		private void OnMove(InputValue movementValue) {
			if (enabled) {
				var direction = movementValue.Get<float>();
				if (direction > 0) { //move right
					transform.position += new Vector3(1, 0, 0);
				} else if (direction < 0) { //move left
					transform.position += new Vector3(-1, 0, 0);
				}
			}
		}

		/// <summary>
		/// rotate piece
		/// </summary>
		/// <param name="rotateValue">up key press</param>
		protected void OnRotate(InputValue rotateValue) {
			if (enabled) {
				transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
			}
		}
	}
}