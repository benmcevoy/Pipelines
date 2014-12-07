# Using aliases for class name collisions #

> Filed under “things I did not know until resharper automatically fixed something for me”.

If you need to use a namespace that collides with another, perhaps by having the same names for classes under both (ambiguous names) you will use a name space alias to clear up and confusion.

<pre><code>
using System;
// alias the namespace
using myAlias = NameSpace.That.Collides;
</code></pre>

However, you can also use an alias to change a class name directly.

<pre><code>
using System;
using Namespace.With.Customer;
// only alias the class
using myCustomer = AnotherNameSpace.WithCustomer.Customer;
</code></pre>

Handy.
