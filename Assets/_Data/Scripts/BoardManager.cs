using UnityEngine;
using UnityEngine.Tilemaps;


public class BoardManager : MonoBehaviour
{

    private CellData[,] boardData; // Mảng 2 chiều chứa dữ liệu của bảng
    // Tilemap dùng để vẽ bản đồ ô vuông
    [SerializeField] protected Tilemap tileMap;
    // Chiều rộng của tilemap (số ô)
    [SerializeField]protected int widthTileMap = 8;
    // Chiều cao của tilemap (số ô)
    [SerializeField]protected int heightTileMap = 8;
    // Mảng các tile nền có thể sử dụng để tạo bản đồ
    [SerializeField]protected Tile[] groundTiles;

    [SerializeField]protected Tile[] wallTiles;

    // Hàm này được gọi một lần khi bắt đầu game
    void Start()
    {
        // Lấy component Tilemap từ các thành phần con của GameObject
        this.tileMap = transform.GetComponentInChildren<Tilemap>();
        this.boardData = new CellData[widthTileMap, heightTileMap];

        this.CreateTileMap();
       
    }

    protected virtual void CreateTileMap()
    {
     // Duyệt qua từng ô trong tilemap theo chiều cao và chiều rộng
        for (int y = 0; y < this.heightTileMap; ++y)
        {
            for (int x = 0; x < this.widthTileMap; ++x)
            {
                Tile tile;
                this.boardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == this.widthTileMap - 1 || y == this.heightTileMap - 1)
                {
                    // Chọn ngẫu nhiên một tile từ mảng wallTiles
                    tile = wallTiles[Random.Range(0, wallTiles.Length)];
                    this.boardData[x, y].Passable = false; // Ô tường không thể đi qua
                   
                }
                else
                {
                    // Chọn ngẫu nhiên một tile từ mảng groundTiles
                    tile = groundTiles[Random.Range(0, groundTiles.Length)];
                    this.boardData[x, y].Passable = true; // Ô đất có thể đi qua
                    
                }


                // Đặt tile vừa chọn vào vị trí (x, y) trên tilemap
                tileMap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
   }
}
    [System.Serializable]
     public class CellData
    {
        public bool Passable; // Ô có thể đi qua hay không
    }
