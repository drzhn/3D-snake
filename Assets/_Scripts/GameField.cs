using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public int numSnakes = 3;
    public uint width = 15;
    public uint height = 15;
    public uint depth = 15;

    public GameObject pointPrefab;
    public GameObject applePrefab;
    public GameObject snakePrefab;

    public Grid grid;
    public Vector3Int appleCoord;
    private GameObject appleObj;
    public List<Snake> snakes = new List<Snake>();

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
//        foreach (var node in grid.NeighborOf(new Node(){coords = new Vector3Int(1, 1, 1)}))
//        {
//            print(node);
//        }
        //path = grid.AStar(new Vector3Int(1, 1, 1), new Vector3Int(4, 4, 4));
        //print(path.Count);
//        HashSet<Node> nodes = new HashSet<Node>()
//        {
//            new Node() {g = 0, h = 0},
//        };
//        nodes.Add(new Node() {g = 0, h = 0});
        //print(nodes.Count);
//        for (int i = 0; i < width; i++)
//        {
//            for (int j = 0; j < height; j++)
//            {
//                for (int k = 0; k < depth; k++)
//                {
//                    GameObject point = Instantiate(pointPrefab, new Vector3(i, j, k), Quaternion.identity);
//                    point.transform.SetParent(transform);
//                }
//            }
//        }
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