using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Bricks {
	/// <summary>
	/// runs the game, maintains player score, directs the brick spawner
	/// </summary>
	public class GameManager : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI scoreText, linesText;
		[SerializeField] private GameObject nextBrick;
		[SerializeField] private GameObject suspendPanel, losePanel, pausePanel;

		private bool _gameOver;
		private int _score, _lines;

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

		/// <summary>
		/// keep track of player score and update on screen
		/// </summary>
		/// <param name="lines">number of lines cleared</param>
		public void UpdateScore(int lines) {
			_score += 10 * Mathf.RoundToInt(Mathf.Pow(3, lines - 1));
			scoreText.text = _score.ToString();

			_lines += lines;
			linesText.text = _lines.ToString();
		}

		/// <summary>
		/// display the next brick scheduled to be spawned
		/// </summary>
		/// <param name="brick">next brick</param>
		public void UpdateNext(GameObject brick) {
			//clear previous entry
			Transform transform = nextBrick.transform;
			for (int i = 0; i < transform.childCount; i++) {
				Destroy(transform.GetChild(i).gameObject);
			}

			//add next
			Vector3 nextPosition = transform.position;
			GameObject brickObject = Instantiate(brick, Vector3.zero, Quaternion.identity);
			BrickController controller = brickObject.GetComponent<BrickController>();
			brickObject.transform.parent = transform;
			Vector3 rotationPoint = controller.RotationPoint;
			brickObject.transform.position = nextPosition - rotationPoint;
		}
	}
}