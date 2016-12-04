using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class button : MonoBehaviour {


    public GameObject shadowPlay;
    public GameObject shadowQuit;
    bool play = true;
    private AudioSource[] _sources;

	// Use this for initialization
	void Start () {
        //gameObject.renderer.enabled = false;
        _sources = GetComponents<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
            if (Input.GetButton("P1_fire") || Input.GetButton("P2_fire"))
            {
            StartCoroutine(Delay());
            
            }

        if (Input.GetAxis("Horizontal") < 0)
        {
            Debug.Log("yo");
            shadowQuit.GetComponent<Renderer>().enabled = false;
            shadowPlay.GetComponent<Renderer>().enabled = true;
            play = true;
            if (_sources[0].isPlaying == false)
                _sources[0].Play();

        }
        else if(Input.GetAxis("Horizontal") > 0)
        {
            shadowPlay.GetComponent<Renderer>().enabled = false;
            shadowQuit.GetComponent<Renderer>().enabled = true;
            play = false;
            if(_sources[0].isPlaying == false)
                _sources[0].Play();
        }
	}

    IEnumerator Delay()
    {
        if (_sources[1].isPlaying == false)
            _sources[1].Play();
        yield return new WaitForSeconds(1);
        
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
}
