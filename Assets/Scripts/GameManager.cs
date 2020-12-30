using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bricks {
	/// <summary>
	/// runs the game, maintains player score, directs the brick spawner
	/// </summary>
	public class GameManager : MonoBehaviour {
		private const int LINE_SCORE = 10,
			MULTI_LINE_BASE = 3,
			MAX_LINES = 10;
		private const float LEVEL_BONUS = .5f;

		[SerializeField] private TextMeshProUGUI scoreText, linesText, levelText, finalScoreText, startLevelText;
		[SerializeField] private GameObject playfield;
		[SerializeField] private GameObject inputHandler;
		[SerializeField] private GameObject nextBrick;
		[SerializeField] private GameObject suspendPanel, losePanel, pausePanel, homeScreen;
		[SerializeField] private GameObject brickSpawner;

		private int _score, _lines;
		private PlayerInputHandler _playerInputHandler;
		private BrickSpawner _brickSpawner;
		private Playfield _playfield;
		private bool _isHome = true;

		private int _level = 1;
		/// <summary>
		/// current game level
		/// </summary>
		private int Level {
			get => _level;
			set {
				_level = value;
				if (_brickSpawner != null) _brickSpawner.Level = value;
				levelText.text = value.ToString();
			}
		}

		private bool _isSuspended;
		/// <summary>
		/// game action is suspended (paused or game over)
		/// </summary>
		private bool IsSuspended {
			get => _isSuspended;
			set {
				_isSuspended = value;
				suspendPanel.SetActive(value);
				Time.timeScale = value ? 0 : 1;
				_playerInputHandler.CanControlBrick = !value;
			}
		}

		/// <summary>
		/// grab reference to playing field
		/// </summary>
		private void Start() {
			Time.timeScale = 1;
			_playerInputHandler = inputHandler.GetComponent<PlayerInputHandler>();
			_playfield = playfield.GetComponent<Playfield>();
			scoreText.text = _score.ToString();
			linesText.text = _lines.ToString();
			levelText.text = Level.ToString();
		}

		/// <summary>
		/// subscribe to input events
		/// </summary>
		private void OnEnable() {
			PlayerInputHandler.NotifyPause += Pause;
			PlayerInputHandler.NotifyConfirm += Confirm;
			PlayerInputHandler.NotifyChangeStartLevel += ChangeStartLevel;
			BrickController.NotifyRest += HandleRest;
		}

		/// <summary>
		/// unsubscribe from input events
		/// </summary>
		private void OnDisable() {
			PlayerInputHandler.NotifyPause -= Pause;
			PlayerInputHandler.NotifyConfirm -= Confirm;
			PlayerInputHandler.NotifyChangeStartLevel -= ChangeStartLevel;
			BrickController.NotifyRest -= HandleRest;
		}

		/// <summary>
		/// end game with a game over notification
		/// </summary>
		private void GameOver() {
			IsSuspended = true;
			losePanel.SetActive(true);
			_playerInputHandler.GameActive = false;
			finalScoreText.text = $"Final Score: {_score}";
		}

		/// <summary>
		/// handle pause button press
		/// </summary>
		private void Pause() {
			IsSuspended = !IsSuspended;
			pausePanel.SetActive(IsSuspended);
		}

		/// <summary>
		/// confirm game start or reset
		/// </summary>
		private void Confirm() {
			if (_isHome) {
				//start game
				homeScreen.SetActive(false);
				_isHome = false;
				_playerInputHandler.IsHome = false;
				playfield.SetActive(true);
				brickSpawner.SetActive(true);
				_brickSpawner = brickSpawner.GetComponent<BrickSpawner>();
				_brickSpawner.Level = Level;
				SpawnNext();
			} else if (IsSuspended) {
				//reset game
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}

		/// <summary>
		/// increase or decrease the game's starting level
		/// </summary>
		/// <param name="direction">positive: increment level; negative: decrement level</param>
		private void ChangeStartLevel(float direction) {
			if (direction > 0) {
				Level++;
			} else if (direction < 0 && Level > 1) {
				Level--;
			}

			startLevelText.text = Level.ToString();
		}

		/// <summary>
		/// record final position of brick in game grid, check for completed lines, and spawn a new brick
		/// 
		/// </summary>
		/// <param name="brick">brick to store in grid</param>
		private void HandleRest(Transform brick) {
			if (_playfield.AddToGrid(brick)) {
				int lines = _playfield.ClearLines();
				if (lines > 0) {
					UpdateScore(lines);
				}
				SpawnNext();
			} else {
				GameOver();
			}
		}

		/// <summary>
		/// spawn another brick into the playfield
		/// </summary>
		private void SpawnNext() {
			_brickSpawner.SpawnBrick(_playfield);
			UpdateNext(_brickSpawner.Next);
		}

		/// <summary>
		/// keep track of player score and update on screen
		/// increase level at fixed interval of lines
		/// </summary>
		/// <param name="lines">number of lines cleared</param>
		public void UpdateScore(int lines) {
			//score is SCORE * BASE^(n-1) * (1 + (level - 1) * BONUS)
			_score += Mathf.RoundToInt(LINE_SCORE * Mathf.Pow(MULTI_LINE_BASE, lines - 1)
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