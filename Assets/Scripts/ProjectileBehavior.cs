using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour {
    public string launchedby;
    private Renderer red;


    // Use this for initialization
    void Start () {
        red = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!red.isVisible)
        {
            //Debug.Log("projectile hors ecran");
            //Destroy(gameObject);
            this.gameObject.SetActive(false);
        }
            
	}
    void OnTriggerEnter2D(Collider2D coll)
    {
        //Debug.Log("projectile touched: " + coll.gameObject.name);
        /*if (coll.collider.tag == "Player")
        {
            coll.gameObject.SendMessage("ApplyDamage", 1);
            Explode();
        }
        if (coll.collider.tag == "WeakPoint")
        {
            coll.gameObject.SendMessage("ApplyDamage", 1);
            Explode();
        }
        if (coll.collider.name == "Shield")
        {
            //Destroy(gameObject);
            this.gameObject.SetActive(false);
        }
        */
    }
    void Explode()
    {
        //var exp = GetComponent<ParticleSystem>();
        //exp.Play();
        //Destroy(gameObject);//, exp.duration);
        this.gameObject.SetActive(false);
    }
}
