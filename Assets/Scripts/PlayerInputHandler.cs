using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks {
	public class PlayerInputHandler : MonoBehaviour {
		private enum InputType {
			Move,
			Rotate,
			Drop,
			HardDrop
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
		///		indicates a hard drop (instantly drops brick to bottom of field)
		///		soft drop will drop brick by one row at a time
		///	</param>
		internal delegate void DropAction(bool hard);
		internal static event DropAction NotifyDrop;

		private bool _canControlBrick = true;

		/// <summary>
		/// subscribe to input events
		/// </summary>
		private void Awake() {
			GetComponent<PlayerInput>().onActionTriggered += HandleInput;
			GameManager.NotifySuspend += (suspended) => _canControlBrick = !suspended;
		}

		/// <summary>
		/// dispatch event corresponding to input type
		/// </summary>
		/// <param name="context">player input</param>
		public void HandleInput(InputAction.CallbackContext context) {
			if (_canControlBrick && context.action.phase == InputActionPhase.Performed) {
				switch (Enum.Parse(typeof(InputType), context.action.name, true)) {
					case InputType.Move:
						var direction = context.ReadValue<float>();
						NotifyMove?.Invoke(direction);
						break;

					case InputType.Rotate:
						NotifyRotate?.Invoke();
						break;

					case InputType.Drop:
						NotifyDrop?.Invoke(false);
						break;

					case InputType.HardDrop:
						NotifyDrop?.Invoke(true);
						break;
				}
			}
		}
	}
}
