#import <AdColony/AdColony.h>
#import "UnityAppController.h"
#import "PluginBase/AppDelegateListener.h"
#import "AdColonyUnityConstants.h"
#import "AdColonyUnityJson.h"


// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil

#define NSSTRING_OR_EMPTY(x)                        (x ? x : @"")
#define NSDICTIONARY_OR_EMPTY(x)                    (x ? x : @{})
#define IS_STRING_SET(x)                            (x && ![x isEqualToString:@""])

void UnitySendMessage(const char *className, const char *methodName, const char *param);

void SafeUnitySendMessage(const char *className, const char *methodName, const char *param) {
    if (className == NULL) {
        NSLog(@"Invalid className for UnitySendMessage, make sure ManagerName is set in platform object constructor.");
    }
    if (methodName == NULL) {
        methodName = "";
    }
    if (param == NULL) {
        param = "";
    }
    UnitySendMessage(className, methodName, param);
}

NSString *getGUID() {
    CFUUIDRef newUniqueId = CFUUIDCreate(kCFAllocatorDefault);
    NSString *uuidString = (__bridge_transfer NSString *)CFUUIDCreateString(kCFAllocatorDefault, newUniqueId);
    CFRelease(newUniqueId);
    return uuidString;
}


@interface AdcAdsUnityController : NSObject
@property (nonatomic, copy) NSString *managerName;
@property (nonatomic, copy) NSString *adapterVersion;
@property (nonatomic, strong) NSMutableDictionary *interstitialAds;
@property (nonatomic, copy) NSString *appOptionsJson;
@end

@implementation AdcAdsUnityController

#pragma mark -

+ (AdcAdsUnityController *)sharedInstance {
    static AdcAdsUnityController * instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{ instance = [[AdcAdsUnityController alloc] init]; });
    return instance;
}

- (id)init {
    if (self = [super init]) {
        self.interstitialAds = @{}.mutableCopy;
    }
    return self;
}

@end


// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C" {

    // Ads

    void _AdcSetManagerNameAds(const char *managerName, const char *adapterVersion) {
        [AdcAdsUnityController sharedInstance].managerName = GetStringParam(managerName);
        [AdcAdsUnityController sharedInstance].adapterVersion = GetStringParam(adapterVersion);
    }

    void _AdcConfigure(const char *appId_, const char *appOptionsJson_, int zoneIdsCount_, const char *zoneIds_[]) {
        NSString *appId = GetStringParamOrNil(appId_);

        NSMutableArray *zoneIds = @[].mutableCopy;
        for (int i = 0; i < zoneIdsCount_; ++i) {
            [zoneIds addObject:GetStringParamOrNil(zoneIds_[i])];
        }

        NSString *appOptionsJson = GetStringParamOrNil(appOptionsJson_);
        [AdcAdsUnityController sharedInstance].appOptionsJson = appOptionsJson;

        // SDK-40 appOptions can no longer be nil all the time; the metadata
        // regarding the plugin must be set now
        AdColonyAppOptions *appOptions = [[AdColonyAppOptions alloc] init];
        if (appOptionsJson) {
            [appOptions setupWithJson:appOptionsJson];
        }
        [appOptions setPlugin:ADCUnity];
        [appOptions setPluginVersion:[AdcAdsUnityController sharedInstance].adapterVersion];

        [AdColony configureWithAppID:appId zoneIDs:zoneIds options:appOptions completion:^(NSArray<AdColonyZone *> *zones) {
            NSMutableArray *zoneJsonArray = [NSMutableArray array];
            for (AdColonyZone *zone in zones) {
                [zoneJsonArray addObject:[zone toJsonString]];

                if (zone.rewarded) {
                    // For each zone returned that is also a rewarded zone, register a callback that will then call _OnRewardGranted.
                    NSString *zoneID = zone.identifier;
                    [zone setReward:^(BOOL success, NSString * _Nonnull name, int amount) {
                        NSDictionary *rewardDict = @{ADC_ON_REWARD_GRANTED_ZONEID_KEY  : zoneID,
                                                     ADC_ON_REWARD_GRANTED_SUCCESS_KEY : [NSNumber numberWithBool:success],
                                                     ADC_ON_REWARD_GRANTED_NAME_KEY    : name,
                                                     ADC_ON_REWARD_GRANTED_AMOUNT_KEY  : [NSString stringWithFormat:@"%d", amount]};
                        SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnRewardGranted", MakeStringCopy([rewardDict toJsonString]));
                    }];
                }
            }

            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnConfigure", MakeStringCopy([zoneJsonArray toJsonString]));
        }];
    }

    const char *_AdcGetSDKVersion() {
        return MakeStringCopy([AdColony getSDKVersion]);
    }

    void _AdcShowInterstitialAd(const char *id) {
        NSString *adId = GetStringParamOrNil(id);
        if (adId) {
            AdColonyInterstitial *ad = [[AdcAdsUnityController sharedInstance].interstitialAds objectForKey:adId];
            if (ad) {
                UnityAppController* unityAppController = GetAppController();
                [ad showWithPresentingViewController:unityAppController.rootViewController];
                return;
            }
        }
    }

    void _AdcCancelInterstitialAd(const char *id) {
        NSString *adId = GetStringParamOrNil(id);
        if (adId) {
            AdColonyInterstitial *ad = [[AdcAdsUnityController sharedInstance].interstitialAds objectForKey:adId];
            if (ad) {
                [ad cancel];
                return;
            }
        }
    }

    void _AdcDestroyInterstitialAd(const char *id) {
        NSString *adId = GetStringParamOrNil(id);
        [[AdcAdsUnityController sharedInstance].interstitialAds removeObjectForKey:adId];
    }

    void _AdcRequestInterstitialAd(const char *zoneId, const char *adOptionsJson) {
        NSString *myAdOptionsJson = GetStringParamOrNil(adOptionsJson);
        AdColonyAdOptions *adOptions = nil;
        if (myAdOptionsJson) {
            adOptions = [[AdColonyAdOptions alloc] init];
            [adOptions setupWithJson:myAdOptionsJson];
        }

        NSString *zoneIdStr = GetStringParam(zoneId);

        [AdColony requestInterstitialInZone:zoneIdStr
                                    options:adOptions
                                    success:^(AdColonyInterstitial *ad) {
                                        NSString *adId = getGUID();
                                        [AdcAdsUnityController sharedInstance].interstitialAds[adId] = ad;
                                        NSDictionary *dict = @{@"zone_id": ad.zoneID,
                                                               @"expired": @(ad.expired),
                                                               @"audio_enabled": @(ad.audioEnabled),
                                                               @"iap_enabled": @(ad.iapEnabled),
                                                               @"id": adId};

                                        // Setup callbacks
                                        [ad setOpen:^{
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnOpened", MakeStringCopy([dict toJsonString]));
                                        }];
                                        [ad setClose:^{
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnClosed", MakeStringCopy([dict toJsonString]));
                                        }];
                                        [ad setAudioStop:^{
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnAudioStopped", MakeStringCopy([dict toJsonString]));
                                        }];
                                        [ad setAudioStart:^{
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnAudioStarted", MakeStringCopy([dict toJsonString]));
                                        }];
                                        [ad setExpire:^{
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnExpiring", MakeStringCopy([dict toJsonString]));
                                        }];
                                        [ad setLeftApplication:^{
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnLeftApplication", MakeStringCopy([dict toJsonString]));
                                        }];
                                        [ad setClick:^{
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnClicked", MakeStringCopy([dict toJsonString]));
                                        }];
                                        [ad setIapOpportunity:^(NSString * _Nonnull iapProductID, AdColonyIAPEngagement engagement) {
                                            NSMutableDictionary *mutableDict = dict.mutableCopy;
                                            [mutableDict setObject:iapProductID forKey:ADC_ON_IAP_OPPORTUNITY_IAP_PRODUCT_ID_KEY];
                                            [mutableDict setObject:@((int)engagement) forKey:ADC_ON_IAP_OPPORTUNITY_ENGAGEMENT_KEY];
                                            SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnIAPOpportunity", MakeStringCopy([mutableDict toJsonString]));
                                        }];

                                        SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnRequestInterstitial", MakeStringCopy([dict toJsonString]));
                                    }
                                    failure:^(AdColonyAdRequestError *error) {
                                        NSDictionary *dict = @{@"error_code": @(error.code),
                                                               @"zone_id": zoneIdStr};
                                        SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnRequestInterstitialFailed", MakeStringCopy([dict toJsonString]));
                                    }];
    }

    // should return JSON
    const char *_AdcGetZone(const char *zoneId) {
        NSString *zoneString = GetStringParamOrNil(zoneId);
        if (zoneString == nil) {
            return nil;
        }

        AdColonyZone *zone = [AdColony zoneForID:zoneString];
        if (zone == nil) {
            return nil;
        }

        return MakeStringCopy([zone toJsonString]);
    }

    const char *_AdcGetUserID() {
        return MakeStringCopy([AdColony getUserID]);
    }

    void _AdcSetAppOptions(const char *appOptionsJson) {
        [AdcAdsUnityController sharedInstance].appOptionsJson = GetStringParam(appOptionsJson);

        AdColonyAppOptions *appOptions = [[AdColonyAppOptions alloc] init];
        [appOptions setupWithJson:[AdcAdsUnityController sharedInstance].appOptionsJson];
        [AdColony setAppOptions:appOptions];
    }

    const char *_AdcGetAppOptions() {
        return MakeStringCopy([AdcAdsUnityController sharedInstance].appOptionsJson);
    }

    void _AdcSendCustomMessage(const char *type, const char *content) {
        NSString *typeString = GetStringParamOrNil(type);
        if (typeString != nil) {
            [AdColony sendCustomMessageOfType:typeString
                                  withContent:GetStringParamOrNil(content)
                                        reply:^(id _Nullable obj) {
                                            if ([obj isKindOfClass:[NSString class]]) {
                                                NSDictionary *messageDictionary = @{ADC_ON_CUSTOM_MESSAGE_RECEIVED_TYPE_KEY    : typeString,
                                                                                    ADC_ON_CUSTOM_MESSAGE_RECEIVED_MESSAGE_KEY : GetStringParam(content)};
                                                SafeUnitySendMessage(MakeStringCopy([AdcAdsUnityController sharedInstance].managerName), "_OnCustomMessageRecieved", MakeStringCopy([messageDictionary toJsonString]));
                                            }
                                        }];
        }
    }

    void _AdcLogInAppPurchase(const char *transactionId, const char *productId, int purchasePriceMicro, const char *currencyCode) {
        [AdColony iapCompleteWithTransactionID:GetStringParam(transactionId)
                                     productID:GetStringParam(productId)
                                         price:[NSNumber numberWithInt:purchasePriceMicro]
                                  currencyCode:GetStringParamOrNil(currencyCode)];
    }

    void _AdcLogTransactionWithID(const char *itemID, int quantity, double price, const char *currencyCode, const char *receipt, const char *store, const char *description) {
        [AdColonyEventTracker logTransactionWithID:GetStringParam(itemID)
            quantity:(NSInteger)quantity
            price:[NSNumber numberWithDouble:price]
            currencyCode:GetStringParam(currencyCode)
            receipt:GetStringParam(receipt)
            store:GetStringParam(store)
            description:GetStringParam(description)];
    }
    void _AdcLogCreditsSpentWithName(const char *name, int quantity, double value, const char *currencyCode) {
        [AdColonyEventTracker logCreditsSpentWithName:GetStringParam(name)
            quantity:(NSInteger)quantity
            value:[NSNumber numberWithDouble:value]
            currencyCode:GetStringParam(currencyCode)];
    }
    void _AdcLogPaymentInfoAdded() {
        [AdColonyEventTracker logPaymentInfoAdded];
    }
    void _AdcLogAchievementUnlocked(const char *description) {
        [AdColonyEventTracker logAchievementUnlocked:GetStringParam(description)];
    }
    void _AdcLogLevelAchieved(int level) {
        [AdColonyEventTracker logLevelAchieved:(NSInteger)level];
    }
    void _AdcLogAppRated() {
        [AdColonyEventTracker logAppRated];
    }
    void _AdcLogActivated() {
        [AdColonyEventTracker logActivated];
    }
    void _AdcLogTutorialCompleted() {
        [AdColonyEventTracker logTutorialCompleted];
    }
    void _AdcLogSocialSharingEventWithNetwork(const char *network, const char *description) {
        [AdColonyEventTracker logSocialSharingEventWithNetwork:GetStringParam(network) description:GetStringParam(description)];
    }
    void _AdcLogRegistrationCompletedWithMethod(const char *method, const char *description) {
        [AdColonyEventTracker logRegistrationCompletedWithMethod:GetStringParam(method) description:GetStringParam(description)];
    }
    void _AdcLogCustomEvent(const char *event, const char *description) {
        [AdColonyEventTracker logCustomEvent:GetStringParam(event) description:GetStringParam(description)];
    }
    void _AdcLogAddToCartWithID(const char *itemID) {
        [AdColonyEventTracker logAddToCartWithID:GetStringParam(itemID)];
    }
    void _AdcLogAddToWishlistWithID(const char *itemID) {
        [AdColonyEventTracker logAddToWishlistWithID:GetStringParam(itemID)];
    }
    void _AdcLogCheckoutInitiated() {
        [AdColonyEventTracker logCheckoutInitiated];
    }
    void _AdcLogContentViewWithID(const char *contentID, const char *contentType) {
        [AdColonyEventTracker logContentViewWithID:GetStringParam(contentID) contentType:GetStringParam(contentType)];
    }
    void _AdcLogInvite() {
        [AdColonyEventTracker logInvite];
    }
    void _AdcLogLoginWithMethod(const char *method) {
        [AdColonyEventTracker logLoginWithMethod:GetStringParam(method)];
    }
    void _AdcLogReservation() {
        [AdColonyEventTracker logReservation];
    }
    void _AdcLogSearchWithQuery(const char *queryString) {
        [AdColonyEventTracker logSearchWithQuery:GetStringParam(queryString)];
    }
    void _AdcLogEvent(const char *name, const char *dataAsJson) {
        NSString *dataAsJsonStr = GetStringParamOrNil(dataAsJson);
        NSDictionary *dict = [dataAsJsonStr jsonStringToDictionary];
        if (dict) {
            [AdColonyEventTracker logEvent:GetStringParamOrNil(name) withDictionary:[dict mutableCopy]];
        }
    }
}
