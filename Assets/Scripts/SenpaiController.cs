using UnityEngine;
using System.Collections;

public class SenpaiController : MonoBehaviour {
    public int rotateSpeed;
    public int startingHealth = 10;
    private int _currentdirection;
    private int _health;
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
}
