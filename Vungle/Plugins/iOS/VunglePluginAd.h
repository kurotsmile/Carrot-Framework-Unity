//
//  VunglePluginAd.h
//  Unity-iPhone
//

// TODO: Make this class instance based after re-write
@interface VunglePluginAd : NSObject {
}

- (instancetype)init NS_UNAVAILABLE;
- (instancetype)initWithPlacement:(NSString *)placementID;
+ (void)load:(NSString *)placementID;
+ (void)show:(NSString *)placementID options:(NSString *)options;
+ (BOOL)canShow:(NSString *)placementID;

@end
