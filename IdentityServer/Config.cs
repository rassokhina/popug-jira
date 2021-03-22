using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

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
                  Password = "admin",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "admin"),
                      new Claim("family_name", "admin"),
                      new Claim("role", "Admin")
                  }
              },
              new TestUser
              {
                  SubjectId = "c95ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "employee",
                  Password = "employee",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "employee"),
                      new Claim("family_name", "employee"),
                      new Claim("role", "Employee")
                  }
              },
               new TestUser
               {
                  SubjectId = "c85ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "manager",
                  Password = "manager",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "manager"),
                      new Claim("family_name", "manager"),
                      new Claim("role", "Manager")
                  }
               },
                new TestUser
               {
                  SubjectId = "c89ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "accountant",
                  Password = "accountant",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "accountant"),
                      new Claim("family_name", "accountant"),
                      new Claim("role", "Accountant")
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
                }
            };
    }
}