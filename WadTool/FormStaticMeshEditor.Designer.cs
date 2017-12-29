﻿namespace WadTool
{
    partial class FormStaticMeshEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            this.panelRendering = new WadTool.Controls.PanelRenderingStaticMeshEditor();
            this.cbVisibilityBox = new DarkUI.Controls.DarkCheckBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.tbVisibilityBoxMinX = new DarkUI.Controls.DarkTextBox();
            this.tbVisibilityBoxMinY = new DarkUI.Controls.DarkTextBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.tbVisibilityBoxMinZ = new DarkUI.Controls.DarkTextBox();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.tbVisibilityBoxMaxZ = new DarkUI.Controls.DarkTextBox();
            this.darkLabel5 = new DarkUI.Controls.DarkLabel();
            this.tbVisibilityBoxMaxY = new DarkUI.Controls.DarkTextBox();
            this.darkLabel6 = new DarkUI.Controls.DarkLabel();
            this.tbVisibilityBoxMaxX = new DarkUI.Controls.DarkTextBox();
            this.darkLabel7 = new DarkUI.Controls.DarkLabel();
            this.butCalculateVisibilityBox = new DarkUI.Controls.DarkButton();
            this.butSaveChanges = new DarkUI.Controls.DarkButton();
            this.butCalculateCollisionBox = new DarkUI.Controls.DarkButton();
            this.tbCollisionBoxMaxZ = new DarkUI.Controls.DarkTextBox();
            this.darkLabel8 = new DarkUI.Controls.DarkLabel();
            this.tbCollisionBoxMaxY = new DarkUI.Controls.DarkTextBox();
            this.darkLabel9 = new DarkUI.Controls.DarkLabel();
            this.tbCollisionBoxMaxX = new DarkUI.Controls.DarkTextBox();
            this.darkLabel10 = new DarkUI.Controls.DarkLabel();
            this.tbCollisionBoxMinZ = new DarkUI.Controls.DarkTextBox();
            this.darkLabel11 = new DarkUI.Controls.DarkLabel();
            this.tbCollisionBoxMinY = new DarkUI.Controls.DarkTextBox();
            this.darkLabel12 = new DarkUI.Controls.DarkLabel();
            this.tbCollisionBoxMinX = new DarkUI.Controls.DarkTextBox();
            this.darkLabel13 = new DarkUI.Controls.DarkLabel();
            this.cbCollisionBox = new DarkUI.Controls.DarkCheckBox();
            this.cbDrawGrid = new DarkUI.Controls.DarkCheckBox();
            this.cbDrawGizmo = new DarkUI.Controls.DarkCheckBox();
            this.SuspendLayout();
            // 
            // darkStatusStrip1
            // 
            this.darkStatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkStatusStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkStatusStrip1.Location = new System.Drawing.Point(0, 540);
            this.darkStatusStrip1.Name = "darkStatusStrip1";
            this.darkStatusStrip1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.darkStatusStrip1.Size = new System.Drawing.Size(829, 24);
            this.darkStatusStrip1.TabIndex = 0;
            this.darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // panelRendering
            // 
            this.panelRendering.Camera = null;
            this.panelRendering.DrawCollisionBox = false;
            this.panelRendering.DrawGrid = false;
            this.panelRendering.DrawVisibilityBox = false;
            this.panelRendering.Location = new System.Drawing.Point(13, 13);
            this.panelRendering.Name = "panelRendering";
            this.panelRendering.Size = new System.Drawing.Size(564, 513);
            this.panelRendering.StaticMesh = null;
            this.panelRendering.StaticScale = 1F;
            this.panelRendering.TabIndex = 1;
            // 
            // cbVisibilityBox
            // 
            this.cbVisibilityBox.AutoSize = true;
            this.cbVisibilityBox.Location = new System.Drawing.Point(587, 14);
            this.cbVisibilityBox.Name = "cbVisibilityBox";
            this.cbVisibilityBox.Size = new System.Drawing.Size(91, 17);
            this.cbVisibilityBox.TabIndex = 50;
            this.cbVisibilityBox.Text = "Visibility Box";
            this.cbVisibilityBox.CheckedChanged += new System.EventHandler(this.cbVisibilityBox_CheckedChanged);
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(584, 40);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(38, 13);
            this.darkLabel2.TabIndex = 51;
            this.darkLabel2.Text = "X min:";
            // 
            // tbVisibilityBoxMinX
            // 
            this.tbVisibilityBoxMinX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVisibilityBoxMinX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVisibilityBoxMinX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVisibilityBoxMinX.Location = new System.Drawing.Point(587, 57);
            this.tbVisibilityBoxMinX.Name = "tbVisibilityBoxMinX";
            this.tbVisibilityBoxMinX.Size = new System.Drawing.Size(73, 22);
            this.tbVisibilityBoxMinX.TabIndex = 52;
            // 
            // tbVisibilityBoxMinY
            // 
            this.tbVisibilityBoxMinY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVisibilityBoxMinY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVisibilityBoxMinY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVisibilityBoxMinY.Location = new System.Drawing.Point(666, 57);
            this.tbVisibilityBoxMinY.Name = "tbVisibilityBoxMinY";
            this.tbVisibilityBoxMinY.Size = new System.Drawing.Size(73, 22);
            this.tbVisibilityBoxMinY.TabIndex = 54;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(663, 40);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(37, 13);
            this.darkLabel3.TabIndex = 53;
            this.darkLabel3.Text = "Y min:";
            // 
            // tbVisibilityBoxMinZ
            // 
            this.tbVisibilityBoxMinZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVisibilityBoxMinZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVisibilityBoxMinZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVisibilityBoxMinZ.Location = new System.Drawing.Point(745, 57);
            this.tbVisibilityBoxMinZ.Name = "tbVisibilityBoxMinZ";
            this.tbVisibilityBoxMinZ.Size = new System.Drawing.Size(72, 22);
            this.tbVisibilityBoxMinZ.TabIndex = 56;
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(742, 40);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(38, 13);
            this.darkLabel4.TabIndex = 55;
            this.darkLabel4.Text = "Z min:";
            // 
            // tbVisibilityBoxMaxZ
            // 
            this.tbVisibilityBoxMaxZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVisibilityBoxMaxZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVisibilityBoxMaxZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVisibilityBoxMaxZ.Location = new System.Drawing.Point(745, 105);
            this.tbVisibilityBoxMaxZ.Name = "tbVisibilityBoxMaxZ";
            this.tbVisibilityBoxMaxZ.Size = new System.Drawing.Size(72, 22);
            this.tbVisibilityBoxMaxZ.TabIndex = 62;
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel5.Location = new System.Drawing.Point(742, 88);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(39, 13);
            this.darkLabel5.TabIndex = 61;
            this.darkLabel5.Text = "Z max:";
            // 
            // tbVisibilityBoxMaxY
            // 
            this.tbVisibilityBoxMaxY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVisibilityBoxMaxY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVisibilityBoxMaxY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVisibilityBoxMaxY.Location = new System.Drawing.Point(666, 105);
            this.tbVisibilityBoxMaxY.Name = "tbVisibilityBoxMaxY";
            this.tbVisibilityBoxMaxY.Size = new System.Drawing.Size(73, 22);
            this.tbVisibilityBoxMaxY.TabIndex = 60;
            // 
            // darkLabel6
            // 
            this.darkLabel6.AutoSize = true;
            this.darkLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel6.Location = new System.Drawing.Point(663, 88);
            this.darkLabel6.Name = "darkLabel6";
            this.darkLabel6.Size = new System.Drawing.Size(38, 13);
            this.darkLabel6.TabIndex = 59;
            this.darkLabel6.Text = "Y max:";
            // 
            // tbVisibilityBoxMaxX
            // 
            this.tbVisibilityBoxMaxX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbVisibilityBoxMaxX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbVisibilityBoxMaxX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbVisibilityBoxMaxX.Location = new System.Drawing.Point(587, 105);
            this.tbVisibilityBoxMaxX.Name = "tbVisibilityBoxMaxX";
            this.tbVisibilityBoxMaxX.Size = new System.Drawing.Size(73, 22);
            this.tbVisibilityBoxMaxX.TabIndex = 58;
            // 
            // darkLabel7
            // 
            this.darkLabel7.AutoSize = true;
            this.darkLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel7.Location = new System.Drawing.Point(584, 88);
            this.darkLabel7.Name = "darkLabel7";
            this.darkLabel7.Size = new System.Drawing.Size(39, 13);
            this.darkLabel7.TabIndex = 57;
            this.darkLabel7.Text = "X max:";
            // 
            // butCalculateVisibilityBox
            // 
            this.butCalculateVisibilityBox.Image = global::WadTool.Properties.Resources.resize_16;
            this.butCalculateVisibilityBox.Location = new System.Drawing.Point(587, 143);
            this.butCalculateVisibilityBox.Name = "butCalculateVisibilityBox";
            this.butCalculateVisibilityBox.Padding = new System.Windows.Forms.Padding(5);
            this.butCalculateVisibilityBox.Size = new System.Drawing.Size(230, 23);
            this.butCalculateVisibilityBox.TabIndex = 63;
            this.butCalculateVisibilityBox.Text = "Calculate visibility box";
            this.butCalculateVisibilityBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butCalculateVisibilityBox.Click += new System.EventHandler(this.butCalculateVisibilityBox_Click);
            // 
            // butSaveChanges
            // 
            this.butSaveChanges.Image = global::WadTool.Properties.Resources.save_16;
            this.butSaveChanges.Location = new System.Drawing.Point(705, 503);
            this.butSaveChanges.Name = "butSaveChanges";
            this.butSaveChanges.Padding = new System.Windows.Forms.Padding(5);
            this.butSaveChanges.Size = new System.Drawing.Size(112, 23);
            this.butSaveChanges.TabIndex = 46;
            this.butSaveChanges.Text = "Save changes";
            this.butSaveChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butSaveChanges.Click += new System.EventHandler(this.butSaveChanges_Click);
            // 
            // butCalculateCollisionBox
            // 
            this.butCalculateCollisionBox.Image = global::WadTool.Properties.Resources.resize_16;
            this.butCalculateCollisionBox.Location = new System.Drawing.Point(587, 317);
            this.butCalculateCollisionBox.Name = "butCalculateCollisionBox";
            this.butCalculateCollisionBox.Padding = new System.Windows.Forms.Padding(5);
            this.butCalculateCollisionBox.Size = new System.Drawing.Size(230, 23);
            this.butCalculateCollisionBox.TabIndex = 77;
            this.butCalculateCollisionBox.Text = "Calculate collision box";
            this.butCalculateCollisionBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butCalculateCollisionBox.Click += new System.EventHandler(this.butCalculateCollisionBox_Click);
            // 
            // tbCollisionBoxMaxZ
            // 
            this.tbCollisionBoxMaxZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbCollisionBoxMaxZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCollisionBoxMaxZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbCollisionBoxMaxZ.Location = new System.Drawing.Point(745, 279);
            this.tbCollisionBoxMaxZ.Name = "tbCollisionBoxMaxZ";
            this.tbCollisionBoxMaxZ.Size = new System.Drawing.Size(72, 22);
            this.tbCollisionBoxMaxZ.TabIndex = 76;
            // 
            // darkLabel8
            // 
            this.darkLabel8.AutoSize = true;
            this.darkLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel8.Location = new System.Drawing.Point(742, 262);
            this.darkLabel8.Name = "darkLabel8";
            this.darkLabel8.Size = new System.Drawing.Size(39, 13);
            this.darkLabel8.TabIndex = 75;
            this.darkLabel8.Text = "Z max:";
            // 
            // tbCollisionBoxMaxY
            // 
            this.tbCollisionBoxMaxY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbCollisionBoxMaxY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCollisionBoxMaxY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbCollisionBoxMaxY.Location = new System.Drawing.Point(666, 279);
            this.tbCollisionBoxMaxY.Name = "tbCollisionBoxMaxY";
            this.tbCollisionBoxMaxY.Size = new System.Drawing.Size(73, 22);
            this.tbCollisionBoxMaxY.TabIndex = 74;
            // 
            // darkLabel9
            // 
            this.darkLabel9.AutoSize = true;
            this.darkLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel9.Location = new System.Drawing.Point(663, 262);
            this.darkLabel9.Name = "darkLabel9";
            this.darkLabel9.Size = new System.Drawing.Size(38, 13);
            this.darkLabel9.TabIndex = 73;
            this.darkLabel9.Text = "Y max:";
            // 
            // tbCollisionBoxMaxX
            // 
            this.tbCollisionBoxMaxX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbCollisionBoxMaxX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCollisionBoxMaxX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbCollisionBoxMaxX.Location = new System.Drawing.Point(587, 279);
            this.tbCollisionBoxMaxX.Name = "tbCollisionBoxMaxX";
            this.tbCollisionBoxMaxX.Size = new System.Drawing.Size(73, 22);
            this.tbCollisionBoxMaxX.TabIndex = 72;
            // 
            // darkLabel10
            // 
            this.darkLabel10.AutoSize = true;
            this.darkLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel10.Location = new System.Drawing.Point(584, 262);
            this.darkLabel10.Name = "darkLabel10";
            this.darkLabel10.Size = new System.Drawing.Size(39, 13);
            this.darkLabel10.TabIndex = 71;
            this.darkLabel10.Text = "X max:";
            // 
            // tbCollisionBoxMinZ
            // 
            this.tbCollisionBoxMinZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbCollisionBoxMinZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCollisionBoxMinZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbCollisionBoxMinZ.Location = new System.Drawing.Point(745, 231);
            this.tbCollisionBoxMinZ.Name = "tbCollisionBoxMinZ";
            this.tbCollisionBoxMinZ.Size = new System.Drawing.Size(72, 22);
            this.tbCollisionBoxMinZ.TabIndex = 70;
            // 
            // darkLabel11
            // 
            this.darkLabel11.AutoSize = true;
            this.darkLabel11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel11.Location = new System.Drawing.Point(742, 214);
            this.darkLabel11.Name = "darkLabel11";
            this.darkLabel11.Size = new System.Drawing.Size(38, 13);
            this.darkLabel11.TabIndex = 69;
            this.darkLabel11.Text = "Z min:";
            // 
            // tbCollisionBoxMinY
            // 
            this.tbCollisionBoxMinY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbCollisionBoxMinY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCollisionBoxMinY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbCollisionBoxMinY.Location = new System.Drawing.Point(666, 231);
            this.tbCollisionBoxMinY.Name = "tbCollisionBoxMinY";
            this.tbCollisionBoxMinY.Size = new System.Drawing.Size(73, 22);
            this.tbCollisionBoxMinY.TabIndex = 68;
            // 
            // darkLabel12
            // 
            this.darkLabel12.AutoSize = true;
            this.darkLabel12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel12.Location = new System.Drawing.Point(663, 214);
            this.darkLabel12.Name = "darkLabel12";
            this.darkLabel12.Size = new System.Drawing.Size(37, 13);
            this.darkLabel12.TabIndex = 67;
            this.darkLabel12.Text = "Y min:";
            // 
            // tbCollisionBoxMinX
            // 
            this.tbCollisionBoxMinX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.tbCollisionBoxMinX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCollisionBoxMinX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tbCollisionBoxMinX.Location = new System.Drawing.Point(587, 231);
            this.tbCollisionBoxMinX.Name = "tbCollisionBoxMinX";
            this.tbCollisionBoxMinX.Size = new System.Drawing.Size(73, 22);
            this.tbCollisionBoxMinX.TabIndex = 66;
            // 
            // darkLabel13
            // 
            this.darkLabel13.AutoSize = true;
            this.darkLabel13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel13.Location = new System.Drawing.Point(584, 214);
            this.darkLabel13.Name = "darkLabel13";
            this.darkLabel13.Size = new System.Drawing.Size(38, 13);
            this.darkLabel13.TabIndex = 65;
            this.darkLabel13.Text = "X min:";
            // 
            // cbCollisionBox
            // 
            this.cbCollisionBox.AutoSize = true;
            this.cbCollisionBox.Location = new System.Drawing.Point(587, 188);
            this.cbCollisionBox.Name = "cbCollisionBox";
            this.cbCollisionBox.Size = new System.Drawing.Size(93, 17);
            this.cbCollisionBox.TabIndex = 64;
            this.cbCollisionBox.Text = "Collision Box";
            this.cbCollisionBox.CheckedChanged += new System.EventHandler(this.cbCollisionBox_CheckedChanged);
            // 
            // cbDrawGrid
            // 
            this.cbDrawGrid.AutoSize = true;
            this.cbDrawGrid.Checked = true;
            this.cbDrawGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDrawGrid.Location = new System.Drawing.Point(587, 507);
            this.cbDrawGrid.Name = "cbDrawGrid";
            this.cbDrawGrid.Size = new System.Drawing.Size(77, 17);
            this.cbDrawGrid.TabIndex = 78;
            this.cbDrawGrid.Text = "Draw grid";
            this.cbDrawGrid.CheckedChanged += new System.EventHandler(this.cbDrawGrid_CheckedChanged);
            // 
            // cbDrawGizmo
            // 
            this.cbDrawGizmo.AutoSize = true;
            this.cbDrawGizmo.Checked = true;
            this.cbDrawGizmo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDrawGizmo.Location = new System.Drawing.Point(587, 484);
            this.cbDrawGizmo.Name = "cbDrawGizmo";
            this.cbDrawGizmo.Size = new System.Drawing.Size(87, 17);
            this.cbDrawGizmo.TabIndex = 79;
            this.cbDrawGizmo.Text = "Draw gizmo";
            this.cbDrawGizmo.CheckedChanged += new System.EventHandler(this.cbDrawGizmo_CheckedChanged);
            // 
            // FormStaticMeshEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 564);
            this.Controls.Add(this.cbDrawGizmo);
            this.Controls.Add(this.cbDrawGrid);
            this.Controls.Add(this.butCalculateCollisionBox);
            this.Controls.Add(this.tbCollisionBoxMaxZ);
            this.Controls.Add(this.darkLabel8);
            this.Controls.Add(this.tbCollisionBoxMaxY);
            this.Controls.Add(this.darkLabel9);
            this.Controls.Add(this.tbCollisionBoxMaxX);
            this.Controls.Add(this.darkLabel10);
            this.Controls.Add(this.tbCollisionBoxMinZ);
            this.Controls.Add(this.darkLabel11);
            this.Controls.Add(this.tbCollisionBoxMinY);
            this.Controls.Add(this.darkLabel12);
            this.Controls.Add(this.tbCollisionBoxMinX);
            this.Controls.Add(this.darkLabel13);
            this.Controls.Add(this.cbCollisionBox);
            this.Controls.Add(this.butCalculateVisibilityBox);
            this.Controls.Add(this.tbVisibilityBoxMaxZ);
            this.Controls.Add(this.darkLabel5);
            this.Controls.Add(this.tbVisibilityBoxMaxY);
            this.Controls.Add(this.darkLabel6);
            this.Controls.Add(this.tbVisibilityBoxMaxX);
            this.Controls.Add(this.darkLabel7);
            this.Controls.Add(this.tbVisibilityBoxMinZ);
            this.Controls.Add(this.darkLabel4);
            this.Controls.Add(this.tbVisibilityBoxMinY);
            this.Controls.Add(this.darkLabel3);
            this.Controls.Add(this.tbVisibilityBoxMinX);
            this.Controls.Add(this.darkLabel2);
            this.Controls.Add(this.cbVisibilityBox);
            this.Controls.Add(this.butSaveChanges);
            this.Controls.Add(this.panelRendering);
            this.Controls.Add(this.darkStatusStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormStaticMeshEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Static mesh editor";
            this.Load += new System.EventHandler(this.FormStaticMeshEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private Controls.PanelRenderingStaticMeshEditor panelRendering;
        private DarkUI.Controls.DarkButton butSaveChanges;
        private DarkUI.Controls.DarkCheckBox cbVisibilityBox;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox tbVisibilityBoxMinX;
        private DarkUI.Controls.DarkTextBox tbVisibilityBoxMinY;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkTextBox tbVisibilityBoxMinZ;
        private DarkUI.Controls.DarkLabel darkLabel4;
        private DarkUI.Controls.DarkTextBox tbVisibilityBoxMaxZ;
        private DarkUI.Controls.DarkLabel darkLabel5;
        private DarkUI.Controls.DarkTextBox tbVisibilityBoxMaxY;
        private DarkUI.Controls.DarkLabel darkLabel6;
        private DarkUI.Controls.DarkTextBox tbVisibilityBoxMaxX;
        private DarkUI.Controls.DarkLabel darkLabel7;
        private DarkUI.Controls.DarkButton butCalculateVisibilityBox;
        private DarkUI.Controls.DarkButton butCalculateCollisionBox;
        private DarkUI.Controls.DarkTextBox tbCollisionBoxMaxZ;
        private DarkUI.Controls.DarkLabel darkLabel8;
        private DarkUI.Controls.DarkTextBox tbCollisionBoxMaxY;
        private DarkUI.Controls.DarkLabel darkLabel9;
        private DarkUI.Controls.DarkTextBox tbCollisionBoxMaxX;
        private DarkUI.Controls.DarkLabel darkLabel10;
        private DarkUI.Controls.DarkTextBox tbCollisionBoxMinZ;
        private DarkUI.Controls.DarkLabel darkLabel11;
        private DarkUI.Controls.DarkTextBox tbCollisionBoxMinY;
        private DarkUI.Controls.DarkLabel darkLabel12;
        private DarkUI.Controls.DarkTextBox tbCollisionBoxMinX;
        private DarkUI.Controls.DarkLabel darkLabel13;
        private DarkUI.Controls.DarkCheckBox cbCollisionBox;
        private DarkUI.Controls.DarkCheckBox cbDrawGrid;
        private DarkUI.Controls.DarkCheckBox cbDrawGizmo;
    }
}