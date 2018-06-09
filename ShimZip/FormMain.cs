using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShimZip {
   public partial class FormMain : Form {
      public FormMain() {
         InitializeComponent();
      }

      private void DirDataToTree(DirData dirData, TreeNodeCollection nodes) {
         var node = nodes.Add(dirData.name);
         node.Tag = dirData;
         foreach (var subDir in dirData.dirs) {
            this.DirDataToTree(subDir, node.Nodes);
         }
      }

      private void ZipToTree(DirData dirData) {
         this.trvZip.Nodes.Clear();
         this.DirDataToTree(dirData, this.trvZip.Nodes);
         this.trvZip.ExpandAll();
      }

      private void zipToolStripMenuItem_Click(object sender, EventArgs e) {
         this.dlgFolder.SelectedPath = @"D:\joy\Game\3. Old FPS\Quake";
         if (this.dlgFolder.ShowDialog(this) != DialogResult.OK)
            return;
                  
         DirData dirData = ShimZip.GetDirData(this.dlgFolder.SelectedPath);
         this.ZipToTree(dirData);
      }

      private void trvZip_AfterSelect(object sender, TreeViewEventArgs e) {
         this.lvwFiles.Items.Clear();
         DirData dirData = e.Node.Tag as DirData;
         foreach (var file in dirData.files) {
            string[] subItems = new string[] { file.name, file.index.ToString(), file.length.ToString() };
            this.lvwFiles.Items.Add(new ListViewItem(subItems));
         }
      }
   }
}
