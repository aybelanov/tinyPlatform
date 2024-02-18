using System;


namespace Hub.Services.Media.RoxyFileman;

#pragma warning disable CS1591

public partial record RoxyImageInfo(string RelativePath, DateTimeOffset LastWriteTime, long FileLength, int Width, int Height);
public partial record RoxyDirectoryInfo(string RelativePath, int CountFiles, int CountDirectories);

#pragma warning restore CS1591