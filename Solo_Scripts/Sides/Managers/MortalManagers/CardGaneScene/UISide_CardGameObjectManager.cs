using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameResult
{
    Draw,
    Lose,
    Win
}


public class UISide_CardGameObjectManager : MortalManager<UISide_CardGameObjectManager>
{
    [SerializeField] private GameObject cardPrefab;
    [FormerlySerializedAs("npcDeckPosition")] [SerializeField] private GameObject npcDeck;
    [FormerlySerializedAs("playerDeckPosition")] [SerializeField] private GameObject playerDeck;
    
    [SerializeField] private List<Sprite> heartSprites;
    [SerializeField] private List<Sprite> diamondSprites;
    [SerializeField] private List<Sprite> spadeSprites;
    [SerializeField] private List<Sprite> cloverSprites;
    
    [SerializeField] private RectTransform npcHandsPosition;
    [SerializeField] private RectTransform playerHandsPosition;

    [SerializeField] private Canvas objectCanvas;

    [SerializeField] private float cardMoveSpeed;
    
    [SerializeField] private Material mainCardMaterial;
    [SerializeField] private Material secondaryCardMaterial;
    

    //랜덤 문양 해주기
    private Dictionary<int, List<Sprite>> cardSprites;
    private List<Sprite> selectedPlayerSprites;
    private List<Sprite> selectedNpcSprites;
    
    //10장의 수를 넣을 덱리스트
    public List<int> PlayerDeckList { get; private set; }
    public List<int> NpcDeckList{ get; private set; }
    
    //카드오브젝트
    private List<GameObject> playerGameObjectHands = new List<GameObject>();
    private List<GameObject> npcGameObjectHands = new List<GameObject>();
    
    //게임 참가자들이 들고있는 밸류값.
    public List<int> PlayerHands{ get; private set; }
    public List<int> NpcHands{ get; private set; }

    private bool IsCardMoving;

    public void ReGame()
    {
        cardSprites = null;
        selectedPlayerSprites = null;
        selectedNpcSprites = null;

        PlayerDeckList = null;
        NpcDeckList = null;

        PlayerHands = null;
        NpcHands = null;
        
        for (int i = 0; i < playerGameObjectHands.Count; i++)
        {
            DestroyImmediate(playerGameObjectHands[i]);
            DestroyImmediate(npcGameObjectHands[i]);
        }
        playerGameObjectHands.Clear();
        npcGameObjectHands.Clear();
        
        PlayerCards.Clear();
        NpcCards.Clear();
        PlayerScore = 0;
        NpcScore = 0;
        
        SatInitialCard = false;
        IsTurnOverCard = false;
        SetAdditionalCard = false;

        EndGame = false;
        gameResult = GameResult.Draw;
        Start();
    }
    
    //카드의 순서
    public Dictionary<int, GameObject> PlayerCards { get; private set; } = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> NpcCards { get; private set; } = new Dictionary<int, GameObject>();
    
    public int PlayerScore { get; private set; }
    public int NpcScore { get; private set; }
    
    public bool SatInitialCard { get; private set; }
    public bool SetAdditionalCard { get; private set; }
    
    public bool IsTurnOverCard { get; private set; }
    
    public bool EndGame { get; private set; }


    private GameResult gameResult;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        cardSprites = new Dictionary<int, List<Sprite>>()
        {
            { 0, heartSprites },
            { 1, diamondSprites },
            { 2, spadeSprites },
            { 3, cloverSprites }
        };
        
        List<int> keys = new List<int>(cardSprites.Keys); // Dictionary의 모든 키 가져오기

        int randomIndex = Random.Range(0, keys.Count); // 0부터 키 개수 - 1까지 랜덤 선택
        int selectedKey = keys[randomIndex];
        selectedPlayerSprites = new List<Sprite>(cardSprites[selectedKey]); // 선택된 키의 값 가져오기
        cardSprites.Remove(selectedKey); // 사용한 키 제거

