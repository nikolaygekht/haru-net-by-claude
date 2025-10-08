#r "cs-src/Haru/bin/Debug/net8.0/Haru.dll"
using Haru.Xref;
using Haru.Objects;
using Haru.Streams;

var xref = new HpdfXref(0);
xref.Add(new HpdfNumber(1));
xref.Add(new HpdfNumber(2));

Console.WriteLine($"Before write: Entry[1].ByteOffset = {xref.Entries[1].ByteOffset}");
Console.WriteLine($"Before write: Entry[2].ByteOffset = {xref.Entries[2].ByteOffset}");

var stream = new HpdfMemoryStream();
xref.WriteToStream(stream);

Console.WriteLine($"After write: Entry[1].ByteOffset = {xref.Entries[1].ByteOffset}");
Console.WriteLine($"After write: Entry[2].ByteOffset = {xref.Entries[2].ByteOffset}");
Console.WriteLine($"Stream size: {stream.Size}");
