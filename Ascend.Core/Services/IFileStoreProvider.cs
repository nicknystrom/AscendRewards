using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ascend.Core.Services
{
    public static class AttachmentExtensions
    {
        public static FileType GetFileTypeFromName(string fileName)
        {
            var ext = new FileInfo(fileName).Extension;
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".png":
                case ".bmp":
                    return FileType.Image;
                case ".pdf":
                    return FileType.PDF;
                case ".csv":
                case ".xls":
                case ".xlxs":
                    return FileType.Spreadsheet;
                case ".doc":
                case ".docx":
                case ".txt":
                case ".rtf":
                    return FileType.Document;
                case ".zip":
                case ".7z":
                case ".rar":
                    return FileType.Archive;
                case ".html":
                case ".xhtml":
                case ".htm":
                    return FileType.HTML;
                case ".avi":
                case ".mpeg":
                case ".mpg":
                case ".mkv":
                case ".mov":
                case ".m4v":
                    return FileType.Video;
            }
            return FileType.Unknown;
        }

        public static FileType GetFileTypeFromContent(string contentType)
        {
            switch (contentType)
            {
                case "image/gif":
                case "image/jpeg":
                case "image/pjpeg":
                case "image/png":
                case "image/x-png":
                case "image/bmp":
                    return FileType.Image;
                case "application/pdf":
                    return FileType.PDF;
                case "text/csv":
                case "application/vnd.ms-excel":
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    return FileType.Spreadsheet;
                case "text/html":
                case "application/xhtml+xml":
                    return FileType.HTML;
                case "application/msword":
                    return FileType.Document;
                case "application/zip":
                    return FileType.Archive;
            }
            if (contentType.StartsWith("video/")) return FileType.Video;
            return FileType.Unknown;
        }
    }

    public enum FileType
    {
        Unknown = 0,
        Document,
        Spreadsheet,
        Image,
        Video,
        PDF,
        HTML,
        Archive,
    }

    public interface IFileStoreProvider
    {
        string[] ListFileStores();
        IFileStore GetFileStore(string name, bool create = true);
    }

    public interface IFileStore
    {
        string Name { get; }

        bool Contains(string name);
        ICollection<string> List();
        Tuple<byte[], string> Get(string name);
        void Put(string name, byte[] contents, string type);
        void Delete(string name);
        string GetUrl(string name);
    }
}
