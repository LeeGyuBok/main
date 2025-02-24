using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiManager : SceneSingleton<UiManager>
{
    [SerializeField] private Button start;
    [SerializeField] private Button exit;
    [SerializeField] private Button upgrade;
    //텍스트메쉬프로유지유아이이다. 
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI moneyText;

    private int score;
    private int money;
    private int upgradeCost;
    
    public int Money => money;

    //유아이매니저: 업그레이드버튼눌려욧!
    //군인: 그래요? 그럼 이걸 실행시켜주세욧!
    public event Action UpgradeWeapon;
    public int UpgradeCount { get; private set; }

    [SerializeField] private GameObject BossAlarmPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Instance!=null)
        {
            Instance.score = 0;
            Instance.money = 120;
            upgradeCost = 20;

            Instance.scoreText.text = $"Score: {score}";
            Instance.moneyText.text = $"Money: {money} $";
            Instance.upgrade.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade Weapon: {upgradeCost} $";

            UpgradeCount = 0;
        }
        else
        {
            Debug.Log("Critical Error: UiManager");
        }
    }

    public void GameStart()
    {
        Debug.Log(nameof(GameStart));
    }

    public void GameExit()
    {
        Debug.Log(nameof(GameExit));
        #if UNITY_EDITOR
                // 플레이 모드 종료
                EditorApplication.isPlaying = false;
        #else
                // 애플리케이션 종료
                Application.Quit();
        #endif
    }

    public void Upgrade()
    {
        Debug.Log(nameof(Upgrade));
        /*군인 DMG올리는 로직*/
        if (money >= upgradeCost)
        {
            UpgradeWeapon?.Invoke();
            money -= upgradeCost;
            Instance.moneyText.text = $"Money: {money} $";
            upgradeCost *= 2;
            UpgradeCount++;
            Instance.upgrade.GetComponentInChildren<TextMeshProUGUI>().text = $"Upgrade Weapon: {upgradeCost} $"; 
        }
        else
        {
            Debug.Log("not enough money");
        }
        
    }

    public void CalculateMoney(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out ZombieStatus zombie))
        {
            score += zombie.MaxHp;
        }
        else
        {
            Debug.Log("Error");
            score += 100;
        }
        
        Instance.scoreText.text = $"Score: {score}";
    }

    public void Scoring()
    {
        money += 2;
        Instance.moneyText.text = $"Money: {money} $";
    }

    public void SpawnSoldier()
    {
        money -= 60;
        Instance.moneyText.text = $"Money: {money} $";
    }

    public void Alarm()
    {
        BossAlarm();
    }

    private void BossAlarm()
    {
        StartCoroutine(FadeInAndFadeOut(BossAlarmPanel, 0.4f, 0.01f));
    }

    /// <summary>
    /// yield return value is 0.02f.
    /// </summary>
    /// <param name="image">Gameobject having image component what u want to control Alpha</param>
    /// <param name="maxAlphaValue">0~1, float</param>
    /// <param name="coefficient">increase Or decrease amount</param>
    /// <returns></returns>
    private IEnumerator FadeInAndFadeOut(GameObject image, float maxAlphaValue, float coefficient)
    {
        image.SetActive(true);
        if (image.TryGetComponent(out Image targetImage))
        {
            float maxAlpha = 0;
            while (maxAlpha <= maxAlphaValue)
            {
                maxAlpha += coefficient;
                Color colorAlpha = targetImage.color;
                colorAlpha.a = maxAlpha;
                targetImage.color = colorAlpha;//알파값을 변경한 컬러를 다시 할당해줘야합니다.
                yield return new WaitForSeconds(0.02f);
            }
            while (maxAlpha > 0)
            {
                maxAlpha -= coefficient;
                Color colorAlpha = targetImage.color;
                colorAlpha.a = maxAlpha;
                targetImage.color = colorAlpha;
                yield return new WaitForSeconds(0.02f);
            } 
        }
        image.SetActive(false);
    }
}
