using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Declarações
    public static GameManager I;
    [SerializeField] private Transform[] boardArea;
    [HideInInspector] public static float[] worldBoardBounds = new float[4];
    private List<int> uniqueNumber;
    private List<int> uniqueID;
    /*[HideInInspector] */public int[] playBoard;
    /*[HideInInspector]*/ public SaveData savedGameData;
    private string saveDataPath;

    public delegate void OnLoading();
    public static event OnLoading onFirstGame;
    public static event OnLoading onOngoingMatch;
    public static event OnLoading onRestartGame;

    public delegate void OnSaving();
    public static event OnSaving onGameSave;

    private void Awake()
    {
        I = this;
        saveDataPath = Application.persistentDataPath + "/save data.txt";

        worldBoardBounds[0] = Mathf.Min(boardArea[0].position.x, boardArea[1].position.x, boardArea[2].position.x, boardArea[3].position.x);
        worldBoardBounds[1] = Mathf.Max(boardArea[0].position.x, boardArea[1].position.x, boardArea[2].position.x, boardArea[3].position.x);
        worldBoardBounds[2] = Mathf.Min(boardArea[0].position.y, boardArea[1].position.y, boardArea[2].position.y, boardArea[3].position.y);
        worldBoardBounds[3] = Mathf.Max(boardArea[0].position.y, boardArea[1].position.y, boardArea[2].position.y, boardArea[3].position.y);

        uniqueID = new List<int>();
        for (int i = 0; i <= 14; i++) uniqueID.Add(i);

        playBoard = new int[16];
    }

    private void Start()
    {
        if (System.IO.File.Exists(saveDataPath))
        {
            // Load Save Data
            string json = System.IO.File.ReadAllText(saveDataPath);
            savedGameData = JsonUtility.FromJson<SaveData>(json);

            // Run saved game loading event
            onOngoingMatch?.Invoke();
        }
        else
        {
            // Create new game state

            // Initialize valid tile numbers
            uniqueNumber = new List<int>();
            for (int i = 1; i <= 15; i++) uniqueNumber.Add(i);

            // Run initialization event
            onFirstGame?.Invoke();

            // Save data
            SaveGameData();
        }
    }

    /// <summary>
    /// Returns a unique tile number
    /// </summary>
    /// <returns></returns>
    public int GetNumber()
    {
        int p = Random.Range(0, uniqueNumber.Count);
        int number = uniqueNumber[p];
        uniqueNumber.RemoveAt(p);
        return number;
    }

    /// <summary>
    /// Returns a unique tile id
    /// </summary>
    /// <returns></returns>
    public int GetID()
    {
        int p = Random.Range(0, uniqueID.Count);
        int number = uniqueID[p];
        uniqueID.RemoveAt(p);
        return number;
    }

    /// <summary>
    /// Returns the data for a given tile id
    /// </summary>
    /// <returns></returns>
    public SaveData.TileData GetSavedTileData(int id)
    {
        foreach (SaveData.TileData data in savedGameData.tiles)
        {
            if (data.ID == id) return data;
        }

        return null;
    }

    /// <summary>
    /// Updates the saved game data
    /// </summary>
    public void SaveGameData()
    {
        // Save data
        savedGameData = new SaveData();
        onGameSave?.Invoke();

        // Save json into file
        string json = JsonUtility.ToJson(savedGameData);
        System.IO.File.WriteAllText(saveDataPath, json);
    }

    /// <summary>
    /// Resets the save data
    /// </summary>
    public void RestartGame()
    {
        // Resets available IDs
        uniqueID = new List<int>();
        for (int i = 0; i <= 14; i++) uniqueID.Add(i);

        // Resets available numbers
        uniqueNumber = new List<int>();
        for (int i = 1; i <= 15; i++) uniqueNumber.Add(i);

        // Calls game reset events
        onRestartGame?.Invoke();

        // Call new game event again
        onFirstGame?.Invoke();

        // Saves clean save data
        SaveGameData();
    }

    /// <summary>
    /// Updates the value of the tile occupying a given coordinate
    /// </summary>
    public void UpdateTile(int value, Vector2Int coordinate) => playBoard[coordinate.y * 4 + coordinate.x] = value;

    /// <summary>
    /// Checks if the numbers are in the proper order to win the game
    /// </summary>
    public void CheckForVictory()
    {
        bool victory = true;

        for (int i = 0; i < 15; i++)
        {
            if (playBoard[i] != i++)
            {
                victory = false;
                break;
            }
        }

        if (victory) RunVictorySequence();
    }

    /// <summary>
    /// Executes the sequence when the player wins
    /// </summary>
    [ContextMenu("Automatic Victory")]
    public void RunVictorySequence()
    {
        VictoryScreen.I.OpenScreen();
    }
}
