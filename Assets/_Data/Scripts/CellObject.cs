using UnityEngine;
using System.IO;

public class CellObject : MonoBehaviour
{
       protected Vector2Int cell;

        [HideInInspector]
        [SerializeField]
        protected string id;

        public string ID => id;

        public virtual void Init(Vector2Int coord)
        {
            this.cell = coord;
        }

        public void RemoveFromBoard()
        {
            var data = GameManager.Instance.boardManager.GetCellData(this.cell);
            data.RemoveObject(this);
        }

        /// <summary>
        /// Override this to return true if the CellObject need to be Unique. Only 1 Unique Cell Object can exist at a time
        /// in a given cell. This help for example to stop Enemy and Wall to occupy the same Cell.
        /// </summary>
        /// <returns>True if this need to be Unqiue in cell, false otherwise</returns>
        public virtual bool IsUnique()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool PlayerWantToEnter()
        {
            return true;
        }
    
        //Called when the player enter the cell in which that object is
        public virtual void PlayerEntered()
        {
        
        }

        public void SetID()
        {
            var root = transform.root;
            this.id = root.gameObject.name;
        }

        public virtual void Save(BinaryWriter writer)
        {
        
        }

        public virtual void Load(BinaryReader reader)
        {
        
        }
}
