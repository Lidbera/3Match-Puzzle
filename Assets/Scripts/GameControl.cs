using System;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public Coord tsize, size;
    public bool[] tmap = new bool[100];
    public bool[,] map = new bool[10, 10];

    public int blockcount;

    private BlockControl block;

    private void Start()
    {
        blockcount = Enum.GetNames(typeof(BlockKind)).Length - 2;

        int temp = 0;
        for (int j = size.y - 1; j >= 0; j--)
        {
            for (int i = 0; i < size.x; i++)
            {
                map[i, j] = tmap[temp];
                temp++;
            }
        }
        block = GetComponent<BlockControl>();
        block.GameSet(map);
    }
}

public enum ObjectList
{
    block
}

public enum PuzzleCommand
{
    clickOver
}

public enum BlockKind
{
    NULL,
    WALL,
    TEST1,
    TEST2,
    TEST3,
    TEST4
}

[System.Serializable]
public struct Coord
{
    public int x, y;

    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public bool IsJoin(Coord an)
    {
        if ((x.Equals(an.x) && Mathf.Abs(y - an.y).Equals(1)) ||
            (y.Equals(an.y) && Mathf.Abs(x - an.x).Equals(1)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}