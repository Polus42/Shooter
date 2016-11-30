using UnityEngine;
using System.Collections;

public class SunBehavior : MonoBehaviour {
    public GameObject[] projectile;
    public float force;
    private int _health;
    public int startinghealth = 100;
    // Use this for initialization
    void Start () {
        _health = startinghealth;
    }
	
	// Update is called once per frame
	void Update () {
	    if((int)Random.Range(0,10) == 0)
        {
            //emitProjectile();
        }
	}
    void emitProjectile()
    {
        GameObject go = (GameObject)Object.Instantiate(projectile[0],transform.position,Quaternion.identity);
        go.GetComponent<ProjectileBehavior>().launchedby = "sun";
        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-10,10)*force, Random.Range(-10, 10)*force));
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.GetComponent<PlayerProjectile>() != null)
        {
            _health--;
            if (_health <= 0)
            {
                if (coll.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
                {
                    GameObject.FindGameObjectWithTag("Senpai").SendMessage("OnP1DestroySun");
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Senpai").SendMessage("OnP2DestroySun");
                }
                Destroy(gameObject);
            }
            else
            {
                if (coll.gameObject.GetComponent<PlayerProjectile>().launchedby == "P1")
                {
                    GameObject.FindGameObjectWithTag("Senpai").SendMessage("OnP1AttackSun");
                }
                else
                {
                    GameObject.FindGameObjectWithTag("Senpai").SendMessage("OnP2AttackSun");
                }
            }
        }
        Destroy(coll.gameObject);
    }
}
