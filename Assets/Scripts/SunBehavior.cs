using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof(CircleCollider2D))]
public class SunBehavior : MonoBehaviour {
    public GameObject[] typeProjectiles;

    //References
    private OptionsHolder.SunOP sunOP;
    private OptionsHolder.PatternsOP patternsOP;
    private List<OptionsHolder.IOptionPattern> patterns;
    private List<float> probs;

    //Used by patterns
    public float force
    {
        get
        {
            return sunOP.force;
        }
        set { }
    }
    
    private CircleCollider2D cc;
    private int currentHits = 0;

    private IPattern currentPattern;
    private Dictionary<string, IPattern> patternsAvailable = new Dictionary<string, IPattern>();

    //private IEnumerator coroutine
    private Coroutine lastRoutine = null;//holding selecting patterns coroutine
    private Coroutine launchSelecting;//holding launching selecting patterns coroutine
    //private List<IEnumerator> cooldowns;
    private List<Coroutine> cooldowns;

    private Transform weakPoint;

    private bool block = false;//avoid taking damage in triggerevent

    void Awake()
    {
        updateReferences();

        cooldowns = new List<Coroutine>();

        RotationPattern rotationPattern = new RotationPattern(this);
        CyclicPattern cyclicPattern = new CyclicPattern(this);
        WavePattern wavePattern = new WavePattern(this);

        patternsAvailable.Add("RotationPattern", rotationPattern);
        patternsAvailable.Add("CyclicPattern", cyclicPattern);
        patternsAvailable.Add("WavePattern", wavePattern);

        EventManager.StartListening("OnSurvivalPhase", goVulnerable);
        EventManager.StartListening("OnCounterPhase", goInvicible);

        //Pause between each phase
        EventManager.StartListening("OnSurvivalPhase", onChangingPhase);
        EventManager.StartListening("OnCounterPhase", onChangingPhase);
        EventManager.StartListening("OnAdaptationPhase", onChangingPhase);

        cc = GetComponent<CircleCollider2D>();

        //coroutine = SelectingPaterns();

        weakPoint = transform.Find("WeakPoint");
    }
    
	void Start () {
        //StartCoroutine(SelectingPaterns());
    }

    private void updateReferences()
    {
        OptionsManager.Instance.getSunOptions(out sunOP);
        OptionsManager.Instance.getPatternsOptions(out patternsOP);
        OptionsManager.Instance.getCurrentPatterns(out patterns);
        OptionsManager.Instance.getProbs(out probs);
    }

    private void goInvicible()
    {
        Debug.Log("go invicible");
        cc.enabled = false;

        Debug.Log("weak point name: " + weakPoint.name);
        weakPoint.gameObject.SetActive(true);
        
    }

    private void goVulnerable()
    {
        Debug.Log("go vulnerable");
        cc.enabled = true;
        block = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Test only
        if (block)
            return;
        currentHits++;
        if(currentHits >= sunOP.health)
        {
            block = true;
            currentHits = 0;
           
            EventManager.TriggerEvent("SunInjured");
        }
        Debug.Log("sun touched");
        
        
    }
	
	void Update () {

        moveRandom();
        if (_currentdirection == 1)
        {
            goLeft();
        }
        else if (_currentdirection == -1)
        {
            goRight();
        }

        if (currentPattern != null)
            currentPattern.UpdatePattern();
	}

