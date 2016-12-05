using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class changeScene_generq : MonoBehaviour {

    void Start()
    {
        StartCoroutine(WaitAndPrint());
    }

    IEnumerator WaitAndPrint()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Menu");
    }


}
