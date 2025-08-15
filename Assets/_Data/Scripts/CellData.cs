using System.Collections.Generic;
using UnityEngine;

public class CellData 
{
    public bool Passable; // Ô có thể đi qua

    public List<CellObject> containedObjects = new();
    public CellObject uniqueCellObject = null;

    public bool PlayerWantToEnter()
    {
        foreach (var cellObject in containedObjects)
        {
            if (!cellObject.PlayerWantToEnter())
                return false;
        }

        return true;
    }

    public void PlayerEntered()
    {
        foreach (var cellObject in this.containedObjects)
        {
            cellObject.PlayerEntered();
        }
    }

    public bool HasObjects() => this.containedObjects.Count > 0;

    public void AddObject(CellObject obj)
    {
        if (obj.IsUnique())
        {
            if (this.uniqueCellObject != null)
            {
                //we tried to add a unique cell object to a cell already containing one, this shouldn't happen! Error out
                Debug.LogError($"Tried to the unique cell object {obj.name} to a cell already containing one {this.uniqueCellObject.name}");
                return;
            }

            this.uniqueCellObject = obj;
        }

        this.containedObjects.Add(obj);
    }

    // public bool HaveAttackable(out AttackableCellObject attackable)
    // {
    //     if (UniqueCellObject != null && UniqueCellObject is AttackableCellObject obj)
    //     {
    //         attackable = obj;
    //         return true;
    //     }

    //     attackable = null;
    //     return false;
    // }

    public void RemoveObject(CellObject obj)
    {
        if (obj == this.uniqueCellObject)
            this.uniqueCellObject = null;

        this.containedObjects.Remove(obj);
    }

    // public void ClearObjects()
    // {
    //     foreach (var cellObject in this.containedObjects)
    //     {
    //         Destroy(cellObject.gameObject);
    //     }
    //     this.containedObjects.Clear();
    // }
}

