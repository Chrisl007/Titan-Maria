using UnityEngine;
using System.Collections;

public class QuestObjective : MonoBehaviour
{

    public enum ObjectiveType
    {
        Defend,
        //Survive, basically the same as defend
        //Discover, this is same as go currently
        Scan,
        Go//, Could not find way to distinguish Go and follow
        //Following
    }

    public string ObjectiveName;
    public string ObjectiveDescription;
    public ObjectiveType Type;
    public GameObject scanObject;
    public float GoDistance;
    public bool IsComplete = false;
    Transform _trans;
    // Use this for initialization
    void Start()
    {
        _trans = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentQuest != null)
        {
            if (GameManager.Instance.CurrentQuest.CurrentObjective == this)
            {
                switch (Type)
                {
                    // go/follow/discover type quest
                    case ObjectiveType.Go:
                        float dist = Vector3.Distance(_trans.position, TitanPlayerState.Instance.myTransform.position);
                        if (dist <= GoDistance)
                            IsComplete = true;
                        break;

                    //defend quest type Currently due to implementation method this is an empty state
                    //Reason for empty is to provide logical interface to level designers
                    case ObjectiveType.Defend:
                        break;

                    //currently empty as it is dependent on team member creating method for game object refernece
                    case ObjectiveType.Scan:
                        if(scanObject.activeInHierarchy == true)
                            IsComplete = true;
                            //print("Success");
                        break;
                }

                //print("hi");
            }
        }

    }
}
