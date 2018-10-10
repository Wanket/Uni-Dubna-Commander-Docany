using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using DokanNet;
using FileAccess = DokanNet.FileAccess;

namespace Uni_Dubna_Commander
{
    public class DokanOperations : IDokanOperations
    {
        private readonly Login _login;

        private readonly FileSystem _fileSystem;

        public DokanOperations(Login login)
        {
            _login = login;
            _fileSystem = new FileSystem(_login);
        }

        public NtStatus CreateFile(string fileName, FileAccess access, FileShare share, FileMode mode,
            FileOptions options, FileAttributes attributes, DokanFileInfo info)
        {
            if (new Item(fileName).SplitPath.Last() == "desktop.ini")
            {
                return DokanResult.AccessDenied;
            }

            try
            {
                info.IsDirectory = _fileSystem.Get(new Item(fileName)) is Dir;

                return DokanResult.Success;
            }
            catch (ArgumentException)
            {
                return info.IsDirectory ? DokanResult.PathNotFound : DokanResult.FileNotFound;
            }
        }

        public void Cleanup(string fileName, DokanFileInfo info)
        {
        }

        public void CloseFile(string fileName, DokanFileInfo info)
        {
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, DokanFileInfo info)
        {
            try
            {
                var stream = ((File) _fileSystem.Get(new Item(fileName))).GetMemoryStream(_login);
                stream.Position = offset;
                bytesRead = stream.Read(buffer, 0, buffer.Length);

                return DokanResult.Success;
            }
            catch (Exception)
            {
                bytesRead = 0;
                return DokanResult.FileNotFound;
            }
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, DokanFileInfo info)
        {
            bytesWritten = 0;
            return DokanResult.AccessDenied;
        }

        public NtStatus FlushFileBuffers(string fileName, DokanFileInfo info) => DokanResult.AccessDenied;

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, DokanFileInfo info)
        {
            try
            {
                fileInfo = _fileSystem.Get(new Item(fileName)).FileInformation;
                return DokanResult.Success;
            }
            catch (ArgumentException)
            {
                fileInfo = new FileInformation();
                return info.IsDirectory ? DokanResult.PathNotFound : DokanResult.FileNotFound;
            }
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, DokanFileInfo info)
        {
            try
            {
                var dir = ((Dir) _fileSystem.Get(new Item(fileName)));
                if (dir.Childs == null)
                {
                    _fileSystem.UpdateChilds(dir);
                }

                files = new List<FileInformation>();

                foreach (var keyValuePair in dir.Childs)
                {
                    files.Add(keyValuePair.Value.FileInformation);
                }

                return DokanResult.Success;
            }
            catch (Exception)
            {
                files = null;
                return DokanResult.PathNotFound;
            }
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files,
            DokanFileInfo info)
        {
            files = null;
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, DokanFileInfo info) =>
            DokanResult.AccessDenied;

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime,
            DateTime? lastWriteTime, DokanFileInfo info) => DokanResult.AccessDenied;

        public NtStatus DeleteFile(string fileName, DokanFileInfo info) => DokanResult.AccessDenied;

        public NtStatus DeleteDirectory(string fileName, DokanFileInfo info) => DokanResult.AccessDenied;

        public NtStatus MoveFile(string oldName, string newName, bool replace, DokanFileInfo info) =>
            DokanResult.AccessDenied;

        public NtStatus SetEndOfFile(string fileName, long length, DokanFileInfo info) => DokanResult.AccessDenied;

        public NtStatus SetAllocationSize(string fileName, long length, DokanFileInfo info) => DokanResult.AccessDenied;

        public NtStatus LockFile(string fileName, long offset, long length, DokanFileInfo info) =>
            DokanResult.AccessDenied;

        public NtStatus UnlockFile(string fileName, long offset, long length, DokanFileInfo info) =>
            DokanResult.AccessDenied;

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes,
            out long totalNumberOfFreeBytes, DokanFileInfo info)
        {
            freeBytesAvailable = 0;
            totalNumberOfBytes = 0;
            totalNumberOfFreeBytes = 0;
            return DokanResult.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features,
            out string fileSystemName, out uint maximumComponentLength, DokanFileInfo info)
        {
            volumeLabel = "Uni-Dubna";
            features = FileSystemFeatures.ReadOnlyVolume;
            fileSystemName = "dubnafs";
            maximumComponentLength = 0;
            return DokanResult.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security,
            AccessControlSections sections, DokanFileInfo info)
        {
            security = null;
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections,
            DokanFileInfo info) => DokanResult.AccessDenied;

        public NtStatus Mounted(DokanFileInfo info) => DokanResult.Success;

        public NtStatus Unmounted(DokanFileInfo info) => DokanResult.Success;

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, DokanFileInfo info)
        {
            streams = null;
            return DokanResult.NotImplemented;
        }
    }
}