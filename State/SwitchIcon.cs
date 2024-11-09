using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchIcon : MonoBehaviour
{
    [SerializeField] public GameObject icon;
    private void Start() {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    

    public void ToggleSwitchIcon()
    {
        Debug.Log("ToggleSwitchIcon");
        if (gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }


}
