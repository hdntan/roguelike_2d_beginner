using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Roguelike2D
{
    /// <summary>
    /// Main class that handle the Board, the surface on which the game happens. It will keep data for each cell, and take
    /// care of generating the level randomly.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        /// <summary>
        /// Class that store data for a single cell of the Board
        /// </summary>
        public class CellData
        {
            public bool Passable; // Ô này có thể đi qua không
            public List<CellObject> ContainedObjects = new(); // Danh sách các vật thể trong ô này
            public CellObject UniqueCellObject = null; // Vật thể duy nhất (ví dụ: cửa ra, boss...)

            // Kiểm tra xem tất cả vật thể trong ô có cho phép người chơi vào không
            public bool PlayerWantToEnter()
            {
                foreach (var cellObject in ContainedObjects)
                {
                    if (!cellObject.PlayerWantToEnter())
                        return false;
                }
                return true;
            }

            // Gọi hàm PlayerEntered() của tất cả vật thể trong ô khi người chơi bước vào
            public void PlayerEntered()
            {
                foreach (var cellObject in ContainedObjects)
                {
                    cellObject.PlayerEntered();
                }
            }

            // Ô này có vật thể nào không
            public bool HasObjects() => ContainedObjects.Count > 0;

            // Thêm vật thể vào ô, kiểm tra nếu là vật thể duy nhất
            public void AddObject(CellObject obj)
            {
                if (obj.IsUnique())
                {
                    if (UniqueCellObject != null)
                    {
                        // Nếu đã có vật thể duy nhất thì báo lỗi
                        Debug.LogError($"Tried to the unique cell object {obj.name} to a cell already containing one {UniqueCellObject.name}");
                        return;
                    }
                    UniqueCellObject = obj;
                }
                ContainedObjects.Add(obj);
            }

            // Kiểm tra ô có vật thể có thể bị tấn công không (ví dụ: quái vật)
            public bool HaveAttackable(out AttackableCellObject attackable)
            {
                if (UniqueCellObject != null && UniqueCellObject is AttackableCellObject obj)
                {
                    attackable = obj;
                    return true;
                }
                attackable = null;
                return false;
            }

            // Xóa vật thể khỏi ô
            public void RemoveObject(CellObject obj)
            {
                if (obj == UniqueCellObject)
                    UniqueCellObject = null;
                ContainedObjects.Remove(obj);
            }

            // Xóa tất cả vật thể trong ô (dùng khi reset map)
            public void ClearObjects()
            {
                foreach (var cellObject in ContainedObjects)
                {
                    Destroy(cellObject.gameObject);
                }
                ContainedObjects.Clear();
            }
        }
    
        public BoxCollider2D CameraConfinerBound; // Giới hạn camera

        [SerializeField]private Tilemap m_Tilemap; // Tilemap để vẽ bản đồ
        [SerializeField]private Grid m_Grid; // Grid để chuyển đổi vị trí ô <-> thế giới
        [SerializeField]private CellData[,] m_BoardData; // Dữ liệu từng ô trên bản đồ
        [SerializeField]private WorldSettings.LevelData m_CurrentLevelData; // Dữ liệu level hiện tại
        [SerializeField]private List<Vector2Int> m_EmptyCellsList; // Danh sách các ô trống (dùng để spawn vật thể)
        [SerializeField]private List<CellObject> m_CurrentAvailableObjectList; // Danh sách vật thể có thể spawn
        [SerializeField]private List<Enemy> m_CurrentAvailableEnemyList; // Danh sách quái có thể spawn

        private int m_Width; // Chiều rộng bản đồ
        private int m_Height; // Chiều cao bản đồ

        // Hàm Awake lấy các component cần thiết
        public void Awake()
        {
            m_Tilemap = GetComponentInChildren<Tilemap>();
            m_Grid = GetComponent<Grid>();
        }

        /// <summary>
        /// Khởi tạo bản đồ, sinh ngẫu nhiên dựa trên dữ liệu level
        /// </summary>
        public void Init()
        {
            m_EmptyCellsList = new List<Vector2Int>();
            m_CurrentAvailableObjectList = new List<CellObject>();
            m_CurrentAvailableEnemyList = new List<Enemy>();

            m_CurrentLevelData = GameManager.Instance.WorldSettings.GetLevelDataForLevel(GameManager.Instance.CurrentLevel);
            var theme = GameManager.Instance.WorldSettings.Theme;

            m_Width = m_CurrentLevelData.Width;
            m_Height = m_CurrentLevelData.Height;
        
            m_BoardData = new CellData[m_Width, m_Height];

            // Sinh tilemap, mở rộng ra ngoài màn hình để tạo hiệu ứng nền
            for (int y = -20; y < m_Height + 20; ++y)
            {
                for(int x = -20; x < m_Width + 20; ++x)
                {
                    // Nếu ngoài vùng chơi, đặt tile nền ngẫu nhiên
                    if (x < 0 || y < 0 || x >= m_Width || y >= m_Height)
                    {
                        m_Tilemap.SetTile(new Vector3Int(x, y, 0), theme.GetRandomGround());
                        continue;
                    }
                
                    Tile tile;
                    m_BoardData[x, y] = new CellData();

                    // Viền ngoài là tường không thể đi qua
                    if (x == 0 || y == 0 || x == m_Width - 1 || y == m_Height - 1)
                    {
                        tile = theme.GetRandomBlocking();
                        m_BoardData[x, y].Passable = false;
                    }
                    else
                    {
                        // Các ô còn lại là nền, có thể đi qua, thêm vào danh sách ô trống
                        tile = theme.GetRandomGround();
                        m_BoardData[x, y].Passable = true;
                        m_EmptyCellsList.Add(new Vector2Int(x,y));
                    }

                    m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        
            // Xóa ô spawn của player khỏi danh sách ô trống
            m_EmptyCellsList.Remove(new Vector2Int(1, 1));
        
            // Thêm ô exit (cửa ra) và xóa khỏi ô trống
            var inst = Instantiate(theme.ExitCellPrefab);
            Vector2Int endCoord = new Vector2Int(m_Width - 2, m_Height - 2);
            AddObject(inst, endCoord);
            m_EmptyCellsList.Remove(endCoord);
        
            GenerateWall();    // Sinh tường ngẫu nhiên trong map
            GenerateObject();  // Sinh vật phẩm, vật thể
            GenerateEnemies(); // Sinh quái vật
        
            UpdateCameraConfiner(); // Cập nhật vùng giới hạn camera
        }

        // Xóa sạch dữ liệu bản đồ (dùng khi load map mới)
        public void Clean()
        {
            if (m_BoardData == null) return;
        
            int width = m_CurrentLevelData.Width;
            int height = m_CurrentLevelData.Height;
        
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var cellData = m_BoardData[x, y];
                    cellData.ClearObjects();
                }
            }
        }

        // Sinh vật phẩm (item, vật thể) ngẫu nhiên trên map
        void GenerateObject()
        {
            int itemCount = GameManager.Instance.WorldSettings.Items.GetRandomItemCount(GameManager.Instance.CurrentLevel);
            GameManager.Instance.WorldSettings.Items.GetRandomItemList(ref m_CurrentAvailableObjectList, GameManager.Instance.CurrentLevel);
        
            for (int i = 0; i < itemCount; ++i)
            {
                int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
                Vector2Int coord = m_EmptyCellsList[randomIndex];
            
                m_EmptyCellsList.RemoveAt(randomIndex);
            
                var newObject = Instantiate(m_CurrentAvailableObjectList[Random.Range(0, m_CurrentAvailableObjectList.Count)]);
                AddObject(newObject, coord);
            }
        }

        // Sinh tường ngẫu nhiên trong map (không phải viền)
        void GenerateWall()
        {
            int wallCount = Random.Range(m_CurrentLevelData.LowestWallCount, m_CurrentLevelData.HighestWallCount+1);

            for (int i = 0; i < wallCount; ++i)
            {
                int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
                Vector2Int coord = m_EmptyCellsList[randomIndex];
            
                m_EmptyCellsList.RemoveAt(randomIndex);
            
                WallObject newWall = Instantiate(GameManager.Instance.WorldSettings.Theme.GetRandomWall());
                AddObject(newWall, coord);
            }
        }

        // Sinh quái vật ngẫu nhiên trên map
        void GenerateEnemies()
        {
            int enemyCount = GameManager.Instance.WorldSettings.Enemies.GetRandomEnemyCount(GameManager.Instance.CurrentLevel);
            GameManager.Instance.WorldSettings.Enemies.GetRandomEnemies(ref m_CurrentAvailableEnemyList, GameManager.Instance.CurrentLevel);
        
            for (int i = 0; i < enemyCount; ++i)
            {
                int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
                Vector2Int coord = m_EmptyCellsList[randomIndex];
            
                m_EmptyCellsList.RemoveAt(randomIndex);
            
                Enemy newEnemy = Instantiate(m_CurrentAvailableEnemyList[Random.Range(0, m_CurrentAvailableEnemyList.Count)]);
                AddObject(newEnemy, coord);
            }
        }

        // Cập nhật vùng giới hạn camera theo kích thước map
        void UpdateCameraConfiner()
        {
            Vector3 centerPos = CellToWorld(new Vector2Int(m_CurrentLevelData.Width / 2, m_CurrentLevelData.Height / 2));

            if (m_CurrentLevelData.Width % 2 == 0)
            {
                // Nếu chiều rộng chẵn thì dịch tâm sang trái nửa ô
                centerPos.x -= m_Tilemap.cellSize.x * 0.5f;
            }

            if (m_CurrentLevelData.Height % 2 == 0)
            {
                // Nếu chiều cao chẵn thì dịch tâm xuống dưới nửa ô
                centerPos.y -= m_Tilemap.cellSize.y * 0.5f;
            }

            CameraConfinerBound.transform.position = centerPos;
            CameraConfinerBound.size = new Vector2((m_CurrentLevelData.Width) * m_Tilemap.cellSize.x,
                (m_CurrentLevelData.Height) * m_Tilemap.cellSize.y);
        
            GameManager.Instance.CameraConfiner.InvalidateBoundingShapeCache();
        }

        // Thêm vật thể vào ô (và gọi Init cho vật thể)
        void AddObject(CellObject obj, Vector2Int coord)
        {
            CellData data = m_BoardData[coord.x, coord.y];
            obj.transform.position = CellToWorld(coord);
            data.AddObject(obj);
            obj.Init(coord);
        }

        // Chuyển vị trí ô sang vị trí thế giới
        public Vector3 CellToWorld(Vector2Int cellIndex)
        {
            return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
        }

        // Lấy dữ liệu ô tại vị trí chỉ định
        public CellData GetCellData(Vector2Int cellIndex)
        {
            if (cellIndex.x < 0 || cellIndex.x >= m_CurrentLevelData.Width
                                || cellIndex.y < 0 || cellIndex.y >= m_CurrentLevelData.Height)
            {
                return null;
            }
            return m_BoardData[cellIndex.x, cellIndex.y];
        }

        // Lấy tile tại vị trí ô
        public Tile GetCellTile(Vector2Int cellIndex)
        {
            return m_Tilemap.GetTile<Tile>((Vector3Int)cellIndex);
        }

        // Đặt tile tại vị trí ô
        public void SetCellTile(Vector2Int cellIndex, Tile tile)
        {
            m_Tilemap.SetTile((Vector3Int)cellIndex, tile);
        }
    
        // Lưu trạng thái bản đồ ra file (dùng cho save game)
        public void Save(BinaryWriter writer)
        {
            //save the board size
            writer.Write(m_Width);
            writer.Write(m_Height);

            for (int y = 0; y < m_Height; ++y)
            {
                for (int x = 0; x < m_Width; ++x)
                {
                    var cellData = m_BoardData[x, y];
                
                    //save the tile used there
                    writer.Write(m_Tilemap.GetTile<Tile>(new Vector3Int(x,y,0)).name);
                
                    //save the cell data
                    writer.Write(cellData.Passable);
                
                    //save all object
                    writer.Write(cellData.ContainedObjects.Count);
                    for (int i = 0; i < cellData.ContainedObjects.Count; ++i)
                    {
                        var obj = cellData.ContainedObjects[i];
                        //save ID used to lookup on database on reload
                        writer.Write(obj.ID);
                        //then call save on the object itself so it can save some internal data if it wants (e.g. enemy save health)
                        obj.Save(writer);
                    }
                }
            }
        }

        // Đọc trạng thái bản đồ từ file (dùng cho load game)
        public void Load(BinaryReader reader)
        {
            Clean();
        
            //load size
            m_Width = reader.ReadInt32();
            m_Height = reader.ReadInt32();

            m_BoardData = new CellData[m_Width, m_Height];
        
            //like generation we overshoot on both direction to place some visual ground outside the playable area
            for (int y = -20; y < m_Height + 20; ++y)
            {
                for (int x = -20; x < m_Width + 20; ++x)
                {
                    // Nếu ngoài vùng chơi, đặt tile nền ngẫu nhiên
                    if (x < 0 || y < 0 || x >= m_Width || y >= m_Height)
                    {
                        m_Tilemap.SetTile(new Vector3Int(x, y, 0), GameManager.Instance.WorldSettings.Theme.GetRandomGround());
                        continue;
                    }
                
                    // Đọc tên tile và lấy tile từ database
                    string tileId = reader.ReadString();
                    var tile = GameManager.Instance.ReferenceDatabase.GetTileFromInstanceID(tileId);
                    m_Tilemap.SetTile(new Vector3Int(x,y,0), tile);
                
                    var passable = reader.ReadBoolean();
                
                    // Khởi tạo dữ liệu ô
                    m_BoardData[x, y] = new CellData()
                    {
                        Passable = passable
                    };

                    // Thêm lại các vật thể và load trạng thái (ví dụ: máu quái vật)
                    int objectCount = reader.ReadInt32();
                    for (int i = 0; i < objectCount; ++i)
                    {
                        var objId = reader.ReadString();
                        var obj = Instantiate(GameManager.Instance.ReferenceDatabase.GetCellObjectFromID(objId));
                        AddObject(obj, new Vector2Int(x, y));
                        obj.Load(reader);
                    }
                }
            }
        
            // Lấy lại dữ liệu level hiện tại
            m_CurrentLevelData = GameManager.Instance.WorldSettings.GetLevelDataForLevel(GameManager.Instance.CurrentLevel);
            UpdateCameraConfiner();
        }
    }
}