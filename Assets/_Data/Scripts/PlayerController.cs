using UnityEngine;
using UnityEngine.InputSystem;

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
        // Kiểm tra xem người chơi có nhấn phím di chuyển không
        this.CanMove();
    }
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        this.board = boardManager;
        this.cellPosition = cell;

        // Di chuyển tới đúng vị trí...
        this.MoveTo(cell);

    }

    public void MoveTo(Vector2Int cell)
    {
        cellPosition = cell;
        transform.position = board.CellToWorld(cellPosition);
    }

    public void CanMove()
    {
        Vector2Int newCellTarget = cellPosition;
        bool hasMoved = false;
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;

        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }

        if (hasMoved)
        {
            CellData cellData = board.GetCellData(newCellTarget);
            // Kiểm tra ô mới có thể đi qua không
            if (cellData != null && cellData.Passable)
            {
                // Di chuyển tới ô mới

                this.MoveTo(newCellTarget);
            }
            else
            {
                Debug.Log("Ô không thể đi qua!");
            }
        }
    }
}
