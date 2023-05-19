//
//  VungleBanner.mm
//  Vungle Unity Plugin 6.9.0
//
//  Copyright (c) 2013-Present Vungle Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "VungleBanner.h"
#import <VungleSDK/VungleSDK.h>
#import "VungleUtility.h"
#import "VungleSDKDelegate.h"

@interface VungleBanner ()

@property (nonatomic, readwrite, assign) BOOL muteIsSet;
@property (nonatomic, readwrite, assign) BOOL muted;
@property (nonatomic, strong) UIView *adViewContainer;
@property (nonatomic, strong) UIViewController *viewController;
@property (nonatomic, strong) NSLayoutConstraint* firstConstraint;
@property (nonatomic, strong) NSLayoutConstraint* secondConstraint;
@property (nonatomic) int x;
@property (nonatomic) int y;
@property (nonatomic, strong) NSString *placementID;

@end

@implementation VungleBanner

- (void)dealloc {
    self.adViewContainer = nil;
    self.viewController = nil;
    self.firstConstraint = nil;
    self.secondConstraint = nil;
    self.placementID = nil;
}

#pragma mark - Banner interface

- (instancetype)initWithPlacement:(NSString *)placementID size:(int)size position:(int)position viewController:(UIViewController *)viewController {
    _bannerSize = size;
    _bannerPosition = position;
    _placementID = placementID;
    _viewController = viewController;
    return self;
}

- (void)loadBanner {
    // Check if requested size is "MREC" or not. If MREC, `adSize` is not necessary.
    NSError *error;
    BOOL result;
    if (self.bannerSize != VunglePluginAdSizeBannerMrec) {
        VungleAdSize requestedAdSize = [VungleBanner getVungleBannerSize:self.bannerSize];
        if ([[VungleSDK sharedSDK] isAdCachedForPlacementID:self.placementID withSize:requestedAdSize]) {
            [[VungleSDKDelegate instance] vungleAdPlayabilityUpdate:YES placementID:self.placementID error:nil];
            return;
        }
        result = [[VungleSDK sharedSDK] loadPlacementWithID:self.placementID withSize:requestedAdSize error:&error];
    } else {
        if ([[VungleSDK sharedSDK] isAdCachedForPlacementID:self.placementID]) {
            [[VungleSDKDelegate instance] vungleAdPlayabilityUpdate:YES placementID:self.placementID error:nil];
            return;
        }
        result = [[VungleSDK sharedSDK] loadPlacementWithID:self.placementID error:&error];
    }
    if (!result) {
        if (error) {
            if (error.code == VungleSDKResetPlacementForDifferentAdSize) {
                // This is fine for banners. The placement will auto-reload the new size
                return;
            }
            NSString *message = [NSString stringWithFormat:@"Vungle: Failed to request banner %@ - %@", self.placementID, [error localizedDescription]];
            [VungleUtility sendErrorMessage:message];
        } else {
            NSString *message = [NSString stringWithFormat:@"Vungle: Failed to request banner %@", self.placementID];
            [VungleUtility sendErrorMessage:message];
        }
    }
}

- (void)showBanner:(NSString *)options {
    if (self.adViewContainer) {
        // already showing banner so return success
        return;
    }

    [self createViewContainer];
    [self.viewController.view addSubview:self.adViewContainer];
    [self.viewController.view bringSubviewToFront:self.adViewContainer];
    [self positionViewContainer];

    NSMutableDictionary* playOptions = [self parseOptions:options];
    NSError *error;
    BOOL succeed = [VungleSDK.sharedSDK addAdViewToView:self.adViewContainer withOptions:playOptions placementID:self.placementID error:&error];
    if (!succeed) {
        [VungleUtility sendErrorMessage:[NSString stringWithFormat:@"Vungle: Failed to show banner %@ with error %@", self.placementID, [error localizedDescription]]];
        [self closeBanner];
    } else {
        // Need to pause the audio session or there will be overlapping audio for MREC
        if (self.bannerSize == VunglePluginAdSizeBannerMrec) {
            if (self.muteIsSet && self.muted) {
                NSLog(@"Vungle: Pausing audio session");
                [VungleUtility pauseAudioSession];
            }
        }
    }
}

- (NSMutableDictionary *)parseOptions:(NSString *)options {
    NSMutableDictionary *opt = [NSMutableDictionary dictionary];
    NSError *error;
    if (options.length) {
        NSData *data = [NSData dataWithBytes:options.UTF8String length:options.length];
        NSDictionary *json = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingAllowFragments error:&error];
        if ([json objectForKey:@"muted"]) {
            self.muteIsSet = YES;
            self.muted = [[json objectForKey:@"muted"] boolValue];
            opt[VunglePlayAdOptionKeyStartMuted] = @(self.muted);
        }
    }
    return opt;
}

