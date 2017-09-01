using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AdColony {
    public enum AdsIAPEngagementType {
        AdColonyIAPEngagementEndCard = 0,
        AdColonyIAPEngagementOverlay
    }

    public enum AdZoneType {
        AdColonyZoneTypeInterstitial = 0,
        AdColonyZoneTypeBanner,
        AdColonyZoneTypeNative
    }

    public enum AdOrientationType {
        AdColonyOrientationPortrait = 0,
        AdColonyOrientationLandscape,
        AdColonyOrientationAll
    }

    public enum AdMetadataGenderType {
        Unknown = 0,
        Male,
        Female
    }

    public enum AdMetadataMaritalStatusType {
        Unknown = 0,
        Single,
        Married
    }

    public enum AdMetadataEducationLevelType {
        Unknown = 0,
        GradeSchool,
        SomeHighSchool,
        HighSchool,
        SomeCollege,
        AssociateDegree,
        BachelorDegree,
        GraduateDegree
    }
}
