Implements an option type (Strilanc.Value.May<T>) that encourages usage based on pattern matching rather than ForceGetValue. Also includes utility methods for producing, consuming and transforming May<T>.

Note on null: May<T> treats null like any other value. May.NoValue is distinct from null, and both are distinct from ((object)null).Maybe().

Blog post with usage examples: http://twistedoakstudios.com/blog/Post1130_when-null-is-not-enough-an-option-type-for-c

NuGet package: https://nuget.org/packages/Strilanc.Value.May