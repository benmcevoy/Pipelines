#App Upgrade Manager #
I seem to have found a new hammer called reflection :)

I have been writing a few client apps of late, for example [stuff for the WP7](http://radio7.com.au/) and more recently started mucking around with Windows 8 apps.  In the past when building WPF or other clients we needed to take consideration of upgrades to the client data model.  The client will already be installed, with maybe an SQL CE database, or XML or something.  When you start rewriting schema things are gonna break.

So the usual method is to chuck some kind of version number in, and then write code to update the schema.  If a client misses a few updates this is OK, as we can just "play" all the updates in order and get them back up to the tip.

I currently making some update to a WP7 app that involves a schema change, so naturally I dropped everything to write a little upgrade/version manager.

And reflection seemed to fit the bill nicely:)

[Code is here][1]

##The Spec##

I had a few things in mind:

 - to be able to have the update code separate from the main client
 - for the update manager to be generic and reusable
 - for the developer (me!) to have an easy life writing future updates

A few things seemed to jump out at me, notably using attributes to decorate Upgrade classes and the current app version, and scanning an assembly for those attributes.

## The Solution ##

The idea is to create an assembly with one or more Upgrade classes in it.  Each Upgrade class inherits from UpgradeBase.  This should probably also have been implemented as an interface, but oh well.

The important thing to note here is:

 - An abstract method called Upgrade
 - The work flow in TryUpgrade - IsUpgradeRequired? Upgrade, Complete

<pre><code>
    public abstract class UpgradeBase
    {
        private static Mutex _mutex = new Mutex(false, "{C4A348B4-4FF3-4506-B1DD-73B237B0B58C}");
        private readonly int _oldVersionNumber;

        public UpgradeBase()
        {
            _oldVersionNumber = UpgradeHelper.GetVersionNumberFromIso();
        }

        protected abstract void Upgrade();

        internal int VersionNumber { get; set; }

        public virtual bool TryUpgrade()
        {
            // upgrades may be invoked from the foreground app or on a background task
            // attempting to protect isolated storage or other shared resources with mutex
            _mutex.WaitOne();

            try
            {
                if (IsUpgradeRequired())
                {
                    Upgrade();
                    Complete();
                    return true;
                }

                return false;
            }
            finally { _mutex.ReleaseMutex(); }
        }

        protected virtual void Complete()
        {
            UpgradeHelper.SetVersionNumberToIso(this.VersionNumber);
        }

        protected virtual bool IsUpgradeRequired()
        {
            return (_oldVersionNumber < this.VersionNumber);
        }
    }
</code></pre>

## Version Numbers ##
There are two important version numbers:

 - the app's version
 - the Upgrade class version

The app version represents the current version of that app's code base.  The first release is version 1, the second version 2 and so on.

The version attribute is defined simply as:

<pre><code>
    [AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public VersionAttribute(int versionNumber)
        {
            this.VersionNumber = versionNumber;
        }

        public int VersionNumber { get; set; }
    }
</code></pre>

The app's version is represented as an Assembly attribute, in the AssemblyInfo.cs class of the Upgrade assembly.

<pre><code>
    [assembly: Radio7.Phone.Version.Version(1)]
</code></pre>

Each Upgrade class with have a version as well.  This represents the version this class will upgrade the app to.

<pre><code>
    [Version(1)]
    public class MyUpgrade : UpgradeBase
    {
        protected override void Upgrade()
        {
            // do what you do best
        }
    }
</code></pre>

When an upgrade is complete the new version number is committed to isolated storage.  This allows us to know if updates have been applied and avoid continuously attempting to rerun them.

As a consumer, the developer, this is all you have to do. Set the new version number of your app. Write a class that inherits from UpgradeBase.  That's pretty much it.

## Sharing State With Phone Background Tasks ##
At first I tried to use the Application State to store the version number, but quickly discovered that the foreground client app and any background tasks do not share the application state.

Instead, we write to isolated storage.  There is a risk of file locking or corruption, as the background task or the foreground client may try to set state.  We try and alleviate this by slapping a Mutex around the TryUpgrade in the UpgradeBase and when trying to save the version number to isolated storage.

