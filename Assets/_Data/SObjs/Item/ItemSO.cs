using UnityEngine;

// ScriptableObject dùng để lưu trữ cấu hình các loại item cho game
[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    // Mảng cấu hình số lượng item xuất hiện theo từng level
    public ItemCountEntry[] ItemsCount;

    // Mảng cấu hình các loại item và xác suất xuất hiện của chúng theo từng level
    public ItemSettingEntry[] Items;
}

// Lưu thông tin số lượng item sẽ spawn ở mỗi level
[System.Serializable]
public class ItemCountEntry
{
    public int Level;    // Level tương ứng
    public int Minimum;  // Số lượng item tối thiểu sẽ spawn ở level này
    public int Maximum;  // Số lượng item tối đa sẽ spawn ở level này
}

// Lưu thông tin cấu hình từng loại item và xác suất xuất hiện theo level
[System.Serializable]
public class ItemSettingEntry
{
    // Lưu xác suất xuất hiện của item này ở từng level
    [System.Serializable]
    public class ItemProbability
    {
        public int Level;   // Level tương ứng
        public int Weight;  // Trọng số xác suất xuất hiện (càng lớn càng dễ xuất hiện)
    }

    public CellObject Item;                // Prefab hoặc ScriptableObject đại diện cho item này
    public ItemProbability[] Probabilities; // Mảng xác suất xuất hiện của item này theo từng level
}