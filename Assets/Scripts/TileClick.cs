using UnityEngine;

public class TileClick : MonoBehaviour
{
    private Vector2Int pos;
    private AStarGridView view;
    public Renderer r;

    public void Init(Vector2Int p, AStarGridView v)
    {
        pos = p;
        view = v;
    }

    public void OnClick()
    {
        Debug.Log(pos);
        view.SetStartEndPos(pos);
    }
}
