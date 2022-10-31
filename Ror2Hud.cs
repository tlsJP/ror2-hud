



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


    private RoR2.UI.HUD _hud = null;

    public void Awake()
    {
      _logger = BepInEx.Logging.Logger.CreateLogSource("Ror2Hud");

      On.RoR2.UI.HUD.Awake += MyHud;

    }


    private void MyHud(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
    {
      orig(self);
      var mainContainer = self.mainContainer.transform;

      GameObject gameObject = new GameObject("MyHUD");
      gameObject.transform.SetParent(mainContainer);

      // (0,0) = bottom left
      // (1,1) = top right
      RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
      rectTransform.anchorMin = Vector2.zero;
      rectTransform.anchorMax = Vector2.one;

      // difference in size of object relative to the anchors
      rectTransform.sizeDelta = Vector2.zero;

      // Move the object to transalte by a certain amount relative to its anchor
      rectTransform.anchoredPosition = Vector2.zero;

      var image = gameObject.AddComponent<Image>();
      image.sprite = Resources.Load<Sprite>("textures/itemicons/texBearIcon");



    }

    private void OnDestroy()
    {
      On.RoR2.UI.HUD.Awake -= DestroyMyHud;
    }

    private void DestroyMyHud(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
    {
      // todo ?
    }

  }
}