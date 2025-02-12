using System;
using MadCore.API.Registry;
using UnityEngine.UI;

namespace MadCore.API.World.Command
{
    public class MadCommand : IDHolder
    {
        public readonly string Command;
        public readonly Action<string[]> Execute;
        
        private ID _id;

        public MadCommand(string command, Action<string[]> execute)
        {
            Command = command;
            Execute = execute;
        }

        public void SetID(ID id)
        {
            _id = id;
        }

        public ID GetID()
        {
            return _id;
        }
    }
}