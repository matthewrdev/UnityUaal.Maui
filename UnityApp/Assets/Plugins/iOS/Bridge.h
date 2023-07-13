// [!] important set UnityFramework in Target Membership for this file
// [!]           and set Public header visibility

#import <Foundation/Foundation.h>

// Bridge defines protocol with methods you want to be called from managed
@protocol Bridge
@required
- (void) onUnityContent:(NSString*)eventName:(NSString*)eventContent;
// other methods
@end

__attribute__ ((visibility("default")))
@interface FrameworkLibAPI : NSObject
// call it any time after UnityFrameworkLoad to set object implementing Bridge methods
+(void) registerBridge:(id<Bridge>) bridge;

@end


