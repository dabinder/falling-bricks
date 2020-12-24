using UnityEngine;

namespace Bricks {
	public class GameManager : MonoBehaviour {
		[SerializeField] GameObject playfieldObject;
		[SerializeField] GameObject pausePanel;
		[SerializeField] GameObject loseText;

		private Playfield _playfield;

		private bool _isStopped;
		private bool IsStopped {
			get => _isStopped;
			set {
				_isStopped = value;
				pausePanel.SetActive(value);
				Time.timeScale = value ? 0 : 1;
			}
		}

		/// <summary>
		/// grab reference to playing field
		/// </summary>
		private void Start() {
			_playfield = playfieldObject.GetComponent<Playfield>();
		}

		/// <summary>
		/// end game with a game over notification
		/// </summary>
		private void GameOver() {
			IsStopped = true;
			loseText.SetActive(true);
		}
	}
}