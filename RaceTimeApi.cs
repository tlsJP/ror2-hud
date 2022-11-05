using BepInEx.Logging;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;



namespace com.thejpaproject.ror2hud
{

  class RaceTimeApi
  {
    private static protected ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("RaceTimeApi");

    private const string HOST = "https://zjbjowthie.execute-api.us-east-1.amazonaws.com/RaceDev";

    private static DateTime _epoch = new DateTime(1970, 1, 1);
    private static DateTime _endTime = DateTime.MaxValue;
    private static DateTime _startTime = DateTime.MaxValue;
    private static RaceApiTimeData _timeData;

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
        _logger.LogInfo($"Raw : {response}");
        _timeData = JsonConvert.DeserializeObject<RaceApiTimeData>(response);
        _startTime = _epoch.AddSeconds(_timeData.body.item.endTime);
        _endTime = _epoch.AddSeconds(_timeData.body.item.endTime);

        // DEBUG TESTING

        // FUTURE RACE
        // _startTime = DateTime.Now.AddHours(1);
        // _endTime = DateTime.Now.AddHours(2);

        // RACE HAPPENING
        // _startTime = DateTime.Now;
        // _endTime = DateTime.Now.AddHours(1).AddMinutes(1);

        // RACE ENDING
        // _startTime = DateTime.Now;
        // _endTime = DateTime.Now.AddMinutes(1);

      }
    }

    public static DateTime GetStartTime()
    {
      return _startTime;
    }

    public static bool IsRaceFuture()
    {
      var now = DateTime.Now;
      return now < _startTime && now < _endTime;
    }

    public static bool IsRaceNow()
    {
      var now = DateTime.Now;
      return _startTime < now && now < _endTime;
    }

    public static bool IsRacePast()
    {
      var now = DateTime.Now;
      return now > _startTime && now > _endTime;
    }

    public static TimeSpan GetRemaining()
    {
      if (IsRaceFuture() || IsRacePast())
      {
        return TimeSpan.MaxValue;
      }

      return _endTime.Subtract(DateTime.Now);
    }

  }

  [System.Serializable]
  public class RaceApiTimeData
  {
    public string statusCode;
    [SerializeField] public Body body;
  }


  [System.Serializable]
  public class Body
  {
    public Item item;
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