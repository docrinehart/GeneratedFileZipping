namespace GeneratedFileZipping.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public FileResult XmlExport()
        {
            // Do the work to generate your data into models with whatever meta info you need
            var myGeneratedFiles = new List<XmlFileModel>
            {
                XmlFileBuilder.MakeFile("myFile1.xml"),
                XmlFileBuilder.MakeFile("myFile2.xml"),
                XmlFileBuilder.MakeFile("myFile3.xml")
            };

            byte[] zipFileBytes; // Prepare byte array needed to stream the zip file to the client
            using (var memoryStream = new MemoryStream())
            {
                // Create a new zip archive in memory
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var fileModel in myGeneratedFiles)
                    {
                        // Create a new "file" in the archive
                        var file = archive.CreateEntry(fileModel.FileName);

                        // Open file stream and write the xml into it
                        using (var entryStream = file.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.Write(fileModel.XmlString);
                        }
                    }
                }

                // After writing each generated file into the archive, write the archive bytes to memory
                zipFileBytes = memoryStream.ToArray();
            }

            // Build up the FileResult for MVC to stream the zip file to the client as a download
            var result = new FileContentResult(zipFileBytes, "application/xml")
            {
                FileDownloadName = "MyZipFileName.zip"
            };

            return result;
        }
    }

    public class XmlFileModel
    {
        public string FileName { get; set; }
        public string XmlString { get; set; }

        public XmlFileModel(string fileName)
        {
            FileName = fileName;
            XmlString = $"<MyDoc><FileName>{fileName}</FileName></MyDoc>";
        }
    }

    public static class XmlFileBuilder
    {
        public static XmlFileModel MakeFile(string fileName)
        {
            return new XmlFileModel(fileName);
        }
    }
}