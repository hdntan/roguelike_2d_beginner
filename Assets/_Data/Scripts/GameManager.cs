using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BoardManager boardManager;
    public PlayerController playerController;
    public TurnManager turnManager;
    


    public int foodAmount = 100;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            this.playerController = Instantiate(this.playerController); 
            // Camera.Follow = m_Player.transform;
        
            this.playerController.Init();
          
        }


    public void ChangeFoodAmount(int amount)
    {
        this.foodAmount += amount;

    }
}
