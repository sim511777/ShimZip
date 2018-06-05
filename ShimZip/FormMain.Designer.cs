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
         this.openZipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.dlgSave = new System.Windows.Forms.SaveFileDialog();
         this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
         this.treeView1 = new System.Windows.Forms.TreeView();
         this.splitter1 = new System.Windows.Forms.Splitter();
         this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
         this.listView1 = new System.Windows.Forms.ListView();
         this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.menuStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zipToolStripMenuItem,
            this.openZipToolStripMenuItem,
            this.extractToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(886, 24);
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
         // openZipToolStripMenuItem
         // 
         this.openZipToolStripMenuItem.Name = "openZipToolStripMenuItem";
         this.openZipToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
         this.openZipToolStripMenuItem.Text = "Open Zip";
         // 
         // extractToolStripMenuItem
         // 
         this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
         this.extractToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
         this.extractToolStripMenuItem.Text = "Extract";
         // 
         // dlgSave
         // 
         this.dlgSave.Filter = "Shim Zip FIile|*.szip";
         // 
         // dlgOpen
         // 
         this.dlgOpen.FileName = "openFileDialog1";
         // 
         // treeView1
         // 
         this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
         this.treeView1.Location = new System.Drawing.Point(0, 24);
         this.treeView1.Name = "treeView1";
         this.treeView1.Size = new System.Drawing.Size(233, 527);
         this.treeView1.TabIndex = 1;
         // 
         // splitter1
         // 
         this.splitter1.Location = new System.Drawing.Point(233, 24);
         this.splitter1.Name = "splitter1";
         this.splitter1.Size = new System.Drawing.Size(3, 527);
         this.splitter1.TabIndex = 2;
         this.splitter1.TabStop = false;
         // 
         // listView1
         // 
         this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
         this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listView1.Location = new System.Drawing.Point(236, 24);
         this.listView1.Name = "listView1";
         this.listView1.Size = new System.Drawing.Size(650, 527);
         this.listView1.TabIndex = 3;
         this.listView1.UseCompatibleStateImageBehavior = false;
         this.listView1.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "이름";
         this.columnHeader1.Width = 200;
         // 
         // columnHeader2
         // 
         this.columnHeader2.Text = "유형";
         // 
         // columnHeader3
         // 
         this.columnHeader3.Text = "크기";
         this.columnHeader3.Width = 100;
         // 
         // FormMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(886, 551);
         this.Controls.Add(this.listView1);
         this.Controls.Add(this.splitter1);
         this.Controls.Add(this.treeView1);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "FormMain";
         this.Text = "Form1";
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem zipToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openZipToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
      private System.Windows.Forms.FolderBrowserDialog dlgFolder;
      private System.Windows.Forms.SaveFileDialog dlgSave;
      private System.Windows.Forms.OpenFileDialog dlgOpen;
      private System.Windows.Forms.TreeView treeView1;
      private System.Windows.Forms.Splitter splitter1;
      private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
      private System.Windows.Forms.ListView listView1;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.ColumnHeader columnHeader3;
   }
}

