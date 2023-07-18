#import <Foundation/Foundation.h>
#import "Bridge.h"


@implementation Bridge

id<IUnityContentReceiver> mContentReceiver = NULL;
+(void) registerUnityContentReceiver:(id<IUnityContentReceiver>) contentReceiver
{
    mContentReceiver = contentReceiver;
}

@end

extern "C" {
    void onUnityContent(const char* eventName, const char* eventContent) { 
        return [mContentReceiver receiveUnityContent:[NSString stringWithUTF8String:eventName]:[NSString stringWithUTF8String:eventContent]]; 
    }
}

