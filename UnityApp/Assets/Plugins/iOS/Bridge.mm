#import <Foundation/Foundation.h>
#import "Bridge.h"

@implementation Bridge

id<UnityContentReceiver> mContentReceiver = NULL;
+(void) registerUnityContentReceiver:(id<UnityContentReceiver>) contentReceiver
{
    mContentReceiver = contentReceiver;
}

@end

extern "C" {
    void onUnityContent(const char* content) {
        return [mContentReceiver onReceivedUnityContent:[NSString stringWithUTF8String:content]];
    }
}

