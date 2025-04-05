// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var dir = "C:\\Users\\jzq\\repos\\Dynamo\\src\\Dynamo.Wpf\\Assets";
HashSet<string> files = new HashSet<string>();
foreach(var d in Directory.GetFiles(dir,"*.*",SearchOption.AllDirectories))
{
    var fileName=Path.GetFileNameWithoutExtension(d);
    if(files.Contains(fileName) )
    {
        Console.WriteLine(d);
    }
    files.Add(fileName);

   
}
Console.ReadLine();
