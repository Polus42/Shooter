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


    public GameObject greyBackground;

    List<List<UnityAction>> missionsAvailable;
    List<UnityAction> currentMissions;
    UnityAction lastMission = null;

    private Coroutine currentCoroutineMission;

    void Awake()
    {
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
    }

    // Use this for initialization
    void Start() {
        _health = startingHealth;
        StartCoroutine(startMissionSystem());
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
        else
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
    void say(string whatisay,float timelasting)
    {
        GetComponents<AudioSource>()[0].Play();
        StartCoroutine(initSaying(timelasting,whatisay));
        greyBackground.SetActive(true);
    }
    IEnumerator initSaying(float time,string text)
    {
        Time.timeScale = 0.1f;
        _whatimsaying =text;
        yield return new WaitForSeconds(time*0.1f);
        _whatimsaying = "";
        Time.timeScale = 1;
        greyBackground.SetActive(false);
    }
    // Missions d'entraide ///////////////////////////////////
    // Misssion Destroy Weakpoint ///////////////////////////////////
    void startMissionDestroyWeakpoint(float tempsmission)
    {
        _MissionDestroyWeakpoint = true;
        say(destroyWeakpointPhrase, missionTextTime);
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
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += pointsOnWeakpointDestroyed;
            if (GameObject.Find("Player2")!= null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += pointsOnWeakpointDestroyed;
            _MissionDestroyWeakpoint = false;
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
        }
    }
    // Misssion Attack sun ///////////////////////////////////
    void startMissionAttackSun(float tempsmission)
    {
        _MissionAttackSun = true;
        say(attackSunPhrase, missionTextTime);
        currentCoroutineMission = StartCoroutine(checkMissionAttackSun(tempsmission));
    }
    IEnumerator checkMissionAttackSun(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (_playersTouchedSun[0] > SunAttackAmount)
        {
            if (GameObject.Find("Player1") != null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            if (GameObject.Find("Player1") != null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
        }
        if (_playersTouchedSun[1] > SunAttackAmount)
        {
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
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
        say(staynearmePhrase, missionTextTime);
        currentCoroutineMission = StartCoroutine(checkMissionStayNearMe(tempsmission));
    }
    IEnumerator checkMissionStayNearMe(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (Time.time - _P1Entered > stayCloseToMeTime)
        {
            if (GameObject.Find("Player1")!=null)
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            if (GameObject.Find("Player1") != null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
        }
        if (Time.time - _P2Entered > stayCloseToMeTime)
        {
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            if (GameObject.Find("Player2")!= null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
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
        say(stopFireingPhrase, missionTextTime);
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
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
            startRancuneP1();
        }
        else
        {
            if (GameObject.Find("Player1") != null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
            _rancuneP1 = false;
        }
        if (_P2Fired)
        {
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
            startRancuneP2();
        }
        else
        {
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
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
        say(getAwayPhrase, missionTextTime);
        currentCoroutineMission = StartCoroutine(checkMissionGetAway(tempsmission));
    }
    IEnumerator checkMissionGetAway(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (Time.time - _P1EnteredOpposed > getAwayTime)
        {
            if (GameObject.Find("Player1")!= null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            if (GameObject.Find("Player1")!= null)
                GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
            startRancuneP1();
        }
        if (Time.time - _P2EnteredOpposed > getAwayTime)
        {
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            if (GameObject.Find("Player2") != null)
                GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
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
            GetComponents<AudioSource>()[1].Play();
            Destroy(coll.gameObject);
            if (coll.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
            {
                if (GameObject.Find("Player1") != null)
                    GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= pointsLostOnTouchSenpai;
            }
            else
            {
                if (GameObject.Find("Player2") != null)
                    GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= pointsLostOnTouchSenpai;
            }
        }
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
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenMission, maxTimeBetweenMission));
            startRandomMission();
        }
    }
}
