using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private List<Sprite> spritesWeath;
    [SerializeField] private List<Sprite> spritesTime;
    [SerializeField] protected List<Sprite> spritesEmojis;
    [SerializeField] protected GameObject iconPlayer;
    [SerializeField] protected GameObject statePlayer;
    [SerializeField] protected GameObject nameText;
    [SerializeField] protected GameObject idText;

    [SerializeField] protected DateTime dateTime;
    protected TimeOfDay timeOfDay;

    protected int day = 0;
    protected string nameS = "-1";

    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static StateManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    //

    int lv = -1;
    int exp = -1;

    private void Start()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    private void Update()
    {
        dateTime = DateTime.Now;

        if (dateTime.Hour >= 6 && dateTime.Hour < 12)
        {
            statePlayer.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spritesWeath[0];
            statePlayer.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = spritesTime[0];
            timeOfDay = TimeOfDay.Morning;
        }
        else if (dateTime.Hour >= 12 && dateTime.Hour < 18)
        {
            statePlayer.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spritesWeath[1];
            statePlayer.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = spritesTime[1];
            timeOfDay = TimeOfDay.Afternoon;
        }
        else
        {
            statePlayer.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spritesWeath[2];
            statePlayer.transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = spritesTime[2];
            timeOfDay = TimeOfDay.Evening;
        }
        if (day != dateTime.Day)
        {
            day = dateTime.Day;
            statePlayer.transform.GetChild(0).GetChild(4).GetComponent<TMPro.TMP_Text>().text = dateTime.ToString("MMM") + "." + day.ToString();
        }

        if (nameS != player.GetComponent<Player>().playerName)
        {
            nameS = player.GetComponent<Player>().playerName;
            nameText.GetComponent<TMP_Text>().text = nameS;
            statePlayer.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = player.GetComponent<Player>().playerName;
        }

        if (lv != player.GetComponent<Player>().level)
        {
            lv = player.GetComponent<Player>().level;
            statePlayer.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = lv.ToString();
        }

        if (exp != player.GetComponent<Player>().exp)
        {
            exp = player.GetComponent<Player>().exp;
            statePlayer.transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<Slider>().value = (float)exp / player.GetComponent<Player>().expNeed;
        }
    }

    public void ChangeIconPlayer()
    {
        GameObject bt = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        foreach (var spritesEmoji in spritesEmojis)
        {

            if (spritesEmoji.name == bt.GetComponent<Image>().sprite.name)
            {
                Debug.Log(spritesEmojis.IndexOf(spritesEmoji));
                statePlayer.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Animator>().SetFloat("Statut", (float)(spritesEmojis.IndexOf(spritesEmoji)) / 14);
                break;
            }
        }
    }
    public void ToggleSlot()
    {
        if (gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            OnCloseInventory.Invoke();
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            OnShowInventory.Invoke();
            idText.GetComponent<TMP_Text>().text = "ID: " + player.GetComponent<Player>().playerID;
        }
    }

    // lưu idtext.text vào bộ nhớ tạm 
    public void SaveID()
    {
        string idTextValue = idText.GetComponent<TMP_Text>().text;
        GUIUtility.systemCopyBuffer = idTextValue;
        // Debug.Log("ID đã được lưu vào clipboard: " + idTextValue);
    }

    //Log out
    public void LogOut()
    {
        ReadWriteData.instance.SaveDataToFirebase(ReadWriteData.instance.auth.UserId);
        SceneManager.LoadScene("Login");
    }
}

public enum TimeOfDay
{
    Morning,
    Afternoon,
    Evening
}

