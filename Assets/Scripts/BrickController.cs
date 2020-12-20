using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks {
	public class BrickController : MonoBehaviour {
		[SerializeField] private Vector3 rotationPoint;

		/// <summary>
		/// move active piece left or right
		/// </summary>
		/// <param name="movementValue">negative (left) or positive (right) value</param>
		private void OnMove(InputValue movementValue) {
			if (enabled) {
				var direction = movementValue.Get<float>();
				Vector3 move;
				if (direction > 0) { //move right
					move = new Vector3(1, 0, 0);
				} else if (direction < 0) { //move left
					move = new Vector3(-1, 0, 0);
				} else {
					return;
				}
				transform.position += move;
				if (!IsValidMove()) transform.position -= move;
				
			}
		}

		/// <summary>
		/// rotate piece
		/// </summary>
		/// <param name="_">up key press</param>
		protected void OnRotate(InputValue _) {
			if (enabled) {
				Vector3 point = transform.TransformPoint(rotationPoint),
					axis = new Vector3(0, 0, 1);
				transform.RotateAround(point, axis, -90);
				if (!IsValidMove()) transform.RotateAround(point, axis, 90);
			}
		}

		/// <summary>
		/// drop piece by one row
		/// </summary>
		/// <param name="_">down key press</param>
		private void OnDrop(InputValue _) {
			if (enabled) {
				Vector3 move = new Vector3(0, -1, 0);
				transform.position += move;
				if (!IsValidMove()) transform.position -= move;
			}
		}

		/// <summary>
		/// check whether the brick's current location is a legal move
		/// </summary>
		/// <returns>new location is a valid move</returns>
		private bool IsValidMove() {
			foreach (Transform block in transform) {
				if (!Playfield.IsInsideField(block.position)) {
					return false;
				}
			}

			return true;
		}
	}
}