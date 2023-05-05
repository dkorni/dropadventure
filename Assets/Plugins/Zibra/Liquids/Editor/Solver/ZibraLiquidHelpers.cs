using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using com.zibra.liquid.Utilities;
#if ZIBRA_LIQUID_PAID_VERSION
using com.zibra.liquid.Editor.SDFObjects;
#endif
using com.zibra.liquid.Solver;

namespace com.zibra.liquid.Editor.Solver
{
    internal static class ZibraLiquidHelpers
    {
        static string[] GetImportedRenderPipelines()
        {
            return new string[] {
                "BuiltInRP",
#if UNITY_PIPELINE_HDRP
                "HDRP",
#endif
#if UNITY_PIPELINE_URP
                "URP",
#endif
                };
        }

        static string[] GetSupportedFormats()
        {
            List<string> result = new List<string>();
            GraphicsFormat[] formatsToTest = new GraphicsFormat[]
            {
                GraphicsFormat.R16_SFloat,
                GraphicsFormat.R32_SFloat,
                GraphicsFormat.R8G8B8A8_UNorm,
                GraphicsFormat.R16G16B16A16_SFloat,
                GraphicsFormat.R32G32B32A32_SFloat,
                GraphicsFormat.B10G11R11_UFloatPack32,
            };

            FormatUsage[] usagesToTest = new FormatUsage[]
            {
                FormatUsage.Sample,
                FormatUsage.Render,
                FormatUsage.Blend,
                FormatUsage.ReadPixels,
                FormatUsage.LoadStore,
            };

            foreach (GraphicsFormat format in formatsToTest)
            {
                string usages = "";
                foreach (FormatUsage usage in usagesToTest)
                {
                    if (SystemInfo.IsFormatSupported(format, usage))
                    {
                        usages += $".{usage}";
                    }
                }
                if (usages == ".Sample.Render.Blend.ReadPixels.LoadStore")
                {
                    usages = ".All";
                }
                if (usages == "")
                {
                    usages = ".Unsupported";
                }
                result.Add($"{format}{usages}");
            }

            return result.ToArray();
        }

        static string[] GetWarnings()
        {
            List<string> result = new List<string>();

            if (RenderPipelineDetector.IsURPMissingRenderComponent())
            {
                result.Add("URP Liquid Rendering Component is missing!");
            }

            return result.ToArray();
        }

        [Serializable]
        class DiagInfo
        {
            public string PluginVersion = ZibraLiquid.PluginVersion;
            public string PluginSKU = ZibraLiquid.PluginSKU;
            public string UnityVersion = Application.unityVersion;
            public string RenderPipeline = "" + RenderPipelineDetector.GetRenderPipelineType();
            public string[] ImportedRenderPipelines = GetImportedRenderPipelines();
            public string OS = SystemInfo.operatingSystem;
            public string TargetPlatform = "" + EditorUserBuildSettings.activeBuildTarget;
            public string GPU = SystemInfo.graphicsDeviceName;
            public string GPUFeatureLevel = SystemInfo.graphicsDeviceVersion;
            public string[] SupportedFormats = GetSupportedFormats();
#if ZIBRA_LIQUID_PAID_VERSION
            public string ServerStatus = "" + ZibraServerAuthenticationManager.GetInstance().GetStatus();
            public string Key = (ZibraServerAuthenticationManager.GetInstance().PluginLicenseKey == "" ? "Unset" : "Set");
#endif
            public string[] Warnings;
        };

        [MenuItem("Zibra AI/Zibra Liquids/Copy diagnostic information to clipboard", false, 800)]
        public static void Copy()
        {
            DiagInfo info = new DiagInfo();
            string diagInfo = "```\nZibra Liquids Diagnostic Information Begin\n";
            diagInfo += JsonUtility.ToJson(info, true) + "\n";
            diagInfo += "Zibra Liquids Diagnostic Information End\n```\n";
            GUIUtility.systemCopyBuffer = diagInfo;
        }
    }
}