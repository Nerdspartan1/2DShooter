using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum MenuTab { start, select_selection, select_desc, options, pause, gameOver, levelCleared} 

public class GameManager : MonoBehaviour {

	private static GameManager instance = null;
	private static GameObject eventSystem = null;

	private MenuTab menuTab;

	private GameObject mainMenu;
	private GameObject startMenu;
	private GameObject selectMenu;
	private GameObject levelSelect;
	private GameObject levelDesc;

	private Button newGameButtonMain;
	private Button exitButtonMain;


	private GameObject pauseMenu;
	private GameObject gameOverMenu;
	private GameObject levelClearedMenu;
	private GameObject sceneClearedMenu;
	public Level[] levels;
	public Button levelButton;
	private Button[] levelButtons;
	private int levelReached = 1;

	public AudioClip levelClearedST;
	private AudioSource audioSource;

	private Fading fading;

	public int enemiesAlive;
	public bool levelPlaying = false;
	public bool levelCleared = false;
	public bool sceneCleared = false;
	public GameObject nextScene;

	public GameObject[] tier1;
	public GameObject[] tier2;
	public GameObject[] tier3;

	public bool gamePaused = false;
	private float timeScaleBeforePause = 1f;



	void Awake ()
	{
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (instance);
		} else if (instance != this) {
			Destroy (gameObject);
		}
		if (eventSystem == null) {
			eventSystem = GameObject.Find ("EventSystem");
			if (eventSystem == null) {
				eventSystem = Instantiate (Resources.Load ("EventSystem") as GameObject);
			}
			DontDestroyOnLoad (eventSystem);
		}
	}

	public static GameManager Instance
	{
		get{
			if(instance == null){

				instance = FindObjectOfType<GameManager>();

				if (instance == null) {
					GameObject go = Instantiate(Resources.Load("GameManager") as GameObject);
					instance = go.GetComponent<GameManager>();
					DontDestroyOnLoad (instance);
				}
			}

			return instance;
		}
	}

	void Start ()
	{
		

		mainMenu = transform.Find ("MainMenu").gameObject;
		selectMenu = transform.Find ("MainMenu/Select").gameObject;
		startMenu = transform.Find ("MainMenu/Start").gameObject;
		levelSelect = transform.Find ("MainMenu/Select/LevelSelect").gameObject;
		levelDesc = transform.Find ("MainMenu/Select/LevelDesc").gameObject;

		pauseMenu = transform.Find ("PauseMenu").gameObject;
		gameOverMenu = transform.Find ("GameOverMenu").gameObject;
		sceneClearedMenu = transform.Find ("SceneClearedMenu").gameObject;
		levelClearedMenu = transform.Find ("LevelClearedMenu").gameObject;
		audioSource = GetComponent<AudioSource> ();

		fading = GetComponent<Fading> ();

		if (LevelIndex != -1) {
			StartScene ();
		}

		levelButtons = new Button[levels.Length];
		Transform content = levelSelect.transform.Find ("Background/ScrollRect/Content");
		for (int i = 0; i < levels.Length; i++) {
			Button b = Instantiate (levelButton, content);
			b.GetComponentInChildren<Text> ().text = levels [i].levelName;
			int j = i;
			b.onClick.AddListener (delegate {OpenLevelDesc (j);});
			if (i > levelReached) {
				b.interactable = false;
			}
			levelButtons[i] = b;
		}

	}

	void Update ()
	{
		audioSource.pitch = Mathf.Pow (Time.timeScale, 0.25f);

		if (levelPlaying) {
			if (gameOverMenu.activeInHierarchy) {
				ShowLevelCleared (false);
				if (Input.GetKeyDown (KeyCode.R)) {
					levelPlaying = false;
					Restart();

				}
			} else if (!sceneCleared && enemiesAlive == 0) {
				sceneCleared = true;
				nextScene.SetActive (true);
				ShowSceneCleared(true);
				if (levels [LevelIndex].sceneNames.Length - 1 == SceneIndex) {
					levelCleared = true;
					audioSource.Stop ();
					audioSource.clip = levelClearedST;
					audioSource.Play ();
					ShowSceneCleared(false);
					ShowLevelCleared (true);
				}	
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if(gamePaused) Resume();
				else Pause();
			}
		}

	}

	public void Restart ()
	{
		Load (LevelIndex, SceneIndex);
		Resume();
	}

	public void LoadMainMenu ()
	{
		Load(-1,0);

	}

	public void LoadNextLevel ()
	{
		int i = LevelIndex+1;
		if (i < levels.Length) {



		}
	}


	public void LoadPreviousScene ()
	{
		if (SceneIndex > 0) {
			Load(LevelIndex,SceneIndex-1);
		}
	}

	public void LoadNextScene ()
	{
		if (SceneIndex + 1 < levels[LevelIndex].sceneNames.Length) {
			Load(LevelIndex,SceneIndex+1);

		}
	}

	public void Load (int levelIndex,int sceneIndex)
	{
		StartCoroutine(LoadCoroutine(levelIndex,sceneIndex));
	}


	IEnumerator LoadCoroutine (int levelIndex, int sceneIndex)
	{
		yield return new WaitForSecondsRealtime (1 / fading.BeginFade (1));
		if (levelIndex != -1) {
			SceneManager.LoadScene (levels [levelIndex].sceneNames [sceneIndex]);
			yield return null;
			StartScene ();
		} else {
			SceneManager.LoadScene ("MainMenu");
			levelCleared = false;
			sceneCleared = false;
			levelPlaying = false;
			audioSource.Stop();
			HideAll();
			fading.BeginFade(-1);
			Resume();
			Time.timeScale = 1f;
			ShowMainMenu(true);
			CloseLevelDesc();



		}
	}

	public void StartScene ()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		enemiesAlive = enemies.Length;
		levelCleared = false;
		sceneCleared = false;
		levelPlaying = true;
		nextScene = GameObject.Find ("ChangeScene");
		nextScene.SetActive (false);
		HideAll ();
		if (LevelIndex < levels.Length) {
			if (audioSource.clip != levels[LevelIndex].music) {
				audioSource.Stop();
				audioSource.clip = levels[LevelIndex].music;
				audioSource.Play ();
			}else if (!audioSource.isPlaying ) {
				audioSource.Play ();
			}
		}
		fading.BeginFade(-1);
	}


	public void OpenSelectMenu ()
	{
		startMenu.SetActive(false);
		selectMenu.SetActive(true);
	}

	public void CloseSelectMenu ()
	{
		startMenu.SetActive(true);
		selectMenu.SetActive(false);
	}


	public void OpenLevelDesc (int index)
	{
		levelSelect.SetActive(false);
		levelDesc.SetActive(true);
		levelDesc.transform.Find("DescBackground/Description").GetComponent<Text>().text = levels[index].description;
		levelDesc.transform.Find("NameBackground/Name").GetComponent<Text>().text = levels[index].levelName;
		Button b = levelDesc.transform.Find("StartButton").GetComponent<Button>();
		b.onClick.RemoveAllListeners();
		b.onClick.AddListener(delegate{Load(index,0);});


	}

	public void CloseLevelDesc ()
	{
		levelSelect.SetActive(true);
		levelDesc.SetActive(false);
	}


	public void ShowMainMenu (bool val)
	{
		mainMenu.SetActive(val);
	}

	public void Pause ()
	{
		pauseMenu.SetActive(true);
		timeScaleBeforePause = Time.timeScale;
		Time.timeScale = 0;
		gamePaused = true;
	}

	public void Resume ()
	{
		pauseMenu.SetActive(false);
		Time.timeScale = timeScaleBeforePause;
		gamePaused = false;
	}

	public void ShowSceneCleared (bool val)
	{
		sceneClearedMenu.SetActive(val);
	}

	public void ShowLevelCleared (bool val)
	{
		levelClearedMenu.SetActive(val);
	}

	public void ShowGameOver (bool val)
	{
		gameOverMenu.SetActive(val);
	}

	public void HideAll ()
	{	
		ShowMainMenu(false);
		ShowGameOver(false);
		ShowSceneCleared(false);
		ShowLevelCleared(false);
	}


	public int LevelIndex {
		get {
			if (SceneManager.GetActiveScene ().name == "MainMenu")
				return -1;
			else {
				for (int j = 0; j < levels.Length; j++) {
					for (int i = 0; i < levels [j].sceneNames.Length; i++) {
						if (SceneManager.GetActiveScene ().name == levels [j].sceneNames [i]) {
							return j;
						}
					}
				}
			
				return -2;
			}
		}
	}

	public int SceneIndex {
		get {
			int levelIndex = LevelIndex;
			for (int i = 0; i < levels [levelIndex].sceneNames.Length; i++) {
				if (SceneManager.GetActiveScene ().name == levels [levelIndex].sceneNames [i]) {
					return i;
				}
			} 
			return -2;
		}
	}

	public int LevelReached {
		get{ return levelReached;}
		set{ 
			levelReached = value;
			for (int i = 0; i < levels.Length; i++) {
				levelButtons[i].interactable = i <= levelReached;
			} 
		}
	}

	public GameObject GetRandomWeapon (int tier)
	{
		if (tier == 3) {
			int r = Random.Range (0, tier3.Length);
			return tier3 [r];
		} else if (tier == 2) {
			int r = Random.Range (0, tier2.Length);
			return tier2 [r];
		} else if(tier == 1){
			int r= Random.Range(0,tier1.Length);
			return tier1[r];
		} else{
			return null;
		}

	}

	public void Exit ()
	{
		Application.Quit();
	}

}
