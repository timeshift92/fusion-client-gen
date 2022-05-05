using System.Drawing;
using MudBlazor;

namespace Uztelecom.Template.Client.Shared.Theme
{
    public class UtcTheme : MudTheme
    {
        public UtcTheme()
        {
            Palette = new Palette()
            {
                Primary = "#4471AB",
                Background = "#F0F0F5",
                Success = "#7BD08C",
                Error = "#D32F2F",
                AppbarBackground = "#ffffff",
                DrawerBackground = "#ffffff",
                DrawerIcon = "#4471AB",
                TextPrimary = "#262626",
                DrawerText = "#262626",
                TableHover = "rgba(0, 0, 0, 0.04)",
            };
            PaletteDark = new Palette()
            {
                Primary = "#90CAF9",
                Background = "#0F0F12",
                AppbarBackground = "#202128",
                DrawerBackground = "#202128",
                Surface = "#202128",
                Success = "#7BD08C",
                Error = "#F44336",
                DrawerIcon = "#90CAF9",
                TextPrimary = "#d9d9d9",
                TextSecondary = "#d9d9d9",
                DrawerText = "#d9d9d9",
                TableLines = "#595959",
                ActionDefault = "#d9d9d9",
                ActionDisabled = "#d9d9d9",
                PrimaryContrastText = "#262626",
                TableHover = "rgba(255, 255, 255, 0.08)",
            };
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "8px",
            };

        }


    }

}
