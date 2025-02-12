using System;
using MadCore.Example;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MadCore.API.Gui.Component
{
    public class ModList : MonoBehaviour
    {
        public GameObject Container;
        public Button CloseButton;
        
        private void Awake()
        {
            Container = transform.Find("Canvas/modList/Viewport/Content").gameObject;
            CloseButton = transform.Find("Canvas/closeButton").GetComponent<Button>();
            CloseButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }

        public ModEntry AddEntry(string modName)
        {
            return ModEntry.Create(modName, null, Container.transform);
        }

        public static ModList Create(Transform parent = null)
        {
            var prefab = ExampleInit.AssetBundle.LoadAsset<GameObject>("assets/modassets/ui/ModList.prefab");
            var instance = Instantiate(prefab, parent);
            return instance.AddComponent<ModList>();
        }
    }
}