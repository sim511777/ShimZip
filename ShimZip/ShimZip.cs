using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShimZip {
   class ShimZip {
      public static DirData GetDirData(string dir) {
         DirData dirData = GetDirDataRec(dir);
         return dirData;
      }

      private static DirData GetDirDataRec(string dir) {
         DirData dirData = new DirData();
         dirData.name = Path.GetFileName(dir);
         
         string[] subDirs = Directory.GetDirectories(dir);
         foreach (var subDir in subDirs) {
            dirData.dirs.Add(GetDirDataRec(subDir));
         }

         string[] files = Directory.GetFiles(dir);
         foreach (var file in files) {
            dirData.files.Add(new FileData(file));
         }

         return dirData;
      }
   }

   class DirData {
      public string name = string.Empty;
      public List<DirData> dirs = new List<DirData>();
      public List<FileData> files = new List<FileData>();
   }

   class FileData {
      public string name = string.Empty;
      public long index = 0;
      public long length = 0;
      public FileData(string filePath) {
         name = Path.GetFileName(filePath);
         var fi = new FileInfo(filePath);
         this.length = fi.Length;
      }
   }
}
