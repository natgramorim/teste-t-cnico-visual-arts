using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public class TileData
    {
        public int ID;
        public int number;
        public Vector3 position;
    }

    public List<TileData> tiles = new List<TileData>();
}