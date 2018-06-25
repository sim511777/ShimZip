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
      public const int BlockSize = 1000000;

      // ==== write zip file ====
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
            FileToStream(file, bw);
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
      private static void FileToStream(string filePath, BinaryWriter bw) {
         //byte[] blockBuf = new byte[BlockSize];
         //using (var stream = File.OpenRead(filePath)) {
         //   var br = new BinaryReader(stream);
         //   int readSize;
         //   while ((readSize = stream.Read(blockBuf, 0, BlockSize)) > 0) {
         //      bw.Write(blockBuf, 0, readSize);
         //   }
         //}

         // todo: long length file?
         byte[] data = File.ReadAllBytes(filePath);
         bw.Write(data);
      }

      // ==== read zip file structure ====
      // zip file => zipData struct
      public static ZipData GetZipData(BinaryReader br) {
         ZipData zipData = new ZipData();
         
         // header
         zipData.header = br.ReadBytes(4);
         zipData.version = br.ReadInt16();
         
         GetZipDataRecursive(zipData, br);
         return zipData;
      }

      // recursive
      private static void GetZipDataRecursive(ZipData zipData, BinaryReader br) {
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
            zipData.fileDatas.Add(fileData);
         }

         // dirs
         int dirCount = br.ReadInt32();
         for (int i=0; i<dirCount; i++) {
            string dirName = br.ReadString();
            DateTime creationTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DateTime lastAccessTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DateTime lastWriteTimeUtc = DateTime.FromBinary(br.ReadInt64());
            DirData dirData = new DirData(dirName, creationTimeUtc, lastAccessTimeUtc, lastWriteTimeUtc);
            zipData.dirDatas.Add(dirData);
            GetZipDataRecursive(dirData, br);
         }
      }

      // zip file extract 
      // file/dirs => zip file
      public static int UnzipFile(List<FileData> fileDatas, List<DirData> dirDatas, BinaryReader br, string unzipDir) {
         return UnzipFileRecursive(fileDatas, dirDatas, br, unzipDir);
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
            StreamToFile(br, fileData, filePath);
         }

         // dirs
         foreach (var subDirData in dirDatas) {
            string subDir = dir + "\\" + subDirData.name;
            fileCnt += UnzipFileRecursive(subDirData.fileDatas, subDirData.dirDatas, br, subDir, subDirData);
         }

         return fileCnt;
      }

      // stream => file
      public static void StreamToFile(BinaryReader br, FileData fileData, string filePath) {
         //byte[] blockBuf = new byte[BlockSize];
         //using (var stream = File.Create(filePath)) {
         //   long remains = fileData.length;
         //   while (remains > 0) {
         //      int readSize = br.Read(blockBuf, 0, (remains >= BlockSize) ? BlockSize : (int)remains);
         //      stream.Write(blockBuf, 0, readSize);
         //      remains -= readSize;
         //   }
         //}

         // todo: long length file?
         br.BaseStream.Seek(fileData.offset, SeekOrigin.Begin);
         byte[] data = br.ReadBytes((int)fileData.length);
         File.WriteAllBytes(filePath, data);

         File.SetCreationTimeUtc(filePath, fileData.creationTimeUtc);
         File.SetLastAccessTimeUtc(filePath, fileData.lastAccessTimeUtc);
         File.SetLastWriteTimeUtc(filePath, fileData.lastWriteTimeUtc);
      }
   }

   public class ZipData {
      public byte[] header;
      public short version;
      public List<FileData> fileDatas = new List<FileData>();
      public List<DirData> dirDatas = new List<DirData>();
   }

   public class FileData {
      public string name;
      public long offset;
      public long length;
      public DateTime creationTimeUtc;
      public DateTime lastAccessTimeUtc;
      public DateTime lastWriteTimeUtc;
      public FileData(string name, long offset, long length, DateTime creationTimeUtc, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc) {
         this.name = name;
         this.offset = offset;
         this.length = length;
         this.creationTimeUtc = creationTimeUtc;
         this.lastAccessTimeUtc = lastAccessTimeUtc;
         this.lastWriteTimeUtc = lastWriteTimeUtc;
      }
   }

   public class DirData : ZipData {
      public string name;
      public DateTime creationTimeUtc;
      public DateTime lastAccessTimeUtc;
      public DateTime lastWriteTimeUtc;
      public FileSystemInfo info;
      public DirData(string name, DateTime creationTimeUtc, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc) {
         this.name = name;
         this.creationTimeUtc = creationTimeUtc;
         this.lastAccessTimeUtc = lastAccessTimeUtc;
         this.lastWriteTimeUtc = lastWriteTimeUtc;
      }
   }
}
