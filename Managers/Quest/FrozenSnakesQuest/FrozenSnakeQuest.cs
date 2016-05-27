using UnityEngine;
using System.Collections;

public class FrozenSnakeQuest : MonoBehaviour
{

    public GameObject ThatisNoIceberg;
    public GameObject[] frozenSnakeQuest;
    int cQuestIndex = 0;
    // Use this for initialization
    void Start()
    {
        if (frozenSnakeQuest.Length < 1)
        {
            Debug.LogError("No objectives set.");
            return;
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject == frozenSnakeQuest[1]) // GO TO THE FIRST RECON POINT
        {
            frozenSnakeQuest[1].SetActive(false);
            frozenSnakeQuest[2].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == frozenSnakeQuest[2]) // GO TO THE FIRST RECON POINT
        {
            frozenSnakeQuest[2].SetActive(false);
            frozenSnakeQuest[3].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == frozenSnakeQuest[3]) // GO TO THE FIRST RECON POINT
        {
            frozenSnakeQuest[3].SetActive(false);
            frozenSnakeQuest[4].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == frozenSnakeQuest[4]) // GO TO THE FIRST RECON POINT
        {
            frozenSnakeQuest[4].SetActive(false);
            frozenSnakeQuest[5].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == frozenSnakeQuest[5]) // GO TO THE FIRST RECON POINT
        {
            print("you beat this quest");
            ThatisNoIceberg.SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
    }
}