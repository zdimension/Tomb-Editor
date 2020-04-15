﻿namespace TombIDE.ScriptEditor
{
	partial class FormSettings
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.button_Apply = new DarkUI.Controls.DarkButton();
			this.button_Cancel = new DarkUI.Controls.DarkButton();
			this.button_CommentColor = new DarkUI.Controls.DarkButton();
			this.button_NewCommandColor = new DarkUI.Controls.DarkButton();
			this.button_OldCommandColor = new DarkUI.Controls.DarkButton();
			this.button_ReferenceColor = new DarkUI.Controls.DarkButton();
			this.button_ReindentRules = new DarkUI.Controls.DarkButton();
			this.button_ResetDefault = new DarkUI.Controls.DarkButton();
			this.button_SectionColor = new DarkUI.Controls.DarkButton();
			this.button_UnknownCommandColor = new DarkUI.Controls.DarkButton();
			this.button_ValueColor = new DarkUI.Controls.DarkButton();
			this.checkBox_AutoCloseBrackets = new DarkUI.Controls.DarkCheckBox();
			this.checkBox_AutoCloseQuotes = new DarkUI.Controls.DarkCheckBox();
			this.checkBox_Autocomplete = new DarkUI.Controls.DarkCheckBox();
			this.checkBox_LiveErrors = new DarkUI.Controls.DarkCheckBox();
			this.checkBox_ReindentOnSave = new DarkUI.Controls.DarkCheckBox();
			this.checkBox_WordWrap = new DarkUI.Controls.DarkCheckBox();
			this.comboBox_FontFace = new DarkUI.Controls.DarkComboBox();
			this.darkGroupBox_01 = new DarkUI.Controls.DarkGroupBox();
			this.label_UndoStackSize = new DarkUI.Controls.DarkLabel();
			this.numeric_UndoStackSize = new DarkUI.Controls.DarkNumericUpDown();
			this.darkGroupBox_02 = new DarkUI.Controls.DarkGroupBox();
			this.label_OldCommandColor = new DarkUI.Controls.DarkLabel();
			this.label_UnknownCommandColor = new DarkUI.Controls.DarkLabel();
			this.label_NewCommandColor = new DarkUI.Controls.DarkLabel();
			this.label_SectionColor = new DarkUI.Controls.DarkLabel();
			this.label_ValueColor = new DarkUI.Controls.DarkLabel();
			this.label_ReferenceColor = new DarkUI.Controls.DarkLabel();
			this.label_CommentColor = new DarkUI.Controls.DarkLabel();
			this.label_FontFace = new DarkUI.Controls.DarkLabel();
			this.numeric_FontSize = new DarkUI.Controls.DarkNumericUpDown();
			this.label_FontSize = new DarkUI.Controls.DarkLabel();
			this.dialog_CommentColor = new System.Windows.Forms.ColorDialog();
			this.dialog_NewCommandColor = new System.Windows.Forms.ColorDialog();
			this.dialog_OldCommandColor = new System.Windows.Forms.ColorDialog();
			this.dialog_ReferenceColor = new System.Windows.Forms.ColorDialog();
			this.dialog_SectionColor = new System.Windows.Forms.ColorDialog();
			this.dialog_UnknownCommandColor = new System.Windows.Forms.ColorDialog();
			this.dialog_ValueColor = new System.Windows.Forms.ColorDialog();
			this.label_RestartRequired = new DarkUI.Controls.DarkLabel();
			this.panel_Buttons = new System.Windows.Forms.Panel();
			this.panel_Main = new System.Windows.Forms.Panel();
			this.darkGroupBox_01.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numeric_UndoStackSize)).BeginInit();
			this.darkGroupBox_02.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numeric_FontSize)).BeginInit();
			this.panel_Buttons.SuspendLayout();
			this.panel_Main.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Apply
			// 
			this.button_Apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Apply.Checked = false;
			this.button_Apply.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Apply.Location = new System.Drawing.Point(316, 8);
			this.button_Apply.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this.button_Apply.Name = "button_Apply";
			this.button_Apply.Size = new System.Drawing.Size(75, 24);
			this.button_Apply.TabIndex = 1;
			this.button_Apply.Text = "Apply";
			this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.Checked = false;
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(397, 8);
			this.button_Cancel.Margin = new System.Windows.Forms.Padding(3, 6, 6, 6);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 24);
			this.button_Cancel.TabIndex = 0;
			this.button_Cancel.Text = "Cancel";
			// 
			// button_CommentColor
			// 
			this.button_CommentColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.button_CommentColor.BackColor = System.Drawing.Color.Green;
			this.button_CommentColor.BackColorUseGeneric = false;
			this.button_CommentColor.Checked = false;
			this.button_CommentColor.Location = new System.Drawing.Point(6, 48);
			this.button_CommentColor.Name = "button_CommentColor";
			this.button_CommentColor.Size = new System.Drawing.Size(276, 25);
			this.button_CommentColor.TabIndex = 18;
			this.button_CommentColor.Click += new System.EventHandler(this.button_CommentColor_Click);
			// 
			// button_NewCommandColor
			// 
			this.button_NewCommandColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.button_NewCommandColor.BackColor = System.Drawing.Color.SpringGreen;
			this.button_NewCommandColor.BackColorUseGeneric = false;
			this.button_NewCommandColor.Checked = false;
			this.button_NewCommandColor.Location = new System.Drawing.Point(6, 256);
			this.button_NewCommandColor.Name = "button_NewCommandColor";
			this.button_NewCommandColor.Size = new System.Drawing.Size(276, 25);
			this.button_NewCommandColor.TabIndex = 22;
			this.button_NewCommandColor.Click += new System.EventHandler(this.button_NewCommandColor_Click);
			// 
			// button_OldCommandColor
			// 
			this.button_OldCommandColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.button_OldCommandColor.BackColor = System.Drawing.Color.MediumAquamarine;
			this.button_OldCommandColor.BackColorUseGeneric = false;
			this.button_OldCommandColor.Checked = false;
			this.button_OldCommandColor.Location = new System.Drawing.Point(6, 308);
			this.button_OldCommandColor.Name = "button_OldCommandColor";
			this.button_OldCommandColor.Size = new System.Drawing.Size(276, 25);
			this.button_OldCommandColor.TabIndex = 25;
			this.button_OldCommandColor.Click += new System.EventHandler(this.button_OldCommandColor_Click);
			// 
			// button_ReferenceColor
			// 
			this.button_ReferenceColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.button_ReferenceColor.BackColor = System.Drawing.Color.Orchid;
			this.button_ReferenceColor.BackColorUseGeneric = false;
			this.button_ReferenceColor.Checked = false;
			this.button_ReferenceColor.Location = new System.Drawing.Point(6, 100);
			this.button_ReferenceColor.Name = "button_ReferenceColor";
			this.button_ReferenceColor.Size = new System.Drawing.Size(276, 25);
			this.button_ReferenceColor.TabIndex = 19;
			this.button_ReferenceColor.Click += new System.EventHandler(this.button_ReferenceColor_Click);
			// 
			// button_ReindentRules
			// 
			this.button_ReindentRules.Checked = false;
			this.button_ReindentRules.Location = new System.Drawing.Point(6, 393);
			this.button_ReindentRules.Name = "button_ReindentRules";
			this.button_ReindentRules.Size = new System.Drawing.Size(150, 23);
			this.button_ReindentRules.TabIndex = 16;
			this.button_ReindentRules.Text = "Reindent Rules...";
			this.button_ReindentRules.Click += new System.EventHandler(this.button_ReindentRules_Click);
			// 
			// button_ResetDefault
			// 
			this.button_ResetDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_ResetDefault.Checked = false;
			this.button_ResetDefault.Location = new System.Drawing.Point(6, 8);
			this.button_ResetDefault.Margin = new System.Windows.Forms.Padding(6, 6, 0, 6);
			this.button_ResetDefault.Name = "button_ResetDefault";
			this.button_ResetDefault.Size = new System.Drawing.Size(150, 24);
			this.button_ResetDefault.TabIndex = 2;
			this.button_ResetDefault.Text = "Reset settings to default";
			this.button_ResetDefault.Click += new System.EventHandler(this.button_ResetDefault_Click);
			// 
			// button_SectionColor
			// 
			this.button_SectionColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.button_SectionColor.BackColor = System.Drawing.Color.SteelBlue;
			this.button_SectionColor.BackColorUseGeneric = false;
			this.button_SectionColor.Checked = false;
			this.button_SectionColor.Location = new System.Drawing.Point(6, 204);
			this.button_SectionColor.Name = "button_SectionColor";
			this.button_SectionColor.Size = new System.Drawing.Size(276, 25);
			this.button_SectionColor.TabIndex = 21;
			this.button_SectionColor.Click += new System.EventHandler(this.button_SectionColor_Click);
			// 
			// button_UnknownCommandColor
			// 
			this.button_UnknownCommandColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.button_UnknownCommandColor.BackColor = System.Drawing.Color.Red;
			this.button_UnknownCommandColor.BackColorUseGeneric = false;
			this.button_UnknownCommandColor.Checked = false;
			this.button_UnknownCommandColor.Location = new System.Drawing.Point(6, 360);
			this.button_UnknownCommandColor.Name = "button_UnknownCommandColor";
			this.button_UnknownCommandColor.Size = new System.Drawing.Size(276, 25);
			this.button_UnknownCommandColor.TabIndex = 23;
			this.button_UnknownCommandColor.Click += new System.EventHandler(this.button_UnknownCommandColor_Click);
			// 
			// button_ValueColor
			// 
			this.button_ValueColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.button_ValueColor.BackColor = System.Drawing.Color.LightSalmon;
			this.button_ValueColor.BackColorUseGeneric = false;
			this.button_ValueColor.Checked = false;
			this.button_ValueColor.Location = new System.Drawing.Point(6, 152);
			this.button_ValueColor.Name = "button_ValueColor";
			this.button_ValueColor.Size = new System.Drawing.Size(276, 25);
			this.button_ValueColor.TabIndex = 20;
			this.button_ValueColor.Click += new System.EventHandler(this.button_ValueColor_Click);
			// 
			// checkBox_AutoCloseBrackets
			// 
			this.checkBox_AutoCloseBrackets.AutoSize = true;
			this.checkBox_AutoCloseBrackets.Location = new System.Drawing.Point(6, 217);
			this.checkBox_AutoCloseBrackets.Name = "checkBox_AutoCloseBrackets";
			this.checkBox_AutoCloseBrackets.Size = new System.Drawing.Size(122, 17);
			this.checkBox_AutoCloseBrackets.TabIndex = 9;
			this.checkBox_AutoCloseBrackets.Text = "Auto Close Brackets";
			// 
			// checkBox_AutoCloseQuotes
			// 
			this.checkBox_AutoCloseQuotes.AutoSize = true;
			this.checkBox_AutoCloseQuotes.Location = new System.Drawing.Point(6, 240);
			this.checkBox_AutoCloseQuotes.Name = "checkBox_AutoCloseQuotes";
			this.checkBox_AutoCloseQuotes.Size = new System.Drawing.Size(114, 17);
			this.checkBox_AutoCloseQuotes.TabIndex = 21;
			this.checkBox_AutoCloseQuotes.Text = "Auto Close Quotes";
			// 
			// checkBox_Autocomplete
			// 
			this.checkBox_Autocomplete.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.checkBox_Autocomplete.AutoSize = true;
			this.checkBox_Autocomplete.Location = new System.Drawing.Point(6, 156);
			this.checkBox_Autocomplete.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
			this.checkBox_Autocomplete.Name = "checkBox_Autocomplete";
			this.checkBox_Autocomplete.Size = new System.Drawing.Size(127, 17);
			this.checkBox_Autocomplete.TabIndex = 13;
			this.checkBox_Autocomplete.Text = "Enable Autocomplete";
			this.checkBox_Autocomplete.CheckedChanged += new System.EventHandler(this.checkBox_Autocomplete_CheckedChanged);
			// 
			// checkBox_LiveErrors
			// 
			this.checkBox_LiveErrors.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.checkBox_LiveErrors.AutoSize = true;
			this.checkBox_LiveErrors.Location = new System.Drawing.Point(6, 179);
			this.checkBox_LiveErrors.Name = "checkBox_LiveErrors";
			this.checkBox_LiveErrors.Size = new System.Drawing.Size(120, 17);
			this.checkBox_LiveErrors.TabIndex = 20;
			this.checkBox_LiveErrors.Text = "Live Error Detection";
			// 
			// checkBox_ReindentOnSave
			// 
			this.checkBox_ReindentOnSave.AutoSize = true;
			this.checkBox_ReindentOnSave.Location = new System.Drawing.Point(6, 370);
			this.checkBox_ReindentOnSave.Margin = new System.Windows.Forms.Padding(3, 16, 3, 3);
			this.checkBox_ReindentOnSave.Name = "checkBox_ReindentOnSave";
			this.checkBox_ReindentOnSave.Size = new System.Drawing.Size(112, 17);
			this.checkBox_ReindentOnSave.TabIndex = 10;
			this.checkBox_ReindentOnSave.Text = "Reindent on Save";
			// 
			// checkBox_WordWrap
			// 
			this.checkBox_WordWrap.AutoSize = true;
			this.checkBox_WordWrap.Location = new System.Drawing.Point(6, 263);
			this.checkBox_WordWrap.Name = "checkBox_WordWrap";
			this.checkBox_WordWrap.Size = new System.Drawing.Size(81, 17);
			this.checkBox_WordWrap.TabIndex = 12;
			this.checkBox_WordWrap.Text = "Word Wrap";
			// 
			// comboBox_FontFace
			// 
			this.comboBox_FontFace.FormattingEnabled = true;
			this.comboBox_FontFace.Location = new System.Drawing.Point(6, 77);
			this.comboBox_FontFace.Name = "comboBox_FontFace";
			this.comboBox_FontFace.Size = new System.Drawing.Size(150, 21);
			this.comboBox_FontFace.TabIndex = 3;
			// 
			// darkGroupBox_01
			// 
			this.darkGroupBox_01.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.darkGroupBox_01.Controls.Add(this.checkBox_AutoCloseQuotes);
			this.darkGroupBox_01.Controls.Add(this.checkBox_LiveErrors);
			this.darkGroupBox_01.Controls.Add(this.label_UndoStackSize);
			this.darkGroupBox_01.Controls.Add(this.numeric_UndoStackSize);
			this.darkGroupBox_01.Controls.Add(this.button_ReindentRules);
			this.darkGroupBox_01.Controls.Add(this.checkBox_Autocomplete);
			this.darkGroupBox_01.Controls.Add(this.checkBox_WordWrap);
			this.darkGroupBox_01.Controls.Add(this.checkBox_ReindentOnSave);
			this.darkGroupBox_01.Controls.Add(this.darkGroupBox_02);
			this.darkGroupBox_01.Controls.Add(this.checkBox_AutoCloseBrackets);
			this.darkGroupBox_01.Controls.Add(this.comboBox_FontFace);
			this.darkGroupBox_01.Controls.Add(this.label_FontFace);
			this.darkGroupBox_01.Controls.Add(this.numeric_FontSize);
			this.darkGroupBox_01.Controls.Add(this.label_FontSize);
			this.darkGroupBox_01.Location = new System.Drawing.Point(11, 11);
			this.darkGroupBox_01.Name = "darkGroupBox_01";
			this.darkGroupBox_01.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.darkGroupBox_01.Size = new System.Drawing.Size(456, 422);
			this.darkGroupBox_01.TabIndex = 2;
			this.darkGroupBox_01.TabStop = false;
			// 
			// label_UndoStackSize
			// 
			this.label_UndoStackSize.AutoSize = true;
			this.label_UndoStackSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_UndoStackSize.Location = new System.Drawing.Point(6, 104);
			this.label_UndoStackSize.Margin = new System.Windows.Forms.Padding(3);
			this.label_UndoStackSize.Name = "label_UndoStackSize";
			this.label_UndoStackSize.Size = new System.Drawing.Size(90, 13);
			this.label_UndoStackSize.TabIndex = 19;
			this.label_UndoStackSize.Text = "Undo Stack Size:";
			// 
			// numeric_UndoStackSize
			// 
			this.numeric_UndoStackSize.IncrementAlternate = new decimal(new int[] {
            10,
            0,
            0,
            65536});
			this.numeric_UndoStackSize.Location = new System.Drawing.Point(6, 123);
			this.numeric_UndoStackSize.LoopValues = false;
			this.numeric_UndoStackSize.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			this.numeric_UndoStackSize.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numeric_UndoStackSize.Name = "numeric_UndoStackSize";
			this.numeric_UndoStackSize.Size = new System.Drawing.Size(150, 20);
			this.numeric_UndoStackSize.TabIndex = 18;
			this.numeric_UndoStackSize.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.numeric_UndoStackSize.ValueChanged += new System.EventHandler(this.numeric_UndoStackSize_ValueChanged);
			// 
			// darkGroupBox_02
			// 
			this.darkGroupBox_02.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.darkGroupBox_02.Controls.Add(this.button_OldCommandColor);
			this.darkGroupBox_02.Controls.Add(this.label_OldCommandColor);
			this.darkGroupBox_02.Controls.Add(this.button_UnknownCommandColor);
			this.darkGroupBox_02.Controls.Add(this.button_NewCommandColor);
			this.darkGroupBox_02.Controls.Add(this.button_SectionColor);
			this.darkGroupBox_02.Controls.Add(this.button_ValueColor);
			this.darkGroupBox_02.Controls.Add(this.button_ReferenceColor);
			this.darkGroupBox_02.Controls.Add(this.button_CommentColor);
			this.darkGroupBox_02.Controls.Add(this.label_UnknownCommandColor);
			this.darkGroupBox_02.Controls.Add(this.label_NewCommandColor);
			this.darkGroupBox_02.Controls.Add(this.label_SectionColor);
			this.darkGroupBox_02.Controls.Add(this.label_ValueColor);
			this.darkGroupBox_02.Controls.Add(this.label_ReferenceColor);
			this.darkGroupBox_02.Controls.Add(this.label_CommentColor);
			this.darkGroupBox_02.Location = new System.Drawing.Point(162, 13);
			this.darkGroupBox_02.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.darkGroupBox_02.Name = "darkGroupBox_02";
			this.darkGroupBox_02.Size = new System.Drawing.Size(288, 403);
			this.darkGroupBox_02.TabIndex = 6;
			this.darkGroupBox_02.TabStop = false;
			this.darkGroupBox_02.Text = "Syntax Colors";
			// 
			// label_OldCommandColor
			// 
			this.label_OldCommandColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label_OldCommandColor.AutoSize = true;
			this.label_OldCommandColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_OldCommandColor.Location = new System.Drawing.Point(6, 288);
			this.label_OldCommandColor.Name = "label_OldCommandColor";
			this.label_OldCommandColor.Size = new System.Drawing.Size(108, 13);
			this.label_OldCommandColor.TabIndex = 24;
			this.label_OldCommandColor.Text = "Standard Commands:";
			// 
			// label_UnknownCommandColor
			// 
			this.label_UnknownCommandColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label_UnknownCommandColor.AutoSize = true;
			this.label_UnknownCommandColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_UnknownCommandColor.Location = new System.Drawing.Point(6, 340);
			this.label_UnknownCommandColor.Name = "label_UnknownCommandColor";
			this.label_UnknownCommandColor.Size = new System.Drawing.Size(56, 13);
			this.label_UnknownCommandColor.TabIndex = 17;
			this.label_UnknownCommandColor.Text = "Unknown:";
			// 
			// label_NewCommandColor
			// 
			this.label_NewCommandColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label_NewCommandColor.AutoSize = true;
			this.label_NewCommandColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_NewCommandColor.Location = new System.Drawing.Point(6, 236);
			this.label_NewCommandColor.Name = "label_NewCommandColor";
			this.label_NewCommandColor.Size = new System.Drawing.Size(151, 13);
			this.label_NewCommandColor.TabIndex = 16;
			this.label_NewCommandColor.Text = "TRNG / TR5Main Commands:";
			// 
			// label_SectionColor
			// 
			this.label_SectionColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label_SectionColor.AutoSize = true;
			this.label_SectionColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_SectionColor.Location = new System.Drawing.Point(6, 184);
			this.label_SectionColor.Name = "label_SectionColor";
			this.label_SectionColor.Size = new System.Drawing.Size(51, 13);
			this.label_SectionColor.TabIndex = 15;
			this.label_SectionColor.Text = "Sections:";
			// 
			// label_ValueColor
			// 
			this.label_ValueColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label_ValueColor.AutoSize = true;
			this.label_ValueColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_ValueColor.Location = new System.Drawing.Point(6, 132);
			this.label_ValueColor.Name = "label_ValueColor";
			this.label_ValueColor.Size = new System.Drawing.Size(42, 13);
			this.label_ValueColor.TabIndex = 14;
			this.label_ValueColor.Text = "Values:";
			// 
			// label_ReferenceColor
			// 
			this.label_ReferenceColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label_ReferenceColor.AutoSize = true;
			this.label_ReferenceColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_ReferenceColor.Location = new System.Drawing.Point(6, 80);
			this.label_ReferenceColor.Name = "label_ReferenceColor";
			this.label_ReferenceColor.Size = new System.Drawing.Size(65, 13);
			this.label_ReferenceColor.TabIndex = 13;
			this.label_ReferenceColor.Text = "References:";
			// 
			// label_CommentColor
			// 
			this.label_CommentColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label_CommentColor.AutoSize = true;
			this.label_CommentColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_CommentColor.Location = new System.Drawing.Point(6, 28);
			this.label_CommentColor.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.label_CommentColor.Name = "label_CommentColor";
			this.label_CommentColor.Size = new System.Drawing.Size(59, 13);
			this.label_CommentColor.TabIndex = 12;
			this.label_CommentColor.Text = "Comments:";
			// 
			// label_FontFace
			// 
			this.label_FontFace.AutoSize = true;
			this.label_FontFace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_FontFace.Location = new System.Drawing.Point(6, 58);
			this.label_FontFace.Margin = new System.Windows.Forms.Padding(3);
			this.label_FontFace.Name = "label_FontFace";
			this.label_FontFace.Size = new System.Drawing.Size(58, 13);
			this.label_FontFace.TabIndex = 2;
			this.label_FontFace.Text = "Font Face:";
			// 
			// numeric_FontSize
			// 
			this.numeric_FontSize.IncrementAlternate = new decimal(new int[] {
            10,
            0,
            0,
            65536});
			this.numeric_FontSize.Location = new System.Drawing.Point(6, 32);
			this.numeric_FontSize.LoopValues = false;
			this.numeric_FontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.numeric_FontSize.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.numeric_FontSize.Name = "numeric_FontSize";
			this.numeric_FontSize.Size = new System.Drawing.Size(150, 20);
			this.numeric_FontSize.TabIndex = 1;
			this.numeric_FontSize.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			// 
			// label_FontSize
			// 
			this.label_FontSize.AutoSize = true;
			this.label_FontSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_FontSize.Location = new System.Drawing.Point(6, 13);
			this.label_FontSize.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.label_FontSize.Name = "label_FontSize";
			this.label_FontSize.Size = new System.Drawing.Size(54, 13);
			this.label_FontSize.TabIndex = 0;
			this.label_FontSize.Text = "Font Size:";
			// 
			// dialog_CommentColor
			// 
			this.dialog_CommentColor.AnyColor = true;
			this.dialog_CommentColor.FullOpen = true;
			// 
			// dialog_NewCommandColor
			// 
			this.dialog_NewCommandColor.AnyColor = true;
			this.dialog_NewCommandColor.FullOpen = true;
			// 
			// dialog_OldCommandColor
			// 
			this.dialog_OldCommandColor.AnyColor = true;
			this.dialog_OldCommandColor.FullOpen = true;
			// 
			// dialog_ReferenceColor
			// 
			this.dialog_ReferenceColor.AnyColor = true;
			this.dialog_ReferenceColor.FullOpen = true;
			// 
			// dialog_SectionColor
			// 
			this.dialog_SectionColor.AnyColor = true;
			this.dialog_SectionColor.FullOpen = true;
			// 
			// dialog_UnknownCommandColor
			// 
			this.dialog_UnknownCommandColor.AnyColor = true;
			this.dialog_UnknownCommandColor.FullOpen = true;
			// 
			// dialog_ValueColor
			// 
			this.dialog_ValueColor.AnyColor = true;
			this.dialog_ValueColor.FullOpen = true;
			// 
			// label_RestartRequired
			// 
			this.label_RestartRequired.AutoSize = true;
			this.label_RestartRequired.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.label_RestartRequired.Location = new System.Drawing.Point(225, 14);
			this.label_RestartRequired.Name = "label_RestartRequired";
			this.label_RestartRequired.Size = new System.Drawing.Size(85, 13);
			this.label_RestartRequired.TabIndex = 3;
			this.label_RestartRequired.Text = "Restart required.";
			this.label_RestartRequired.Visible = false;
			// 
			// panel_Buttons
			// 
			this.panel_Buttons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_Buttons.Controls.Add(this.label_RestartRequired);
			this.panel_Buttons.Controls.Add(this.button_ResetDefault);
			this.panel_Buttons.Controls.Add(this.button_Apply);
			this.panel_Buttons.Controls.Add(this.button_Cancel);
			this.panel_Buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel_Buttons.Location = new System.Drawing.Point(0, 440);
			this.panel_Buttons.Name = "panel_Buttons";
			this.panel_Buttons.Size = new System.Drawing.Size(480, 40);
			this.panel_Buttons.TabIndex = 3;
			// 
			// panel_Main
			// 
			this.panel_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel_Main.Controls.Add(this.darkGroupBox_01);
			this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Main.Location = new System.Drawing.Point(0, 0);
			this.panel_Main.Name = "panel_Main";
			this.panel_Main.Size = new System.Drawing.Size(480, 440);
			this.panel_Main.TabIndex = 4;
			// 
			// FormSettings
			// 
			this.AcceptButton = this.button_Apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(480, 480);
			this.Controls.Add(this.panel_Main);
			this.Controls.Add(this.panel_Buttons);
			this.FlatBorder = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FormSettings";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Script Editor Settings";
			this.darkGroupBox_01.ResumeLayout(false);
			this.darkGroupBox_01.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numeric_UndoStackSize)).EndInit();
			this.darkGroupBox_02.ResumeLayout(false);
			this.darkGroupBox_02.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numeric_FontSize)).EndInit();
			this.panel_Buttons.ResumeLayout(false);
			this.panel_Buttons.PerformLayout();
			this.panel_Main.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DarkUI.Controls.DarkButton button_Apply;
		private DarkUI.Controls.DarkButton button_Cancel;
		private DarkUI.Controls.DarkButton button_CommentColor;
		private DarkUI.Controls.DarkButton button_NewCommandColor;
		private DarkUI.Controls.DarkButton button_OldCommandColor;
		private DarkUI.Controls.DarkButton button_ReferenceColor;
		private DarkUI.Controls.DarkButton button_ReindentRules;
		private DarkUI.Controls.DarkButton button_ResetDefault;
		private DarkUI.Controls.DarkButton button_SectionColor;
		private DarkUI.Controls.DarkButton button_UnknownCommandColor;
		private DarkUI.Controls.DarkButton button_ValueColor;
		private DarkUI.Controls.DarkCheckBox checkBox_AutoCloseBrackets;
		private DarkUI.Controls.DarkCheckBox checkBox_AutoCloseQuotes;
		private DarkUI.Controls.DarkCheckBox checkBox_Autocomplete;
		private DarkUI.Controls.DarkCheckBox checkBox_LiveErrors;
		private DarkUI.Controls.DarkCheckBox checkBox_ReindentOnSave;
		private DarkUI.Controls.DarkCheckBox checkBox_WordWrap;
		private DarkUI.Controls.DarkComboBox comboBox_FontFace;
		private DarkUI.Controls.DarkGroupBox darkGroupBox_01;
		private DarkUI.Controls.DarkGroupBox darkGroupBox_02;
		private DarkUI.Controls.DarkLabel label_CommentColor;
		private DarkUI.Controls.DarkLabel label_FontFace;
		private DarkUI.Controls.DarkLabel label_FontSize;
		private DarkUI.Controls.DarkLabel label_NewCommandColor;
		private DarkUI.Controls.DarkLabel label_OldCommandColor;
		private DarkUI.Controls.DarkLabel label_ReferenceColor;
		private DarkUI.Controls.DarkLabel label_RestartRequired;
		private DarkUI.Controls.DarkLabel label_SectionColor;
		private DarkUI.Controls.DarkLabel label_UndoStackSize;
		private DarkUI.Controls.DarkLabel label_UnknownCommandColor;
		private DarkUI.Controls.DarkLabel label_ValueColor;
		private DarkUI.Controls.DarkNumericUpDown numeric_FontSize;
		private DarkUI.Controls.DarkNumericUpDown numeric_UndoStackSize;
		private System.Windows.Forms.ColorDialog dialog_CommentColor;
		private System.Windows.Forms.ColorDialog dialog_NewCommandColor;
		private System.Windows.Forms.ColorDialog dialog_OldCommandColor;
		private System.Windows.Forms.ColorDialog dialog_ReferenceColor;
		private System.Windows.Forms.ColorDialog dialog_SectionColor;
		private System.Windows.Forms.ColorDialog dialog_UnknownCommandColor;
		private System.Windows.Forms.ColorDialog dialog_ValueColor;
		private System.Windows.Forms.Panel panel_Buttons;
		private System.Windows.Forms.Panel panel_Main;
	}
}