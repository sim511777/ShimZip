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

      private void zipToolStripMenuItem_Click(object sender, EventArgs e) {
         this.dlgFolder.SelectedPath = @"E:\joy\Game\Quake";
         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
                  
         string dir = this.dlgFolder.SelectedPath;
         string zipFilePath = dir + "_.sip";

         var files = Directory.GetFiles(dir);
         var dirs = Directory.GetDirectories(dir);
         ShimZip.ZipFile(files, dirs, zipFilePath);
         MessageBox.Show(zipFilePath + " ziped");
      }

      private void trvZip_AfterSelect(object sender, TreeViewEventArgs e) {
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
