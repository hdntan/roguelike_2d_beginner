using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BoardManager board;
    [SerializeField] private Vector2Int cellPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        this.board = boardManager;
        this.cellPosition = cell;
        
         // Di chuyển tới đúng vị trí...
        transform.position = boardManager.CellToWorld(cell);

    }
}
