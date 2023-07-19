// [!] important set UnityFramework in Target Membership for this file
// [!]           and set Public header visibility

#import <Foundation/Foundation.h>
#include <UnityFramework/UnityFramework.h>

@protocol UnityContentReceiver
@required
- (void) receiveUnityContent:(NSString*)eventName
                            :(NSString*)eventContent;
// other methods
@end

__attribute__ ((visibility("default")))
@interface Bridge : NSObject
// call it any time after UnityFrameworkLoad to set object implementing the content receiver methods
+(void) registerUnityContentReceiver:(id<UnityContentReceiver>) contentReceiver;
@end


