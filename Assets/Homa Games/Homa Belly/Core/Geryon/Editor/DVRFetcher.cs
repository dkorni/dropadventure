using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomaGames.HomaBelly;
using HomaGames.HomaBelly.Utilities;
using UnityEditor;
using UnityEngine;

namespace HomaGames.Geryon
{
    [InitializeOnLoad]
    public static class DVRFetcher
    {
        private static readonly string DVR_FETCHING_ENDPOINT = $"{HomaBellyConstants.API_HOST}/appbase";
        private const string UPDATE_NTESTING_MENU_ITEM = "Window/Homa Games/Update N-Testing Values";
        
        private enum DvrOs
        {
            I_OS, ANDROID
        }

        private struct DvrDefinition
        {
            public string IOS, Android, Default;
        }
        
        static DVRFetcher()
        {
            CorePackageImportListener.OnCorePackageImported += AutoFetch;
        }
        
        /// <summary>
        /// If Geryon is enabled and the DVR file is not present in the project,
        /// automatically fetch and write it.
        /// </summary>
        private static void AutoFetch()
        {
            // If there is no manifest, just return and ignore
            PluginManifest manifest = PluginManifest.LoadFromLocalFile();
            if (manifest != null)
            {
                // If there is no DVR within the project, automatically fetch
                if (!TryGetDvrAssetPath(out _))
                {
                    EditorApplication.delayCall += () =>
                    {
                        new DvrFetcherObject(DvrFetcherObject.FetchMode.Automatic)
                            .StartUpdateDvrAsync()
                            .ListenForErrors();
                    };
                }
            }
        }

        // Await/async does not work in menu item, so we use a dummy window.
        [MenuItem(UPDATE_NTESTING_MENU_ITEM)]
        private static void UpdateDvrWithUiMenuItem()
        {
            UpdateDvrWithUiAsync(DvrFetcherObject.FetchMode.Manual)
                .ListenForErrors();
        }
        
