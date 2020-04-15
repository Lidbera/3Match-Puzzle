using UnityEngine;

public class Block : MonoBehaviour
{
    public Coord coord;
    public bool fall = false;

    public BlockKind kind;
    private SpriteRenderer sprite;
    private Material mat;
    private BlockControl control;

    private float blockSpeed, blockinterval;

    public BlockKind Kind
    {
        get { return kind; }
        set {
            kind = value;
            switch (kind)
            {
                case BlockKind.WALL:
                    sprite.color = Color.black;
                    break;
                case BlockKind.TEST1:
                    sprite.color = Color.yellow;
                    break;
                case BlockKind.TEST2:
                    sprite.color = Color.green;
                    break;
                case BlockKind.TEST3:
                    sprite.color = Color.blue;
                    break;
                case BlockKind.TEST4:
                    sprite.color = Color.magenta;
                    break;
            }
        }
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        control = FindObjectOfType<BlockControl>();
    }

    private void Start()
    {
        mat = sprite.material;
        blockSpeed = control.blockFallSpeed;
        blockinterval = control.blockinterval;
    }

    public void BlockSet(BlockKind _kind, Coord _coord)
    {
        Kind = _kind;
        coord = _coord;
        transform.position = new Vector2(coord.x, coord.y);
    }

    public void Remove()
    {
        Kind = BlockKind.NULL;
        coord = new Coord(-1, -1);
        gameObject.SetActive(false);
    }

    public void Click(int click)
    {
        if (!control.clickable || kind.Equals(BlockKind.WALL)) return;

        if (click.Equals(1))
        {
            mat.color = new Color(1.5f, 1.5f, 1.5f);
            control.CallByBlock(1, this);
        }
        else if (click.Equals(2))
        {
            if (control.first == null) return;

            if (control.first.coord.x >= 0 && control.first.coord.y >= 0)
            {
                if (control.first.coord.IsJoin(coord))
                {
                    control.CallByBlock(2, this);
                }
            }
        }
    }

    public void ClickOver()
    {
        mat.color = Color.white;
    }

    public void FallCheck(int mlr)
    {
        Coord dest = new Coord();

        if(coord.x.Equals(-1) || coord.y <= 0) return;
        bool flag = false;
        if (mlr.Equals(0))
        {
            if (control.IfBatchedBlockNull(coord.x, coord.y - 1))
            {
                //아래로
                flag = true;
                dest = new Coord(coord.x, coord.y - 1);
            }
        }
        else if(control.batchedwalls[coord.x, coord.y - 1] && !kind.Equals(BlockKind.WALL))
        {
            if (mlr.Equals(1) && coord.x > 0)
            {
                if (control.IfBatchedBlockNull(coord.x - 1, coord.y - 1))
                {
                    //왼쪽 아래로
                    flag = true;
                    dest = new Coord(coord.x - 1, coord.y - 1);
                }
            }
            else if (mlr.Equals(2) && coord.x < control.GetBatchedBlockLength(0) - 1)
            {
                if (control.IfBatchedBlockNull(coord.x + 1, coord.y - 1))
                {
                    //오른쪽 아래로
                    flag = true;
                    dest = new Coord(coord.x + 1, coord.y - 1);
                }
            }
        }

        if (flag)
        {
            control.batchedblocks[coord.x, coord.y] = null;
            control.batchedblocks[dest.x, dest.y] = this;
            coord = dest;
            fall = true;
        }
    }

    private void Update()
    {
        if (fall)
        {
            Vector2 pos = new Vector2(coord.x, coord.y);
            transform.position = Vector2.Lerp(transform.position, pos, blockSpeed * Time.fixedDeltaTime);

            Vector2 offset = (Vector2)transform.position - pos;
            if (Vector2.SqrMagnitude(offset) < blockinterval)
            {
                transform.position = pos;
                fall = false;
            }
        }
    }
}
