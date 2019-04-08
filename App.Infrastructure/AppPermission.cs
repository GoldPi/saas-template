using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace App.Infrastructure
{
    public sealed class AppPermission
    {
        public AppPermission()
        {

        }
        public AppPermission(string name, string value, string groupName, string description = null)
        {
            Name = name;
            Value = value;
            GroupName = groupName;
            Description = description;
        }
        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Value;
        }


        public static implicit operator string(AppPermission permission)
        {
            return permission.Value;
        }
    }

    public static class ApplicationPermissions
    {
        public static ReadOnlyCollection<AppPermission> AllPermissions;


        public const string UsersPermissionGroupName = "User Permissions";
        public static AppPermission ViewUsers = new AppPermission("View Users", "users.view", UsersPermissionGroupName, "Permission to view other users account details");
        public static AppPermission ManageUsers = new AppPermission("Manage Users", "users.manage", UsersPermissionGroupName, "Permission to create, delete and modify other users account details");

        public const string RolesPermissionGroupName = "Role Permissions";
        public static AppPermission ViewRoles = new AppPermission("View Roles", "roles.view", RolesPermissionGroupName, "Permission to view available roles");
        public static AppPermission ManageRoles = new AppPermission("Manage Roles", "roles.manage", RolesPermissionGroupName, "Permission to create, delete and modify roles");
        public static AppPermission AssignRoles = new AppPermission("Assign Roles", "roles.assign", RolesPermissionGroupName, "Permission to assign roles to users");


        static ApplicationPermissions()
        {
            List<AppPermission> allPermissions = new List<AppPermission>()
            {
                ViewUsers,
                ManageUsers,

                ViewRoles,
                ManageRoles,
                AssignRoles
            };

            AllPermissions = allPermissions.AsReadOnly();
        }

        public static AppPermission GetPermissionByName(string permissionName)
        {
            return AllPermissions.Where(p => p.Name == permissionName).SingleOrDefault();
        }

        public static AppPermission GetPermissionByValue(string permissionValue)
        {
            return AllPermissions.Where(p => p.Value == permissionValue).SingleOrDefault();
        }

        public static string[] GetAllPermissionValues()
        {
            return AllPermissions.Select(p => p.Value).ToArray();
        }

        public static string[] GetAdministrativePermissionValues()
        {
            return new string[] { ManageUsers, ManageRoles, AssignRoles };
        }
    }
}
