using System.Windows.Controls;

namespace dotnetAnima.Core
{
    public static class ButtonHelper
    {
        public static void DisableButton(Button buttonName, bool enable)
        {
            if (!enable)
            {
                buttonName.Opacity = 0.3;
                buttonName.IsEnabled = false;
            }
            else
            {
                buttonName.Opacity = 1;
                buttonName.IsEnabled = true;
            }
        }
    }
}
