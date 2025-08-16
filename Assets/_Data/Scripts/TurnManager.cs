using UnityEngine;

public class TurnManager
{
 private int turnCount;
 public event System.Action OnTick;

    public TurnManager()
    {
        this.turnCount = 1;
    }

    public void Tick()
    {
        
        this.turnCount += 1;
          this.OnTick?.Invoke();
        Debug.Log("Current turn count : " + this.turnCount);
    }
    
}
