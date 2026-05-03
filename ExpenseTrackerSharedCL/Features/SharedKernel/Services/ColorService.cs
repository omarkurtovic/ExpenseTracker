using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.SharedKernel.Services
{
    public class ColorService
    {
        public static string GetContrastText(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor) || !hexColor.StartsWith("#") || hexColor.Length != 7)
                return "#FFFFFF";

            var r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
            var g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
            var b = Convert.ToInt32(hexColor.Substring(5, 2), 16);

            var luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

            return luminance > 0.5 ? "#000000" : "#FFFFFF";
        }
    }
}
