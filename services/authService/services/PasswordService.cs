using System.Security.Cryptography;
using System.Text;

namespace authService.Services;

public static class PasswordService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    /// <summary>
    /// Hashes a plain text password using PBKDF2 with SHA256
    /// </summary>
    /// <param name="password">The plain text password to hash</param>
    /// <returns>The hashed password with salt (base64 encoded)</returns>
    public static string Encode(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Generate a random salt
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Hash the password with the salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        // Combine salt and hash
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Return as base64 string
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifies a plain text password against a hashed password
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="hashedPassword">The hashed password to compare against</param>
    /// <returns>True if the password matches, false otherwise</returns>
    public static bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
        
        if (string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            // Decode the base64 string
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            
            // Ensure we have the right length
            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            // Extract salt and hash
            byte[] salt = new byte[SaltSize];
            byte[] storedHash = new byte[HashSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);
            Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

            // Hash the provided password with the extracted salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] computedHash = pbkdf2.GetBytes(HashSize);

            // Compare hashes using constant-time comparison
            return ConstantTimeEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Constant-time comparison to prevent timing attacks
    /// </summary>
    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}
