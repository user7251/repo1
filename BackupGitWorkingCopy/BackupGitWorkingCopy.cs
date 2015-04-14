using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace GitHubUser7251 {
    class BackupGitWorkingCopy_Main {
        static void Main(string[] args) {
            var f = new BackupGitWorkingCopy();
            f.Run(); } 
    }
    class BackupGitWorkingCopy {
        // configure:
        const string REPO_PATH_ROOT = @"C:\git-repo";
        const string BACKUP_PATH_ROOT = @"C:\backup";
        //
        DirectoryInfo _diBackup, _diRepo;
        LinePrefix[] _linePrefixes = new LinePrefix[] { 
            new LinePrefix ( "	modified:   " ), new LinePrefix ( "	new file:   " ) };
        public void Run() {
            PrepFolders();
            string line;
            while ( ( line = Console.In.ReadLine() ) != null ) RunLine ( line );
        }
        void PrepFolders() {
            _diRepo = new DirectoryInfo ( REPO_PATH_ROOT );
            if ( ! _diRepo.Exists ) throw new Exception("! _diRepo.Exists");
            _diBackup = new DirectoryInfo ( string.Concat ( BACKUP_PATH_ROOT, @"\", DateTime.Now.ToString("yyyy-MM-dd-HHmm") ) );
            if ( ! _diBackup.Exists ) _diBackup.Create();
        }
        void RunLine ( string line ) {
            int i = -1;
            foreach ( var lp in _linePrefixes ) {
                i = lp.GetIndexAfterPrefix ( line );
                if ( i > -1 ) break; }
            if ( i < 0 ) return;
            string subPath = line.Substring ( i );
            string srcPath = string.Concat ( REPO_PATH_ROOT, @"\", subPath );
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
