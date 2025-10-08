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

Target for `.netstandard 2.0`

Write C# sources to `cs-src` library

# TESTING

We will create a test suite for the ported code

Use:

xunit for unittest organization

FluentAssertions for simplify assertions

Moq for mocking data

# PROJECT STRUCTURE

The project solution must have two projects initially:

* Haru library 
* Haru.Test test suite