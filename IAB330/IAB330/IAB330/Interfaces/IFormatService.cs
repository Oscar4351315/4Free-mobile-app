using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace IAB330.Interfaces
{
    public interface IFormatService
    {
        string FormatTimeRemainingToString(TimeSpan end, TimeSpan start);
        string FormatDistanceToString(Position src, Position dst);
    }
}
