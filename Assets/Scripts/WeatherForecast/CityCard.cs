using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CityCard : MonoBehaviour
{
    [SerializeField] WeatherAppInterface menuManager;
    [SerializeField] TextMeshProUGUI cityName;
    WeatherAPI.WeatherStruct cityWeather;

    public void SetCity(WeatherAPI.WeatherStruct weatherData)
    {
        cityWeather = weatherData;
        cityName.text = cityWeather.CAPITAL;
    }

    public void OpenCity()
    {
        menuManager.SelectCity(cityWeather);
    }
}