- (void)closeBanner {
    NSLog(@"Vungle: Reverting audio session");
    [[VungleSDK sharedSDK] finishDisplayingAd:self.placementID];
    [VungleUtility revertAudioSession];
    [self.adViewContainer removeFromSuperview];
}

#pragma mark - Conversion methods
+ (VungleAdSize) getVungleBannerSize:(int)adSizeIndex {
    VungleAdSize adSize;
    switch (adSizeIndex) {
        case VunglePluginAdSizeBanner:
            adSize = VungleAdSizeBanner;
            break;
        case VunglePluginAdSizeBannerShort:
            adSize = VungleAdSizeBannerShort;
            break;
        case VunglePluginAdSizeBannerLeaderboard:
            adSize = VungleAdSizeBannerLeaderboard;
            break;
        case VunglePluginAdSizeUnknown:
        default:
            [VungleUtility sendWarningMessage:@"Vungle: Unknown banner size provided."];
            adSize = VungleAdSizeUnknown;
            break;

    }
    return adSize;
}

- (VungleBannerPosition) getVungleBannerPosition:(int)adSizeIndex {
    enum VungleBannerPosition position;
    switch (adSizeIndex) {
        case 0:
            position = TopLeft;
            break;
        case 1:
            position = TopCenter;
            break;
        case 2:
            position = TopRight;
            break;
        case 3:
            position = Centered;
            break;
        case 4:
            position = BottomLeft;
            break;
        case 5:
            position = BottomCenter;
            break;
        case 6:
            position = BottomRight;
            break;
        default:
            [VungleUtility sendWarningMessage:@"Vungle: Unknown banner position provided."];
            position = Unknown;
            break;
    }
    return position;
}

- (CGSize)convertVunglePluginAdSizeToCGSize:(VunglePluginAdSize)size {
    CGSize returnSize;
    switch (size) {
        case VunglePluginAdSizeUnknown:
            returnSize = CGSizeZero;
            break;
        case VunglePluginAdSizeBanner:
            returnSize = kVNGPluginAdSizeBanner;
            break;
        case VunglePluginAdSizeBannerShort:
            returnSize = kVNGPluginAdSizeBannerShort;
            break;
        case VunglePluginAdSizeBannerMrec:
            returnSize = kVNGPluginAdSizeMediumRectangle;
            break;
        case VunglePluginAdSizeBannerLeaderboard:
            returnSize = kVNGPluginAdSizeLeaderboard;
            break;
        default:
            [VungleUtility sendErrorMessage:@"Vungle: Invalid ad size provided."];
            returnSize = CGSizeZero;
            break;
    }
    return returnSize;
}

#pragma mark - Positioning methods

- (void)createViewContainer {
    CGSize adSize = [self convertVunglePluginAdSizeToCGSize:(VunglePluginAdSize)self.bannerSize];
    if (self.adViewContainer) {
        // same placement was requested
        // if there is already an ad view, remove it. it might be a different size
        [self.adViewContainer removeFromSuperview];
        self.adViewContainer = nil;
    }
    self.adViewContainer = [[UIView alloc] initWithFrame:CGRectMake(0, 0, adSize.width, adSize.height)];
}

