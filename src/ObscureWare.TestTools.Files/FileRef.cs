namespace ObscureWare.TestTools.Files
{
    using System;
    using System.IO;
    using System.Linq;

    using Conditions;

    using JetBrains.Annotations;

    /// <summary>
    /// Object that contains file reference as abject
    /// </summary>
    [CannotApplyEqualityOperator]
    [PublicAPI]
    public class FileRef
    {
        private static readonly char[] InvalidNameCharacters = Path.GetInvalidFileNameChars();
        private static readonly char[] InvalidPathCharacters = Path.GetInvalidPathChars();


        protected FileRef([PathReference] string filePath)
        {
            filePath.Requires(nameof(filePath)).IsNotNullOrWhiteSpace();

            this.FullPath = filePath;
            this.Name = Path.GetFileName(filePath);
        }

        /// <summary>
        /// Full path to the file
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Full file's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// File's name without extension. (And dot.)
        /// </summary>
        public string SimpleName => Path.GetFileNameWithoutExtension(this.Name);

        /// <summary>
        /// Gets extension of the file. (With dot.)
        /// </summary>
        public string Extension => Path.GetExtension(this.Name);

        /// <summary>
        /// Gets true if file exists
        /// </summary>
        public bool Exists => File.Exists(this.FullPath);

        /// <summary>
        /// Gets true if name and path of the file consist of proper characters
        /// </summary>
        public bool FilePathIsValid => !this.FullPath.ToCharArray().Intersect(InvalidPathCharacters).Any();

        /// <summary>
        /// Gets true if name of the file consist of proper characters only
        /// </summary>
        public bool FileNameIsValid => !this.Name.ToCharArray().Intersect(InvalidNameCharacters).Any();

        /// <summary>
        /// Opens temporary stream writer to access the file
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [MustUseReturnValue]
        public FileStream OpenStreamWriter()
        {
            return new FileStream(this.FullPath, FileMode.OpenOrCreate, FileAccess.Write);
        }

        /// <summary>
        /// Opens temporary stream writer to access the file
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [MustUseReturnValue]
        public FileStream OpenStreamReader()
        {
            return new FileStream(this.FullPath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Deletes file, physically
        /// </summary>
        /// <remarks>This might fail if file is still in use. This is fine - that indicates something wrong with tested code (or test itself...)</remarks>
        public void Delete()
        {
            if (File.Exists(this.FullPath))
            {
                File.Delete(this.FullPath);
            }
        }

        /// <summary>
        /// Creates instance of FileRef from absolute path. If virtual one has been specified - exception will be thrown instantly.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [MustUseReturnValue]
        public static FileRef CreateAbsolute([PathReference] string filePath)
        {
            filePath.Requires(nameof(filePath)).IsNotNullOrWhiteSpace();

            if (!Path.IsPathRooted(filePath))
            {
                throw new ArgumentException($"Received virtual path where absolute one was expected: '{filePath}'", nameof(filePath));
            }

            return new FileRef(filePath);
        }

        /// <summary>
        /// Creates instance of FileRef from virtual path. If absolute one  has been specified - exception will be thrown instantly.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [MustUseReturnValue]
        public static FileRef CreateVirtual([PathReference] string filePath)
        {
            filePath.Requires(nameof(filePath)).IsNotNullOrWhiteSpace();

            if (Path.IsPathRooted(filePath))
            {
                throw new ArgumentException($"Received absolute path where virtual one was expected: '{filePath}'", nameof(filePath));
            }

            filePath = Path.GetFullPath(filePath);

            return new FileRef(filePath);
        }

        /// <summary>
        /// Creates instance of FileRef from path of unknown type - absolute or virtual
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [MustUseReturnValue]
        public static FileRef Create([PathReference] string filePath)
        {
            filePath.Requires(nameof(filePath)).IsNotNullOrWhiteSpace();

            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.GetFullPath(filePath);
            }

            return new FileRef(filePath);
        }
    }
}
