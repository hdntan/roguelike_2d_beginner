using UnityEngine;
using UnityEngine.Tilemaps;

// Quản lý bản đồ ô vuông cho game
public class BoardManager : MonoBehaviour
{
    // Mảng 2 chiều lưu dữ liệu từng ô trên bản đồ (không hiển thị trên Inspector)
    private CellData[,] boardData;

    // Tilemap dùng để vẽ bản đồ ô vuông (gán trong Inspector)
    [SerializeField] protected Tilemap tileMap;
    // Kích thước chiều rộng của tilemap (số ô, gán trong Inspector)
    [SerializeField] protected int widthTileMap = 8;
    // Kích thước chiều cao của tilemap (số ô, gán trong Inspector)
    [SerializeField] protected int heightTileMap = 8;
    // Các tile nền có thể sử dụng để tạo bản đồ (gán trong Inspector)
    [SerializeField] protected Tile[] groundTiles;
    // Các tile tường có thể sử dụng để tạo viền bản đồ (gán trong Inspector)
    [SerializeField] protected Tile[] wallTiles;
    // Tham chiếu tới PlayerController để spawn nhân vật (gán trong Inspector)
    [SerializeField] protected PlayerController player;
     [SerializeField] protected GameObject itemPrefab;

    // Tham chiếu tới Grid dùng để chuyển đổi vị trí ô sang vị trí thế giới
    private Grid grid;

    // Hàm này được gọi một lần khi bắt đầu game
    public void Init()
    {
        // Lấy component Tilemap từ các thành phần con của GameObject
        this.tileMap = transform.GetComponentInChildren<Tilemap>();
        // Lấy component Grid từ các thành phần con của GameObject
        this.grid = transform.GetComponentInChildren<Grid>();
        // Khởi tạo mảng dữ liệu cho từng ô trên bản đồ
        this.boardData = new CellData[widthTileMap, heightTileMap];

        // Tạo tilemap và dữ liệu cho từng ô
        this.CreateTileMap();

        this.SpawnItemAtCell(new Vector2Int(3, 3)); // Ví dụ spawn item tại ô (3, 3)
    
    }

    // Tạo tilemap và dữ liệu cho từng ô
    protected virtual void CreateTileMap()
    {
        // Duyệt qua từng ô trong tilemap theo chiều cao và chiều rộng
        for (int y = 0; y < this.heightTileMap; ++y)
        {
            for (int x = 0; x < this.widthTileMap; ++x)
            {
                Tile tile;
                // Khởi tạo dữ liệu cho ô hiện tại
                this.boardData[x, y] = new CellData();

                // Nếu là viền bản đồ thì đặt tile tường
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

    // Chuyển vị trí ô sang vị trí thế giới
    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return this.grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= widthTileMap ||
           cellIndex.y < 0 || cellIndex.y >= heightTileMap)
        {
            Debug.LogError($"Vị trí ô {cellIndex} không hợp lệ!");
            return null;
        }
        return this.boardData[cellIndex.x, cellIndex.y];
    }

    public void SpawnItemAtCell(Vector2Int cell)
    {
        // Kiểm tra vị trí hợp lệ
        var cellData = this.GetCellData(cell);
        if (cellData == null & cellData.Passable == false)
        {
            Debug.LogError("Vị trí spawn item không hợp lệ!");
            return;
        }

        // Tạo item tại vị trí ô trên bản đồ
        Vector3 spawnPos = this.CellToWorld(cell);
        GameObject itemObj = Instantiate(itemPrefab, spawnPos, Quaternion.identity);

        // Gán item vào ContainedObject của CellData (item phải implement ICellObject)
        ICellObject cellObject = itemObj.GetComponent<ICellObject>();
        if (cellObject == null)
        {
            Debug.LogError("Prefab item không có component ICellObject!");
            Destroy(itemObj);
            return;
        }
        cellData.ContainedObject = cellObject;
    }
}

// Lưu dữ liệu cho từng ô trên bản đồ
[System.Serializable]
public class CellData
{
    public bool Passable; // Ô có thể đi qua

    // Vật thể nằm trên ô (có thể là quái, vật phẩm, bẫy...)
    public ICellObject ContainedObject;
}

