using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Globalization;

public class WeatherAppInterface : MonoBehaviour
{
    WeatherAPI.WeatherStruct[] weatherData;

    [Header("Splash Screen")]
    [SerializeField] UIAnim splashAnimation;

    [Header("Cities")]
    [SerializeField] UIAnim cityListAnimation;
    [SerializeField] Transform cityListParent;
    [SerializeField] CityCard cityCardPrefab;
    List<CityCard> instantiatedCityCards = new List<CityCard>();
    List<CityCard> cityCardsPool = new List<CityCard>();

    [Header("Weather Interface")]
    [SerializeField] TextMeshProUGUI cityLabel;
    [SerializeField] TextMeshProUGUI minTemperatureText;
    [SerializeField] TextMeshProUGUI maxTemperatureText;

    [Header("Day Time")]
    [SerializeField] TextMeshProUGUI clockText;
    [SerializeField] int clockUpdateFrameRate = 5;
    int lastHour = -1;
    [SerializeField] int dayStartHour = 6;
    [SerializeField] int afternoonStartHour = 15;
    [SerializeField] int nightStartHour = 19;
    [SerializeField] MeshRenderer sunMeshRenderer;
    [SerializeField] MeshRenderer[] cloudMeshRenderers;
    [SerializeField] Material sunDayMat;
    [SerializeField] Material sunAfternoonMat;
    [SerializeField] Material moonMat;
    [SerializeField] Material cloudDayMat;
    [SerializeField] Material cloudAfternoonMat;
    [SerializeField] Material cloudNightMat;

    void Start()
    {
        //System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        clockText.text = System.DateTime.Now.ToString("HH:mm");
        string todaysDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        GetWeather(todaysDate);
    }

    void GetWeather(string todaysDate)
    {
        StartCoroutine(WeatherAPI.GetWeather(todaysDate, OnGetWeather, 
            (error) => 
            {
                print(error);
                GetWeather(todaysDate);
            }));
    }

    private void Update()
    {
        if (Time.frameCount % clockUpdateFrameRate == 0)
        {
            clockText.text = System.DateTime.Now.ToString("HH:mm");
            int currentHour = int.Parse(System.DateTime.Now.ToString("HH"));
            if (lastHour != currentHour)
            {
                UpdateSun(currentHour);
                lastHour = currentHour;
            }
        }
    }

    #region City and Weather
    void OnGetWeather(string result)
    {
        splashAnimation.OpenAnim(0);

        weatherData = JsonConvert.DeserializeObject<WeatherAPI.WeatherStruct[]>(result);
        PopulateCitiesCards();
    }

    void PopulateCitiesCards()
    {
        SendAllCityCardsToPool();
        bool citySelected = false;
        foreach (WeatherAPI.WeatherStruct weather in weatherData)
        {
            CityCard newCityCard = GetCityCardInPool();

            if (newCityCard == null)
            {
                newCityCard = Instantiate(cityCardPrefab, cityListParent);
            }

            instantiatedCityCards.Add(newCityCard);
            newCityCard.SetCity(weather);
            newCityCard.gameObject.SetActive(true);
            if (!citySelected)
            {
                newCityCard.OpenCity();
                citySelected = true;
            }
        }
    }

    CityCard GetCityCardInPool()
    {
        if (cityCardsPool.Count == 0)
            return null;

        CityCard cityCard = cityCardsPool[0];
        cityCardsPool.RemoveAt(0);
        return cityCard;
    }

    void SendAllCityCardsToPool()
    {
        foreach (CityCard cityCard in instantiatedCityCards)
        {
            cityCard.gameObject.SetActive(false);
            cityCardsPool.Add(cityCard);
        }
        instantiatedCityCards.Clear();
    }

    public void OpenCityList()
    {
        if (cityListAnimation.anims[0].state == UIAnim.Animation.State.Open)
        {
            CloseCityList();
        }
        else
        {
            cityListAnimation.OpenAnim(0);
        }
    }

    public void CloseCityList()
    {
        cityListAnimation.CloseAnim(0);
    }

    public void SelectCity(WeatherAPI.WeatherStruct cityWeather)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        cityLabel.text = textInfo.ToTitleCase(cityWeather.CAPITAL.ToLower());

        minTemperatureText.text = cityWeather.TMIN18.Replace("*", "") + "°C";
        maxTemperatureText.text = cityWeather.TMAX18.Replace("*", "") + "°C";
    }
    #endregion

    #region Day Time
    void UpdateSun(int currentHour)
    {
        if (currentHour >= dayStartHour)
        {
            sunMeshRenderer.material = moonMat;
            foreach (MeshRenderer cloud in cloudMeshRenderers)
            {
                cloud.material = cloudNightMat;
            }
        }
        if (currentHour < afternoonStartHour)
        {
            sunMeshRenderer.material = sunDayMat;
            foreach (MeshRenderer cloud in cloudMeshRenderers)
            {
                cloud.material = cloudDayMat;
            }
        }
        else if (currentHour < nightStartHour)
        {
            sunMeshRenderer.material = sunAfternoonMat;
            foreach (MeshRenderer cloud in cloudMeshRenderers)
            {
                cloud.material = cloudAfternoonMat;
            }
        }
        else
        {
            sunMeshRenderer.material = moonMat;
            foreach (MeshRenderer cloud in cloudMeshRenderers)
            {
                cloud.material = cloudNightMat;
            }
        }
    }
    #endregion
}
