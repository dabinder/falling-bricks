using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks {
	/// <summary>
	/// controller to manage brick movement on playing field
	/// </summary>
	public class BrickController : MonoBehaviour {
		[SerializeField] private Vector3 rotationPoint;
		public Vector3 RotationPoint {
			get => rotationPoint;
		}
		
		internal Playfield Playfield { get; set; }

		private float _previousDrop;
		private float _dropTime = 1f;

		/// <summary>
		/// event to fire when a brick comes to rest after falling
		/// </summary>
		internal delegate void RestAction(Transform sender);
		internal static event RestAction NotifyRest;

		/// <summary>
		/// automatically drop piece over time
		/// </summary>
		private void Update() {
			if (Time.time - _previousDrop > _dropTime) {
				DropPiece();
				_previousDrop = Time.time;
			}
		}

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
		protected virtual void OnRotate(InputValue _) {
			if (enabled) {
				Vector3 point = transform.TransformPoint(rotationPoint),
					axis = new Vector3(0, 0, 1);
				transform.RotateAround(point, axis, -90);
				if (!IsValidMove()) transform.RotateAround(point, axis, 90);
			}
		}

		/// <summary>
		/// handle down key input - drop piece by a row
		/// </summary>
		/// <param name="_">down key press</param>
		private void OnDrop(InputValue _) {
			if (enabled) {
				DropPiece();
			}
		}

		/// <summary>
		/// immediately drop piece to bottom of playing field
		/// </summary>
		/// <param name="_">spacebar press</param>
		private void OnHardDrop(InputValue _) {
			while (enabled && IsValidMove()) {
				DropPiece();
			}
		}

		/// <summary>
		/// drop piece by one row
		/// </summary>
		private void DropPiece() {
			if (enabled) {
				Vector3 move = new Vector3(0, -1, 0);
				transform.position += move;
				if (!IsValidMove()) {
					transform.position -= move;
					enabled = false;
					NotifyRest?.Invoke(transform);
				}
			}
		}

		/// <summary>
		/// check whether the brick's current location is a legal move
		/// </summary>
		/// <returns>new location is a valid move</returns>
		private bool IsValidMove() {
			foreach (Transform block in transform) {
				Vector2 position = block.position;
				if (!Playfield.IsInsideField(position) ||
					Playfield.IsOccupied(position)
				) {
					return false;
				}
			}
			return true;
		}
	}
}