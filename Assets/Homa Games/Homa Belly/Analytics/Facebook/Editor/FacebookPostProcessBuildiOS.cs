#if UNITY_IOS
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class FacebookPostProcessBuildiOS
    {
        /// <summary>
        /// Fix for Facebook SDK 15.1+ not being able to find pods when using static linking.
        /// Source: https://github.com/facebook/facebook-sdk-for-unity/issues/659#issuecomment-1385324906
        /// <code>CallbackOrder must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40)
        /// and that it's added before "pod install" (50).</code>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buildPath"></param>
        [PostProcessBuild(44)]
        private static void OnPostProcessBuildiOS(BuildTarget target, string buildPath)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(buildPath + "/Podfile"))
                {
                    if (File.ReadLines(buildPath + "/Podfile").Last() != "use_frameworks!")
                    {
                        sw.WriteLine("use_frameworks!");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error while trying to add 'use_frameworks!' to Podfile: " + e);
            }
        }
    }
}
#endif