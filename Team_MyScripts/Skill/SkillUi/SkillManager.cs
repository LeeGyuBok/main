using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [SerializeField] private GameObject barrierPrefab;

    [SerializeField] private List<Button> skillButtons;
    [SerializeField] private List<Image> lockImages;
    [SerializeField] private List<TextMeshProUGUI> coolTimeText;
    

    private Liminex liminex;
    public Liminex Liminex
    {
        get => liminex;
        set
        {
            if (liminex == null)
            {
                liminex = value;
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetSkillImageOnUi();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject InstantiateBarrierPrefabObject()
    {
        GameObject barrierPrefabObject = Instantiate(barrierPrefab, UiManager.Instance.Player.transform.position, barrierPrefab.transform.rotation);
        return barrierPrefabObject;
    }

    public void ActivateSkill(IEnumerator iamSkill)
    {
        StartCoroutine(iamSkill);
    }

    private void SetSkillImageOnUi()
    {
        if (liminex != null)
        {
            for (int i = 0; i < liminex.skills.Count; i++)
            {
                //스킬 이미지 할당
                skillButtons[i].image.sprite = liminex.skills[i].SkillImage;
                //해당 스킬이 투자되있으면 
                if (liminex.skills[i].IsAvailable)
                {
                    lockImages[i].gameObject.SetActive(false);
                    int coolTime = (int)liminex.skills[i].BaseCoolTime;
                    coolTimeText[i].text = coolTime.ToString();
                }
                else//그렇지 않으면
                {
                    lockImages[i].gameObject.SetActive(true);
                    coolTimeText[i].text = "Lock";
                }
            }
        }
        else
        {
            Debug.Log("liminex disconnected");
        }
    }

    public void RequestSetSkillUi()
    {
        SetSkillImageOnUi();
    }
}
