using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Quản lý bản đồ ô vuông cho game
public class BoardManager : MonoBehaviour
{
    // Mảng 2 chiều lưu dữ liệu từng ô trên bản đồ (không hiển thị trên Inspector)
    private CellData[,] boardData;

    // Tilemap dùng để vẽ bản đồ ô vuông (gán trong Inspector)
    [SerializeField] protected Tilemap tileMap;
 

    // Tham chiếu tới Grid dùng để chuyển đổi vị trí ô sang vị trí thế giới
    [SerializeField]private Grid grid;

    [SerializeField] private LevelData currentLevelData; // Dữ liệu level từ ScriptableObject
    [SerializeField] private List<Vector2Int> emptyCellsList; // Danh sách các ô trống (dùng để spawn vật thể)
    [SerializeField] private List<CellObject> currentAvailableObjectList; // Danh sách vật thể có thể spawn
    [SerializeField] private List<Enemy> currentAvailableEnemyList; // Danh sách quái có thể spawn
    [SerializeField] protected WallObject wallObjectPrefab; // Prefab tường để spawn


      [SerializeField] protected int widthTileMap;
    // Kích thước chiều cao của tilemap (số ô, gán trong Inspector)
    [SerializeField] protected int heightTileMap;
    [SerializeField] protected WorldThemeSO worldTheme; // Chủ đề thế giới từ ScriptableObject

     public BoxCollider2D cameraConfinerBound; // Giới hạn camera

    // Hàm này được gọi một lần khi bắt đầu game
    public void Init()
    {
        this.emptyCellsList = new List<Vector2Int>();
        this.currentAvailableObjectList = new List<CellObject>();
        this.currentAvailableEnemyList = new List<Enemy>();

        this.currentLevelData = GameManager.Instance.worldSetingsSO.levelDatas[GameManager.Instance.currentLevel];
        this.worldTheme = GameManager.Instance.worldSetingsSO.theme;
        this.widthTileMap = this.currentLevelData.Width;
        this.heightTileMap = this.currentLevelData.Height;
        // Lấy component Tilemap từ các thành phần con của GameObject
        this.tileMap = transform.GetComponentInChildren<Tilemap>();
        // Lấy component Grid từ các thành phần con của GameObject
        this.grid = transform.GetComponentInChildren<Grid>();
        // Khởi tạo mảng dữ liệu cho từng ô trên bản đồ
        this.boardData = new CellData[widthTileMap, heightTileMap];

        // Tạo tilemap và dữ liệu cho từng ô
        this.CreateTileMap();

        // Xóa ô spawn của player khỏi danh sách ô trống
        this.emptyCellsList.Remove(new Vector2Int(1, 1));


        this.GenerateObject();  // Sinh vật phẩm, vật thể
        this.GenerateEnemies(); // Sinh quái vật
        this.GenerateWall();    // Sinh tường
                                // this.SpawnItemAtCell(new Vector2Int(3, 3)); // Ví dụ spawn item tại ô (3, 3)
        
        this.UpdateCameraConfiner(); // Cập nhật giới hạn camera theo kích thước bản đồ

    }

