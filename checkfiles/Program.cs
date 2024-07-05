// Check OpenEdge source files for inconsistent line endings
// Credits to Raymond Chen https://devblogs.microsoft.com/oldnewthing/20190422-00/?p=102433 for the initial program
// This program is currently very rough with hardcoded parameters for my particular needs

using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static IEnumerable<FileInfo> EnumerateFiles(string dir)
    {
        var info = new System.IO.DirectoryInfo(dir);
        foreach (var f in info.EnumerateFileSystemInfos(
                           "*.*", SearchOption.AllDirectories))
        {
            if (f.Attributes.HasFlag(FileAttributes.Hidden))
            {
                continue;
            }

            if (f.Attributes.HasFlag(FileAttributes.Directory))
            {
                switch (f.Name.ToLower())
                {
                    case "bin":
                    case "obj":
                        continue;
                }

                foreach (var inner in EnumerateFiles(f.FullName))
                {
                    yield return inner;
                }
            }
            else
            {
                yield return (FileInfo)f;
            }
        }
    }

    // Starting in the current directory, enumerate files
    // (see EnumerateFiles for criteria), and report what
    // type of line ending each file uses.

    static void Main()
    {
        var Verbose = true;
        foreach (var f in EnumerateFiles("."))
        {
            // Skip obvious binary files.
            switch (f.Extension.ToLower())
            {
                case ".p":
                case ".i":
                case ".w":
                case ".cls":
                case ".wx":
                case ".df":
                    break;
                default:
                    continue;
            }

            int line = 0; // total number of lines found
            int cr = 0;   // number of lines that end in CR
            int crlf = 0; // number of lines that end in CRLF
            int lf = 0;   // number of lines that end in LF

            var stream = new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
            using (var br = new BinaryReader(stream))
            {
                // Slurp the entire file into memory.
                var bytes = br.ReadBytes((int)f.Length);
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == '\r')
                    {
                        if (i + 1 < bytes.Length &&
                            bytes[i + 1] == '\n')
                        {
                            line++;
                            crlf++;
                            i++;
                        }
                        else
                        {
                            line++;
                            cr++;
                        }
                    }
                    else if (bytes[i] == '\n')
                    {
                        lf++;
                        line++;
                    }
                }
            }

            if (cr == line)
            {
                if (Verbose) Console.WriteLine("{0}, {1}", f.FullName, "CR");
            }
            else if (crlf == line)
            {
                if (Verbose) Console.WriteLine("{0}, {1}", f.FullName, "CRLF");
            }
            else if (lf == line)
            {
                if (Verbose) Console.WriteLine("{0}, {1}", f.FullName, "LF");
            }
            else
            {
                if (Verbose) Console.WriteLine("{0}, {1}, {2}, {3}, {4}", f.FullName, "Mixed", cr, lf, crlf);
                else Console.WriteLine("{0}", f.FullName);
            }
        }
    }
}