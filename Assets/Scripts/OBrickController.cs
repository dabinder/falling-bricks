using UnityEngine.InputSystem;

namespace Bricks {
	/// <summary>
	/// controller to manage non-rotatable O-bricks
	/// </summary>
	public class OBrickController : BrickController {
		/// <summary>
		/// O-bricks cannot be rotated so ignore input
		/// </summary>
		protected override void Rotate() { }
	}
}
