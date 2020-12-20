using UnityEngine.InputSystem;

namespace Bricks.Controllers {
	class OBrickController : BrickController {
		/// <summary>
		/// O-bricks cannot be rotated so ignore input
		/// </summary>
		/// <param name="rotateValue">up key press</param>
		protected new void OnRotate(InputValue rotateValue) {
			return;
		}
	}
}
