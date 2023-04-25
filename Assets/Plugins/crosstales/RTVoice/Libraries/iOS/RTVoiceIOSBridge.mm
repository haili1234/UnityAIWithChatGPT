//
//  RTVoiceIOSBridge.mm
//  Version 2020.1.1
//
//  © 2016-2020 crosstales LLC (https://www.crosstales.com)
//
#import "RTVoiceIOSBridge.h"
#import <AVFoundation/AVFoundation.h>
#import <Foundation/Foundation.h>
//#define DEBUG


@implementation RTVoiceIOSBridge

static AVSpeechSynthesizer *_synthesizer;
static NSArray *_voices;

+ (AVSpeechSynthesizer *)synthesizer {
  if (_synthesizer == nil) {
      _synthesizer = [[AVSpeechSynthesizer alloc] init];
      _synthesizer.delegate = self;
  }
  return _synthesizer;
}

+ (NSArray *)voices {
  if (_voices == nil) {
      _voices = [AVSpeechSynthesisVoice speechVoices];
  }
  return _voices;
}

/**
 * Speaks the string with a given rate, pitch, volume and culture.
 * @param id ID of the voice to speak
 * @param text Text to speak
 * @param rate Speech rate of the speaker in percent
 * @param pitch Pitch of the speech in percent
 * @param volume Volume of the speaker in percent
 */
+ (void)speak: (NSString *)id text:(NSString *)text rate:(float)rate pitch:(float)pitch volume:(float)volume
{
#ifdef DEBUG
    NSLog(@"speak: %@ - Text: %@, Rate: %.3f, Pitch: %.3f, Volume: %.3f", id, text, rate, pitch, volume);
#endif

    if (text)
    {
        //[RTVoiceIOSBridge stop];

        if (RTVoiceIOSBridge.voices) {
            AVSpeechSynthesisVoice *voice = RTVoiceIOSBridge.voices[0]; // one voice must be available
            
            for (AVSpeechSynthesisVoice *v in RTVoiceIOSBridge.voices) {
                if ([v.identifier isEqualToString:id])
                {
                    voice = v;
                    break;
                }
            }

#ifdef DEBUG
            NSLog(@"speak - selected voice: %@", voice.name);
#endif
            AVSpeechUtterance *utterance = [[AVSpeechUtterance alloc] initWithString:text];
            utterance.voice = voice;

            float adjustedRate = AVSpeechUtteranceDefaultSpeechRate * rate;
            
            if (adjustedRate > AVSpeechUtteranceMaximumSpeechRate)
            {
                adjustedRate = AVSpeechUtteranceMaximumSpeechRate;
            }

            if (adjustedRate < AVSpeechUtteranceMinimumSpeechRate)
            {
                adjustedRate = AVSpeechUtteranceMinimumSpeechRate;
            }

            utterance.rate = adjustedRate;
            utterance.volume = volume;

            utterance.pitchMultiplier = pitch;

            [RTVoiceIOSBridge.synthesizer speakUtterance:utterance];
        } else {
            NSLog(@"ERROR: no voices found - could not speak!");
        }
    } else {
        NSLog(@"WARNING: text was null!");
    }
}

/**
 * Stops speaking
 */
+ (void)stop
{
#ifdef DEBUG
    NSLog(@"stop");
#endif

    [RTVoiceIOSBridge.synthesizer stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
}

/** 
 * Collects and sends all voices to RT-Voice.
 */
+ (void)setVoices
{
    NSString *appendstring = @"";

    if (RTVoiceIOSBridge.voices) {
        for (AVSpeechSynthesisVoice *voice in RTVoiceIOSBridge.voices) {
            appendstring = [appendstring stringByAppendingString:voice.identifier];
            appendstring = [appendstring stringByAppendingString:@","];
            appendstring = [appendstring stringByAppendingString:voice.name];
            appendstring = [appendstring stringByAppendingString:@","];
            appendstring = [appendstring stringByAppendingString:voice.language];
            appendstring = [appendstring stringByAppendingString:@","];
            
#ifdef DEBUG
            NSLog(@"Voice-ID: %@ - Name: %@, Language: %@, Quality: %ld", voice.identifier, voice.name, voice.language, (long)voice.quality);
#endif
        }
    } else {
        NSLog(@"ERROR: no voices found!");
    }
    
#ifdef DEBUG
    NSLog(@"setVoices: %@", appendstring);
#endif

    UnitySendMessage("RTVoice", "SetVoices", [appendstring UTF8String]);
}

/**
 * Called when the speak is finished and informs RT-Voice.
 */
+ (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer didFinishSpeechUtterance:(AVSpeechUtterance *)utterance
{
#ifdef DEBUG
    NSLog(@"didFinishSpeechUtterance");
#endif

    UnitySendMessage("RTVoice", "SetState", "Finish");
}

/** 
 * Called when the synthesizer have began to speak a word and informs RT-Voice.
 */
+ (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer willSpeakRangeOfSpeechString:(NSRange)characterRange utterance:(AVSpeechUtterance *)utterance
{
#ifdef DEBUG
    NSLog(@"willSpeakRangeOfSpeechString");
#endif

    UnitySendMessage("RTVoice", "WordSpoken", "w");//[substringcutout UTF8String]);
}

/**
 * Called when the speak is canceled and informs RTVoice.
 */
+ (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer didCancelSpeechUtterance:(AVSpeechUtterance *)utterance
{
#ifdef DEBUG
    NSLog(@"didCancelSpeechUtterance");
#endif

    UnitySendMessage("RTVoice", "SetState", "Cancel");
}

/**
 * Called when the speak is started and informs RT-Voice.
 */
+ (void)speechSynthesizer:(AVSpeechSynthesizer *)synthesizer didStartSpeechUtterance:(AVSpeechUtterance *)utterance
{
#ifdef DEBUG
    NSLog(@"didStartSpeechUtterance");
#endif

    UnitySendMessage("RTVoice", "SetState", "Start");
}

@end

extern void sendMessage(const char *, const char *, const char *);

extern "C" {
    
    /**
     * Bridge to speak the string that it receives with a given rate, pitch, volume and identifier.
     * @param id ID of the voice to speak
     * @param text Text to speak
     * @param rate Speech rate of the speaker in percent
     * @param pitch Pitch of the speech in percent
     * @param volume Volume of the speaker in percent
     */
    void Speak(char *id, char *text, float rate, float pitch, float volume)
    {
        NSString *voiceId = [NSString stringWithUTF8String:id];
        NSString *messageFromRTVoice = [NSString stringWithUTF8String:text];

#ifdef DEBUG
        NSLog(@"Speak: %@ - Text: %@, Rate: %.3f, Pitch: %.3f, Volume: %.3f", voiceId, messageFromRTVoice, rate, pitch, volume);
#endif

        [RTVoiceIOSBridge speak:voiceId text:messageFromRTVoice rate:rate pitch:pitch volume:volume];
    }
    
    /**
     * Bridge to stop speaking.
     */
    void Stop()
    {
#ifdef DEBUG
        NSLog(@"Stop");
#endif

        [RTVoiceIOSBridge stop];
    }
    
    /** 
     * Bridge to get all voices.
     */
    void GetVoices()
    {
#ifdef DEBUG
        NSLog(@"GetVoices");
#endif

        [RTVoiceIOSBridge setVoices];
    }
}
