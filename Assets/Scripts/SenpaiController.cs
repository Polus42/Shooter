using UnityEngine;
using System.Collections;

public class SenpaiController : MonoBehaviour {
    public int rotateSpeed;
    public int startingHealth = 10;
    private int _currentdirection;
    private int _health;
    private string _whatimsaying = "";
    // Use this for initialization
    void Start () {
	    _health = startingHealth;
        say("Bonjour",50);
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
            GUI.Box(new Rect(v.x, v.y, 50, 20), _whatimsaying);
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
}
