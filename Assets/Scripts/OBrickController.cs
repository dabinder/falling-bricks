using UnityEngine.InputSystem;

namespace Bricks {
	public class OBrickController : BrickController {
		/// <summary>
		/// O-bricks cannot be rotated so ignore input
		/// </summary>
		/// <param name="_">up key press</param>
		protected override void OnRotate(InputValue _) {
			return;
		}
	}
}
