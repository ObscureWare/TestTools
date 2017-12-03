namespace ObscureWare.TestTools.Files
{
    using System;
    using System.IO;

    using JetBrains.Annotations;

    [PublicAPIAttribute]
    public static class IoHelpers
    {
        private static readonly Random Rnd = new Random();

        public static TemporaryFile CreateTemporaryFile(int fileSize = 0, string folderPath = "")
        {
            return TemporaryFile.FromManagedFile(CreateManagedTemporaryFile(fileSize, folderPath));
        }

        /// <summary>
        /// Changes files size. The content of the new file is random bytes.
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileRef OverwriteFile(int fileSize, [PathReference] string filePath)
        {
            FileStream streamWriter = null;
            try
            {
                var fileRef = FileRef.Create(filePath);
                streamWriter = fileRef.OpenStreamWriter();

                WriteRandomData(fileSize, streamWriter);

                return fileRef;
            }
            finally
            {
                // this stream is no longer needed - we keep only file path later
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates files with a specific file size (random file content). Ensures the path exists.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        [MustUseReturnValue]
        public static FileRef CreateFile([PathReference] string filePath, int fileSize)
        {
            FileStream streamWriter = null;
            try
            {
                var path = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(path);

                streamWriter = File.Create(filePath);

                WriteRandomData(fileSize, streamWriter);
                return FileRef.Create(filePath);
            }
            finally
            {
                // this stream is no longer needed - we keep only file path later
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        [MustUseReturnValue]
        public static FileRef CreateManagedTemporaryFile(int fileSize, [PathReference]  string folderPath)
        {
            FileStream streamWriter = null;
            try
            {
                var filePath = GetPath(folderPath);
                var fileRef = FileRef.Create(filePath);

                if (folderPath != string.Empty)
                {
                    // GetRandomFilePath unlike GetTempFileName does not create a file. We need to create and use its stream.
                    streamWriter = File.Create(filePath);
                }
                else
                {
                    // open stream to existing file
                    streamWriter = fileRef.OpenStreamWriter();
                }

                WriteRandomData(fileSize, streamWriter);
                return fileRef;
            }
            finally
            {
                // this stream is no longer needed - we keep only file path later
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
        }

        private static string GetPath([PathReference] string folderPath)
        {
            string filePath;
            if (folderPath != string.Empty)
            {
                filePath = Path.Combine(folderPath, Path.GetRandomFileName());
            }
            else
            {
                filePath = Path.GetTempFileName();
            }

            filePath = filePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return filePath;
        }

        private static void WriteRandomData(int fileSize, FileStream sw)
        {
            if (fileSize > 0)
            {
                for (int i = 0; i < fileSize; i++)
                {
                    sw.WriteByte((byte)Rnd.Next(0, 256));
                }

                sw.Flush();
                sw.Close();
            }
        }

        private const int BYTES_TO_READ = sizeof(Int64);

        /// <summary>
        /// Compares whether two files are equal. Fast.
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns>TRUE if files are equal, or both paths point to a same file.</returns>
        [MustUseReturnValue]
        public static bool CompareFiles([PathReference] string path1, [PathReference] string path2)
        {
            if (string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase))
            {
                return true; // hehe
            }

            FileInfo f1 = new FileInfo(path1);
            FileInfo f2 = new FileInfo(path2);

            if (f1.FullName.Equals(f2.FullName))
            {
                return true; // just to make sure, including virtual paths this time
            }

            if (f1.Length != f2.Length)
            {
                return false; // simple as that
            }

            int iterations = (int)Math.Ceiling((double)f1.Length / BYTES_TO_READ);

            using (FileStream fs1 = f1.OpenRead())
            {
                using (FileStream fs2 = f2.OpenRead())
                {
                    byte[] one = new byte[BYTES_TO_READ];
                    byte[] two = new byte[BYTES_TO_READ];

                    for (int i = 0; i < iterations; i++)
                    {
                        fs1.Read(one, 0, BYTES_TO_READ);
                        fs2.Read(two, 0, BYTES_TO_READ);

                        if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}