        keys = new List<int>(cardSprites.Keys); // 남은 키 목록 갱신
        randomIndex = Random.Range(0, keys.Count); // 새 랜덤 키 선택
        selectedKey = keys[randomIndex];
        selectedNpcSprites = new List<Sprite>(cardSprites[selectedKey]); // NPC를 위한 값 가져오기
        cardSprites.Remove(selectedKey);
        /*SettingCardGame();
        PickAndSortCards();
        ShowSecondLargeCard();
        AddNewCardFromOpponentCardList();
        ShowResult();*/
        SettingCardGame();
        Debug.Log("Card game started");
        //StartCoroutine(CardMove(card, playerHandsPosition[0].transform.position));
        //Debug.Log(card.GetComponent<RectTransform>().transform.localPosition);
    }

    public void SettingCardGame()
    {
        //카드 섞는 모션. 카드뭉치 등..
        PlayerDeckList = new List<int>();
        NpcDeckList = new List<int>();
        for (int i = 1; i < 11; i++)
        {
            PlayerDeckList.Add(i);
            NpcDeckList.Add(i);
        }
        PlayerHands = new List<int>();
        NpcHands = new List<int>();
    }

    public void PickAndSortCards()
    {
        if (SatInitialCard) return;
        //카드 나눠주는 모션
        for (int i = 0; i < 3; i++)
        {
            CardToPlayer(PlayerDeckList, PlayerHands,  playerDeck, playerHandsPosition, playerGameObjectHands, PlayerCards, i);
            /*int randomIndex = Random.Range(0, PlayerDeckList.Count);
            PlayerHands.Add(PlayerDeckList[randomIndex]);
            PlayerDeckList.RemoveAt(randomIndex);
            GameObject playerCard = PickAndSortCardsAnim(playerDeck, playerHandsPosition, i);
            playerGameObjectHands.Add(playerCard);
            PlayerCards[i] = playerCard;*/
            
            CardToPlayer(NpcDeckList, NpcHands,  npcDeck, npcHandsPosition, npcGameObjectHands, NpcCards, i);
            /*randomIndex = Random.Range(0, NpcDeckList.Count);
            NpcHands.Add(NpcDeckList[randomIndex]);
            NpcDeckList.RemoveAt(randomIndex);
            GameObject npcCard = PickAndSortCardsAnim(npcDeck, npcHandsPosition, i);
            npcGameObjectHands.Add(npcCard);
            NpcCards[i] = npcCard;*/
        }
        
        PlayerHands = PlayerHands.OrderBy(card => card).ToList();
        NpcHands = NpcHands.OrderBy(card => card).ToList();

        for (int i = 0; i < 3; i++)
        {
            //StartCoroutine(TurnOverCard(PlayerCards, i, PlayerHands, selectedPlayerSprites));
            PlayerCards[i].GetComponent<Image>().material = secondaryCardMaterial;
            StartCoroutine(CardEffector(PlayerCards, i, PlayerHands, selectedPlayerSprites));
        }
        
        SatInitialCard = true;
    }

    private void CardToPlayer(List<int> userDeckList, List<int> userHands, GameObject userDeckObject, RectTransform userHandPosition, 
        List<GameObject> userGameObjectHands, Dictionary<int, GameObject> userCards, int index)
    {
        int randomIndex = Random.Range(0, userDeckList.Count);
        userHands.Add(userDeckList[randomIndex]);
        userDeckList.RemoveAt(randomIndex);
        GameObject playerCard = PickAndSortCardsAnim(userDeckObject, userHandPosition, index);
        userGameObjectHands.Add(playerCard);
        userCards[index] = playerCard;
    }
    
    private GameObject PickAndSortCardsAnim(GameObject deckPosition, RectTransform handPosition, int handIndex)
    {
        //Ui도 이동할 때, 월드포지션으로 하자. 로컬포지션은 부모의 영향을 받아서 난리난다.
        GameObject card = Instantiate(cardPrefab, objectCanvas.transform);//오브젝트 캔버스에 생성하고
        card.transform.position = deckPosition.transform.position;//위치를 덱 포지션으로 잡는다.
        card.transform.rotation = deckPosition.transform.rotation;//회전 정도도 덱 포지션으로 잡는다.
        StartCoroutine(CardMove(card, handPosition.GetComponent<RectTransform>().GetChild(handIndex).gameObject));
        return card;
    }

    private void TurnOverSecondaryCards()
    {
        NpcCards[0].GetComponent<Image>().material = secondaryCardMaterial;
        NpcCards[2].GetComponent<Image>().material = secondaryCardMaterial;
        StartCoroutine(CardEffector(NpcCards, 1, NpcHands, selectedNpcSprites));
        //StartCoroutine(TurnOverCard(NpcCards, 1, NpcHands, selectedNpcSprites));
        Debug.Log($"PlayerSecondaryCard: {PlayerHands[1]}, NpcSecondaryCard: {NpcHands[1]}");
    }
    
    /// <summary>
    /// npc only
    /// </summary>
    public void ShowSecondLargeCard()
    {
        //카드 뒤집는 모션 
        if (IsTurnOverCard || IsCardMoving) return;
        TurnOverSecondaryCards();
        IsTurnOverCard = true;
    }
    
    public void AddNewCardFromOpponentCardList()
    {
        NpcCards[1].GetComponent<Image>().material = secondaryCardMaterial;
        if (SetAdditionalCard || IsCardMoving) return;
     
        //추가 카드는 상대의 덱에서 내 손으로 가져온다.
        CardToPlayer(PlayerDeckList, NpcHands, playerDeck, npcHandsPosition, npcGameObjectHands, NpcCards, 3);
        NpcCards[3].GetComponent<Image>().material = secondaryCardMaterial;
        /*int randomIndex = Random.Range(0, PlayerDeckList.Count);
        NpcHands.Add(PlayerDeckList[randomIndex]);
        PlayerDeckList.RemoveAt(randomIndex);
        npcGameObjectHands.Add(PickAndSortCardsAnim(playerDeck, npcHandsPosition, 3));*/
        
        CardToPlayer(NpcDeckList, PlayerHands, npcDeck, playerHandsPosition, playerGameObjectHands, PlayerCards, 3);
        //StartCoroutine(TurnOverCard(PlayerCards, 3, PlayerHands, selectedNpcSprites));
        //PlayerCards[3].GetComponent<Image>().material = secondaryCardMaterial;
        StartCoroutine(CardEffector(PlayerCards, 3, PlayerHands, selectedNpcSprites));
        /*randomIndex = Random.Range(0, NpcDeckList.Count);
        PlayerHands.Add(NpcDeckList[randomIndex]);
        NpcDeckList.RemoveAt(randomIndex);
        playerGameObjectHands.Add(PickAndSortCardsAnim(npcDeck, playerHandsPosition, 3));
        SetAdditionalCard = true;*/
        SetAdditionalCard = true;
        Debug.Log($"{PlayerHands[3]}, {NpcHands[3]}");
    }

    public void ShowResult()
    {
        if (EndGame || IsCardMoving) return;
        
        //결과보기
        if (PlayerHands.Count != NpcHands.Count)
        {
            Debug.Log("critical error in each hands");
            return;
        }
        for (int i = 0; i < PlayerHands.Count; i++)
        {
            if (i != 1)
            {
                NpcCards[i].GetComponent<Image>().material = mainCardMaterial;    
            }
            PlayerCards[i].GetComponent<Image>().material = mainCardMaterial;
            
            
            PlayerScore += PlayerHands[i];
            NpcScore += NpcHands[i];
        }
        Debug.Log($"{PlayerScore}, {NpcScore}");
        for (int i = 0; i < NpcHands.Count; i++)
        {
            /*StartCoroutine(i == NpcHands.Count - 1
                ? TurnOverCard(NpcCards, i, NpcHands, selectedPlayerSprites)
                : TurnOverCard(NpcCards, i, NpcHands, selectedNpcSprites));*/
            StartCoroutine(i == NpcHands.Count - 1
                ? CardEffector(NpcCards, i, NpcHands, selectedPlayerSprites)
                : CardEffector(NpcCards, i, NpcHands, selectedNpcSprites));
        }

        if (PlayerScore > NpcScore)
        {
            gameResult = GameResult.Win;
            Debug.Log(gameResult);
            UISide_CardGameUiManager.Instance.SetButtonByResult(gameResult);
            return;
        }

        if (PlayerScore < NpcScore)
        {
            gameResult = GameResult.Lose;
            Debug.Log(gameResult);
            UISide_CardGameUiManager.Instance.SetButtonByResult(gameResult);
            return;
        }
        gameResult = GameResult.Draw;
        UISide_CardGameUiManager.Instance.SetButtonByResult(gameResult);
        EndGame = true;
        Debug.Log(gameResult);
    }

    /// <summary>
    /// move with position
    /// </summary>
    /// <param name="card"></param>
    /// <param name="targetObject"></param>
    /// <returns></returns>
    private IEnumerator CardMove(GameObject card, GameObject targetObject)
    {
        Vector3 startPosition = card.transform.position;
        Vector3 targetPosition = targetObject.transform.position;
        Quaternion startRotation = card.transform.rotation; // 퀘터니언 사용
        Quaternion targetRotation = Quaternion.Euler(targetObject.transform.rotation.eulerAngles);
        //Vector3 direction = (targetObject - card.GetComponent<RectTransform>().localPosition).normalized;
        while (targetPosition != startPosition)
        {
            card.transform.position = Vector3.MoveTowards(card.transform.position, targetPosition, cardMoveSpeed*0.8f * Time.fixedDeltaTime);
            //Quaternion.RotateTowards(card.transform.rotation, targetRotation, 20f * Time.fixedDeltaTime);
            startPosition = card.transform.position;
            yield return null;
        }

        while (targetRotation != startRotation)
        {
            card.transform.rotation = Quaternion.RotateTowards(card.transform.rotation, targetRotation, cardMoveSpeed * 1.5f * Time.fixedDeltaTime);
            startRotation = card.transform.rotation;
            yield return null;
        }
    }

    //카드 뒤집기
    private IEnumerator TurnOverCard(Dictionary<int, GameObject> userCards, int index, List<int> userHands, List<Sprite> userSprites)
    {
        GameObject card = userCards[index];
        Quaternion startRotation = card.transform.rotation; // 퀘터니언 사용
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        yield return new WaitForSeconds(1f);
        while (targetRotation != startRotation)
        {
            card.transform.rotation = Quaternion.RotateTowards(card.transform.rotation, targetRotation, cardMoveSpeed * 0.3f * Time.fixedDeltaTime);
            startRotation = card.transform.rotation;
            yield return null;
        }
        //여기서 이미지 설정. 작은 순서대로 정렬된 유저의 핸드에 있는 값.
        card.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        Debug.Log("Turn over end");
        card.GetComponent<Image>().sprite = userSprites[userHands[index]-1];
        /*targetRotation = Quaternion.Euler(new Vector3(-180f, 0f, 0f));
        while (targetRotation != startRotation)
        {
            card.transform.rotation = Quaternion.RotateTowards(card.transform.rotation, targetRotation, cardMoveSpeed * 0.3f * Time.fixedDeltaTime);
            startRotation = card.transform.rotation;
            yield return null;
        }*/
    }
    
    //쉐이더 적용하기
    private IEnumerator CardEffector(Dictionary<int, GameObject> userCards, int index, List<int> userHands, List<Sprite> userSprites)
    {
        IsCardMoving = true;
        Image cardImage = userCards[index].GetComponent<Image>();
        Material cardMaterial = cardImage.material;
        yield return new WaitForSeconds(1f);
        float value = cardMaterial.GetFloat(Shader.PropertyToID("_SplitValue"));
        while (value > 0)
        {
            value -= Time.deltaTime;
            cardMaterial.SetFloat("_SplitValue", value);
            yield return null;
        }
        cardImage.sprite = userSprites[userHands[index]-1];
        while (value < 1)
        {
            value += Time.deltaTime;
            cardMaterial.SetFloat("_SplitValue", value);    
            yield return null;
        }

        IsCardMoving = false;
    }
}
