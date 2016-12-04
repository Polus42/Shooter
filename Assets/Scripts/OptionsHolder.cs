using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Alias
using Patterns = System.Collections.Generic.List<OptionsHolder.IOptionPattern>;
using PatternsPhase = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<OptionsHolder.IOptionPattern>>;

public class OptionsHolder
{
    public GeneralOP _generalOP;
    public AsteroidOP _asteroidOP;
    public SunOP _sunOP;
    public PatternsOP _patternsOP;

    private PatternsPhase _patternsPhase;//hold all phases and their patterns
    public Patterns _currentPatterns;//all curent patterns for a difficulty and a phase
    public List<float> _probs;

    public OptionsHolder()
    {
        initVariables();
    }

    public OptionsHolder(JSONObject options)
    {
        initVariables();
        Debug.Log("options: " + options.ToString());
        Debug.Log("general: " + options["General"].ToString());
        //if a field of the object is not present in the JSON representation, that field will be left unchanged.
        JsonUtility.FromJsonOverwrite(options["General"].ToString(), _generalOP);
        Debug.Log("general ok ? : " + _generalOP.adaptationPhaseDuration);
        JsonUtility.FromJsonOverwrite(options["Asteroid"].ToString(), _asteroidOP);
        JsonUtility.FromJsonOverwrite(options["Sun"].ToString(), _sunOP);
        JsonUtility.FromJsonOverwrite(options["PatternsOptions"].ToString(), _patternsOP);
        //JsonUtility.FromJsonOverwrite(options["Patterns"].ToString(), _patterns);
        JSONObject patterns = options["Patterns"];
        Debug.Log("patterns : " + patterns);

        JSONObject phase;
        foreach(string key in patterns.keys)
        {
            Debug.Log("key : " + key);
            phase = patterns[key];
            Debug.Log("phase loading " + key + " ::: " + phase.ToString());
            Patterns optionsPatterns = new Patterns();
            IOptionPattern pa = null;
            foreach (JSONObject pattern in phase.list)
            {
                Debug.Log("pattern " + pattern.ToString());
                switch (pattern["name"].str)
                {
                    case "RotationPattern":
                        pa = new RotationPatternOP();
                        break;
                    case "WavePattern":
                        pa = new WavePatternOP();
                        break;
                    case "CyclicPattern":
                        pa = new CyclicPatternOP();
                        break;
                    case "LaserPattern":
                        pa = new LaserPatternOP();
                        break;
                    default:
                        Debug.Log("PATTERN JSON NULL");
                        pa = null;
                        break;
                }
                if (pa != null)
                    JsonUtility.FromJsonOverwrite(pattern.ToString(), pa);
                optionsPatterns.Add(pa);
            }
            Debug.Log("phase name: " + key);
            _patternsPhase.Add(key, optionsPatterns);
        }
        /*
        foreach(JSONObject phase in patterns.list)
        {
            
        }
        */
    }

    private void initVariables()
    {
        _generalOP = new GeneralOP();//dans ce cas on peut mettre des valeurs par defaut
        _asteroidOP = new AsteroidOP();
        _sunOP = new SunOP();
        _patternsOP = new PatternsOP();
        _patternsPhase = new PatternsPhase();
        _probs = new List<float>();
    }

    public interface IOption{}
    
    [Serializable]
    public class GeneralOP : IOption
    {
        public float gravity;
        public float adaptationPhaseDuration;
        public float asteroidFrequency;
    }

    [Serializable]
    public class AsteroidOP : IOption
    {
        public float force;
        public float rotateMin;
        public float rotateMax;
    }

    [Serializable]
    public class SunOP : IOption
    {
        public int health;
        public float force;
        public float pauseTimeBetweenPhase;
        public int weakPointHealth;
        public float minWaitMove;
        public float maxWaitMove;
        public float timeStatic;
        public float rotatingSpeed;
    }

    [Serializable]
    public class PatternsOP : IOption
    {
        public float startWait;
        public float waveWait;
        public float nextWait;
    }

    public class IOptionPattern
    {
        public string name;
        public float probability;
        public float cooldown;
        public float bulletspeed;
        public float waitAfter;
        public float durationMin;
        public float durationMax;
    }

    [Serializable]
    public class RotationPatternOP : IOptionPattern
    {
        public float frequency;
        public float angle;
        public float radius;
        public float bulletSpeed;
        public float rotatingSpeed;
        public float changeDirection;
        public bool repeatChangeDirection;
        public float maxRadius;
        public int numberHelixes;
    }

    [Serializable]
    public class WavePatternOP : IOptionPattern
    {
        public float frequency;
        public int angleAdd;
        public float angle;
        public float radius;
        public int count;
        public float multiplicatorSpeed;
        public bool angleWillVariateExtern;
        public float angleVariationExtern;
        public bool angleWillVariateIntern;
        public bool chaosOn;
        public int forceAType;
    }

    [Serializable]
    public class CyclicPatternOP : IOptionPattern
    {
        public float angle;
        public float frequency;
        public float mult;
        public float radius;
        public int count;
        public float angleVariation;
    }

    [Serializable]
    public class LaserPatternOP : IOptionPattern
    {
        public float frequency;
        public float duration;
        public float startTime;
        public float rotationSpeed;
    }

    private void computeProbs()
    {
        Debug.Log("computeProbs");
        _probs.Clear();

        //Sort list pattern by probability
        _currentPatterns.Sort((firstPair, nextPair) =>
        {
            return firstPair.probability.CompareTo(nextPair.probability);
        });
        //Probs will be sorted
        foreach (OptionsHolder.IOptionPattern pattern in this._currentPatterns)
        {
            _probs.Add(pattern.probability);
        }

        /*_probs.Sort((firstPair, nextPair) =>
        {
            return firstPair.CompareTo(nextPair);
        });*/
        //_probs.Sort();
        Debug.Log(_probs);
    }

    //switch references for all options
    public void switchDifficulty(OptionsHolder other)
    {
        Debug.Log("switchDifficulty");
        this._generalOP = other._generalOP;
        this._asteroidOP = other._asteroidOP;
        this._sunOP = other._sunOP;
        this._patternsOP = other._patternsOP;
        this._patternsPhase = other._patternsPhase;
    }

    //switch _currentPatterns reference
    public void switchPhase(string name)
    {
        Debug.Log("switchPhase to "+name);
        Debug.Log("phases available:");
        foreach(String key in _patternsPhase.Keys)
        {
            Debug.Log("+++ "+key);
        }
        this._currentPatterns = _patternsPhase[name];
        computeProbs();
        Debug.Log("new first pattern: "+ _currentPatterns[0].name + ", count: " + _currentPatterns.Count);
    }
}
