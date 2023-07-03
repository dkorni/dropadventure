using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class CorePackageImportListener : AssetPostprocessor
{
    private const string FIRST_TIME_PREF_KEY = "homagames.core_already_in_project";
    
    private static bool _corePackageImportedThisSession;
    private static bool CorePackageImportedThisSession
    {
        get => _corePackageImportedThisSession;
        set
        {
            bool previousValue = _corePackageImportedThisSession;
            _corePackageImportedThisSession = value;
            if (!previousValue && _corePackageImportedThisSession)
                _onCorePackageImported?.Invoke();
        }
    }

    private static Action _onCorePackageImported;
    /// <summary>
    /// This callback will be called at least once for every core import/Homa Belly
    /// refreshes.<br /><br />
    /// Note: It may be called multiple time between imports. 
    /// </summary>
    public static event Action OnCorePackageImported
    {
        add
        {
            if (CorePackageImportedThisSession)
                value.Invoke();
            else
                _onCorePackageImported += value;
        }

        remove
        {
            if (_onCorePackageImported.GetInvocationList().Contains(value))
                _onCorePackageImported -= value;
        }
    }

    static CorePackageImportListener()
    {
        if (Application.isBatchMode)
            return;
        
        // This part will check all the HB refreshes, but not the first install
        AssetDatabase.importPackageCompleted += OnPackageImported;

        // This part will check the first install
        if (! PlayerPrefs.HasKey(FIRST_TIME_PREF_KEY))
        {
            PlayerPrefs.SetInt(FIRST_TIME_PREF_KEY, 1);
            CorePackageImportedThisSession = true;
        }
    }

    private static void OnPackageImported(string packageName)
    {
        packageName = packageName.ToLower();
        if (packageName.Contains("homabelly") && packageName.Contains("core"))
        {
            CorePackageImportedThisSession = true;
        }
    }
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
    {
        if (deletedAssets.Any(path => Path.GetFileName(path) == nameof(CorePackageImportListener) + ".cs"))
        {
            PlayerPrefs.DeleteKey(FIRST_TIME_PREF_KEY);
        }
    }
}
