using System.Collections.Generic;
using UnityEngine;

// ScriptableObject dùng để lưu trữ cấu hình các loại item cho game
[CreateAssetMenu(fileName = "ItemsSO", menuName = "ScriptableObjects/ItemsSO")]
public class ItemsSO : ScriptableObject
{
    // Mảng cấu hình số lượng item xuất hiện theo từng level
    public ItemCountEntry[] ItemsCount;

    // Mảng cấu hình các loại item và xác suất xuất hiện của chúng theo từng level
    public ItemSettingEntry[] Items;

            public int GetRandomItemCount(int level)
        {
            if (ItemsCount.Length == 0 || ItemsCount[0].Level > level)
                return 0;
        
            int entry = 0;
            while (entry < ItemsCount.Length - 1 && ItemsCount[entry+1].Level < level)
            {
                entry++;
            }

            return Random.Range(ItemsCount[entry].Minimum, ItemsCount[entry].Maximum + 1);
        }

        /// <summary>
        /// Will return in the given list a list of objects with their number linked to their probability. Picking a random
        /// entry in this list will pick a random items which take the probabilities 
        /// </summary>
        /// <param name="objectsList">This List will contain a list of items, duplicated based on their probabailities</param>
        /// <param name="level">The level for which get a random items</param>
        public void GetRandomItemList(ref List<CellObject> objectsList, int level)
        {
            objectsList.Clear();
            foreach (var item in Items)
            {
                if(item.Probabilities.Length == 0 || item.Probabilities[0].Level > level)
                    continue;

                int entry = 0;
                while (entry < item.Probabilities.Length - 1 && item.Probabilities[entry+1].Level <= level)
                    entry++;
            
                for (int i = 0; i < item.Probabilities[entry].Weight; ++i)
                {
                    objectsList.Add(item.Item);
                }
            }
        }
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