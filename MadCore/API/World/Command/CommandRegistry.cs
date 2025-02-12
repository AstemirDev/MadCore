using HarmonyLib;
using MadCore.API.Registry;
using UnityEngine.UI;

namespace MadCore.API.World.Command
{
    public class CommandRegistry : MadRegistry<MadCommand>
    {
        public static readonly CommandRegistry Instance = new CommandRegistry();
        private static InputField _inputField;

        [HarmonyPatch(typeof(DebugTool), "Start")]
        [HarmonyPostfix]
        private static void Start(DebugTool __instance)
        {
            _inputField = __instance.debugCanvas.transform.Find("InputField").GetComponent<InputField>();
        }
            
        [HarmonyPatch(typeof(DebugTool), "DebugCodeEnter")]
        [HarmonyPrefix]
        private static bool DebugCodeEnter(DebugTool __instance, string code = "")
        {
            var strSplit = SplitCommand(code);
            if (strSplit.Length <= 0) return true;
            var commandStr = strSplit[0];
            var args = GetArguments(strSplit);
            foreach (var command in Instance.Values)
            {
                if (!command.Command.Equals(commandStr)) continue;
                command.Execute.DynamicInvoke(new object[] {args});
                return false;
            }
            return true;
        }

        private static string[] SplitCommand(string command = "")
        {
            return command != "" ? command.Split(' ') : _inputField.text.Split(' ');
        }
        
        private static string[] GetArguments(string[] split)
        {
            if (split.Length > 1)
            {
                var args = new string[split.Length - 1];
                for (var i = 1; i < split.Length; i++)
                {
                    args[i-1] = split[i];
                }
                return args;
            }
            return new string[]{};
        }

        private static InputField GetInput(DebugTool debugTool)
        { 
            return new Traverse(debugTool).Field("debugInput").GetValue<InputField>();
        }
    }
}