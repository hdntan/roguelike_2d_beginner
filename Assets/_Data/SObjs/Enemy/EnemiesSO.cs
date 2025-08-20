using System.Collections.Generic;
using UnityEngine;

// Tạo menu để tạo asset EnemiesSO trong Unity Editor
[CreateAssetMenu(fileName = "EnemiesSO", menuName = "ScriptableObjects/EnemiesSO")]
public class EnemiesSO : ScriptableObject
{
    // Mảng lưu thông tin số lượng kẻ địch xuất hiện ở từng level
    public EnemyCountEntry[] EnemyCount;

    // Mảng lưu thông tin cấu hình từng loại kẻ địch
    public EnemySettingEntry[] Enemies;

    // Hàm trả về số lượng kẻ địch ngẫu nhiên dựa trên level hiện tại
    public int GetRandomEnemyCount(int level)
    {
        // Nếu không có dữ liệu hoặc level nhỏ hơn level đầu tiên thì trả về 0
        if (EnemyCount.Length == 0 || EnemyCount[0].Level > level)
            return 0;

        // Tìm entry có level nhỏ nhất nhưng gần nhất với level truyền vào (hoặc đúng level nếu có)
        int entry = 0;
        while (entry < EnemyCount.Length - 1 && EnemyCount[entry + 1].Level < level)
        {
            entry++;
        }

        // Trả về số lượng ngẫu nhiên trong khoảng min-max của entry tìm được
        return Random.Range(EnemyCount[entry].Minimum, EnemyCount[entry].Maximum + 1);
    }

    // Hàm lấy danh sách kẻ địch ngẫu nhiên dựa trên xác suất từng loại ở level hiện tại
    public void GetRandomEnemies(ref List<Enemy> enemiesList, int level)
    {
        enemiesList.Clear(); // Xóa danh sách cũ

        // Duyệt qua từng loại kẻ địch
        foreach (var enemy in Enemies)
        {
            // Nếu không có xác suất hoặc level nhỏ hơn level đầu tiên thì bỏ qua
            if (enemy.Probabilities.Length == 0 || enemy.Probabilities[0].Level > level)
                continue;

            // Tìm entry xác suất phù hợp với level hiện tại
            int entry = 0;
            while (entry < enemy.Probabilities.Length - 1 && enemy.Probabilities[entry + 1].Level <= level)
                entry++;

            // Thêm kẻ địch vào danh sách theo trọng số (Weight)
            for (int i = 0; i < enemy.Probabilities[entry].Weight; ++i)
            {
                enemiesList.Add(enemy.Enemy);
            }
        }
    }
}

// Lưu thông tin số lượng kẻ địch cho từng level
[System.Serializable]
public class EnemyCountEntry
{
    public int Level;     // Level hiện tại
    public int Minimum;   // Số lượng kẻ địch tối thiểu
    public int Maximum;   // Số lượng kẻ địch tối đa
}

// Lưu thông tin cấu hình từng loại kẻ địch
[System.Serializable]
public class EnemySettingEntry
{
    // Lưu xác suất xuất hiện của kẻ địch theo từng level
    [System.Serializable]
    public class EnemyProbability
    {
        public int Level;   // Level áp dụng xác suất này
        public int Weight;  // Trọng số xác suất xuất hiện
    }

    public Enemy Enemy;                        // Tham chiếu đến prefab hoặc script của kẻ địch
    public EnemyProbability[] Probabilities;   // Mảng xác suất xuất hiện
}
