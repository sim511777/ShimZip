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
            byte[] blockBuf = new byte[ZipData.BlockSize];
            int readSize;
            while ((readSize = stream.Read(blockBuf, 0, ZipData.BlockSize)) > 0) {
               bw.Write(blockBuf, 0, readSize);
            }
         }
      }

      // ==== read directory structure ====
      public static ZipData GetZipData(string zipFilePath, ref int fileCnt, ref long totalByte) {
         ZipData zipData = new ZipData();
         using (var sr = File.OpenRead(zipFilePath))
         using (var br = new BinaryReader(sr)) {
            string magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            var version = br.ReadInt16();
            if (magic != ShimZip.magic || version > ShimZip.version)
               throw new Exception("Invalid ShimZip header");
         
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
   }
}
