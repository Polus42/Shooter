using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class button : MonoBehaviour {


    public GameObject shadowPlay;
    public GameObject shadowQuit;
    bool play = true;

	// Use this for initialization
	void Start () {
        //gameObject.renderer.enabled = false;
        
    }
	
	// Update is called once per frame
	void Update () {
            if (Input.GetButton("P1_fire") || Input.GetButton("P2_fire"))
            {
            
                if (play)
                {
                    SceneManager.LoadScene("tutorial");
                }
                else
                {
                Debug.Log(play);
                Application.Quit();
                }
            }

	    if(Input.GetAxis("Horizontal") < 0)
        {
            Debug.Log("yo");
            shadowQuit.GetComponent<Renderer>().enabled = false;
            shadowPlay.GetComponent<Renderer>().enabled = true;
            play = true;
        }
        else if(Input.GetAxis("Horizontal") > 0)
        {
            shadowPlay.GetComponent<Renderer>().enabled = false;
            shadowQuit.GetComponent<Renderer>().enabled = true;
            play = false;
        }
	}
}
