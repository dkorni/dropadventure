PACKAGE=publishing

VERSION=1.34.0
DATE=2023-04-17

================================================================================ VERSION 1.34.0
- Removed level_finished Event for Tenjin
- Added analytics for iOS consent flow
- Improved AndroidManifest handling
- Minor improvements :
    - Update Unity IAP auto install
    - Fixed ImportConfig in menu
- Upgrade Max

================================================================================ VERSION 1.33.2
- Fix bitcode auto Disable

================================================================================ VERSION 1.33.1
- Improved iOS consent flow
- Minor Improvements :
    - Improved "settings" window
    - Improved French translation
    - ShowInterstitial now accepts no parameter

================================================================================ VERSION 1.33.0
- Added iOS ATT Consent Flow (Disabled MAX one)
- Minor Improvements :
    - Automatically disabling bitcode
- Upgrade MAX

================================================================================ VERSION 1.32.2
- Bug fixes

================================================================================ VERSION 1.32.1
- Added possible Interstitial delay override for easier ABs  
- Minor improvements and bug fixes :
    - Fix inapps in editor for Android platform
    - i18n improvement
- Upgrade MAX

================================================================================ VERSION 1.32.0
- Update Facebook 14.1.0
- Removed Google Ad Manager
- Minor improvements and bug fixes :
    - Added YcEnumShow attribute
    - Fix YCCustomWindows for Unity 2019 and lower
    - Added StopEffect for sounds
    - Added Tenjin events
- Upgrade MAX

================================================================================ VERSION 1.31.0
- Added YsoNetwork Adapters
- Added back MyTarget
- Minor improvements and bug fixes :
    - Added inapp Init Event (for safer price display, see documentation)
    - Fix DebugWindow Reset
    - Clearer UpdateGUWindow
    - fix YCConfigData loading for menu
- Update EDM4U 1.2.175
- Upgrade MAX

================================================================================ VERSION 1.30.0
- Fix Amazon Integration
- New features in the top menu
- Upgrade MAX

================================================================================ VERSION 1.29.0
- Added inapp purchases local verification
- Added Amazon support
- Added TextMesPro handling for i18n translations
- Added Tenjin ILRD for Android
- Update Unity IAP 4.4.1 for Unity 2020.3+
- Upgrade MAX
- Fixes and adjustments

================================================================================ VERSION 1.28.0
- Update External Dependency Manager (1.2.172)
- Removed MyTarget from Applovin
- Added Verve in Applovin
- Added AD_ID Android permission
- Minor improvements
- Upgrade MAX

================================================================================ VERSION 1.27.1
- Fix bug with Mintegral network in MAX

================================================================================ VERSION 1.27.0
- Added the Tool "Debug Window" in the Gameutils top menu
- Added Tenjin events
- Added custom events for analytics
    Use this : YCManager.instance.analyticsManager.AddCustomEvent(int id);
    The parameter must be between 0 and 15 included
- Upgrade MAX
- Bug fixes

================================================================================ VERSION 1.26.1
- Upgrade MAX
- Minor bug fixes

================================================================================ VERSION 1.26.0
- Upgrade MAX
- Added a Game Utils Menu
- Improved log system (in YCConfigData)
- New analytics data

================================================================================ VERSION 1.25.2
- InApp Bug Fix

================================================================================ VERSION 1.25.1
- InApp Bug Fix

================================================================================ VERSION 1.25.0
- Update External Dependency Manager (1.2.170)
- Improve Sound Manager
- Improve Import Config Popup
- Improve InApps
- Update Max (Removed MyTarget)
- Bug Fixes
 
================================================================================ VERSION 1.23.0
- Added Firebase tROAS event (RedRock)
- Update Max

================================================================================ VERSION 1.22.1
- Update Max

================================================================================ VERSION 1.22.0
- Update Max
- Removed YCJson
- Use NewtonJson
- Disable Crosspromo when not on Android or iOS

================================================================================ VERSION 1.21.0
- Update Max 5.1.1
- Update Mediations
- Add Google Ad Manager
- Add YCJson
- Remove NewtonJson on 

================================================================================ VERSION 1.20.1
- Update Max 5.0.1
- YcConfig BannerDisplayOnInitEditor
- ResetTransform
- WinLose

================================================================================ VERSION 1.20.0
- Bug IDFA Analytics
- Update Mediation

================================================================================ VERSION 1.19.0
- Update Mediation
- Upgrade Tenjin 1.12.7

================================================================================ VERSION 1.18.1
Update Mediation
Set MaxSdk.SetUserId

