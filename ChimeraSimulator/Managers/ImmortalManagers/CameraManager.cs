using UnityEngine;

public class CameraManager : ImmortalObject<CameraManager>
{

    private Vector3 _originPosition;
    [SerializeField] private Vector3 _createPosition;

    protected override void Awake()
    {
        base.Awake();
        _originPosition = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(string sceneName)
    {
        //Debug.Log(sceneName);
        switch (sceneName)
        {
            case "CreateChimeraScene":
                transform.position = _createPosition;
                break;
            default:
                transform.position = _originPosition;
                break;
        }
    }
}
