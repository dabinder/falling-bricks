using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bricks {
	/// <summary>
	/// central location for handling player inputs to dispatch appropriate
	/// </summary>
	public class PlayerInputHandler : MonoBehaviour {
		private const string PLAYER_MAP = "Player";
		private const string HOME_MAP = "Home";

		private enum InputType {
			//brick actions
			Move,
			Rotate,
			Drop,
			HardDrop,

			//game actions
			Pause,
			Confirm,
			
			//set level on home screen
			ChangeStartLevel
		}

		/// <summary>
		/// handle input with a positive/negative/zero flag
		/// </summary>
		/// <param name="indicator">indicates positive, negative, or a zero input value</param>
		internal delegate void PosNegAction(float indicator);

		/// <summary>
		/// handle input intended to be performed continuously until instructed to stop
		/// </summary>
		/// <param name="start">true starts input action; false ends it</param>
		internal delegate void ContinuousAction(bool start);

		/// <summary>
		/// handle input intended to be performed once per button press
		/// </summary>
		internal delegate void SingularAction();

		/// <summary>
		/// event to handle left/right movement
		/// negative is left, positive right
		/// </summary>
		internal static event PosNegAction NotifyMove;

		/// <summary>
		/// event to handle brick rotation
		/// </summary>
		internal static event SingularAction NotifyRotate;

		/// <summary>
		/// event to handle brick drops
		/// true indicates drop is starting; false stopping
		/// </summary>
		internal static event ContinuousAction NotifyDrop;

		/// <summary>
		/// event to handle instant brick drop to bottom of playing field
		/// </summary>
		internal static event SingularAction NotifyHardDrop;

		/// <summary>
		/// event to handle pause button press
		/// </summary>
		internal static event SingularAction NotifyPause;

		/// <summary>
		/// event to handle confirm button press
		/// </summary>
		internal static event SingularAction NotifyConfirm;

		/// <summary>
		/// event to set starting level
		/// positive increments level, negative decrements
		/// </summary>
		internal static event PosNegAction NotifyChangeStartLevel;

		/// <summary>
		/// indicate whether bricks can currently be controlled by the player
		/// </summary>
		internal bool CanControlBrick { get; set; } = true;

		/// <summary>
		/// indicate whether the game is currently active and whether game-related input should be relayed
		/// </summary>
		internal bool IsGameActive { get; set; } = true;

		private bool _isHome;
		/// <summary>
		/// indicate whether game is currently on home screen (false indicates in-game)
		/// switch action map accordingly
		/// </summary>
		internal bool IsHome {
			get => _isHome;
			set {
				_isHome = value;
				_playerInput?.SwitchCurrentActionMap(value ? HOME_MAP : PLAYER_MAP);
			}
		}

		private PlayerInput _playerInput;

		/// <summary>
		/// subscribe to input events
		/// </summary>
		private void OnEnable() {
			_playerInput = GetComponent<PlayerInput>();
			_playerInput.onActionTriggered += HandleInput;
		}

		/// <summary>
		/// unsubscribe from input events
		/// </summary>
		private void OnDisable() {
			_playerInput.onActionTriggered -= HandleInput;
		}

		/// <summary>
		/// dispatch event corresponding to input type
		/// </summary>
		/// <param name="context">player input</param>
		private void HandleInput(InputAction.CallbackContext context) {
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
						if (IsGameActive) {
							NotifyPause?.Invoke();
						}
						break;

					case InputType.Confirm:
						NotifyConfirm?.Invoke();
						break;

					//set level (home screen only)
					case InputType.ChangeStartLevel:
						NotifyChangeStartLevel?.Invoke(context.ReadValue<float>());
						break;
				}
			} else if (phase == InputActionPhase.Started || phase == InputActionPhase.Canceled) {
				//move and drop can be held down to repeat action
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
