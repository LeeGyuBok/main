using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode
{
    public string sentence; // 현재 노드의 대사
    public List<DialogueNode> responses; // 다음 노드 또는 플레이어 선택지
    public bool isEndNode; // 대화가 종료되는 노드인지 표시

    public DialogueNode(string sentence, List<DialogueNode> responses, bool isEndNode = false)
    {
        this.sentence = sentence;
        this.responses = responses;
        this.isEndNode = isEndNode;
    }
}

public class CommanderCharacter : ITrustable
{
    #region ITrustable

    public TrustableData TrustData { get;}
    public bool IsInitialized { get; private set; }
    public int CurrentTrust { get; private set; }
    
    public void AddTrust(int amount)
    {
        CurrentTrust += amount;
    }

    public void RemoveTrust(int amount)
    {
        CurrentTrust -= amount;
        if (CurrentTrust < 0)
        {
            CurrentTrust = 0;
        }
    }

    public void SetJsonTrustData(int jsonTrust)
    {
        throw new System.NotImplementedException();
    }

    public void SetInitialTrustData()
    {
        throw new System.NotImplementedException();
    }

    #endregion
    
    public CommanderCharacter(TrustableData data)
    {
        this.TrustData = data;
        CurrentTrust = data.BaseTrust;
        IsInitialized = true;
        startDialogueNode = new DialogueNode("hi, playerName", new List<DialogueNode>());
    }
    
    public DialogueNode startDialogueNode { get; private set; }

    public DialogueNode SetDialogueNode(Queue<string> playerActivationLogQueue)
    {
        Queue<string> logQueue = new Queue<string>(playerActivationLogQueue);
        /*for (int i = 0; i < logQueue.Count; i++)
        {
            Debug.Log(logQueue.Dequeue());
        }

        logQueue = new Queue<string>(playerActivationLogQueue);*/
        
        //여기서 디큐하면서 1이 아니니까 그 밑으로 내려가버렸네?
        DialogueNode middleNode1 = new DialogueNode($"some questions about {logQueue.Dequeue()}, first", new List<DialogueNode>());
        startDialogueNode.responses.Add(middleNode1);
        //Sight - M1
        //여기가 버튼화 되는 노드들
        DialogueNode treeNode1 = new DialogueNode("good", new List<DialogueNode>());
        DialogueNode treeNode2 = new DialogueNode("not bad", new List<DialogueNode>());
        DialogueNode treeNode3 = new DialogueNode("bad", new List<DialogueNode>());
        
        middleNode1.responses.Add(treeNode1);
        middleNode1.responses.Add(treeNode2);
        middleNode1.responses.Add(treeNode3);
        if (logQueue.Count == 0)
        {
            DialogueNode endNode1 = new DialogueNode("end", new List<DialogueNode>(), true);
        
            treeNode1.responses.Add(endNode1);
            treeNode2.responses.Add(endNode1);
            treeNode3.responses.Add(endNode1);
            return startDialogueNode;
            //Sight - M1 - 3 - E
        }
        //Sight - M1 - 3
        
        
        
        DialogueNode middleNode2 = new DialogueNode($"ok. next. {logQueue.Dequeue()}", new List<DialogueNode>());
        treeNode1.responses.Add(middleNode2);
        treeNode2.responses.Add(middleNode2);
        treeNode3.responses.Add(middleNode2);
        //Sight - M1 - 3 - M2
        
        DialogueNode secTreeNode1 = new DialogueNode("good", new List<DialogueNode>());
        DialogueNode secTreeNode2 = new DialogueNode("not bad", new List<DialogueNode>());
        DialogueNode secTreeNode3 = new DialogueNode("bad", new List<DialogueNode>());
        
        middleNode2.responses.Add(secTreeNode1);
        middleNode2.responses.Add(secTreeNode2);
        middleNode2.responses.Add(secTreeNode3);
        
        if (logQueue.Count == 0)
        {
            DialogueNode endNode1 = new DialogueNode("end", new List<DialogueNode>(), true);
        
            secTreeNode1.responses.Add(endNode1);
            secTreeNode2.responses.Add(endNode1);
            secTreeNode3.responses.Add(endNode1);
            return startDialogueNode;
            //Sight - M1 - 3 - M2 - 3 - E
        }
        //Sight - M1 - 3 - M2 - 3
        
        DialogueNode middleNode3 = new DialogueNode($"ok. last. {logQueue.Dequeue()}", new List<DialogueNode>());
        secTreeNode1.responses.Add(middleNode3);
        secTreeNode2.responses.Add(middleNode3);
        secTreeNode3.responses.Add(middleNode3);
        
        //Sight - M1 - 3 - M2 - 3 - M3
        
        DialogueNode thirdTreeNode1 = new DialogueNode("good", new List<DialogueNode>());
        DialogueNode thirdTreeNode2 = new DialogueNode("not bad", new List<DialogueNode>());
        DialogueNode thirdTreeNode3 = new DialogueNode("bad", new List<DialogueNode>());
        
        middleNode3.responses.Add(thirdTreeNode1);
        middleNode3.responses.Add(thirdTreeNode2);
        middleNode3.responses.Add(thirdTreeNode3);
        //Sight - M1 - 3 - M2 - 3 - M3 - 3
        
        DialogueNode endNode = new DialogueNode("end", new List<DialogueNode>(), true);
        
        thirdTreeNode1.responses.Add(endNode);
        thirdTreeNode2.responses.Add(endNode);
        thirdTreeNode3.responses.Add(endNode);
        
        //Sight - M1 - 3 - M2 - 3 - M3 - 3 - E

        return startDialogueNode;
    }

    public void InitializeDialogueNode()
    {
        startDialogueNode = new DialogueNode("hi, playerName", new List<DialogueNode>());
    }

}
