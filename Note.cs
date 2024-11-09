using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] public GameObject text;
    [SerializeField] protected GameObject button;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ToggleNote(){
        if(gameObject.activeSelf){
            gameObject.SetActive(false);
        }else{
            gameObject.SetActive(true);
        }
    }
}
