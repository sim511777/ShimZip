using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShimZip {
   class CommandLine {
      public static void Run(string[] args) {
         var a0 = args[0];
         var a1 = args[1];
         var filePaths = args.Skip(2).ToArray();
         if (a0 == "-x") {
            if (a1 == "-here")
               ExtractHere(filePaths);
            else if (a1 == "-auto")
               ExtractAuto(filePaths);
            else if (a1 == "-filenamedir")
               ExtractFilenameDir(filePaths);
            else if (a1 == "-dirdlg")
               ExtractDirDlg(filePaths);
         } else if (args[0] == "-a") {
            if (a1 == "-filename")
               ArchiveFilename(filePaths);
            else if (a1 == "-filedlg")
               ArchiveFileDlg(filePaths);
            else if (a1 == "-dirname")
               ArchiveDirname(filePaths);
         }
      }

      // 여기에 풀기 -x -here "aaa.sip"
      // 각각 여기에 풀기 -x -here "aaa.sip" "bbb.sip"
      // 현재 폴더에 푼다.
      private static void ExtractHere(string[] zipFilePaths) {

      }

      // 파일명 폴더에 풀기 -x -filenamedir "aaa.sip"
      // 각각 파일명 폴더에 풀기 -x -filenamedir "aaa.sip" "bbb.sip"
      // zip파일명 폴더에 푼다
      private static void ExtractFilenameDir(string[] zipFilePaths) {

      }

      // 알아서 풀기 -x -auto "aaa.sip"
      // 각각 알아서 풀기 -x -auto "aaa.sip" "bbb.sip"
      // 루트에 하나만 있으면 여기에 풀기, 아니면 파일명 폴더에 풀기
      private static void ExtractAuto(string[] zipFilePaths) {

      }


      // 반디집으로 풀기 -x -dirdlg "aaa.sip"
      // 반디집으로 풀기 -x -dirdlg "aaa.sip" "bbb.sip"
      // 폴더 선택 대화상자 에서 지정된 폴더에 푼다.
      private static void ExtractDirDlg(string[] zipFilePaths) {

      }

      // 파일명.zip으로 압축하기 -a -filename "aaa.txt"
      // 각각 파일명.zip으로 압축하기 -a -filename "aaa.sip" "bbb.sip"
      // 각각 파일명.zip으로 압축하기 -a -filename "aaa.txt" "bbb.txt"
      // 루트 아이템 하나당 각각 아이템 이름.zip으로 압축
      private static void ArchiveFilename(string[] filePaths) {

      }

      // 반디집으로 압축하기 -a -filedlg "aaa.txt"
      // 반디집으로 압축하기 -a -filedlg "aaa.sip" "bbb.sip"
      // 반디집으로 압축하기 -a -filedlg "aaa.txt" "bbb.txt"
      // 파일 선택 대화상자 에서 지정된 파일로 압축
      private static void ArchiveFileDlg(string[] filePaths) {

      }

      // 폴더명.zip으로 압축하기 -a -dirname "aaa.sip" "bbb.sip"
      // 폴더명.zip으로 압축하기 -a -dirname "aaa.txt" "bbb.txt"
      // 최상위 아이템이 있는 디렉토리.zip으로 압축
      private static void ArchiveDirname(string[] filePaths) {

      }
   }
}