    //-------------------
    int _currentdirection = -1;
    void moveRandom()
    {
        if (Random.Range(0, 100) == 0)
        {
            _currentdirection = Random.Range(-1, 2);
        }
    }
    void goLeft()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z + -10f * Time.deltaTime);
    }
    void goRight()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z + 10f * Time.deltaTime);
    }
    //-------------------


    /*
    private float startWait = 1f;
    private float waveWait = 5f;
    private float spawnWait = 2f;
    private float loopingCount = 5;//depending difficulty
    */

    //coroutine for selecting patterns
    IEnumerator SelectingPaterns()
    {
        /*
        Debug.Log("start " + startWait);
        Dictionary<string, float> myDict = new Dictionary<string, float>();
        myDict.Add("RotationPattern", 0.3f);
        myDict.Add("CyclicPattern", 0.3f);
        myDict.Add("WavePattern", 0.3f);

        List<KeyValuePair<string, float>> myList = myDict.ToList();

        myList.Sort((firstPair, nextPair) =>
        {
            return firstPair.Value.CompareTo(nextPair.Value);
        });

        float[] probs = new float[myList.Count];
        int index = 0;

        foreach (KeyValuePair<string, float> entry in myList)
        {
            probs[index] = entry.Value;
            index++;
        }
        */
        OptionsHolder.IOptionPattern currentOptions;
        int chosen;
        float timeWait, waitAfter;
        Debug.Log("startWait: "+ patternsOP.startWait);
        yield return new WaitForSeconds(patternsOP.startWait);
        while (true)//is paused entre phase ?
        {
            Debug.Log("Chose next pattern");

            chosen = Choose(probs);
            currentOptions = patterns[chosen];//patterns is sorted
            //Debug.Log("test");
            Debug.Log("patterns count: "+ patterns.Count);

            Debug.Log("prob : " + chosen);
            Debug.Log("pattern chosen: " + currentOptions.name);
            Debug.Log("pattern chosen proba: " + currentOptions.probability);

            patternsAvailable.TryGetValue(currentOptions.name, out currentPattern);
            currentPattern.setOptions(currentOptions);

            timeWait = patternsOP.waveWait;//Duration of the pattern
            if (currentOptions.durationMin > 0 && currentOptions.durationMax > currentOptions.durationMin)
            {
                timeWait = Random.Range(currentOptions.durationMin, currentOptions.durationMax);
            }

            if(currentOptions.cooldown > 0)
            {
                //IEnumerator coo = cooldown(currentOptions.cooldown, chosen, probs[chosen]);
                //cooldowns.Add(coo);
                Debug.Log("launch cooldown: "+ currentOptions.cooldown);
                cooldowns.Add(StartCoroutine(cooldown(currentOptions.cooldown, chosen, probs[chosen])));
                probs[chosen] = 0f;
            }
            Debug.Log("timeWait: " + timeWait);
            yield return new WaitForSeconds(timeWait);
            
            if(currentPattern == null)
            {
                Debug.Log("wtf null");
            }
            currentPattern.EndPattern();
            Debug.Log("Devient null");
            currentPattern = null;

            waitAfter = patternsOP.waveWait;//Time before new pattern selected
            if(currentOptions.waitAfter > 0)
                waitAfter = currentOptions.waitAfter;
            Debug.Log("wait after: " + waitAfter);
            yield return new WaitForSeconds(waitAfter);
        }
    }

    private void cleanCooldowns()
    {
        foreach(Coroutine coo in cooldowns)
        {
            StopCoroutine(coo);
        }
    }


    

    private void onChangingPhase()
    {
        updateReferences();
        cleanCooldowns();

        //StopCoroutine(coroutine);
        //StartCoroutine(coroutine);

        if (launchSelecting != null)
        {
            StopCoroutine(launchSelecting);
        }

        Debug.Log("stop coroutine");
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        if (currentPattern != null)
        {
            Debug.Log("force ending pattern");
            currentPattern.EndPattern();
            currentPattern = null;
        }

        launchSelecting = StartCoroutine(launchSelectingPatterns(sunOP.pauseTimeBetweenPhase));
    }

    private IEnumerator launchSelectingPatterns(float timePaused)
    {
        Debug.Log("phased changed, launch selecting patterns or paused it for: " + timePaused);
        /*Debug.Log("stop coroutine");
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        if (currentPattern != null)
        {
            Debug.Log("force ending pattern");
            currentPattern.EndPattern();
            currentPattern = null;
        }
        */
        yield return new WaitForSeconds(timePaused);
        Debug.Log("start coroutine");
        //StartCoroutine(coroutine);
        lastRoutine = StartCoroutine(SelectingPaterns());
    }
    

    private IEnumerator cooldown(float cooldown, int index, float initTime)
    {
        yield return new WaitForSeconds(cooldown);
        Debug.Log("coooldown over");
        probs[index] = initTime;
    }
    
    //Choosing Items with Different Probabilities, return index chosen
    private int Choose(List<float> probs)
    {
        float total = 0;
        foreach (float elem in probs)
        {
            total += elem;
        }
        float randomPoint = Random.value * total;
        for (int i = 0; i < probs.Count; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Count - 1;
    }
}
