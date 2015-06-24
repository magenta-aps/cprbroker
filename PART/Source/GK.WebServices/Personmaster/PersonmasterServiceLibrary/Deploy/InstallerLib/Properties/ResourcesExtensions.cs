using System;

namespace PersonMasterInstallers.Properties
{
    public static class ResourcesExtensions
    {
        public static string AllStoredProceduresSQL
        {
            get
            {
                var arr = new string[]{
                    Resources.spGK_PM_GetObjectIDsFromCPRArray
                };

                return string.Join(
                    Environment.NewLine + "GO" + Environment.NewLine,
                    arr);
            }
        }
    }
}