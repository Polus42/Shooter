using UnityEngine;
using System.Collections;

public class ProjectileBehavior : MonoBehaviour {
    public string launchedby;
    public int _health = 1;
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
        transform.Rotate(new Vector3(0,0,1));
            
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerProjectile>() != null)
        {
            Destroy(other.gameObject);
            _health--;
            if (_health <= 0)
            {
                gameObject.SetActive(false);
                if (other.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
                {
                    GameObject.Find("Senpai").SendMessage("OnP1DestroyProjectile");
                }
                else
                {
                    GameObject.Find("Senpai").SendMessage("OnP2DestroyProjectile");
                }
            }
        }
        else if(other.gameObject.GetComponent<PlayerBehavior>() != null)
        {
            other.gameObject.SendMessage("ApplyDamage",1);
            Destroy(gameObject);
        }
    }
    void Explode()
    {
        //var exp = GetComponent<ParticleSystem>();
        //exp.Play();
        //Destroy(gameObject);//, exp.duration);
        this.gameObject.SetActive(false);
    }

}
