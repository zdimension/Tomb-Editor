﻿using DarkUI.Controls;
using System;

namespace TombEditor.WPF.ToolWindows
{
    public partial class ToolPaletteFloating : DarkFloatingToolbox
    {
        public ToolPaletteFloating()
        {
            InitializeComponent();
        }

        private void toolBox_SizeChanged(object sender, EventArgs e)
        {
            AutoSize = true;
        }
    }
}
