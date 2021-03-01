using System;

namespace YouTubeCLI.Libraries{
    public abstract class LibraryBase {
        internal string _directory;

        public LibraryBase() {
            this._directory  = Environment.CurrentDirectory;
        }
    }
}