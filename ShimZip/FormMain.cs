using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ShimZip {
   public partial class FormMain : Form {
      public FormMain() {
         InitializeComponent();
      }

      private void DirDataToTree(DirData dirData, TreeNodeCollection nodes) {
         var node = nodes.Add(dirData.name);
         node.Tag = dirData;
         foreach (var subDir in dirData.dirDatas) {
            this.DirDataToTree(subDir, node.Nodes);
         }
      }

      private void ZipToTree(DirData dirData) {
         this.trvZip.Nodes.Clear();
         this.DirDataToTree(dirData, this.trvZip.Nodes);
         this.trvZip.ExpandAll();
      }

      private void zipToolStripMenuItem_Click(object sender, EventArgs e) {
         this.dlgFolder.SelectedPath = @"E:\joy\Game\Quake";
         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
                  
         string dir = this.dlgFolder.SelectedPath;
         string zipFilePath = dir + "_.sip";

         DirData dirData = ShimZip.GetDirData(dir);
         this.ZipToTree(dirData);

         var files = Directory.GetFiles(dir);
         var dirs = Directory.GetDirectories(dir);
         ShimZip.ZipFile(files, dirs, zipFilePath);
         MessageBox.Show(zipFilePath + " ziped");
      }

      private void trvZip_AfterSelect(object sender, TreeViewEventArgs e) {
         this.lvwFiles.Items.Clear();
         DirData dirData = e.Node.Tag as DirData;
         foreach (var dir in dirData.dirDatas) {
            string[] subItems = new string[] { dir.name, "Dir", string.Empty };
            this.lvwFiles.Items.Add(new ListViewItem(subItems));
         }
         foreach (var file in dirData.fileDatas) {
            string[] subItems = new string[] { file.name, "File", string.Empty };
            this.lvwFiles.Items.Add(new ListViewItem(subItems));
         }
      }

      private void UnzipToolStripMenuItem_Click(object sender, EventArgs e) {
         this.dlgOpen.FileName = @"E:\joy\Game\Quake_.sip";
         if (this.dlgOpen.ShowDialog() != DialogResult.OK)
            return;
         string zipFilePath = this.dlgOpen.FileName;
         string unzipDir = Path.GetDirectoryName(zipFilePath) + "\\" + Path.GetFileNameWithoutExtension(zipFilePath);
         ShimZip.UnzipFile(this.dlgOpen.FileName, unzipDir);
         MessageBox.Show(zipFilePath + " unziped");
      }
   }
}
