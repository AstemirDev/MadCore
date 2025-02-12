using System;
using System.Collections.Generic;
using System.Linq;
using MadCore.API.Utils;
using MadCore.API.World.Entity;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;

namespace MadCore.API.World.Item
{
    public class CustomAttachment
    {
	    public readonly string SlotName;
	    public readonly string AttachmentName;
	    public Dictionary<NPCId, AttachmentInfo> AttachmentMap;
	    
	    public CustomAttachment(string slotName, string attachmentName, Dictionary<NPCId, AttachmentInfo> attachmentMap)
	    {
		    SlotName = slotName;
		    AttachmentName = attachmentName;
		    AttachmentMap = attachmentMap;
	    }

	    public void AttachToSkeleton(Skeleton skeleton, NPCId npcId)
	    {
		    var slotIndex = skeleton.FindSlotIndex(SlotName);
		    var defaultSkin = skeleton.Data.DefaultSkin;
		    if (defaultSkin.GetAttachment(slotIndex, AttachmentName) == null)
		    {
			    var attachmentInfo = AttachmentMap[npcId];
			    var material = new Material(defaultSkin.GetAttachment(slotIndex, "sword_blood").GetMaterial()) {
				    mainTexture = attachmentInfo.Sprite.texture
			    };
			    var atlasRegion = attachmentInfo.Sprite.ToAtlasRegion(material);
			    var	meshAttachment = BuildMesh(atlasRegion, npcId, 1.0F/attachmentInfo.Sprite.pixelsPerUnit);
			    defaultSkin.Attachments.Add(new Skin.SkinEntry(slotIndex, AttachmentName, meshAttachment), meshAttachment);
		    }
		    skeleton.SetAttachment(SlotName, AttachmentName);
	    }

        public MeshAttachment BuildMesh(AtlasRegion atlasRegion, NPCId npcId, float scale)
        {
	        var attachmentInfo = AttachmentMap[npcId];
	        var meshAttachment = new MeshAttachment(AttachmentName)
	        {
		        RendererObject = atlasRegion,
		        RegionU = atlasRegion.u,
		        RegionV = atlasRegion.v,
		        RegionU2 = atlasRegion.u2,
		        RegionV2 = atlasRegion.v2,
		        RegionRotate = atlasRegion.rotate,
		        RegionDegrees = atlasRegion.degrees,
		        RegionOffsetX = atlasRegion.offsetX,
		        RegionOffsetY = atlasRegion.offsetY,
		        RegionWidth = atlasRegion.width,
		        RegionHeight =  atlasRegion.height,
		        RegionOriginalWidth =  atlasRegion.originalWidth,
		        RegionOriginalHeight =  atlasRegion.originalHeight,
		        Width = attachmentInfo.Width*scale,
		        Height = attachmentInfo.Height*scale,
	        };
	        ReadVertices(meshAttachment, attachmentInfo.Vertices, attachmentInfo.Uvs.Length, scale);
	        meshAttachment.Triangles = attachmentInfo.Triangles;
	        meshAttachment.RegionUVs = attachmentInfo.Uvs;
	        meshAttachment.UpdateUVs();
	        meshAttachment.HullLength = attachmentInfo.HullLength;
	        meshAttachment.Edges = attachmentInfo.Edges;
	        return meshAttachment;
        }
        
        private void ReadVertices(
	        VertexAttachment attachment,
	        float[] vertices,
	        int verticesLength,
	        float scale)
        {
	        attachment.WorldVerticesLength = verticesLength;
	        float[] floatArray = vertices;
	        if (verticesLength == floatArray.Length)
	        {
		        if (scale != 1.0)
		        {
			        for (int index = 0; index < floatArray.Length; ++index)
				        floatArray[index] *= scale;
		        }
		        attachment.Vertices = floatArray;
	        }
	        else
	        {
		        ExposedList<float> exposedList1 = new ExposedList<float>(verticesLength * 3 * 3);
		        ExposedList<int> exposedList2 = new ExposedList<int>(verticesLength * 3);
		        int index1 = 0;
		        int length = floatArray.Length;
		        while (index1 < length)
		        {
			        int num = (int) floatArray[index1++];
			        exposedList2.Add(num);
			        for (int index2 = index1 + num * 4; index1 < index2; index1 += 4)
			        {
				        exposedList2.Add((int) floatArray[index1]);
				        exposedList1.Add(floatArray[index1 + 1] * scale);
				        exposedList1.Add(floatArray[index1 + 2] * scale);
				        exposedList1.Add(floatArray[index1 + 3]);
			        }
		        }
		        attachment.Bones = exposedList2.ToArray();
		        attachment.Vertices = exposedList1.ToArray();
	        }
        }
        
        public static CustomAttachment Load(string path)
        {
	        var json = JsonUtils.ReadJson(AssetUtils.GetEmbeddedAsset(path));
	        var slotName = json.Keys.First();
	        var attachmentJson = json[slotName] as Dictionary<string,object>;
	        if (attachmentJson == null) return null;
	        var attachmentName = attachmentJson.Keys.First();
	        var attachmentsListJson = attachmentJson[attachmentName] as Dictionary<string,object>;
	        if (attachmentsListJson == null) return null;
	        var attachmentMap = new Dictionary<NPCId, AttachmentInfo>();
	        foreach (var key in attachmentsListJson.Keys)
	        {
		        var attachmentData = attachmentsListJson[key] as Dictionary<string,object>;
		        var uvs = JsonUtils.GetFloatArray(attachmentData, "uvs");
		        var vertices = JsonUtils.GetFloatArray(attachmentData, "vertices");
		        var triangles = JsonUtils.GetIntArray(attachmentData, "triangles");
		        var edges = JsonUtils.GetIntArray(attachmentData, "edges");
		        var hull = JsonUtils.GetInt(attachmentData, "hull", 0)*2;
		        var width = JsonUtils.GetFloat(attachmentData, "width", 0.0F);
		        var height = JsonUtils.GetFloat(attachmentData, "height", 0.0F);
		        var sprite = AssetUtils.LoadSprite(JsonUtils.GetString(attachmentData, "texture", ""));
		        var attachmentInfo = new AttachmentInfo(sprite, uvs, vertices, triangles, edges, width, height, hull);
		        var npcId = (NPCId) Enum.Parse(typeof(NPCId), key, true);
		        attachmentMap[npcId] = attachmentInfo;
	        }
	        return new CustomAttachment(slotName, attachmentName, attachmentMap);
        }

        public class AttachmentInfo
        {
	        public readonly float[] Uvs;
	        public readonly float[] Vertices;
	        public readonly int[] Triangles;
	        public readonly int[] Edges;
	        public readonly float Width;
	        public readonly float Height;
	        public readonly int HullLength;
	        public Sprite Sprite;
	        
	        public AttachmentInfo(Sprite sprite, float[] uvs, float[] vertices, int[] triangles, int[] edges, float width, float height, int hullLength)
	        {
		        Sprite = sprite;
		        Uvs = uvs;
		        Vertices = vertices;
		        Width = width;
		        Triangles = triangles;
		        Edges = edges;
		        Height = height;
		        HullLength = hullLength;
	        }
        }
    }
}