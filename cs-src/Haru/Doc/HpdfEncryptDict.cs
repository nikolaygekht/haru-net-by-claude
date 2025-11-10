using System;
using System.Security.Cryptography;
using System.Text;
using Haru.Objects;
using Haru.Security;
using Haru.Types;
using Haru.Xref;

namespace Haru.Doc
{
    /// <summary>
    /// Represents the encryption dictionary in a PDF document.
    /// This dictionary specifies the encryption algorithm and parameters.
    /// </summary>
    public class HpdfEncryptDict
    {
        private readonly HpdfDict _dict;
        private readonly HpdfEncrypt _encrypt;

        /// <summary>
        /// Gets the underlying PDF dictionary.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the encryption handler.
        /// </summary>
        public HpdfEncrypt Encrypt => _encrypt;

        /// <summary>
        /// Initializes a new instance of the HpdfEncryptDict class.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        public HpdfEncryptDict(HpdfXref xref)
        {
            ArgumentNullException.ThrowIfNull(xref);
            _dict = new HpdfDict();
            _encrypt = new HpdfEncrypt();

            // Add to xref
            xref.Add(_dict);

            // Set Filter entry (encryption handler name)
            _dict["Filter"] = new HpdfName("Standard");
        }

        /// <summary>
        /// Prepares the encryption dictionary for use in the PDF.
        /// This must be called after setting passwords and permissions.
        /// </summary>
        /// <param name="encryptId">The document encryption ID (file identifier).</param>
        /// <param name="info">Optional document info dictionary for ID generation.</param>
        public void Prepare(byte[] encryptId, HpdfInfo? info = null)
        {
            // If no encryption ID provided, generate one
            if (encryptId == null || encryptId.Length < 16)
            {
                encryptId = GenerateEncryptId(info);
            }

            _encrypt.EncryptId = encryptId;

            // Generate encryption keys
            _encrypt.CreateOwnerKey();
            _encrypt.CreateEncryptionKey();
            _encrypt.CreateUserKey();

            // Populate dictionary entries based on encryption mode
            SetupDictionary();
        }

        /// <summary>
        /// Generates a file encryption ID based on document metadata.
        /// </summary>
        private byte[] GenerateEncryptId(HpdfInfo? info)
        {
            using (var md5 = MD5.Create())
            {
                // Use current time
                byte[] timeBytes = BitConverter.GetBytes(DateTime.Now.Ticks);
                md5.TransformBlock(timeBytes, 0, timeBytes.Length, null, 0);

                // Include document metadata if available
                if (info != null)
                {
                    AddStringToHash(md5, info.Author);
                    AddStringToHash(md5, info.Creator);
                    AddStringToHash(md5, info.Producer);
                    AddStringToHash(md5, info.Title);
                    AddStringToHash(md5, info.Subject);
                }

                // Add random GUID for extra entropy
                byte[] guidBytes = Guid.NewGuid().ToByteArray();
                md5.TransformFinalBlock(guidBytes, 0, guidBytes.Length);

                return md5.Hash ?? throw new HpdfException(HpdfErrorCode.InvalidDocument, "Failed to generate encryption ID");
            }
        }

        /// <summary>
        /// Adds a string to the MD5 hash if it's not null or empty.
        /// </summary>
        private void AddStringToHash(MD5 md5, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
                md5.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
        }

        /// <summary>
        /// Sets up the encryption dictionary entries based on the encryption mode.
        /// </summary>
        private void SetupDictionary()
        {
            // Common entries for all versions
            _dict["O"] = new HpdfBinary(_encrypt.OwnerKey);
            _dict["U"] = new HpdfBinary(_encrypt.UserKey);
            _dict["P"] = new HpdfNumber((int)_encrypt.Permission);

            // Version-specific entries
            switch (_encrypt.Mode)
            {
                case HpdfEncryptMode.R2:
                    _dict["V"] = new HpdfNumber(1);
                    _dict["R"] = new HpdfNumber(2);
                    break;

                case HpdfEncryptMode.R3:
                    _dict["V"] = new HpdfNumber(2);
                    _dict["R"] = new HpdfNumber(3);
                    _dict["Length"] = new HpdfNumber(_encrypt.KeyLength * 8); // In bits
                    break;

                case HpdfEncryptMode.R4:
                    _dict["V"] = new HpdfNumber(4);
                    _dict["R"] = new HpdfNumber(4);
                    _dict["Length"] = new HpdfNumber(128); // AES-128

                    // StmF and StrF specify encryption for streams and strings
                    _dict["StmF"] = new HpdfName("StdCF");
                    _dict["StrF"] = new HpdfName("StdCF");

                    // CF dictionary defines the crypt filter
                    var cf = new HpdfDict();
                    var stdCF = new HpdfDict();
                    stdCF["CFM"] = new HpdfName("AESV2"); // AES version 2 (128-bit)
                    stdCF["AuthEvent"] = new HpdfName("DocOpen");
                    stdCF["Length"] = new HpdfNumber(16); // Key length in bytes
                    cf["StdCF"] = stdCF;
                    _dict["CF"] = cf;
                    break;
            }
        }

        /// <summary>
        /// Sets the encryption mode and key length.
        /// </summary>
        /// <param name="mode">The encryption mode (R2, R3, or R4).</param>
        /// <param name="keyLength">The key length in bytes (5 for 40-bit, 16 for 128-bit).</param>
        public void SetEncryptionMode(HpdfEncryptMode mode, int keyLength)
        {
            _encrypt.Mode = mode;
            _encrypt.KeyLength = keyLength;
        }

        /// <summary>
        /// Sets the user password.
        /// </summary>
        /// <param name="password">The user password.</param>
        public void SetUserPassword(string password)
        {
            _encrypt.SetUserPassword(password);
        }

        /// <summary>
        /// Sets the owner password.
        /// </summary>
        /// <param name="password">The owner password.</param>
        public void SetOwnerPassword(string password)
        {
            _encrypt.SetOwnerPassword(password);
        }

        /// <summary>
        /// Sets the document permissions.
        /// </summary>
        /// <param name="permission">The permission flags.</param>
        public void SetPermission(HpdfPermission permission)
        {
            // Always include padding bits
            _encrypt.Permission = permission | HpdfPermission.PaddingBits;
        }
    }
}
