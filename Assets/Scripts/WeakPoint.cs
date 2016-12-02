using UnityEngine;
using System.Collections;

public class WeakPoint : MonoBehaviour {
    private OptionsHolder.SunOP sunOP;

    private int _health;

    float blinkDuration;//a mettre dans json
    float scalingDuration;


    void Awake()
    {
        //Debug.Log("Awake weak point");
        //EventManager.StartListening("OnCounterPhase", Awakening);
        
    }
    
	void Start () {
        //Awakening();
        //startinghealth = sunOP.weakPointHealth;
        //_health = startinghealth;
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnEnable()
    {
        Awakening();
    }

    private void Awakening()
    {
        //this.gameObject.SetActive(true);
        Debug.Log("weak point awakening");
        OptionsManager.Instance.getSunOptions(out sunOP);
        _health = sunOP.weakPointHealth;
        canBeTouched = false;
        StartCoroutine(scaling(0f, 1f, 2f, false));

        //var material = GetComponent<SpriteRenderer>().material;//mettre variable
        //material.SetColor("_BlinkColor", new Color(0,0,0,0));
        //Test only
        //this.gameObject.SetActive(false);
        //EventManager.TriggerEvent("WeakPointDestroyed");
    }

    bool canBeTouched;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canBeTouched && other.gameObject.GetComponent<PlayerProjectile>() != null)
        {
            Destroy(other.gameObject);
            canBeTouched = false;
            _health--;
            Debug.Log("weak point touched");
            StartCoroutine(blinkSmooth(Time.timeScale, 0.3f, Color.yellow));//frame blink + invulnerability
            if (_health<=0)
            {
                if (other.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
                {
                    GameObject.Find("Senpai").SendMessage("OnP1DestroyWeakpoint");
                }
                else
                {
                    GameObject.Find("Senpai").SendMessage("OnP2DestroyWeakpoint");
                }
                StartCoroutine(scaling(1f, 0f, 2f, true));
            }
        }
    }

    IEnumerator scaling(float scaleIn, float scaleOut, float duration, bool die)
    {
        var elapsedTime = 0f;
        float scale;
        while (elapsedTime <= duration)
        {
            scale = Mathf.Lerp(scaleIn, scaleOut, elapsedTime / duration);
            transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(die)
        {
            this.gameObject.SetActive(false);
            EventManager.TriggerEvent("WeakPointDestroyed");
        }
        else
        {
            canBeTouched = true;
        }
    }

    IEnumerator blinkSmooth(float timeScale, float duration, Color blinkColor)
    {
        blinkColor.a = 0f;
        var material = GetComponent<SpriteRenderer>().material;
        //var elapsedTime = 0f;
        //material.SetColor("_BlinkColor", blinkColor);
        /*while (elapsedTime <= (float) duration/2)
        {
            blinkColor.a = Mathf.PingPong(elapsedTime * timeScale, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }*/
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


        // revert to our standard sprite color
        //blinkColor.a = 0f;
        //material.SetColor("_BlinkColor", blinkColor);
        canBeTouched = true;
    }

}
