using UnityEngine.InputSystem;

namespace Bricks {
	/// <summary>
	/// controller to manage non-rotatable O-bricks
	/// </summary>
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
