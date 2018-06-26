using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShimZip {
   class CommandLine {
      public static void Run(string[] args) {
         // 명령줄 포멧이 맞는지 확인
         if (CheckCommandFormatCorrect(args) == false) {
            string msg = string.Empty;
            MessageBox.Show(msg);
            return;
         }
         // 파라미터가 유효한지 확인
         if (CheckCommandParamValid(args) == false) {
            string msg = string.Empty;
            MessageBox.Show(msg);
            return;
         }
      }

      private static bool CheckCommandFormatCorrect(string[] args) {
         return true;
      }

      private static bool CheckCommandParamValid(string[] args) {
         return true;
      }

      private static void ExtractHere(string[] zipFilePaths) {

      }

      private static void ExtractAuto(string[] zipFilePaths) {

      }

      private static void ExtractFilenameDir(string[] zipFilePaths) {

      }

      private static void ExtractDirDlg(string[] zipFilePaths) {

      }

      private static void ExtractOpen(string zipFilePath) {

      }

      private static void ArchiveFilename(string[] filePaths) {

      }

      private static void ArchiveFileDlg(string[] filePaths) {

      }

      private static void ArchiveDirname(string[] filePaths) {

      }
   }
}
