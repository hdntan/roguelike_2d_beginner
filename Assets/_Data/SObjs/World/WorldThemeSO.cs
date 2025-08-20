using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "WorldThemeSO", menuName = "ScriptableObjects/WorldThemeSO")]
public class WorldThemeSO : ScriptableObject
{
    public PlayerController playerPrefab;
    public Tile[] groundTiles;
    public Tile[] wallTiles;

    public WallObject[] wallPrefabs;

    public Tile GetRandomGround()
    {
        return groundTiles[Random.Range(0, groundTiles.Length)];
    }

    public Tile GetRandomBlocking()
    {
        return wallTiles[Random.Range(0, wallTiles.Length)];
    }

    public WallObject GetRandomWall()
    {
        return wallPrefabs[Random.Range(0, wallPrefabs.Length)];
    }
}
