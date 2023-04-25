//
//  RTVoiceIOSBridge.h
//  Version 2020.1.0
//
//  © 2016-2020 crosstales LLC (https://www.crosstales.com)
//
#ifndef RTVoiceIOSBridge_h
#define RTVoiceIOSBridge_h
#import <AVFoundation/AVFoundation.h>

@interface RTVoiceIOSBridge:NSObject<AVSpeechSynthesizerDelegate>
+ (void)setVoices;
+ (void)speak:(NSString *)id text:(NSString *)text rate:(float)rate pitch:(float)pitch volume:(float)volume;
+ (void)stop;
@end


#ifdef __cplusplus
extern "C" {
    void UnitySendMessage(const char *, const char *, const char *);
}
#endif

#endif
