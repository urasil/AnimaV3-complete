using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace dotnetAnima.Core
{
    public static class ButtonHelper
    {
        public static void DisableButton(Button buttonName, bool status) 
        {
            if(!status)
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
