using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Bricks {
	public class GameManager : MonoBehaviour {
		[SerializeField] GameObject playfieldObject;
		[SerializeField] GameObject suspendPanel, losePanel, pausePanel;

		private Playfield _playfield;
		private bool _gameOver;

		private bool _isPaused;
		private bool IsPaused {
			get => _isPaused;
			set {
				_isPaused = value;
				suspendPanel.SetActive(value);
				Time.timeScale = value ? 0 : 1;
			}
		}

		/// <summary>
		/// grab reference to playing field
		/// </summary>
		private void Start() {
			_playfield = playfieldObject.GetComponent<Playfield>();
			Time.timeScale = 1;
		}

		/// <summary>
		/// end game with a game over notification
		/// </summary>
		private void GameOver() {
			IsPaused = true;
			losePanel.SetActive(true);
			_gameOver = true;
		}

		/// <summary>
		/// handle pause button press
		/// </summary>
		/// <param name="_">pause button press</param>
		private void OnPause(InputValue _) {
			if (_gameOver) return;
			bool paused = !IsPaused;
			IsPaused = paused;
			pausePanel.SetActive(paused);
		}

		/// <summary>
		/// confirm reset after end of game or from pause menu
		/// </summary>
		/// <param name="_">confirm button press</param>
		private void OnConfirm(InputValue _) {
			if (IsPaused) {
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
	}
}