using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Roguelike2D
{
    // Điều khiển nhân vật người chơi, đồng thời implement interface ITurnEntity để tham gia hệ thống lượt
    public class PlayerController : MonoBehaviour, TurnManager.ITurnEntity
    {
        // Tốc độ di chuyển của nhân vật
        public float MoveSpeed = 5.0f;
        // Asset chứa các InputAction (gán trong Inspector)
        public InputActionAsset InputActionAsset;

        // Các chỉ số khởi đầu của nhân vật
        public int StartAttack = 1;
        public int StartDefense = 0;
        public int StartSpeed = 1;    
    
        // Các thuộc tính chỉ đọc trả về chỉ số hiện tại
        public int PlayerAttack => m_CurrentAttack;
        public int PlayerDefense => m_CurrentDefense;
        public int PlayerSpeed => m_CurrentSpeed;
    
        // Vị trí hiện tại trên lưới (ô)
        public Vector2Int Cell => m_CellPosition;

        [Header("Audio")] 
        public AudioClip[] WalkingSFX;
        public AudioClip[] AttackSFX;
        public AudioClip[] DamageSFX;
    
        private BoardManager m_Board; // Tham chiếu tới BoardManager
        private Vector2Int m_CellPosition; // Vị trí hiện tại trên lưới

        private bool m_IsPaused; // Đang tạm dừng game
        private bool m_IsGameOver; // Đã kết thúc game

        private bool m_ControlLocked; // Khóa điều khiển khi đang di chuyển hoặc thực hiện hành động

        // Các chỉ số hiện tại của nhân vật
        private int m_CurrentAttack;
        private int m_CurrentDefense;
        private int m_CurrentSpeed;

        private SpriteRenderer m_SpriteRenderer; // Để lật sprite khi di chuyển trái/phải
        private Animator m_Animator; // Để điều khiển animation

        // Các InputAction cho từng phím di chuyển và chờ
        private InputAction m_MoveUpAction;
        private InputAction m_MoveRightAction;
        private InputAction m_MoveDownAction;
        private InputAction m_MoveLeftAction;
        private InputAction m_WaitAction;

        // Hàm Awake khởi tạo các thành phần cần thiết
        private void Awake()
        {
            m_Animator = GetComponent<Animator>();

            // Lấy các InputAction từ asset
            m_MoveUpAction = InputActionAsset.FindAction("Gameplay/MoveUp");
            m_MoveUpAction.Enable();
            m_MoveRightAction = InputActionAsset.FindAction("Gameplay/MoveRight");
            m_MoveRightAction.Enable();
            m_MoveDownAction = InputActionAsset.FindAction("Gameplay/MoveDown");
            m_MoveDownAction.Enable();
            m_MoveLeftAction = InputActionAsset.FindAction("Gameplay/MoveLeft");
            m_MoveLeftAction.Enable();

            m_WaitAction = InputActionAsset.FindAction("Gameplay/Wait");
            m_WaitAction.Enable();

            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Hàm tấn công mục tiêu (thường là quái vật hoặc vật thể có thể bị tấn công)
        public void Attacking(AttackableCellObject target)
        {
            var targetObject = target as MonoBehaviour;
        
            // Yêu cầu hệ thống di chuyển thực hiện animation di chuyển tới mục tiêu rồi quay về
            GameManager.Instance.MovingObjectSystem.AddMoveRequest(transform, targetObject.transform.position,
                MoveSpeed, true, 0, isMidway =>
                {
                    // Khi tới giữa đường (isMidway == true), gây sát thương cho mục tiêu
                    if (isMidway)
                    {
                        target.Damaged(m_CurrentAttack);
                        // Phát âm thanh tấn công nếu có
                        if(AttackSFX.Length != 0)
                            GameManager.Instance.PlayAudioSFX(AttackSFX[Random.Range(0, AttackSFX.Length)], transform.position);
                    }
                });
            m_Animator.SetTrigger("Attack"); // Kích hoạt animation tấn công
        }

        // Hàm nhận sát thương
        public void Damage(int damageAmount)
        {
            GameManager.Instance.ChangeFood(-damageAmount); // Trừ máu hoặc tài nguyên
            m_Animator.SetTrigger("Hit"); // Animation bị đánh
            // Phát âm thanh bị đánh nếu có
            if(DamageSFX.Length != 0)
                GameManager.Instance.PlayAudioSFX(DamageSFX[Random.Range(0, DamageSFX.Length)], transform.position);
        }
    
        // Hàm spawn nhân vật tại vị trí ô trên bản đồ
        public void Spawn(BoardManager board, Vector2Int cell)
        {
            m_Board = board;
            MoveTo(cell, true); // Di chuyển ngay lập tức tới vị trí spawn
        }

        // Di chuyển tới một ô mới, immediateMove = true thì dịch chuyển ngay lập tức, false thì di chuyển mượt
        public void MoveTo(Vector2Int cell, bool immediateMove)
        {
            m_CellPosition = cell;

            if (immediateMove)
            {
                transform.position = m_Board.CellToWorld(m_CellPosition);
                m_ControlLocked = false;
            }
            else
            {
                BoardManager.CellData cellData;
                // Yêu cầu hệ thống di chuyển thực hiện animation di chuyển tới ô mới
                GameManager.Instance.MovingObjectSystem.AddMoveRequest(transform, m_Board.CellToWorld(m_CellPosition),
                    MoveSpeed, false, 0, isMidway =>
                    {
                        m_ControlLocked = false;
                        m_Animator.SetBool("Moving", false);

                        cellData = m_Board.GetCellData(m_CellPosition);
                        cellData.PlayerEntered(); // Gọi hàm xử lý khi người chơi bước vào ô này
                    });
            
                // Phát âm thanh bước chân nếu có
                if(WalkingSFX.Length != 0)
                    GameManager.Instance.PlayAudioSFX(WalkingSFX[Random.Range(0, WalkingSFX.Length)], transform.position);

                m_ControlLocked = true; // Khóa điều khiển khi đang di chuyển
                m_Animator.SetBool("Moving", m_ControlLocked); // Bật animation di chuyển
            }
        }

        // Khởi tạo lại trạng thái nhân vật khi bắt đầu game mới
        public void Init()
        {
            m_IsGameOver = false;
            m_ControlLocked = false;
            m_IsPaused = false;

            m_CurrentAttack = StartAttack;
            m_CurrentDefense = StartDefense;
            m_CurrentSpeed = StartSpeed;
        
            GameManager.Instance.UpdatePlayerStats(); // Cập nhật UI chỉ số
            GameManager.Instance.TurnManager.RegisterPlayer(this); // Đăng ký vào hệ thống lượt
        }
    
        // Đánh dấu game đã kết thúc
        public void GameOver()
        {
            m_IsGameOver = true;
        }

        // Tạm dừng game
        public void Pause()
        { 
            m_IsPaused = true;
        }

        // Bỏ tạm dừng game
        public void Unpause()
        {
            m_IsPaused = false;
        }
    
        // Hàm Update xử lý input và logic di chuyển/tấn công mỗi frame
        private void Update()
        {
            if (m_IsPaused)
            {
                return;
            }
        
            if (m_IsGameOver)
            {
                // Nếu game over, nhấn Enter để bắt đầu lại
                if (Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    GameManager.Instance.StartOrLoadNewGame();
                }
                return;
            }
        
            if (m_ControlLocked)
                return; // Đang di chuyển hoặc thực hiện hành động thì không nhận input

            // Nếu nhấn phím chờ (Wait), kết thúc lượt với năng lượng cố định
            if (m_WaitAction.WasPerformedThisFrame())
            {
                GameManager.Instance.TurnManager.PlayerAct(10);
                return;
            }
        
            Vector2Int newCellTarget = m_CellPosition;
            bool hasMove = false;

            // Kiểm tra các phím di chuyển, cập nhật vị trí mục tiêu và trạng thái lật sprite
            if (m_MoveUpAction.WasPressedThisFrame())
            {
                newCellTarget.y += 1;
                hasMove = true;

            }
            else if (m_MoveDownAction.WasPressedThisFrame())
            {
                newCellTarget.y -= 1;
                hasMove = true;
            }

            if (m_MoveRightAction.WasPressedThisFrame())
            {
                newCellTarget.x += 1;
                hasMove = true;
                m_SpriteRenderer.flipX = false; // Quay mặt sang phải
            }
            else if(m_MoveLeftAction.WasPressedThisFrame())
            {
                newCellTarget.x -= 1;
                hasMove = true;
                m_SpriteRenderer.flipX = true; // Quay mặt sang trái
            }
        
            if (!hasMove) return; // Không có input di chuyển thì thoát
        
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
            // Nếu ô hợp lệ và có thể đi qua
            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.PlayerInput(); // Xử lý input người chơi (ví dụ: trừ năng lượng...)

                // Nếu ô không có vật thể cản trở, di chuyển tới ô mới
                if (cellData.ContainedObjects.Count == 0)
                {
                    MoveTo(newCellTarget, false);
                    GameManager.Instance.TurnManager.PlayerAct(10); // Kết thúc lượt
                }
                // Nếu có vật thể có thể tấn công (quái, v.v.)
                else if (cellData.HaveAttackable(out var attackable))
                {
                    Attacking(attackable);
                    GameManager.Instance.TurnManager.PlayerAct(10); // Kết thúc lượt
                } 
                // Nếu có vật thể đặc biệt cho phép vào (ví dụ: cửa, vật phẩm...)
                else if (cellData.PlayerWantToEnter())
                {
                    MoveTo(newCellTarget, false);
                    GameManager.Instance.TurnManager.PlayerAct(10); // Kết thúc lượt
                }
            }
        }

        // Trả về năng lượng lượt của nhân vật (dùng cho hệ thống lượt)
        public int GetTurnEnergy()
        {
            return m_CurrentSpeed * 10;
        }

        // Lưu trạng thái nhân vật ra file (dùng cho save game)
        public void Save(BinaryWriter writer)
        {
            writer.Write(m_CellPosition.x);
            writer.Write(m_CellPosition.y);
        
            writer.Write(m_CurrentAttack);
            writer.Write(m_CurrentDefense);
            writer.Write(m_CurrentSpeed);
        }

        // Đọc trạng thái nhân vật từ file (dùng cho load game)
        public void Load(BinaryReader reader)
        {
            m_CellPosition.x = reader.ReadInt32();
            m_CellPosition.y = reader.ReadInt32();
        
            m_CurrentAttack = reader.ReadInt32();
            m_CurrentDefense = reader.ReadInt32();
            m_CurrentSpeed = reader.ReadInt32();
        }
    }
}