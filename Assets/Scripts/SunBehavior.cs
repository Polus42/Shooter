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

    //Sprites
    public Sprite hurtSprite;
    public Sprite sleepingSprite;
    public Sprite normalSprite;
    public Sprite angrySprite;

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

    private IEnumerator coroutine;
    private List<IEnumerator> cooldowns;
    
    private Transform weakPoint;

    private bool block = false;//avoid taking damage in triggerevent

    void Awake()
    {
        updateReferences();

        cooldowns = new List<IEnumerator>();

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

        coroutine = SelectingPaterns();

        weakPoint = transform.Find("WeakPoint");
    }
    
	void Start () {
        StartCoroutine(SelectingPaterns());
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
        GetComponent<SpriteRenderer>().sprite = normalSprite;
        Debug.Log("go vulnerable");
        cc.enabled = true;
        block = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerProjectile p;
        p = other.gameObject.GetComponent<PlayerProjectile>();
        if (p!=null)
        {
            Destroy(other.gameObject);
            // Telling senpai we are stronk
            GameObject.Find("Senpai").SendMessage("On" + p.launchedby + "AttackSun");
            if (block)
                return;
            StartCoroutine(beHurt());
            currentHits++;
            if (currentHits >= sunOP.health)
            {
                block = true;
                currentHits = 0;

                EventManager.TriggerEvent("SunInjured");
            }
            Debug.Log("sun touched");
        }
    }

    IEnumerator beHurt()
    {
        if (!GetComponents<AudioSource>()[3].isPlaying)
            GetComponents<AudioSource>()[3].Play();
        GetComponent<SpriteRenderer>().sprite = hurtSprite;
        yield return new WaitForSeconds(0.1f);
        if (block)
        {
            GetComponent<SpriteRenderer>().sprite = angrySprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = normalSprite;
        }
    }
	
	void Update () {
        if(currentPattern != null)
            currentPattern.UpdatePattern();
	}

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
        yield return new WaitForSeconds(patternsOP.startWait);
        while (true)//is paused entre phase ?
        {
            chosen = Choose(probs);
            Debug.Log("test");
            currentOptions = patterns[chosen];//patterns is sorted
            Debug.Log("test");
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
                IEnumerator coo = cooldown(currentOptions.cooldown, chosen, probs[chosen]);
                cooldowns.Add(coo);
                StartCoroutine(coo);
                probs[chosen] = 0f;
            }

            yield return new WaitForSeconds(timeWait);
            
            if(currentPattern == null)
            {
                Debug.Log("wtf null");
            }
            currentPattern.EndPattern();
            currentPattern = null;

            waitAfter = patternsOP.waveWait;//Time before new pattern selected
            if(currentOptions.waitAfter > 0)
                waitAfter = currentOptions.waitAfter;

            yield return new WaitForSeconds(waitAfter);
        }
    }

    private void onChangingPhase()
    {
        updateReferences();
        cleanCooldowns();
        StartCoroutine(launchSelectingPatterns(sunOP.pauseTimeBetweenPhase));
    }

    private void cleanCooldowns()
    {
        foreach(IEnumerator coo in cooldowns)
        {
            StopCoroutine(coo);
        }
    }

    private IEnumerator launchSelectingPatterns(float timePaused)
    {
        Debug.Log("phased changed, launch selecting patterns or paused it");
        StopCoroutine(coroutine);
        yield return new WaitForSeconds(timePaused);
        StartCoroutine(coroutine);
    }

    private IEnumerator cooldown(float cooldown, int index, float initTime)
    {
        yield return new WaitForSeconds(cooldown);
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