    // Tạo tilemap và dữ liệu cho từng ô
    protected virtual void CreateTileMap()
    {
         // Sinh tilemap, mở rộng ra ngoài màn hình để tạo hiệu ứng nền
            for (int y = -20; y < this.heightTileMap + 20; ++y)
            {
                for(int x = -20; x < this.widthTileMap + 20; ++x)
                {
                    // Nếu ngoài vùng chơi, đặt tile nền ngẫu nhiên
                    if (x < 0 || y < 0 || x >= this.widthTileMap || y >= this.heightTileMap)
                    {
                        this.tileMap.SetTile(new Vector3Int(x, y, 0), this.worldTheme.GetRandomGround());
                        continue;
                    }
                
                    Tile tile;
                    this.boardData[x, y] = new CellData();

                    // Viền ngoài là tường không thể đi qua
                    if (x == 0 || y == 0 || x == this.widthTileMap - 1 || y == this.heightTileMap - 1)
                    {
                        tile = this.worldTheme.GetRandomBlocking();
                        this.boardData[x, y].Passable = false;
                    }
                    else
                    {
                        // Các ô còn lại là nền, có thể đi qua, thêm vào danh sách ô trống
                        tile = this.worldTheme.GetRandomGround();
                        this.boardData[x, y].Passable = true;
                        this.emptyCellsList.Add(new Vector2Int(x,y));
                    }

                    this.tileMap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        // // Duyệt qua từng ô trong tilemap theo chiều cao và chiều rộng
        // for (int y = 0; y < this.heightTileMap; ++y)
        // {
        //     for (int x = 0; x < this.widthTileMap; ++x)
        //     {
        //         Tile tile;
        //         // Khởi tạo dữ liệu cho ô hiện tại
        // this.boardData[x, y] = new CellData();

        // // Kiểm tra cellData đã được tạo chưa
        // if (this.boardData[x, y] == null)
        // {
        //     Debug.LogError($"CellData tại ({x},{y}) chưa được tạo!");
        // }


        //         // Nếu là viền bản đồ thì đặt tile tường
        //         if (x == 0 || y == 0 || x == this.widthTileMap - 1 || y == this.heightTileMap - 1)
        //         {
        //             // Chọn ngẫu nhiên một tile từ mảng wallTiles
        //             tile = wallTiles[Random.Range(0, wallTiles.Length)];
        //             this.boardData[x, y].Passable = false; // Ô tường không thể đi qua
        //         }
        //         else
        //         {
        //             // Chọn ngẫu nhiên một tile từ mảng groundTiles
        //             tile = groundTiles[Random.Range(0, groundTiles.Length)];
        //             this.boardData[x, y].Passable = true; // Ô đất có thể đi qua
        //             this.emptyCellsList.Add(new Vector2Int(x, y)); // Thêm ô vào danh sách ô trống
        //         }

        //         // Đặt tile vừa chọn vào vị trí (x, y) trên tilemap
        //         tileMap.SetTile(new Vector3Int(x, y, 0), tile);
        //     }
        // }
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

          public Tile GetCellTile(Vector2Int cellIndex)
        {
            return this.tileMap.GetTile<Tile>((Vector3Int)cellIndex);
        }

         // Đặt tile tại vị trí ô
        public void SetCellTile(Vector2Int cellIndex, Tile tile)
        {
            this.tileMap.SetTile((Vector3Int)cellIndex, tile);
        }
    

   protected  void GenerateObject()
    {
        int itemCount = GameManager.Instance.worldSetingsSO.items.GetRandomItemCount(GameManager.Instance.currentLevel);
        Debug.Log($"Tạo {this.currentAvailableObjectList.Count} vật thể ngẫu nhiên");
        GameManager.Instance.worldSetingsSO.items.GetRandomItemList(ref this.currentAvailableObjectList, GameManager.Instance.currentLevel);
        Debug.Log($"Tạo {itemCount} vật thể ngẫu nhiên");
        for (int i = 0; i < itemCount; ++i)
        {
            int randomIndex = Random.Range(0, this.emptyCellsList.Count);
            Vector2Int coord = this.emptyCellsList[randomIndex];

            this.emptyCellsList.RemoveAt(randomIndex);

            var newObject = Instantiate(this.currentAvailableObjectList[Random.Range(0, this.currentAvailableObjectList.Count)]);
            AddObject(newObject, coord, true);
            Debug.Log($"Đã tạo vật thể {newObject.name} tại ô {coord}");
        }
    }

        protected  void GenerateEnemies()
        {
            int enemyCount = GameManager.Instance.worldSetingsSO.enemies.GetRandomEnemyCount(GameManager.Instance.currentLevel);
            GameManager.Instance.worldSetingsSO.enemies.GetRandomEnemies(ref this.currentAvailableEnemyList, GameManager.Instance.currentLevel);
        
            for (int i = 0; i < enemyCount; ++i)
            {
                int randomIndex = Random.Range(0, this.emptyCellsList.Count);
                Vector2Int coord = this.emptyCellsList[randomIndex];
            
                this.emptyCellsList.RemoveAt(randomIndex);
            
                Enemy newEnemy = Instantiate(this.currentAvailableEnemyList[Random.Range(0, this.currentAvailableEnemyList.Count)]);
                AddObject(newEnemy, coord, false);
            }
        }

          protected  void GenerateWall()
        {
           int wallCount = Random.Range(this.currentLevelData.LowestWallCount, this.currentLevelData.HighestWallCount); // Số lượng tường ngẫu nhiên từ 1 đến 4

            for (int i = 0; i < wallCount; ++i)
            {
                int randomIndex = Random.Range(0, this.emptyCellsList.Count);
                Vector2Int coord = this.emptyCellsList[randomIndex];
            
                this.emptyCellsList.RemoveAt(randomIndex);
            
                WallObject newWall = Instantiate(GameManager.Instance.worldSetingsSO.theme.GetRandomWall());
                AddObject(newWall, coord, false);
            }
        }



    void AddObject(CellObject obj, Vector2Int coord, bool passable)
    {
        
        CellData data = this.boardData[coord.x, coord.y];
        data.Passable = passable; // Đặt ô là có thể đi qua nếu cần
        obj.transform.position = this.CellToWorld(coord);
        data.AddObject(obj);
        obj.Init(coord);
    }

    void UpdateCameraConfiner()
        {
            Vector3 centerPos = CellToWorld(new Vector2Int(this.widthTileMap / 2, this.heightTileMap / 2));

            if (this.widthTileMap % 2 == 0)
            {
                // Nếu chiều rộng chẵn thì dịch tâm sang trái nửa ô
                centerPos.x -= this.tileMap.cellSize.x * 0.5f;
            }

            if (this.heightTileMap % 2 == 0)
            {
                // Nếu chiều cao chẵn thì dịch tâm xuống dưới nửa ô
                centerPos.y -= this.tileMap.cellSize.y * 0.5f;
            }

            cameraConfinerBound.transform.position = centerPos;
            cameraConfinerBound.size = new Vector2((this.widthTileMap) * this.tileMap.cellSize.x,
                (this.heightTileMap) * this.tileMap.cellSize.y);
        
            GameManager.Instance.cameraConfiner.InvalidateBoundingShapeCache();
        }

   
}


