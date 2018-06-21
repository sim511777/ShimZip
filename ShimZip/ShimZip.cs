using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShimZip {
   class ShimZip {
      public static void ZipFile(string[] files, string[] dirs, string zipFilePath) {
         var sr = File.OpenWrite(zipFilePath);
         using (BinaryWriter bw = new BinaryWriter(sr)) {
            ZipRecursive(files, dirs, bw);
         }
      }

      public static void UnzipFile(string zipFilePath, string unzipDir) {
         var sr = File.OpenRead(zipFilePath);
         using (BinaryReader br = new BinaryReader(sr)) {
            UnzipRecursive(unzipDir, br);
         }
      }

      public static void ZipRecursive(string[] files, string[] dirs, BinaryWriter bw) {
         // files
         bw.Write(files.Length);
         foreach (var file in files) {
            FileInfo fi = new FileInfo(file);
            bw.Write(fi.Name);
            bw.Write((int)fi.Length);
            var data = File.ReadAllBytes(file);
            bw.Write(data);
         }

         // dirs
         bw.Write(dirs.Length);
         foreach (var dir in dirs) {
            DirectoryInfo di = new DirectoryInfo(dir);
            bw.Write(di.Name);
            var subFiles = Directory.GetFiles(dir);
            var subDirs = Directory.GetDirectories(dir);
            ZipRecursive(subFiles, subDirs, bw);
         }
      }

      public static void UnzipRecursive(string dir, BinaryReader br) {
         Directory.CreateDirectory(dir);
         
         // files
         int fileCount = br.ReadInt32();
         for (int i=0; i<fileCount; i++) {
            string fileName = br.ReadString();
            int fileLength = br.ReadInt32();
            string filePath = dir + "\\" + fileName;
            byte[] data = br.ReadBytes(fileLength);
            File.WriteAllBytes(filePath, data);
         }

         // dirs
         int dirCount = br.ReadInt32();
         for (int i=0; i<dirCount; i++) {
            string subDirName = br.ReadString();
            string subDir = dir + "\\" + subDirName;
            UnzipRecursive(subDir, br);
         }
      }
   }
}
