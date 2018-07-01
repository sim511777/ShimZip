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

      public FormMain(string zipFilePath) {
         InitializeComponent();
         this.OpenZipFile(zipFilePath);
      }

      private string zipFilePath;

      // dirData -> tree
      private void ZipToTree(ZipData zipData, string rootName) {
         this.trvZip.Nodes.Clear();
         var node = this.trvZip.Nodes.Add(rootName);
         node.Tag = zipData.dirData;
         this.DirDataToTree(zipData.dirData, node.Nodes);

         this.trvZip.SelectedNode = node;
         node.Expand();
      }

      // dirData -> tree recursive
      private void DirDataToTree(DirData dirData, TreeNodeCollection nodes) {
         foreach (var subDir in dirData.dirDatas) {
            var node = nodes.Add(subDir.name);
            node.Tag = subDir;
            this.DirDataToTree(subDir, node.Nodes);
         }
      }

      private void OpenZipFile(string zipFilePath) {
         this.zipFilePath = zipFilePath;
         int fileCnt = 0;
         long totalByte = 0;
         
         string ext = Path.GetExtension(zipFilePath).ToLower();
         ZipData zipData = null;
         if (ext == ".sip")
            zipData = ShimZip.GetZipData(this.zipFilePath, ref fileCnt, ref totalByte);
         else if (ext == ".pak")
            zipData = QuakePak.GetZipData(this.zipFilePath, ref fileCnt, ref totalByte);

         string zipFileName = Path.GetFileName(zipFilePath);
         this.ZipToTree(zipData, Path.GetFileName(zipFilePath));
      }

      // == event handler
      private void zipToolStripMenuItem_Click(object sender, EventArgs e) {
         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
         string dir = this.dlgFolder.SelectedPath;

         this.dlgSave.InitialDirectory = Path.GetDirectoryName(dir);
         this.dlgSave.FileName = Path.GetFileName(dir) + ".sip";
         if (this.dlgSave.ShowDialog(this) != DialogResult.OK)
            return;
         string zipFilePath = this.dlgSave.FileName;

         var files = Directory.GetFiles(dir);
         var dirs = Directory.GetDirectories(dir);

         var t1 = DateTime.Now;
         int fileCnt = 0;
         long totalByte = 0;
         var zipData = ZipData.MakeZipData(files, dirs, ref fileCnt, ref totalByte);
         
         int encFileCnt = 0;
         if (Path.GetExtension(zipFilePath).ToLower() == ".sip") {
            encFileCnt = ShimZip.ZipFile(zipData, zipFilePath);
         } else {
         }
         var dt = DateTime.Now - t1;         
         MessageBox.Show($"{encFileCnt} files ziped to {zipFilePath}.\r\nelapse time : {dt.TotalSeconds:0.0}s");
      }

      private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
         if (this.dlgOpen.ShowDialog() != DialogResult.OK)
            return;
         
         string zipFilePath = this.dlgOpen.FileName;
         this.OpenZipFile(zipFilePath);
      }

      private void trvZip_AfterSelect(object sender, TreeViewEventArgs e) {
         CultureInfo culture = CultureInfo.CreateSpecificCulture("ko-KR");
         string format = "g";
         this.lvwFiles.BeginUpdate();
         this.lvwFiles.Items.Clear();
         DirData dirData = e.Node.Tag as DirData;
         foreach (var subDirData in dirData.dirDatas) {
            string[] subItems = new string[] { subDirData.name, "[DIR]", string.Empty, subDirData.creationTimeUtc.ToLocalTime().ToString(format, culture), subDirData.lastAccessTimeUtc.ToLocalTime().ToString(format, culture), subDirData.lastWriteTimeUtc.ToLocalTime().ToString(format, culture) };
            var item = this.lvwFiles.Items.Add(new ListViewItem(subItems));
            item.Tag = subDirData;
         }
         foreach (var fileData in dirData.fileDatas) {
            var ext = Path.GetExtension(fileData.name);
            string[] subItems = new string[] { fileData.name, ext, fileData.length.ToString(), fileData.creationTimeUtc.ToLocalTime().ToString(format, culture), fileData.lastAccessTimeUtc.ToLocalTime().ToString(format, culture), fileData.lastWriteTimeUtc.ToLocalTime().ToString(format, culture) };
            var item = this.lvwFiles.Items.Add(new ListViewItem(subItems));
            item.Tag = fileData;
         }
         this.lvwFiles.EndUpdate();
      }

      private void extractAllToolStripMenuItem_Click(object sender, EventArgs e) {
         if (this.zipFilePath == null || this.trvZip.Nodes.Count == 0)
            return;

         var dirData = this.trvZip.Nodes[0].Tag as DirData;
         if (dirData == null)
            return;

         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
         string unzipDir = this.dlgFolder.SelectedPath;

         var t1 = DateTime.Now;
         int decfileCnt = ZipData.UnzipFile(dirData.fileDatas, dirData.dirDatas, this.zipFilePath, unzipDir);
         var dt = DateTime.Now - t1;

         MessageBox.Show($"{decfileCnt} files unziped to {unzipDir}.\r\nelapse time : {dt.TotalSeconds:0.0}s");
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

         int decfileCnt = ZipData.UnzipFile(fileDatas, dirDatas, this.zipFilePath, unzipDir);
         var t1 = DateTime.Now;
         var dt = DateTime.Now - t1;

         MessageBox.Show($"{decfileCnt} files unziped to {unzipDir}.\r\nelapse time : {dt.TotalSeconds:0.0}s");
      }
   }
}
