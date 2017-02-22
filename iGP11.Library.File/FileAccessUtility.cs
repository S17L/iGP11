using System;
using System.Security;
using System.Security.Permissions;

namespace iGP11.Library.File
{
    internal class FileAccessUtility
    {
        public static bool HasAccess(string filePath, FileIOPermissionAccess permissionAccess)
        {
            var permissionSet = new PermissionSet(PermissionState.None);
            var permission = new FileIOPermission(permissionAccess, filePath);
            permissionSet.AddPermission(permission);

            return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }
    }
}