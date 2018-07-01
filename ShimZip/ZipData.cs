using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShimZip {
   public class ZipData {
      public const int BlockSize = 1000000;

      public DirData dirData;

      public static ZipData MakeZipData(string[] files, string[] dirs, ref int fileCnt, ref long totalByte) {
         ZipData zipData = new ZipData();
         zipData.dirData = new DirData();
         MakeDirDataRecursive(files, dirs, zipData.dirData, ref fileCnt, ref totalByte);
         return zipData;
      }

      // recursive
      private static void MakeDirDataRecursive(string[] files, string[] dirs, DirData dirData, ref int fileCnt, ref long totalByte) {
         for (int i = 0; i < files.Length; i++) {
            fileCnt++;
            var filePath = files[i];
            FileInfo fi = new FileInfo(filePath);
            totalByte += fi.Length;
            var fileData = new FileData(fi.Name, filePath, fi.Length, fi.CreationTimeUtc, fi.LastAccessTimeUtc, fi.LastWriteTimeUtc);
            dirData.fileDatas.Add(fileData);
         }

         for (int i = 0; i < dirs.Length; i++) {
            string dir = dirs[i];
            DirectoryInfo di = new DirectoryInfo(dir);
            var subDirData = new DirData(di.Name, di.CreationTimeUtc, di.LastAccessTimeUtc, di.LastWriteTimeUtc);
            dirData.dirDatas.Add(subDirData);
            MakeDirDataRecursive(Directory.GetFiles(dir), Directory.GetDirectories(dir), subDirData, ref fileCnt, ref totalByte);
         }
      }

      // zip file extract 
      public static int UnzipFile(List<FileData> fileDatas, List<DirData> dirDatas, string zipFilePath, string unzipDir) {
         using (var sr = File.OpenRead(zipFilePath))
         using (var br = new BinaryReader(sr)) {
            return UnzipFileRecursive(fileDatas, dirDatas, br, unzipDir);
         }
      }

      // recursive
      private static int UnzipFileRecursive(List<FileData> fileDatas, List<DirData> dirDatas, BinaryReader br, string dir, DirData dirData = null) {
         if (Directory.Exists(dir) == false) {
            Directory.CreateDirectory(dir);
            if (dirData != null) {
               try {
                  Directory.SetCreationTimeUtc(dir, dirData.creationTimeUtc);
                  Directory.SetLastAccessTimeUtc(dir, dirData.lastAccessTimeUtc);
                  Directory.SetLastWriteTimeUtc(dir, dirData.lastWriteTimeUtc);
               } catch {
               }
            }
         }

         // files
         foreach (var fileData in fileDatas) {
            string filePath = dir + "\\" + fileData.name;
            DecodeFile(br, fileData, filePath);
         }

         int fileCnt = fileDatas.Count;
         // dirs
         foreach (var subDirData in dirDatas) {
            string subDir = dir + "\\" + subDirData.name;
            fileCnt += UnzipFileRecursive(subDirData.fileDatas, subDirData.dirDatas, br, subDir, subDirData);
         }

         return fileCnt;
      }

      // file decoding
      private static void DecodeFile(BinaryReader br, FileData fileData, string filePath) {
         br.BaseStream.Seek(fileData.offset, SeekOrigin.Begin);
         using (var stream = File.Create(filePath)) {
            byte[] blockBuf = new byte[BlockSize];
            long remains = fileData.length;
            while (remains > 0) {
               int readSize = br.Read(blockBuf, 0, (remains >= BlockSize) ? BlockSize : (int)remains);
               stream.Write(blockBuf, 0, readSize);
               remains -= readSize;
            }
         }

         try {
            File.SetCreationTimeUtc(filePath, fileData.creationTimeUtc);
            File.SetLastAccessTimeUtc(filePath, fileData.lastAccessTimeUtc);
            File.SetLastWriteTimeUtc(filePath, fileData.lastWriteTimeUtc);
         } catch {
         }
      }
   }

   public class FileData {
      public string name;
      public long length;
      public DateTime creationTimeUtc;
      public DateTime lastAccessTimeUtc;
      public DateTime lastWriteTimeUtc;

      public long offset;
      public string realPath;

      public FileData(string name, long offset, long length, DateTime creationTimeUtc, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc) {
         this.name = name;
         this.offset = offset;
         this.length = length;
         this.creationTimeUtc = creationTimeUtc;
         this.lastAccessTimeUtc = lastAccessTimeUtc;
         this.lastWriteTimeUtc = lastWriteTimeUtc;
      }
      public FileData(string name, string realPath, long length, DateTime creationTimeUtc, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc) {
         this.name = name;
         this.realPath = realPath;
         this.length = length;
         this.creationTimeUtc = creationTimeUtc;
         this.lastAccessTimeUtc = lastAccessTimeUtc;
         this.lastWriteTimeUtc = lastWriteTimeUtc;
      }
   }

   public class DirData {
      public string name;
      public DateTime creationTimeUtc;
      public DateTime lastAccessTimeUtc;
      public DateTime lastWriteTimeUtc;

      public List<FileData> fileDatas = new List<FileData>();
      public List<DirData> dirDatas = new List<DirData>();

      public DirData() {
      }

      public DirData(string name, DateTime creationTimeUtc, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc) {
         this.name = name;
         this.creationTimeUtc = creationTimeUtc;
         this.lastAccessTimeUtc = lastAccessTimeUtc;
         this.lastWriteTimeUtc = lastWriteTimeUtc;
      }
   }
}
