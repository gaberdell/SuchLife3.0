using NUnit.Framework;
using System.Collections.Generic;

public interface ISaveable {
    public SaveFileFormat Save(); //Returns save file format from file

    public SaveFileFormat Load()
  public void Load(string json); // for a single json line only, call multiple times if you have multiple json strings

}