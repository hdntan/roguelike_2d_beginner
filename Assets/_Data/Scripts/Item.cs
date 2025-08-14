using UnityEngine;

public class Item : MonoBehaviour, ICellObject
{
    public void PlayerEntered()
    {
        Debug.Log("Player nhặt vật phẩm!");
        // Xử lý logic nhặt vật phẩm ở đây
        Destroy(gameObject); // Xóa vật phẩm khỏi scene
    }
}