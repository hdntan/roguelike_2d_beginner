using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;

    // public WorldSettings[] WorldSettings;
    public Tilemap backgroundTilemap;

    public List<Tile> groundTiles;


    private void Awake()
    {
        this.uIDocument = GetComponent<UIDocument>();
       this.backgroundTilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        //this.continueButton = this.uIDocument.rootVisualElement.Q<Button>("Continue");
        var newButton = this.uIDocument.rootVisualElement.Q<Button>("StartGame");
        var quitButton = this.uIDocument.rootVisualElement.Q<Button>("QuitGame");
        var themeDropdown = this.uIDocument.rootVisualElement.Q<DropdownField>("ThemeDropdown");
        Debug.Log("MainMenu Awake called" + themeDropdown + " " + newButton + " " + quitButton);
        newButton.clicked += StartNewGame;
        quitButton.clicked += Application.Quit;
        themeDropdown.choices = new List<string> { "Snow", "Desert", "Forest", "Dungeon" };
        themeDropdown.value = "Snow";
        this.FillBackgroundFromTheme();
    }

    void StartNewGame()
    {

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    void FillBackgroundFromTheme()
    {
        for (int y = -50; y < 50; ++y)
        {
            for (int x = -50; x < 50; ++x)
            {
                this.backgroundTilemap.SetTile(new Vector3Int(x, y, 0), this.RandomTileGround());
            }
        }
    }
    public Tile RandomTileGround()
    {
            return groundTiles[Random.Range(0, groundTiles.Count)];
        }

}
