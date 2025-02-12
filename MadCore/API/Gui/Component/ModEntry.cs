using System;
using MadCore.Example;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MadCore.API.Gui.Component
{
    public class ModEntry : MonoBehaviour
    {
        public string ModName;
        public Sprite Preview;
        
        private void Start()
        {
            transform.Find("Name").GetComponent<TextMeshProUGUI>().text = ModName;
            if (Preview)
            {
                transform.Find("Preview").GetComponent<Image>().sprite = Preview;
            }
        }

        public static ModEntry Create(string modName, Sprite preview, Transform parent)
        {
            var prefab = ExampleInit.AssetBundle.LoadAsset<GameObject>("assets/modassets/ui/ModEntry.prefab");
            var instance = Instantiate(prefab, parent);
            var entry = instance.AddComponent<ModEntry>();
            entry.ModName = modName;
            entry.Preview = preview;
            return entry;
        }
    }
}