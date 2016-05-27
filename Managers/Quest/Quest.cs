using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class Quest : MonoBehaviour
{
    /*[XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Type")]
    public string questType;

    [XmlElement("Description")]
    public string description;

    [XmlElement("Parts")]
    public string parts;*/
    
    public enum Types
    {
        Defend,
        Survive,
        Discover,
        Go,
        Following
    }
    
    public int QuestId = 1;
    public string QuestName;
    public string Description;
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public QuestObjective CurrentObjective;
    QuestManager questReference;
    public Quest Predecessor;
    public Quest Successor;
    public bool IsComplete;
    public bool IsInProgress;
    
    void Awake()
    {
        //questIndex = GetComponent<QuestManager>(); //.Instance.currentQuest;
        //the one below is better!!!!
        
    }

    void Start()
    {
        AssignValues();
        gameObject.SetActive(CanPlay());
    }
    
    void Update()
    {
        if(GameManager.Instance.CurrentQuest == this)
        {
            if(CurrentObjective.IsComplete == true)
            {
                int CurObjIndex = objectives.IndexOf(CurrentObjective);
                if (CurObjIndex == objectives.Count-1)
                { 
                    OnCompleteQuest();
                }
                   
                else
                {
                    CurrentObjective = objectives[CurObjIndex +1];
                    CurrentObjective.IsComplete = false;
                    DialogManager.Instance.ObjectiveDescription.text = CurrentObjective.ObjectiveName;
                }
            }
        }
    }
    
    void AssignValues() 
    {
        questReference = GameObject.Find("QuestManager").GetComponent<QuestManager>();

        /*****Object Reference Test********* /
        int test;
        test = questReference.currentQuest;
        print(test);
        // *********************************/
        
        int i = QuestId;
        string questName = "";
        string description = "";
        //testName = questReference.quests[i-1].ToString();
        questReference.GetQuests();
        questReference.GetCurrentQuest();
        questReference.quests[i - questReference.currentQuest].TryGetValue("Name", out questName);
        questReference.quests[i - questReference.currentQuest].TryGetValue("Description", out description);
        print(questName);
	    print(description); 
        QuestName = questName;
        Description = description;
        //foreach (var s in questIndex.quests)
        //TryGetValue("Name", out QuestName);
        //print(s.TryGetValue("Name", out QuestName));
    }
    
    public bool CanPlay()
    {
        if (!IsComplete)
        {
            //check for predeccessor
            if (Predecessor != null)
            {
                //return true if predeccessor is complete
                if (Predecessor.IsComplete)
                    return true;
                else 
                    return false;
            }
            return true;
            //print("Complete");
        }
        else
        {
            //return false quest is complete or unable to start
            return false;
        }
    }
    
    public void OnStartQuest()
    {
        //Quest is started and in progress
        IsInProgress = true;
        IsComplete = false;
        CurrentObjective = objectives[0];
        GameManager.Instance.CurrentQuest = this;

        //set UI to active
        //DialogManager.Instance.QuestDialog.gameObject.SetActive(true);
        DialogManager.Instance.QuestName.gameObject.SetActive(true);
        DialogManager.Instance.ObjectiveDescription.gameObject.SetActive(true);

        //Get Fields for UI
       
        DialogManager.Instance.QuestName.text = QuestName;
        DialogManager.Instance.ObjectiveDescription.text = CurrentObjective.ObjectiveName;
            
    }
    
    public void OnCompleteQuest()
    {
        //Quest is complete
        IsInProgress = false;
        IsComplete = true;
        GameManager.Instance.CurrentQuest = null;
        

        //set UI to off
        //DialogManager.Instance.QuestDialog.gameObject.SetActive(false);
        DialogManager.Instance.QuestName.gameObject.SetActive(false);
        DialogManager.Instance.ObjectiveDescription.gameObject.SetActive(false);
    }
    
    /**/ void OnTriggerEnter(Collider other)
    {
        if(CanPlay())
        {
            if(other.CompareTag("Player"))
            {
                if(!IsInProgress && !IsComplete)
                OnStartQuest();
            }
            
        }
    }
}
