using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GCost is 시작 노드에서 현재 노드까지의 거리 비용. HCost is 목표까지의 예상 비용(휴리스틱비용)
/// </summary>
public class Node
{
    public Vector3Int Position;   // 노드의 3D 위치
    public Node Parent;           // 부모 노드 (경로 추적용)
    public int GCost;             // 시작 노드에서 현재 노드까지의 거리 비용
    public int HCost;             // 휴리스틱 (목표까지의 예상 비용)

    public int FCost => GCost + HCost; // F = G + H (총 비용)

    public Node(Vector3Int position)
    {
        Position = position;
    }
}


public class TestAstarAlgorithm
{
    public Vector3Int StartPosition; // 시작 지점
    public Vector3Int TargetPosition; // 목표 지점

    private List<Node> openList = new List<Node>(); // 탐색할 노드 목록
    private List<Node> closedList = new List<Node>(); // 이미 탐색한 노드 목록

    private int[,,] grid; // 3D 그리드 맵 (0: 이동 가능, 1: 장애물)
    private Vector3Int gridSize;

    void Start()
    {
        //벡터스리인트.제로 에 들어갈것
        ShortestDistanceInitialize(Vector3Int.zero);

        //시작지점과 목표지점을 설정하고, 경로가 존재하면 해당 경로를 반환함.
        List<Vector3Int> path = FindPath(StartPosition, TargetPosition);

        // 경로 출력
        if (path != null)
        {
            foreach (var position in path)
            {
                Debug.Log($"Path Node: {position}");
            }
        }
        else
        {
            Debug.Log("경로를 찾을 수 없습니다.");
        }
    }

    private void ShortestDistanceInitialize(Vector3Int vector3Int)
    {
        //그리드사이즈에는 맵의 정보를 넣어야한다.
        gridSize = vector3Int;
        grid = new int[gridSize.x, gridSize.y, gridSize.z];

        // 장애물 추가. 맵의 정보를 토대로 장애물위치를 입력하는 로직이 필요하다.
        grid[2, 2, 2] = 1; // 장애물 위치
        grid[2, 2, 3] = 1;
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
    {
        //탐색할 노드 목록
        openList.Clear();
        //이미 탐색한 노드 목록
        closedList.Clear();

        //시작노드와 목표노드를 생성한다(위치를 기반으로).
        Node startNode = new Node(start);
        Node targetNode = new Node(target);

        //시작노드를 탐색할 노드 목록에 추가한다.
        openList.Add(startNode);

        //더이상 탐색할 노드가 없을 때까지 진행한다.
        while (openList.Count > 0)
        {
            // FCost가 가장 낮은 노드 찾기 -> 예상 비용이 가장적은 노드 == 가장 가까운 노드.
            // 만약 FCost가 같은 노드가 다수이면 HCost가 가장 낮은 노드를 설정
            //탐색할 노드 중 가장 처음 노드를 현재 노드로 설정
            Node currentNode = openList[0];
            foreach (var node in openList)
            {
                if (node.FCost < currentNode.FCost || 
                    (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                {
                    currentNode = node;
                }
            }

            //선택된 노드를 탐색'할'노드에서 제거하고 탐색'한'노드로 이동 == 탐색완료
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            
            // 목표 지점에 도달했는지 확인. 목표지점에 도달했다면 해당 경로를 반환.
            if (currentNode.Position == targetNode.Position)
            {
                return RetracePath(startNode, currentNode);
            }
            

            // 만약 현재 지점이 목표지점이 아니라면, 현재 노드의 이웃 노드 탐색
            foreach (var neighbor in GetNeighbors360(currentNode))
            {
                if (closedList.Exists(n => n.Position == neighbor.Position) || //이미 탐색한 노드이거나
                    grid[neighbor.Position.x, neighbor.Position.y, neighbor.Position.z] == 1)//장애물이거나
                {
                    continue; // 이미 탐색했거나 장애물인 경우
                }

                int newGCost = currentNode.GCost + 1; // 이동 비용
                if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = GetHeuristic(neighbor.Position, targetNode.Position);
                    neighbor.Parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; // 경로를 찾을 수 없음
    }

    private List<Vector3Int> RetracePath(Node startNode, Node endNode)
    {
        //각 경로의 부모를 이전 경로로 설정하여 반환하는 로직
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse(); // 경로를 올바른 순서로 정렬
        return path;
    }

    private List<Node> GetNeighbors360(Node currentNode, int angleStep = 10)
    {
        List<Node> neighbors = new List<Node>();

        // 360도를 angleStep 간격으로 나눔
        for (int angle = 0; angle < 360; angle += angleStep)
        {
            // 라디안 단위로 변환
            float radian = Mathf.Deg2Rad * angle;

            // 방향 계산 (단위 벡터)
            int x = Mathf.RoundToInt(Mathf.Cos(radian));
            int z = Mathf.RoundToInt(Mathf.Sin(radian));

            // 현재 위치에서 이동한 위치
            Vector3Int neighborPosition = currentNode.Position + new Vector3Int(x, 0, z);

            if (IsValidPosition360(neighborPosition))
            {
                neighbors.Add(new Node(neighborPosition));
            }
        }

        return neighbors;
    }
    
    private bool IsValidPosition360(Vector3Int position)
    {
        if (position.x < 0 || position.x >= gridSize.x ||
            position.z < 0 || position.z >= gridSize.z)
        {
            return false; // 범위 초과
        }

        if (grid[position.x, position.y, position.z] == 1)
        {
            return false; // 장애물 위치
        }

        return true;
    }

    private bool IsValidPosition(Vector3Int position)
    {
        return position.x >= 0 && position.x < grid.GetLength(0) &&
               position.y >= 0 && position.y < grid.GetLength(1) &&
               position.z >= 0 && position.z < grid.GetLength(2);
    }

    private int GetHeuristic(Vector3Int a, Vector3Int b)
    {
        // 맨해튼 거리 계산
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
