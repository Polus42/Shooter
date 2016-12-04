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

    private bool canMove = false;
    
    private CircleCollider2D cc;
    private int currentHits = 0;

    private IPattern currentPattern;
    private Dictionary<string, IPattern> patternsAvailable = new Dictionary<string, IPattern>();

    private Coroutine lastRoutine = null;//holding selecting patterns coroutine
    private Coroutine launchSelecting;//holding launching selecting patterns coroutine
    private List<Coroutine> cooldowns;

    private Transform weakPoint;

    private bool block = false;//avoid taking damage in triggerevent

    //Rotation
    private int _currentdirection = -1;
    private float timeToWait = 0f;
    private Coroutine rotateInitial;

    void Awake()
    {
        updateReferences();

        cooldowns = new List<Coroutine>();

        RotationPattern rotationPattern = new RotationPattern(this);
        CyclicPattern cyclicPattern = new CyclicPattern(this);
        WavePattern wavePattern = new WavePattern(this);
        LaserPattern laserPattern = new LaserPattern(this);

        patternsAvailable.Add("RotationPattern", rotationPattern);
        patternsAvailable.Add("CyclicPattern", cyclicPattern);
        patternsAvailable.Add("WavePattern", wavePattern);
        patternsAvailable.Add("LaserPattern", laserPattern);

        //Take damage or not
        EventManager.StartListening("OnSurvivalPhase", goVulnerable);
        EventManager.StartListening("OnCounterPhase", goInvicible);
        EventManager.StartListening("OnCounterPhase", growWeakPoint); 
        EventManager.StartListening("OnAdaptationPhase", goInvicible);

        //Pause between each phase
        EventManager.StartListening("OnSurvivalPhase", onChangingPhase);
        EventManager.StartListening("OnCounterPhase", onChangingPhase);
        EventManager.StartListening("OnAdaptationPhase", onChangingPhase);

        //Rotating or not
        EventManager.StartListening("OnSurvivalPhase", restrictMovement);
        EventManager.StartListening("OnCounterPhase", allowMovement);
        EventManager.StartListening("OnAdaptationPhase", restrictMovement);

        cc = GetComponent<CircleCollider2D>();

        //coroutine = SelectingPaterns();
        GetComponent<SpriteRenderer>().sprite = sleepingSprite;

        weakPoint = transform.Find("WeakPoint");
    }

    void Start()
    {
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
        GetComponent<SpriteRenderer>().sprite = angrySprite;
        Debug.Log("go invicible");
        //cc.enabled = false;
        block = true;
    }

    private void growWeakPoint()
    {
        Debug.Log("weak point name: " + weakPoint.name);
        weakPoint.gameObject.SetActive(true);
    }

    private void goVulnerable()
    {
        GetComponent<SpriteRenderer>().sprite = normalSprite;
        Debug.Log("go vulnerable");
        //cc.enabled = true;
        block = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerProjectile p;
        p = other.gameObject.GetComponent<PlayerProjectile>();
        if (p!=null)
        {
            Destroy(other.gameObject);
            
            if (block)
                return;

            // Telling senpai we are stronk
            GameObject.Find("Senpai").SendMessage("On" + p.launchedby + "AttackSun");

            if (beHurtCo != null)
                StopCoroutine(beHurtCo);
            beHurtCo = StartCoroutine(beHurt());
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

    private Coroutine beHurtCo;

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
        if(canMove)
            moveRandom();

        if (currentPattern != null)
            currentPattern.UpdatePattern();
	}

    //-------------------MOVEMENT
    void moveRandom()// => min max duration par mouvement (l,r,static)
    {
        //Debug.Log("sunOP " + sunOP.minWaitMove + sunOP.maxWaitMove + sunOP.timeStatic + sunOP.rotatingSpeed);
        timeToWait += Time.deltaTime;
        if(timeToWait >= 0f)
        {
            _currentdirection = Random.Range(-1, 2);//even if already static, he can remain static
            if(_currentdirection == 0)
            {
                timeToWait = -sunOP.timeStatic;
            }
            else
            {
                timeToWait = -Random.Range(sunOP.minWaitMove, sunOP.maxWaitMove);
            }
        }
        if (_currentdirection == 1)
        {
            goLeft();
        }
        else if (_currentdirection == -1)
        {
            goRight();
        }
    }

    void goLeft()
    {
        transform.Rotate(new Vector3(0, 0, sunOP.rotatingSpeed * Time.deltaTime));
        //transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z + -10f * Time.deltaTime);
    }

    void goRight()
    {
        transform.Rotate(new Vector3(0, 0, -sunOP.rotatingSpeed * Time.deltaTime));
        //transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z + 10f * Time.deltaTime);
    }

    IEnumerator rotateToInitial()
    {
        /*int leftOrRight = 1;
        
        if (transform.rotation.z % 180 >= 0)
        {
            leftOrRight = -1;
        }
        */
        float elapsedTime = 0f;
        float duration = 3f;
        float initZ = transform.rotation.z;
        //Debug.Log("rotate init");
        while (elapsedTime <= duration)
        {
            //Debug.Log("rotate: " + transform.rotation.z);
            //transform.Rotate(new Vector3(0, 0, Mathf.LerpAngle(initZ, 0, elapsedTime / duration)));
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(0.0166f);
        }
        //transform.Rotate(new Vector3(0, 0, 0));
        yield return new WaitForSeconds(0.5f);
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
        Debug.Log("startWait: " + patternsOP.startWait);
        yield return new WaitForSeconds(patternsOP.startWait);
        while (true)//is paused entre phase ?
        {
            Debug.Log("Chose next pattern");

            chosen = Choose(probs);
            currentOptions = patterns[chosen];//patterns is sorted
            //Debug.Log("test");
            Debug.Log("patterns count: " + patterns.Count);

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

            if (currentOptions.cooldown > 0)
            {
                //IEnumerator coo = cooldown(currentOptions.cooldown, chosen, probs[chosen]);
                //cooldowns.Add(coo);
                Debug.Log("launch cooldown: " + currentOptions.cooldown);
                cooldowns.Add(StartCoroutine(cooldown(currentOptions.cooldown, chosen, probs[chosen])));
                probs[chosen] = 0f;
            }
            Debug.Log("timeWait: " + timeWait);
            yield return new WaitForSeconds(timeWait);

            if (currentPattern == null)
            {
                Debug.Log("wtf null");
            }
            currentPattern.EndPattern();
            Debug.Log("Devient null");
            currentPattern = null;

            waitAfter = patternsOP.waveWait;//Time before new pattern selected
            if (currentOptions.waitAfter > 0)
                waitAfter = currentOptions.waitAfter;
            Debug.Log("wait after: " + waitAfter);
            yield return new WaitForSeconds(waitAfter);
        }
    }

    private void cleanCooldowns()
    {
        foreach (Coroutine coo in cooldowns)
        {
            StopCoroutine(coo);
        }
    }
    
    private void restrictMovement()
    {
        if(canMove)
        {
            canMove = false;
            launchRotateInitial();
        }
        
    }

    public void launchRotateInitial()
    {
        if (rotateInitial != null)
            StopCoroutine(rotateInitial);
        rotateInitial = StartCoroutine(rotateToInitial());
    }

    private void allowMovement()
    {
        canMove = true;
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
