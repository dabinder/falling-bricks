using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks {
	/// <summary>
	/// central location for handling player inputs to dispatch appropriate
	/// </summary>
	public class PlayerInputHandler : MonoBehaviour {
		private enum InputType {
			//brick actions
			Move,
			Rotate,
			Drop,
			HardDrop,

			//game actions
			Pause,
			Confirm
		}

		/// <summary>
		/// event to handle left/right movement
		/// </summary>
		/// <param name="direction"></param>
		internal delegate void MoveAction(float direction);
		internal static event MoveAction NotifyMove;

		/// <summary>
		/// event to handle brick rotation
		/// </summary>
		internal delegate void RotateAction();
		internal static event RotateAction NotifyRotate;

		/// <summary>
		/// event to handle brick drops
		/// </summary>
		/// <param name="hard">
		/// indicates a hard drop (instantly drops brick to bottom of field)
		/// soft drop will drop brick by one row at a time
		/// </param>
		internal delegate void DropAction(bool hard);
		internal static event DropAction NotifyDrop;

		/// <summary>
		/// event to handle pause button press
		/// </summary>
		internal delegate void PauseAction();
		internal static event PauseAction NotifyPause;

		/// <summary>
		/// event to handle confirm button press
		/// </summary>
		internal delegate void ConfirmAction();
		internal static event ConfirmAction NotifyConfirm;

		/// <summary>
		/// indicate whether bricks can currently be controlled by the player
		/// </summary>
		public bool CanControlBrick { get; set; } = true;

		/// <summary>
		/// indicate whether the game is currently active and whether game-related input should be relayed
		/// </summary>
		public bool GameActive { get; set; } = true;

		/// <summary>
		/// subscribe to input events
		/// </summary>
		private void OnEnable() {
			GetComponent<PlayerInput>().onActionTriggered += HandleInput;
		}

		/// <summary>
		/// unsubscribe from input events
		/// </summary>
		private void OnDisable() {
			GetComponent<PlayerInput>().onActionTriggered -= HandleInput;
		}

		/// <summary>
		/// dispatch event corresponding to input type
		/// </summary>
		/// <param name="context">player input</param>
		public void HandleInput(InputAction.CallbackContext context) {
			if (context.action.phase == InputActionPhase.Performed) {
				switch (Enum.Parse(typeof(InputType), context.action.name, true)) {
					//brick controls
					case InputType.Move:
						if (!CanControlBrick) return;
						var direction = context.ReadValue<float>();
						NotifyMove?.Invoke(direction);
						break;

					case InputType.Rotate:
						if (!CanControlBrick) return;
						NotifyRotate?.Invoke();
						break;

					case InputType.Drop:
						if (!CanControlBrick) return;
						NotifyDrop?.Invoke(false);
						break;

					case InputType.HardDrop:
						if (!CanControlBrick) return;
						NotifyDrop?.Invoke(true);
						break;

					//game controls
					case InputType.Pause:
						if (!GameActive) return;
						NotifyPause?.Invoke();
						break;

					case InputType.Confirm:
						NotifyConfirm?.Invoke();
						break;
				}
			}
		}
	}
}
