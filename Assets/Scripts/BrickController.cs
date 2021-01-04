using UnityEngine;

namespace Bricks {
	/// <summary>
	/// controller to manage brick movement on playing field
	/// </summary>
	public class BrickController : MonoBehaviour {
		//initial speed measured in seconds between auto-drops
		private const float BASE_DROP_TIME = 1f;
		//percent increase in speed each new level
		private const float LEVEL_MULTIPLIER = .1f;

		private const float MOVE_TIME_INTERVAL = .1f;

		[SerializeField] private Vector3 rotationPoint;
		/// <summary>
		/// get the center of rotation point for the current brick
		/// </summary>
		internal Vector3 RotationPoint {
			get => rotationPoint;
		}

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

		/// <summary>
		/// event to fire when a brick comes to rest after falling
		/// </summary>
		internal delegate void RestAction(Transform sender);
		internal static event RestAction NotifyRest;

		private float _previousDrop, _previousMove;
		private float _dropTime = BASE_DROP_TIME;
		private Vector3Int _moveDirection;
		private bool _isDropping;

		/// <summary>
		/// subscribe to input events
		/// </summary>
		private void OnEnable() {
			PlayerInputHandler.NotifyMove += Move;
			PlayerInputHandler.NotifyRotate += Rotate;
			PlayerInputHandler.NotifyDrop += Drop;
			PlayerInputHandler.NotifyHardDrop += HardDrop;
		}

		/// <summary>
		/// unsubscribe from input events
		/// </summary>
		private void OnDisable() {
			PlayerInputHandler.NotifyMove -= Move;
			PlayerInputHandler.NotifyRotate -= Rotate;
			PlayerInputHandler.NotifyDrop -= Drop;
			PlayerInputHandler.NotifyHardDrop -= HardDrop;
		}

		/// <summary>
		/// handle piece moves and drops, plus auto-drop piece over time
		/// </summary>
		private void Update() {
			//continually move piece while button is pressed
			if (_moveDirection != Vector3.zero && Time.time - _previousMove > MOVE_TIME_INTERVAL) {
				transform.position += _moveDirection;
				if (!IsValidMove()) transform.position -= _moveDirection;
				_previousMove = Time.time;
			}

			//continually drop piece while button is pressed; otherwise drop on fixed time interval
			if ((_isDropping && Time.time - _previousDrop > MOVE_TIME_INTERVAL) ||
				(!_isDropping && Time.time - _previousDrop > _dropTime)
			) {
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
				if (direction > 0) { //move right
					_moveDirection = new Vector3Int(1, 0, 0);
				} else if (direction < 0) { //move left
					_moveDirection = new Vector3Int(-1, 0, 0);
				} else { //stop moving
					_moveDirection = new Vector3Int(0, 0, 0);
				}

				//perform initial move immediately
				if (_moveDirection != Vector3.zero) {
					transform.position += _moveDirection;
					if (!IsValidMove()) transform.position -= _moveDirection;
					_previousMove = Time.time;
				}
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
		/// drop piece by one row at a time
		/// </summary>
		/// <param name="start">true indicates drop action is starting; false stopping</param>
		private void Drop(bool start) {
			if (enabled) {
				if (start) {
					_isDropping = start;

					//perform initial drop immediately
					DropPiece();
					_previousDrop = Time.time;
				} else {
					_isDropping = false;
				}
			}
		}

		/// <summary>
		/// instantly drop piece to bottom of field
		/// </summary>
		private void HardDrop() {
			while (enabled && IsValidMove()) {
				DropPiece();
			}
		}

		/// <summary>
		/// drop piece by one row
		/// </summary>
		private void DropPiece() {
			if (enabled) {
				Vector3Int move = new Vector3Int(0, -1, 0);
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