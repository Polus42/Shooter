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
        EventManager.StartListening("OutroOver", senpaiGoneAway);
        EventManager.StartListening("PlayerDead", playerDead);
    }
    
	void Start () {
        
    }

    public GameObject uiPanelGameOver;
    public GameObject uiPanelVictory;

    GameObject messageToShow;
    bool endgame = false;


    void endGame()
    {
        endgame = true;
        currentPhase = null;
        EventManager.TriggerEvent("EndGame");
        messageToShow = uiPanelVictory;
        //uiPanelVictory.SetActive(true);
    }

    void senpaiGoneAway()
    {
        StartCoroutine(SwitchSceneToMenu());
    }

    void playerDead()
    {
        currentPhase = null;
        //uiPanelGameOver.SetActive(true);
        messageToShow = uiPanelGameOver;
        StartCoroutine(SwitchSceneToMenu());
    }

    private IEnumerator SwitchSceneToMenu()
    {
        yield return new WaitForSeconds(2f);
        messageToShow.SetActive(true);
        yield return new WaitForSeconds(2f);

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
        
        //ici car c'est l'adaptation phase qui declenche difficulty++ dans EndPhase
        if (endgame)
        {
            return;
        }

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
