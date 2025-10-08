using System;
using Haru.Objects;
using Haru.Xref;
using Haru.Streams;

var xref = new HpdfXref(0);
var dict = new HpdfDict();
var array = new HpdfArray();
array.Add(new HpdfNumber(0));
array.Add(new HpdfNumber(0));
array.Add(new HpdfNumber(612));
array.Add(new HpdfNumber(792));

Console.WriteLine($"Before adding to xref:");
Console.WriteLine($"  dict.IsIndirect = {dict.IsIndirect}, ObjectId = {dict.ObjectId}");
Console.WriteLine($"  array.IsIndirect = {array.IsIndirect}, ObjectId = {array.ObjectId}");

xref.Add(dict);

Console.WriteLine($"\nAfter adding dict to xref:");
Console.WriteLine($"  dict.IsIndirect = {dict.IsIndirect}, ObjectId = {dict.ObjectId}");
Console.WriteLine($"  array.IsIndirect = {array.IsIndirect}, ObjectId = {array.ObjectId}");

dict.Add("MediaBox", array);

Console.WriteLine($"\nAfter adding array to dict:");
Console.WriteLine($"  dict.IsIndirect = {dict.IsIndirect}, ObjectId = {dict.ObjectId}");
Console.WriteLine($"  array.IsIndirect = {array.IsIndirect}, ObjectId = {array.ObjectId}");

var stream = new HpdfMemoryStream();
dict.WriteValue(stream);
var output = System.Text.Encoding.ASCII.GetString(stream.ToArray());
Console.WriteLine($"\nDict output:\n{output}");
