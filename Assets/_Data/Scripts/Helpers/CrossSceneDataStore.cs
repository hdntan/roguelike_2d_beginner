using UnityEngine;

public class CrossSceneDataStore : MonoBehaviour
{
    public static CrossSceneDataStore Instance { get; private set; }

    public WorldSO selectedWorldSettings = null;

    public static void CreateNewInstance()
    {
        var gameObject = new GameObject("CrossSceneDataStore");
        //gameObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(gameObject);

        Instance = gameObject.AddComponent<CrossSceneDataStore>();
    }
}
