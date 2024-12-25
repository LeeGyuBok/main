using System;
using System.Collections.Generic;
using UnityEngine;


//어디냐 그.. MyAStar가 생성하는 그리드 배열의 구성원.
public class MyNode2
{
    public bool Walkable;        // 이동 가능 여부
    public Vector3 WorldPosition; // 노드의 월드 좌표
    public int GridX, GridZ;     // 그리드 상의 X, Z 좌표

    public bool IsStartPoint;
    public bool IsTargetPoint;
    public bool IsReturnPoint;
    
    public int GCost;             // 시작 노드에서 현재 노드까지의 거리 비용
    public int HCost;             // 휴리스틱 (목표까지의 예상 비용)

    public int FCost => GCost + HCost; // F = G + H (총 비용)

    public MyNode2 Parent;
    
    public Vector3Int Position;

    public MyNode2(bool walkable, Vector3 worldPosition, int gridX, int gridZ)
    {
        Walkable = walkable;
        WorldPosition = worldPosition;
        GridX = gridX;
        GridZ = gridZ;

        Position = Vector3Int.RoundToInt(worldPosition);
    }
}

public class MyAStar : MortalManager<MyAStar>
{
    [SerializeField] private GameObject planeObject; // Plane을 참조할 객체
    [SerializeField] private float nodeRadius;  // 각 노드의 반지름. 이 반지름은... 그 뭐냐, 인게임 오브젝트의 크기에 따라 결정하자.
    [SerializeField] private OperatorBattleStatus TestOperater;
    
    private MyNode2[,] grid;          // 2D 배열로 그리드 관리
    public LayerMask unwalkableMask; // 이동 불가능한 레이어
    public LayerMask startPointMask; // 스타트지점 레이어
    public LayerMask targetPointMask; // 목표지점 레이어
    public LayerMask returnPointMask; // 복귀지점 레이어

    private int gridSizeX, gridSizeZ; // 그리드의 X, Z 방향 노드 개수
    private float nodeDiameter;       // 노드의 지름
    private Vector3 planeSize;        // Plane의 크기
    
    private List<MyNode2> openList = new List<MyNode2>();
    private List<MyNode2> closedList = new List<MyNode2>();
    
    private List<MyNode2> startNodes = new List<MyNode2>();
    private List<MyNode2> targetNodes = new List<MyNode2>();
    private List<MyNode2> returnNodes = new List<MyNode2>();
    
    public List<Vector3Int> TargetPath { get; private set; } = new List<Vector3Int>();
    public List<Vector3Int> ReturnPath { get; private set; } = new List<Vector3Int>();

    protected override void Awake()
    {
        base.Awake();
        InitializeGrid();
    }

