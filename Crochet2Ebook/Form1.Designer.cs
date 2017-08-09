namespace Crochet2Ebook
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "42       ",
            "#FFFFFF"}, 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("x 23", 0);
            this.listView_Palette = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList_Palette = new System.Windows.Forms.ImageList(this.components);
            this.button_LoadImage = new System.Windows.Forms.Button();
            this.contextMenu_Palette = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dieseFarbeUmbenennenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog_Picture = new System.Windows.Forms.OpenFileDialog();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.button_Zoom = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.listView_LineDescription = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_ToggleOptions = new System.Windows.Forms.Button();
            this.checkBox_Ratiocorrection = new System.Windows.Forms.CheckBox();
            this.label_Palette = new System.Windows.Forms.Label();
            this.button_createPDF = new System.Windows.Forms.Button();
            this.label_Titel = new System.Windows.Forms.Label();
            this.textBox_Titel = new System.Windows.Forms.TextBox();
            this.pictureBox_Display = new System.Windows.Forms.PictureBox();
            this.contextMenu_Palette.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Display)).BeginInit();
            this.SuspendLayout();
            // 
            // listView_Palette
            // 
            this.listView_Palette.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_Palette.LabelEdit = true;
            this.listView_Palette.LargeImageList = this.imageList_Palette;
            this.listView_Palette.Location = new System.Drawing.Point(0, 69);
            this.listView_Palette.Name = "listView_Palette";
            this.listView_Palette.Size = new System.Drawing.Size(129, 148);
            this.listView_Palette.SmallImageList = this.imageList_Palette;
            this.listView_Palette.TabIndex = 1;
            this.listView_Palette.UseCompatibleStateImageBehavior = false;
            this.listView_Palette.View = System.Windows.Forms.View.SmallIcon;
            this.listView_Palette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView_Palette_MouseDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Farbcode";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Farbcode";
            // 
            // imageList_Palette
            // 
            this.imageList_Palette.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Palette.ImageStream")));
            this.imageList_Palette.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Palette.Images.SetKeyName(0, "Uschi_Hellblau.png");
            // 
            // button_LoadImage
            // 
            this.button_LoadImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_LoadImage.Location = new System.Drawing.Point(0, 0);
            this.button_LoadImage.Name = "button_LoadImage";
            this.button_LoadImage.Size = new System.Drawing.Size(95, 36);
            this.button_LoadImage.TabIndex = 8;
            this.button_LoadImage.Text = "Bild laden";
            this.button_LoadImage.UseVisualStyleBackColor = true;
            this.button_LoadImage.Click += new System.EventHandler(this.button_LoadImage_Click);
            // 
            // contextMenu_Palette
            // 
            this.contextMenu_Palette.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dieseFarbeUmbenennenToolStripMenuItem});
            this.contextMenu_Palette.Name = "contextMenuStrip1";
            this.contextMenu_Palette.Size = new System.Drawing.Size(208, 26);
            // 
            // dieseFarbeUmbenennenToolStripMenuItem
            // 
            this.dieseFarbeUmbenennenToolStripMenuItem.Name = "dieseFarbeUmbenennenToolStripMenuItem";
            this.dieseFarbeUmbenennenToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.dieseFarbeUmbenennenToolStripMenuItem.Text = "diese Farbe umbenennen";
            this.dieseFarbeUmbenennenToolStripMenuItem.Click += new System.EventHandler(this.dieseFarbeUmbenennenToolStripMenuItem_Click);
            // 
            // openFileDialog_Picture
            // 
            this.openFileDialog_Picture.FileName = "openFileDialog1";
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 890);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1297, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 13;
            this.progressBar1.UseWaitCursor = true;
            this.progressBar1.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox_Display);
            this.splitContainer1.Panel2.Controls.Add(this.progressBar1);
            this.splitContainer1.Size = new System.Drawing.Size(1430, 915);
            this.splitContainer1.SplitterDistance = 129;
            this.splitContainer1.TabIndex = 17;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer2.Size = new System.Drawing.Size(129, 915);
            this.splitContainer2.SplitterDistance = 80;
            this.splitContainer2.TabIndex = 20;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.button_Zoom);
            this.splitContainer3.Panel1.Controls.Add(this.button_LoadImage);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.numericUpDown1);
            this.splitContainer3.Panel2.Controls.Add(this.textBox1);
            this.splitContainer3.Size = new System.Drawing.Size(129, 80);
            this.splitContainer3.SplitterDistance = 36;
            this.splitContainer3.TabIndex = 20;
            // 
            // button_Zoom
            // 
            this.button_Zoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Zoom.Enabled = false;
            this.button_Zoom.Image = ((System.Drawing.Image)(resources.GetObject("button_Zoom.Image")));
            this.button_Zoom.Location = new System.Drawing.Point(95, 0);
            this.button_Zoom.Name = "button_Zoom";
            this.button_Zoom.Size = new System.Drawing.Size(34, 36);
            this.button_Zoom.TabIndex = 16;
            this.button_Zoom.UseVisualStyleBackColor = true;
            this.button_Zoom.Click += new System.EventHandler(this.button_Zoom_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(0, 0);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(129, 40);
            this.numericUpDown1.TabIndex = 16;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 288);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(112, 98);
            this.textBox1.TabIndex = 15;
            this.textBox1.Visible = false;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.splitContainer5);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.checkBox_Ratiocorrection);
            this.splitContainer4.Panel2.Controls.Add(this.label_Palette);
            this.splitContainer4.Panel2.Controls.Add(this.button_createPDF);
            this.splitContainer4.Panel2.Controls.Add(this.label_Titel);
            this.splitContainer4.Panel2.Controls.Add(this.listView_Palette);
            this.splitContainer4.Panel2.Controls.Add(this.textBox_Titel);
            this.splitContainer4.Size = new System.Drawing.Size(129, 831);
            this.splitContainer4.SplitterDistance = 540;
            this.splitContainer4.TabIndex = 15;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.listView_LineDescription);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.button_ToggleOptions);
            this.splitContainer5.Size = new System.Drawing.Size(129, 540);
            this.splitContainer5.SplitterDistance = 511;
            this.splitContainer5.TabIndex = 14;
            // 
            // listView_LineDescription
            // 
            this.listView_LineDescription.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listView_LineDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_LineDescription.Enabled = false;
            this.listView_LineDescription.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            listViewItem1.StateImageIndex = 0;
            listViewItem1.UseItemStyleForSubItems = false;
            this.listView_LineDescription.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.listView_LineDescription.LabelEdit = true;
            this.listView_LineDescription.LargeImageList = this.imageList_Palette;
            this.listView_LineDescription.Location = new System.Drawing.Point(0, 0);
            this.listView_LineDescription.MultiSelect = false;
            this.listView_LineDescription.Name = "listView_LineDescription";
            this.listView_LineDescription.Size = new System.Drawing.Size(129, 511);
            this.listView_LineDescription.SmallImageList = this.imageList_Palette;
            this.listView_LineDescription.TabIndex = 1;
            this.listView_LineDescription.UseCompatibleStateImageBehavior = false;
            this.listView_LineDescription.View = System.Windows.Forms.View.SmallIcon;
            this.listView_LineDescription.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_LineDescription_ItemSelectionChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Farbcode";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Farbcode";
            // 
            // button_ToggleOptions
            // 
            this.button_ToggleOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_ToggleOptions.Location = new System.Drawing.Point(0, 0);
            this.button_ToggleOptions.Name = "button_ToggleOptions";
            this.button_ToggleOptions.Size = new System.Drawing.Size(129, 25);
            this.button_ToggleOptions.TabIndex = 14;
            this.button_ToggleOptions.Text = "mehr Optionen";
            this.button_ToggleOptions.UseVisualStyleBackColor = true;
            this.button_ToggleOptions.Click += new System.EventHandler(this.button_ToggleOptions_Click);
            // 
            // checkBox_Ratiocorrection
            // 
            this.checkBox_Ratiocorrection.AutoSize = true;
            this.checkBox_Ratiocorrection.Checked = true;
            this.checkBox_Ratiocorrection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Ratiocorrection.Location = new System.Drawing.Point(12, 232);
            this.checkBox_Ratiocorrection.Name = "checkBox_Ratiocorrection";
            this.checkBox_Ratiocorrection.Size = new System.Drawing.Size(96, 17);
            this.checkBox_Ratiocorrection.TabIndex = 14;
            this.checkBox_Ratiocorrection.Text = "Ratio korrektur";
            this.checkBox_Ratiocorrection.UseVisualStyleBackColor = true;
            this.checkBox_Ratiocorrection.CheckedChanged += new System.EventHandler(this.checkBox_Ratiocorrection_CheckedChanged);
            // 
            // label_Palette
            // 
            this.label_Palette.AutoSize = true;
            this.label_Palette.Location = new System.Drawing.Point(-3, 53);
            this.label_Palette.Name = "label_Palette";
            this.label_Palette.Size = new System.Drawing.Size(63, 13);
            this.label_Palette.TabIndex = 14;
            this.label_Palette.Text = "Farbpalette:";
            // 
            // button_createPDF
            // 
            this.button_createPDF.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button_createPDF.Enabled = false;
            this.button_createPDF.Location = new System.Drawing.Point(0, 258);
            this.button_createPDF.Name = "button_createPDF";
            this.button_createPDF.Size = new System.Drawing.Size(129, 29);
            this.button_createPDF.TabIndex = 14;
            this.button_createPDF.Text = "PDF erstellen";
            this.button_createPDF.UseVisualStyleBackColor = true;
            this.button_createPDF.Click += new System.EventHandler(this.button_createPDF_Click);
            // 
            // label_Titel
            // 
            this.label_Titel.AutoSize = true;
            this.label_Titel.Location = new System.Drawing.Point(-3, 9);
            this.label_Titel.Name = "label_Titel";
            this.label_Titel.Size = new System.Drawing.Size(86, 13);
            this.label_Titel.TabIndex = 14;
            this.label_Titel.Text = "Titel für die PDF:";
            // 
            // textBox_Titel
            // 
            this.textBox_Titel.Location = new System.Drawing.Point(0, 25);
            this.textBox_Titel.MaxLength = 50;
            this.textBox_Titel.Name = "textBox_Titel";
            this.textBox_Titel.Size = new System.Drawing.Size(129, 20);
            this.textBox_Titel.TabIndex = 14;
            this.textBox_Titel.WordWrap = false;
            this.textBox_Titel.TextChanged += new System.EventHandler(this.textBox_Titel_TextChanged);
            // 
            // pictureBox_Display
            // 
            this.pictureBox_Display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_Display.Location = new System.Drawing.Point(59, 40);
            this.pictureBox_Display.Name = "pictureBox_Display";
            this.pictureBox_Display.Size = new System.Drawing.Size(168, 110);
            this.pictureBox_Display.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_Display.TabIndex = 11;
            this.pictureBox_Display.TabStop = false;
            this.pictureBox_Display.Visible = false;
            this.pictureBox_Display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Display_MouseClick);
            this.pictureBox_Display.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_Display_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1430, 915);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PixelCounter 2.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_SizeChanged);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.Resize += new System.EventHandler(this.Form1_SizeChanged);
            this.StyleChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.contextMenu_Palette.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Display)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView_Palette;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button button_LoadImage;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Palette;
        private System.Windows.Forms.ToolStripMenuItem dieseFarbeUmbenennenToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList_Palette;
        private System.Windows.Forms.OpenFileDialog openFileDialog_Picture;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox_Display;
        private System.Windows.Forms.Button button_Zoom;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListView listView_LineDescription;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button button_createPDF;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.Button button_ToggleOptions;
        private System.Windows.Forms.CheckBox checkBox_Ratiocorrection;
        private System.Windows.Forms.Label label_Palette;
        private System.Windows.Forms.Label label_Titel;
        private System.Windows.Forms.TextBox textBox_Titel;
    }
}

