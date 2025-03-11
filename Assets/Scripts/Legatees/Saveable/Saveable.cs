using NUnit.Framework;
using System.Collections.Generic;

public interface Saveable {
  public string Save();
  public void Load(string json);
  public void Load(List<string> jsons);
}