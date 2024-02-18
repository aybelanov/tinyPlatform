using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using Shared.Clients;

namespace Hub.Services.ExportImport;

/// <summary>
/// Represents a export file implementation
/// </summary>
public sealed class ExportFile
{
   /// <summary>
   /// Filename of the exporting file
   /// </summary>
   public string FileName { get; init; }

   /// <summary>
   /// Export file byte array
   /// </summary>
   public byte[] ByteArray { get; init; }

   /// <summary>
   /// File mime type
   /// </summary>
   public string MimeType { get; init; }

   /// <summary>
   /// File extension
   /// </summary>
   public string Extension { get; init; }

   /// <summary>
   /// Full file name
   /// </summary>
   public string FullName
   {
      get
      {
         if (string.IsNullOrWhiteSpace(Extension))
            return FileName;
         else
            return FileName + "." + Extension;
      }
   }

   /// <summary>
   /// Init data for file generation
   /// </summary>
   public object InitialData { get; init; }

   /// <summary>
   /// File encoding
   /// </summary>
   public Encoding FileEncoding { get; init; }

   /// <summary>
   /// String of the file
   /// </summary>
   public string FileString
   {
      get
      {
         if (FileEncoding == null || ByteArray == null)
            return null;
         else
            return FileEncoding.GetString(ByteArray);
      }
   }


   /// <summary>
   /// DefaultCtor
   /// </summary>
   public ExportFile() { }

   /// <summary>
   /// TXT Ctor
   /// </summary>
   /// <param name="text"></param>
   /// <param name="codePage"></param>
   /// <param name="fileName"></param>
   public ExportFile(string text, int codePage = 65001, string fileName = "txtdata")
   {
      ArgumentNullException.ThrowIfNull(text, nameof(text));

      FileEncoding = Encoding.GetEncoding(codePage);
      Extension = "txt";
      ByteArray = Encoding.GetEncoding(codePage).GetBytes(text);
      FileName = fileName;
      MimeType = "text/plain";
      InitialData = text;
   }

   /// <summary>
   /// XDoc (xml) ctor
   /// </summary>
   /// <param name="xdoc">XDoc object with data</param>
   /// <param name="codePage">Encoding ID (default UTF-8)</param>
   /// <param name="fileName">File name (without extension)</param>
   public ExportFile(XDocument xdoc, int codePage = 65001, string fileName = "xmldata")
   {
      ArgumentNullException.ThrowIfNull(xdoc, nameof(xdoc));

      FileEncoding = Encoding.GetEncoding(codePage);
      Extension = "xml";
      ByteArray = XmlToByteArray(xdoc);
      FileName = fileName;
      MimeType = "text/xml";
      InitialData = xdoc;
   }

   /// <summary>
   /// CSV Ctor
   /// </summary>
   /// <param name="csvrecords">Collection object of CSV data</param>
   /// <param name="codePage">Encoding ID (default UTF-8)</param>
   /// <param name="fileName">File name (without extension)</param>
   /// <param name="delimiter">Value delimiter</param>
   /// <param name="header">Is header including</param>
   public ExportFile(IEnumerable csvrecords, int codePage = 65001, string fileName = "csvdata", string delimiter = "\t", bool header = true)
   {
      ArgumentNullException.ThrowIfNull(csvrecords, nameof(csvrecords));

      FileEncoding = Encoding.GetEncoding(codePage);
      Extension = "csv";
      FileName = fileName;
      MimeType = "text/csv";
      ByteArray = CsvToByteArray(csvrecords, delimiter, header);
      InitialData = csvrecords;
   }

   /// <summary>
   /// Excel (xlsx) Ctor
   /// </summary>
   /// <param name="package">Excell book</param>
   /// <param name="codePage">Encoding ID (default UTF-8)</param>
   /// <param name="fileName">File name (without extension)</param>
   public ExportFile(ExcelPackage package, int codePage = 65001, string fileName = "xlsxdata")
   {
      ArgumentNullException.ThrowIfNull(package, nameof(package));

      FileEncoding = Encoding.GetEncoding(codePage);
      Extension = "xlsx";
      FileName = fileName;
      MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
      ByteArray = XlsxToByteArray(package);
      InitialData = package;
   }

