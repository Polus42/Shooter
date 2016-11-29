using UnityEngine;
using System.Collections;

public interface IMainPhase
{
    void InitPhase();
    void UpdatePhase();
    void EndPhase();
}
