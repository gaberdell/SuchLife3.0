using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
[System.Serializable]
public class Recipe : ScriptableObject
{
    public const int GridSize = 3;

    public int width = 3;
    public int height = 3;

    public List<Item> recipe_details; //If AnyOrder = true, don't add any nothing spaces. If it is false, do add nothing spaces ot designate shape.
    public Item result; 
    public int resultQuantity = 1;

    public bool anyOrder; //True if you can make this in any order. False if not. 
    public bool allowMirror = true;

    public Item emptyitem; //For ease of comparison. 

    private bool IsEmpty(Item i) { return i == null || i == emptyitem; }
    private bool Same(Item a, Item b) { return (IsEmpty(a) && IsEmpty(b)) || a == b; }

    public bool compareRecipes(List<Item> grid)
    {
        if (grid == null || grid.Count < GridSize * GridSize) return false;
        if (recipe_details == null || recipe_details.Count < width * height) return false;

        return anyOrder ? MatchShapeless(grid) : MatchShaped(grid);
    }

    private bool MatchShaped(List<Item> grid)
    {
        int gw, gh, rw, rh;
        List<Item> g = Trim(grid, GridSize, GridSize, out gw, out gh);
        List<Item> r = Trim(recipe_details, width, height, out rw, out rh);

        if (gw == 0 || gw != rw || gh != rh) return false;
        if (SameList(g, r)) return true;

        if (allowMirror)
        {
            List<Item> mirrored = new List<Item>();
            for (int y = 0; y < rh; y++)
                for (int x = rw - 1; x >= 0; x--)
                    mirrored.Add(r[y * rw + x]);
            return SameList(g, mirrored);
        }
        return false;
    }

    private bool MatchShapeless(List<Item> grid)
    {
        List<Item> needed = new List<Item>();
        for (int i = 0; i < width * height; i++)
            if (!IsEmpty(recipe_details[i])) needed.Add(recipe_details[i]);

        int placed = 0;
        for (int i = 0; i < GridSize * GridSize; i++)
        {
            if (IsEmpty(grid[i])) continue;
            placed++;
            if (!needed.Remove(grid[i])) return false; //item the recipe doesn't want (or too many of it)
        }
        return placed > 0 && needed.Count == 0;
    }

    //Cuts a w x h item list down to the bounding box of its non-empty entries.
    private List<Item> Trim(List<Item> items, int w, int h, out int outW, out int outH)
    {
        int minX = w, maxX = -1, minY = h, maxY = -1;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                if (!IsEmpty(items[y * w + x]))
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }

        List<Item> trimmed = new List<Item>();
        if (maxX < 0) { outW = 0; outH = 0; return trimmed; } //nothing in it

        outW = maxX - minX + 1;
        outH = maxY - minY + 1;
        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
                trimmed.Add(items[y * w + x]);
        return trimmed;
    }

    private bool SameList(List<Item> a, List<Item> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
            if (!Same(a[i], b[i])) return false;
        return true;
    }
}