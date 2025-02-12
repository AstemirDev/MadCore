using MadCore.API;
using MadCore.API.Gui;
using MadCore.API.Gui.Component;
using MadCore.API.Utils;
using MadCore.API.World;
using MadCore.Example.Item;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MadCore.Example
{
    public static class ExampleInit
    {
        public static AssetBundle AssetBundle; 

        public static void Load()
        {
            AssetBundle = AssetBundle.LoadFromStream(AssetUtils.GetEmbeddedAsset("Assets/bundles/mod"));
            ExampleFX.RegisterAll();
            ExampleSounds.RegisterAll();
            ExampleItems.RegisterAll();
            ExampleCraft.RegisterAll();
            ExampleCommands.RegisterAll();
            MadTitleScreen.OnLoad += canvas =>
            {
                var menuPanel = canvas.transform.Find("TitleMenuPanel");
                var loadButton = menuPanel.Find("LoadButton");
                var loadButtonBtn = loadButton.GetComponent<Button>();
                var textMeshPro = loadButton.Find("Text").GetComponent<TextMeshProUGUI>();
                
                var modButton = new GameObject("ModButton");
                var rectTransform = modButton.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(137.6899F, 31.7011F);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                var button = modButton.AddComponent<Button>();
                button.colors = loadButtonBtn.colors;
                
                var text = new GameObject("Text");
                var meshPro = text.AddComponent<TextMeshProUGUI>();
                meshPro.text = "Mods";
                meshPro.font = textMeshPro.font;
                meshPro.fontSize = textMeshPro.fontSize;
                text.transform.SetParent(modButton.transform);
                

                modButton.transform.SetParent(menuPanel, false);
                
                CreateDecoLabel(canvas.transform);
                // var modList = ModList.Create();
                // modList.AddEntry("MadCore v1.0");
            };
            
            MadIsland.OnWorldLoad += managers => {
                MadCore.Logger.LogInfo("World Started!");
                MadIsland.InjectPlayerAnimation(PlayerType.Yona, AssetUtils.GetEmbeddedAsset("Assets/animations/yona_test.json"));
            };
        }

        public static void CreateDecoLabel(Transform transform)
        {
            var text = new GameObject("ModLabel");
            var textMeshPro = text.AddComponent<TextMeshProUGUI>();
            textMeshPro.text = $"{MadCore.Name} {MadCore.Version}";
            textMeshPro.font = TMP_FontAsset.CreateFontAsset(AssetBundle.LoadAsset<Font>("assets/modassets/font/Eater-Regular.ttf"));
            textMeshPro.fontSize = 10;
            textMeshPro.outlineWidth = 0.5F;
            textMeshPro.outlineColor = Color.white;
            textMeshPro.color = Color.cyan;
            textMeshPro.transform.localScale = new Vector3(2.80F, 2.80F, 1.0F);
            textMeshPro.transform.position = new Vector3(289, 10F, 0);
            text.transform.SetParent(transform, true);
        }
    }
}