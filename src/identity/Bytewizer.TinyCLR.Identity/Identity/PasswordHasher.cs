﻿using System;

using GHIElectronics.TinyCLR.Cryptography;

namespace Bytewizer.TinyCLR.Identity
{
    /// <summary>
    /// Implements HMAC-SHA256 identity password hashing.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Returns a hashed representation of the supplied <paramref name="password"/> for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose password is to be hashed.</param>
        /// <param name="password">The password to hash.</param>
        /// <returns>A hashed representation of the supplied <paramref name="password"/> for the specified <paramref name="user"/>.</returns>
        public byte[] HashPassword(IIdentityUser user, byte[] password)
        {
            Random rnd = new Random();

            var key  = new byte[32];
            rnd.NextBytes(key);
            
            var bytes = new byte[key.Length + password.Length];
            Array.Copy(key, 0, bytes, 0, key.Length);
            Array.Copy(password, 0, bytes, key.Length, password.Length);

            user.PasswordSalt = bytes;
            user.PasswordHash = new HMACSHA256(bytes).ComputeHash(password);

            return user.PasswordHash;
        }

        /// <summary>
        /// Returns a <see cref="IdentityResult"/> indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="user">The user whose password should be verified.</param>
        /// <param name="hashedPassword">The hash value for a user's stored password.</param>
        /// <param name="providedPassword">The password supplied for comparison.</param>
        /// <returns>A <see cref="IdentityResult"/> indicating the result of a password hash comparison.</returns>
        public IdentityResult VerifyHashedPassword(IIdentityUser user, byte[] hashedPassword, byte[] providedPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }

            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            try
            {
                var hashBytes = new HMACSHA256(user.PasswordSalt).ComputeHash(providedPassword);

                if (VerifyHashedPassword(hashedPassword, hashBytes))
                {
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed("Authentication failed: Invalid security hash value.");
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex);
            }
        }

        private static bool VerifyHashedPassword(byte[] hashedPassword, byte[] password)
        {
            if (hashedPassword.Length < 1 || password.Length < 1)
            {
                return false; // bad size
            }

            return ByteArraysEqual(hashedPassword, password);
        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var isSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                isSame &= (a[i] == b[i]);
            }

            return isSame;
        }
    }
}
