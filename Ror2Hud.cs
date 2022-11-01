



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

    public void Awake()
    {
      _logger = BepInEx.Logging.Logger.CreateLogSource("Ror2Hud");

      On.RoR2.UI.HUD.Awake += MyHud;
      On.RoR2.UI.HUD.Update += Update;

      On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += OnEnter;

      _logger.LogInfo("Loaded");

      RaceApiTimeData myTest = new RaceApiTimeData();
      myTest.statusCode = "444";
      myTest.body = new Body();
      myTest.body.item = new Item();
      myTest.body.item.endTime = 1666929600000;
      myTest.body.item.startTime = 1666911600000;
      myTest.body.item.ID = "Times";
      myTest.body.item.timeInc = 18000000;

      _logger.LogInfo(myTest + " :  " + JsonUtility.ToJson(myTest.body.item, true));

      _logger.LogInfo("body ?" + myTest.body);
      _logger.LogInfo("body.item ?" + myTest.body.item);
      _logger.LogInfo(myTest + " :  " + JsonUtility.ToJson(myTest.body, true));



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
        var children = self.GetComponentsInChildren<Text>();
        foreach (Text t in children)
        {
          _logger.LogInfo(DateTime.Now);
          _logger.LogInfo("Time Data ? " + RacesApi.TimeData.body);
          _logger.LogInfo("Time Data ? " + RacesApi.TimeData.body?.item?.endTime);

          t.text = DateTime.Now.ToString();
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