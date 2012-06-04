﻿using System;
using System.Data.Services.Common;

namespace NuGet
{
    public interface IPackageInfoType
    {
        bool ShowAll { get; set; }
        bool IsUnlisted { get; }
    }

    [DataServiceKey("Id", "Version")]
    [HasStreamAttribute]
    public class PackageInfo : IPackageInfoType
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }
        public int VersionDownloadCount { get; set; }
        public int DownloadCount { get; set; }
        public string PackageHash { get; set; }
        public Uri DownloadUrl { get; set; }
        public long PackageSize { get; set; }
        public bool ShowAll { get; set; }
        public DateTimeOffset? Published { get; set; }

        private bool? _isPrerelease;

        public bool IsPrerelease
        {
            get
            {
                if (_isPrerelease == null) 
                {
                    SemanticVersion value;
                    _isPrerelease = SemanticVersion.TryParse(Version, out value) && 
                                    !String.IsNullOrEmpty(value.SpecialVersion);
                }

                return _isPrerelease.Value;
            }
        }

        public bool IsUnlisted
        {
            get
            {
                return Published == Constants.Unpublished;
            }
        }

        public int EffectiveDownloadCount
        {
            get
            {
                return ShowAll ? VersionDownloadCount : DownloadCount;
            }
        }

        public bool IsLocalPackage
        {
            get
            {
                return DownloadUrl.IsFile;
            }
        }

        public DataServicePackage AsDataServicePackage()
        {
            return new DataServicePackage
                   {
                       Id = Id,
                       Version = Version,
                       Authors = Authors,
                       VersionDownloadCount = VersionDownloadCount,
                       DownloadCount = DownloadCount,
                       PackageHash = PackageHash,
                       Published = Published
                   };
        }
    }
}