using UnityEngine;
using System.Collections;

public class SenpaiController : MonoBehaviour {
    public int rotateSpeed;
    public int startingHealth = 10;
    public Vector2 messageSize;
    // Messages list
    public GUISkin messages;
    private int _currentdirection;
    private int _health;
    private string _whatimsaying = "";
    // Paramètres missions
    public float missionTime = 10;
    public float missionTextTime = 5;
    public int PF_lost = 100;
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
    // Use this for initialization
    void Start () {
	    _health = startingHealth;
        //startMissionDestroyWeakpoint(10);
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

        startRandomMission();
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
        StartCoroutine(initSaying(timelasting,whatisay));
    }
    IEnumerator initSaying(float time,string text)
    {
        _whatimsaying =text;
        yield return new WaitForSeconds(time);
        _whatimsaying = "";
    }
    // Missions d'entraide ///////////////////////////////////
    // Misssion Destroy Weakpoint ///////////////////////////////////
    void startMissionDestroyWeakpoint(float tempsmission)
    {
        _MissionDestroyWeakpoint = true;
        say(destroyWeakpointPhrase, missionTextTime);
        StartCoroutine(checkMissionDestroyWeakpoint(tempsmission));
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
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
            _MissionDestroyWeakpoint = false;
        }
    }
    void OnP2DestroyWeakpoint()
    {
        if (_MissionDestroyWeakpoint)
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
            _MissionDestroyWeakpoint = false;
        }
    }
    // Misssion Attack sun ///////////////////////////////////
    void startMissionAttackSun(float tempsmission)
    {
        _MissionAttackSun = true;
        say(attackSunPhrase, missionTextTime);
        StartCoroutine(checkMissionAttackSun(tempsmission));
    }
    IEnumerator checkMissionAttackSun(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (_playersTouchedSun[0] > SunAttackAmount)
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
        }
        if (_playersTouchedSun[1] > SunAttackAmount)
        {
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
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
        _P1Entered = Mathf.Infinity;
        _P2Entered = Mathf.Infinity;
        say(staynearmePhrase, missionTextTime);
        StartCoroutine(checkMissionStayNearMe(tempsmission));
    }
    IEnumerator checkMissionStayNearMe(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (Time.time - _P1Entered > stayCloseToMeTime)
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
        }
        if (Time.time - _P2Entered > stayCloseToMeTime)
        {
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
        }
        _MissionStayNearMe = false;

    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.name == "Player1")
        {
            _P1Entered = Time.time;
            // Rancune
            if (_rancuneP1)
            {
                coll.gameObject.GetComponent<PlayerBehavior>().slow(3);
                _rancuneP1 = false;
            }
        }
        else if (coll.gameObject.name == "Player2")
        {
            _P2Entered = Time.time;
            // Rancune
            if (_rancuneP2)
            {
                coll.gameObject.GetComponent<PlayerBehavior>().slow(3);
                _rancuneP2 = false;
            }
        }
    }
    // Missions caprice ///////////////////////////////////
    // Misssion stop fireing ///////////////////////////////////
    void startMissionStopFireing(float tempsmission)
    {
        _MissionStopFireing = true;
        say(stopFireingPhrase, missionTextTime);
        StartCoroutine(checkMissionStopFireing(tempsmission));
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
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
            startRancuneP1();
        }
        else
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
            _rancuneP1 = false;
        }
        if (_P2Fired)
        {
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF -= PF_lost;
            startRancuneP2();
        }
        else
        {
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
        _P1EnteredOpposed = Mathf.Infinity;
        _P2EnteredOpposed = Mathf.Infinity;
        say(getAwayPhrase, missionTextTime);
        StartCoroutine(checkMissionGetAway(tempsmission));
    }
    IEnumerator checkMissionGetAway(float temps)
    {
        yield return new WaitForSeconds(temps);
        if (Time.time - _P1EnteredOpposed > getAwayTime)
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
            GameObject.Find("Player1").GetComponent<PlayerBehavior>()._PF -= PF_lost;
            startRancuneP1();
        }
        if (Time.time - _P2EnteredOpposed > getAwayTime)
        {
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
        }
        else
        {
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
    //////////////////////////////////////////////////////////////////////
    void startRandomMission()
    {
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
    }
}
