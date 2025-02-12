using System;
using System.IO;
using MadCore.API.Event;
using MadCore.API.Scripts;
using MadCore.API.World;
using MadCore.API.World.Entity;
using Spine;
using UnityEngine;

namespace MadCore.API
{
    public static class MadIsland
    {
        public const string Namespace = "MadIsland";
        public const string ExecutableName = "Mad Island.exe";
        
        public static ManagersScript Managers;
        public static GameObject CameraRig;
        public static CustomCameraShaker CameraShaker;

        public static event EventWorld.WorldLoad OnWorldLoad;
        
        public static event EventWorld.WorldUpdate OnWorldUpdate;
        
        public static void WorldStart(ManagersScript managers)
        {
            Managers = managers;
            CameraRig = GameObject.Find("CameraRig");
            CameraShaker = CameraRig.AddComponent<CustomCameraShaker>();
            PlayerExtension.Inject();
            OnWorldLoad?.DynamicInvoke(managers);
        }

        public static void WorldUpdate(float deltaTime)
        {
            OnWorldUpdate?.DynamicInvoke(deltaTime);   
        }

        public static void InjectPlayerAnimation(PlayerType playerType, Stream animationsStream)
        {
            var player = FindPlayer(playerType);
            var atlasAssetBases = player.anim.skeletonDataAsset.atlasAssets;
            var atlases = new Atlas[atlasAssetBases.Length];
            for (var i = 0; i < atlasAssetBases.Length; i++)
            {
                atlases[i] = atlasAssetBases[i].GetAtlas();
            }
            var attachmentLoader = new AtlasAttachmentLoader(atlases);
            var skeletonJson = new SkeletonJson(attachmentLoader);
            var vanillaData = player.anim.skeletonDataAsset.GetSkeletonData(true);
            var modifiedData = skeletonJson.ReadSkeletonData(new StreamReader(animationsStream));
            vanillaData.Animations.AddRange(modifiedData.Animations);
        }

        public static CommonStates FindPlayer(PlayerType playerType)
        {
            switch (playerType)
            {
                case PlayerType.Yona:
                    return Yona;
                case PlayerType.Man:
                    return Man;
                default:
                    return null;
            }
        }
        
        public static CommonStates Yona => Managers.gameMN.playerCommons[0];
        public static CommonStates Man => Managers.gameMN.playerCommons[1];
        
        public static CommonStates ActivePlayer => Managers.gameMN.playerCommons[GameManager.selectPlayer];

        public static CommonStates[] Players => Managers.gameMN.playerCommons;
    }
}