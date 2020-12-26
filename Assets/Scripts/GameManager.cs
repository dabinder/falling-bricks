using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Bricks {
	/// <summary>
	/// runs the game, maintains player score, directs the brick spawner
	/// </summary>
	public class GameManager : MonoBehaviour {
		private const int LINE_SCORE = 10,
			LINE_BASE = 3,
			MAX_LINES = 10;
		private const float LEVEL_BONUS = .5f;

		[SerializeField] private TextMeshProUGUI scoreText, linesText, levelText;
		[SerializeField] private GameObject nextBrick;
		[SerializeField] private GameObject suspendPanel, losePanel, pausePanel;

		/// <summary>
		/// event to fire when level is increased
		/// </summary>
		internal delegate void LevelAction(int level);
		internal static event LevelAction NotifyLevel;

		private bool _gameOver;
		private int _level = 1;
		private int Level {
			get => _level;
			set {
				_level = value;
				NotifyLevel?.Invoke(value);
			}
		}
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
			scoreText.text = _score.ToString();
			linesText.text = _lines.ToString();
			levelText.text = Level.ToString();
			NotifyLevel?.Invoke(Level);
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
		/// increase level at fixed interval of lines
		/// </summary>
		/// <param name="lines">number of lines cleared</param>
		public void UpdateScore(int lines) {
			//score is SCORE * BASE^(n-1) * (1 + (level - 1) * BONUS)
			_score += Mathf.RoundToInt(LINE_SCORE * Mathf.Pow(LINE_BASE, lines - 1)
				* (1 + (Level - 1) * LEVEL_BONUS));
			scoreText.text = _score.ToString();

			//record total lines
			int startingLines = _lines;
			_lines += lines;
			linesText.text = _lines.ToString();

			//check for level up
			int startDiv = startingLines / MAX_LINES,
				endDiv = _lines / MAX_LINES;
			if (endDiv > startDiv) {
				Level += endDiv - startDiv;
				levelText.text = Level.ToString();
			}
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