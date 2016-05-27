using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    //GameObject questDialog;
    public Quest CurrentQuest;
    public Image questDialog;
    //public TitanPlayerState Player;
	// Use this for initialization
	void Start () {
       questDialog = GameObject.Find("QuestDialog").GetComponent<Image>();
	   Instance = this;
       
	}
	
	// Update is called once per frame
	void Update () 
    {
	   if(CurrentQuest == null)
       {
           questDialog.enabled = false;
       }
       
       else
       {
           questDialog.enabled = true;
       }
	}
}
