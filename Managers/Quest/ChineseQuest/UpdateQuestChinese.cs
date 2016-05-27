using UnityEngine;
using System.Collections;

public class UpdateQuestChinese : MonoBehaviour {
    
    public GameObject radioStatic;
    // Use this for initialization
    void Start () 
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            radioStatic.GetComponent<AudioSource>().enabled = false;
                //enabled(false);
                //SetActive(false);
        }
        
    }
    // Update is called once per frame
    void Update () {
	
	}
}