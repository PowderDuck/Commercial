using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class TimerManager : MonoBehaviour
{
    public static TimerManager manager;

    [SerializeField] private float synchronizationTime = 3600f;
    [SerializeField] private float updateTime = 0.1f;
    public List<Arrow> timeArrows = new List<Arrow>();
    [SerializeField] private TMPro.TextMeshProUGUI timeText = null;

    public bool dragging = false;
    public bool updating = false;

    public float currentTime = 0f;
    private float currentUpdateTime = 0f;
    public Vector2 cameraDimensions = Vector2.zero;
    private float currentSynchronizationTime = 0f;

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

    private void Start()
    {
        cameraDimensions = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        currentSynchronizationTime = synchronizationTime - Time.deltaTime;
    }

    /*private async Task<string> GetCurrentTime()
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://yandex.com/time/sync.json");
        string result = await response.Content.ReadAsStringAsync();

        return result;
    }*/

    private IEnumerator GetCurrentTime()
    {
        UnityWebRequest req = UnityWebRequest.Get("https://yandex.com/time/sync.json");
        yield return req.SendWebRequest();

        string cTime = req.downloadHandler.text;
        Debug.Log(cTime);
        OnlineTime onlineTime = JsonUtility.FromJson<OnlineTime>(cTime);
        currentTime = Mathf.Floor(onlineTime.time / 1000f) % 86400f;
        UpdateVisuals(currentTime);
        Debug.Log(currentTime);
    }

    private void Update()
    {
        Synchronize();
        Timer();
    }

    private async void Synchronize()
    {
        if(currentSynchronizationTime < synchronizationTime)
        {
            currentSynchronizationTime += Time.deltaTime;
            currentSynchronizationTime = Mathf.Min(currentSynchronizationTime, synchronizationTime);

            if(currentSynchronizationTime >= synchronizationTime)
            {
                /*string cTime = await GetCurrentTime();
                OnlineTime onlineTime = JsonUtility.FromJson<OnlineTime>(cTime);
                currentTime = Mathf.Floor(onlineTime.time / 1000f) % 86400f;
                UpdateVisuals(currentTime);*/
                StartCoroutine(GetCurrentTime());
                Debug.Log(currentTime);
            }
        }
    }

    private void Timer()
    {
        if(currentUpdateTime < updateTime)
        {
            currentUpdateTime += Time.deltaTime;
            currentUpdateTime = Mathf.Min(currentUpdateTime, updateTime);

            if(currentUpdateTime >= updateTime)
            {
                currentTime = dragging ? currentTime : GetSeconds();
                currentTime += updateTime;
                UpdateVisuals(currentTime);
                currentUpdateTime = 0f;
            }
        }
    }

    public void SetTime(float setTime)
    {
        currentTime = setTime;
        UpdateVisuals(currentTime);
    }

    private float GetSeconds()
    {
        float seconds = 0f;
        Arrow currentArrow = timeArrows[0];
        float addition = currentArrow.seconds * currentArrow.divider *
                currentArrow.percentage / currentArrow.repetitions;
        seconds += addition;

        return seconds;
    }

    private void UpdateVisuals(float clockTime)
    {
        float originalTime = Mathf.Abs(clockTime);
        string displayTime = "";
        for (int t = 0; t < timeArrows.Count; t++)
        {
            Arrow currentArrow = timeArrows[t];

            float multiplier = originalTime / currentArrow.seconds;
            float percentage = 
                ((multiplier % currentArrow.divider) / currentArrow.divider) * currentArrow.repetitions;
            currentArrow.transform.eulerAngles = new Vector3(0f, 0f, -360f * percentage);
            currentArrow.percentage = percentage;

            float remainder = Mathf.Floor(multiplier % currentArrow.divider);
            string displayer = remainder < 10f ? $"0{remainder}" : remainder.ToString();
            displayTime += t == 0f ? displayer : $":{displayer}";
            originalTime %= currentArrow.seconds;

            if(!updating)
                timeText.text = displayTime;
        }
    }
}

public class OnlineTime
{
    public float time = 0f;
}