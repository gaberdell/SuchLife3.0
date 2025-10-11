using NUnit.Framework;
using PlasticGui;
using System.Collections.Generic;

public interface ISaveable {
    public SaveFileFormat Save(); //Returns save file format from file

    public void Load(SaveFileFormat saveToLoad);

}