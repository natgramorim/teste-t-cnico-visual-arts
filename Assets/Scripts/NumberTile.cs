using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Grabs and holds the number value for the tile
/// </summary>
public class NumberTile : MonoBehaviour
{
    [HideInInspector] public int number;
    private int ID;
    private TextMeshProUGUI numberText;
    private Vector3 initialPosition;

    private void Awake()
    {
        numberText = GetComponentInChildren<TextMeshProUGUI>();

        initialPosition = transform.position;

        GameManager.onFirstGame += CreateNewTile;
        GameManager.onOngoingMatch += LoadSavedTile;
        GameManager.onRestartGame += ResetPosition;
        GameManager.onGameSave += RegisterTile;
    }

    /// <summary>
    /// Initializes a new tile for a new game
    /// </summary>
    private void CreateNewTile()
    {
        ID = GameManager.I.GetID();
        number = GameManager.I.GetNumber();
        numberText.text = number.ToString();
    }

    /// <summary>
    /// Loads a saved tile from a saved game
    /// </summary>
    private void LoadSavedTile()
    {
        ID = GameManager.I.GetID();
        SaveData.TileData tileData = GameManager.I.GetSavedTileData(ID);

        // Load number
        number = tileData.number;
        numberText.text = number.ToString();

        // Reposition
        transform.position = tileData.position;
    }

    /// <summary>
    /// Registers the tile information on the main game save
    /// </summary>
    private void RegisterTile()
    {
        SaveData.TileData data = new SaveData.TileData();
        data.ID = ID;
        data.number = number;
        data.position = transform.position;

        GameManager.I.savedGameData.tiles.Add(data);
    }

    /// <summary>
    /// Returns to the original pre-loading position
    /// </summary>
    private void ResetPosition()
    {
        transform.position = initialPosition;
    }

    private void OnDestroy()
    {
        GameManager.onFirstGame -= CreateNewTile;
        GameManager.onOngoingMatch -= LoadSavedTile;
        GameManager.onRestartGame -= ResetPosition;
        GameManager.onGameSave -= RegisterTile;
    }
}