- (void)positionViewContainer {
    VungleBannerPosition adPosition = [self getVungleBannerPosition:self.bannerPosition];
    CGFloat screenWidth = [UIScreen mainScreen].bounds.size.width;
    CGFloat screenHeight = [UIScreen mainScreen].bounds.size.height;

    // iOS 11 APIs
    if (@available(iOS 11.0, *)) {
        UIView* superview = self.adViewContainer.superview;
        self.adViewContainer.translatesAutoresizingMaskIntoConstraints = NO;
        NSMutableArray<NSLayoutConstraint*>* constraints = [NSMutableArray arrayWithArray:@[
            [self.adViewContainer.widthAnchor constraintEqualToConstant:CGRectGetWidth(self.adViewContainer.frame)],
            [self.adViewContainer.heightAnchor constraintEqualToConstant:CGRectGetHeight(self.adViewContainer.frame)],
        ]];
        if (self.firstConstraint && self.secondConstraint) {
            [NSLayoutConstraint deactivateConstraints:@[self.firstConstraint, self.secondConstraint]];
        }
        switch (adPosition) {
            case TopLeft:
                [constraints addObjectsFromArray:@[
                    self.firstConstraint = [self.adViewContainer.leftAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.leftAnchor constant:self.x < 0 ? 0 : self.x],
                    self.secondConstraint = [self.adViewContainer.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor constant:self.y < 0 ? 0 : self.y]]];
                break;
            case TopCenter:
                [constraints addObjectsFromArray:@[
                    self.firstConstraint = [self.adViewContainer.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor constant:self.x],
                    self.secondConstraint = [self.adViewContainer.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor constant:self.y < 0 ? 0 : self.y]]];
                break;
            case TopRight:
                [constraints addObjectsFromArray:@[
                    self.firstConstraint = [self.adViewContainer.rightAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.rightAnchor constant:self.x < 0 ? self.x : 0],
                    self.secondConstraint = [self.adViewContainer.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor constant:self.y < 0 ? 0 : self.y]]];
                break;
            case Centered:
                [constraints addObjectsFromArray:@[
                    self.firstConstraint = [self.adViewContainer.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor constant:self.x],
                    self.secondConstraint = [self.adViewContainer.centerYAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerYAnchor constant:self.y]]];
                break;
            case BottomLeft:
                [constraints addObjectsFromArray:@[
                    self.firstConstraint = [self.adViewContainer.leftAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.leftAnchor constant:self.x < 0 ? 0 : self.x],
                    self.secondConstraint = [self.adViewContainer.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor constant:self.y < 0 ? self.y : 0]]];
                break;
            case BottomCenter:
                [constraints addObjectsFromArray:@[
                    self.firstConstraint = [self.adViewContainer.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor constant:self.x],
                    self.secondConstraint = [self.adViewContainer.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor constant:self.y < 0 ? self.y : 0]]];
                break;
            case BottomRight:
                [constraints addObjectsFromArray:@[
                    self.firstConstraint = [self.adViewContainer.rightAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.rightAnchor constant:self.x < 0 ? self.x : 0],
                    self.secondConstraint = [self.adViewContainer.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor constant:self.y < 0 ? self.y : 0]]];
                break;
            default:
                NSLog(@"An ad position is not specified.");
                break;
        }
        [NSLayoutConstraint activateConstraints:constraints];
    } else {
        CGRect viewContainerFrame = self.adViewContainer.frame;

        switch(adPosition) {
            case TopLeft:
                viewContainerFrame.origin.x = self.x < 0 ? 0 : self.x;
                viewContainerFrame.origin.y = self.y < 0 ? 0 : self.y;
                self.adViewContainer.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case TopCenter:
                viewContainerFrame.origin.x = (screenWidth / 2) - (viewContainerFrame.size.width / 2) + self.x;
                viewContainerFrame.origin.y = self.y < 0 ? 0 : self.y;
                self.adViewContainer.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case TopRight:
                viewContainerFrame.origin.x = screenWidth - viewContainerFrame.size.width + self.x < 0 ? self.x : 0;
                viewContainerFrame.origin.y = self.y < 0 ? 0 : self.y;
                self.adViewContainer.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case Centered:
                viewContainerFrame.origin.x = (screenWidth / 2) - (viewContainerFrame.size.width / 2) + self.x;
                viewContainerFrame.origin.y = (screenHeight / 2) - (viewContainerFrame.size.height / 2) + self.y;
                self.adViewContainer.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case BottomLeft:
                viewContainerFrame.origin.x = self.x < 0 ? 0 : self.x;
                viewContainerFrame.origin.y = screenHeight - viewContainerFrame.size.height + self.y < 0 ? self.y : 0;
                self.adViewContainer.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
            case BottomCenter:
                viewContainerFrame.origin.x = (screenWidth / 2) - (viewContainerFrame.size.width / 2) + self.x;
                viewContainerFrame.origin.y = screenHeight - viewContainerFrame.size.height + self.y < 0 ? self.y : 0;
                self.adViewContainer.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
            case BottomRight:
                viewContainerFrame.origin.x = screenWidth - viewContainerFrame.size.width + self.x < 0 ? self.x : 0;
                viewContainerFrame.origin.y = screenHeight - viewContainerFrame.size.height + self.y < 0 ? self.y : 0;
                self.adViewContainer.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
            default:
                NSLog(@"Vungle: An ad position is not specified.");
                break;
        }
        self.adViewContainer.frame = viewContainerFrame;
    }
}

- (void)setOffset:(int)x y:(int)y {
    self.x = x;
    self.y = y;
    if (self.adViewContainer) {
        [self positionViewContainer];
    }
}

@end
