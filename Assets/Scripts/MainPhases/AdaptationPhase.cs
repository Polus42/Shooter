using UnityEngine;
using System.Collections;
using System;

public class AdaptationPhase : IMainPhase
{
    private readonly GameController gc;

    private OptionsHolder.GeneralOP generalOP;

    private float totalDuration = 0;//seconds

    public AdaptationPhase(GameController gc)
    {
        this.gc = gc;
    }

    public void InitPhase()
    {
        OptionsManager.Instance.getGeneralOptions(out generalOP);
        totalDuration = 0;
        //gravite++
        //asteroid++
        //soleil taille ++
    }

    public void UpdatePhase()
    {
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
}
