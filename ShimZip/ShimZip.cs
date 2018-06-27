using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Security.AccessControl;

namespace ShimZip {
   class ShimZip {
      private const int BlockSize = 1000000;

      // ==== write zip file ====
      //public static ZipData GetZipData()
      // file/dirs => zip file
      public static int ZipFile(string[] files, string[] dirs, string zipFilePath) {
         var sr = File.OpenWrite(zipFilePath);
         using (BinaryWriter bw = new BinaryWriter(sr)) {
            // header
            byte[] magic = Encoding.ASCII.GetBytes("SZIP");
            short version = 1;
            bw.Write(magic);
            bw.Write(version);

            // write file recursive
            return ZipFileRecursive(files, dirs, bw);
         }
      }

      // recursive
      private static int ZipFileRecursive(string[] files, string[] dirs, BinaryWriter bw) {
         int fileCnt = files.Length;
         
         // files
         bw.Write(files.Length);
         foreach (var file in files) {
            FileInfo fi = new FileInfo(file);
            bw.Write(fi.Name);
            bw.Write(fi.Length);
            bw.Write(fi.CreationTimeUtc.ToBinary());
            bw.Write(fi.LastAccessTimeUtc.ToBinary());
            bw.Write(fi.LastWriteTimeUtc.ToBinary());
            EncodeFile(file, bw);
         }

         // dirs
         bw.Write(dirs.Length);
         foreach (var dir in dirs) {
            DirectoryInfo di = new DirectoryInfo(dir);
            bw.Write(di.Name);
            bw.Write(di.CreationTimeUtc.ToBinary());
            bw.Write(di.LastAccessTimeUtc.ToBinary());
            bw.Write(di.LastWriteTimeUtc.ToBinary());

            var subFiles = Directory.GetFiles(dir);
            var subDirs = Directory.GetDirectories(dir);
            fileCnt += ZipFileRecursive(subFiles, subDirs, bw);
         }

         return fileCnt;
      }

      // file => stream
      private static void EncodeFile(string filePath, BinaryWriter bw) {
         using (var stream = File.OpenRead(filePath)) {
            byte[] blockBuf = new byte[BlockSize];
            int readSize;
            while ((readSize = stream.Read(blockBuf, 0, BlockSize)) > 0) {
               bw.Write(blockBuf, 0, readSize);
            }
         }
      }

      // ==== read zip file structure ====
      // zip file => DirData struct
      public static ZipData GetZipData(string zipFilePath) {
         ZipData zipData = new ZipData();
         using (var sr = File.OpenRead(zipFilePath))
         using (var br = new BinaryReader(sr)) {
            zipData.header = br.ReadBytes(4);
            zipData.version = br.ReadInt16();
         
            zipData.dirData = new DirData();
            GetDirDataRecursive(zipData.dirData, br);
         }
         return zipData;
      }

      // recursive
      private static void GetDirDataRecursive(DirData dirData, BinaryReader br) {
         // files
         int fileCount = br.ReadInt32();
         for (int i=0; i<fileCount; i++) {
            string fileName = br.ReadString();
            long fileLength = br.ReadInt64();
            DateTime creationTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DateTime lastAccessTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DateTime lastWriteTimeUtc = DateTime.FromBinary(br.ReadInt64());
            long fileOffset = br.BaseStream.Position;
            br.BaseStream.Seek(fileLength, SeekOrigin.Current);
            
            FileData fileData = new FileData(fileName, fileOffset, fileLength, creationTimeUtc, lastAccessTimeUtc, lastWriteTimeUtc);
            dirData.fileDatas.Add(fileData);
         }

         // dirs
         int dirCount = br.ReadInt32();
         for (int i=0; i<dirCount; i++) {
            string dirName = br.ReadString();
            DateTime creationTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DateTime lastAccessTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DateTime lastWriteTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DirData subDirData = new DirData(dirName, creationTimeUtc, lastAccessTimeUtc, lastWriteTimeUtc);
            dirData.dirDatas.Add(subDirData);
            GetDirDataRecursive(subDirData, br);
         }
      }

      // zip file extract 
      // file/dirs => zip file
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
               Directory.SetCreationTimeUtc(dir, dirData.creationTimeUtc);
               Directory.SetLastAccessTimeUtc(dir, dirData.lastAccessTimeUtc);
               Directory.SetLastWriteTimeUtc(dir, dirData.lastWriteTimeUtc);
            }
         }

         int fileCnt = fileDatas.Count;
         
         // files
         foreach (var fileData in fileDatas) {
            string filePath = dir + "\\" + fileData.name;
            DecodeFile(br, fileData, filePath);
         }

         // dirs
         foreach (var subDirData in dirDatas) {
            string subDir = dir + "\\" + subDirData.name;
            fileCnt += UnzipFileRecursive(subDirData.fileDatas, subDirData.dirDatas, br, subDir, subDirData);
         }

         return fileCnt;
      }

      // stream => file
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

         File.SetCreationTimeUtc(filePath, fileData.creationTimeUtc);
         File.SetLastAccessTimeUtc(filePath, fileData.lastAccessTimeUtc);
         File.SetLastWriteTimeUtc(filePath, fileData.lastWriteTimeUtc);
      }
   }

   public class ZipData {
      public byte[] header;
      public short version;
      public DirData dirData;
   }

   public class FileData {
      public string name;
      public long length;
      public DateTime creationTimeUtc;
      public DateTime lastAccessTimeUtc;
      public DateTime lastWriteTimeUtc;

      public long offset;

      public FileData(string name, long offset, long length, DateTime creationTimeUtc, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc) {
         this.name = name;
         this.offset = offset;
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
