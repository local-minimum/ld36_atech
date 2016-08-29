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
    static int max_lvl = 3;
    static int _scoreLast = 0;
    static bool _lastWasNegative = false;

    public static int _score = 0;

    static int[] _lvlThresholds = new int[] { 1000, 2500, 4000 };

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

    public static int ScoreLast
    {
        get
        {
            return _scoreLast;
        }
    }

    public static bool LastWasNegative
    {
        get
        {
            return _lastWasNegative;
        }
    }

    public static void AddScore(int change, bool isNegativeResponse)
    {
        _scoreLast = change;
        _score += change;
        _lastWasNegative = isNegativeResponse;
        Debug.Log(string.Format("Scoring (Total: {0}, Last: {1}, LastHasNegative {2}", _score, _scoreLast, isNegativeResponse));
        if (OnNewScore != null)
        {
            OnNewScore(_score);
        }

        var last = _lvlThresholds.Select((val, i) => new { index = i + 1, value = val }).Where(e => e.value < _score).LastOrDefault();        
        if (last != null && last.value > _lvl)
        {
            NextLevel();
            Debug.Log(string.Format("Level {0}=>{1}", _lvl, last));
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

    public static bool GameOver
    {
        get {
            return _lvl >= max_lvl;
        }
    }
}
