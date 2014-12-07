# Optimising C# #

Premature optimisation man! YAGNI man!

Well sometimes you do need it.  I've been writing a little game on the windows phone and I needed MORE POWER! Trying to get more explosions on screen at once.

## Accessing a member ##

Now someone on the internet had said that accessing a class member was slow.

<pre><code>
	private float _member;
	
	// do something with that class member
	_member++;
</code></pre>

With a release compiled console app, accessing that member 100000000 times:

- about 0.34 of a second in VS2012
- about 0.34 of a second in VS2010

OK, so that's our base line.

## Inlining member access ##

So the wisdom is to inline that reference:

<pre><code>
	private float _member;
	
	// inline it
	var m = _member;

	// do something with that local reference
	m++;
</code></pre>

and we get....

- about 0.34 of a second in VS2012
- about 0.34 of a second in VS2010

Hmmm... not a very good optimisation at all.

But wait! I care about the Windows Phone. So what's the damage?
Well, the phone is a wee bit slower than my desktop, so running in the 512MB emulator (OS7.1)

- Member reference about 2.16 seconds
- Inline reference **about 1.547 seconds!**

Hold the fsking phone!

And, I'd like to point out, running on a REAL phone the results are similarly good.

- Member reference about 3.51 seconds
- Inline reference **about 2.12 seconds!**

## Accessing a property ##

OK, so the wise internet monkeys also pronounce that property access is killer. And you could reason easily that property access probably involves more CPU, as it must go to the backing member.  But surely compiler optimisations will make that cool?

<pre><code>
	public float Member { get; set; }

	// do something with that property
	Member++;

</code></pre>

And for Property Get

- about 0.581 of a second in VS2012
- about 0.581 of a second in VS2010
- about 2.17 seconds on the phone emulator

And for Property Set

- about 0.493 of a second in VS2012
- about 0.493 of a second in VS2010
- about 1.90 seconds on the phone emulator

Comparing to a Method get

- about 0.581 of a second in VS2012
- about 0.581 of a second in VS2010
- about 2.16 seconds on the phone emulator

And the method set

- about 0.493 of a second in VS2012
- about 0.493 of a second in VS2010
- about 1.90 seconds on the phone emulator

So.... no noticeable difference...

However, if your properties are doing more than just getting and setting a backing member, well you should probably turn them into methods anyway.  For the common and trivial case of something like:

<pre><code>
        private float _myProperty;
        public float MyProperty { get { return _myProperty; } set { _myProperty = value; } }
</code></pre>

You probably ain't gonna need it, man!

My code was an implementation of a 2d vector.  It had properties like Length and Angle.  If you look at the System.Windows.Vector it has methods like Length() and so on. Make of that what you will.  

The moral of the story is test your assumptions, and test your compiler/platform.

It's a nice feeling to write code that is noticeably faster in release than debug. This ain't no LOB app!

These optimisations made a noticeable difference on the running game but... unfortunately there was still not enough explosions, so it's off to XNA anyways...

## Test Harness ##
<pre><code>
    static void Main(string[] args)
    {
        var tc = new TestClass();

        // warm up, I don't think this really matters. Maybe GC is affected
        tc.TestMember();
        tc.TestInlineMember();

        for (int i = 0; i &lt; 10; i++)
        {
            var sw = Stopwatch.StartNew();

                tc.TestMember();

                sw.Stop();

                Console.WriteLine("TestMember " + sw.Elapsed);

                sw = Stopwatch.StartNew();

                tc.TestInlineMember();

                sw.Stop();
                Console.WriteLine("TestInlineMember " + sw.Elapsed);

                sw = Stopwatch.StartNew();

                tc.TestPropertyGet();

                sw.Stop();
                Console.WriteLine("TestPropertyGet " + sw.Elapsed);

                sw = Stopwatch.StartNew();

                tc.TestPropertySet();

                sw.Stop();
                Console.WriteLine("TestPropertySet " + sw.Elapsed);

                sw = Stopwatch.StartNew();

                tc.TestMethodGet();

                sw.Stop();
                Console.WriteLine("TestMethodGet " + sw.Elapsed);

                sw = Stopwatch.StartNew();

                tc.TestMethodSet();

                sw.Stop();
                Console.WriteLine("TestMethodSet " + sw.Elapsed);
        }

        Console.ReadKey();
    }
</code></pre>

## Test Class ##
<pre><code>
    class TestClass
    {
        private float _member;

        public float Member { get; set; }

        public float TestMember()
        {
            _member = 0;

            for (int i = 0; i < 100000000; i++)
            {
                _member++;
            }

            return _member;
        }

        public float TestInlineMember()
        {
            var m = _member;

            for (int i = 0; i < 100000000; i++)
            {
                m++;
            }

            return m;
        }

        public float TestPropertyGet()
        {
            Member = 1;
            var m = 0f;

            for (int i = 0; i < 100000000; i++)
            {
                m += Member;
            }

            return Member;
        }

        public void TestPropertySet()
        {
            for (int i = 0; i < 100000000; i++)
            {
                Member = i;
            }
        }

        public float TestMethodGet()
        {
            SetMember(1);

            var m = 0f;

            for (int i = 0; i < 100000000; i++)
            {
                m += GetMember();
            }

            return GetMember();
        }

        public void TestMethodSet()
        {
            for (int i = 0; i < 100000000; i++)
            {
                SetMember(i);
            }
        }

        public float GetMember()
        {
            return _member;
        }

        public void SetMember(float value)
        {
            _member = value;
        }
    }
</code></pre>