   /// <summary>
   /// JSON Ctor
   /// </summary>
   /// <param name="jdata">JSON object</param>
   /// <param name="codePage">Encoding ID (default UTF-8)</param>
   /// <param name="fileName">File name (without extension)</param>
   public ExportFile(JsonObject jdata, int codePage = 65001, string fileName = "jsondata")
   {
      ArgumentNullException.ThrowIfNull(jdata, nameof(jdata));

      FileEncoding = Encoding.GetEncoding(codePage);
      Extension = "json";
      FileName = fileName;
      MimeType = "application/json";
      ByteArray = JObjectToByteArray(jdata);
      InitialData = jdata;
   }

   /// <summary>
   /// JSON object to a byte array
   /// </summary>
   /// <param name="jdoc">JSON document</param>
   /// <returns>Byte array</returns>
   private byte[] JObjectToByteArray(JsonObject jdoc)
   {
      ArgumentNullException.ThrowIfNull(jdoc, nameof(jdoc));

      var fileArray = FileEncoding.GetBytes(jdoc.ToJsonString());
      return fileArray;
   }

   /// <summary>
   /// Xlm object to a byte array
   /// </summary>
   /// <param name="xdoc">XML document</param>
   /// <returns>Byte array</returns>
   private byte[] XmlToByteArray(XDocument xdoc)
   {
      ArgumentNullException.ThrowIfNull(xdoc, nameof(xdoc));

      using (var str = new MemoryStream())
      using (var wr = new StreamWriter(str, FileEncoding))
      {
         xdoc.Save(wr);
         return str.ToArray();
      }
   }

