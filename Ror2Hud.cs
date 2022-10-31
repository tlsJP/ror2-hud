



using BepInEx;
using BepInEx.Logging;

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
    public const string PluginName = "ror2hud";
    public const string PluginVersion = "1.0.2";


    public void Awake()
    {
      _logger = BepInEx.Logging.Logger.CreateLogSource("Ror2Hud");

      On.RoR2.UI.HUD.Awake += MyHud;

      _logger.LogInfo("loaded");
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