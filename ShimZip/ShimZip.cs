using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShimZip {
   class ShimZip {
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
            FileToStream(file, bw);
         }

         // dirs
         bw.Write(dirs.Length);
         foreach (var dir in dirs) {
            DirectoryInfo di = new DirectoryInfo(dir);
            bw.Write(di.Name);
            var subFiles = Directory.GetFiles(dir);
            var subDirs = Directory.GetDirectories(dir);
            fileCnt += ZipFileRecursive(subFiles, subDirs, bw);
         }

         return fileCnt;
      }

      // file => stream
      private static void FileToStream(string filePath, BinaryWriter bw) {
         // todo: long length file?
         byte[] data = File.ReadAllBytes(filePath);
         bw.Write(data);
      }

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
            long fileOffset = br.BaseStream.Position;
            br.BaseStream.Seek(fileLength, SeekOrigin.Current);
            
            FileData fileData = new FileData(fileName, fileOffset, fileLength);
            zipData.fileDatas.Add(fileData);
         }

         // dirs
         int dirCount = br.ReadInt32();
         for (int i=0; i<dirCount; i++) {
            string dirName = br.ReadString();

            DirData dirData = new DirData(dirName);
            zipData.dirDatas.Add(dirData);
            GetZipDataRecursive(dirData, br);
         }
      }

      // file/dirs => zip file
      public static int UnzipFile(List<FileData> fileDatas, List<DirData> dirDatas, BinaryReader br, string unzipDir) {
         return UnzipFileRecursive(fileDatas, dirDatas, br, unzipDir);
      }

      // recursive
      private static int UnzipFileRecursive(List<FileData> fileDatas, List<DirData> dirDatas, BinaryReader br, string dir) {
         if (Directory.Exists(dir) == false)
            Directory.CreateDirectory(dir);

         int fileCnt = fileDatas.Count;
         
         // files
         foreach (var fileData in fileDatas) {
            string filePath = dir + "\\" + fileData.name;
            StreamToFile(br, fileData.offset, fileData.length, filePath);
         }

         // dirs
         foreach (var dirData in dirDatas) {
            string subDir = dir + "\\" + dirData.name;
            fileCnt += UnzipFileRecursive(dirData.fileDatas, dirData.dirDatas, br, subDir);
         }

         return fileCnt;
      }

      // stream => file
      public static void StreamToFile(BinaryReader br, long offset, long length, string filePath) {
         // todo: long length file?
         br.BaseStream.Seek(offset, SeekOrigin.Begin);
         byte[] data = br.ReadBytes((int)length);
         File.WriteAllBytes(filePath, data);
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
      public FileData(string name, long offset, long length) {
         this.name = name;
         this.offset = offset;
         this.length = length;
      }
   }

   public class DirData : ZipData {
      public string name;
      public DirData(string name) {
         this.name = name;
      }
   }
}
