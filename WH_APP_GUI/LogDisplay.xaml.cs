﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WH_APP_GUI
{
    public partial class LogDisplay : Page
    {
        public LogDisplay()
        {
            InitializeComponent();

            List<string[]> Datas = SQL.SqlQuery("SELECT * FROM log");
            for (int i = 0; i < Datas.Count; i++)
            {
                Logs.Items.Add($"[{Datas[i][3]}]: {Datas[i][2]}");
            }
        }
    }
}
