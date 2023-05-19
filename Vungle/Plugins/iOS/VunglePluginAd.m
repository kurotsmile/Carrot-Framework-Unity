//
//  VunglePluginAd.m
//  Unity-iPhone
//

#import <Foundation/Foundation.h>
#import "VunglePluginAd.h"
#import <VungleSDK/VungleSDK.h>
#import "VungleSDKDelegate.h"
#import "VungleUtility.h"

@interface VunglePluginAd ()

@property (nonatomic, strong) NSString *placementID;

@end

@implementation VunglePluginAd

- (void)dealloc {
    self.placementID = nil;
}

#pragma mark - VungleAd interface

- (instancetype)initWithPlacement:(NSString *)placementID {
    _placementID = placementID;
    return self;
}

+ (void)load:(NSString *)placementID {
    if ([VunglePluginAd canShow:placementID]) {
        [[VungleSDKDelegate instance] vungleAdPlayabilityUpdate:YES placementID:placementID error:nil];
        return;
    }
    
    NSError *error;
    BOOL result = [[VungleSDK sharedSDK] loadPlacementWithID:placementID error:&error];
    if (result) {
        // Success!
        return;
    }
    
    if (error) {
        NSString *message = [NSString stringWithFormat:@"Vungle: Failed to request vungle ad %@ - %@", placementID, [error localizedDescription]];
        [VungleUtility sendErrorMessage:message];
        return;
    }
    
    NSString *message = [NSString stringWithFormat:@"Vungle: Failed to request vungle ad %@", placementID];
    [VungleUtility sendErrorMessage:message];
}

+ (void)show:(NSString *)placementID options:(NSString *)options {
    NSError *error;
    NSMutableDictionary *opt = [self parseOptions:options];
    BOOL result = [[VungleSDK sharedSDK] playAd:UnityGetGLViewController() options:opt placementID:placementID error:&error];
    if (result) {
        // Success!
        return;
    }
    
    if (error) {
        NSString *message = [NSString stringWithFormat:@"Vungle: Failed to display vungle ad %@ - %@", placementID, [error localizedDescription]];
        [VungleUtility sendErrorMessage:message];
        return;
    }
    
    NSString *message = [NSString stringWithFormat:@"Vungle: Failed to display vungle ad %@", placementID];
    [VungleUtility sendErrorMessage:message];
}

+ (BOOL)canShow:(NSString *)placementID {
    return [[VungleSDK sharedSDK] isAdCachedForPlacementID:placementID];
}

#pragma mark - Private

+ (NSMutableDictionary *)parseOptions:(NSString *)options {
    NSMutableDictionary *opt = nil;
    if (options.length) {
        opt = [NSMutableDictionary dictionary];
        NSDictionary *from = [VungleUtility objectFromJson:options];
        
        if ([from objectForKey:@"orientation"]) {
            opt[VunglePlayAdOptionKeyOrientations] = @([self makeOrientation:from[@"orientation"]]);
        }
        if ([from objectForKey:@"muted"]) {
            opt[VunglePlayAdOptionKeyStartMuted] = [from objectForKey:@"muted"];
        }
        if ([from objectForKey:@"userTag"]) {
            opt[VunglePlayAdOptionKeyUser] = from[@"userTag"];
        }
        if ([from objectForKey:@"alertTitle"]) {
            opt[VunglePlayAdOptionKeyIncentivizedAlertTitleText] = from[@"alertTitle"];
        }
        if ([from objectForKey:@"alertText"]) {
            opt[VunglePlayAdOptionKeyIncentivizedAlertBodyText] = from[@"alertText"];
        }
        if ([from objectForKey:@"closeText"]) {
            opt[VunglePlayAdOptionKeyIncentivizedAlertCloseButtonText] = from[@"closeText"];
        }
        if ([from objectForKey:@"continueText"]) {
            opt[VunglePlayAdOptionKeyIncentivizedAlertContinueButtonText] = from[@"continueText"];
        }
        if ([from objectForKey:@"ordinal"]) {
            opt[VunglePlayAdOptionKeyOrdinal] = [NSNumber numberWithUnsignedInteger:[from[@"ordinal"] integerValue]];
        }
    }
    return opt;
}

+ (UIInterfaceOrientationMask)makeOrientation:(NSNumber *)code {
    UIInterfaceOrientationMask orientationMask;
    int i = [code intValue];
    switch(i) {
        case 1:
            orientationMask = UIInterfaceOrientationMaskPortrait;
            break;
        case 2:
            orientationMask = UIInterfaceOrientationMaskLandscapeLeft;
            break;
        case 3:
            orientationMask = UIInterfaceOrientationMaskLandscapeRight;
            break;
        case 4:
            orientationMask = UIInterfaceOrientationMaskPortraitUpsideDown;
            break;
        case 5:
            orientationMask = UIInterfaceOrientationMaskLandscape;
            break;
        case 6:
            orientationMask = UIInterfaceOrientationMaskAll;
            break;
        case 7:
            orientationMask = UIInterfaceOrientationMaskAllButUpsideDown;
            break;
        default:
            orientationMask = UIInterfaceOrientationMaskAllButUpsideDown;
    }
    return orientationMask;
}

@end
