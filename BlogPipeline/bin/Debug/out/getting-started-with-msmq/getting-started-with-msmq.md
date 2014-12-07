I recently needed to know a little bit about MSQM, so I put together a very basic little prototype.

MSMQ has a number of features, but I think the obvious ones are:

- it's a Microsoft product (not sure if that’s a feature, but what the hey)
- It's included in the OS from XP onwards
- It's wrapped up in .NET in the System.Messaging namespace
- It's pretty easy to use

It may not be the fastest or the best, but I think that feature set makes it a good choice for dipping your toe in the world of message queues.

We can start by enabling MSMQ in the control panel (under Windows Features).  This will start up a service and we will be good to go.
First of all we want to create a source of messages.
Here we just send a very basic string.  In practice you may want to create a message class, perhaps with a message type, Id and maybe a serialized DTO payload.
<pre><code>
	// create a queue
	if (!MessageQueue.Exists(_queueName))
	{
		MessageQueue.Create(_queueName);
	}

	// send a bunch of messages
	for (int i = 0; i < 100; i++)
	{
		// slow things down a bit so we can see what’s happening
		Thread.Sleep(100);
		SendPublic(i);
	}

	public static void SendPublic(int i )
	{
		var myQueue = new MessageQueue(_queueName);
		
		string body = string.Format("{0} Public queue by path name.", i);

		var msg = new Message(body);
			
		msg.Formatter = new MessageFormatter();

		myQueue.Send(msg);
		Console.WriteLine(body);
		return;
	}

</code></pre>
By default MSMQ uses an XML message formatter, and reading the message body is done through streams.  This is sometimes a bit inconvenient.
I’ve added a simple message formatter (nicked from MSDN) to ease things.
<pre><code>
	public class MessageFormatter : IMessageFormatter
	{
		public bool CanRead(Message message)
		{
			return true;
		}

		public object Read(Message message)
		{
			//Obtain the BodyStream for the message.
			var stm = message.BodyStream;
			stm.Position = 0;
			//Create a StreamReader object used for reading from the stream.
			var reader = new StreamReader(stm);

			//Return the string read from the stream.
			//StreamReader.ReadToEnd returns a string.
			return reader.ReadToEnd();
		}

		public void Write(Message message, object obj)
		{
			//Declare a buffer.
			byte[] buff = Encoding.UTF8.GetBytes(obj.ToString());

			//Create a new MemoryStream object passing the buffer.
			var stm = new MemoryStream(buff);

			//Assign the stream to the message's BodyStream property.
			message.BodyStream = stm;
		}

		public object Clone()
		{
			return new MessageFormatter();
		}
	}
</code></pre>
We can then start up several process, just simple console apps, to listen to the queue and read the messages.  I was running three or four, and they would read the messages off the queue in a round robin fashion.  I found that quite pleasing :)
<pre><code>
	public static void MonitorQueue ()
	{
		var myQueue = new MessageQueue(_queueName);

		// enter an idle loop till a message is recieved
		while (true)
		{
			var msg = myQueue.Receive();
			// Process the journal message.
			if(msg != null)
			{
				msg.Formatter= new MessageFormatter();
				
                // read the payload via the BodyStream
				var rdr = new StreamReader(msg.BodyStream);
				var str = rdr.ReadToEnd();
				// or read it directly thanks to the message formatter
				Console.WriteLine(msg.Body);
			}
		}
	}
</code></pre>
I’m writing this at home and immediately discovered a few problems that are casting quite a few shadows on MSMQ.

It seems there a few limitations when running MSMQ when you aren’t on a domain (which would be pretty much everyone when they’re not at work, geeks excepted).  At work (on a domain) I had no trouble. The messages were sent from one process and several listening processes merrily picked them up and ran with it.  Life was good.  At home (on a workgroup) my most trivial test does not work.

I’ve only barely scratched the surface here and I’m not sure I like what I’m smelling.

I’d strongly think twice before using MSMQ.  If you’re rocking the “enterprise” and building things for a known environment then by all means.  If you need to cross boundaries (network, OS, etc) or you want to run over the internet, you want it to “just work” – maybe look a little deeper. RabbitMQ, ZeroMQ, nServiceBus, maybe there is something else out there.

Update:
So... I should have read the error message, but it was late.
There were a couple of problems with WORKGROUP mode.

The first one I do not really understand (I guess I need to RTFM).  If you "Manage" your computer you can see a Message Queuing node has been added.  I created a queue directly there, copied the name (something like machinename\private$\testqueue) and used that.  All good.  Apparantly you can only use private queus in WORKGROUP mode and must reference them with a specific naming format.  I found a few forum posts alluding to this, but no definitive answer anywhere. I'm sure it's mentioned somewhere in MSDN... 

![Alt text](http://benmcevoy.com.au/blog/get/msmq/manage_queue.JPG "manage queues manually")

Secondly, although you can't see it here :) I was also enumerating the public queues on my machine, as a check that the queue was created.

Calling MessageQueue.GetPublicQueues is not supported in WORKGROUP mdoe, which was a bit of red herring.  I love the [MSDN documentation](http://msdn.microsoft.com/en-us/library/hd4s6c1z.aspx) which lists whether the method is available. Guess the answer is no?

![Alt text](http://benmcevoy.com.au/blog/get/msmq/queue_test.JPG "Finally everything runs! One app is spitting messages out, while listeners grab the messages off round robin style")
