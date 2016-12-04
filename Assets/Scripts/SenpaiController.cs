using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SenpaiController : MonoBehaviour {
    public int rotateSpeed;
    public int startingHealth = 10;
    public Vector2 messageSize;
    // Sprites
    public Sprite happySprite;
    public Sprite angrySprite;
    public Sprite neutralSprite;
    public Sprite touchedSprite;
    // Messages list
    public GUISkin messages;
    private int _currentdirection;
    private int _health;
    private string _whatimsaying = "";
    public int pointsOnDestroyProjectile = 1;
    public int pointsOnWeakpointDestroyed = 100;
    // Paramètres missions
    public float missionTime = 10;
    public float missionTextTime = 1.5f;
    public int PF_lost = 100;
    public float minTimeBetweenMission = 4;
    public float maxTimeBetweenMission = 6;
    public int pointsLostOnTouchSenpai = 100;
    // to check if a mission is currently launched
    private bool _MissionDestroyWeakpoint = false;
    public string destroyWeakpointPhrase = "";
    // mission attack sun
    private bool _MissionAttackSun = false;
    private int[] _playersTouchedSun = new int[2];
    public string attackSunPhrase = "";
    public int SunAttackAmount = 10;
    // mission stay near me
    private bool _MissionStayNearMe =false;
    public string staynearmePhrase ="";
    public float stayCloseToMeTime = 5;
    private float _P1Entered = Mathf.Infinity; // time when P1 entered the zone around SENPAIIIIIIIIII
    private float _P2Entered = Mathf.Infinity;
    // Mission Stop Fireing
    private bool _MissionStopFireing = false;
    public string stopFireingPhrase = "";
    private bool _P1Fired;
    private bool _P2Fired;
    // Mission getAway
    private bool _MissionGetAway = false;
    public string getAwayPhrase = "";
    public float getAwayTime = 5;
    private float _P1EnteredOpposed = Mathf.Infinity; // time when P1 entered the zone around SENPAIIIIIIIIII
    private float _P2EnteredOpposed = Mathf.Infinity;
    // Rancune
    private bool _rancuneP1 = false;
    private bool _rancuneP2 = false;

    infoScore InfoScore;
    
    public GameObject greyBackground;

    //Rajout pour missions "dynamiques" en fonction de la phase
    private List<List<UnityAction>> missionsAvailable;
    private List<UnityAction> currentMissions;
    private UnityAction lastMission = null;
    private Coroutine currentCoroutineMission;
    //Messages en attente d'affichage
    private List<IEnumerator> messagesWaiting;
    private Coroutine isSayingSomething;

    public float slowTimeScale = 0.1f;
    private Coroutine blink;
    private bool canBeTouched = true;

    void Awake()
    {
        messagesWaiting = new List<IEnumerator>();

        // Declare the list of missions per phase
        missionsAvailable = new List<List<UnityAction>>();
        missionsAvailable.Add(new List<UnityAction>());
        missionsAvailable.Add(new List<UnityAction>());
        missionsAvailable.Add(new List<UnityAction>());

        // Add two delegates to the list that point to missions
        //Survival Phase
        missionsAvailable[0].Add(() => startMissionStayNearMe(missionTime));
        missionsAvailable[0].Add(() => startMissionGetAway(missionTime));
        missionsAvailable[0].Add(() => startMissionStopFireing(missionTime));
        missionsAvailable[0].Add(() => startMissionAttackSun(missionTime));
        //Counter Phase
        missionsAvailable[1].Add(() => startMissionDestroyWeakpoint(missionTime));
        missionsAvailable[1].Add(() => startMissionStayNearMe(missionTime));
        missionsAvailable[1].Add(() => startMissionGetAway(missionTime));
        missionsAvailable[1].Add(() => startMissionStopFireing(missionTime));
        //Adaptation Phase
        missionsAvailable[2].Add(() => startMissionStayNearMe(missionTime));
        missionsAvailable[2].Add(() => startMissionGetAway(missionTime));
        missionsAvailable[2].Add(() => startMissionStopFireing(missionTime));

        EventManager.StartListening("OnSurvivalPhase", () => switchPhase(0));
        EventManager.StartListening("OnCounterPhase", () => switchPhase(1));
        EventManager.StartListening("OnAdaptationPhase", () => switchPhase(2));

        EventManager.StartListening("EndGame", launchOutro);
    }

    // Use this for initialization
    void Start() {
        _health = startingHealth;

        StartCoroutine(introSpeaking());

        InfoScore = GameObject.Find("Canvas").GetComponent<infoScore>();
    }

    IEnumerator introSpeaking()
    {
        say("le soleil pique une crise !", 2, true);
        say("detruisez-le avant qu'il\nne nous carbonise !", 2, true);
        say("lorsqu'il explosera…\nje pourrais m'echapper", 2, true);
        say("mais je n'aurais qu'une\nseule place à bord", 2, true);
        say("si vous voulez survivre\nfaites-moi plaisir !", 2, true);
        say("ha ha ha !", 2, false);

        yield return new WaitForSeconds(12f);
        EventManager.TriggerEvent("IntroOver");
        StartCoroutine(startMissionSystem());
    }

    private bool endgame = false;

    void launchOutro()
    {
        StopAllCoroutines();

        endgame = true;
        GetComponent<SpriteRenderer>().sprite = happySprite;
        rotateSpeed = 0;

        //tp most valuable player
        GameObject p1 = GameObject.Find("Player2");
        GameObject p2 = GameObject.Find("Player2");
        GameObject winner;
        if (p2.GetComponent<PlayerBehavior>()._PF > p1.GetComponent<PlayerBehavior>()._PF)
        {
            winner = p2;
        }
        else
        {
            winner = p1;
        }

        float xLeftRight = transform.position.x;
        float offset = -1f;

        if (xLeftRight > 0)
        {
            offset = -offset;
        }

        winner.transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        winner.transform.rotation = Quaternion.identity;

        StartCoroutine(outroSpeaking(winner));
    }

    IEnumerator outroSpeaking(GameObject winner)
    {
        say("bien joue\nil va peter !", 2, false);
        say("maintenant il est\ntemps de partir", 2, false);
        say("viens, tu es mon\njoueur prefere", 2, false);
        say("bye bye !", 2, false);

        yield return new WaitForSeconds(12f);

        this.gameObject.SetActive(false);
        winner.gameObject.SetActive(false);

        EventManager.TriggerEvent("OutroOver");
    }

    private void switchPhase(int index)
    {
        Debug.Log("senpai change phase");
        stopCurrentMission();
        lastMission = null; //prob possible ici ?
        currentMissions = missionsAvailable[index];
    }
	
	// Update is called once per frame
	void Update () {
        if (endgame)
            return;


        moveRandom();
        if (_currentdirection == 1)
        {
            goLeft();
        }
        else if (_currentdirection == -1)
        {
            goRight();
        }

        Vector3 diff = new Vector3(0,0,0) - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z +90);
        GameObject.Find("OpposedZoneAnchor").transform.rotation = Quaternion.Euler(0f, 0f, rot_z );

        updateFace();
    }

    void updateFace()
    {
        if (_rancuneP1 || _rancuneP2)
        {
            GetComponent<SpriteRenderer>().sprite = angrySprite;
            //message de caprice ratée
        }
        else if(blink == null)
        {
            GetComponent<SpriteRenderer>().sprite = neutralSprite;
        }
    }
    void moveRandom()
    {
        if (Random.Range(0,100)==0)
        {
            _currentdirection = Random.Range(-1,2);
        }
    }
    void goLeft()
    {
        transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), rotateSpeed * Time.deltaTime);
    }
    void goRight()
    {
        transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), -rotateSpeed * Time.deltaTime);
    }

    void ApplyDamage(int amount)
    {
        Debug.Log("sempai lose health");
        _health -= amount;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Displaying speech bubbles
    void OnGUI()
    {
        GUI.skin = messages;
        if (_whatimsaying!="")
        {
            Vector2 v = Vector2.Lerp(new Vector3(0,0,0),transform.position,0.5f);
            Vector2 v2 = Camera.main.WorldToScreenPoint(v);
            GUI.Box(new Rect(v2.x-messageSize.x/2, Screen.height- v2.y-messageSize.y/2, messageSize.x, messageSize.y), _whatimsaying);
        }
    }

    void say(string whatisay, float timelasting, bool slowTime)
    {
        if(isSayingSomething != null)
        {
            //Debug.Log("is saying somehting: " + whatisay);
            messagesWaiting.Add(initSaying(timelasting, whatisay, slowTime));
            return;
        }

        GetComponents<AudioSource>()[0].Play();
        isSayingSomething = StartCoroutine(initSaying(timelasting, whatisay, slowTime));
        
    }

    IEnumerator initSaying(float time, string text, bool slowTime)
    {
        if(slowTime)
        {
            greyBackground.SetActive(true);
            Time.timeScale = slowTimeScale;
        }
            
        _whatimsaying = text;
        yield return new WaitForSeconds(time * ((slowTime) ? slowTimeScale : 1));
        _whatimsaying = "";

        if (slowTime)
        {
            Time.timeScale = 1;
            greyBackground.SetActive(false);
        }

        if(messagesWaiting.Count > 0)
        {
            IEnumerator messageWaiting = messagesWaiting[0];
            messagesWaiting.Remove(messageWaiting);
            isSayingSomething = StartCoroutine(messageWaiting);
        }
        else
        {
            isSayingSomething = null;//sinon met tout les messages dans attente
        }
    }
    // Missions d'entraide ///////////////////////////////////
    // Misssion Destroy Weakpoint ///////////////////////////////////
    void startMissionDestroyWeakpoint(float tempsmission)
    {
        _MissionDestroyWeakpoint = true;
        say(destroyWeakpointPhrase, missionTextTime, true);
        currentCoroutineMission = StartCoroutine(checkMissionDestroyWeakpoint(tempsmission));
    }
    IEnumerator checkMissionDestroyWeakpoint(float temps)
    {
        yield return new WaitForSeconds(temps);
        _MissionDestroyWeakpoint = false;
    }
    void OnP1DestroyWeakpoint()
    {
        if (_MissionDestroyWeakpoint)
        {
            if (GameObject.Find("Player1") != null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += pointsOnWeakpointDestroyed +1;//besoin impair pour pas egalite
            if (GameObject.Find("Player2")!= null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += pointsOnWeakpointDestroyed +1;
            _MissionDestroyWeakpoint = false;
            InfoScore.weakPointDestroyed_J1();
        }
    }
    void OnP2DestroyWeakpoint()
    {
        if (_MissionDestroyWeakpoint)
        {
            if (GameObject.Find("Player1") != null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += pointsOnWeakpointDestroyed;
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += pointsOnWeakpointDestroyed;
            _MissionDestroyWeakpoint = false;
            InfoScore.weakPointDestroyed_J2();
        }
    }
    // Misssion Attack sun ///////////////////////////////////
    void startMissionAttackSun(float tempsmission)
    {
        _MissionAttackSun = true;
        say(attackSunPhrase, missionTextTime, true);
        currentCoroutineMission = StartCoroutine(checkMissionAttackSun(tempsmission));
    }
    IEnumerator checkMissionAttackSun(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (_playersTouchedSun[0] > SunAttackAmount)
        {
            if (GameObject.Find("Player1") != null)
            {
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J1();
            }
        }
        else
        {
            if (GameObject.Find("Player1") != null)
            {
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J1();
            }
        }
        if (_playersTouchedSun[1] > SunAttackAmount)
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J2();
            }
        }
        else
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J2();
            }
        }
        _playersTouchedSun[0] = 0;
        _playersTouchedSun[1] = 0;
        _MissionAttackSun = false;
    }
    void OnP1AttackSun()
    {
        _playersTouchedSun[0]++;
    }
    void OnP2AttackSun()
    {
        _playersTouchedSun[1]++;
    }
    // Missions de protection ///////////////////////////////////
    // Misssion stay near me ///////////////////////////////////
    void startMissionStayNearMe(float tempsmission)
    {
        _MissionStayNearMe = true;
        say(staynearmePhrase, missionTextTime, true);
        currentCoroutineMission = StartCoroutine(checkMissionStayNearMe(tempsmission));
    }
    IEnumerator checkMissionStayNearMe(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (Time.time - _P1Entered > stayCloseToMeTime)
        {
            if (GameObject.Find("Player1")!=null)
            { 
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J1();
            }
        }
        else
        {
            if (GameObject.Find("Player1") != null)
            {
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J1();
            }
        }
        if (Time.time - _P2Entered > stayCloseToMeTime)
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J2();
            }
        }
        else
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J2();
            }
        }
        _MissionStayNearMe = false;

    }
    void OnP1EnterCloseZone()
    {
        _P1Entered = Time.time;
        // Rancune
        if (_rancuneP1)
        {
            if (GameObject.Find("Player1")!=null)
            GameObject.Find("Player1").GetComponent<PlayerBehavior>().slow(3);
            _rancuneP1 = false;
        }
    }
    void OnP2EnterCloseZone()
    {
        _P2Entered = Time.time;
        // Rancune
        if (_rancuneP2)
        {
            if (GameObject.Find("Player2") != null)
            GameObject.Find("Player2").GetComponent<PlayerBehavior>().slow(3);
            _rancuneP2 = false;
        }
    }
    void OnP1ExitCloseZone()
    {
        _P1Entered = Mathf.Infinity;
    }
    void OnP2ExitCloseZone()
    {
        _P2Entered = Mathf.Infinity;
    }
    // Missions caprice ///////////////////////////////////
    // Misssion stop fireing ///////////////////////////////////
    void startMissionStopFireing(float tempsmission)
    {
        _MissionStopFireing = true;
        say(stopFireingPhrase, missionTextTime, true);
        currentCoroutineMission = StartCoroutine(checkMissionStopFireing(tempsmission));
    }
    IEnumerator checkMissionStopFireing(float temps)
    {
        yield return new WaitForSeconds(1);
        _P1Fired = false;
        _P2Fired = false;
        yield return new WaitForSeconds(temps);
        _MissionStopFireing = false;
        if (_P1Fired)
        {
            if (GameObject.Find("Player1") != null)
            {
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J1();
            }
            startRancuneP1();
        }
        else
        {
            if (GameObject.Find("Player1") != null)
            {
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J1();
            }
            _rancuneP1 = false;
        }
        if (_P2Fired)
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J2();
            }
            startRancuneP2();
        }
        else
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J2();
            }
            _rancuneP2 = false;
        }
    }
    void OnP1shoot()
    {
        _P1Fired = true;
    }
    void OnP2shoot()
    {
        _P2Fired = true;
    }
    // Mission Get Away //////////////////////////////////////////////////
    void startMissionGetAway(float tempsmission)
    {
        _MissionGetAway = true;
        say(getAwayPhrase, missionTextTime, true);
        currentCoroutineMission = StartCoroutine(checkMissionGetAway(tempsmission));
    }
    IEnumerator checkMissionGetAway(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (Time.time - _P1EnteredOpposed > getAwayTime)
        {
            if (GameObject.Find("Player1") != null)
            {
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J1();
            }
        }
        else
        {
            if (GameObject.Find("Player1") != null)
            {
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J1();
            }
                startRancuneP1();
        }
        if (Time.time - _P2EnteredOpposed > getAwayTime)
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
                InfoScore.missionReussie_J2();
            }
        }
        else
        {
            if (GameObject.Find("Player2") != null)
            {
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
                InfoScore.missionFailure_J1();
            }
            startRancuneP2();
        }
        _MissionGetAway = false;
    }
    void OnP1EnterOpposed()
    {
        _P1EnteredOpposed = Time.time;
    }
    void OnP2EnterOpposed()
    {
        _P2EnteredOpposed = Time.time;
    }
    void OnP1ExitOpposed()
    {
        _P1EnteredOpposed = Mathf.Infinity;
    }
    void OnP2ExitOpposed()
    {
        _P2EnteredOpposed = Mathf.Infinity;
    }
    //////////////////////////////////////////////////////////////////////
    // Rancune     ///////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////
    void startRancuneP1()
    {
        StartCoroutine(rancuneP1());
    }
    IEnumerator rancuneP1()
    {
        yield return new WaitForSeconds(Random.Range(4,6));
        _rancuneP1 = true;
    }
    void startRancuneP2()
    {
        StartCoroutine(rancuneP2());
    }
    IEnumerator rancuneP2()
    {
        yield return new WaitForSeconds(Random.Range(4, 6));
        _rancuneP2 = true;
    }
    // Destroying missiles
    void OnP1DestroyProjectile()
    {
        if (GameObject.Find("Player1") != null)
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += pointsOnDestroyProjectile;
    }
    void OnP2DestroyProjectile()
    {
        if (GameObject.Find("Player2") != null)
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += pointsOnDestroyProjectile;
    }
    //////////////////////////////////////////////////////////////////////
    // When player touch Senpaiiiii
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<PlayerProjectile>() != null)
        {
            Destroy(coll.gameObject);

            if (!canBeTouched)
                return;

            GetComponents<AudioSource>()[1].Play();
            
            if (coll.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
            {
                if (GameObject.Find("Player1") != null)
                {
                    GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= pointsLostOnTouchSenpai;
                    InfoScore.friendlyFire_J1();
                }
            }
            else
            {
                if (GameObject.Find("Player2") != null)
                {
                    GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= pointsLostOnTouchSenpai;
                    InfoScore.friendlyFire_J2();
                }
            }

            Debug.Log("sempai collision touched by: " + coll.gameObject.name);
            startBlink();
        }
    }
    
    void startBlink()
    {
        canBeTouched = false;
        if (blink == null)
            blink = StartCoroutine(blinkSmooth(Time.timeScale, 0.4f, Color.white));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("sempai trigger touched by: " + other.gameObject.name);

        if (!canBeTouched || other.GetComponent<PlayerBehavior>() != null)//peut pas etre touché par le joueur
            return;
        
        startBlink();
    }

    IEnumerator blinkSmooth(float timeScale, float duration, Color blinkColor)
    {
        Sprite lastSprite = GetComponent<SpriteRenderer>().sprite;
        GetComponent<SpriteRenderer>().sprite = touchedSprite;
        blinkColor.a = 0f;
        var material = GetComponent<SpriteRenderer>().material;
        float alpha = 0f;
        float startTime = Time.time;
        duration = duration / 2;
        while (alpha < 1f)
        {
            alpha = Mathf.Lerp(0f, 1f, (Time.time - startTime) / duration);
            blinkColor.a = alpha;
            material.SetColor("_BlinkColor", blinkColor);
            yield return null;
        }
        startTime = Time.time;
        while (alpha > 0f)
        {
            alpha = Mathf.Lerp(1f, 0f, (Time.time - startTime) / duration);
            blinkColor.a = alpha;
            material.SetColor("_BlinkColor", blinkColor);
            yield return null;
        }
        
        canBeTouched = true;

        if(touchedSprite == GetComponent<SpriteRenderer>().sprite)
            GetComponent<SpriteRenderer>().sprite = lastSprite;

        blink = null;
    }


    //////////////////////////////////////////////////////////////////////
    void moveFront(UnityAction currentMission)//move item in list to front (index 0)
    {
        int currentIndex = currentMissions.IndexOf(currentMission);
        if (currentIndex < 0)
            return;
        UnityAction item = currentMissions[currentMissions.IndexOf(currentMission)];
        currentMissions.RemoveAt(currentIndex);
        currentMissions.Insert(0, item);
    }

    void stopCurrentMission()
    {
        if(currentCoroutineMission != null)
        {
            StopCoroutine(currentCoroutineMission);
            _MissionDestroyWeakpoint = _MissionAttackSun = _MissionGetAway = _MissionStopFireing = _MissionStayNearMe = false;
        }
    }

    void startRandomMission()
    {
        if(currentMissions == null)
        {
            return;
        }

        if (_MissionDestroyWeakpoint || _MissionAttackSun || _MissionGetAway || _MissionStopFireing || _MissionStayNearMe)
        {
            return;
        }

        int mission;
        if(lastMission != null)
        {
            moveFront(lastMission);
            mission = Random.Range(1, currentMissions.Count);
        }
        else
        {
            mission = Random.Range(0, currentMissions.Count);
        }
        lastMission = currentMissions[mission];
        lastMission.Invoke();

        /*
        if (!_MissionDestroyWeakpoint && !_MissionAttackSun && !_MissionGetAway && !_MissionStopFireing && ! _MissionStayNearMe)
        {
            int mission = Random.Range(1,6);
            if (mission == 1)
            {
                startMissionDestroyWeakpoint(missionTime);
            }
            else if (mission == 2)
            {
                startMissionAttackSun(missionTime);
            }
            else if (mission == 3)
            {
                startMissionStayNearMe(missionTime);
            }
            else if (mission == 4)
            {
                startMissionGetAway(missionTime);
            }
            else if (mission == 5)
            {
                startMissionStopFireing(missionTime);
            }
        }
        */
    }
    IEnumerator startMissionSystem()
    {
        Debug.Log("mission system launched");
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenMission, maxTimeBetweenMission));
            startRandomMission();
        }
    }
}
