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
using System.Globalization;

namespace ShimZip {
   public partial class FormMain : Form {
      public FormMain() {
         InitializeComponent();
      }

      private BinaryReader br;

      // zipData -> tree
      private void ZipToTree(string zipFileName, ZipData zipData) {
         this.trvZip.Nodes.Clear();
         var node = this.trvZip.Nodes.Add(zipFileName);
         node.Tag = zipData;
         this.DirDataToTree(zipData, node.Nodes);

         this.trvZip.SelectedNode = node;
         node.Expand();
      }

      // dirData -> tree recursive
      private void DirDataToTree(ZipData dirData, TreeNodeCollection nodes) {
         foreach (var subDir in dirData.dirDatas) {
            var node = nodes.Add(subDir.name);
            node.Tag = subDir;
            this.DirDataToTree(subDir, node.Nodes);
         }
      }

      // == event handler
      private void zipToolStripMenuItem_Click(object sender, EventArgs e) {
         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
         string dir = this.dlgFolder.SelectedPath;

         this.dlgSave.InitialDirectory = Path.GetDirectoryName(dir);
         this.dlgSave.FileName = Path.GetFileName(dir) + ".szip";
         if (this.dlgSave.ShowDialog(this) != DialogResult.OK)
            return;
         string zipFilePath = this.dlgSave.FileName;

         var files = Directory.GetFiles(dir);
         var dirs = Directory.GetDirectories(dir);

         var t1 = DateTime.Now;
         int fileCnt = ShimZip.ZipFile(files, dirs, zipFilePath);
         var dt = DateTime.Now - t1;
         
         MessageBox.Show($"{fileCnt} files ziped to {zipFilePath}.\r\nelapse time : {dt.TotalSeconds:0.0}s");
      }

      private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
         if (this.dlgOpen.ShowDialog() != DialogResult.OK)
            return;
         
         string zipFilePath = this.dlgOpen.FileName;
         this.br = new BinaryReader(File.OpenRead(zipFilePath));
         var zipData = ShimZip.GetZipData(this.br);

         string zipFileName = Path.GetFileName(zipFilePath);
         this.ZipToTree(zipFileName, zipData);
      }

      private void trvZip_AfterSelect(object sender, TreeViewEventArgs e) {
         CultureInfo culture = CultureInfo.CreateSpecificCulture("ko-KR");
         string format = "g";
         this.lvwFiles.BeginUpdate();
         this.lvwFiles.Items.Clear();
         ZipData zipData = e.Node.Tag as ZipData;
         foreach (var dirData in zipData.dirDatas) {
            string[] subItems = new string[] { dirData.name, "[DIR]", string.Empty, dirData.creationTimeUtc.ToLocalTime().ToString(format, culture), dirData.lastAccessTimeUtc.ToLocalTime().ToString(format, culture), dirData.lastWriteTimeUtc.ToLocalTime().ToString(format, culture) };
            var item = this.lvwFiles.Items.Add(new ListViewItem(subItems));
            item.Tag = dirData;
            item.Tag = dirData;
         }
         foreach (var fileData in zipData.fileDatas) {
            var ext = Path.GetExtension(fileData.name);
            string[] subItems = new string[] { fileData.name, ext, fileData.length.ToString(), fileData.creationTimeUtc.ToLocalTime().ToString(format, culture), fileData.lastAccessTimeUtc.ToLocalTime().ToString(format, culture), fileData.lastWriteTimeUtc.ToLocalTime().ToString(format, culture) };
            var item = this.lvwFiles.Items.Add(new ListViewItem(subItems));
            item.Tag = fileData;
         }
         this.lvwFiles.EndUpdate();
      }

      private void extractAllToolStripMenuItem_Click(object sender, EventArgs e) {
         if (this.br == null || this.trvZip.Nodes.Count == 0)
            return;

         var zipData = this.trvZip.Nodes[0].Tag as ZipData;
         if (zipData == null)
            return;

         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
         string unzipDir = this.dlgFolder.SelectedPath;

         var t1 = DateTime.Now;
         int fileCnt = ShimZip.UnzipFile(zipData.fileDatas, zipData.dirDatas, this.br, unzipDir);
         var dt = DateTime.Now - t1;

         MessageBox.Show($"{fileCnt} files unziped to {unzipDir}.\r\nelapse time : {dt.TotalSeconds:0.0}s");
      }

      private void exractSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
         var selItems = this.lvwFiles.SelectedItems.Cast<ListViewItem>();
         if (selItems.Count() == 0)
            return;

         var fileDatas = selItems.Where(item => item.Tag is FileData).Select(item => item.Tag as FileData).ToList();
         var dirDatas = selItems.Where(item => item.Tag is DirData).Select(item => item.Tag as DirData).ToList();

         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;

         string unzipDir = this.dlgFolder.SelectedPath;

         int fileCnt = ShimZip.UnzipFile(fileDatas, dirDatas, this.br, unzipDir);
         var t1 = DateTime.Now;
         var dt = DateTime.Now - t1;

         MessageBox.Show($"{fileCnt} files unziped to {unzipDir}.\r\nelapse time : {dt.TotalSeconds:0.0}s");
      }
   }
}
