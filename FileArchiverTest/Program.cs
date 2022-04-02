using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiverTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FileProcessing fileProcessing = new FileProcessing();
            fileProcessing.ArchiveAndRemoveFiles();

		}

	}
}
