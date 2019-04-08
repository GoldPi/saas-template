using App.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure
{
    public interface IAccountManager<Tu,Tr> where Tu : AppUser where Tr : AppRole
    {

        Task<bool> CheckPasswordAsync(Tu user, string password);
        Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(Tr role, IEnumerable<string> claims);
        Task<(bool Succeeded, string[] Errors)> CreateUserAsync(Tu user, IEnumerable<string> roles, string password);
        Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(Tr role);
        Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string roleName);
        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(Tu user);
        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId);
        Task<Tr> GetRoleByIdAsync(string roleId);
        Task<Tr> GetRoleByNameAsync(string roleName);
        Task<Tr> GetRoleLoadRelatedAsync(string roleName);
        Task<List<Tr>> GetRolesLoadRelatedAsync(int page, int pageSize);
        Task<(Tu User, string[] Roles)?> GetUserAndRolesAsync(string userId);
        Task<Tu> GetUserByEmailAsync(string email);
        Task<Tu> GetUserByIdAsync(string userId);
        Task<Tu> GetUserByUserNameAsync(string userName);
        Task<IList<string>> GetUserRolesAsync(Tu user);
        Task<List<(Tu User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize);
        Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(Tu user, string newPassword);
        Task<bool> TestCanDeleteRoleAsync(string roleId);
        Task<bool> TestCanDeleteUserAsync(string userId);
        Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(Tu user, string currentPassword, string newPassword);
        Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(Tr role, IEnumerable<string> claims);
        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(Tu user);
        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(Tu user, IEnumerable<string> roles);
    }
    public class AccountManager<Tu,Tr> : IAccountManager<Tu,Tr> where Tu: AppUser where Tr : AppRole
    {
        private readonly AppDbContext<Tu,Tr> _context;
        private readonly UserManager<Tu> _userManager;
        private readonly RoleManager<Tr> _roleManager;


        public AccountManager(
            AppDbContext<Tu,Tr> context,
            UserManager<Tu> userManager,
            RoleManager<Tr> roleManager
            )
        {
            _context = context;
           
            _userManager = userManager;
            _roleManager = roleManager;

        }

        public AccountManager()
        {
        }

        public async Task<Tu> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<Tu> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<Tu> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetUserRolesAsync(Tu user)
        {
            return await _userManager.GetRolesAsync(user);
        }


        public async Task<(Tu User, string[] Roles)?> GetUserAndRolesAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArrayAsync();

            return (user, roles);
        }


        public async Task<List<(Tu User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize)
        {
            IQueryable<Tu> usersQuery = _context.Users
                .Include(u => u.Roles)
                .OrderBy(u => u.UserName);

            if (page != -1)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                usersQuery = usersQuery.Take(pageSize);

            var users = await usersQuery.ToListAsync();

            var userRoleIds = users.SelectMany(u => u.Roles.Select(r => r.RoleId)).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();

            return users
                .Select(u => (u, roles.Where(r => u.Roles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name).ToArray()))
                .ToList();
        }


        public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(Tu user, IEnumerable<string> roles, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            user = await _userManager.FindByNameAsync(user.UserName);

            try
            {
                result = await this._userManager.AddToRolesAsync(user, roles.Distinct());
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(Tu user)
        {
            return await UpdateUserAsync(user, null);
        }


        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(Tu user, IEnumerable<string> roles)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            if (roles != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var rolesToRemove = userRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Any())
                {
                    result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Any())
                {
                    result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(Tu user, string newPassword)
        {
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(Tu user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, new string[] { });
        }

        public async Task<bool> CheckPasswordAsync(Tu user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if (!_userManager.SupportsUserLockout)
                    await _userManager.AccessFailedAsync(user);

                return false;
            }

            return true;
        }


        public async Task<bool> TestCanDeleteUserAsync(string userId)
        {
            //if (await _context.Orders.Where(o => o.CashierId == userId).AnyAsync())
            //    return false;

            //canDelete = !await ; //Do other tests...

            return true;
        }


        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
                return await DeleteUserAsync(user);

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(Tu user)
        {
            var result = await _userManager.DeleteAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }






        public async Task<Tr> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }


        public async Task<Tr> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }


        public async Task<Tr> GetRoleLoadRelatedAsync(string roleName)
        {
            var role = await _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.Users)
                .Where(r => r.Name == roleName)
                .SingleOrDefaultAsync();

            return role;
        }


        public async Task<List<Tr>> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<Tr> rolesQuery = _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.Users)
                .OrderBy(r => r.Name);

            if (page != -1)
                rolesQuery = rolesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                rolesQuery = rolesQuery.Take(pageSize);

            var roles = await rolesQuery.ToListAsync();

            return roles;
        }


        public async Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(Tr role, IEnumerable<string> claims)
        {
            if (claims == null)
                claims = new string[] { };

            string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
            if (invalidClaims.Any())
                return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });


            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            role = await _roleManager.FindByNameAsync(role.Name);

            foreach (string claim in claims.Distinct())
            {
                result = await this._roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));

                if (!result.Succeeded)
                {
                    await DeleteRoleAsync(role);
                    return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(Tr role, IEnumerable<string> claims)
        {
            if (claims != null)
            {
                string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
                if (invalidClaims.Any())
                    return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
            }


            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            if (claims != null)
            {
                var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == ClaimConstants.Permission);
                var roleClaimValues = roleClaims.Select(c => c.Value).ToArray();

                var claimsToRemove = roleClaimValues.Except(claims).ToArray();
                var claimsToAdd = claims.Except(roleClaimValues).Distinct().ToArray();

                if (claimsToRemove.Any())
                {
                    foreach (string claim in claimsToRemove)
                    {
                        result = await _roleManager.RemoveClaimAsync(role, roleClaims.Where(c => c.Value == claim).FirstOrDefault());
                        if (!result.Succeeded)
                            return (false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }

                if (claimsToAdd.Any())
                {
                    foreach (string claim in claimsToAdd)
                    {
                        result = await _roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));
                        if (!result.Succeeded)
                            return (false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }
            }

            return (true, new string[] { });
        }


        public async Task<bool> TestCanDeleteRoleAsync(string roleId)
        {
            return !await _context.UserRoles.Where(r => r.RoleId == roleId).AnyAsync();
        }


        public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
                return await DeleteRoleAsync(role);

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(Tr role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

    }
}
