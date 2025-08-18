using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : AttackableCellObject
{
      public Tile wallTile;           // Tile mặc định của tường
        public Tile wallTileDamaged;    // Tile khi tường bị hư hại
        public int maxHealth = 3;       // Máu tối đa của tường

        [SerializeField]private Tile originalTile;    // Lưu tile gốc để khôi phục khi tường bị phá
        [SerializeField]private int currentHealth;    // Máu hiện tại của tường


    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        // Lưu lại tile gốc tại vị trí này
        this.originalTile = GameManager.Instance.boardManager.GetCellTile(coord);
        // Đặt tile tường lên tilemap
        GameManager.Instance.boardManager.SetCellTile(coord, this.wallTile);

        this.currentHealth = this.maxHealth;
            
        }
       // Không cho phép người chơi đi vào ô có tường
        public override bool PlayerWantToEnter()
        {
            return false;
        }

        private void OnDestroy()
        {
            RemoveFromBoard();
        }
    public override void Damaged(int amount)
    {
    this.currentHealth -= amount;

        // Nếu máu còn 1, đổi tile sang tile hư hại
        if (this.currentHealth == 1)
        {
            GameManager.Instance.boardManager.SetCellTile(this.cell, this.wallTileDamaged);
        }
        // Nếu máu còn 0, khôi phục tile gốc và hủy object
        else if (this.currentHealth <= 0)
        {
            GameManager.Instance.boardManager.SetCellTile(this.cell, this.originalTile);
            Destroy(gameObject);
        }
    }
}
