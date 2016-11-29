using UnityEngine;
using System.Collections;

public class CounterPhase : IMainPhase
{
    private readonly GameController gc;

    public CounterPhase(GameController gc)
    {
        this.gc = gc;
    }

    public void InitPhase()
    {

    }

    public void UpdatePhase()
    {
        //Debug.Log("counter phase on");
    }

    public void EndPhase()
    {

    }
}
