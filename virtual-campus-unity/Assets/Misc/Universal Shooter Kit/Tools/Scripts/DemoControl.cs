using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	public class DemoControl : MonoBehaviour
	{
		public GameObject mainCanvas;
		public GameObject firstBoard;
		public GameObject secondBoard;

		public Button resume;
		public Button start;

		[HideInInspector] public GameManager gameManager;
		[HideInInspector] public Lobby lobby;
		[HideInInspector] public RoomManager roomManager;
		[HideInInspector] public Controller controller;

		private bool showMenu;
		private bool canShowMenu;
		private bool canShowMultiplayerMenu;
		private bool showInputMenu;

		private void Awake()
		{
			canShowMenu = CheckPlayerPrefs("ShowMenu");
			canShowMultiplayerMenu = CheckPlayerPrefs("ShowMultiplayerMenu");
		}

		void Start()
		{
			if (FindObjectOfType<GameManager>())
			{
				gameManager = FindObjectOfType<GameManager>();
			}
			else if (FindObjectOfType<Lobby>())
			{
				lobby = FindObjectOfType<Lobby>();
			}
			else if (FindObjectOfType<RoomManager>())
			{
				roomManager = FindObjectOfType<RoomManager>();
			}

			if (gameManager && canShowMenu || lobby && canShowMultiplayerMenu || roomManager)
			{
				if (mainCanvas)
					mainCanvas.SetActive(true);

				if (roomManager)
				{
					if (firstBoard)
						firstBoard.SetActive(false);

					if (secondBoard)
						secondBoard.SetActive(false);
				}
				else
				{
					if (firstBoard)
						firstBoard.SetActive(true);

					if (secondBoard)
						secondBoard.SetActive(false);

					if (resume)
						resume.onClick.AddListener(OpenNextBoard);

					if (start)
						start.onClick.AddListener(StartGame);
				}
			}
			else
			{
				if (mainCanvas)
					mainCanvas.SetActive(false);

				return;
			}

			if (gameManager)
				gameManager.gameStarted = false;

		}

		void OpenNextBoard()
		{
			if (firstBoard)
				firstBoard.SetActive(false);

			if (secondBoard)
				secondBoard.SetActive(true);

			if (lobby)
			{
				StartCoroutine(ShowInfoTimer());
			}
		}

		void StartGame()
		{
			if (gameManager)
			{
				if (mainCanvas)
					mainCanvas.SetActive(false);


				gameManager.Pause(false);
				gameManager.gameStarted = true;
			}
		}

		IEnumerator ShowInfoTimer()
		{
			yield return new WaitForSeconds(20);

			if (mainCanvas)
				mainCanvas.SetActive(false);
		}

		bool CheckPlayerPrefs(string value)
		{
			if (PlayerPrefs.HasKey(value))
			{
				if (PlayerPrefs.GetInt(value) == 0)
				{
					PlayerPrefs.SetInt(value, 1);
					return true;
				}

				return false;
			}

			PlayerPrefs.SetInt(value, 1);
			return true;
		}



		void Update()
		{
#if PHOTON_UNITY_NETWORKING
			if (lobby && lobby.openAnyMenu && mainCanvas.activeSelf)
			{
				StopAllCoroutines();

				if (mainCanvas)
					mainCanvas.SetActive(false);
			}

			if (roomManager)
			{
				if (Input.GetKeyDown(KeyCode.F1))
				{
					showInputMenu = true;
				}
				else if (Input.GetKeyUp(KeyCode.F1))
				{
					showInputMenu = false;
				}

				if (roomManager && !roomManager.isPause && (roomManager.GameStarted && roomManager.Player || !roomManager.GameStarted && roomManager.preMatchTimer > 0))
				{
					if (firstBoard)
						firstBoard.SetActive(showInputMenu);
				}
				else
				{
					if (firstBoard)
						firstBoard.SetActive(false);
				}
			}
#endif

			if (gameManager && canShowMenu) //|| lobby && canShowMultiplayerMenu) //|| !singlePlayer && roomManager && canShowInputMenu)
			{
				if (!showMenu)
				{
					controller = gameManager.controllers[gameManager.CurrentCharacter];
					controller.CameraController.canUseCursorInPause = true;
					gameManager.Pause(false);
					showMenu = true;
				}
			}
		}
	}
}
