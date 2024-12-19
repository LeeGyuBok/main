using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


//노드가 가지고있어야할 정보
//이 노드로 이동할 수 있는지? walkable
//이 노드의 실제 위치(월드포지션)가 어디인지? Vector3 <- 3차원 평면. y값 제외. 왠만하면 정수에 맞추자?
//이 노드가 맵을 2차원 배열화한 자료구조(그리드포지션)에서 어디에 위치하고있는지? <- 2차원 배열, 
//배열이기때문에, vector3Int로 사용한거네. int girdX, gridZ
//특정 위치를 의미하는지? <- 이넘
//이 노드로 이동하기 위한 비용(시작노드~여기까지, 여기에서 목표까지- 추정비용)
//이 노드를 경유하여 목표까지 도달했을 때의 총비용 == 시작노드~여기까지 + 여기에서 목표까지
//이 노드의 이전 노드(부모노드로, 추적위함)

public enum SignPoint
{
    None,
    StartPoint,
    TargetPoint,
    ReturnPoint
}
public class MyNode
{
    public bool Walkable;//이동 가능 여부
    public Vector3 WorldPosition;// 월드 내 위치
    public Vector3Int Position;//월드 내 위치의 정수표현
    public int GridX, GridZ;//그리드 2차원배열 상 위치
    public SignPoint Sign;//이 노드가 어떤 노드인지? 시작지점? 목표지점? 복귀지점?

    public int CurrentCost;//여기까지 오는데 든 비용
    public int HeuristicCost;//여기에서 목표까지 가는데 드는 예상 비용
    public int TotalCost => CurrentCost + HeuristicCost;//총 추정 비용

    public MyNode Parent;//추적용 부모노드.

    public MyNode(bool walkable, Vector3 worldPosition, int gridX, int gridZ, SignPoint sign)
    {
        Walkable = walkable;
        worldPosition.y = 0;
        WorldPosition = worldPosition;
        GridX = gridX;
        GridZ = gridZ;
        Sign = sign;
        
        Position = Vector3Int.RoundToInt(WorldPosition);
    }
}

public class MySecondAStar : MortalManager<MySecondAStar>
{
    private GameObject planeObject;
    [SerializeField] private GameObject testGizmo;
    [SerializeField] private int gridSize;//나는 정사각형으로 할래
    [SerializeField] private OperatorBattleStatus myOperator;

    [SerializeField] private List<GameObject> MapPrefabs;
    
    //지형위에 만들어놓을 그리드. 2차원 배열
    public MyNode[,] Grid { get; private set; }
    private int gridSizeX;
    private int gridSizeZ;
    private Vector3 planeSize;
    
    //sign을 표지하는 역할을 한다.
    public LayerMask unwalkableMask; // 이동 불가능한 레이어
    public LayerMask startPointMask; // 스타트지점 레이어
    public LayerMask targetPointMask; // 목표지점 레이어
    public LayerMask returnPointMask; // 복귀지점 레이어
    
    private List<MyNode> possibleNodes = new List<MyNode>();
    private List<MyNode> exceptedNodes = new List<MyNode>();

    private List<MyNode> startNodes = new List<MyNode>();
    private List<MyNode> targetNodes = new List<MyNode>();
    private List<MyNode> returnNodes = new List<MyNode>();

    private MyNode StartNode { get; set; }
    private MyNode TargetNode { get; set; }
    private MyNode ReturnNode { get; set; }
    
