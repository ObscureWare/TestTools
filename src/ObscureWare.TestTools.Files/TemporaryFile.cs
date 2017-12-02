namespace ObscureWare.TestTools.Files
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Extends reference of physical file with automated file deletion during disposal
    /// </summary>
    [PublicAPI]
    public class TemporaryFile : FileRef, IDisposable
    {
        private bool _disposed = false;

        public TemporaryFile([PathReference] string filePath) : base(filePath)
        {
        }

        ~TemporaryFile()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            if (!this._disposed)
            {
                base.Delete();

                this._disposed = true;
            }
        }

        public static TemporaryFile FromManagedFile(FileRef managedFile)
        {
            // TODO: perform validation? Or add similar construction methods?

            return new TemporaryFile(managedFile.FullPath);
        }
    }
}