   /// <summary>
   /// CSV object to a byte array
   /// </summary>
   /// <param name="csvrecords">Collection of the csv records</param>
   /// <param name="delimitter">Valeu delimiter</param>
   /// <param name="header">including a header</param>
   /// <returns>Byte array</returns>
   private byte[] CsvToByteArray(IEnumerable csvrecords, string delimitter, bool header)
   {
      ArgumentNullException.ThrowIfNull(csvrecords, nameof(csvrecords));

      using (var outStream = new MemoryStream())
      {
         using (var textWritter = new StreamWriter(outStream, FileEncoding))
         using (var csv = new CsvWriter(textWritter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = delimitter, HasHeaderRecord = header }))
         {
            csv.WriteRecords(csvrecords);
            return outStream.ToArray();
         }
      }
   }

   /// <summary>
   /// Xlsx document to a byte array
   /// </summary>
   /// <param name="package">xls-document</param>
   /// <returns>Byte array</returns>
   private byte[] XlsxToByteArray(ExcelPackage package)
   {
      ArgumentNullException.ThrowIfNull(package, nameof(package));

      using (var str = new MemoryStream())
      {
         package.SaveAs(str);
         return str.ToArray();
      }
   }

   /// <summary>
   /// Compresses a export file
   /// </summary>
   /// <param name="compression">Compression type</param>
   /// <returns>Compressed export file</returns>
   /// <exception cref="NotImplementedException"></exception>
   public ExportFile Compress(FileCompressionType compression)
   {
      return compression switch
      {
         FileCompressionType.None => this,
         FileCompressionType.GZIP => new ExportFile
         {

            FileEncoding = FileEncoding,
            InitialData = InitialData,
            ByteArray = CompressionToGzip(ByteArray),
            FileName = FullName,
            Extension = "gz",
            MimeType = "application/gzip"
         },
         FileCompressionType.ZIP => new ExportFile
         {
            FileEncoding = FileEncoding,
            InitialData = InitialData,
            ByteArray = CompressionToZip(ByteArray, FullName, FileEncoding),
            FileName = FullName,
            Extension = "zip",
            MimeType = "application/zip"
         },
         _ => throw new NotImplementedException(),
      };
   }

   /// <summary>
   /// Creates a ZIP-archive file from a source file
   /// </summary>
   /// <param name="sourceFile">Filename (with the extension) into the archive</param>
   /// <param name="codePage">Encoding ID (default UTF-8)</param>
   /// <returns></returns>
   public static async Task CompressionFileToZipAsync(string sourceFile, int codePage = 65001)
   {
      ArgumentNullException.ThrowIfNull(sourceFile);

      using (var zipFileStream = File.Create(sourceFile + ".zip"))
      {
         using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create, true, Encoding.GetEncoding(codePage)))
         {
            var fileInArchive = archive.CreateEntry(Path.GetFileName(sourceFile), System.IO.Compression.CompressionLevel.Optimal);
            using (var entryStream = fileInArchive.Open())
            {
               using (var sourceStream = File.OpenRead(sourceFile))
               {
                  await sourceStream.CopyToAsync(entryStream);
                  await sourceStream.FlushAsync();
                  await entryStream.FlushAsync();
                  await zipFileStream.FlushAsync();
               }
            }
         }
      }
   }

   /// <summary>
   /// Creates a ZIP-archive file from a source file
   /// </summary>
   /// <param name="sourceFile">Filename (with the extension) into the archive</param>
   /// <returns></returns>
   public static async Task CompressionFileToGzipAsync(string sourceFile)
   {
      ArgumentNullException.ThrowIfNull(sourceFile);

      using (var gzipFileStream = File.Create(sourceFile + ".gzip"))
      {
         using (var gzipStream = new GZipStream(gzipFileStream, System.IO.Compression.CompressionLevel.Optimal))
         {
            using (var sourceStream = File.OpenRead(sourceFile))
            {
               await sourceStream.CopyToAsync(gzipStream);
               await sourceStream.FlushAsync();
               await gzipStream.FlushAsync();
               await gzipFileStream.FlushAsync();
            }
         }  
      }
   }

   /// <summary>
   /// GZIP byte array from the byte array
   /// </summary>
   /// <param name="fileArray">Init byte array</param>
   /// <returns>Byte array</returns>
   private static byte[] CompressionToGzip(byte[] fileArray)
   {
      ArgumentNullException.ThrowIfNull(fileArray, nameof(fileArray));

      using (var outStr = new MemoryStream())
      {
         using (var gzipStream = new GZipStream(outStr, CompressionMode.Compress))
         using (var mStream = new MemoryStream(fileArray))
            mStream.CopyTo(gzipStream);

         return outStr.ToArray();
      }
   }

   /// <summary>
   /// ZIP byte array from the byte array
   /// </summary>
   /// <param name="fileArray">Init byte array</param>
   /// <param name="enc">Encoding</param>
   /// <param name="fileName">Filename (with the extension) into the archive</param>
   /// <returns>Byte array</returns>
   private static byte[] CompressionToZip(byte[] fileArray, string fileName, Encoding enc)
   {
      ArgumentNullException.ThrowIfNull(fileArray, nameof(fileArray));
      if (string.IsNullOrWhiteSpace(fileName))
         throw new ArgumentNullException(nameof(fileName));

      using (var outStr = new MemoryStream())
      {
         using (var zip = new ZipArchive(outStr, ZipArchiveMode.Create, false, enc))
         {
            var fileInArchive = zip.CreateEntry(fileName, System.IO.Compression.CompressionLevel.Optimal);
            using (var entryStream = fileInArchive.Open())
            using (var fileToCompressStream = new MemoryStream(fileArray))
               fileToCompressStream.CopyTo(entryStream);
         }

         return outStr.ToArray();
      }
   }

   /// <summary>
   /// Records the file byte array to the path 
   /// </summary>
   /// <param name="fileArray">File byte array</param>
   /// <param name="path">File path</param>
   /// <param name="fileName">Fiel extension (with the extension)</param>
   public static void SaveStaticFile(byte[] fileArray, string fileName, string path = "content\\files\\exportimport")
   {
      ArgumentNullException.ThrowIfNull(fileArray, nameof(fileArray));
      if (string.IsNullOrWhiteSpace(fileName))
         throw new ArgumentNullException(nameof(fileName));

      var filePath = Path.Combine(path, fileName);
      using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
      fs.Write(fileArray, 0, fileArray.Length);
   }
}