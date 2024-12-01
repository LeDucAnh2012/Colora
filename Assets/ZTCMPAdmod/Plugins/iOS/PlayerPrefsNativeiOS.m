// PlayerPrefsNativeiOS.m
#import <Foundation/Foundation.h>

@interface PlayerPrefsNativeiOS : NSObject
@end

@implementation PlayerPrefsNativeiOS

+ (NSString *)getStringForKey:(NSString *)key withDefaultValue:(NSString *)defaultValue {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    NSString *value = [defaults stringForKey:key];
    return value ? value : defaultValue;
}

+ (int)getIntForKey:(NSString *)key withDefaultValue:(int)defaultValue {
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    if ([defaults objectForKey:key] != nil) {
        return (int)[defaults integerForKey:key];
    }
    return defaultValue;
}

@end