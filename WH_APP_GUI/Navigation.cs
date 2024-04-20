using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WH_APP_GUI
{
    internal class Navigation
    {
        public static Frame content2 = new Frame();
        
        public static void RemoveParent()
        {
            if(content2.Parent !=  null) {
                Grid parentGrid = content2.Parent as Grid;
                parentGrid.Children.Remove(content2);
            }
            
        }
    }
}
