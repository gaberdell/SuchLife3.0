using NUnit.Framework;
using System.Collections.Generic;

public interface Saveable {
  public string Save(); // for multiple json lines, return them separated by a '\n'
  public void Load(string json); // for a single json line only, call multiple times if you have multiple json strings
}