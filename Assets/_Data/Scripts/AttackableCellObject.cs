using UnityEngine;

public abstract class AttackableCellObject : CellObject
{
    public override bool IsUnique()
    {
        return true; // Only one attackable object per cell
    }
    public abstract void Damaged(int amount);
    
}
