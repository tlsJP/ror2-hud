



using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace com.thejpaproject.ror2hud
{

  [BepInDependency(R2API.R2API.PluginGUID)]
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Ror2Hud : BaseUnityPlugin
  {
    private static protected ManualLogSource _logger;
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "com.thejpaproject";
    public const string PluginName = "Ror2Hud";
    public const string PluginVersion = "1.0.2";

    private bool timeDataLoaded = false;
    private float timer = 0f;
    private float delay = 1f;

    private long fakeEndTime = (long)(DateTime.Now.AddHours(1) - new DateTime(1970, 1, 1)).TotalSeconds;

    public void Awake()
    {
      _logger = BepInEx.Logging.Logger.CreateLogSource("Ror2Hud");

      On.RoR2.UI.HUD.Awake += MyHud;
      On.RoR2.UI.HUD.Update += Update;

      On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += OnEnter;

      _logger.LogInfo("Loaded");

      // RaceApiTimeData myTest = new RaceApiTimeData();
      // myTest.statusCode = "444";

      var item = new Item();
      item.endTime = 1666929600000;
      item.startTime = 1666911600000;
      item.ID = "Times";
      item.timeInc = 18000000;


      _logger.LogInfo("Item :  " + JsonUtility.ToJson(item, true));

      var body = new Body(item);

      _logger.LogInfo("body ?" + JsonUtility.ToJson(body, true));

      _logger.LogInfo(fakeEndTime);
      _logger.LogInfo(new DateTime().AddSeconds(fakeEndTime));

    }


    void OnEnter(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_OnEnter orig, RoR2.UI.MainMenu.BaseMainMenuScreen self, RoR2.UI.MainMenu.MainMenuController mainMenuController)
    {
      orig(self, mainMenuController);
      if (!timeDataLoaded)
      {
        RacesApi.LoadTimeData(self);
        timeDataLoaded = true;
      }
    }


    private void Update(On.RoR2.UI.HUD.orig_Update orig, RoR2.UI.HUD self)
    {
      orig(self);

      timer += Time.deltaTime;

      if (timer > delay)
      {

        // var endTime = RacesApi.TimeData.body.item.endTime;
        var endDateTime = new DateTime(1970, 1, 1).AddSeconds(fakeEndTime);

        _logger.LogInfo(DateTime.Now);
        _logger.LogInfo(endDateTime);

        var remaining = endDateTime.Subtract(DateTime.Now);

        _logger.LogInfo("hours - " + remaining.Hours);
        _logger.LogInfo("mins - " + remaining.Minutes);
        _logger.LogInfo("secs - " + remaining.Seconds);
        _logger.LogInfo("g - " + remaining.ToString("g"));



        var children = self.GetComponentsInChildren<Text>();
        foreach (Text t in children)
        {

          var remH = remaining.Hours.ToString("D2");
          var remM = remaining.Minutes.ToString("D2");
          var remS = remaining.Seconds.ToString("D2");

          // t.text = $"Time Remaining - {remH}:{remM}:{remS}";

          var formatted = remaining.ToString(@"hh\:mm\:ss");
          t.text = $"Time Remaining - {formatted}";
        }
        timer = timer - delay;
      }
    }


    private void MyHud(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
    {
      orig(self);



      GameObject gameObject = new GameObject("MyHUD");
      gameObject.transform.SetParent(self.mainContainer.transform);

      // (0,0) = bottom left
      // (1,1) = top right
      RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
      rectTransform.anchorMin = new Vector2(0f, .25f);
      rectTransform.anchorMax = new Vector2(.5f, .75f);

      // difference in size of object relative to the anchors
      rectTransform.sizeDelta = Vector2.zero;

      // Move the object to transalte by a certain amount relative to its anchor
      rectTransform.anchoredPosition = Vector2.zero;


      Font arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
      var text = gameObject.AddComponent<Text>();
      text.font = arial;
      text.text = "HELLO WORLD";
      text.fontSize = 48;
      text.alignment = TextAnchor.MiddleCenter;

      // RectTransform ttr;
      // ttr = text.GetComponent<RectTransform>();
      // ttr.localPosition = new Vector3(0, 0, 0);
      // ttr.sizeDelta = new Vector2(600, 200);

    }

    // Adding this seems to prevent the awake event from firing 
    // private void OnDestroy()
    // {
    //   On.RoR2.UI.HUD.Awake -= MyHud;
    // }



  }
}