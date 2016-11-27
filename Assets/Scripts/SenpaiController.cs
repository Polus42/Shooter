using UnityEngine;
using System.Collections;

public class SenpaiController : MonoBehaviour {
    public int rotateSpeed;
    public int startingHealth = 10;
    // Messages list
    public GUISkin messages;
    private int _currentdirection;
    private int _health;
    private string _whatimsaying = "";
    // to check if a mission is currently launched
    private bool _MissionDestroyWeakpoint = false;
    private bool _MissionAttackSun = false;
    // Use this for initialization
    void Start () {
	    _health = startingHealth;
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
        else
        {
            //don't move
        }
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
            Vector3 v = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, -transform.position.y, transform.position.z));
            // Avoid clipping
            if (v.y < 10)
            {
                v.y = 10;
            }
            else if (v.y + 20 > Screen.height)
            {
                v.y = Screen.height - 20;
            }
            GUI.Box(new Rect(v.x, v.y, 100, 80), _whatimsaying);
        }
    }
    void say(string whatisay,float timelasting)
    {
        StartCoroutine(initSaying(timelasting,whatisay));
    }
    IEnumerator initSaying(float time,string text)
    {
        for (int i =0;i<text.Length;i++)
        {
            _whatimsaying +=text[i];
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(time);
        _whatimsaying = "";
    }
    // Missions d'entraide ///////////////////////////////////
    void startMissionDestroyWeakpoint(float tempsmission)
    {
        _MissionDestroyWeakpoint = true;
        say("Détruisez le point faible !",5);
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
            _MissionDestroyWeakpoint = false;
        }
    }
    void OnP2DestroyWeakpoint()
    {
        if (_MissionDestroyWeakpoint)
        {
            GameObject.Find("Player2").GetComponent<PlayerBehavior>()._PF += 100;
            _MissionDestroyWeakpoint = false;
        }
    }
    void startMissionAttackSun(float tempsmission)
    {
        _MissionAttackSun = true;
        say("Attaquez le !", 5);
        StartCoroutine(checkMissionDestroyWeakpoint(tempsmission));
    }
    IEnumerator checkMissionAttackSun(float temps)
    {
        yield return new WaitForSeconds(temps);
        _MissionAttackSun = false;
    }
    // Missions de protection ///////////////////////////////////
    // Missions caprice ///////////////////////////////////
    void startRandomMission()
    {
        if (!_MissionDestroyWeakpoint && !_MissionAttackSun)
        {
            int mission = Random.Range(1,3);
            if (mission == 1)
            {
                startMissionDestroyWeakpoint(10);
            }
            else if (mission == 2)
            {
                startMissionAttackSun(10);
            }
        }
    }
}
