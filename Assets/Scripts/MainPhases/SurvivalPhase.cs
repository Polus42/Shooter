using UnityEngine;
using System.Collections;
using System;

public class SurvivalPhase : IMainPhase
{
    private readonly GameController gc;

    public SurvivalPhase(GameController gc)
    {
        this.gc = gc;
    }

    public void InitPhase()
    {

    }

    public void UpdatePhase()
    {
        //Debug.Log("Survival On");
    }

    public void EndPhase()
    {

    }
}