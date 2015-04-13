using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#if _
- This console app backs-up changed files and added files in a Git working copy.
- The next version of this app could allow the user to run "git status | GitWorkingCopyBackup.exe".
- Motivation: Git does not have a way to backup changed files and added files in a working copy.  Many people use 
    "git commit" and "git push", but that pollutes the repo with backups.  Another option is to use "git diff", but 
    it has an issue with Visual Studio 2010 (VS).  Sometimes, VS saves C# files as UTF-8, sometimes UTF-16LE.  When 
    VS switches a file from one to the other, the "git diff" patch file says, "Binary files a/path1/file1.cs and 
    b/path1/file1.cs differ," and "git apply" fails to apply the patch.
- To backup:
    - Configure the paths in the BackupGitWorkingCopy class.
    - In "git bash," run "git status > git-status.txt"
    - Run this app.
- To restore, use Windows Explorer to copy (merge) the backup folder into the local git repo folder.
#endif
namespace GitHubUser7251 {
    class BackupGitWorkingCopy_Main {
        static void Main(string[] args) {
            var f = new BackupGitWorkingCopy();
            f.Run(); } 
    }
    class BackupGitWorkingCopy {
        // configure:
        const string GIT_STATUS_OUTPUT_FILE_PATH = @"C:\git-repo\git-status.txt";
        const string REPO_PATH_ROOT = @"C:\git-repo";
        const string BACKUP_PATH_ROOT = @"C:\backup";
        //
        DirectoryInfo _diBackup, _diRepo;
        LinePrefix[] _linePrefixes = new LinePrefix[] { 
            new LinePrefix ( "	modified:   " ), new LinePrefix ( "	new file:   " ) };
        public void Run() {
            PrepFolders();
            var f = new FileInfo ( GIT_STATUS_OUTPUT_FILE_PATH );
            var fs = f.Open ( FileMode.Open, FileAccess.Read );
            StreamReader sr = new StreamReader(fs);
            string line;
            while ((line = sr.ReadLine()) != null) RunLine ( line );
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
