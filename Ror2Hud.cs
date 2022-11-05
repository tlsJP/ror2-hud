
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

    private static DateTime epoch = new DateTime(1970, 1, 1);

    private long fakeEndTime = (long)(DateTime.Now.AddHours(1).AddMinutes(2) - epoch).TotalSeconds;

    public void Awake()
    {
      _logger = BepInEx.Logging.Logger.CreateLogSource("Ror2Hud");

      On.RoR2.UI.HUD.Awake += MyHud;
      On.RoR2.UI.HUD.Update += Update;

      On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += OnEnter;

      _logger.LogInfo("Loaded");

    }


    void OnEnter(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_OnEnter orig, RoR2.UI.MainMenu.BaseMainMenuScreen self, RoR2.UI.MainMenu.MainMenuController mainMenuController)
    {
      orig(self, mainMenuController);
      if (!timeDataLoaded)
      {
        RaceTimeApi.LoadTimeData(self);
        timeDataLoaded = true;
      }
    }



    private void Update(On.RoR2.UI.HUD.orig_Update orig, RoR2.UI.HUD self)
    {
      orig(self);

      timer += Time.deltaTime;

      if (timer > delay)
      {

        var remaining = RaceTimeApi.GetRemaining();

        var children = self.GetComponentsInChildren<Text>();
        foreach (Text t in children)
        {
          if (RaceTimeApi.IsRaceFuture())
          {
            // var endTime = RacesApi.TimeData.body.item.endTime;
            t.text = $"Race Start : {RaceTimeApi.GetStartTime()}";
          }
          else if (RaceTimeApi.IsRaceNow())
          {
            var formatted = remaining.ToString(@"hh\:mm\:ss");
            t.text = $"Time Remaining - {formatted}";
          }
          else
          {
            t.text = "Race has ended";
          }

          ApplyTimerColor(t, remaining);

        }
        timer = timer - delay;
      }
    }

    void ApplyTimerColor(Text t, TimeSpan remaining)
    {
      // We only want to apply custom color to the actual timer, not its drop shadow
      if (!t.name.Equals("Timer"))
      {
        return;
      }

      if (RaceTimeApi.IsRacePast())
      {
        t.color = Color.grey;
        return;
      }

      if (remaining.Hours < 1)
      {
        t.color = Color.yellow;
        return;
      }


    }


    private void MyHud(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
    {
      int fontSize = 36;
      Font arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
      orig(self);

      // This is all just math to orient the timer text and its drop shadow
      var maxX = .31f;
      var maxY = .07f;
      var sDeltaX = .002f;
      var sDeltaY = .004f;
      var rectMin = Vector2.zero;
      var textMax = new Vector2(maxX, maxY);
      var shadowMax = new Vector2(maxX + sDeltaX, maxY - sDeltaY);

      // Draw drop shadow
      GameObject shadowObject = new GameObject("TimerShadow");
      shadowObject.transform.SetParent(self.mainContainer.transform);

      // (0,0) = bottom left
      // (1,1) = top right
      RectTransform shadowRectTranform = shadowObject.AddComponent<RectTransform>();
      shadowRectTranform.anchorMin = rectMin;
      shadowRectTranform.anchorMax = shadowMax;

      // difference in size of object relative to the anchors
      shadowRectTranform.sizeDelta = Vector2.zero;

      // Move the object to transalte by a certain amount relative to its anchor
      shadowRectTranform.anchoredPosition = Vector2.zero;

      var shadowText = shadowObject.AddComponent<Text>();
      shadowText.font = arial;
      shadowText.text = "ඞ";
      shadowText.fontSize = fontSize;
      shadowText.color = Color.black;
      shadowText.alignment = TextAnchor.MiddleCenter;


      // Draw the main timer
      GameObject timerObject = new GameObject("Timer");
      timerObject.transform.SetParent(self.mainContainer.transform);

      // (0,0) = bottom left
      // (1,1) = top right
      RectTransform timerRectTransform = timerObject.AddComponent<RectTransform>();
      timerRectTransform.anchorMin = rectMin;
      timerRectTransform.anchorMax = textMax;

      // difference in size of object relative to the anchors
      timerRectTransform.sizeDelta = Vector2.zero;

      // Move the object to transalte by a certain amount relative to its anchor
      timerRectTransform.anchoredPosition = Vector2.zero;

      var timerText = timerObject.AddComponent<Text>();
      timerText.font = arial;
      timerText.text = "ඞ";
      timerText.fontSize = fontSize;
      timerText.alignment = TextAnchor.MiddleCenter;


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