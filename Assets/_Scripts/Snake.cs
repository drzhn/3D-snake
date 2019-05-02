using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Snake : MonoBehaviour
{
    public GameObject onlyHeadPrefab;
    public GameObject headPrefab;
    public GameObject bodyPrefab;
    public GameObject cornerPrefab;

    [NonSerialized] public LinkedList<Vector3Int> segments = new LinkedList<Vector3Int>();

    private GameField _field;

    public Color color;

    public void Init(GameField field, Vector3Int initCoord)
    {
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        _field = field;
        segments.AddFirst(initCoord);
        ReconstructBody();
    }

    private void Start()
    {
        InvokeRepeating("RequestPath", 1, 0.3f);
    }

    /// <summary>
    /// Запрашиваем новый путь
    /// </summary>
    void RequestPath()
    {
        _field.grid.RequestPath(segments.First.Value, _field.appleCoord, ref path);
        if (path == null)
        {
            _field.SnakeCantFindPath(this);
            return;
        }

        //print(path.Count);
        if (path.Count >= 2)
        {
            // Если путь найден, то мы ставим голову змейки по этому пути, а хвост удаляем
            segments.AddFirst(path[path.Count - 2].coords);
            if (path.Count == 2)
            {
                _field.SnakeEatsApple(this); // 
            }
            else
            {
                segments.RemoveLast();
            }
        }

        ReconstructBody();
    }

    List<GameObject> bodyParts = new List<GameObject>();

    /// <summary>
    /// Перестройка тела змейки
    /// </summary>
    void ReconstructBody()
    {
        // каждый раз, когда вызывается эта функция, мы удаляем предыдущее тело и заново строим из кусочков новое. 
        // это дорогая операция, которую в будущем можно будет заменить на Object Pooling
        // решил оставить как есть, тк к задаче это не относится
        bodyParts.ForEach(Destroy);
        bodyParts.Clear();
        if (segments.Count == 1)
        {
            var part = Instantiate(onlyHeadPrefab, segments.First.Value, Quaternion.identity);
            part.transform.SetParent(transform);
            bodyParts.Add(part);
        }
        else
        {
            LinkedListNode<Vector3Int> current = segments.First;
            while (current != null)
            {
                if (current == segments.First)
                {
                    var part = Instantiate(headPrefab, current.Value, Quaternion.identity);
                    part.transform.forward = current.Next.Value - current.Value;
                    part.transform.SetParent(transform);
                    bodyParts.Add(part);
                }

                if (current == segments.Last)
                {
                    var part = Instantiate(headPrefab, current.Value, Quaternion.identity);
                    part.transform.forward = current.Previous.Value - current.Value;
                    part.transform.SetParent(transform);
                    bodyParts.Add(part);
                }

                if (current.Next != null && current.Previous != null)
                {
                    if (Vector3.Angle(current.Previous.Value - current.Value, current.Next.Value - current.Value) > 100)
                    {
                        var part = Instantiate(bodyPrefab, current.Value, Quaternion.identity);
                        part.transform.forward = current.Previous.Value - current.Next.Value;
                        part.transform.SetParent(transform);
                        bodyParts.Add(part);
                    }
                    else
                    {
                        var part = Instantiate(cornerPrefab, current.Value, Quaternion.identity);
                        part.transform.rotation = Quaternion.LookRotation(
                            current.Value - current.Next.Value,
                            -Vector3.Cross(current.Previous.Value - current.Value, current.Next.Value - current.Value)
                        );

                        part.transform.SetParent(transform);
                        bodyParts.Add(part);
                    }
                }
                current = current.Next;
            }
        }

        foreach (var part in bodyParts)
        {
            part.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private List<Node> path = new List<Node>();

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        for (var i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i].coords, path[i + 1].coords);
        }
    }
}