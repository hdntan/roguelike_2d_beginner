using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;

    public WorldSO[] worldSettings;
    public Tilemap backgroundTilemap;



    [SerializeField] private DropdownField worldSettingsThemeDropdown;
    [SerializeField] private WorldSO selectedSettings;


    private void Awake()
    {
        this.uIDocument = GetComponent<UIDocument>();
        this.backgroundTilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        //this.continueButton = this.uIDocument.rootVisualElement.Q<Button>("Continue");
        var newButton = this.uIDocument.rootVisualElement.Q<Button>("StartGame");
        var quitButton = this.uIDocument.rootVisualElement.Q<Button>("QuitGame");
        this.worldSettingsThemeDropdown = this.uIDocument.rootVisualElement.Q<DropdownField>("ThemeDropdown");

        newButton.clicked += StartNewGame;
        quitButton.clicked += Application.Quit;

        this.selectedSettings = this.worldSettings[0];
        var worldNames = this.worldSettings.Select(settings => settings.worldName).ToList();
        this.worldSettingsThemeDropdown.choices = worldNames;


        this.worldSettingsThemeDropdown.RegisterValueChangedCallback(evt =>
        {
            this.selectedSettings = this.worldSettings[this.worldSettingsThemeDropdown.index];
            FillBackgroundFromTheme();
        });
        this.worldSettingsThemeDropdown.index = 0;
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
                this.backgroundTilemap.SetTile(new Vector3Int(x, y, 0), this.selectedSettings.theme.GetRandomGround());
            }
        }
    }
}
