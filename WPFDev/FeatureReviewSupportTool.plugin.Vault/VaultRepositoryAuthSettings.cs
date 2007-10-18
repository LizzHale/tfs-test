using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Exortech.NetReflector;
using Microsoft.Win32;

namespace Sep.ConfigurationManagement.Vault.CodeReview
{
    [ReflectorType( "settings" )]
    public class VaultRepositoryAuthSettings
    {
        private readonly Encoding passwordEncoding = Encoding.Unicode;

        private bool isDirty;

        private string user = string.Empty;
        private string password = string.Empty;
        private string host = string.Empty;
        private string repository = string.Empty;

        public VaultRepositoryAuthSettings( )
        {
            LoadVaultClientSettings( );
            WipeOffTheMud( );
        }

        public bool IsDirty
        {
            get { return isDirty; }
        }

        [ReflectorProperty( "user", Required = false )]
        public string User
        {
            get { return user; }
            set
            {
                if( user != value )
                {
                    isDirty = true;
                    user = value;
                }
            }
        }

        [ReflectorProperty( "password", Required = false )]
        public string EncryptedPassword
        {
            get { return Encrypt( Password ); }
            set { Password = Decrypt( value ); }
        }

        public string Password
        {
            get { return password; }
            set
            {
                if( password != value )
                {
                    isDirty = true;
                    password = value;
                }
            }
        }

        [ReflectorProperty( "host", Required = false )]
        public string Host
        {
            get { return host; }
            set
            {
                if( host != value )
                {
                    isDirty = true;
                    host = value;
                }
            }
        }

        [ReflectorProperty( "repository", Required = false )]
        public string Repository
        {
            get { return repository; }
            set
            {
                if( repository != value )
                {
                    isDirty = true;
                    repository = value;
                }
            }
        }

        public string Url
        {
            get { return "http://" + Host + "/VaultService"; }
            set
            {
                if( value == string.Empty )
                {
                    Host = value;
                }
                else
                {
                    Uri uri = new Uri( value );
                    Host = uri.Host;
                }
            }
        }

        /// <summary>
        /// Clears the "dirty" bit.
        /// </summary>
        public void WipeOffTheMud( )
        {
            isDirty = false;
        }

        #region Password protection

        private string Decrypt( string cypherText )
        {
            return passwordEncoding.GetString( Transform( CreateDecryptor(), Convert.FromBase64String( cypherText ) ) );
        }

        private string Encrypt( string plainText )
        {
            return Convert.ToBase64String( Transform( CreateEncryptor(), passwordEncoding.GetBytes( plainText ) ) );
        }

        private byte [] Transform( ICryptoTransform transformer, byte [] input )
        {
            MemoryStream transformedData = new MemoryStream();
            CryptoStream transformingStream = new CryptoStream( transformedData, transformer, CryptoStreamMode.Write );
            transformingStream.Write( input, 0, input.Length );
            transformingStream.Close();
            return transformedData.ToArray();
        }

        private ICryptoTransform CreateDecryptor( )
        {
            return CreateCryptoServiceProvider( ).CreateDecryptor( );
        }

        private ICryptoTransform CreateEncryptor( )
        {
            return CreateCryptoServiceProvider( ).CreateEncryptor( );
        }

        private SymmetricAlgorithm CreateCryptoServiceProvider( )
        {
            DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider( );
            // At one point, these were generated randomly.
            cryptic.Key = new byte [] { 0x42, 0xFD, 0x3E, 0x1F, 0x87, 0xE7, 0x3A, 0x5 };
            cryptic.IV = new byte [] { 0x21, 0xB4, 0x9F, 0x4B, 0x64, 0x5E, 0xB4, 0x10 };
            return cryptic;
        }

        private string ToHexString( byte [] b )
        {
            string [] bytes = new string [ b.Length ];
            for( int i = 0; i < b.Length; i++ )
            {
                bytes[i] = "0x" + b[i].ToString( "X" );
            }
            return "{ " + string.Join( ", ", bytes ) + " }";
        }

        #endregion

        private void LoadVaultClientSettings( )
        {
            RegistryKey vaultClientSettings = Registry.CurrentUser.OpenSubKey( @"Software\SourceGear\Vault\Client\Settings" );
            if( vaultClientSettings != null )
            {
                User = vaultClientSettings.GetValue( "LastUsername", User ).ToString( );
                Url = vaultClientSettings.GetValue( "LastURLBase", Url ).ToString( );
            }
        }

        private bool IsEmpty( string s )
        {
            return s == null || s == string.Empty;
        }
    }
}
