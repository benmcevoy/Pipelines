If you are making web requests on the windows phone you should check network availability first.  

<pre><code>
     Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsNetworkAvailable
</code></pre>

If you are trying to be a conscientious developer you may only want to make web requests when the phone is connected to a wifi network, for instance in a background agent update.

The API offers:

<pre><code>
     DeviceNetworkInformation.IsWiFiEnabled
</code></pre>

This, however only tells you the user has wifi enabled.  It does not tell you if the phone is connected and using a wifi network. 

You must also check the *[NetworkInterfaceType][1]*.  Note that the majority of interface types are not supported on the phone. There is really only **Wireless80211**, **MobileBroadbandGsm** and **MobileBroadbandCdma** to worry about.  I *believe* **Ethernet** is available when connected to a PC with Zune running, as I *think* Zune allows the phone to piggy back off the PC's network connection. AFAIK.

<pre><code>
    NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
</code></pre>

Put it all together.

<pre><code>

        private bool IsWifiConnected()
        {
            if(IsNetworkAvailable() && IsWifiEnabled())
            {
                return NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
            }

            return false;
        }

        private bool IsWifiEnabled()
        {
            return DeviceNetworkInformation.IsWiFiEnabled;
        }

        private bool IsNetworkAvailable()
        {
            return DeviceNetworkInformation.IsNetworkAvailable;
        }


</code></pre>

##This thing is slooooow##

After implementing something like this in a background agent on the phone, I was surprised to see the agent frequently not running correctly, usually timing out.

As far as as I can tell, the call to **NetworkInterface.NetworkInterfaceType** can be very slow, sometimes 20-30 seconds, and sometimes simply returning *None*, even though the phone is clearly connected to the network.

People on stackoverflow suggest wrapping these calls up in some async pattern, either threadpool or whatever, as it will be a blocking call otherwise.

In the case of a background agent I can't see how to determine if we are on a wifi network (or connected at all) in a timely manner.  The background task must complete in, I think, 25 seconds or so.  If the network takes 20-30 seconds, well you can see the problem.

##WIFI is sleeping##

The phone has certain behaviors you must be aware of.

 - wifi will turn off if the phone is asleep
 - the phone will preferentially use the mobile connection

On the windows phone 8 a recent update allows the user to specify "Keep WiFi on when screen times out".

On windows phone 7.5 it seems to keep WiFi on by default. I am not sure.

Basically, during a background task:

 - if the user has wifi enabled and mobile data turned on the phone will opt for mobile data
 - if the user has wifi enabled and wifi enabled during sleep (and mobile data) the phone *should* opt for wifi
 - if the user has wifi enabled, wifi enabled during sleep and mobile data off, the check for network will succeed (uses wifi)
 - if the user has wifi enabled, mobile data off, the check for network will fail (timeout)
 - if the user has wifi disabled, mobile data on, the check for network will  succeed (uses mobile data)

At the end of the day the best you can do is inform your users.

"If you wish to use wifi for background data requests please check "Keep WiFi on when screen times out" is enabled under the advanced settings of your wifi connection." 


  [1]: http://msdn.microsoft.com/en-US/library/windowsphone/develop/microsoft.phone.net.networkinformation.networkinterfacetype(v=vs.105).aspx