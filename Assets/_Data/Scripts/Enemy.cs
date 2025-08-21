using Unity.VisualScripting;
using UnityEngine;

public class Enemy : AttackableCellObject
{
    public int health = 3;

    [SerializeField] private int currentHealth;
    [SerializeField] private Vector3 moveTarget;
    [SerializeField] private bool isMoving = false;
    [SerializeField] protected Animator animator;
    private void Awake()
    {
        GameManager.Instance.turnManager.OnTick += TurnHappened;
        this.animator = GetComponent<Animator>();
    }

    // void Update()
    // {

    //     if (this.isMoving)
    //     {
    //         // Di chuyển vị trí thực tế của GameObject tới moveTarget với tốc độ moveSpeed
    //         transform.position = Vector3.MoveTowards(transform.position, this.moveTarget, 3f * Time.deltaTime);

    //         // Khi đã đến đích thì dừng di chuyển và tắt animation "Moving"
    //         if (transform.position == this.moveTarget)
    //         {
    //             this.isMoving = false;
    //             this.animator.SetBool("isMoving", false);
    //             //var cellData = this.board.GetCellData(this.cellPosition);
    //             //Nếu có vật thể trong ô thì có thể xử lý tại đây (đã comment lại)
    //             // cellData?.PlayerEntered();
    //         }

    //         return; // Không thực hiện các hành động khác khi đang di chuyển
    //     }
    // }

    private void OnDestroy()
    {
        this.RemoveFromBoard();
        GameManager.Instance.turnManager.OnTick -= TurnHappened;
    }
    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        this.currentHealth = this.health;
    }

    public override bool PlayerWantToEnter()
    {
        
        return false;
    }

    // bool MoveTo(Vector2Int coord)
    // {

    //     var targetCell = GameManager.Instance.boardManager.GetCellData(coord);


    //     if (targetCell == null
    //         || !targetCell.Passable
    //         || targetCell.uniqueCellObject != null)
    //     {
    //         this.isMoving = false;
    //         return false;
    //     }

    //     //remove enemy from current cell
    //     CellData currentCell = GameManager.Instance.boardManager.GetCellData(this.cell);
    //     Debug.Log($"Removing enemy from cell {currentCell} at position {this.cell}");
    //     currentCell.RemoveObject(this);

    //     //   //add it to the next cell
    //     //   targetCell.ContainedObject = this;
    //     this.cell = coord;
    //     targetCell.AddObject(this);
    //     this.moveTarget = GameManager.Instance.boardManager.CellToWorld(coord);
    //     this.isMoving = true;
    //     this.animator.SetBool("isMoving", true);



    //     return true;
    // }
    
    
        bool MoveTo(Vector2Int coord)
        {
            var targetCell = GameManager.Instance.boardManager.GetCellData(coord);
            Debug.Log($"Moving enemy to cell {targetCell} at position {coord}");

            if (targetCell == null
            || !targetCell.Passable
            || targetCell.uniqueCellObject != null)
        {
            return false;
        }
        
            //remove enemy from current cell
            var currentCell = GameManager.Instance.boardManager.GetCellData(this.cell);
            currentCell.RemoveObject(this);
        
            // if(WalkSFX.Length > 0)
            //     GameManager.Instance.PlayAudioSFX(WalkSFX[Random.Range(0, WalkSFX.Length)], transform.position);
        
            //added right away to the next cell, as other thing will test if that cell is free right now. The movement is
            //only visual, internally the enemy is already on the new cell
            this.cell = coord;
            targetCell.AddObject(this);

            // GameManager.Instance.MovingObjectSystem.AddMoveRequest(transform,  GameManager.Instance.Board.CellToWorld(coord),
            //     4.0f, false, 1, isMidway =>
            //     {
            //         m_Animator.SetBool("Moving", false);
            //     }, () =>
            //     {
            //         m_Animator.SetBool("Moving", true);
            //     });

            return true;
        }


    void TurnHappened()
    {
        var playerCell = GameManager.Instance.player.CellPosition; ;

        int xDist = playerCell.x - this.cell.x;
        int yDist = playerCell.y - this.cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        // Nếu ở ngay cạnh player
        if ((xDist == 0 && absYDist == 1)
            || (yDist == 0 && absXDist == 1))
        {
            GameManager.Instance.ChangeFoodAmount(-10); // Tấn công player
        }
        else
        {
            // Nếu không cạnh player → cố gắng di chuyển gần player hơn
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }
    bool TryMoveInX(int xDist)
    {
        if (xDist > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Quay mặt sang phải
            return MoveTo(this.cell + Vector2Int.right);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // Quay mặt sang trái
            return MoveTo(this.cell + Vector2Int.left);

        }
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0) return MoveTo(this.cell + Vector2Int.up);
        return MoveTo(this.cell + Vector2Int.down);
    }

    public override void Damaged(int amount)
    {
        this.currentHealth -= amount;
        if(this.currentHealth <= 0)
        {
         
            Destroy(gameObject); // Xóa enemy khỏi game
        }
      
    }
}
