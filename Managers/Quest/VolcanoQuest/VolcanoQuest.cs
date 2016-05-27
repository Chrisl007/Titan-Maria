using UnityEngine;
using System.Collections;

public class VolcanoQuest : MonoBehaviour {

    public GameObject FrozenSnakesofFireQuest;
    public GameObject[] volcanoQuest;
    int cQuestIndex = 0;
    // Use this for initialization
    void Start()
    {
        if (volcanoQuest.Length < 1)
        {
            Debug.LogError("No objectives set.");
            return;
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject == volcanoQuest[1]) // GO TO THE FIRST RECON POINT
        {
            volcanoQuest[1].SetActive(false);
            volcanoQuest[2].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == volcanoQuest[2]) // GO TO THE FIRST RECON POINT
        {
            volcanoQuest[2].SetActive(false);
            volcanoQuest[3].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == volcanoQuest[3]) // GO TO THE FIRST RECON POINT
        {
            volcanoQuest[3].SetActive(false);
            volcanoQuest[4].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == volcanoQuest[4]) // GO TO THE FIRST RECON POINT
        {
            volcanoQuest[4].SetActive(false);
            volcanoQuest[5].SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
        if (col.gameObject == volcanoQuest[5]) // GO TO THE FIRST RECON POINT
        {
            print("you beat this quest");
            FrozenSnakesofFireQuest.SetActive(true);
            //PrimaryObjective2.SetActive(true);
            //text_recon_point_reached.SetActive(true);
        }
    }
}