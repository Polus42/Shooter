using UnityEngine;
using System.Collections;

public class SunBehavior : MonoBehaviour {
    public GameObject[] projectile;
    public float force;
	// Use this for initialization
	void Start () {

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
}
