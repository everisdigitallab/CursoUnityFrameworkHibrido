using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherAPI
{

    const string apiURL = "https://apitempo.inmet.gov.br/condicao/capitais/";
    public struct WeatherStruct
    {
        public string CAPITAL;
        public string TMIN18;
        public string TMAX18;
        public string UMIN18;
        public string PMAX12;
    }

    public delegate void ResponseEvent(string response);
    static public IEnumerator GetWeather(string date, ResponseEvent onSuccess, ResponseEvent onFail)
    {
        string uri = apiURL + date;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                onFail?.Invoke(webRequest.error);
            }
        }
    }
}
