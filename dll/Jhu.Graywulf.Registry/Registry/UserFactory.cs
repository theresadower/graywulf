﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Jhu.Graywulf.Registry
{
    public class UserFactory : ContextObject
    {
        /// <summary>
        /// Creates an object with a valid context.
        /// </summary>
        /// <param name="context">A valid context object.</param>
        public UserFactory(Context context)
            : base(context)
        {
        }

        #region User authentication

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public User LoginUser(Domain domain, string nameOrEmail, string password)
        {
            var user = new User(Context);

            // Load user from the database
            string sql = "spLoginUser";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier).Value = domain.Guid;
                cmd.Parameters.Add("@NameOrEmail", SqlDbType.NVarChar, 50).Value = nameOrEmail;

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        throw new EntityNotFoundException(ExceptionMessages.LoginFailed);
                    }

                    user.LoadFromDataReader(dr);
                }
            }

            // Compute password hash
            bool hashok = true;
            byte[] hash = User.ComputePasswordHash(password);

            // Compare the hash with the one in the database
            if (hash.Length != user.PasswordHash.Length)
            {
                hashok = false;
            }
            else
            {
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != user.PasswordHash[i])
                    {
                        hashok = false;
                        break;
                    }
                }
            }

            if (!hashok)
            {
                throw new SecurityException(ExceptionMessages.LoginFailed);
            }

            // Update context
            Context.UserGuid = user.Guid;
            Context.UserName = user.Name;

            Context.LogEvent(new Jhu.Graywulf.Logging.Event("Jhu.Graywulf.Registry.UserFactory.LoginUser", user.Guid));

            return user;
        }

        public bool CheckEmailDuplicate(Domain domain, string email)
        {
            var sql = @"spCheckUserEmailDuplicate";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@ParentGuid", SqlDbType.UniqueIdentifier).Value = domain.Guid;                
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 128).Value = email.Trim();
                cmd.Parameters.Add("RETVAL", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                return (int)cmd.Parameters["RETVAL"].Value > 0;
            }
        }

        public User FindUserByEmail(Domain domain, string email)
        {
            return FindUserByEmailInternal(domain, email);
        }

        private User FindUserByEmailInternal(Entity parent, string email)
        {
            var user = new User(Context);

            string sql = "spFindUser_byDomainEmail";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@DomainGuid", SqlDbType.UniqueIdentifier).Value = parent.Guid;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 128).Value = email.Trim();

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        throw new EntityNotFoundException(ExceptionMessages.LoginFailed);
                    }

                    user.LoadFromDataReader(dr);
                    dr.Close();
                }
            }

            return user;
        }

        public User FindUserByActivationCode(Domain domain, string code)
        {
            return FindUserByActivationCodeInternal(domain, code);
        }

        /// <summary>
        /// Find user by activation code
        /// </summary>
        /// <param name="code"></param>
        private User FindUserByActivationCodeInternal(Entity parent, string code)
        {
            User user = null;

            if (code != null && code != String.Empty)
            {
                var sql = "spFindUser_byDomainActivationCode";

                using (var cmd = Context.CreateStoredProcedureCommand(sql))
                {
                    cmd.Parameters.Add("@DomainGuid", SqlDbType.UniqueIdentifier).Value = parent.Guid;
                    cmd.Parameters.Add("@ActivationCode", SqlDbType.NVarChar, 50).Value = code;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            user = new User(Context);
                            user.LoadFromDataReader(dr);
                        }

                        dr.Close();
                    }
                }
            }

            return user;
        }

        public User LoadUser(Guid guid)
        {
            var ef = new EntityFactory(Context);
            return ef.LoadEntity<User>(guid);
        }

        public User LoadUser(string name)
        {
            var ef = new EntityFactory(Context);
            return ef.LoadEntity<User>(name);
        }

        public User FindUserByName(Domain domain, string name)
        {
            var ef = new EntityFactory(Context);
            return ef.LoadEntity<User>(domain.GetFullyQualifiedName(), name);
        }

        public User FindUserByIdentity(Domain domain, string protocol, string authority, string identifier)
        {
            var user = new User(Context);

            var sql = "spFindUser_byIdentity";

            using (var cmd = Context.CreateStoredProcedureCommand(sql))
            {
                cmd.Parameters.Add("@DomainGuid", SqlDbType.UniqueIdentifier).Value = domain.Guid;
                cmd.Parameters.Add("@Protocol", SqlDbType.NVarChar, 25).Value = protocol;
                cmd.Parameters.Add("@Authority", SqlDbType.NVarChar, 250).Value = authority;
                cmd.Parameters.Add("@Identifier", SqlDbType.NVarChar, 250).Value = identifier;

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        throw new EntityNotFoundException(ExceptionMessages.LoginFailed);
                    }

                    user.LoadFromDataReader(dr);
                    dr.Close();
                }
            }

            return user;
        }

        #endregion
    }
}
