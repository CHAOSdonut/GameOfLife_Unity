using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private Transform gameLayer;
    [SerializeField] private Button button;
    [SerializeField] private float AdvanceCooldown = 1;
    private static int boardSizeX = WorldControllerScript.boardSizeX;
    private static int boardSizeY = WorldControllerScript.boardSizeY;
    private bool[,] gameBoard = new bool[boardSizeX, boardSizeY];
    private bool[,] newGameBoard = new bool[boardSizeX, boardSizeY];
    private WorldControllerScript worldcontroller;
    private List<List<int>> patern = new List<List<int>>();
    private bool autoAdvancing = false;
    private List<bool[,]> history = new List<bool[,]>();

    // Start is called before the first frame update
    void Start()
    {
        worldcontroller = FindObjectOfType<WorldControllerScript>();
        
        patern.Add(new List<int>(){-1,-1});
        patern.Add(new List<int>(){-1,0});
        patern.Add(new List<int>(){-1,1});
        patern.Add(new List<int>(){0,-1});
        patern.Add(new List<int>(){0,1});
        patern.Add(new List<int>(){1,-1});
        patern.Add(new List<int>(){1,0});
        patern.Add(new List<int>(){1,1});
    }

    // Update is called once per frame
    void Update()
    {
        SpawnOrKill();
    }

    void SpawnOrKill()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(worldPosition.x + 0.5f);
            int y = Mathf.FloorToInt(worldPosition.y + 0.5f);

            if ((0 <= x && x < boardSizeX)&&(0 <= y && y < boardSizeY))
            {
                gameBoard[x, y] = !gameBoard[x, y];
            
                DrawBoard();
            }
        }
    }
    
    void DrawBoard()
    {
        foreach (Transform child in gameLayer) {
            Destroy(child.gameObject);
        }
        for (int y = 0; y < boardSizeY; y++)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                if (gameBoard[x,y])
                {
                    worldcontroller.SpawnTile(x,y,-2, new Color(0,0,0), gameLayer);
                }
            } 
        }
    }

    public void NextGen()
    {
        newGameBoard = new bool[boardSizeX, boardSizeY];
        for (int y = 0; y < boardSizeY; y++)
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                newGameBoard[x, y] = CheckIfAlive(x, y);
            } 
        }

        AddToHistory(gameBoard);
        gameBoard = newGameBoard;
        DrawBoard();
    }

    bool CheckIfAlive(int x, int y)
    {
        int aliveNeighbours = 0;
        
        foreach (var pos in patern)
        {
            int myX = x + pos[0];
            int myY = y + pos[1];
            
            if ((0 <= myX && myX < boardSizeX) && (0 <= myY && myY < boardSizeY))
            {
                if (gameBoard[myX,myY])
                {
                    aliveNeighbours++; 
                }
            }
        }

        if (gameBoard[x,y])
        {
            if (aliveNeighbours is 2 or 3)
            {
                return true;
            }

            return false;
        }
        if (aliveNeighbours == 3)
        {
            return true;
        }

        return false;
    }

    public void ClearBoard()
    {
        gameBoard = new bool[boardSizeX, boardSizeY];
        DrawBoard();
    }

    public IEnumerator nextGenCoroutine()
    {
        while (autoAdvancing)
        {
            yield return new WaitForSeconds(AdvanceCooldown);
            NextGen();
        }
    }
    
    public void AutoAdvance()
    {
        autoAdvancing = !autoAdvancing;

        if (autoAdvancing)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.green;
            colors.highlightedColor = new Color32(100, 255, 100, 255);
            button.colors = colors;

            StartCoroutine(nextGenCoroutine());
        }
        else
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = new Color32(255, 100, 100, 255);
            button.colors = colors;
            
            StopAllCoroutines();
        }
    }

    public void ClearHistory()
    {
        history = new List<bool[,]>();
    }

    private void AddToHistory(bool[,] board)
    {
        history.Add(board);

        if (history.Count > 10)
        {
            history.RemoveAt(0);
        }
    }

    public void SelectHistoy(int num)
    {
        if (history.Count > num)
        {
            gameBoard = history[num];
            DrawBoard();
        }
        else
        {
            Debug.Log("History not yet set.");
        }
    }
}
