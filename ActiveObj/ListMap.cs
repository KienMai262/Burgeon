using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListMap : MonoBehaviour
{
    [SerializeField] public List<bool> mapList;
    private void Awake()
    {
        mapList = new List<bool>();
        for (int i = 0; i < transform.childCount; i++)
        {
            mapList.Add(false);
        }
    }


}
