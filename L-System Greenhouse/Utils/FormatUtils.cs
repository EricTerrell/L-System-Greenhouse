using System;

namespace L_System_Greenhouse.Utils;

public static class FormatUtils
{
    public static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $@"{timeSpan:dd\.hh\:mm\:ss}";
    }
}