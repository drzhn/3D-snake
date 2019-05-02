using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public int numSnakes = 3;
    public uint width = 15;
    public uint height = 15;
    public uint depth = 15;

    public GameObject applePrefab;
    public GameObject snakePrefab;

    [NonSerialized]
    public Grid grid;
    [NonSerialized]
    public Vector3Int appleCoord;
    private GameObject appleObj;
    private List<Snake> snakes = new List<Snake>();

    void Awake()
    {
        for (int i = 0; i < numSnakes; i++)
        {
            var snake = Instantiate(snakePrefab, Vector3.zero, Quaternion.identity).GetComponent<Snake>();
            snakes.Add(snake);
            snake.Init(this, new Vector3Int(
                (int) UnityEngine.Random.Range(0, width - 1),
                (int) UnityEngine.Random.Range(0, height - 1),
                (int) UnityEngine.Random.Range(0, depth - 1)));
        }

        grid = new Grid(width, height, depth, snakes);
        appleCoord = grid.RequestNewAppleCoord();
        appleObj = Instantiate(applePrefab, appleCoord, Quaternion.identity);
    }

    public void SnakeEatsApple(Snake s)
    {
        appleCoord = grid.RequestNewAppleCoord();
        Destroy(appleObj);
        appleObj = Instantiate(applePrefab, appleCoord, Quaternion.identity);
    }

    public void SnakeCantFindPath(Snake s)
    {
        Debug.Log("Snake destroy");
        snakes.Remove(s);
        Destroy(s.gameObject);
    }
}