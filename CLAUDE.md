# GOAL

The goal of the project is to port Haru (PDF Writing Library) from C to .NET

# SOURCE

The source of the Haru library in C is located in `c-src` folder

# DEPENDENCIES

The Haru library depends on zlib and pnglib.

Use `System.IO.Compression` in place of zlib

For pnglib:

* Create a facade which has the interface defined by the service needed by haru
* Create initial implementation of the facade using SixLabors.ImageSharp

# MEMORY MANAGEMENT

Rely on .NET GC instead of explicit C-style memory management.

# TARGET

Target is `net 8.0`

Write C# sources to `cs-src` library

The structure of the target project is

 * ./cs-src/Haru.sln is solution file
 * ./cs-src/Haru is library sources
 * ./cs-src/Haru.Test is unit test project
 * ./cs-src/Haru.Demo is demo project


# TESTING

We will create a test suite for the ported code

Use:

xunit for unittest organization

FluentAssertions (7.2.0, the exact version!!!) for simplify assertions

Moq for mocking data

# DEBUGGING

If you need to debug:
  * You can create console .NET application or use dotnet-script
  * Put all debugging and service code to ./test folder


# Additional information

  * LAST.md contains the last status of the project. Always read it in the beginning of the session and update it before quiting.
  * REMAINING.md contains the list of what remains to be done. Always read it in the beginning of the session and update it before quiting.
  * cs-src/Haru.Demos/CLAUDE.md contains the information about Demo project. Read it before do any modification of demo project
