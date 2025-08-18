using System.Collections.Generic;
using UnityEngine;

// Lớp lưu trữ dữ liệu cho một ô trên bản đồ
public class CellData
{
    public bool Passable; // Ô này có thể đi qua không (true: đi qua được, false: là tường hoặc vật cản)

    public List<CellObject> containedObjects = new(); // Danh sách các vật thể trong ô này (item, enemy, ...)
    public CellObject uniqueCellObject = null;        // Vật thể duy nhất trong ô (ví dụ: tường, enemy), chỉ cho phép 1 object loại này

    // Kiểm tra xem player có được phép đi vào ô này không (dựa vào tất cả object trong ô)
    public bool PlayerWantToEnter()
    {
        foreach (var cellObject in containedObjects)
        {
            // Nếu có object nào không cho vào thì trả về false
            if (!cellObject.PlayerWantToEnter())
                return false;
        }
        return true; // Nếu tất cả đều cho vào thì trả về true
    }

    // Gọi hàm PlayerEntered() của tất cả object trong ô khi player bước vào
    public void PlayerEntered()
    {
        foreach (var cellObject in this.containedObjects)
        {

            cellObject.PlayerEntered();

        }
    }

    // Ô này có vật thể nào không?
    public bool HasObjects() => this.containedObjects.Count > 0;

    // Thêm object vào ô, kiểm tra nếu là object duy nhất (unique)
    public void AddObject(CellObject obj)
    {
        if (obj.IsUnique())
        {
            if (this.uniqueCellObject != null)
            {
                // Nếu đã có object unique thì báo lỗi (không cho phép 2 unique object cùng ô)
                Debug.LogError($"Tried to the unique cell object {obj.name} to a cell already containing one {this.uniqueCellObject.name}");
                return;
            }
            this.uniqueCellObject = obj;
        }
        this.containedObjects.Add(obj);
    }

    // Kiểm tra ô này có object nào có thể bị tấn công không (ví dụ: enemy, wall)
    public bool HaveAttackable(out AttackableCellObject attackable)
    {
        if (this.uniqueCellObject != null && this.uniqueCellObject is AttackableCellObject obj)
        {
            attackable = obj;
            return true;
        }
        attackable = null;
        return false;
    }

    // Xóa object khỏi ô (nếu là unique thì xóa luôn uniqueCellObject)
    public void RemoveObject(CellObject obj)
    {
        if (obj == this.uniqueCellObject)
            this.uniqueCellObject = null;

        this.containedObjects.Remove(obj);
    }

    // Xóa tất cả object trong ô (dùng khi reset map)
    // public void ClearObjects()
    // {
    //     foreach (var cellObject in this.containedObjects)
    //     {
    //         Destroy(cellObject.gameObject);
    //     }
    //     this.containedObjects.Clear();
    // }
}

