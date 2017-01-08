using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Graphing;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace UnityEditor.VFX.Test
{
    [TestFixture]
    public class VFXSerializationTests
    {
        private class VFXContextDescInit : VFXContextDesc
        {
            public VFXContextDescInit() : base(VFXContextDesc.Type.kTypeInit, "init") { }
        }

        private class VFXContextDescUpdate : VFXContextDesc
        {
            public VFXContextDescUpdate() : base(VFXContextDesc.Type.kTypeUpdate, "update") { }
        }

        private class VFXContextDescOutput : VFXContextDesc
        {
            public VFXContextDescOutput() : base(VFXContextDesc.Type.kTypeOutput, "output") { }
        }

        private readonly static string kTestAssetDir = "Assets/VFXEditorNew/Editor/Tests";
        private readonly static string kTestAssetName = "TestAsset";
        private readonly static string kTestAssetPath = kTestAssetDir + "/" + kTestAssetName + ".asset";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            string[] guids = AssetDatabase.FindAssets(kTestAssetName, new string[] { kTestAssetDir });

            // If the asset does not exist, create it
            if (guids.Length == 0)
            {
                VFXModelContainer asset = ScriptableObject.CreateInstance<VFXModelContainer>();
                InitAsset(asset);
                AssetDatabase.CreateAsset(asset,kTestAssetPath);
            }
        }

        [Test]
        public void SerializeModel()
        {
            VFXModelContainer assetSrc = ScriptableObject.CreateInstance<VFXModelContainer>();
            VFXModelContainer assetDst = ScriptableObject.CreateInstance<VFXModelContainer>();

            InitAsset(assetSrc);
            EditorUtility.CopySerialized(assetSrc, assetDst);
            CheckAsset(assetDst);

            Object.DestroyImmediate(assetSrc);
            Object.DestroyImmediate(assetDst);
        }

        [Test]
        public void LoadAssetFromPath()
        {
            VFXModelContainer asset = AssetDatabase.LoadAssetAtPath<VFXModelContainer>(kTestAssetPath);
            CheckAsset(asset);
        }

        private void InitAsset(VFXModelContainer asset)
        {
            asset.m_Roots.Clear();

            VFXSystem system0 = new VFXSystem();
            system0.AddChild(new VFXContext(new VFXContextDescInit()));
            system0.AddChild(new VFXContext(new VFXContextDescUpdate()));
            system0.AddChild(new VFXContext(new VFXContextDescOutput()));

            VFXSystem system1 = new VFXSystem();
            system1.AddChild(new VFXContext(new VFXContextDescInit()));
            system1.AddChild(new VFXContext(new VFXContextDescOutput()));

            asset.m_Roots.Add(system0);
            asset.m_Roots.Add(system1);
        }

        private void CheckAsset(VFXModelContainer asset)
        {
            Assert.AreEqual(2, asset.m_Roots.Count);
            Assert.AreEqual(3, asset.m_Roots[0].GetNbChildren());
            Assert.AreEqual(2, asset.m_Roots[1].GetNbChildren());

            Assert.IsNotNull(((VFXSystem)(asset.m_Roots[0])).GetChild(0).Desc);
            Assert.IsNotNull(((VFXSystem)(asset.m_Roots[0])).GetChild(1).Desc);
            Assert.IsNotNull(((VFXSystem)(asset.m_Roots[0])).GetChild(2).Desc);
            Assert.IsNotNull(((VFXSystem)(asset.m_Roots[1])).GetChild(0).Desc);
            Assert.IsNotNull(((VFXSystem)(asset.m_Roots[1])).GetChild(1).Desc);
        }
    }
}