using UnityEngine;

public class TurnManager
{
 private int turnCount;

    public TurnManager()
    {
        this.turnCount = 1;
    }

    public void Tick()
    {
        this.turnCount += 1;
        Debug.Log("Current turn count : " + this.turnCount);
    }
    
}
