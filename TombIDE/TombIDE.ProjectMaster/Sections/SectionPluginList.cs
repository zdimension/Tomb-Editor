﻿using System.Windows.Forms;
using TombIDE.Shared;

namespace TombIDE.ProjectMaster
{
	public partial class SectionPluginList : UserControl
	{
		private IDE _ide;

		public SectionPluginList()
		{
			InitializeComponent();
		}

		public void Initialize(IDE ide)
		{
			_ide = ide;

			// TODO
		}

		private void button_ManagePlugins_Click(object sender, System.EventArgs e)
		{
			using (FormPluginLibrary form = new FormPluginLibrary(_ide))
			{
				form.ShowDialog(this);
			}
		}
	}
}
