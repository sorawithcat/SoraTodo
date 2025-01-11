using UnityEngine;
[CreateAssetMenu(fileName = "TodoManagerData", menuName = "ScriptableObjects/ClassifyButtonManagerData", order = 1)]
[System.Serializable]
public class ClassifyButtonManagerData : ScriptableObject
{


    public string classifyButtonManagerDataGuid;

    // 用于在运行时生成新的 GUID
    public void GenerateNewGuid()
    {
        classifyButtonManagerDataGuid = System.Guid.NewGuid().ToString();
    }

}
