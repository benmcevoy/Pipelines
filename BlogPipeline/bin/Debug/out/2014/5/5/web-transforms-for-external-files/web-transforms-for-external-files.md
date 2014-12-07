#Web Transforms for external files#

So often it's easy to come up with the complex solution and hard to do the simplest thing. And sometimes it's a case of just not seeing the wood for the trees.

When working with systems like Sitecore or Umbraco you find *a lot* of external config files. Sitecore has it's *patch*ing (a kind of custom transform engine) and there's always web transforms, but what to do about environmental differences in all those external files?

##The complex way##

All we wanted was to be able to "add transform" to any old config file.  So we reach for solutions like [SlowCheetah](http://www.hanselman.com/blog/SlowCheetahWebconfigTransformationSyntaxNowGeneralizedForAnyXMLConfigurationFile.aspx) or msbuild scripts using Web Transform Task and others.

It works and seems easy enough (expecially SlowCheetah).

But now any one coming to the project has friction and has to install a Visual Studio extension, or god forbid, edit an msbuild script.

##The simple way##

Rather than trying to transform the external config file, we just need to [*transform the path*](http://stackoverflow.com/a/14842051) to the file.

Like a light bulb. So simple. So obvious.  Took so long to recognise it.

Just a bunch of config files with each environment in the name.

Add a web transform and use the Condition locator.  Say **Web.Staging.config**:

<pre><code>
    &lt;myModule configsource="config/myModule.<strong>STAGING</strong>.config" 
          xdt:Locator="Condition(@configSource='config/myModule.config')" 
          xdt:Transfrom="Replace" /&gt;
</code></pre>

Sure, you don't get nicely nested files in the solution explorer, and it's not completely DRY. But it's simple. And easy. And it works.  

##But##

You will end up deploying multiple config files, e.g. myModule.config, mymodule.staging.config etc. and that might be an issue if there are sensitive settings in there, like connection strings or passwords. 

Cheers.


