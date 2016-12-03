using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsManager : Singleton<OptionsManager>
{
    private OptionsHolder currentOptions = new OptionsHolder();//never change
    private List<OptionsHolder> allOptions;

    protected OptionsManager() { }

    public void init(int startDifficulty, string startPhase, TextAsset options) {
        Debug.Log("init start");
        allOptions = new List<OptionsHolder>();
        JSONObject json = new JSONObject(options.text);
        foreach(JSONObject difficulties in json.list)
        {
            Debug.Log("diff " + difficulties);
            allOptions.Add(new OptionsHolder(difficulties));
        }
        Debug.Log("alloptions count : " + allOptions.Count);
        //currentOptions = allOptions[startDifficulty];

        //Eviter deux fois charger 1ere phase, startDifficulty et startPhase inutiles ?
        currentOptions.switchDifficulty(allOptions[startDifficulty]);
        //currentOptions.switchPhase(startPhase);


        Debug.Log("init end");
    }

    public void changeDifficulty(int difficulty)
    {
        Debug.Log("next diff: " + difficulty + " - count diff available: " + allOptions.Count);
        if (difficulty > allOptions.Count - 1)
            Debug.Log("difficulty not available");
        else      
            currentOptions.switchDifficulty(allOptions[difficulty]);
    }
    
    public void changePhase(string name)
    {
        currentOptions.switchPhase(name);
    }

    public void getCurrentOptions(out OptionsHolder output)
    {
        output = currentOptions;
    }

    public void getGeneralOptions(out OptionsHolder.GeneralOP output)
    {
        output = currentOptions._generalOP;
    }

    public void getAsteroidOptions(out OptionsHolder.AsteroidOP output)
    {
        output = currentOptions._asteroidOP;
    }

    public void getSunOptions(out OptionsHolder.SunOP output)
    {
        output = currentOptions._sunOP;
    }

    public void getPatternsOptions(out OptionsHolder.PatternsOP output)
    {
        output = currentOptions._patternsOP;
    }

    public void getCurrentPatterns(out List<OptionsHolder.IOptionPattern> output)
    {
        output = currentOptions._currentPatterns;
    }

    public void getProbs(out List<float> output)
    {
        output = currentOptions._probs;
    }
}
