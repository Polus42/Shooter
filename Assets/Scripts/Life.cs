using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour {
    private int _rotateSpeed;
	// Use this for initialization
	void Start () {
        _rotateSpeed = Random.Range(50,150);
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), _rotateSpeed* Time.deltaTime);
    }
}
