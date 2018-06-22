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

      public static ZipData GetZipData(string[] files, string[] dirs, string zipFilePath) {
         ZipData zipData = new ZipData();
         GetZipDataRecursive(files, dirs, zipData, 0);
         return zipData;
      }

      public static void GetZipDataRecursive(string[] files, string[] dirs, ZipData zipData) {
         // files
         foreach (var file in files) {
            FileInfo fi = new FileInfo(file);
            FileData fileData = new FileData(fi.Name, 0, fi.Length, fi.FullName);
            zipData.fileDatas.Add(fileData);
         }

         // dirs
         foreach (var dir in dirs) {
            DirectoryInfo di = new DirectoryInfo(dir);
            DirData dirData = new DirData(di.Name, di.FullName);
            zipData.dirDatas.Add(dirData);
            
            var subFiles = Directory.GetFiles(dir);
            var subDirs = Directory.GetDirectories(dir);
            GetZipDataRecursive(subFiles, subDirs, dirData);
         }
      }

      public static ZipData GetZipData(string zipFilePath) {
         ZipData zipData = new ZipData();
         return zipData;
      }
   }

   public class ZipData {
      public List<FileData> fileDatas = new List<FileData>();
      public List<DirData> dirDatas = new List<DirData>();
   }

   public class FileData {
      public string name;
      public int offset;
      public long length;
      public string realPath;
      public FileData(string name, int offset, long length, string realPath) {
         this.name = name;
         this.realPath = realPath;
         this.offset = offset;
         this.length = length;
      }
   }

   public class DirData : ZipData {
      public string name;
      public string realPath;
      public DirData(string name, string realPath) {
         this.name = name;
         this.realPath = realPath;
      }
   }
}
