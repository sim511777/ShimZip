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
         this.dlgFolder.SelectedPath = @"e:\joy\game\Quake";
         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
         
         this.dlgSave.InitialDirectory = Path.GetDirectoryName(this.dlgFolder.SelectedPath);
         this.dlgSave.FileName = Path.GetFileName(this.dlgFolder.SelectedPath) + ".szip";
         if (this.dlgSave.ShowDialog(this) != DialogResult.OK)
            return;
         ShimZip.SaveZip(this.dlgFolder.SelectedPath, this.dlgSave.FileName);
      }
   }
}
