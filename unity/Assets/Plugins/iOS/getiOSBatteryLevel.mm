//
//  getiOSBatteryLevel.mm
//  Unity-iPhone
//
//

#import <Foundation/Foundation.h>

extern "C" float _getiOSBatteryLevel() {
  [[UIDevice currentDevice] setBatteryMonitoringEnabled:YES];
  return [[UIDevice currentDevice] batteryLevel];
}
  
