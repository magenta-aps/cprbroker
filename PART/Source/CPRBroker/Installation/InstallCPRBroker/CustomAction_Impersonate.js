// Usage: CustomAction_NoImpersonate.js <msi-file>
// Performs a post-build fixup of an MSI to change all
// deferred custom actions to include NoImpersonate

// Constant values from Windows Installer
var msiOpenDatabaseModeTransact = 1;

var msiViewModifyInsert = 1
var msiViewModifyUpdate = 2
var msiViewModifyAssign = 3
var msiViewModifyReplace = 4
var msiViewModifyDelete = 6

var msidbCustomActionTypeInScript = 0x00000400;
var msidbCustomActionTypeNoImpersonate = 0x00000800

if (WScript.Arguments.Length != 1) {
    WScript.StdErr.WriteLine(WScript.ScriptName + " file");
    WScript.Quit(1);
}

var filespec = WScript.Arguments(0);
var installer = WScript.CreateObject("WindowsInstaller.Installer");
var database = installer.OpenDatabase(filespec, msiOpenDatabaseModeTransact);

var sql
var view
var record

try {

    var dllFileName = "CPRBroker.SetupDatabase.dll";
    var dllId = null;

    // Select all files
    fileSql = "SELECT `File`,`FileName` FROM `File` ";
    fileView = database.OpenView(fileSql);
    fileView.Execute();
    fileRecord = fileView.Fetch();


    // Loop over files
    while (fileRecord) {
        var fileName = fileRecord.StringData(2);

        // If the file name is the one we are looking for
        if (fileName.indexOf(dllFileName, 0) > -1) {

            fileId = fileRecord.StringData(1);
            WScript.echo("Searching for actions of : " + fileName + "   With ID : " + fileId);
            // Select custom actions
            customActionSql = "SELECT `Action`, `Type`, `Source`, `Target` FROM `CustomAction` Order BY `Action`"
            customActionView = database.OpenView(customActionSql);
            customActionView.Execute();
            customActionRecord = customActionView.Fetch();

            var previousCustomActionRecord = null;

            while (customActionRecord) {
                var target = customActionRecord.StringData(4);

                // if custom action target contains file id
                if (target.indexOf(fileId, 0) > -1) {
                    WScript.echo("Custom action match: " + target);                    

                    if (previousCustomActionRecord.IntegerData(2) & msidbCustomActionTypeInScript) {
                        WScript.echo("Updating :" + previousCustomActionRecord.StringData(1));
                        previousCustomActionRecord.IntegerData(2) = previousCustomActionRecord.IntegerData(2) ^ msidbCustomActionTypeNoImpersonate;
                        customActionView.Modify(msiViewModifyAssign, previousCustomActionRecord);
                    }
                }
                previousCustomActionRecord = customActionRecord;
                customActionRecord = customActionView.Fetch();
            }
            customActionView.Close();
        }
        fileRecord = fileView.Fetch();
    }
    fileView.Close();
    database.Commit();
}
catch (e) {
    WScript.StdErr.WriteLine(e);
    WScript.Quit(1);
}