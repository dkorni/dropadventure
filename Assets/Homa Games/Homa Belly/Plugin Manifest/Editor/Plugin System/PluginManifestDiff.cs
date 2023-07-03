using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class PluginManifestDiff
    {
        private PluginManifest _previousManifest;
        private PluginManifest _latestManifest;

        public PluginManifestDiff([CanBeNull] PluginManifest previousManifest, [CanBeNull] PluginManifest newManifest)
        {
            _previousManifest = previousManifest;
            _latestManifest = newManifest;

            if (newManifest == null)
                return;
            if (previousManifest == null)
            {
                foreach (var package in newManifest.Packages.GetAllPackages())
                {
                    NewPackageVersions.Add(package.Id, package);
                    NewPackages.Add(package.Id, package);
                }

                return;
            }

            var previousPackages = previousManifest.Packages.GetAllPackages();
            var newPackages = newManifest.Packages.GetAllPackages();
            foreach (var previousPackage in previousPackages)
            {
                var packageInNewManifest = newManifest.Packages.GetPackageComponent(previousPackage.Id);
                if (packageInNewManifest != null)
                {
                    if (CompareVersion(packageInNewManifest.Version, previousPackage.Version) > 0)
                    {
                        OutDatedPackages.Add(previousPackage.Id, previousPackage);
                        NewPackageVersions.Add(packageInNewManifest.Id, packageInNewManifest);
                    }

                    if (CompareVersion(packageInNewManifest.Version, previousPackage.Version) < 0)
                    {
                        DevPackages.Add(previousPackage.Id, previousPackage);
                    }
                }
                else
                {
                    RemovedPackages.Add(previousPackage.Id, previousPackage);
                }
            }

            foreach (var newPackage in newPackages)
            {
                if (previousManifest.Packages.GetPackageComponent(newPackage.Id) == null)
                {
                    NewPackages.Add(newPackage.Id, newPackage);
                    NewPackageVersions.Add(newPackage.Id, newPackage);
                }
            }
        }

        private int CompareVersion(string v1, string v2)
        {
            var v1Split = v1.Split('.');
            var v2Split = v2.Split('.');
            var lenghtDiff = v1.Length - v2.Length;
            if (lenghtDiff > 0)
                Array.Resize(ref v2Split, v1Split.Length);
            if (lenghtDiff < 0)
                Array.Resize(ref v1Split, v2Split.Length);
            for (int i = 0; i < v1Split.Length; i++)
            {
                var compare = String.CompareOrdinal(v1Split[i], v2Split[i]);
                if (compare != 0)
                    return compare;
            }

            return 0;
        }

        /// <summary>
        /// List new version of packages needing an update
        /// </summary>
        public readonly Dictionary<string, PackageComponent> OutDatedPackages =
            new Dictionary<string, PackageComponent>();

        /// <summary>
        /// List of packages removed
        /// </summary>
        public readonly Dictionary<string, PackageComponent> RemovedPackages =
            new Dictionary<string, PackageComponent>();

        /// <summary>
        /// List of packages that weren't previously installed
        /// </summary>
        public readonly Dictionary<string, PackageComponent> NewPackages = new Dictionary<string, PackageComponent>();

        /// <summary>
        /// List of packages that are new, including new versions. Those probably need to be downloaded.
        /// </summary>
        public readonly Dictionary<string, PackageComponent> NewPackageVersions =
            new Dictionary<string, PackageComponent>();

        /// <summary>
        /// List of packages that have a more recent version locally than remotely.
        /// These are probably being worked on locally.
        /// </summary>
        public readonly Dictionary<string, PackageComponent> DevPackages = new Dictionary<string, PackageComponent>();

        /// <summary>
        /// Returns the new latest version for a package if there is one.
        /// </summary>
        /// <param name="package">The package id.</param>
        /// <param name="newVersion">The new version.</param>
        public bool TryGetNewLatestVersion(string package, out string newVersion)
        {
            newVersion = "";
            if (_latestManifest == null ||
                !OutDatedPackages.TryGetValue(package, out PackageComponent packageComponent)) return false;
            newVersion = _latestManifest.Packages.GetPackageComponent(packageComponent.Id).Version;
            return true;
        }

        public bool IsPackageInDevelopment(string package)
        {
            return DevPackages.ContainsKey(package);
        }
    }
}