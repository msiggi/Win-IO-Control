# Win-IO-RemoteControl
Set and read GPIOs from Raspberry Pi@Windows 10 IoT Core per http-api. Also Read One-Wire-Values.

Win-IO-RemoteControl is a minimized Background-Service for your Raspberry Pi, which allows you to separate your non-hardware-related Software-parts in other projects (for example in a Asp.Net-Core-App, which is maybe better for diagnostics and debugging).

***
early beta-state!
***

## Api

### Switch/Set GPIO #4:

http://[RaspiIp]:8800/api/setgpio/4/true

### Toggle GPIO #4:

http://[RaspiIp]:8800/api/togglegpio/4

## Logging

You can find logfiles here:

\\192.168.XX.XX\c$\Data\Users\DefaultAccount\AppData\Local\Packages\WinIoRc.BackgroundService-uwp_***\LocalState
