namespace RB.Core.Net.Common;

public static class FormatHelper
{
    // https://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc

    private static readonly string[] SizeSuffixes =
        { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string SizeSuffix(long value, int decimalPlaces = 1)
    {
        if (value < 0) return $"-{SizeSuffix(-value, decimalPlaces)}";

        var i = 0;
        decimal dValue = value;
        while (Math.Round(dValue, decimalPlaces) >= 1000)
        {
            dValue /= 1024;
            i++;
        }

        return string.Format($"{{0:n{decimalPlaces}}} {{1}}", dValue, SizeSuffixes[i]);
    }
}