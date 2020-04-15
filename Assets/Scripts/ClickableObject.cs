using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public ObjectList obj;

    private Block block;
    private void Start()
    {
        switch (obj)
        {
            case ObjectList.block:
                block = GetComponent<Block>();
                break;
            default:
                block = null;
                break;
        }
    }

    public void Click(int click)
    {
        switch (obj)
        {
            case ObjectList.block:
                block.Click(click);
                break;
            default:
                break;
        }
    }
}
