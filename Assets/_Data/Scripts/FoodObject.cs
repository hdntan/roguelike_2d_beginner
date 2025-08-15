using UnityEngine;

public class FoodObject : CellObject
{
    public int amountGranted = 10;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        GameManager.Instance.ChangeFoodAmount(amountGranted);
    }
   
}
