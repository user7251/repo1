using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace GitHubUser7251 {
    class BackupGitWorkingCopy_Main {
        static void Main(string[] args) {
            var f = new BackupGitWorkingCopy();
            f.Run ( args ); } 
    }
    class BackupGitWorkingCopy {
        string _repoPathRoot, _backupPathRoot;
        DirectoryInfo _diBackup, _diRepo;
        LinePrefix[] _linePrefixes = new LinePrefix[] { 
            new LinePrefix ( "	modified:   " ), new LinePrefix ( "	new file:   " ) };
        public void Run ( string[] args ) {
            ReadArgs ( args );
            PrepFolders();
            string line;
            while ( ( line = Console.In.ReadLine() ) != null ) RunLine ( line );
        }
        void ReadArgs ( string[] args ) {
            if ( args.Length < 2 ) throw new ArgumentException ( 
                "Missing args.  args.Length{"+args.Length+"}  See the user guide." );
            _repoPathRoot = args[0];
            _backupPathRoot = args[1];
        }
        void PrepFolders() {
            _diRepo = new DirectoryInfo ( _repoPathRoot );
            if ( ! _diRepo.Exists ) throw new Exception("! _diRepo.Exists {" + _repoPathRoot + "}" );
            _diBackup = new DirectoryInfo ( string.Concat ( _backupPathRoot, @"\", DateTime.Now.ToString("yyyy-MM-dd-HHmm") ) );
            if ( ! _diBackup.Exists ) _diBackup.Create();
        }
        void RunLine ( string line ) {
            int i = -1;
            foreach ( var lp in _linePrefixes ) {
                i = lp.GetIndexAfterPrefix ( line );
                if ( i > -1 ) break; }
            if ( i < 0 ) return;
            string subPath = line.Substring ( i );
            string srcPath = string.Concat ( _repoPathRoot, @"\", subPath );
            string targetPath = string.Concat ( _diBackup.FullName, @"\", subPath );
            Directory.CreateDirectory ( Path.GetDirectoryName ( targetPath ) );
            File.Copy ( srcPath, targetPath, overwrite: true );
        }
    }
    class LinePrefix {
        string _prefix;
        public LinePrefix ( string prefix ) { _prefix = prefix; }
        public int GetIndexAfterPrefix ( string line ) {
            int i = line.IndexOf ( _prefix );
            if ( i < 0 ) return i;
            return i + _prefix.Length;
        }
    }
}
