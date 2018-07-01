using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShimZip {
   class QuakePak {
      private const string magic = "PACK";
      private const short version = 1;
      private const int BlockSize = 1000000;

      //// ==== write zip file ====
      //// zipData => zip file
      //public static int ZipFile(ZipData zipData, string zipFilePath) {
      //   var sr = File.OpenWrite(zipFilePath);
      //   using (BinaryWriter bw = new BinaryWriter(sr)) {
      //      // header
      //      bw.Write(Encoding.ASCII.GetBytes(QuakePak.magic));
      //      bw.Write(QuakePak.version);

      //      // write file recursive
      //      return ZipFileRecursive(zipData.dirData.fileDatas, zipData.dirData.dirDatas, bw);
      //   }
      //}

      //// recursive
      //private static int ZipFileRecursive(List<FileData> fileDatas, List<DirData> dirDatas, BinaryWriter bw) {
      //   int fileCnt = fileDatas.Count;
      //   // files
      //   bw.Write(fileDatas.Count);
      //   foreach (var fileData in fileDatas) {
      //      bw.Write(fileData.name);
      //      bw.Write(fileData.length);
      //      bw.Write(fileData.creationTimeUtc.ToBinary());
      //      bw.Write(fileData.lastAccessTimeUtc.ToBinary());
      //      bw.Write(fileData.lastWriteTimeUtc.ToBinary());
      //      EncodeFile(fileData.realPath, bw);
      //   }

      //   // dirs
      //   bw.Write(dirDatas.Count);
      //   foreach (var dirData in dirDatas) {
      //      bw.Write(dirData.name);
      //      bw.Write(dirData.creationTimeUtc.ToBinary());
      //      bw.Write(dirData.lastAccessTimeUtc.ToBinary());
      //      bw.Write(dirData.lastWriteTimeUtc.ToBinary());

      //      fileCnt += ZipFileRecursive(dirData.fileDatas, dirData.dirDatas, bw);
      //   }

      //   return fileCnt;
      //}

      //// file encoding
      //private static void EncodeFile(string filePath, BinaryWriter bw) {
      //   using (var stream = File.OpenRead(filePath)) {
      //      byte[] blockBuf = new byte[BlockSize];
      //      int readSize;
      //      while ((readSize = stream.Read(blockBuf, 0, BlockSize)) > 0) {
      //         bw.Write(blockBuf, 0, readSize);
      //      }
      //   }
      //}

      // ==== read directory structure ====
      public static ZipData GetZipData(string zipFilePath, ref int fileCnt, ref long totalByte) {
         ZipData zipData = new ZipData();
         using (var sr = File.OpenRead(zipFilePath))
         using (var br = new BinaryReader(sr)) {
            string magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            if (magic != QuakePak.magic)
               throw new Exception("Invalid PAK header");

            zipData.dirData = new DirData();
            int tableOffset = br.ReadInt32();
            int tableSize = br.ReadInt32();

            int fileDataSize = 64;
            fileCnt = tableSize/ fileDataSize;
            int filePathSize = 56;

            List<FileData> fileDatas = new List<FileData>();

            totalByte = 0;
            br.BaseStream.Seek(tableOffset, SeekOrigin.Begin);
            for (int i=0; i<fileCnt; i++) {
               var bytes = br.ReadBytes(filePathSize);
               int nullIndex = Array.IndexOf<byte>(bytes, 0);
               string filePath = Encoding.ASCII.GetString(bytes, 0, nullIndex);
               string fileName = Path.GetFileName(filePath);
               int fileOffset = br.ReadInt32();
               int fileSize = br.ReadInt32();
               totalByte += fileSize;

               FileData fileData = new FileData(filePath, fileOffset, fileSize, new DateTime(), new DateTime(), new DateTime());
               fileDatas.Add(fileData);
            }
            
            MakeStructure(zipData, fileDatas);
         }
         return zipData;
      }

      private static void MakeStructure(ZipData zipData, List<FileData> fileDatas) {         
         foreach (var fileData in fileDatas) {
            ZipDataAddFileData(zipData, fileData);
         }         
      }

      private static void ZipDataAddFileData(ZipData zipData, FileData fileData) {
         var tokens = fileData.name.Split('/');
         fileData.name = tokens.Last();
         fileData.realPath = string.Empty;
         var dirs = tokens.Take(tokens.Length - 1).ToList();

         DirData dirData = zipData.dirData;
         foreach (var dir in dirs) {
            DirData findDirData = dirData.dirDatas.Find(subDir => subDir.name == dir);
            if (findDirData == null) {
               findDirData = new DirData(dir, new DateTime(), new DateTime(), new DateTime());
               dirData.dirDatas.Add(findDirData);
            }
            dirData = findDirData;
         }

         dirData.fileDatas.Add(fileData);
      }      
   }
}
