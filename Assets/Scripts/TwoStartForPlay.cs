using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TwoStartForPlay : MonoBehaviour {

    public GameObject startText_Joueur1;
    public GameObject startText_Joueur2;

    private bool startJ1 = false;
    private bool startJ2 = false;

    void Update () {
        if (Input.GetButton("Start_J1"))
        { 
            //print("StartJ1 pressed !");
            startJ1 = true;
            startText_Joueur1.GetComponent<Text>().color = new Color(255, 0, 0);
        }
        if (Input.GetButton("Start_J2"))
        {
            //print("StartJ2 pressed !");
            startJ2 = true;
            startText_Joueur2.GetComponent<Text>().color = new Color(255, 0, 0);
        }

        if(startJ1 && startJ2)
        {
            Application.LoadLevel("Main");
        }
    }
}

