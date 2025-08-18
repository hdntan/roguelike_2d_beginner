using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguelike2D
{
    // WallObject đại diện cho tường trên bản đồ, có thể bị tấn công và phá hủy
    public class WallObject : AttackableCellObject
    {
        public Tile WallTile;           // Tile mặc định của tường
        public Tile WallTileDamaged;    // Tile khi tường bị hư hại
        public int MaxHealth = 3;       // Máu tối đa của tường

        private Tile m_OriginalTile;    // Lưu tile gốc để khôi phục khi tường bị phá
        private int m_CurrentHealth;    // Máu hiện tại của tường
    
        // Hàm khởi tạo, được gọi khi tường được sinh ra trên map
        public override void Init(Vector2Int coord)
        {
            base.Init(coord);

            // Lưu lại tile gốc tại vị trí này
            m_OriginalTile = GameManager.Instance.Board.GetCellTile(coord);
            // Đặt tile tường lên tilemap
            GameManager.Instance.Board.SetCellTile(coord, WallTile);

            // Đặt máu hiện tại về tối đa
            m_CurrentHealth = MaxHealth;
        }

        // Không cho phép người chơi đi vào ô có tường
        public override bool PlayerWantToEnter()
        {
            return false;
        }

        // Khi object bị hủy, xóa khỏi board (nếu cần)
        private void OnDestroy()
        {
            RemoveFromBoard();
        }

        // Hàm nhận sát thương
        public override void Damaged(int amount)
        {
            m_CurrentHealth -= amount;

            // Nếu máu còn 1, đổi tile sang tile hư hại
            if (m_CurrentHealth == 1)
            {
                GameManager.Instance.Board.SetCellTile(m_Cell, WallTileDamaged);
            }
            // Nếu máu còn 0, khôi phục tile gốc và hủy object
            else if (m_CurrentHealth == 0)
            {
                GameManager.Instance.Board.SetCellTile(m_Cell, m_OriginalTile);
                Destroy(gameObject);
            }
        }

        // Lưu trạng thái tường ra file (dùng cho save game)
        public override void Save(BinaryWriter writer)
        {
            writer.Write(m_OriginalTile.name); // Lưu tên tile gốc
            writer.Write(m_CurrentHealth);     // Lưu máu hiện tại
        }

        // Đọc trạng thái tường từ file (dùng cho load game)
        public override void Load(BinaryReader reader)
        {
            string tileId = reader.ReadString();
            m_OriginalTile = GameManager.Instance.ReferenceDatabase.GetTileFromInstanceID(tileId);
            m_CurrentHealth = reader.ReadInt32();
        
            // Nếu máu còn 1, đặt tile hư hại lên tilemap
            if (m_CurrentHealth == 1)
            {
                GameManager.Instance.Board.SetCellTile(m_Cell, WallTileDamaged);
            }
        }
    }
}