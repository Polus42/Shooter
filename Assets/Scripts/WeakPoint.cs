using UnityEngine;
using System.Collections;

public class WeakPoint : MonoBehaviour {
    private OptionsHolder.SunOP sunOP;

    private int _health;
    //public int startinghealth;
	// Use this for initialization

    void Awake()
    {
        //Debug.Log("Awake weak point");
        //EventManager.StartListening("OnCounterPhase", Awakening);
        
    }
    
	void Start () {
        Awakening();
        //startinghealth = sunOP.weakPointHealth;
        //_health = startinghealth;
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void Awakening()
    {
        //this.gameObject.SetActive(true);
        Debug.Log("weak point awakening");
        OptionsManager.Instance.getSunOptions(out sunOP);
        _health = sunOP.weakPointHealth;

        //Test only
        EventManager.TriggerEvent("WeakPointDestroyed");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerProjectile>() != null)
        {
            _health--;
            if (_health<=0)
            {
                this.gameObject.SetActive(false);
                if (other.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
                {
                    GameObject.Find("Senpai").SendMessage("OnP1DestroyWeakpoint");
                }
                else
                {
                    GameObject.Find("Senpai").SendMessage("OnP2DestroyWeakpoint");
                }
                EventManager.TriggerEvent("WeakPointDestroyed");
            }
        }
    }

}
