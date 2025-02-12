using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadCore.API.Utils
{
    public static class GameUtils
    {
        public static GameObject FindInRoot(string search)
        {
            var scene = SceneManager.GetActiveScene();
            var sceneRoots = scene.GetRootGameObjects();
            return sceneRoots.FirstOrDefault(root => root.name.Equals(search));
        }
         
        public static GameObject FindOnScene(string search)
        {
            var scene = SceneManager.GetActiveScene();
            var sceneRoots = scene.GetRootGameObjects();
            GameObject result = null;
            foreach(var root in sceneRoots)
            {
                if(root.name.Equals(search)) return root;
                result = FindRecursive(root, search);
                if(result) break;
            }
            return result;
        }

        private static GameObject FindRecursive(GameObject obj, string search)
        {
            GameObject result = null;
            foreach(Transform child in obj.transform)
            {
                if(child.name.Equals(search)) return child.gameObject;
                result = FindRecursive (child.gameObject, search);
                if(result) break;
            }
            return result;
        }
    }
}