        private static async Task UpdateDvrWithUiAsync(DvrFetcherObject.FetchMode fetchMode)
        {
            DvrFetcherObject fetcher = new DvrFetcherObject(fetchMode);

            void UpdateProgressBar()
            {
                ThreadUtils.RunOnMainThreadAsync(() =>
                {
                    if (fetcher.Progress < 1 && fetcher.Progress >= 0)
                        EditorUtility.DisplayProgressBar("Updating N-Testing...", "Fetching NTesting values...",
                            fetcher.Progress);
                });
            }
            fetcher.OnProgressUpdated += UpdateProgressBar;
            
            try
            {
                UpdateProgressBar();
                await fetcher.StartUpdateDvrAsync();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Debug.LogError("[N-Testing] Exception when trying to fetch DVR values:" + ex);
            }
            catch (Exception e)
            {
                Debug.LogError("[N-Testing] Exception when trying to fetch DVR values:" + e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        
        /// <summary>
        /// Determines if DVR.cs file is present in the project
        /// </summary>
        /// <returns></returns>
        private static bool TryGetDvrAssetPath(out string assetPath)
        {
            string[] dvrFiles = AssetDatabase.FindAssets("DVR");
            if (dvrFiles != null)
            {
                foreach (var dvrFile in dvrFiles)
                {
                    string innerAssetPath = AssetDatabase.GUIDToAssetPath(dvrFile);
                    if (innerAssetPath.EndsWith("/DVR.cs"))
                    {
                        assetPath = innerAssetPath;
                        return true;
                    }
                }
            }

            assetPath = null;
            return false;
        }
        
        private class DvrFetcherObject
        {
            public enum FetchMode
            {
                Automatic, Manual
            }

            private readonly FetchMode CurrentFetchMode;
            
            private float _progress;
            public float Progress
            {
                get => _progress;
                private set
                {
                    _progress = value;
                    OnProgressUpdated?.Invoke();
                }
            }

            public event Action OnProgressUpdated;


            public DvrFetcherObject(FetchMode mode)
            {
                CurrentFetchMode = mode;
            }
            
            public async Task StartUpdateDvrAsync(CancellationToken cancellationToken = default)
            {
                try
                {
                    DvrDefinition dvrDefinition = await Fetch(cancellationToken);
                        
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    WriteDvr(dvrDefinition);
                }
                catch (AggregateException aggregateException)
                {
                    DvrItemNoMatchException itemNoMatchException = (DvrItemNoMatchException)
                        aggregateException.InnerExceptions.FirstOrDefault(e => e is DvrItemNoMatchException);

                    if (itemNoMatchException != null)
                    {
                        OnDvrItemNoMatchException(itemNoMatchException);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DvrItemNoMatchException e)
                {
                    OnDvrItemNoMatchException(e);
                }
            }

            private void OnDvrItemNoMatchException(DvrItemNoMatchException e)
            {
                if (CurrentFetchMode == FetchMode.Manual)
                {
                    EditorUtility.DisplayDialog("Error while fetching N-Testing values",
                        "No remote configuration found for N-Testing for your game. " +
                        "If you are using N-Testing, please contact your Publish Manager. " +
                        "Otherwise you can safely ignore this error.", "OK");
                }
            }
            
            
            private async Task<DvrDefinition> Fetch(CancellationToken cancellationToken)
            {
                // Fetch iOS and Android configuration variables
                DvrModel ProgressUpdater(Task<DvrModel> fetchTask)
                {
                    Progress += 0.3f;
                    return fetchTask.Result;
                }
                
                PluginManifest manifest = PluginManifest.LoadFromLocalFile();
                if (manifest == null)
                {
                    // If Homa Belly Manifest not found, throw an exception
                    throw new Exception("Could not fetch N-Testing values as there is no Homa Belly Manifest installed");
                }
                
                Task<DvrModel> iosFetchTask = Fetch(DvrOs.I_OS, manifest).ContinueWith(ProgressUpdater, cancellationToken); 
                Task<DvrModel> androidFetchTask = Fetch(DvrOs.ANDROID, manifest).ContinueWith(ProgressUpdater, cancellationToken);

                await Task.WhenAll(iosFetchTask, androidFetchTask);

                var iosDvrModel = iosFetchTask.Result;
                var androidDvrModel = androidFetchTask.Result;
                return new DvrDefinition
                {
                    IOS = iosDvrModel.ToDefinition(), 
                    Android = androidDvrModel.ToDefinition(),
                    Default = iosDvrModel.FieldList.Count > androidDvrModel.FieldList.Count 
                        ? iosDvrModel.ToDefaultDefinition() : androidDvrModel.ToDefaultDefinition()
                };
            }
            
            private const string DYNAMIC_VARIABLES_REGISTER_TEMPLATE = "using System;\n" +
                                                                       "#if HOMA_BELLY\n" +
                                                                       "using HomaGames.Geryon;\n" +
                                                                       "#endif\n" +
                                                                       "\n" +
                                                                       "\npublic static class DVR {{\n" +
                                                                       "\t\n" +
                                                                       "\t#if UNITY_IOS && HOMA_BELLY\n" +
                                                                       "\n" +
                                                                       "{0}" +
                                                                       "\t\n" +
                                                                       "\t#elif HOMA_BELLY\n" +
                                                                       "\n" +
                                                                       "{1}" +
                                                                       "\t\n" +
                                                                       "\t#else\n" +
                                                                       "\n" +
                                                                       "{2}" +
                                                                       "\t\n" +
                                                                       "\t#endif\n" +
                                                                       "\t\n" +
                                                                       "}}\n";
            
            private void WriteDvr(DvrDefinition dvrDefinitions)
            {
                string dvrContent = string.Format(DYNAMIC_VARIABLES_REGISTER_TEMPLATE, dvrDefinitions.IOS, dvrDefinitions.Android, dvrDefinitions.Default);
                DeleteDvrFiles();
                Progress = 0.8f;
                WriteDvrFile(dvrContent);
                Progress = 1f;
                
                Debug.Log("DVR data fetched.");
            }
            
            #region DVR File helpers

            /// <summary>
            /// Writes the `DVRTemplate` to the `DVR.cs` file
            /// </summary>
            private static void WriteDvrFile(string content)
            {
                string dvrFilePath = Path.Combine(Application.dataPath, Constants.ABSOLUTE_DVR_PATH);
                EditorFileUtilities.CreateIntermediateDirectoriesIfNecessary(dvrFilePath);
                Debug.Log($"Writing file: {dvrFilePath}");
                File.WriteAllText(dvrFilePath, content);

                EditorApplication.delayCall += AssetDatabase.Refresh;
            }

            /// <summary>
            /// Delete any DVR.cs file
            /// </summary>
            private static void DeleteDvrFiles()
            {
                try
                {
                    if (TryGetDvrAssetPath(out var assetPath))
                    {
                        Debug.Log($"Deleting DVR file: {assetPath}");
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                }
                catch (Exception e)
                {
                    HomaGamesLog.Warning($"Exception deleting DVR files: {e.Message}");
                }
            }

            #endregion
            
            #region DVR Value Fetching
            private static readonly Dictionary<DvrOs, string> OsAgents = new Dictionary<DvrOs, string>
            {
                [DvrOs.I_OS] = "IPHONE",
                [DvrOs.ANDROID] = "ANDROID"
            };


            /// <summary>
            /// Obtains the default configuration variables for a given OS
            /// </summary>
            /// <param name="os">The OS to get the default configuration variables for</param>
            /// <param name="manifest">The current plugin manifest</param>
            private static async Task<DvrModel> Fetch(DvrOs os, PluginManifest manifest)
            {
                // Fetch from server the iOS configuration variables
                GeryonConfigurationModel configurationModel = await new EditorHttpCaller<GeryonConfigurationModel>()
                    .Get(GenerateDvrFetchUri(OsAgents[os], manifest), new ConfigurationResponseDeserializer());

                if (configurationModel.IsItemNoMatch())
                    throw new DvrItemNoMatchException();
                if (!configurationModel.IsStatusOk())
                    throw new Exception(
                        $"Error while fetching DVR code {configurationModel.ResultStatus}: {configurationModel.ResultMessage}");
                
                if (TryParseConfigJson(configurationModel, out DvrModel dvrModel))
                {
                    return dvrModel;
                }

                return new DvrModel();
            }

            private static string GenerateDvrFetchUri(string osAgent, PluginManifest pluginManifest)
            {
                return UriHelper.AddGetParameters(DVR_FETCHING_ENDPOINT, new Dictionary<string, string>
                {
                    {"cv", HomaBellyConstants.API_VERSION}, // Configuration version
                    {"av", Application.version}, // Application version
                    {"sv", HomaBellyConstants.PRODUCT_VERSION}, // SDK version
                    {"ua", osAgent}, // User Agent
                    {"ai", SystemConstants.ApplicationIdentifier}, // Application identifier
                    {"ti", pluginManifest.AppToken } // App Token
                });
            }

            private static bool TryParseConfigJson(GeryonConfigurationModel geryonConfigurationModel, out DvrModel result)
            {
                result = new DvrModel();
                Dictionary<string, Type> flagToTypes = new Dictionary<string, Type>
                {
                    [Constants.STRING_FLAG] = typeof(string),
                    [Constants.BOOL_FLAG] = typeof(bool),
                    [Constants.INT_FLAG] = typeof(int),
                    [Constants.FLOAT_FLAG] = typeof(double),
                };

                if (geryonConfigurationModel?.Configuration != null)
                {
                    // Iterate over all dynamic variables
                    foreach (KeyValuePair<string, object> pair in geryonConfigurationModel.Configuration)
                    {
                        try
                        {
                            // Obtain the variable key and the variable type (flag)
                            string key = pair.Key.ToUpperInvariant();
                            var flag = key.Substring(0, 2);
                            string fieldName = key.Substring(2);

                            // Avoid creating a duplicated variable name if already exists
                            if (result.FieldList.All(field => field.Name != fieldName))
                            {
                                if (flagToTypes.TryGetValue(flag, out var type))
                                {
                                    result.FieldList.Add(new DvrModel.DvrField
                                    {
                                        Key = key,
                                        Name = fieldName,
                                        Type = type,
                                        // reads the value
                                        Value = Convert.ChangeType(pair.Value, type, CultureInfo.InvariantCulture)
                                    });
                                }
                                else
                                {
                                    Debug.LogWarning(
                                        $"Cannot recognize standard type {pair.Value.GetType()} : please get in touch with your publishing manager.");
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"{fieldName} already defined. Skipping {key}");
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"There was an error reading the geryon model for configuration element {pair.Key}: {e}");
                        }
                    }
                }
                else
                {
                    return false;
                }
                
                return true;
            }
            #endregion

            private class DvrModel
            {
                public struct DvrField
                {
                    public string Name;
                    public string Key;
                    public Type Type;
                    public object Value;

                    public string TypeName => Type.Name;
                    
                    public string ValueDefinition {
                        get
                        {
                            string stringValue = Convert.ToString(Value, CultureInfo.InvariantCulture);
                            if (Type == typeof(string))
                                return $"\"{stringValue}\"";
                            if (Type == typeof(bool))
                                return stringValue?.ToLowerInvariant();

                            return stringValue;
                        }
                    }
                }

                public readonly List<DvrField> FieldList = new List<DvrField>();

                public string ToDefinition()
                {
                    return string.Join("\n", FieldList.Select(WriteField));
                }
                
                private string WriteField(DvrField field)
                {
                    return
                        $"\tpublic static {field.TypeName} {field.Name} => DynamicVariable<{field.TypeName}>.Get(\"{field.Key}\", {field.ValueDefinition});";
                }

                public string ToDefaultDefinition()
                {
                    return string.Join("\n", FieldList.Select(WriteDefaultField));
                }

                private string WriteDefaultField(DvrField field)
                {
                    return $"\tpublic static {field.TypeName} {field.Name} => default;";
                }
            }
        }
    }
}
