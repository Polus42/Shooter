using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class infoScore : MonoBehaviour {

    // test pour instancier les textes
    public Transform pivot_J1;
    public Transform pivot_J2;
    public GameObject prefab_J1;
    public GameObject prefab_J2;
    private GameObject newTextJ1;
    private GameObject newTextJ2;

    // Positif
    private string weakPointDestroy = "point faible +200";
    private string sunDestroy = "coup final +201";
    private string protectMissi = "protection +20";
    private string missionReuss = "mission +200";
    // Négatif // changer la couleur ou le style d'écriture
    private string friendlyFi = "tir allié -100";
    private string missionFailu = "mission... -100";

    // Use this for initialization
    void Start () {
        //weakPointDestroyed_J1();
        //sunDestroyed_J2();
    }

    // quand créa d'une nouvelle instance, détruit celles qui la précède pour le joueur correspondant

    //
    public void weakPointDestroyed_J1()
    {
        newTextJ1 = Instantiate(prefab_J1, pivot_J1.position, Quaternion.identity) as GameObject;
        newTextJ1.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ1.GetComponent<Text>().text = weakPointDestroy;
    }

    public void weakPointDestroyed_J2()
    {
        newTextJ2 = Instantiate(prefab_J2, pivot_J2.position, Quaternion.identity) as GameObject;
        newTextJ2.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ2.GetComponent<Text>().text = weakPointDestroy;
    }


    //
    public void sunDestroyed_J1()
    {
        newTextJ1 = Instantiate(prefab_J1, pivot_J1.position, Quaternion.identity) as GameObject;
        newTextJ1.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ1.GetComponent<Text>().text = sunDestroy;
    }

    public void sunDestroyed_J2()
    {
        newTextJ2 = Instantiate(prefab_J2, pivot_J2.position, Quaternion.identity) as GameObject;
        newTextJ2.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ2.GetComponent<Text>().text = sunDestroy;
    }


    //
    public void protectMission_J1()
    {
        newTextJ1 = Instantiate(prefab_J1, pivot_J1.position, Quaternion.identity) as GameObject;
        newTextJ1.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ1.GetComponent<Text>().text = protectMissi;
    }

    public void protectMission_J2()
    {
        newTextJ2 = Instantiate(prefab_J2, pivot_J2.position, Quaternion.identity) as GameObject;
        newTextJ2.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ2.GetComponent<Text>().text = protectMissi;
    }


    //
    public void missionReussie_J1()
    {
        newTextJ1 = Instantiate(prefab_J1, pivot_J1.position, Quaternion.identity) as GameObject;
        newTextJ1.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ1.GetComponent<Text>().text = missionReuss;
    }

    public void missionReussie_J2()
    {
        newTextJ2 = Instantiate(prefab_J2, pivot_J2.position, Quaternion.identity) as GameObject;
        newTextJ2.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ2.GetComponent<Text>().text = missionReuss;
    }


    //
    public void friendlyFire_J1()
    {
        newTextJ1 = Instantiate(prefab_J1, pivot_J1.position, Quaternion.identity) as GameObject;
        newTextJ1.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ1.GetComponent<Text>().text = friendlyFi;
    }

    public void friendlyFire_J2()
    {
        newTextJ2 = Instantiate(prefab_J2, pivot_J2.position, Quaternion.identity) as GameObject;
        newTextJ2.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ2.GetComponent<Text>().text = friendlyFi;
    }


    //
    public void missionFailure_J1()
    {
        newTextJ1 = Instantiate(prefab_J1, pivot_J1.position, Quaternion.identity) as GameObject;
        newTextJ1.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ1.GetComponent<Text>().text = missionFailu;
    }

    public void missionFailure_J2()
    {
        newTextJ2 = Instantiate(prefab_J2, pivot_J2.position, Quaternion.identity) as GameObject;
        newTextJ2.transform.SetParent(transform.Find("Canvas_infoScore"));
        newTextJ2.GetComponent<Text>().text = missionFailu;
    }
}
