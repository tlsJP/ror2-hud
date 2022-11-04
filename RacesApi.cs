


using BepInEx.Logging;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;




namespace com.thejpaproject.ror2hud
{



  class RacesApi
  {
    private static protected ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("RacesApi");

    private static string HOST = "https://zjbjowthie.execute-api.us-east-1.amazonaws.com/RaceDev";


    public static RaceApiTimeData TimeData;

    public static void LoadTimeData(MonoBehaviour monoDelegate)
    {
      _logger.LogInfo("LoadTimeData()");
      monoDelegate.StartCoroutine(GetTimeData());


    }

    private static IEnumerator GetTimeData()
    {
      _logger.LogInfo("GetTimeData()");

      UnityWebRequest request = UnityWebRequest.Get(HOST + "/times");
      yield return request.SendWebRequest();

      if (request.isNetworkError || request.isHttpError)
      {
        _logger.LogError(request.error);
      }
      else
      {
        var response = request.downloadHandler.text;
        _logger.LogInfo(DateTimeOffset.Now.ToString() + " " + response);
        TimeData = JsonUtility.FromJson<RaceApiTimeData>(response);
        _logger.LogInfo(DateTimeOffset.Now.ToString() + " " + JsonUtility.ToJson(TimeData));


      }
    }

  }

  [System.Serializable]
  public class RaceApiTimeData
  {
    public string statusCode;
    [SerializeField] public Body body;
  }



  public class Body
  {

    public Body(Item item)
    {
      this.item = item;
    }

    [field: SerializeField] public Item item;


  }


  [System.Serializable]
  public class Item
  {
    public long startTime;
    public bool overall;
    public long endTime;
    public string ID;
    public long timeInc;


  }



}