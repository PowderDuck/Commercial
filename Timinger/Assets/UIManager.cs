using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager manager;

    [SerializeField] private GameObject modalWindow;
    [SerializeField] private TMPro.TextMeshProUGUI timeText = null;
    [SerializeField] private Image underscore = null;
    [SerializeField] private List<GameObject> underscorePositions = new List<GameObject>();
    public GameObject clockFace = null;

    private List<float> formattedTime = new List<float>();
    private int formatIndex = 0;

    private Dictionary<KeyCode, int> keys = new Dictionary<KeyCode, int>()
    {
        { KeyCode.Alpha0, 0 }, { KeyCode.Alpha1, 1 }, { KeyCode.Alpha2, 2 }, { KeyCode.Alpha3, 3 },
        { KeyCode.Alpha4, 4 }, { KeyCode.Alpha5, 5 }, { KeyCode.Alpha6, 6 }, { KeyCode.Alpha7, 7 },
        { KeyCode.Alpha8, 8 }, { KeyCode.Alpha9, 9 }, { KeyCode.Keypad0, 0 }, { KeyCode.Keypad1, 1 },
        { KeyCode.Keypad2, 2 }, { KeyCode.Keypad3, 3 }, { KeyCode.Keypad4, 4 }, { KeyCode.Keypad5, 5 },
        { KeyCode.Keypad6, 6 }, { KeyCode.Keypad7, 7 }, { KeyCode.Keypad8, 8 }, { KeyCode.Keypad9, 9 },
    };

    private void Awake()
    {
        if(manager == null)
        {
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(modalWindow.activeInHierarchy)
        {
            if(Input.anyKeyDown)
            {
                foreach(KeyCode key in keys.Keys)
                {
                    if(Input.GetKeyDown(key))
                    {
                        formattedTime[formatIndex] = keys[key];
                        UpdateText();
                        formatIndex = Mathf.Min(formatIndex + 1, underscorePositions.Count - 1);
                        underscore.transform.position = underscorePositions[formatIndex].transform.position;
                        break;
                    }
                }
            }
        }
    }

    private void UpdateText()
    {
        string content = "";
        for (int i = 0; i < formattedTime.Count / 2f; i++)
        {
            content += i == 0f ? "" : ":";
            for (int x = 0; x < 2f; x++)
            {
                content += formattedTime[i * 2 + x];
            }
        }

        timeText.text = content;
    }

    public void ChangeTime()
    {
        formatIndex = 0;
        underscore.transform.position = underscorePositions[formatIndex].transform.position;
        formattedTime = new List<float>();
        string[] format = timeText.text.Split(":");

        for (int i = 0; i < format.Length; i++)
        {
            for (int x = 0; x < 2f; x++)
            {
                int addition = 0;
                try
                {
                    addition = int.Parse(format[i][x].ToString());
                }
                catch { }

                formattedTime.Add(addition);
            }
        }

        TimerManager.manager.updating = true;
        modalWindow.SetActive(true);
    }

    public void SetTime(bool cancel = false)
    {
        float seconds = 0f;
        for (int i = 0; i < formattedTime.Count / 2f; i++)
        {
            string digits = "";
            for (int x = 0; x < 2f; x++)
            {
                digits += formattedTime[i * 2 + x];
            }

            int period = 0;
            try
            {
                period = int.Parse(digits);
            }
            catch { }

            seconds += period * TimerManager.manager.timeArrows[i].seconds;
        }

        TimerManager.manager.updating = false;
        TimerManager.manager.SetTime(cancel ? TimerManager.manager.currentTime : seconds);
        modalWindow.SetActive(false);
    }
}