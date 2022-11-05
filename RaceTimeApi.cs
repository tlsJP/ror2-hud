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

      }
    }

    public static bool IsRaceNow()
    {
      var startMs = _timeData.body.item.startTime;
      var endMs = _timeData.body.item.endTime;
      var now = (DateTime.Now - _epoch).TotalMilliseconds;
      return startMs < now && now < endMs;
    }

    public static bool IsRaceFuture()
    {
      var startMs = _timeData.body.item.startTime;
      var endMs = _timeData.body.item.endTime;
      var now = (DateTime.Now - _epoch).TotalMilliseconds;
      return now < startMs && now < endMs;
    }

    public static bool IsRacePast()
    {
      var startMs = _timeData.body.item.startTime;
      var endMs = _timeData.body.item.endTime;
      var now = (DateTime.Now - _epoch).TotalMilliseconds;
      return now > startMs && now > endMs;
    }

    public static DateTime GetEndTime()
    {
      return _endTime;
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