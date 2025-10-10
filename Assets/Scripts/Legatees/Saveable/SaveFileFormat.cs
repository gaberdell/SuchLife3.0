using UnityEngine;

//General Purpose SaveFileFormat designed to be translated read out then read back in
[System.Serializable]
public class SaveFileFormat
{
    public GameObject CopyAbleInstance; //Will contain some ISaveAble

    public int[] IntSaveValues; //Idk use for like states or whatever
    public float[] FloatSaveValues; //Used for like movement or something

    private const uint DEFAULT_ARRAY_SIZE = 9;

    public SaveFileFormat()
    {
        IntSaveValues = new int[DEFAULT_ARRAY_SIZE];
        FloatSaveValues = new float[DEFAULT_ARRAY_SIZE];
    }
}
