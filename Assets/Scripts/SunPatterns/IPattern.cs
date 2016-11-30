using UnityEngine;
using System.Collections;

public interface IPattern
{
    void setOptions(OptionsHolder.IOptionPattern options);
    void UpdatePattern();
    void EndPattern();
}
