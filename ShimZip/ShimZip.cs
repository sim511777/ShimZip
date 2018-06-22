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
         using (BinaryWriter writer = new BinaryWriter(sr)) {
            // header
            byte[] magic = Encoding.ASCII.GetBytes("SZIP");
            short version = 1;
            writer.Write(magic);
            writer.Write(version);

            // write file recursive
            return ZipFileRecursive(files, dirs, writer);
         }
      }

      // recursive
      private static int ZipFileRecursive(string[] files, string[] dirs, BinaryWriter writer) {
         int fileCnt = files.Length;
         
         // files
         writer.Write(files.Length);
         foreach (var file in files) {
            FileInfo fi = new FileInfo(file);
            writer.Write(fi.Name);
            writer.Write((int)fi.Length);
            var data = File.ReadAllBytes(file);
            writer.Write(data);
         }

         // dirs
         writer.Write(dirs.Length);
         foreach (var dir in dirs) {
            DirectoryInfo di = new DirectoryInfo(dir);
            writer.Write(di.Name);
            var subFiles = Directory.GetFiles(dir);
            var subDirs = Directory.GetDirectories(dir);
            fileCnt += ZipFileRecursive(subFiles, subDirs, writer);
         }

         return fileCnt;
      }

      // zip file => zipData struct
      public static ZipData GetZipData(BinaryReader reader) {
         ZipData zipData = new ZipData();
         
         // header
         zipData.header = reader.ReadBytes(4);
         zipData.version = reader.ReadInt16();
         
         GetZipDataRecursive(zipData, reader);
         return zipData;
      }

      // recursive
      private static void GetZipDataRecursive(ZipData zipData, BinaryReader br) {
         // files
         int fileCount = br.ReadInt32();
         for (int i=0; i<fileCount; i++) {
            string fileName = br.ReadString();
            int fileLength = br.ReadInt32();
            int fileOffset = (int)br.BaseStream.Position;
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
            br.BaseStream.Seek(fileData.offset, SeekOrigin.Begin);
            byte[] data = br.ReadBytes((int)fileData.length);
            File.WriteAllBytes(filePath, data);
         }

         // dirs
         foreach (var dirData in dirDatas) {
            string subDir = dir + "\\" + dirData.name;
            fileCnt += UnzipFileRecursive(dirData.fileDatas, dirData.dirDatas, br, subDir);
         }

         return fileCnt;
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
      public int offset;
      public long length;
      public FileData(string name, int offset, long length) {
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
