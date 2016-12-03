using UnityEngine;
using System.Collections;

public class infoScoreMove : MonoBehaviour {

    public float speed = 10;
    private bool endDelay = false;

    void Start()
    {
       StartCoroutine (WaitAndPrint());
    }

	void Update () {
        if (endDelay == true )
        { 
        transform.Translate(Vector3.up * speed * Time.deltaTime);
	    }
    }

    IEnumerator WaitAndPrint()
    {
        yield return new WaitForSeconds(1);
        endDelay = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print("collision");
        if (other.gameObject.tag == "UI")
        {
            print("collisionUI");
            Destroy(gameObject);
        }

    }
}
