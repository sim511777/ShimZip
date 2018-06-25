namespace ShimZip {
   partial class FormMain {
      /// <summary>
      /// 필수 디자이너 변수입니다.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// 사용 중인 모든 리소스를 정리합니다.
      /// </summary>
      /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form 디자이너에서 생성한 코드

      /// <summary>
      /// 디자이너 지원에 필요한 메서드입니다. 
      /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
      /// </summary>
      private void InitializeComponent() {
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.zipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.exractSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.dlgSave = new System.Windows.Forms.SaveFileDialog();
         this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
         this.trvZip = new System.Windows.Forms.TreeView();
         this.splitter1 = new System.Windows.Forms.Splitter();
         this.lvwFiles = new System.Windows.Forms.ListView();
         this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.statusStrip1 = new System.Windows.Forms.StatusStrip();
         this.menuStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zipToolStripMenuItem,
            this.openToolStripMenuItem,
            this.extractAllToolStripMenuItem,
            this.exractSelectedToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(1205, 24);
         this.menuStrip1.TabIndex = 0;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // zipToolStripMenuItem
         // 
         this.zipToolStripMenuItem.Name = "zipToolStripMenuItem";
         this.zipToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
         this.zipToolStripMenuItem.Text = "Zip";
         this.zipToolStripMenuItem.Click += new System.EventHandler(this.zipToolStripMenuItem_Click);
         // 
         // openToolStripMenuItem
         // 
         this.openToolStripMenuItem.Name = "openToolStripMenuItem";
         this.openToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
         this.openToolStripMenuItem.Text = "Open Zip";
         this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
         // 
         // extractAllToolStripMenuItem
         // 
         this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
         this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
         this.extractAllToolStripMenuItem.Text = "Extract All";
         this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.extractAllToolStripMenuItem_Click);
         // 
         // exractSelectedToolStripMenuItem
         // 
         this.exractSelectedToolStripMenuItem.Name = "exractSelectedToolStripMenuItem";
         this.exractSelectedToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
         this.exractSelectedToolStripMenuItem.Text = "Exract Selected";
         this.exractSelectedToolStripMenuItem.Click += new System.EventHandler(this.exractSelectedToolStripMenuItem_Click);
         // 
         // dlgSave
         // 
         this.dlgSave.DefaultExt = "szip";
         this.dlgSave.Filter = "Shim Zip FIile|*.szip";
         // 
         // dlgOpen
         // 
         this.dlgOpen.DefaultExt = "szip";
         this.dlgOpen.FileName = "openFileDialog1";
         this.dlgOpen.Filter = "Shim Zip FIile|*.szip";
         // 
         // trvZip
         // 
         this.trvZip.Dock = System.Windows.Forms.DockStyle.Left;
         this.trvZip.HideSelection = false;
         this.trvZip.Location = new System.Drawing.Point(0, 24);
         this.trvZip.Name = "trvZip";
         this.trvZip.Size = new System.Drawing.Size(233, 533);
         this.trvZip.TabIndex = 1;
         this.trvZip.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvZip_AfterSelect);
         // 
         // splitter1
         // 
         this.splitter1.Location = new System.Drawing.Point(233, 24);
         this.splitter1.Name = "splitter1";
         this.splitter1.Size = new System.Drawing.Size(3, 533);
         this.splitter1.TabIndex = 2;
         this.splitter1.TabStop = false;
         // 
         // lvwFiles
         // 
         this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
         this.lvwFiles.Dock = System.Windows.Forms.DockStyle.Fill;
         this.lvwFiles.FullRowSelect = true;
         this.lvwFiles.GridLines = true;
         this.lvwFiles.HideSelection = false;
         this.lvwFiles.Location = new System.Drawing.Point(236, 24);
         this.lvwFiles.Name = "lvwFiles";
         this.lvwFiles.Size = new System.Drawing.Size(969, 533);
         this.lvwFiles.TabIndex = 3;
         this.lvwFiles.UseCompatibleStateImageBehavior = false;
         this.lvwFiles.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "name";
         this.columnHeader1.Width = 237;
         // 
         // columnHeader2
         // 
         this.columnHeader2.Text = "type";
         this.columnHeader2.Width = 107;
         // 
         // columnHeader3
         // 
         this.columnHeader3.Text = "size";
         this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.columnHeader3.Width = 77;
         // 
         // columnHeader5
         // 
         this.columnHeader5.Text = "create time";
         this.columnHeader5.Width = 140;
         // 
         // columnHeader6
         // 
         this.columnHeader6.Text = "last access time";
         this.columnHeader6.Width = 140;
         // 
         // columnHeader7
         // 
         this.columnHeader7.Text = "last write time";
         this.columnHeader7.Width = 140;
         // 
         // statusStrip1
         // 
         this.statusStrip1.Location = new System.Drawing.Point(0, 557);
         this.statusStrip1.Name = "statusStrip1";
         this.statusStrip1.Size = new System.Drawing.Size(1205, 22);
         this.statusStrip1.TabIndex = 4;
         this.statusStrip1.Text = "statusStrip1";
         // 
         // FormMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1205, 579);
         this.Controls.Add(this.lvwFiles);
         this.Controls.Add(this.splitter1);
         this.Controls.Add(this.trvZip);
         this.Controls.Add(this.menuStrip1);
         this.Controls.Add(this.statusStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "FormMain";
         this.Text = "ShimZip";
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem zipToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
      private System.Windows.Forms.FolderBrowserDialog dlgFolder;
      private System.Windows.Forms.SaveFileDialog dlgSave;
      private System.Windows.Forms.OpenFileDialog dlgOpen;
      private System.Windows.Forms.TreeView trvZip;
      private System.Windows.Forms.Splitter splitter1;
      private System.Windows.Forms.ListView lvwFiles;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.ColumnHeader columnHeader3;
      private System.Windows.Forms.StatusStrip statusStrip1;
      private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem exractSelectedToolStripMenuItem;
      private System.Windows.Forms.ColumnHeader columnHeader5;
      private System.Windows.Forms.ColumnHeader columnHeader6;
      private System.Windows.Forms.ColumnHeader columnHeader7;
   }
}

