# Building a Windows Phone App Part FUUUU!!!! #

## I Give Up ##

I really feel quite despondent, like I've failed.  It just isn't possible to use the BOM FTP.

FTP can be in one of two modes, active or passive.

In passive we create a control channel, in which we connect, log in, and send control commands like change directory and retrieve file.

The FTP server responds with the message:

227 Entering Passive Mode (134,178,63,130,233,247)

This provides an IP Address (134.178.63.130) and port (233*256 + 247).  We open a second connection to this server port.  When we send a RETR message, for instance, the server streams the file back on this transmission port.

So far so good.

Only phone applications are locked to only using [well known ports](https://en.wikipedia.org/wiki/List_of_TCP_and_UDP_port_numbers) (21, 23, 25, 80, 443 etc), or [from a range between 4502-4534](http://msdn.microsoft.com/en-us/library/cc645032%28v=vs.95%29.aspx)
 
> One additional restriction on using the sockets classes is that the destination port range that a network application is allowed to connect to must be within the range of 4502-4534. These are the only destination ports allowed by a connection from a Silverlight application using sockets. If the target port is not within this port range, the attempt to connect will fail. It is possible for a target server to receive connections on a port from this restricted range and redirect it to a different port (a well-known port, for example) if this is needed to support a specific existing application protocol. 

Ahhh... that little, inconsequential restriction.

Unfortunately the rest of the internet cares not one hoot about Windows Phone 7 socket port restrictions and will continue using, in the case of the BOM, high range port numbers for FTP connections.  

The logic for the port restriction is to make it easier for admins to lock down Silverlight apps and the Phone.  But not much use to me.

## What about Active? ##

Ah-ha! I think, How about active?

In an active connection we tell the server to open a connection to US, providing our IP Address and port number. Then we open a socket and start listening.

Only the socket on the Phone has no Listen abilities.

Which again, is reasonable, from a security perspective.   More reasonable than the arbitrary port restrictions on outgoing traffic.

## So Fuuuu!!! ##

It's really depressing.  I can connect to the FTP server, list files and directories, change directory, hell even request a file.

I just can't receive any files. Nor can I be sent any files.

It just isn't allowed.  Unless you elevate to trusted. Which isn't allow on the phone.

So I give up on this idea.  I can't (won't) provide a proxy.  If this worked out OK I'd want to extend it to pull the weather based on location, or for multiple locations.  Then I have to start proxying large amounts of the BOM's copyright data. So no go.

## Sign up  at Marketplace ##

So what the hell, I went and signed up at the market place.  I had been given a free 12 month subscription, and registration unlocks the phone to allow app deployment locally.

There are some things to be aware of when registering.  You are signing up for XBOX dev as well, and it's gets a bit confusing as you end up on the xbox domain during the registration.

If you wanna get paid you also have to fill out a bunch of forms to tell the United States tax office not to tax your income.  It's obviously not an Australian marketplace as such, but a US one.

## Deploy to a Real Phone ##

Once signed up your apphub account will be associated with a windows live ID.  You can then use the Windows Phone Developer Registration tool from the 7.1 SDK to register the phone as developer unlocked.

This took about ten attempts:

 - turn off skype
 - if you once ran Chevron then make sure there are no dodgy host entries that loopback the webservices. Ahem.
 - reboot the phone
 - reboot the machine
 - reboot the phone again
 - try again

Eventually it decided to work.

Now we can use the Application Deployment tool form the SDK to deploy the XAP to the phone.

Just that I have no working application and I am giving up for the time being.

## The End ##

I need some new ideas.  This time I'll:

 - make sure they only rely on web/http stuff
 - are possible before I start (so a bit of prototyping first)
 - don't require building complex chunks of missing functionality (like FTP Clients or WMA encoders)

So yeah. Bummer.  I hate to say impossible, but just really, ridiculous difficult, or possible, but only if you want to root the phone or some other hack.