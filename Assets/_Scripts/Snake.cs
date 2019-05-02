using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public GameObject onlyHeadPrefab;
    public GameObject headPrefab;
    public GameObject bodyPrefab;
    public GameObject cornerPrefab;

    public List<Vector3Int> segments = new List<Vector3Int>();

    private GameField _field;

    private Color color;

    public void Init(GameField field, Vector3Int initCoord)
    {
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        _field = field;
        segments.Add(initCoord);
        ReconstructBody();
    }

    private void Start()
    {
        InvokeRepeating("RequestPath", 1, 0.3f);
    }

    void RequestPath()
    {
        _field.grid.RequestPath(segments[0], _field.appleCoord, ref path);
        if (path == null)
        {
            _field.SnakeCantFindPath(this);
            return;
        }

        //print(path.Count);
        if (path.Count >= 2)
        {
            segments.Insert(0, path[path.Count - 2].coords);
            if (path.Count == 2)
            {
                _field.SnakeEatsApple(this);
            }
            else
            {
                segments.RemoveAt(segments.Count - 1);
            }
        }

        ReconstructBody();
    }

    List<GameObject> bodyParts = new List<GameObject>();

    void ReconstructBody()
    {
        bodyParts.ForEach(Destroy);
        bodyParts.Clear();
        if (segments.Count == 1)
        {
            var part = Instantiate(onlyHeadPrefab, segments[0], Quaternion.identity);
            part.transform.SetParent(transform);
            bodyParts.Add(part);
        }
        else
        {
            for (var i = 0; i < segments.Count; i++)
            {
                if (i == 0)
                {
                    var part = Instantiate(headPrefab, segments[i], Quaternion.identity);
                    part.transform.forward = segments[1] - segments[0];
                    part.transform.SetParent(transform);
                    bodyParts.Add(part);
                    continue;
                }

                if (i == segments.Count - 1)
                {
                    var part = Instantiate(headPrefab, segments[i], Quaternion.identity);
                    part.transform.forward = segments[segments.Count - 2] - segments[segments.Count - 1];
                    part.transform.SetParent(transform);
                    bodyParts.Add(part);
                    continue;
                }

                if (segments.Count <= 2) continue;
                if (Vector3.Angle(segments[i - 1] - segments[i], segments[i + 1] - segments[i]) > 100)
                {
                    var part = Instantiate(bodyPrefab, segments[i], Quaternion.identity);
                    part.transform.forward = segments[i - 1] - segments[i + 1];
                    part.transform.SetParent(transform);
                    bodyParts.Add(part);
                    continue;
                }
                else
                {
                    var part = Instantiate(cornerPrefab, segments[i], Quaternion.identity);
                    part.transform.rotation = Quaternion.LookRotation(
                        segments[i] - segments[i + 1],
                        -Vector3.Cross(segments[i - 1] - segments[i], segments[i + 1] - segments[i])
                    );

                    part.transform.SetParent(transform);
                    bodyParts.Add(part);
                }
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