using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks {
	/// <summary>
	/// controller to manage brick movement on playing field
	/// </summary>
	public class BrickController : MonoBehaviour {
		//initial speed measured in seconds between auto-drops
		private const float BASE_DROP_TIME = 1f;
		//percent increase in speed each new level
		private const float LEVEL_MULTIPLIER = .1f;

		[SerializeField] private Vector3 rotationPoint;
		/// <summary>
		/// get the center of rotation point for the current brick
		/// </summary>
		public Vector3 RotationPoint {
			get => rotationPoint;
		}

		internal Playfield Playfield { get; set; }

		private int _level;
		/// <summary>
		/// decrease the drop time by a fixed percentage for each level after the first
		/// </summary>
		internal int Level {
			get => _level;
			set {
				_level = value;
				_dropTime = BASE_DROP_TIME * Mathf.Pow(1 - LEVEL_MULTIPLIER, value - 1);
			}
		}

		private float _previousDrop;
		private float _dropTime = BASE_DROP_TIME;

		/// <summary>
		/// event to fire when a brick comes to rest after falling
		/// </summary>
		internal delegate void RestAction(Transform sender);
		internal static event RestAction NotifyRest;

		/// <summary>
		/// subscribe to input events
		/// </summary>
		private void OnEnable() {
			PlayerInputHandler.NotifyMove += Move;
			PlayerInputHandler.NotifyRotate += Rotate;
			PlayerInputHandler.NotifyDrop += Drop;
		}

		/// <summary>
		/// unsubscribe from input events
		/// </summary>
		private void OnDisable() {
			PlayerInputHandler.NotifyMove -= Move;
			PlayerInputHandler.NotifyRotate -= Rotate;
			PlayerInputHandler.NotifyDrop -= Drop;
		}

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
		/// <param name="direction">negative (left) or positive (right) value</param>
		private void Move(float direction) {
			if (enabled) {
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
		protected virtual void Rotate() {
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
		/// <param name="hard">
		///		indicates a hard drop (instantly drops brick to bottom of field)
		///		soft drop will drop brick by one row at a time
		///	</param>
		private void Drop(bool hard) {
			if (enabled) {
				if (hard) {
					while (enabled && IsValidMove()) {
						DropPiece();
					}
				} else {
					DropPiece();
				}
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