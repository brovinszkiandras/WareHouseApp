using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WH_APP_GUI
{
    internal class Console
    {
        private static Terminal ThisTerminal = null;
        public static void IniConsole()
        { 
            Terminal terminal = new Terminal();
            terminal.Show();

            ThisTerminal = terminal;
        }

        public static void WriteLine(object text)
        {
            if (ThisTerminal != null)
            {
                ThisTerminal.TerminalBox.Items.Add(text.ToString());
            }
            else
            {
                Terminal terminal = new Terminal();
                terminal.Show();
                ThisTerminal = terminal;
            }
        }
    }
}
