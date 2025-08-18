using System.IO;
using UnityEngine;

namespace Roguelike2D
{
    /// <summary>
    /// Base class for everything that can live inside a cell on the board.
    /// Lớp cha cho mọi vật thể có thể tồn tại trong một ô trên bản đồ (item, enemy, wall, ...)
    /// </summary>
    public class CellObject : MonoBehaviour
    {
        protected Vector2Int m_Cell; // Vị trí ô hiện tại của object trên bản đồ

        [HideInInspector]
        [SerializeField]
        protected string m_ID; // ID dùng để lưu/khôi phục object khi save/load game

        public string ID => m_ID; // Thuộc tính chỉ đọc trả về ID

        // Hàm khởi tạo, được gọi khi object được đặt vào một ô trên bản đồ
        public virtual void Init(Vector2Int coord)
        {
            m_Cell = coord;
        }

        // Xóa object này khỏi danh sách vật thể trong ô hiện tại
        public void RemoveFromBoard()
        {
            var data = GameManager.Instance.Board.GetCellData(m_Cell);
            data.RemoveObject(this);
        }

        /// <summary>
        /// Override this to return true if the CellObject need to be Unique. Only 1 Unique Cell Object can exist at a time
        /// in a given cell. This help for example to stop Enemy and Wall to occupy the same Cell.
        /// </summary>
        /// <returns>True nếu object này là duy nhất trong ô (ví dụ: Wall, Enemy), false nếu có thể nhiều object cùng ô</returns>
        public virtual bool IsUnique()
        {
            return false;
        }

        /// <summary>
        /// Kiểm tra xem player có được phép đi vào ô chứa object này không.
        /// </summary>
        /// <returns>True nếu cho phép, false nếu không (ví dụ: tường không cho vào)</returns>
        public virtual bool PlayerWantToEnter()
        {
            return true;
        }
    
        // Được gọi khi player bước vào ô chứa object này
        public virtual void PlayerEntered()
        {
            // Override ở class con để xử lý logic (nhặt item, nhận sát thương, ...)
        }

        // Đặt ID cho object (dùng cho save/load)
        public void SetID()
        {
            var root = transform.root;
            m_ID = root.gameObject.name;
        }

        // Lưu trạng thái object ra file (override ở class con nếu cần)
        public virtual void Save(BinaryWriter writer)
        {
        
        }

        // Đọc trạng thái object từ file (override ở class con nếu cần)
        public virtual void Load(BinaryReader reader)
        {
        
        }
    }
}