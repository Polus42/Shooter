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
	    if (_health<=0)
        {
            EventManager.TriggerEvent("WeakPointDestroyed");
            //Destroy(gameObject);
            this.gameObject.SetActive(false);
        }
	}

    void ApplyDamage(int howmany)
    {
        Debug.Log("weak point touched");
        _health -= howmany;
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
}
