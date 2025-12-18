using BepInEx;
using RoR2;
using RoR2.UI;
using UnityEngine.UI;
using RoR2.UI.MainMenu;
using UnityEngine;

namespace CombinedMenu
{
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Main : BaseUnityPlugin
  {
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "Nuxlar";
    public const string PluginName = "CombinedMenu";
    public const string PluginVersion = "1.0.0";

    internal static Main Instance { get; private set; }
    public static string PluginDirectory { get; private set; }

    public void Awake()
    {
      Instance = SingletonHelper.Assign(Instance, this);

      Log.Init(Logger);

      On.RoR2.UI.MainMenu.MainMenuController.Awake += EditMainMenu;
    }

    void OnDestroy()
    {
      On.RoR2.UI.MainMenu.MainMenuController.Awake -= EditMainMenu;

      Instance = SingletonHelper.Unassign(Instance, this);
    }

    static void EditMainMenu(On.RoR2.UI.MainMenu.MainMenuController.orig_Awake orig, MainMenuController self)
    {
      orig(self);

      Transform singleplayerButtonTransform = self.transform.Find("MENU: Title/TitleMenu/SafeZone/GenericMenuButtonPanel/JuicePanel/GenericMenuButton (Singleplayer)");
      Transform gamemodeButtonTransform = self.transform.Find("MENU: Title/TitleMenu/SafeZone/GenericMenuButtonPanel/JuicePanel/GenericMenuButton (Extra Game Mode)");
      Transform gamemodeMenuTransform = self.transform.Find("MENU: Extra Game Mode/ExtraGameModeMenu/Main Panel/GenericMenuButtonPanel/JuicePanel/");
      if (singleplayerButtonTransform && gamemodeButtonTransform && gamemodeMenuTransform)
      {
        GameObject newSingleplayerBtn = Instantiate(self.transform.Find("MENU: Extra Game Mode/ExtraGameModeMenu/Main Panel/GenericMenuButtonPanel/JuicePanel/GenericMenuButton (Eclipse)").gameObject, gamemodeMenuTransform);
        newSingleplayerBtn.transform.SetSiblingIndex(1);
        newSingleplayerBtn.GetComponent<LanguageTextMeshController>().token = "Classic";
        HGButton hgButton = newSingleplayerBtn.GetComponent<HGButton>();
        hgButton.hoverToken = "Play the classic Risk of Rain 2 experience.";
        hgButton.onClick = new Button.ButtonClickedEvent();
        hgButton.onClick.AddListener(() =>
        {
          Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
          RoR2.Console.instance.SubmitCmd(null, "transition_command \"gamemode ClassicRun; host 0; \"");
        });

        GameObject.Destroy(singleplayerButtonTransform.gameObject);

        HGButton gamemodeBtn = gamemodeButtonTransform.GetComponent<HGButton>();
        gamemodeButtonTransform.SetSiblingIndex(1);
        gamemodeButtonTransform.GetComponent<LanguageTextMeshController>().token = "Singleplayer";
        gamemodeBtn.hoverToken = "Choose your preferred game mode.";
      }
    }
  }
}