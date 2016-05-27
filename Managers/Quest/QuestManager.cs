using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


public class QuestManager : MonoBehaviour
{
    public TextAsset questAsset;
    public int currentQuest = 1;

    /*static string Cube_Character = "";
    static string Cylinder_Character = "";
    static string Capsule_Character = "";
    static string Sphere_Character = "";*/
    //string questName = "";

    private GameObject QuestName;
    public List<Dictionary<string, string>> quests = new List<Dictionary<string, string>>();//this list/ditionary Stores ALL QUESTS!!!!!!!
    Dictionary<string, string> obj;//This turns elements into objects to be stored in the dictionary!!!
    public QuestManager Instance;
    void Awake()
    {
      //questAsset = Resources.Load<TextAsset>("Quest");
    }
    void Start()
    { 
        //Timeline of the Level creator
        GetQuests();
        GetCurrentQuest();
        Instance = this;
    }
    void Update()
    {
        UpdateCurrentQuest();
        //GetCurrentQuest();
    }
    public void GetQuests()
    {
        XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
        xmlDoc.LoadXml(questAsset.text); // load the file.
        XmlNodeList questList = xmlDoc.GetElementsByTagName("Quest"); // array of the sid quest nodes.
        //XmlNodeList subQuestList = xmlDoc.GetElementsByTagName("SideQuest"); // array of the side quest nodes
        foreach (XmlNode questInfo in questList)
        {
            XmlNodeList questContent = questInfo.ChildNodes;
            obj = new Dictionary<string, string>(); // Create a object(Dictionary) to colect the both nodes inside the level node and then put into quests[] array.
            // obj = new Dictionary<string, string>(); // Create a object(Dictionary) to colect the both nodes inside the level node and then put into SideQuests[] array.
            foreach (XmlNode questsItems in questContent) // levels itens nodes.
            {
                if (questsItems.Name == "Name")
                {
                    obj.Add("Name", questsItems.InnerText); // put this in the dictionary.
                    //print(questsItems.InnerText);
                }

                if (questsItems.Name == "Type")
                {
                    obj.Add("Type", questsItems.InnerText); // put this in the dictionary.
                }

                if (questsItems.Name == "Description")
                {
	                    obj.Add("Description", questsItems.InnerText);

                    /* switch (questsItems.Attributes["name"].Value)
                     {
                         case "Cube": obj.Add("Cube", questsItems.InnerText); break; // put this in the dictionary.
                         case "Cylinder": obj.Add("Cylinder", questsItems.InnerText); break; // put this in the dictionary.
                         case "Capsule": obj.Add("Capsule", questsItems.InnerText); break; // put this in the dictionary.
                         case "Sphere": obj.Add("Sphere", questsItems.InnerText); break; // put this in the dictionary.
                     }*/
                }

                if (questsItems.Name == "Parts")
                {
                    obj.Add("Parts", questsItems.InnerText); // put this in the dictionary.
                }
            }
            quests.Add(obj); // add whole obj dictionary in the levels[].
            //print(questList.Item(1).InnerText);
        }
    }

    public void GetCurrentQuest()
    {
        string questName = "";
        quests[currentQuest - 1].TryGetValue("Name", out questName);
        string description = "";
        quests[currentQuest - 1].TryGetValue("Description", out description);
        //print(questName);
    }

    public void UpdateCurrentQuest()
    {
        //Update Quest(Prototype)

        if (Input.GetKeyDown("u"))
        {
            //print("changing currentQuest up");
            currentQuest++;
            GetCurrentQuest();
        }
        if (Input.GetKeyDown("i"))
        {
            //print("changing currentQuest down");
            currentQuest--;
            GetCurrentQuest();
        }
    }
}

