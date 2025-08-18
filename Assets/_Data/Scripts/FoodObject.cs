using UnityEngine;

public class FoodObject : CellObject
{
    public int amountGranted = 10;
    public override void PlayerEntered()
    {
        GameManager.Instance.ChangeFoodAmount(amountGranted);

        Destroy(gameObject);
    }

      private void OnDestroy()
        {
            RemoveFromBoard();
        }

   
}