## Put it all Together##

So now we can create a satellite assembly with our upgrade logic in it.  This is simply a class libray with on eoro more upgrade classes.  The library has a version number applied at the assembly level, and version numbers applied on each upgrade.

In our foreground client, and/or in the background task agent we can apply any required updates:

From my App.xaml.cs App ctor, for instance we can call

<pre><code>
    private void UpgradeApplication()
    {
        try
        {
            Radio7.Phone.Version.UpgradeHelper.Upgrade(System.Reflection.Assembly.Load("Radio7.Phone.Client.Upgrade"));
        }
        catch
        {
            // handle your problems
        }
    }
</code></pre>

Here we pass a reference to the assembly that contains the upgrade code.

The UpgradeHelper will  scan the assembly for any classes that derive from UpgradeBase, ordering them by version number.  

If the Version number on that class is higher than the version number we find in isolated storage for the app then the Upgrade will be applied.  

If version number is the same as declared in the upgrade assembly then we have no work to do and exit.

I would have liked to have not passed the assembly in via reflection, and just scan for any upgrade classes, but unfortunately I do not currently know how to do that.  That would be nice, then perhaps shit would "just work" - add some Upgrade classes and they just get applied automagically. AppDomain.CurrentDomain.GetAssemblies()?  Version2!

<pre><code>
    public static class UpgradeHelper
    {
        private static Mutex _mutex = new Mutex(false, "{25D25F8A-9A59-4306-83B0-D4928783645B}");
        private const string _versionPath = "Radio7.Phone.Version.VersionNumber.txt";

        public static void Upgrade(Assembly upgradeAssembly)
        {
            int currentVersionNumber = GetVersionNumberFromIso();
            int newVersionNumber = GetVersionNumberFromAssembly(upgradeAssembly);

            if (currentVersionNumber == newVersionNumber)
                return;

            var types = upgradeAssembly
                .GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(UpgradeBase)))
                .ToList();

            var upgrades = CreateUpgradeInstances(currentVersionNumber, types);

            ApplyUpgrades(upgrades, newVersionNumber);

            SetVersionNumberToIso(newVersionNumber);
        }

        private static void ApplyUpgrades(IEnumerable<UpgradeBase> upgrades, int newVersionNumber)
        {
            foreach (var upgrade in upgrades.OrderBy(u => u.VersionNumber))
                upgrade.TryUpgrade();
        }

        private static IEnumerable<UpgradeBase> CreateUpgradeInstances(int currentVersionNumber, List<Type> types)
        {
            var upgrades = new List<UpgradeBase>();

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttributes(typeof(VersionAttribute), false).FirstOrDefault();

                if (attribute != null)
                {
                    var version = (attribute as VersionAttribute).VersionNumber;

                    if (version > currentVersionNumber)
                    {
                        var instance = Activator.CreateInstance(type);
                        (instance as UpgradeBase).VersionNumber = version;
                        upgrades.Add((instance as UpgradeBase));
                    }
                }
            }

            return upgrades;
        }

        private static int GetVersionNumberFromAssembly(Assembly upgradeAssembly)
        {
            var versionAttribute = Attribute.GetCustomAttributes(upgradeAssembly, typeof(VersionAttribute)).FirstOrDefault();

            if (versionAttribute != null)
                return (versionAttribute as VersionAttribute).VersionNumber;

            return 0;
        }

        internal static int GetVersionNumberFromIso()
        {
            _mutex.WaitOne();
            try
            {
                int value = 0;
                var tryvalue = IsolatedStorageHelper.ReadFileAsString(_versionPath);
                int.TryParse(tryvalue, out value);
                return value;

            }
            finally { _mutex.ReleaseMutex(); }
        }

        internal static void SetVersionNumberToIso(int versionNumber)
        {
            _mutex.WaitOne();
            try
            {
                IsolatedStorageHelper.WriteFile(_versionPath, versionNumber.ToString());
            }
            finally { _mutex.ReleaseMutex(); }
        }
    }
</code</pre>


  [1]: https://bitbucket.org/benmcevoy/radio7.phone.common