using UnityEngine;

public class Enemy : CellObject
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

    void Update()
    {

        if (this.isMoving)
        {
            // Di chuyển vị trí thực tế của GameObject tới moveTarget với tốc độ moveSpeed
            transform.position = Vector3.MoveTowards(transform.position, this.moveTarget, 3f * Time.deltaTime);

            // Khi đã đến đích thì dừng di chuyển và tắt animation "Moving"
            if (transform.position == this.moveTarget)
            {
                this.isMoving = false;
                this.animator.SetBool("isMoving", false);
                //var cellData = this.board.GetCellData(this.cellPosition);
                //Nếu có vật thể trong ô thì có thể xử lý tại đây (đã comment lại)
                // cellData?.PlayerEntered();
            }

            return; // Không thực hiện các hành động khác khi đang di chuyển
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.turnManager.OnTick -= TurnHappened;
    }
    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        this.currentHealth = this.health;
    }

    public override bool PlayerWantToEnter()
    {
        this.currentHealth -= 1;
        if (this.currentHealth <= 0)
        {

            Destroy(gameObject);

        }
        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.boardManager;
        var targetCell = board.GetCellData(coord);


        if (targetCell == null
            || !targetCell.Passable
            || targetCell.uniqueCellObject != null)
        {
            this.isMoving = false;
            return false;
        }

        //remove enemy from current cell
        var currentCell = board.GetCellData(this.cell);
        currentCell.RemoveObject(this);

        //   //add it to the next cell
        //   targetCell.ContainedObject = this;
        this.cell = coord;
        targetCell.AddObject(this);
        this.moveTarget = board.CellToWorld(coord);
        this.isMoving = true;
        this.animator.SetBool("isMoving", true);



        return true;
    }
    void TurnHappened()
    {
        var playerCell = GameManager.Instance.playerController.CellPosition; ;

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

}
