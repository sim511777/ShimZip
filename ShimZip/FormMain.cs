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

      private BinaryReader reader;

      private void DirDataToTree(ZipData dirData, TreeNodeCollection nodes) {
         foreach (var subDir in dirData.dirDatas) {
            var node = nodes.Add(subDir.name);
            node.Tag = subDir;
            this.DirDataToTree(subDir, node.Nodes);
         }
      }

      private void ZipToTree(string zipFileName, ZipData zipData) {
         this.trvZip.Nodes.Clear();
         var node = this.trvZip.Nodes.Add(zipFileName);
         node.Tag = zipData;
         this.DirDataToTree(zipData, node.Nodes);

         this.trvZip.SelectedNode = node;
         node.Expand();
      }

      // == event handler
      private void zipToolStripMenuItem_Click(object sender, EventArgs e) {
         this.dlgFolder.SelectedPath = @"E:\joy\Game\Quake";
         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
                  
         string dir = this.dlgFolder.SelectedPath;
         string zipFilePath = dir + ".szip";

         var files = Directory.GetFiles(dir);
         var dirs = Directory.GetDirectories(dir);
         ShimZip.ZipFile(files, dirs, zipFilePath);
         
         MessageBox.Show(zipFilePath + " ziped");
      }

      private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
         this.dlgOpen.FileName = @"E:\joy\Game\Quake.sip";
         if (this.dlgOpen.ShowDialog() != DialogResult.OK)
            return;
         
         string zipFilePath = this.dlgOpen.FileName;
         this.reader = new BinaryReader(File.OpenRead(zipFilePath));
         var zipData = ShimZip.GetZipData(this.reader);

         string zipFileName = Path.GetFileName(zipFilePath);
         this.ZipToTree(zipFileName, zipData);
      }

      private void trvZip_AfterSelect(object sender, TreeViewEventArgs e) {
         this.lvwFiles.Items.Clear();
         ZipData zipData = e.Node.Tag as ZipData;
         foreach (var dirData in zipData.dirDatas) {
            string[] subItems = new string[] { dirData.name, "[DIR]", string.Empty };
            var item = this.lvwFiles.Items.Add(new ListViewItem(subItems));
            item.Tag = dirData;
         }
         foreach (var fileData in zipData.fileDatas) {
            var ext = Path.GetExtension(fileData.name);
            string[] subItems = new string[] { fileData.name, ext, fileData.length.ToString() };
            var item = this.lvwFiles.Items.Add(new ListViewItem(subItems));
            item.Tag = fileData;
         }
      }
   }
}
