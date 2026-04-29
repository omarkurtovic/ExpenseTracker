using MudBlazor;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.SharedKernel.Services
{
    public class ThemeService
    {
        public static MudTheme Theme => new MudTheme()
        {
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "12px"
            },
            PaletteLight = new PaletteLight()
            {
                Primary = "#2563EB",
                PrimaryContrastText = "#FFFFFF",

                Secondary = "#DBEAFE",
                SecondaryContrastText = "#1E3A8A",

                Background = "#F8FAFC",
                Surface = "#FFFFFF",
                AppbarBackground = "#F8FAFC",

                TextPrimary = "#0F172A",
                TextSecondary = "#475569",
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#60A5FA",
                PrimaryContrastText = "#0F172A",

                Secondary = "#1E293B",
                SecondaryContrastText = "#94A3B8",

                Background = "#0F172A",
                Surface = "#1E293B",
                AppbarBackground = "#0F172A",

                TextPrimary = "#F8FAFC",
                TextSecondary = "#94A3B8"
            },

            Typography = new Typography()
            {
                Default = new DefaultTypography()
                {
                    FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                    FontSize = ".875rem",
                    FontWeight = "300",
                    LineHeight = "1.5",
                    LetterSpacing = "-.01em"
                },
                Subtitle1 = new Subtitle1Typography()
                {
                    FontSize = ".875rem",
                    FontWeight = "300",
                    LineHeight = "1.5",
                    LetterSpacing = "-.01em"
                },
                H4 = new H5Typography()
                {
                    FontSize = "2rem",
                    FontWeight = "750",
                    LineHeight = "1.2",
                    LetterSpacing = "-.02em"
                },
                H5 = new H5Typography()
                {
                    FontSize = "1.5rem",
                    FontWeight = "700",
                    LineHeight = "1.2",
                    LetterSpacing = "-.02em"
                },
                Caption = new CaptionTypography()
                {
                    FontSize = "1rem",
                    FontWeight = "600",
                    LineHeight = "1.2",
                    LetterSpacing = ".01em"
                },
                Body1 = new Body1Typography()
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = ".01em"
                }
            }
        };
    }
}
