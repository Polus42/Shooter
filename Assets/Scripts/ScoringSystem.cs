using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoringSystem : MonoBehaviour {
    private Text _PF_P1_text;
    private Text _PF_P2_text;
    private PlayerBehavior _p1;
    private PlayerBehavior _p2;
    // Use this for initialization
    void Start () {
        _PF_P1_text = GameObject.Find("PF P1").GetComponent<Text>();
        _PF_P2_text = GameObject.Find("PF P2").GetComponent<Text>();
        _p1 = GameObject.Find("Player1").GetComponent<PlayerBehavior>();
        _p2 = GameObject.Find("Player2").GetComponent<PlayerBehavior>();
    }
	
	// Update is called once per frame
	void Update () {
        _PF_P1_text.text = _p1._PF.ToString();
        _PF_P2_text.text = _p2._PF.ToString();
	}
}
