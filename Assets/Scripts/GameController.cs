using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    private IMainPhase currentPhase;
    private AdaptationPhase adaptationPhase;
    private CounterPhase counterPhase;
    private SurvivalPhase survivalPhase;

    public TextAsset options;//link to the options.txt
    public string startPhase = "SurvivalPhase";
    public int startDifficulty = 0;
    public int maxDifficulty = 4; // 4 weak point a detruire

    private int currentLoopDifficulty;//++ a chaque retour survival state

    public GameObject asteroidPrefab;

    void Awake() {
        //difficultyManager = new DifficultyManager(startDifficulty, startPhase, options);
        OptionsManager.Instance.init(startDifficulty, startPhase, options);
        currentLoopDifficulty = startDifficulty;
        adaptationPhase = new AdaptationPhase(this);
        counterPhase = new CounterPhase(this);
        survivalPhase = new SurvivalPhase(this);

        EventManager.StartListening("WeakPointDestroyed", goToAdaptationPhase);
        EventManager.StartListening("SunInjured", goToCounterPhase);
        EventManager.StartListening("AdaptationEnded", goToSurvivalPhase);

        EventManager.StartListening("IntroOver", startPhases);

        EventManager.StartListening("PlayerDead", playerDead);
    }
    
	void Start () {
        
    }

    public GameObject uiPanelGameOver;
    public GameObject uiPanelVictory;

    void endGame()
    {
        currentPhase = null;
        EventManager.TriggerEvent("EndGame");
        //uiPanelVictory.SetActive(true);
    }

    void playerDead()
    {
        currentPhase = null;
        uiPanelGameOver.SetActive(true);
        //SceneManager.LoadScene("Menu");
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        //load new scene
        AsyncOperation loadNewScene = SceneManager.LoadSceneAsync("Menu");

        while (!loadNewScene.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        print("Scene Loaded");

        //unload current/old scene
        bool unloaded = SceneManager.UnloadScene(0);

        while (!unloaded)
        {
            yield return new WaitForEndOfFrame();
        }

        print("Scene Unloaded");

        //unload the unused assets/lightmaps
        AsyncOperation unloadCurrentSceneAssets = Resources.UnloadUnusedAssets();

        while (!unloadCurrentSceneAssets.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        print("Unused Assets Removed");
    }

    void startPhases()
    {
        switch (startPhase)
        {
            case "SurvivalPhase":
                goToSurvivalPhase();
                break;
            case "CounterPhase":
                goToCounterPhase();
                break;
            case "AdaptationPhase":
                goToAdaptationPhase();
                break;
            default:
                goToSurvivalPhase();
                break;
        }
    }

    void Update () {
        if(currentPhase != null)
            currentPhase.UpdatePhase();
    }

    //triggered by adaptationphase
    public void UpdateDifficulty()
    {
        currentLoopDifficulty++;
        if (currentLoopDifficulty > maxDifficulty - 1)
        {
            Debug.Log("END GAME");
            endGame();
        }
        else
            OptionsManager.Instance.changeDifficulty(currentLoopDifficulty);
    }

    public void goToAdaptationPhase()
    {
        if (currentPhase != null)
            currentPhase.EndPhase();
        currentPhase = adaptationPhase;
        currentPhase.InitPhase();

        OptionsManager.Instance.changePhase("AdaptationPhase");

        EventManager.TriggerEvent("OnAdaptationPhase");
        Debug.Log("ADAPTATION PHASE");
    }

    public void goToSurvivalPhase()
    {
        if(currentPhase != null)
            currentPhase.EndPhase();
        currentPhase = survivalPhase;
        currentPhase.InitPhase();

        OptionsManager.Instance.changePhase("SurvivalPhase");

        EventManager.TriggerEvent("OnSurvivalPhase");
        Debug.Log("SURVIVAL PHASE");
    }

    public void goToCounterPhase()
    {
        if (currentPhase != null)
            currentPhase.EndPhase();
        currentPhase = counterPhase;
        currentPhase.InitPhase();

        OptionsManager.Instance.changePhase("CounterPhase");

        EventManager.TriggerEvent("OnCounterPhase");
        
        Debug.Log("COUNTER PHASE");
    }
}