    private List<MyNode> TestPath = new List<MyNode>();

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(Grid.GetLength(0) + "x" + Grid.GetLength(1) + "z");
        //Debug.Log(StartNode.WorldPosition);
        ImmortalCamera.Instance.gameObject.transform.position = new Vector3(0f, 25f, 0f);
    }

    public void Spawn()
    {
        Initialize();
        Vector3 spawn = StartNode.WorldPosition + new Vector3(0, 0.6f, 0);
        Instantiate(myOperator, spawn, quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //그리드 만들기
    //그리드는 월드맵에 있는 특정 지형(평면)을 기준으로 만들어짐. 현재는 사각형만됨 (직, 정)
    //그리드에 들어있는 노드들은 고유의 sign을 갖고있암.

    private void Initialize()
    {
        int random = UnityEngine.Random.Range(0, MapPrefabs.Count);
        GameObject selectedMap = Instantiate(MapPrefabs[random], new Vector3(0,0,-6f), Quaternion.identity);
        planeObject = selectedMap.transform.GetChild(0).gameObject;
        
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

        gridSizeX = Mathf.RoundToInt(planeSize.x / gridSize);
        gridSizeZ = Mathf.RoundToInt(planeSize.z / gridSize);

        //오전 2:34 2024-12-03 뭔가 매우 잘못되었음을 인지하다.

        Grid = new MyNode[gridSizeX, gridSizeZ];

        Vector3 bottomLeft = planeObject.transform.position - Vector3.right * planeSize.x / 2 -
                             Vector3.forward * planeSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                //오전 2:34 2024-12-03
                //애초에 이 월드포인트가 잘못된거같은데?
                //아닌데? 얜 정상인데?
                Vector3 worldPoint = bottomLeft + Vector3.right * ((x + 0.5f) * gridSize) +
                                     Vector3.forward * ((z + 0.5f) * gridSize);
                //Instantiate(testGizmo, worldPoint, Quaternion.identity);
                Grid[x, z] = new MyNode(true, worldPoint, x, z, SignPoint.None);

                // 체크박스에 걸리는게 있다 == 장애물이 있다.
                bool isObstacle = Physics.CheckBox(worldPoint,
                    new Vector3(gridSize * 0.5f, gridSize * 0.5f, gridSize * 0.5f), quaternion.identity,
                    unwalkableMask);
                bool isStart = Physics.CheckBox(worldPoint,
                    new Vector3(gridSize * 0.5f, gridSize * 0.5f, gridSize * 0.5f), quaternion.identity,
                    startPointMask);
                bool isTarget = Physics.CheckBox(worldPoint,
                    new Vector3(gridSize * 0.5f, gridSize * 0.5f, gridSize * 0.5f), quaternion.identity,
                    targetPointMask);
                bool isReturn = Physics.CheckBox(worldPoint,
                    new Vector3(gridSize * 0.5f, gridSize * 0.5f, gridSize * 0.5f), quaternion.identity,
                    returnPointMask);
                if (isObstacle)
                {
                    //장애물이 있으니 못간다.
                    Grid[x, z].Walkable = false;
                }

                if (isStart)
                {
                    Grid[x, z].Sign = SignPoint.StartPoint;
                    startNodes.Add(Grid[x, z]);
                    //여기는 정상.
                    Debug.Log(Grid[x, z].WorldPosition); //이건정상
                    //Instantiate(testGizmo, Grid[x,z].WorldPosition, Quaternion.identity);
                }

                if (isTarget)
                {
                    Grid[x, z].Sign = SignPoint.TargetPoint;
                    targetNodes.Add(Grid[x, z]);
                    Debug.Log(Grid[x, z].WorldPosition); //이건정상
                }

                if (isReturn)
                {
                    Grid[x, z].Sign = SignPoint.ReturnPoint;
                    returnNodes.Add(Grid[x, z]);
                }
            }
        }

        //아래 노드들은 그리드 상의 노드다.
        //StartNode = GetNodeFromWorldPosition(MakeAverageWorldPosition(startNodes));

        StartNode = GetNodeFromWorldPosition(SelectNodeInListByWorldPosition(startNodes));
        //Debug.Log("start world: " + StartNode.WorldPosition);
        TargetNode = GetNodeFromWorldPosition(SelectNodeInListByWorldPosition(targetNodes));
        //Debug.Log("target world: " + TargetNode.WorldPosition);
        ReturnNode = GetNodeFromWorldPosition(SelectNodeInListByWorldPosition(returnNodes));
        //Debug.Log("return world: " + ReturnNode.WorldPosition);

        TestPath = FindPath(StartNode, TargetNode);
        /*for (int i = 0; i < TestPath.Count; i++)
        {
            Debug.Log("Path: " + TestPath[i]);
        }*/
        //Debug.Log(TestPath is null);
    }

    //다른 객체들이 경로를 찾기
    public List<MyNode> FindNewPathByWorldPosition(Vector3 startPoint, Vector3 targetPoint)
    {
        MyNode startNode = GetNodeFromWorldPosition(startPoint);
        MyNode targetNode = GetNodeFromWorldPosition(targetPoint);
        return FindPath(startNode, targetNode);
    }
    
    public List<MyNode> FindNewPathByNodes(MyNode startNode, MyNode targetNode)
    {
        return FindPath(startNode, targetNode);
    }

    public MyNode GetStartNode()
    {
        return StartNode;
    }

    public MyNode GetTargetNode()
    {
        return TargetNode;
    }

    public MyNode GetReturnNode()
    {
        return ReturnNode;
    }

    public MyNode GetCurrentNode(Vector3 worldPosition)
    {
        MyNode node = GetNodeFromWorldPosition(worldPosition);
        return node;
    }
    
    //경로찾기
    private List<MyNode> FindPath(MyNode start, MyNode target)
    {
        //오전 1:52 2024-12-03
        //스타트, 타겟 패스 구하는 중. 현재 여기에 들어오는 노드들은 그리드상의 노드다.
        possibleNodes.Clear();
        exceptedNodes.Clear();
        
        MyNode startNode = start;
        MyNode targetNode = target;
        
        possibleNodes.Add(startNode);

        while (possibleNodes.Count > 0)
        {
            MyNode current = possibleNodes[0];
            
            //탐색 가능한 모든 노드의 현재 비용과 추정비용을 비교하여 현재 위치한 노드를 선택한다.
            foreach (var node in possibleNodes)
            {
                if (node.CurrentCost < current.CurrentCost ||// 지금까지의 비용이 더 적거나
                    node.CurrentCost == current.CurrentCost && node.HeuristicCost < current.HeuristicCost)//추정치가 더 적다면
                {
                    current = node;
                }
            }
            //Debug.Log(current.Sign);
            //탐색가능한 노드에서 해당 노드를 지우고 탐색이 끝난 노드에 추가한다.
            possibleNodes.Remove(current);
            exceptedNodes.Add(current);
            
            //현재 스타트노드. 타겟노드 로 패스 구하는중
            //(-22, 0, -6), (-22, 0, 1)
            //Debug.Log($"{current.Position}, {targetNode.Position}");
            
            //현재의 월드 포지션 정수위치가 목표의 월드 포지션 정수위치와 같다면
            //-> 월드 포지션의 실수위치에서 어느정도 비슷한 위치면 도착한 것으로 간주.
            //탐색 종료 노드에 추가한 노드가 목표 노드와 좌표가 같다면, 두 경로를 연결하여 최단거리 경로를 생성하고 리턴한다.
            if (current.Position == targetNode.Position)
            {
                return RetracePath(start, current);
            }
            
            //도착하지 못했다면? 이웃노드를 탐색한다.
            foreach (var neighbor in GetNeighbors360(current))
            {
                //Debug.Log(neighbor.Position);//얘는 그리드상 노드다.
                //제외된 노드에, 노드가 존재하면, 어떤 노드냐면 노드의 월드 내 정수위치와 이웃노드의 정수위치가 같은(동일한 노드) 또는 장애물인
                //여기서 동일한노드란? 이미 확인한 노드..인가?
                if (exceptedNodes.Exists(node => node.Position == neighbor.Position)||
                    !neighbor.Walkable)
                {
                    continue;
                }

                //장애물도아니고 같은 노드도아니면
                int newCurrentCost = current.CurrentCost + 1; //이동비용
                //현재 노드에서 이동하는 비용이 이웃노드로의 이동비용보다 작거나, 탐색가능노드에 이웃노드가 없다면
                if (newCurrentCost < neighbor.CurrentCost || !possibleNodes.Contains(neighbor))
                {
                    neighbor.CurrentCost = newCurrentCost;
                    //최단경로 추정값
                    neighbor.HeuristicCost = GetHeuristic(neighbor.Position, target.Position);
                    neighbor.Parent = current;

                    if (!possibleNodes.Contains(neighbor))
                    {
                        possibleNodes.Add(neighbor);
                    }
                }
            }
        }
        
        return null;
    }

    //네이버 포지션을 못구한다.
    private List<MyNode> GetNeighbors360(MyNode currentNode, int angleStep = 45)
    {
        //스타트, 타겟 패스구하는중
        List<MyNode> neighborNodes = new List<MyNode>();
        for (int angle = 0; angle < 360; angle += angleStep)
        {
            //Debug.Log(angle);
            // 라디안 단위로 변환
            float radian = Mathf.Deg2Rad * angle;

            // 방향 계산 (단위 벡터). 
            float x = Mathf.RoundToInt(Mathf.Cos(radian));
            float z = Mathf.RoundToInt(Mathf.Sin(radian));
            //Debug.Log($"{x}, {z}"); //8개 방향으로 다구함. 이 크기가 좀 짧은듯? 찾았다. 여기다. < 아니엇다. 0157
            
            //단위벡터만 더해서는 이웃노드까지 닿지 않았다. 그래서 에러난듯? < 아니었죠
            //오전 2:05 2024-12-03 
            
            //일단 이 네이버 포지션이 이상하다. 이게 이상하니까 다른것들도 이상하게나오지 근데 위에 8방향은 맞음. 뭐임?
            Vector3 neighborPosition = currentNode.WorldPosition + new Vector3(x, 0, z);
            //Instantiate(testGizmo, neighborPosition, Quaternion.identity);
            
            MyNode neighbor = GetNodeFromWorldPosition(neighborPosition);//여기서 주는 것은 그리드 상 노드다.
            //Debug.Log(neighbor.Position);
            // 여기가 문제다 진짜로. 오전 1:48 2024-12-03에 수정했음. add 되는거 확인완료<- 근데 네이버노드 구하는게 이상한데?
            if (IsValidPosition360(neighborPosition, neighbor))
            {
                //Debug.Log("add");
                neighborNodes.Add(neighbor);    
            }
        }
        return neighborNodes;
    }

    //찐 범인은 이녀석이었고 오후 1:53 2024-12-03
    private MyNode GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        //오전 2:59 2024-12-03 해결완료.
        //월드포지션가지고올 때 잘못되게 가지고옴.
        //Debug.Log(worldPosition);
        //얘랑
        //Instantiate(testGizmo, worldPosition, Quaternion.identity);

        //그리드의 가장 처음 위치. 이것을 잘 설정해줘야합니다.
        Vector3 gridOrigin = planeObject.transform.position - Vector3.right * planeSize.x / 2 -
                             Vector3.forward * planeSize.z / 2;

        // 그리드 내에서 상대 좌표 계산
        //가장 처음 위치에서 아규먼트로 받은 월드포지션이 어느정도 비율에 있는지 확인한다.
        float percentX = (worldPosition.x - gridOrigin.x) / planeSize.x;
        float percentZ = (worldPosition.z - gridOrigin.z) / planeSize.z;

        // 0~1 범위로 클램프
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        // 그리드의 노드 인덱스 계산
        int x = Mathf.Clamp(Mathf.FloorToInt(percentX * gridSizeX), 0, gridSizeX - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt(percentZ * gridSizeZ), 0, gridSizeZ - 1);
        
        //얘가 같아야하는데 드디어 같아짐- 오전 2:59 2024-12-03
        //Instantiate(testGizmo, Grid[x, z].WorldPosition, Quaternion.identity);
        return Grid[x, z];
    }
    
    private bool IsValidPosition360(Vector3 position, MyNode neighbor)
    {
        Vector3Int gridPosition = WorldToGridPosition(position);

        // 그리드 좌표가 유효한지 확인
        if (gridPosition.x < 0 || gridPosition.x >= gridSizeX ||
            gridPosition.z < 0 || gridPosition.z >= gridSizeZ)
        {
            return false; // 범위 초과
        }

        // neighbor가 null이거나 이동 불가능한 경우 확인
        if (neighbor == null || !neighbor.Walkable)
        {
            return false; // 장애물 또는 유효하지 않은 노드
        }

        return true;
    }
    
    private int GetHeuristic(Vector3Int a, Vector3Int b)
    {
        // 맨해튼 거리 계산. 
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
    
    private List<MyNode> RetracePath(MyNode startNode2, MyNode endNode2)
    {
        //각 경로의 부모를 이전 경로로 설정하여 반환하는 로직
        List<MyNode> path = new List<MyNode>();
        MyNode currentNode2 = endNode2;

        while (currentNode2 != startNode2)
        {
            path.Add(currentNode2);
            currentNode2 = currentNode2.Parent;
        }

        path.Reverse(); // 경로를 올바른 순서로 정렬
        return path;
    }
    
    private Vector3 MakeAverageWorldPosition(List<MyNode> nodes)
    {
        //여기가 제일 의심스러운데? 아니다 여기도 맞닼ㅋㅋㅋ
        Vector3 sum = Vector3.zero;
        foreach (var node in nodes)
        {
            sum += node.WorldPosition;
        }
        sum /= nodes.Count;
        //Debug.Log(sum);
        return sum;
    }
    
    private Vector3 SelectNodeInListByWorldPosition(List<MyNode> nodes)
    {
        return nodes[0].WorldPosition;
    }
    
    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + planeSize.x / 2) / planeSize.x;
        float percentZ = (worldPosition.z + planeSize.z / 2) / planeSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return new Vector3Int(x, 0, z); // Y는 필요 없으므로 0으로 설정
    }

    public List<MyNode> GetPath()
    {
        return TestPath;
    }
    
    /*private void OnDrawGizmos()
    {
        if (Grid != null)
        {
            foreach (MyNode node in Grid)
            {
                if (node.Walkable)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.black;
                }
                switch (node.Sign)
                {
                    case SignPoint.StartPoint:
                        Gizmos.color = Color.blue;
                        break;
                    case SignPoint.TargetPoint:
                        Gizmos.color = Color.red;
                        break;
                    case SignPoint.ReturnPoint:
                        Gizmos.color = Color.white;
                        break;
                }
                
                Gizmos.DrawCube(node.WorldPosition, new Vector3(gridSize, gridSize, gridSize)*0.9f);
            }
        }
        
        for (int i = 0; i < TestPath.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(TestPath[i].WorldPosition, 0.5f);
        }
    }*/

}