================================================================================ VERSION 1.18.0
Update Mediation
Analytics add tenjin informations
Analytics add display ads informations

================================================================================ VERSION 1.17.0
Update Mediation
Improve Fps

================================================================================ VERSION 1.16.0
remove Firebase SDK
Update Max
Update GDPR
Add IsInterstitialOrRewardedVisible in AdManager

================================================================================ VERSION 1.15.1
Add removed files firebase

================================================================================ VERSION 1.15.0
Can import Firebase buy clicking inport config
Default AbMaxPercentage
Display AB version in setting
Remove Google in Mediation

================================================================================ VERSION 1.14.3
Add Firebase

================================================================================ VERSION 1.14.2
Fix "R" shortcut
Added game version in the setting Manager
Fix traduction Win/Lose (i18 Element was missing)
Update Max
Load maps when needed
Debug Max

================================================================================ VERSION 1.14.1
Bug Build

================================================================================ VERSION 1.14.0
Update MAX
Correct InApp with IN_APP_PURCHASING

================================================================================ VERSION 1.13.0
NSAdvertisingAttributionReportEndpoint => https://tenjin-skan.com
Update MAX
gitignore ignores Rider files

================================================================================ VERSION 1.12.5
Update MAX
Default ads 20 seconds
Bug fps unscale
Add sdk_version in analytics

================================================================================ VERSION 1.12.4
Bug Tenjin Android

================================================================================ VERSION 1.12.3
Update SoundManager
Update MAX
Update Tenjin
Removed android:debuggable=true

================================================================================ VERSION 1.12.2
Bug SoundManager

================================================================================ VERSION 1.12.1
Change AB other => control
Bug FB with ATT

================================================================================ VERSION 1.12.0
Update FB with ATT
Update Mediation
Update SoundManager

================================================================================ VERSION 1.11.0
Update I18n
YcManager Add NoInternetManager
Bug event Unity <= 2019

================================================================================ VERSION 1.10.0
Update Max
Update FB 11.0.0
Game Invokes OnStateChanged

================================================================================ VERSION 1.9.1
BUG Tenjin android
Update Mediation

================================================================================ VERSION 1.8.4
Optimisation AbTesting
Keep random map on lose
Update MAX
Update Tenjin

================================================================================ VERSION 1.8.3
Bug multiscene YcManager

================================================================================ VERSION 1.8.2
Can Build IOS without module

================================================================================ VERSION 1.8.1
Bug FB

================================================================================ VERSION 1.8.0
Update Max
Update FB 9.2.0
Remove consent flow on start

================================================================================ VERSION 1.7.2
Bug setting
Bug build android
Update Max

================================================================================ VERSION 1.7.1
Bug notification

================================================================================ VERSION 1.7.0
Update Max
Enable ATT
Analytics add app_tracking_status session
Analytics add push_notif_status session

================================================================================ VERSION 1.6.1
Bug PushNotification
Update FB
Update Max

================================================================================ VERSION 1.6.0
Add PushNotification
[EXT] Array RemoveAt

================================================================================ VERSION 1.5.0
Smatter setting FB
Smatter setting MAX
Move script
Add BannerDisplayOnInit in YcConfig
Add More Analytics infos :
public class AppData {
    ...
    public string device_model;
    public string device_os_version;
    public int device_memory_size;
    public int device_processor_count;
    public int device_processor_frequency;
    public string device_processor_type;
    public SessionData session = {
        ...
        public int fps;
    }
    public Dictionary<string, int> events = {
        ...
        banner_show = 0;
        banner_click = 0;
    }
}

================================================================================ VERSION 1.4.5
Update MAX Bug

================================================================================ VERSION 1.4.4
Update MAX

================================================================================ VERSION 1.4.3
Remove Asking Tracking link Tenjin
Review Design Buttons

================================================================================ VERSION 1.4.2
Desable Consent Flow IOS 14
Move _.gitgnore into Assets
Update MAX

================================================================================ VERSION 1.4.1
Add OpenKeyboard in YCBehaviour
Add Object and Array in ADataManager
Add DuplicateReadable in YcTexture2DExtensions
Update MAX
Add Default _.gitgnore (remove _ when import)

================================================================================ VERSION 1.4.0
Change workflow Max for IOS 14
Change workflow Tenjin for IOS 14
Check if google exist before check AdMobIds

================================================================================ VERSION 1.3.1
BUG compilation when InApp Purchase Not Activate

================================================================================ VERSION 1.3.0
Add Default Workflow Maps
Add Shortcuts a, z, w, l
InApp Purchases only activate if service activate
Review Template (Menu Win, Menu Lose)