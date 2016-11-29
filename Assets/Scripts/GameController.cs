using UnityEngine;
using System.Collections;

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
    }
    
	void Start () {
        switch (startPhase)
        {
            case "SurvivalPhase":
                goToSurvivalPhase();
                break;
            case "CounterPhase":
                goToCounterPhase();
                break;
            case "AdaptionPhase":
                goToAdaptationPhase();
                break;
            default:
                goToSurvivalPhase();
                break;
        }
    }
	
	void Update () {
        currentPhase.UpdatePhase();
    }

    //triggered by adaptationphase
    public void UpdateDifficulty()
    {
        currentLoopDifficulty++;
        if (currentLoopDifficulty > maxDifficulty - 1)
        {
            Debug.Log("end game");
        }
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
