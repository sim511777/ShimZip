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
      private const string magic = "SZIP";
      private const short version = 1;
      private const int BlockSize = 1000000;

      // ==== make directory structure ====
      public static ZipData MakeZipData(string[] files, string[] dirs, ref int fileCnt, ref long totalByte) {
         ZipData zipData = new ZipData();
         zipData.magic = ShimZip.magic;
         zipData.version = ShimZip.version;
         zipData.dirData = new DirData();
         MakeDirDataRecursive(files, dirs, zipData.dirData, ref fileCnt, ref totalByte);
         return zipData;
      }

      // recursive
      private static void MakeDirDataRecursive(string[] files, string[] dirs, DirData dirData, ref int fileCnt, ref long totalByte) {
         for (int i=0; i<files.Length; i++) {
            fileCnt++;
            var filePath = files[i];
            FileInfo fi = new FileInfo(filePath);
            totalByte += fi.Length;
            var fileData = new FileData(fi.Name, filePath, fi.Length, fi.CreationTimeUtc, fi.LastAccessTimeUtc, fi.LastWriteTimeUtc);
            dirData.fileDatas.Add(fileData);
         }

         for (int i=0; i<dirs.Length; i++) {
            string dir = dirs[i];
            DirectoryInfo di = new DirectoryInfo(dir);
            var subDirData = new DirData(di.Name, di.CreationTimeUtc, di.LastAccessTimeUtc, di.LastWriteTimeUtc);
            dirData.dirDatas.Add(subDirData);
            MakeDirDataRecursive(Directory.GetFiles(dir), Directory.GetDirectories(dir),subDirData, ref fileCnt, ref totalByte);
         }
      }

      // ==== write zip file ====
      // zipData => zip file
      public static int ZipFile(ZipData zipData, string zipFilePath) {
         var sr = File.OpenWrite(zipFilePath);
         using (BinaryWriter bw = new BinaryWriter(sr)) {
            // header
            bw.Write(Encoding.ASCII.GetBytes(ShimZip.magic));
            bw.Write(ShimZip.version);

            // write file recursive
            return ZipFileRecursive(zipData.dirData.fileDatas, zipData.dirData.dirDatas, bw);
         }
      }

      // recursive
      private static int ZipFileRecursive(List<FileData> fileDatas, List<DirData> dirDatas, BinaryWriter bw) {
         int fileCnt = fileDatas.Count;
         // files
         bw.Write(fileDatas.Count);
         foreach (var fileData in fileDatas) {
            bw.Write(fileData.name);
            bw.Write(fileData.length);
            bw.Write(fileData.creationTimeUtc.ToBinary());
            bw.Write(fileData.lastAccessTimeUtc.ToBinary());
            bw.Write(fileData.lastWriteTimeUtc.ToBinary());
            EncodeFile(fileData.realPath, bw);
         }

         // dirs
         bw.Write(dirDatas.Count);
         foreach (var dirData in dirDatas) {
            bw.Write(dirData.name);
            bw.Write(dirData.creationTimeUtc.ToBinary());
            bw.Write(dirData.lastAccessTimeUtc.ToBinary());
            bw.Write(dirData.lastWriteTimeUtc.ToBinary());

            fileCnt += ZipFileRecursive(dirData.fileDatas, dirData.dirDatas, bw);
         }

         return fileCnt;
      }

      // file encoding
      private static void EncodeFile(string filePath, BinaryWriter bw) {
         using (var stream = File.OpenRead(filePath)) {
            byte[] blockBuf = new byte[BlockSize];
            int readSize;
            while ((readSize = stream.Read(blockBuf, 0, BlockSize)) > 0) {
               bw.Write(blockBuf, 0, readSize);
            }
         }
      }

      // ==== read directory structure ====
      public static ZipData GetZipData(string zipFilePath, ref int fileCnt, ref long totalByte) {
         ZipData zipData = new ZipData();
         using (var sr = File.OpenRead(zipFilePath))
         using (var br = new BinaryReader(sr)) {
            zipData.magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            zipData.version = br.ReadInt16();
            if (zipData.magic != ShimZip.magic || zipData.version > ShimZip.version)
               throw new Exception("Invalid header");
         
            zipData.dirData = new DirData();
            GetDirDataRecursive(zipData.dirData, br, ref fileCnt, ref totalByte);
         }
         return zipData;
      }

      // recursive
      private static void GetDirDataRecursive(DirData dirData, BinaryReader br, ref int fileCnt, ref long totalByte) {
         // files
         int fileCount = br.ReadInt32();
         for (int i=0; i<fileCount; i++) {
            fileCnt ++;
            string fileName = br.ReadString();
            long fileLength = br.ReadInt64();
            totalByte += fileLength;
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
            GetDirDataRecursive(subDirData, br, ref fileCnt, ref totalByte);
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
               Directory.SetCreationTimeUtc(dir, dirData.creationTimeUtc);
               Directory.SetLastAccessTimeUtc(dir, dirData.lastAccessTimeUtc);
               Directory.SetLastWriteTimeUtc(dir, dirData.lastWriteTimeUtc);
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

         File.SetCreationTimeUtc(filePath, fileData.creationTimeUtc);
         File.SetLastAccessTimeUtc(filePath, fileData.lastAccessTimeUtc);
         File.SetLastWriteTimeUtc(filePath, fileData.lastWriteTimeUtc);
      }
   }

   public class ZipData {
      public string magic;
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
