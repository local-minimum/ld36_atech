using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public delegate void LevelEvent(int lvl);
public delegate void ScoreEvent(int score);

public static class World {

    public static event LevelEvent OnNewLevel;
    public static event ScoreEvent OnNewScore;

    public static Dictionary<int, KeyValuePair<int, RocketComponent>> RocketBlueprint = new Dictionary<int, KeyValuePair<int, RocketComponent>>();
    
    static int _lvl = 0;

    public static int _score = 0;
    
    static int[] _lvlThresholds = new int[] {400, 900, 1500, 2500};

    public static int Level
    {
        get
        {
            return _lvl;
        }
    }

    public static int Score {
        get
        {
            return _score;
        }
    }

    public static void AddScore(int change)
    {
        _score += change;

        if (OnNewScore != null)
        {
            OnNewScore(_score);
        }

        int _scoreLvl = _lvlThresholds.Select((val, i) => new { index = i + 1, value = val }).Where(e => e.value < _score).Last().index;
        if (_scoreLvl > _lvl)
        {
            NextLevel();
        }

    }

    static void NextLevel()
    {
        _lvl++;
        if (OnNewLevel != null)
        {
            OnNewLevel(_lvl);
        }
    }
    
    public static void Reset()
    {
        _score = 0;
        _lvl = 0;
        if (OnNewLevel != null)
        {
            OnNewLevel(_lvl);
        }
    }
}
