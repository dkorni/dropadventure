using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HomaGames.HomaBelly.Installer;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class AvailablePackages
    {
        public AvailablePackages(int id,string name,string manifestVersionId,string androidBundleId,string iosBundleId,List<PackageComponent> packageComponents)
        {
            Id = id;
            Name = name;
            ManifestVersionId = manifestVersionId;
            AndroidBundleId = androidBundleId;
            IOSBundleId = iosBundleId;
            // Adding all package components to structures
            foreach (var packageComponent in packageComponents)
            {
                // Only one version per package is allowed
                if (idToVersion.ContainsKey(packageComponent.Id))
                    return;
                idToVersion.Add(packageComponent.Id,packageComponent.Version);
                idToPackageComponents.Add(packageComponent.Id,packageComponent);
                allPackages.Add(packageComponent);
            }
        }
        
        //[JsonProperty("i_manifest_id")]
        public int Id { get; }

        //[JsonProperty("s_manifest_name")]
        public string Name { get; }

        //[JsonProperty("s_manifest_version_id")]
        public string ManifestVersionId { get; }
        
        public string AndroidBundleId { get; }
        public string IOSBundleId { get; }

        //[JsonProperty("ao_core_packages")]
        /// <summary>
        /// Obsolete.
        /// Directly use <see cref="GetAllPackages"/> and filter by PackageType instead.
        /// </summary>
        public List<PackageComponent> CorePackages =>
            new List<PackageComponent>(allPackages.Where(p => p.PackageType == EditorPackageType.CORE_PACKAGE));
        //[JsonProperty("ao_mediation_layers")]
        /// <summary>
        /// Obsolete.
        /// Directly use <see cref="GetAllPackages"/> and filter by PackageType instead.
        /// </summary>
        public List<PackageComponent> MediationLayers=>
            new List<PackageComponent>(allPackages.Where(p => p.PackageType == EditorPackageType.MEDIATION_LAYER));
        //[JsonProperty("ao_attribution_platforms")]
        /// <summary>
        /// Obsolete.
        /// Directly use <see cref="GetAllPackages"/> and filter by PackageType instead.
        /// </summary>
        public List<PackageComponent> AttributionPlatforms=>
            new List<PackageComponent>(allPackages.Where(p => p.PackageType == EditorPackageType.ATTRIBUTION_PLATFORM));
        //[JsonProperty("ao_ad_networks")]
        /// <summary>
        /// Obsolete.
        /// Directly use <see cref="GetAllPackages"/> and filter by PackageType instead.
        /// </summary>
        public List<PackageComponent> AdNetworks=>
            new List<PackageComponent>(allPackages.Where(p => p.PackageType == EditorPackageType.AD_NETWORK));
        //[JsonProperty("ao_analytics_systems")]
        /// <summary>
        /// Obsolete.
        /// Directly use <see cref="GetAllPackages"/> and filter by PackageType instead.
        /// </summary>
        public List<PackageComponent> AnalyticsSystems=>
            new List<PackageComponent>(allPackages.Where(p => p.PackageType == EditorPackageType.ANALYTICS_SYSTEM));
        //[JsonProperty("ao_others")]
        /// <summary>
        /// Obsolete.
        /// Directly use <see cref="GetAllPackages"/> and filter by PackageType instead.
        /// </summary>
        public List<PackageComponent> Others=>
            new List<PackageComponent>(allPackages.Where(p => p.PackageType == EditorPackageType.OTHER));

        private readonly List<PackageComponent> allPackages = new List<PackageComponent>();
        

        private readonly Dictionary<string, string> idToVersion =
            new Dictionary<string, string>();

        private readonly Dictionary<string, PackageComponent> idToPackageComponents =
            new Dictionary<string, PackageComponent>();

        /// <summary>
        /// Obsolete.
        /// The use of <see cref="GetPackageComponent(string)"/> is now the only method to get packages from a Manifest.
        /// </summary>
        public PackageComponent GetPackageComponent(string id, string version)
        {
            return GetPackageComponent(id);
        }

        /// <summary>
        /// Obsolete.
        /// The use of <see cref="GetPackageComponent(string)"/> is now the only method to get packages from a Manifest.
        /// </summary>
        public PackageComponent GetPackageComponent(string id, string version, EditorPackageType type)
        {
            return GetPackageComponent(id);
        }

        /// <summary>
        /// Obsolete.
        /// The use of <see cref="GetPackageComponent(string)"/> is now the only method to get packages from a Manifest.
        /// </summary>
        public PackageComponent GetPackageComponent(string id, EditorPackageType type)
        {
            return GetPackageComponent(id);
        }

        /// <summary>
        /// Gets the PackageComponent for the corresponding package string identifier.
        /// </summary>
        /// <param name="id">Package id/name</param>
        [PublicAPI]
        public PackageComponent GetPackageComponent(string id)
        {
            return idToPackageComponents.TryGetValue(id, out PackageComponent package) ? package : null;
        }

        public string GetDependenciesAsTrackingString()
        {
            List<string> dependencies = new List<string>();
            foreach (var package in GetAllPackages())
            {
                dependencies.Add($"{package.Id}:{package.Version}");
            }

            return string.Join(",", dependencies);
        }

        public List<PackageComponent> GetAllPackages()
        {
            return allPackages;
        }

        public override string ToString()
        {
            string result = "\nAvailable Packages";

            result += "\n\nCore Packages\n----------";
            foreach (PackageComponent component in CorePackages)
            {
                result += ("\n" + component);
            }

            result += "\n\nMediation Layers\n----------";
            foreach (PackageComponent component in MediationLayers)
            {
                result += ("\n" + component);
            }

            result += "\n\nAttribution Systems\n----------";
            foreach (PackageComponent component in AttributionPlatforms)
            {
                result += ("\n" + component);
            }

            result += "\n\nAd Networks\n----------";
            foreach (PackageComponent component in AdNetworks)
            {
                result += ("\n" + component);
            }

            result += "\n\nAnalytics Systems\n----------";
            foreach (PackageComponent component in AnalyticsSystems)
            {
                result += ("\n" + component);
            }

            result += "\n\nOthers\n----------";
            foreach (PackageComponent component in Others)
            {
                result += ("\n" + component);
            }

            return result;
        }
    }
}