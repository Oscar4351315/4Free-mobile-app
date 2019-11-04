using System;
using System.Collections.Generic;
using System.Text;

namespace IAB330.Interfaces
{
    public interface IImageService
    {
        string FormBackgroundColour(string category);
        string CategoryToImage(string category);
    }
}
