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
		/// <param name="direction">movement direction: positive indicates right, negative left</param>
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
		/// <param name="start">true indicates action is starting; false stopping</param>
		internal delegate void DropAction(bool start);
		internal static event DropAction NotifyDrop;

		/// <summary>
		/// event to handle instant brick drop to bottom of playing field
		/// </summary>
		internal delegate void HardDropAction();
		internal static event HardDropAction NotifyHardDrop;

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
			InputActionPhase phase = context.action.phase;
			var inputType = (InputType) Enum.Parse(typeof(InputType), context.action.name, true);

			if (phase == InputActionPhase.Performed) {
				switch (inputType) {
					//brick controls
					case InputType.Rotate:
						if (CanControlBrick) {
							NotifyRotate?.Invoke();
						}
						break;

					case InputType.HardDrop:
						if (CanControlBrick) {
							NotifyHardDrop?.Invoke();
						}
						break;

					//game controls
					case InputType.Pause:
						if (GameActive) {
							NotifyPause?.Invoke();
						}
						break;

					case InputType.Confirm:
						NotifyConfirm?.Invoke();
						break;
				}
			} else if (phase == InputActionPhase.Started || phase == InputActionPhase.Canceled) {
				if (inputType == InputType.Move && CanControlBrick) {
					var direction = context.ReadValue<float>();
					NotifyMove?.Invoke(phase == InputActionPhase.Started ? direction : 0);
				} else if (inputType == InputType.Drop && CanControlBrick) {
					NotifyDrop?.Invoke(phase == InputActionPhase.Started);
				}
			}
		}
	}
}