    void Start()
    {
        //Instantiate(TestOperater, SelectNodeInNodes(startNodes).WorldPosition, Quaternion.identity);
        //내가 원하는 방법
        /*TargetPath = FindPath(SelectNodeInNodes(startNodes), SelectNodeInNodes(targetNodes));
        if (TargetPath == null)
        {
            Debug.Log("No path found: start to target");
        }
        else
        {
            string root = "";
            for (int i = 1; i < TargetPath.Count; i++)
            {
                root += TargetPath[i] + ", ";
                //Gizmos.DrawRay(path[i-1], path[i-1] - path[i]); //온드로우기즈모에서만 작동 ㅋㅋ;

            }
            Debug.Log(root);
        }

        ReturnPath = FindPath(SelectNodeInNodes(targetNodes), SelectNodeInNodes(returnNodes));
        if (ReturnPath == null)
        {
            Debug.Log("No path found: target to return");
        }
        else
        {
            string root2 = "";
            for (int i = 1; i < ReturnPath.Count; i++)
            {
                root2 += ReturnPath[i] + ", ";
                //Gizmos.DrawRay(path2[i-1], path2[i-1] - path2[i]);//온드로우기즈모에서만 작동 ㅋㅋ;

            }
            Debug.Log(root2);
        }*/

        /*if (FindPath(grid[1,1], grid[10,10]) == null)
        {
            Debug.Log("No path found: start to target");
        }

        if (FindPath(grid[10,10], grid[20,20]) == null)
        {
            Debug.Log("No path found: target to return");
        }*/


    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭 시
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // 클릭된 오브젝트 처리
                Debug.Log(hit.point);
                MyNode2 selectedNode2 = GetNodeFromWorldPosition(hit.point);

                if (selectedNode2.Walkable)
                {
                    Debug.Log($"선택한 노드는 이동 가능합니다: {selectedNode2.WorldPosition}");
                }
                else
                {
                    Debug.Log($"선택한 노드는 이동 불가능합니다: {selectedNode2.WorldPosition}");
                }
                //Debug.Log("Clicked on " + hit.collider.name);
                // 여기서 오브젝트 B의 특정 동작을 수행하도록 추가 코드 작성 가능
            }
  
        }
    }

    void InitializeGrid()
    {
        if (planeObject == null)
        {
            Debug.LogError("Plane 객체가 할당되지 않았습니다!");
            return;
        }

        // Plane 크기 계산. 정사각형 또는 직사각형이어야할듯? < 골든 정답 ㅋㅋ
        MeshRenderer planeRenderer = planeObject.GetComponent<MeshRenderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Plane 객체에 MeshRenderer가 없습니다!");
            return;
        }
        
        planeSize = planeRenderer.bounds.size;
        
        Debug.Log(planeSize);

        nodeDiameter = nodeRadius * 2f;
        gridSizeX = Mathf.RoundToInt(planeSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(planeSize.z / nodeDiameter);

        grid = new MyNode2[gridSizeX, gridSizeZ];

        // Plane 위에
        Vector3 bottomLeft = planeObject.transform.position - Vector3.right * planeSize.x / 2 - Vector3.forward * planeSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                //Debug.Log(worldPoint);
                // 장애물 감지. 이부분도 시작지점, 목표지점, 복귀지점을 설정할 때 사용할 수 있을듯.
                //worldpoint위치의 noderadis반지름만큼의 구와 unwalkablemask에서 충돌하는 물체가 있으면 true, 없으면 false
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                
                bool startPoint = Physics.CheckSphere(worldPoint, nodeRadius, startPointMask);
                bool targetPoint = Physics.CheckSphere(worldPoint, nodeRadius, targetPointMask);
                bool returnPoint = Physics.CheckSphere(worldPoint, nodeRadius, returnPointMask);
                
                grid[x, z] = new MyNode2(walkable, worldPoint, x, z);
                //이걸로 각 위치가 지정되는 것을 확인함
                if (startPoint)
                {
                    grid[x, z].IsStartPoint = true;
                    startNodes.Add(grid[x, z]);
                }
                if (targetPoint)
                {
                    grid[x, z].IsTargetPoint = true;
                    targetNodes.Add(grid[x, z]);
                }
                if (returnPoint)
                {
                    grid[x, z].IsReturnPoint = true;
                    returnNodes.Add(grid[x, z]);
                }
            }
        }
        Debug.Log("그리드 초기화 완료!");
        //잘됨

        TargetPath = FindPath(SelectNodeInNodes(startNodes), SelectNodeInNodes(targetNodes));
        ReturnPath = FindPath(SelectNodeInNodes(targetNodes), SelectNodeInNodes(returnNodes));

    }

    private MyNode2 GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        //Debug.Log(worldPosition);
        // 월드 좌표에서 그리드 좌표 반환. 이걸 통해서! 그 뭐시냐. 각 포인트를 지정할 수 있을거같다!
        float percentX = (worldPosition.x + planeSize.x / 2) / planeSize.x;
        float percentZ = (worldPosition.z + planeSize.z / 2) / planeSize.z;

        //Debug.Log($"{percentX}. {percentZ}");
        
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        //Debug.Log($"{percentX}. {percentZ}");
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        //Debug.Log($"{x}. {z}");

        return grid[x, z];
    }

    public MyNode2[,] GetGrid()
    {
        return grid;
    }
    
    void OnDrawGizmos()
    {
        /*if (grid != null)
        {
            foreach (MyNode2 node in grid)
            {
                // 노드가 이동 가능하면 초록색, 아니면 빨간색으로 표시 <- 이거 어쩌면 시작지점, 목표지점, 복귀지점을 설정할 때 사용할수 있을듯
                if (node.Walkable)
                {
                    Gizmos.color = Color.white;
                    if (node.IsStartPoint)
                    {
                        Gizmos.color = Color.blue;
                    }
                    if (node.IsReturnPoint)
                    {
                        Gizmos.color = Color.green;
                    }
                    if (node.IsTargetPoint)
                    {
                        Gizmos.color = Color.red;
                    }
                }
                else
                {
                    Gizmos.color = Color.black;
                }

                Gizmos.DrawSphere(node.WorldPosition, nodeRadius);
            }
        }*/
        
        /*TargetPath = FindPath(SelectNodeInNodes(startNodes), SelectNodeInNodes(targetNodes));
        if (TargetPath == null)
        {
            Debug.Log("No path found: start to target");
        }
        else
        {
            //string root = "";
            for (int i = 1; i < TargetPath.Count; i++)
            {
                //root += TargetPath[i] + ", ";
                Gizmos.DrawSphere(TargetPath[i], nodeRadius);
               
            }
            //Debug.Log(root);
        }
        
        ReturnPath = FindPath(SelectNodeInNodes(targetNodes), SelectNodeInNodes(returnNodes));
        if (ReturnPath == null)
        {
            Debug.Log("No path found: target to return");
        }
        else
        {
            //string root2 = "";
            for (int i = 0; i < ReturnPath.Count; i++)
            {
                //root2 += ReturnPath[i] + ", ";
                Gizmos.DrawSphere(ReturnPath[i], nodeRadius);
               
            }
            //Debug.Log(root2);
        }*/
    }

    public List<Vector3Int> FindPath(MyNode2 startPointNode2, MyNode2 endPointNode2)
    {
        openList.Clear();
        closedList.Clear();

        MyNode2 startNode2 = startPointNode2;
        MyNode2 targetNode2 = endPointNode2;
        
        //Debug.Log($"{startNode2.Position}, {targetNode2.Position}");


        openList.Add(startNode2);

        
        while (openList.Count > 0)
        {
            Debug.Log(openList.Count);
            // FCost가 가장 낮은 노드 찾기 -> 예상 비용이 가장적은 노드 == 가장 가까운 노드.
            // 만약 FCost가 같은 노드가 다수이면 HCost가 가장 낮은 노드를 설정
            //탐색할 노드 중 가장 처음 노드를 현재 노드로 설정
            MyNode2 currentNode2 = openList[0];
            foreach (var node in openList)
            {
                if (node.FCost < currentNode2.FCost || 
                    (node.FCost == currentNode2.FCost && node.HCost < currentNode2.HCost))
                {
                    currentNode2 = node;
                }
            }

            //선택된 노드를 탐색'할'노드에서 제거하고 탐색'한'노드로 이동 == 탐색완료
            openList.Remove(currentNode2);
            closedList.Add(currentNode2);

            
            //Debug.Log($"{currentNode2.Position}, {targetNode2.Position}");
            // 목표 지점에 도달했는지 확인. 목표지점에 도달했다면 해당 경로를 반환.
            if (currentNode2.Position == targetNode2.Position)
            {
                return RetracePath(startNode2, currentNode2);
            }
            

            // 만약 현재 지점이 목표지점이 아니라면, 현재 노드의 이웃 노드 탐색
            foreach (var neighbor in GetNeighbors360(currentNode2))
            {
                Debug.Log($"{neighbor.Position}");
                if (closedList.Exists(n => n.Position == neighbor.Position) || //이미 탐색한 노드이거나
                    !neighbor.Walkable)//장애물이거나
                {
                    continue; // 이미 탐색했거나 장애물인 경우
                }

                int newGCost = currentNode2.GCost + 1; // 이동 비용
                if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = GetHeuristic(neighbor.Position, targetNode2.Position);
                    neighbor.Parent = currentNode2;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; // 경로를 찾을 수 없음
    }
    
    private List<MyNode2> GetNeighbors360(MyNode2 currentNode2, int angleStep = 15)
    {
        List<MyNode2> neighbors = new List<MyNode2>();

        // 360도를 angleStep 간격으로 나눔
        for (int angle = 0; angle < 360; angle += angleStep)
        {
            // 라디안 단위로 변환
            float radian = Mathf.Deg2Rad * angle;

            // 방향 계산 (단위 벡터)
            float x = /*Mathf.RoundToInt*/(Mathf.Cos(radian));
            float z = /*Mathf.RoundToInt*/(Mathf.Sin(radian));

            // 현재 위치에서 이동한 위치
            Vector3 neighborPosition = currentNode2.WorldPosition + new Vector3(x, 0, z);

            MyNode2 neighbor = GetNodeFromWorldPosition(neighborPosition);
            
            if (IsValidPosition360(neighborPosition, neighbor))
            {
                //Debug.Log($"neighbor: {neighbor.Position}");
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
    
    private bool IsValidPosition360(Vector3 position, MyNode2 neighbor)
    {
        if (position.x < -gridSizeX || position.x >= gridSizeX ||
            position.z < -gridSizeZ || position.z >= gridSizeZ)
        {
            return false; // 범위 초과
        }
        
        if (!neighbor.Walkable)
        {
            return false; // 장애물 위치
        }

        return true;
    }

    private MyNode2 AverageWalkableNode(List<MyNode2> nodes)
    {
        Vector3 sum = Vector3.zero;
        int gridx = 0;
        int gridz = 0;
        
        foreach (MyNode2 node in nodes)
        {
            sum += node.WorldPosition;
            gridx += node.GridX;
            gridz += node.GridZ;
        }
        Vector3 average = sum / nodes.Count;
        gridx = Mathf.RoundToInt(average.x / nodes.Count);
        gridz = Mathf.RoundToInt(average.z / nodes.Count);
        
        return new MyNode2(true, average, gridx, gridz);
    }

    private MyNode2 SelectNodeInNodes(List<MyNode2> nodes)
    {
        MyNode2 selectedNode2 = null; // 선택된 노드를 저장할 변수
        int minSum = int.MaxValue; // 초기 최소값을 큰 값으로 설정

        for (int i = 0; i < nodes.Count; i++)
        {
            int sum = nodes[i].GridX + nodes[i].GridZ; // GridX와 GridZ의 합 계산
            if (sum < minSum) // 최소값과 비교
            {
                minSum = sum;
                selectedNode2 = nodes[i]; // 최소값을 가진 노드 업데이트
                
            }
            Debug.Log(selectedNode2.Position);    
        }
        return selectedNode2;
    }
    
    private int GetHeuristic(Vector3Int a, Vector3Int b)
    {
        // 맨해튼 거리 계산
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
    
    private List<Vector3Int> RetracePath(MyNode2 startNode2, MyNode2 endNode2)
    {
        //각 경로의 부모를 이전 경로로 설정하여 반환하는 로직
        List<Vector3Int> path = new List<Vector3Int>();
        MyNode2 currentNode2 = endNode2;

        while (currentNode2 != startNode2)
        {
            path.Add(currentNode2.Position);
            currentNode2 = currentNode2.Parent;
        }

        path.Reverse(); // 경로를 올바른 순서로 정렬
        return path;
    }
}

