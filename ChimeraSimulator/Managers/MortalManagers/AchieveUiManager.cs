using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchieveUiManager : MortalManager<AchieveUiManager>, IGoMain
{
    [SerializeField] private Button achieveButton;
    
    [SerializeField] private Transform officialTestArchiveContent;
    [SerializeField] private Transform nonOfficialTestVictoryArchiveContent;
    [SerializeField] private Transform nonOfficialTestDefeatArchiveContent;
    [SerializeField] private Transform reallocationArchiveContent;
    [SerializeField] private Transform developmentChimeraArchiveContent;

    [SerializeField] private GameObject rewardPanel;

    public Dictionary<Button, OfficialTestAchieveInfo> OfficialTestAchieveInfosByButton;
    public Dictionary<Button, NonOfficialTestVictoryAchieveInfo> NonOfficialTestVictoryAchieveInfosByButton;
    public Dictionary<Button, NonOfficialTestDefeatAchieveInfo> NonOfficialTestDefeatAchieveInfosByButton;
    public Dictionary<Button, ReallocationAchieveInfo> ReallocationAchieveInfosByButton;
    public Dictionary<Button, DevelopmentChimeraAchieveInfo> DevelopmentChimeraAchieveInfosByButton;
    
    

    
    
    protected override void Awake()
    {
        base.Awake();
        OfficialTestAchieveInfosByButton = new Dictionary<Button, OfficialTestAchieveInfo>();
        NonOfficialTestVictoryAchieveInfosByButton = new Dictionary<Button, NonOfficialTestVictoryAchieveInfo>();
        NonOfficialTestDefeatAchieveInfosByButton = new Dictionary<Button, NonOfficialTestDefeatAchieveInfo>();
        ReallocationAchieveInfosByButton = new Dictionary<Button, ReallocationAchieveInfo>();
        DevelopmentChimeraAchieveInfosByButton = new Dictionary<Button, DevelopmentChimeraAchieveInfo>();
    }

    private void Start()
    {
        int i;
        List<Button> siblingButtons = new List<Button>();
        for (i = 0; i < AchieveManager.Instance.OfficialTestAchieveInfos.Count; i++)
        {
            Button button = Instantiate(achieveButton, officialTestArchiveContent);
            OfficialTestAchieveInfosByButton[button] = AchieveManager.Instance.OfficialTestAchieveInfos[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{AchieveManager.Instance.HighestOfficialTestClearStage} / {OfficialTestAchieveInfosByButton[button].NeedClearStageIndex}";
            if (AlreadyRewardedOfficialTestReward(button))
            {
                siblingButtons.Add(button);
                continue;
            }
            button.onClick.AddListener(() => GetOfficialTestReward(button));
            button.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (var siblingButton in siblingButtons)
        {
            siblingButton.transform.SetAsLastSibling();
        }
        siblingButtons.Clear();

        
        for (i = 0; i < AchieveManager.Instance.NonOfficialTestVictoryAchieveInfos.Count; i++)
        {
            Button button = Instantiate(achieveButton, nonOfficialTestVictoryArchiveContent);
            NonOfficialTestVictoryAchieveInfosByButton[button] = AchieveManager.Instance.NonOfficialTestVictoryAchieveInfos[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{AchieveManager.Instance.TotalNonOfficialTestVictoryCount} / {NonOfficialTestVictoryAchieveInfosByButton[button].NeedVictoryCount}";
            if (AlreadyRewardedNonOfficialTestVictoryReward(button))
            {
                siblingButtons.Add(button);
                continue;
            }
            button.onClick.AddListener(() => GetNonOfficialTestVictoryReward(button));
            button.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (var siblingButton in siblingButtons)
        {
            siblingButton.transform.SetAsLastSibling();
        }
        siblingButtons.Clear();
        
        
        for (i = 0; i < AchieveManager.Instance.NonOfficialTestDefeatAchieveInfos.Count; i++)
        {
            Button button = Instantiate(achieveButton, nonOfficialTestDefeatArchiveContent);
            NonOfficialTestDefeatAchieveInfosByButton[button] = AchieveManager.Instance.NonOfficialTestDefeatAchieveInfos[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{AchieveManager.Instance.TotalNonOfficialTestDefeatCount} / {NonOfficialTestDefeatAchieveInfosByButton[button].NeedDefeatCount}";
            if (AlreadyRewardedNonOfficialTestDefeatReward(button))
            {
                siblingButtons.Add(button);
                continue;
            }
            button.onClick.AddListener(() => GetNonOfficialTestDefeatReward(button));
            button.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (var siblingButton in siblingButtons)
        {
            siblingButton.transform.SetAsLastSibling();
        }
        siblingButtons.Clear();
        
        
        for (i = 0; i < AchieveManager.Instance.ReallocationAchieveInfos.Count; i++)
        {
            Button button = Instantiate(achieveButton, reallocationArchiveContent);
            ReallocationAchieveInfosByButton[button] = AchieveManager.Instance.ReallocationAchieveInfos[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{AchieveManager.Instance.TotalReallocationCount} / {ReallocationAchieveInfosByButton[button].NeedReallocationCount}";
            if (AlreadyRewardedReallocationReward(button))
            {
                siblingButtons.Add(button);
                continue;
            }
            button.onClick.AddListener(() => GetReallocationReward(button));
            button.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (var siblingButton in siblingButtons)
        {
            siblingButton.transform.SetAsLastSibling();
        }
        siblingButtons.Clear();
        
        
        for (i = 0; i < AchieveManager.Instance.DevelopmentChimeraAchieveInfos.Count; i++)
        {
            Button button = Instantiate(achieveButton, developmentChimeraArchiveContent);
            DevelopmentChimeraAchieveInfosByButton[button] = AchieveManager.Instance.DevelopmentChimeraAchieveInfos[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{AchieveManager.Instance.TotalDevelopmentChimeraCount} / {DevelopmentChimeraAchieveInfosByButton[button].NeedDevelopmentCount}";
            if (AlreadyRewardedDevelopmentChimeraReward(button))
            {
                siblingButtons.Add(button);
                continue;
            }
            button.onClick.AddListener(() => GetDevelopmentChimeraReward(button));
            button.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (var siblingButton in siblingButtons)
        {
            siblingButton.transform.SetAsLastSibling();
        }
        siblingButtons.Clear();

        rewardPanel.SetActive(false);
    }

    private void GetOfficialTestReward(Button button)
    {
        if (OfficialTestAchieveInfosByButton[button].NeedClearStageIndex > AchieveManager.Instance.HighestOfficialTestClearStage) return;
        OfficialTestAchieveInfosByButton[button].SetCleared();
        OfficialTestAchieveInfosByButton[button].GetReward();
        RewardAlert();
        button.transform.SetAsLastSibling();
        button.transform.GetChild(0).gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();
        UiSoundManager.Instance.AcceptSound();
    }

    private bool AlreadyRewardedOfficialTestReward(Button button)
    {
        if (!OfficialTestAchieveInfosByButton[button].IsRewarded) return false;
        button.transform.GetChild(0).gameObject.SetActive(true);
        return true;
    }

    private void GetNonOfficialTestVictoryReward(Button button)
    {
        if(NonOfficialTestVictoryAchieveInfosByButton[button].NeedVictoryCount > AchieveManager.Instance.TotalNonOfficialTestVictoryCount) return;
        NonOfficialTestVictoryAchieveInfosByButton[button].SetCleared();
        NonOfficialTestVictoryAchieveInfosByButton[button].GetReward();
        RewardAlert();
        button.transform.SetAsLastSibling();
        button.transform.GetChild(0).gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();
        UiSoundManager.Instance.AcceptSound();
    }
    
    private bool AlreadyRewardedNonOfficialTestVictoryReward(Button button)
    {
        if (!NonOfficialTestVictoryAchieveInfosByButton[button].IsRewarded) return false;
        button.transform.GetChild(0).gameObject.SetActive(true);
        return true;
    }

    private void GetNonOfficialTestDefeatReward(Button button)
    {
        if(NonOfficialTestDefeatAchieveInfosByButton[button].NeedDefeatCount > AchieveManager.Instance.TotalNonOfficialTestDefeatCount)  return;
        NonOfficialTestDefeatAchieveInfosByButton[button].SetCleared();
        NonOfficialTestDefeatAchieveInfosByButton[button].GetReward();
        RewardAlert();
        button.transform.SetAsLastSibling();
        button.transform.GetChild(0).gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();
        UiSoundManager.Instance.AcceptSound();
    }
    
    private bool AlreadyRewardedNonOfficialTestDefeatReward(Button button)
    {
        if (!NonOfficialTestDefeatAchieveInfosByButton[button].IsRewarded) return false;
        button.transform.GetChild(0).gameObject.SetActive(true);
        return true;
    }

    private void GetReallocationReward(Button button)
    {
        if(ReallocationAchieveInfosByButton[button].NeedReallocationCount > AchieveManager.Instance.TotalReallocationCount)  return;
        ReallocationAchieveInfosByButton[button].SetCleared();
        ReallocationAchieveInfosByButton[button].GetReward();
        RewardAlert();
        button.transform.SetAsLastSibling();
        button.transform.GetChild(0).gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();
        UiSoundManager.Instance.AcceptSound();
    }
    
    private bool AlreadyRewardedReallocationReward(Button button)
    {
        if (!ReallocationAchieveInfosByButton[button].IsRewarded) return false;
        button.transform.GetChild(0).gameObject.SetActive(true);
        return true;
    }

    private void GetDevelopmentChimeraReward(Button button)
    {
        if(DevelopmentChimeraAchieveInfosByButton[button].NeedDevelopmentCount > AchieveManager.Instance.TotalDevelopmentChimeraCount)  return;
        DevelopmentChimeraAchieveInfosByButton[button].SetCleared();
        DevelopmentChimeraAchieveInfosByButton[button].GetReward();
        RewardAlert();
        button.transform.SetAsLastSibling();
        button.transform.GetChild(0).gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();
        UiSoundManager.Instance.AcceptSound();
    }
    
    private bool AlreadyRewardedDevelopmentChimeraReward(Button button)
    {
        if (!DevelopmentChimeraAchieveInfosByButton[button].IsRewarded) return false;
        button.transform.GetChild(0).gameObject.SetActive(true);
        return true;
    }

    private void RewardAlert()
    {
        UiSoundManager.Instance.InDigitalSound();
        rewardPanel.SetActive(true);
    }

    public void CloseRewardAlert()
    {
        UiSoundManager.Instance.InDigitalSound();
        rewardPanel.SetActive(false);
    }

    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }
    
    public void GoBack()
    {
        UiSoundManager.Instance.SceneTransitionSound();
        ProjectSceneManager.Instance.CallAdditiveScene(3);
    }
}
