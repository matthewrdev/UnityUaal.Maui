// [!] important set UnityFramework in Target Membership for this file
// [!]           and set Public header visibility

#import <Foundation/Foundation.h>

@protocol UnityContentReceiver
@optional
-(void) onReceivedUnityContent:(NSString*)content;
@end

__attribute__ ((visibility("default")))
@interface Bridge : NSObject

+(void) registerUnityContentReceiver:(id<UnityContentReceiver>) contentReceiver;

@end


