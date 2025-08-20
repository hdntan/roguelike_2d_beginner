using UnityEngine;

// Tạo một ScriptableObject mới với tên "WorldSO" trong menu "ScriptableObjects/WorldSO"
[CreateAssetMenu(fileName = "WorldSO", menuName = "ScriptableObjects/WorldSO")]
public class WorldSO : ScriptableObject
{
    // Tên của thế giới (world)
    public string worldName;

    // Tham chiếu đến ScriptableObject chứa thông tin về các vật phẩm (items)
    public ItemsSO items;

    public WorldThemeSO theme;

    // Tham chiếu đến ScriptableObject chứa thông tin về các kẻ địch (enemies)
    public EnemiesSO enemies;

    // Mảng chứa dữ liệu các level trong thế giới này
    public LevelData[] levelDatas;

    // Hàm trả về dữ liệu level phù hợp với level truyền vào
    public LevelData GetLevelDataForLevel(int level)
    {
        // Nếu không có dữ liệu level hoặc level nhỏ hơn level đầu tiên trong mảng
        if (levelDatas.Length == 0 || levelDatas[0].Level > level)
            // Trả về một LevelData mặc định với kích thước 8x8 và level 0
            return new LevelData()
            {
                Height = 8,
                Width = 8,
                Level = 0
            };

        int entry = 0;
        // Tìm entry phù hợp với level truyền vào (lấy entry lớn nhất mà Level <= level)
        while (entry < levelDatas.Length - 1 && levelDatas[entry + 1].Level <= level)
            entry++;

        // Trả về LevelData phù hợp
        return levelDatas[entry];
    }
}

// Định nghĩa class LevelData để lưu thông tin từng level
[System.Serializable]
public class LevelData
{
    // Chiều rộng của map level
    public int Width;
    // Chiều cao của map level
    public int Height;
    // Số lượng tường thấp nhất có thể sinh ra trong level
    public int LowestWallCount;
    // Số lượng tường cao nhất có thể sinh ra trong level
    public int HighestWallCount;
    // Số thứ tự level
    public int Level;
}
