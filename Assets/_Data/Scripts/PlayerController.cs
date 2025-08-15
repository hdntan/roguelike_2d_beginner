using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Tham chiếu tới BoardManager để quản lý bản đồ
    [SerializeField] private BoardManager board;
    // Vị trí hiện tại của người chơi trên lưới (tọa độ ô)
    [SerializeField] private Vector2Int cellPosition;
    // Vị trí đích trong thế giới thực (dùng để di chuyển mượt)
    [SerializeField] private Vector3 moveTarget;

    // Cờ kiểm tra xem người chơi đang di chuyển không
    [SerializeField] private bool isMoving = false;
    // Tốc độ di chuyển của người chơi
    [SerializeField] private float moveSpeed = 2f;

    // Tham chiếu tới Animator để điều khiển animation
    private Animator animator;

    // Hàm Awake được gọi khi script được khởi tạo
    private void Awake()
    {
        this.animator = GetComponent<Animator>(); // Lấy component Animator
    }

    // Hàm Update được gọi mỗi frame
    void Update()
    {
        // Kiểm tra xem người chơi có nhấn phím di chuyển không
        this.CanMove();

        // Nếu đang di chuyển thì thực hiện di chuyển mượt tới vị trí đích
        if (this.isMoving)
        {
            // Di chuyển vị trí thực tế của GameObject tới moveTarget với tốc độ moveSpeed
            transform.position = Vector3.MoveTowards(transform.position, this.moveTarget, this.moveSpeed * Time.deltaTime);
          
            // Khi đã đến đích thì dừng di chuyển và tắt animation "Moving"
            if (transform.position == this.moveTarget)
            {
                this.isMoving = false;
                animator.SetBool("Moving", false);
                var cellData = this.board.GetCellData(this.cellPosition);
                //Nếu có vật thể trong ô thì có thể xử lý tại đây (đã comment lại)
                cellData?.PlayerEntered();
            }

            return; // Không thực hiện các hành động khác khi đang di chuyển
        }

        // Nếu không di chuyển thì kiểm tra hành động tấn công
        this.Attack();
    }

    // Hàm Spawn được gọi khi sinh ra nhân vật
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        this.board = boardManager; // Gán bản đồ
        this.cellPosition = cell;  // Gán vị trí ô

        // Di chuyển tới đúng vị trí ngay lập tức
        this.MoveTo(cell, true);
    }

    // Hàm di chuyển tới một ô mới
    public void MoveTo(Vector2Int cell, bool immediate)
    {
        this.cellPosition = cell; // Cập nhật vị trí ô

        if (immediate)
        {
            // Nếu di chuyển ngay lập tức thì đặt vị trí luôn và tắt di chuyển mượt
            this.isMoving = false;
            transform.position = this.board.CellToWorld(this.cellPosition);
        }
        else
        {
            // Nếu không thì bắt đầu di chuyển mượt tới vị trí mới
            this.isMoving = true;
            this.moveTarget = this.board.CellToWorld(this.cellPosition);
        }

        // Cập nhật animation "Moving"
        this.animator.SetBool("Moving", this.isMoving);
    }

    // Hàm kiểm tra và xử lý di chuyển khi người chơi nhấn phím
    public void CanMove()
    {
        Vector2Int newCellTarget = cellPosition; // Vị trí ô mới dự kiến
        bool hasMoved = false; // Cờ kiểm tra có di chuyển không

        // Kiểm tra các phím mũi tên để xác định hướng di chuyển
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
            transform.localScale = new Vector3(-1, 1, 1); // Quay mặt sang trái
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            transform.localScale = new Vector3(1, 1, 1); // Quay mặt sang phải
            hasMoved = true;
        }

        // Nếu có di chuyển thì kiểm tra ô mới có thể đi qua không
        if (hasMoved)
        {
            CellData cellData = board.GetCellData(newCellTarget);
            Debug.Log("cellData" + cellData); // In ra thông tin ô mới
            // Nếu ô mới tồn tại và có thể đi qua
            if (cellData != null && cellData.Passable)
            {
                // Tăng lượt chơi (tick)
                GameManager.Instance.turnManager.Tick();

                // Di chuyển tới ô mới (không ngay lập tức)
                this.MoveTo(newCellTarget, false);
            }
            else
            {
                Debug.LogError($"Ô {newCellTarget} không hợp lệ hoặc không thể đi qua!"); // In ra lỗi nếu ô không hợp lệ
                Debug.LogError($"Passable {cellData.Passable} không thể đi qua!"); // In ra lỗi nếu ô không thể đi qua
                // Nếu không đi qua được thì in ra log
                Debug.Log("Ô không thể đi qua!");
            }
        }
    }
    
    // Hàm xử lý hành động tấn công
    public virtual void Attack()
    {
        // Nếu nhấn phím Space thì thực hiện tấn công
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Player attacks!"); // In ra log
            this.animator.SetTrigger("Attack"); // Kích hoạt animation "Attack"
        }
    }
}
