#import <Foundation/Foundation.h>
#import "NativeCallProxy.h"


@implementation FrameworkLibAPI

id<Bridge> mBridge = NULL;
+(void) registerBridge:(id<Bridge>) bridge
{
    mBridge = bridge;
}

@end

extern "C" {
    void onUnityContent(const char* eventName, const char* eventContent) { 
        return [mBridge onUnityContent:[NSString stringWithUTF8String:eventName]:[NSString stringWithUTF8String:eventContent]]; 
    }
}

