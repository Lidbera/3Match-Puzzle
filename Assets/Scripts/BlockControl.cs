using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public GameObject block, explosion;
    public Transform puzzle, particle;
    public float blockChangeSpeed = 3f;
    public float blockFallSpeed = 10f;
    public float blockinterval = 0.001f;
    public float blockFallDelay = 0.5f;

    [HideInInspector]
    public bool clickable;
    [HideInInspector]
    public Block first, second, lastclick;

    public Block[,] batchedblocks;
    public bool[,] batchedwalls;

    private GameControl game;
    private bool start, changing, falling, exploding;

    private Block[] blockpool;
    private Queue<GameObject> explosionpool;
    private int currentpool, x, y;

    private void Start()
    {
        clickable = true;
        game = GetComponent<GameControl>();

        currentpool = 0;
        blockpool = new Block[150];
        explosionpool = new Queue<GameObject>();
        for (int i = 0; i < 150; i++)
        {
            Block temp = Instantiate(block, new Vector2(0, 0),
                    Quaternion.identity, puzzle).GetComponent<Block>();
            blockpool[i] = temp;
            temp.Remove();
            temp.gameObject.SetActive(false);

            GameObject e = Instantiate(explosion, Vector2.zero, Quaternion.identity, particle);
            e.SetActive(false);
            explosionpool.Enqueue(e);
        }

        batchedblocks = new Block[game.size.x, game.size.y];
        x = batchedblocks.GetLength(0);
        y = batchedblocks.GetLength(1);
    }

    public void GameSet(bool[,] map)
    {
        //게임시작
        start = true;
        batchedwalls = map;
        Batch();

        clickable = false;
        StartCoroutine(ExplodeBlocks(null));
    }

    public bool IfBatchedBlockNull(int x, int y)
    {
        if (batchedblocks[x, y] == null) return true;
        return false;
    }
    public int GetBatchedBlockLength(int x)
    {
        return batchedblocks.GetLength(x);
    }

    public GameObject GetByPoolExplosion(Vector2 pos)
    {
        GameObject e = explosionpool.Dequeue();
        e.transform.position = new Vector3(pos.x, pos.y, -1);
        e.SetActive(true);
        explosionpool.Enqueue(e);
        return e;
    }

    public Block GetByPool(int x, int y, bool wall)
    {
        //오브젝트풀에서 초기화시켜서 가져오기
        if (currentpool >= 150) currentpool = 0;
        while (blockpool[currentpool].gameObject.activeSelf)
        {
            if (currentpool < 150) currentpool++;
            else currentpool = 0;
        }
        Block temp = blockpool[currentpool];

        if (batchedblocks[x, y] == null ||
                   !batchedblocks[x, y].gameObject.activeSelf)
        {
            batchedblocks[x, y] = temp;
        }
        else return null;

        if (currentpool < 150) currentpool++;
        else currentpool = 0;
        temp.gameObject.SetActive(true);

        if (wall) temp.BlockSet(BlockKind.WALL, new Coord(x, y));
        else temp.BlockSet((BlockKind)Random.Range(2, 2 + game.blockcount), new Coord(x, y));

        return temp;
    }

    public void Batch()
    {
        //첫 배치
        for (int i = 0; i < game.size.x; i++)
        {
            for (int j = 0; j < game.size.y; j++)
            {
                if (batchedwalls[i, j])
                {
                    if(GetByPool(i, j, true) == null)
                    {
                        Debug.LogError("벽 생성 에러");
                    }
                }
                else if(GetByPool(i, j, false) == null)
                {
                    Debug.LogError("블럭 생성 에러");
                }
            }
        }
    }

    public void CallByBlock(int order, Block block)
    {
        //블럭 클릭
        if (!clickable) return;
        if (1 < order)
        {
            if (first.Equals(block)) return;
            else clickable = false;
        }

        if (order.Equals(1))
        {
            first = block;
            lastclick = block;
        }
        else
        {
            second = block;
            StartCoroutine(ChangeBlocks());
        }
    }

    public void ClickOver()
    {
        if (lastclick != null)
        {
            lastclick.ClickOver();
            lastclick = null;
        }
    }

    private List<Block> ExplodeTrigger(Coord[] coord)
    {
        //폭발 조건
        List<Block> explodelist = new List<Block>();

        int[] mxcount = new int[coord.Length], pxcount = new int[coord.Length];
        int[] mycount = new int[coord.Length], pycount = new int[coord.Length];

        for(int i = 0; i < coord.Length; i++)
        {
            if (batchedblocks[coord[i].x, coord[i].y] == null) continue;
            BlockKind kind = batchedblocks[coord[i].x, coord[i].y].Kind;

            int mx = coord[i].x - 1;
            while (mx >= 0 && mx < game.size.x)
            {
                if (batchedblocks[mx, coord[i].y] == null) break;
                if (batchedblocks[mx, coord[i].y].Kind.Equals(kind)
                    && !kind.Equals(BlockKind.WALL))
                {
                    mx--;
                    mxcount[i]++;
                }
                else break;
            }

            int px = coord[i].x + 1;
            while (px >= 0 && px < game.size.x)
            {
                if (batchedblocks[px, coord[i].y] == null) break;
                if (batchedblocks[px, coord[i].y].Kind.Equals(kind)
                    && !kind.Equals(BlockKind.WALL))
                {
                    px++;
                    pxcount[i]++;
                }
                else break;
            }

            int my = coord[i].y - 1;
            while (my >= 0 && my < game.size.y)
            {
                if (batchedblocks[coord[i].x, my] == null) break;
                if (batchedblocks[coord[i].x, my].Kind.Equals(kind)
                    && !kind.Equals(BlockKind.WALL))
                {
                    my--;
                    mycount[i]++;
                }
                else break;
            }

            int py = coord[i].y + 1;
            while (py >= 0 && py < game.size.y)
            {
                if (batchedblocks[coord[i].x, py] == null) break;
                if (batchedblocks[coord[i].x, py].Kind.Equals(kind)
                    && !kind.Equals(BlockKind.WALL))
                {
                    py++;
                    pycount[i]++;
                }
                else break;
            }
        }

        for (int i = 0; i < coord.Length; i++)
        {
            if (mxcount[i] + 1 + pxcount[i] >= 3)
            {
                ExplodeListAdd(coord[i], 0, 0, ref explodelist);
                for (int j = 1; j <= mxcount[i]; j++)
                {
                    ExplodeListAdd(coord[i], -j, 0, ref explodelist);
                }
                for (int j = 1; j <= pxcount[i]; j++)
                {
                    ExplodeListAdd(coord[i], j, 0, ref explodelist);
                }
            }
            if (mycount[i] + 1 + pycount[i] >= 3)
            {
                ExplodeListAdd(coord[i], 0, 0, ref explodelist);
                for (int j = 1; j <= mycount[i]; j++)
                {
                    ExplodeListAdd(coord[i], 0, -j, ref explodelist);
                }
                for (int j = 1; j <= pycount[i]; j++)
                {
                    ExplodeListAdd(coord[i], 0, j, ref explodelist);
                }
            }
        }

        if (explodelist.Count > 0) return explodelist;
        else return null;
    }

    private void ExplodeListAdd(Coord coord, int addx, int addy, ref List<Block> explodelist)
    {
        //조건 만족하는 블럭들 리스트에 추가
        Block add = batchedblocks[coord.x + addx, coord.y + addy];
        foreach (Block b in explodelist)
        {
            if (b.Kind.Equals(BlockKind.WALL)) add = null;
            else if (b.Equals(add)) add = null;
        }
        if (add != null) explodelist.Add(add);
    }

    private void ChangeBlockPos()
    {
        //블럭 위치 변경
        Coord fc = first.coord;
        Coord sc = second.coord;

        //배치
        batchedblocks[fc.x, fc.y] = second;
        batchedblocks[sc.x, sc.y] = first;

        //블럭 내 좌표
        first.coord = sc;
        second.coord = fc;
    }

    private IEnumerator ChangeBlocks()
    {
        //블럭 위치 변경 시도
        changing = true;
        Vector2 firstpos = first.transform.position;
        Vector2 secondpos = second.transform.position;
        bool explode = false;

        while (true)
        {
            first.transform.position = Vector2.Lerp(first.transform.position, secondpos, blockChangeSpeed * Time.fixedDeltaTime);
            second.transform.position = Vector2.Lerp(second.transform.position, firstpos, blockChangeSpeed * Time.fixedDeltaTime);

            Vector2 offset = (Vector2)first.transform.position - secondpos;
            if (Vector2.SqrMagnitude(offset) < blockinterval)
            {
                first.transform.position = secondpos;
                second.transform.position = firstpos;
                break;
            }

            yield return null;
        }

        ChangeBlockPos();
        
        List<Block> blocks = ExplodeTrigger(new Coord[] { first.coord, second.coord });
        if (blocks != null)
        {
            explode = true;
            StartCoroutine(ExplodeBlocks(blocks));
        }
        else
        {
            ChangeBlockPos();

            while (true)
            {
                first.transform.position = Vector2.Lerp(first.transform.position, firstpos, blockChangeSpeed * Time.fixedDeltaTime);
                second.transform.position = Vector2.Lerp(second.transform.position, secondpos, blockChangeSpeed * Time.fixedDeltaTime);

                Vector2 offset = (Vector2)first.transform.position - firstpos;
                if (Vector2.SqrMagnitude(offset) < blockinterval)
                {
                    first.transform.position = firstpos;
                    second.transform.position = secondpos;
                    break;
                }

                yield return null;
            }
        }

        if (!explode)
        {
            ChangeEnd();
        }

        first = null;
        second = null;
        changing = false;
    }

    private void ChangeEnd()
    {
        ClickOver();
        clickable = true;
    }

    //이거
    public IEnumerator FallBlocks()
    {
        falling = true;

        int count, maybe = 0;
        int[] xcount;
        while (true)
        {
            while (exploding) yield return null;

            if (maybe > 100) break;

            for(int check = 0; check < 3; check++)
            {
                for (int j = 0; j < y; j++)
                {
                    for (int i = 0; i < x; i++)
                    {
                        if (batchedblocks[i, j] != null)
                        {
                            batchedblocks[i, j].FallCheck(check);
                        }
                    }
                }
            }

            count = 0;
            xcount = new int[x];
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    if (batchedblocks[i, j] == null)
                    {
                        xcount[i]++;
                        count++;
                    }
                }
            }

            for (int i = 0; i < x; i++)
            {
                if (xcount[i] > 0)
                {
                    int y2 = y - 1;
                    if (batchedblocks[i, y2] == null)
                    {
                        Block newblock = GetByPool(i, y2, false);
                        newblock.transform.position = new Vector2(i, y2 + 1);
                        newblock.fall = true;
                    }
                }
            }

            StartCoroutine(ExplodeBlocks(null));

            if (count.Equals(0)) break;

            yield return new WaitForSeconds(blockFallDelay);
        }

        falling = false;
        ChangeEnd();
    }

    private IEnumerator ExplodeBlocks(List<Block> blocks)
    {
        exploding = true;

        if (blocks != null)
        {
            while (!changing)
            {
                yield return null;
            }
        }

        bool flag = true;
        while (flag)
        {
            if(blocks != null)
            {
                //터짐 테스트
                foreach (Block b in blocks)
                {
                    if (b == null) continue;
                    RemoveBlock(b);
                }

                if (!falling) StartCoroutine(FallBlocks());
            }

            blocks = ExplodeAllBlocks();
            if(blocks == null) flag = false;
        }

        exploding = false;
    }

    private List<Block> ExplodeAllBlocks()
    {
        Coord[] all = new Coord[x * y];
        int temp = 0;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (batchedblocks[i, j] != null)
                {
                    all[temp] = batchedblocks[i, j].coord;
                    temp++;
                }
            }
        }
        List<Block> after = ExplodeTrigger(all);
        return after;
    }

    private void RemoveBlock(Block b)
    {
        GetByPoolExplosion(new Vector2(b.coord.x, b.coord.y));
        batchedblocks[b.coord.x, b.coord.y] = null;
        b.Remove();
    }
}
