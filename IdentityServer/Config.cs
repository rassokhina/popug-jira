using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

using IdentityModel;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new IdentityResource[]
            {
                  new IdentityResources.OpenId(),
                  new IdentityResources.Profile(),
                  new IdentityResource("roles", "User role(s)", new List<string> { "role" })
            };

        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
            };

        public static IEnumerable<ApiResource> GetApiResources() =>
           new List<ApiResource>
           {
           };

        public static List<TestUser> GetUsers() =>
          new List<TestUser>
          {
              new TestUser
              {
                  SubjectId = "a9ea0f25-b964-409f-bcce-c923266249b4",
                  Username = "admin",
                  Password = "Password123!",
                  Claims = new List<Claim>
                  {
                      new Claim(JwtClaimTypes.Role, "Admin")
                  }
              },
              new TestUser
              {
                  SubjectId = "c95ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "employee",
                  Password = "Password123!",
                  Claims = new List<Claim>
                  {
                      new Claim(JwtClaimTypes.Role, "Employee")
                  }
              },
               new TestUser
               {
                  SubjectId = "c85ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "manager",
                  Password = "Password123!",
                  Claims = new List<Claim>
                  {
                      new Claim(JwtClaimTypes.Role, "Manager")
                  }
               },
                new TestUser
               {
                  SubjectId = "c89ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "accountant",
                  Password = "Password123!",
                  Claims = new List<Claim>
                  {
                      new Claim(JwtClaimTypes.Role, "Accountant")
                  }
               },
          };


        public static IEnumerable<Client> GetClients() =>
            new Client[] 
            { 
                new Client
                {
                    ClientId = "TaskTracker.Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    RequirePkce = false,
                    AllowOfflineAccess = true,
                    RequireConsent = true,
                    AllowedScopes = new List<string>
                    {
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       "roles"
                    }
                },
                new Client
                {
                    ClientId = "Accounting.Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5003/signin-oidc" },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },
                    RequirePkce = false,
                    AllowOfflineAccess = true,
                    RequireConsent = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles"
                    }
                },
                new Client
                {
                    ClientId = "Analytics.Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5004/signin-oidc" },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5004/signout-callback-oidc" },
                    RequirePkce = false,
                    AllowOfflineAccess = true,
                    RequireConsent = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles"
                    }
                }
            };
    }
}