using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemaGeneration
{
    public class TempCurrentDirectoryContext : IDisposable
    {
        private string OriginalDirexctory;
        public TempCurrentDirectoryContext()
        {
            OriginalDirexctory = Environment.CurrentDirectory;
        }

        public TempCurrentDirectoryContext(string newDir)
            : this()
        {
            Environment.CurrentDirectory = newDir;
        }

        public void Dispose()
        {
            Environment.CurrentDirectory = OriginalDirexctory;
        }
    }
}
