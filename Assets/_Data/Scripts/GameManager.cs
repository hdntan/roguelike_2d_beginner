using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BoardManager boardManager;
    public PlayerController player;
    public TurnManager turnManager;

    public WorldSO worldSetingsSO;

    public int currentLevel = 1;

    public CinemachineCamera camera;
    public CinemachineConfiner2D cameraConfiner;



    public int foodAmount = 100;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (CrossSceneDataStore.Instance != null)
        {
            this.worldSetingsSO = CrossSceneDataStore.Instance.selectedWorldSettings;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.cameraConfiner = camera.GetComponent<CinemachineConfiner2D>();
        this.turnManager = new TurnManager();
        this.boardManager.Init();
        this.StartOrLoadNewGame();
        // this.playerController.Spawn(this.boardManager, new Vector2Int(1, 1));

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartOrLoadNewGame()
    {
        this.CreatePlayer();
    }

    void CreatePlayer()
    {
        this.player = Instantiate(this.worldSetingsSO.theme.playerPrefab);
        camera.Follow = this.player.transform;

        this.player.Init();
        Debug.Log("Player created and initialized.");   

    }


    public void ChangeFoodAmount(int amount)
    {
        this.foodAmount += amount;

    }
}
