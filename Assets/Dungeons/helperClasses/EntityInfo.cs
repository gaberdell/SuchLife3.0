using UnityEngine;

[System.Serializable]
public class EntityInfo 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string entityName;
    public int relativeX;
    public int relativeY;
    EntityInfo()
    {
        //default values; should never be seen
        entityName = "default";
        relativeX = 0;
        relativeY = 0;
    }
}
