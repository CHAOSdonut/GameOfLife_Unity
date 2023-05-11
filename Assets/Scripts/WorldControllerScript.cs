using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldControllerScript : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private Transform baseLayer;
    [SerializeField] private Transform hoverLayer;
    [SerializeField] public static int boardSizeX = 32;
    [SerializeField] public static int boardSizeY = 18;
    private Vector3 lastMousePos;

    // Start is called before the first frame update
    void Start()
    {
        lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SpawnBaseGrid();
    }

    // Update is called once per frame
    void Update()
    {
        ShowHoveredTile();
    }

    public void SpawnTile(int x, int y, int z, Color color, Transform parent)
    {
        tile.GetComponent<SpriteRenderer>().color = color;
        Instantiate(tile, new Vector3Int(x,y,z), Quaternion.identity, parent);
    }

    void SpawnBaseGrid()
    {
        for (int y = 0; y < boardSizeY; y++)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                if ((x+y)%2 == 0)
                {
                    SpawnTile(x,y,1, new Color(255,255,255), baseLayer);
                }
                else
                {
                    SpawnTile(x,y,1, new Color(0.78f, 0.78f, 0.78f), baseLayer);
                }
            } 
        }
    }

    void ShowHoveredTile()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (worldPosition != lastMousePos)
        {
            foreach (Transform child in hoverLayer) {
                Destroy(child.gameObject);
            }
            if ((0 <= worldPosition.x && worldPosition.x < boardSizeX)&&(0 <= worldPosition.y && worldPosition.y < boardSizeY)){
                lastMousePos = worldPosition;
                SpawnTile(Mathf.FloorToInt(lastMousePos.x+0.5f),Mathf.FloorToInt(lastMousePos.y+0.5f),-3, new Color(0,0,0,.3f), hoverLayer);
            }
        }
    }
}
