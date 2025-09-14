using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHelper : MonoBehaviour
{
    Tilemap tm;
    private void Start()
    {
        tm = GetComponent<Tilemap>();
    }

    public void SetOpacity(float opacity)
    {
        tm.color = Color.white - Color.black + Color.black * opacity;
    }
}
