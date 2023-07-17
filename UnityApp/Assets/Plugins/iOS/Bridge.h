// [!] important set UnityFramework in Target Membership for this file
// [!]           and set Public header visibility

#import <Foundation/Foundation.h>

// Bridge defines protocol with methods you want to be called from managed
@protocol IUnityContentReceiver
@required
- (void) receiveUnityContent:(NSString*)eventName:(NSString*)eventContent;
// other methods
@end

__attribute__ ((visibility("default")))
@interface Bridge : NSObject
// call it any time after UnityFrameworkLoad to set object implementing the content receiver methods
+(void) registerUnityContentReceiver:(id<IUnityContentReceiver>) contentReceiver;

@end


