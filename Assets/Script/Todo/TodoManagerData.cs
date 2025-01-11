using UnityEngine;

[CreateAssetMenu(fileName = "TodoManagerData", menuName = "ScriptableObjects/TodoManagerData", order = 1)]
[System.Serializable]
public class TodoManagerData : ScriptableObject
{
    public string todoGuid;

    // 用于在运行时生成新的 GUID
    public void GenerateNewGuid()
    {
        todoGuid = System.Guid.NewGuid().ToString();
    }
}
