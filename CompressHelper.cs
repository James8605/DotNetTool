using System.IO;
using System.IO.Compression;

using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;


namespace XGS.James.Tool
{
    public class CompressHelper
    {
        public static void CreateTarGZ(string tgzFilename, string sourceDirectory)
        {
            using var zip = File.OpenWrite(tgzFilename);
            using var zipWriter = WriterFactory.Open(zip, ArchiveType.Tar, CompressionType.GZip);
            zipWriter.WriteAll(sourceDirectory, "*", SearchOption.AllDirectories);
        }



        public static void DecompressTarGZ(string gzArchiveName, string destFolder)
        {
            using Stream stream = File.OpenRead(gzArchiveName);
            var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    reader.WriteEntryToDirectory(destFolder, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                }
            }
        }

        public static void UnzipFile(string raw_path, string dir)
        {
            Directory.Delete(dir, true);
            ZipFile.ExtractToDirectory(raw_path, dir);
        }

    }
}
