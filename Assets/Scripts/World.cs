using UnityEngine;
using System.Collections.Generic;

public static class World {

    public static Dictionary<int, KeyValuePair<int, RocketComponent>> RocketBlueprint = new Dictionary<int, KeyValuePair<int, RocketComponent>>();
    
    static int _lvl = 0;

    public static int Level
    {
        get
        {
            return _lvl;
        }
    }

    public static void NextLevel()
    {
        _lvl++;
    }
    
}
