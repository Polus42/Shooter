using UnityEngine;
using System.Collections;
using System;

public class AdaptationPhase : IMainPhase
{
    private readonly GameController gc;
    private OptionsHolder.GeneralOP generalOP;
    private Camera cam;
    private float heightCam;
    private float widthCam;

    private float totalDuration = 0;//seconds
    private float counterAst = 0;

    public AdaptationPhase(GameController gc)
    {
        this.gc = gc;
    }

    public void InitPhase()
    {
        OptionsManager.Instance.getGeneralOptions(out generalOP);
        totalDuration = 0;
        counterAst = 0;
        cam = Camera.main;
        heightCam = 2f * cam.orthographicSize;
        widthCam = heightCam * cam.aspect;

        Debug.Log("cam: " + heightCam + " " + widthCam);

        //gravite++
        //asteroid++
        //soleil taille ++
        
    }

    public void UpdatePhase()
    {
        counterAst += Time.deltaTime;
        if(counterAst > generalOP.asteroidFrequency)
        {
            UnityEngine.GameObject go = (UnityEngine.GameObject)GameObject.Instantiate(gc.asteroidPrefab, getVertexPosition(), Quaternion.identity);
            //go.GetComponent<Rigidbody2D>().AddForce((go.transform.position - gc.transform.position) * -30);
            counterAst = 0;
        }

        //Debug.Log("adaptation phase on");
        totalDuration += Time.deltaTime;
        if(totalDuration > generalOP.adaptationPhaseDuration)
        {
            //EndPhase();
            Debug.Log("adaptation phase end");
            EventManager.TriggerEvent("AdaptationEnded");
        }
    }

    public void EndPhase()
    {
        gc.UpdateDifficulty();
        //gc.goToSurvivalPhase();
    }
    
    Vector2 getVertexPosition()//from main camera
    {
        int x = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        int y = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        return new Vector2(x * widthCam / 2, y * heightCam / 2);
    }
}
