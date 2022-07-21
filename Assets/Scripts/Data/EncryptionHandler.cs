using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EncryptionHandler
{
    public enum EncryptionType { AES, DES, XOR };

    public Encryption_AES AES;
    public Encryption_DES DES;
    public Encryption_XOR XOR;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    public EncryptionHandler()
    {
        AES = new Encryption_AES();
        DES = new Encryption_DES();
        XOR = new Encryption_XOR();
    }

    public EncryptionHandler(string aesKey, string aesIV)
    {
        AES = new Encryption_AES(aesKey, aesIV);
        DES = new Encryption_DES();
        XOR = new Encryption_XOR();
    }

    public EncryptionHandler(string desKey)
    {
        AES = new Encryption_AES();
        DES = new Encryption_DES(desKey);
        XOR = new Encryption_XOR();
    }

    public EncryptionHandler(int xorKey)
    {
        AES = new Encryption_AES();
        DES = new Encryption_DES();
        XOR = new Encryption_XOR(xorKey);
    }

    public EncryptionHandler(string aesKey, string aesIV, string desKey, int xorKey)
    {
        AES = new Encryption_AES(aesKey, aesIV);
        DES = new Encryption_DES(desKey);
        XOR = new Encryption_XOR(xorKey);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public EncryptedObject EncryptObject(EncryptionHandler handler, UnityEngine.Object obj)
    {
        return new EncryptedObject(handler, obj);
    }

    public EncryptedObject EncryptObject(EncryptionHandler handler, UnityEngine.Object obj, EncryptionHandler.EncryptionType encType)
    {
        return new EncryptedObject(handler, obj, encType);
    }
}

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

public class Encryption_AES
{
    private string key; //set any string of 32 chars
    private string keyDefault = "A60A5770FE5E7AB200BA9CFC94E4E8B0"; //set any string of 32 chars
    private string iv; //set any string of 16 chars
    private string ivDefault = "1234567887654321"; //set any string of 16 chars

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    public Encryption_AES()
    {
        key = keyDefault;
        iv = ivDefault;
    }
    
    public Encryption_AES(string key, string iv)
    {
        if (key.Length == 32)
        {
            this.key = key;
        }
        else
        {
            this.key = keyDefault;
            Debug.LogWarning("AES encryption key must be 32 characters, but the given string was " + key.Length + ". Reverting to default.");
        }
        if (iv.Length == 16)
        {
            this.iv = iv;
        }
        else
        {
            this.iv = ivDefault;
            Debug.LogWarning("AES encryption IV must be 16 characters, but the given string was " + iv.Length + ". Reverting to default.");
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetKeys(string newKey, string newIV)
    {
        if (key.Length == 32)
        {
            key = newKey;
        }
        else
        {
            key = keyDefault;
            Debug.LogWarning("AES encryption key must be 32 characters, but the given string was " + key.Length + ". Reverting to default.");
        }
        if (iv.Length == 16)
        {
            iv = newIV;
        }
        else
        {
            iv = ivDefault;
            Debug.LogWarning("AES encryption IV must be 16 characters, but the given string was " + iv.Length + ". Reverting to default.");
        }
    }

    public string Encrypt(string input)
    {
        AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
        aesProvider.BlockSize = 128;
        aesProvider.KeySize = 256;
        aesProvider.Key = Encoding.ASCII.GetBytes(key);
        aesProvider.IV = Encoding.ASCII.GetBytes(iv);
        aesProvider.Mode = CipherMode.CBC;
        aesProvider.Padding = PaddingMode.PKCS7;

        byte[] textBytes = Encoding.ASCII.GetBytes(input);
        ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);

        byte[] result = cryptoTransform.TransformFinalBlock(textBytes, 0, textBytes.Length);
        return Convert.ToBase64String(result);
    }
    
    public string Encrypt(string input, string key, string iv)
    {
        AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
        aesProvider.BlockSize = 128;
        aesProvider.KeySize = 256;
        aesProvider.Key = Encoding.ASCII.GetBytes(key);
        aesProvider.IV = Encoding.ASCII.GetBytes(iv);
        aesProvider.Mode = CipherMode.CBC;
        aesProvider.Padding = PaddingMode.PKCS7;

        byte[] textBytes = Encoding.ASCII.GetBytes(input);
        ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);

        byte[] result = cryptoTransform.TransformFinalBlock(textBytes, 0, textBytes.Length);
        return Convert.ToBase64String(result);
    }
    
    public string Decrypt(string input)
    {
        AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
        aesProvider.BlockSize = 128;
        aesProvider.KeySize = 256;
        aesProvider.Key = Encoding.ASCII.GetBytes(key);
        aesProvider.IV = Encoding.ASCII.GetBytes(iv);
        aesProvider.Mode = CipherMode.CBC;
        aesProvider.Padding = PaddingMode.PKCS7;

        byte[] textBytes = Convert.FromBase64String(input);
        ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor();

        byte[] result = cryptoTransform.TransformFinalBlock(textBytes, 0, textBytes.Length);
        return Encoding.ASCII.GetString(result);
    }
    
    public string Decrypt(string input, string key, string iv)
    {
        AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
        aesProvider.BlockSize = 128;
        aesProvider.KeySize = 256;
        aesProvider.Key = Encoding.ASCII.GetBytes(key);
        aesProvider.IV = Encoding.ASCII.GetBytes(iv);
        aesProvider.Mode = CipherMode.CBC;
        aesProvider.Padding = PaddingMode.PKCS7;

        byte[] textBytes = Convert.FromBase64String(input);
        ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor();

        byte[] result = cryptoTransform.TransformFinalBlock(textBytes, 0, textBytes.Length);
        return Encoding.ASCII.GetString(result);
    }
}

public class Encryption_DES
{
    private string key;
    private string keyDefault;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public Encryption_DES()
    {
        key = "fY8k0Wn2";
    }
    
    public Encryption_DES(string key)
    {
        if (key.Length == 8)
        {
            this.key = key;
        }
        else
        {
            this.key = "fY8k0Wn2";
            Debug.Log("DES encryption key must be 8 characters, but the given string was " + key.Length + ". Reverting to default.");
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetKey(string newKey)
    {
        if (key.Length == 8)
        {
            key = newKey;
        }
        else
        {
            key = "fY8k0Wn2";
            Debug.Log("DES encryption key must be 8 characters, but the given string was " + key.Length + ". Reverting to default.");
        }
    }

    public string Encrypt(string input)
    {
        byte[] textBytes = Encoding.ASCII.GetBytes(input);
        byte[] keyBytes = Encoding.ASCII.GetBytes(key);

        DESCryptoServiceProvider DEScryptoProvider = new DESCryptoServiceProvider();
        ICryptoTransform cryptoTransform = DEScryptoProvider.CreateEncryptor(keyBytes, keyBytes);
        CryptoStreamMode mode = CryptoStreamMode.Write;

        //Set up Stream & Write Encript data
        MemoryStream mStream = new MemoryStream();
        CryptoStream cStream = new CryptoStream(mStream, cryptoTransform, mode);
        cStream.Write(textBytes, 0, textBytes.Length);
        cStream.FlushFinalBlock();

        //Read Ecncrypted Data From Memory Stream
        byte[] result = new byte[mStream.Length];
        mStream.Position = 0;
        mStream.Read(result, 0, result.Length);

        return Convert.ToBase64String(result);
    }
    
    public string Encrypt(string input, string key)
    {
        byte[] textBytes = Encoding.ASCII.GetBytes(input);
        byte[] keyBytes = Encoding.ASCII.GetBytes(key);

        DESCryptoServiceProvider DEScryptoProvider = new DESCryptoServiceProvider();
        ICryptoTransform cryptoTransform = DEScryptoProvider.CreateEncryptor(keyBytes, keyBytes);
        CryptoStreamMode mode = CryptoStreamMode.Write;

        //Set up Stream & Write Encript data
        MemoryStream mStream = new MemoryStream();
        CryptoStream cStream = new CryptoStream(mStream, cryptoTransform, mode);
        cStream.Write(textBytes, 0, textBytes.Length);
        cStream.FlushFinalBlock();

        //Read Ecncrypted Data From Memory Stream
        byte[] result = new byte[mStream.Length];
        mStream.Position = 0;
        mStream.Read(result, 0, result.Length);

        return Convert.ToBase64String(result);
    }
    
    public string Decrypt(string input)
    {
        byte[] textBytes = Convert.FromBase64String(input);
        byte[] keyBytes = Encoding.ASCII.GetBytes(key);

        DESCryptoServiceProvider DEScryptoProvider = new DESCryptoServiceProvider();
        ICryptoTransform cryptoTransform = DEScryptoProvider.CreateDecryptor(keyBytes, keyBytes);
        CryptoStreamMode mode = CryptoStreamMode.Write;

        //Set up Stream & Write Encript data
        MemoryStream mStream = new MemoryStream();
        CryptoStream cStream = new CryptoStream(mStream, cryptoTransform, mode);
        cStream.Write(textBytes, 0, textBytes.Length);
        cStream.FlushFinalBlock();

        //Read Ecncrypted Data From Memory Stream
        byte[] result = new byte[mStream.Length];
        mStream.Position = 0;
        mStream.Read(result, 0, result.Length);

        return Encoding.ASCII.GetString(result);
    }

    public string Decrypt(string input, string key)
    {
        byte[] textBytes = Convert.FromBase64String(input);
        byte[] keyBytes = Encoding.ASCII.GetBytes(key);

        DESCryptoServiceProvider DEScryptoProvider = new DESCryptoServiceProvider();
        ICryptoTransform cryptoTransform = DEScryptoProvider.CreateDecryptor(keyBytes, keyBytes);
        CryptoStreamMode mode = CryptoStreamMode.Write;

        //Set up Stream & Write Encript data
        MemoryStream mStream = new MemoryStream();
        CryptoStream cStream = new CryptoStream(mStream, cryptoTransform, mode);
        cStream.Write(textBytes, 0, textBytes.Length);
        cStream.FlushFinalBlock();

        //Read Ecncrypted Data From Memory Stream
        byte[] result = new byte[mStream.Length];
        mStream.Position = 0;
        mStream.Read(result, 0, result.Length);

        return Encoding.ASCII.GetString(result);
    }
}

public class Encryption_XOR
{
    private int key;
    private int keyDeault;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public Encryption_XOR()
    {
        key = 80714292;
    }

    public Encryption_XOR(int key)
    {
        this.key = key;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetKey(int newKey)
    {
        key = newKey;
    }

    public string EncryptDecrypt(string input)
    {
        StringBuilder output = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            char ch = (char)(input[i] ^ key);
            output.Append(ch);
        }
        return output.ToString();
    }

    public string EncryptDecrypt(string input, int key)
    {
        StringBuilder output = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            char ch = (char)(input[i] ^ key);
            output.Append(ch);
        }
        return output.ToString();
    }
}

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

public class EncryptedObject
{
    public string objectType;
    public string dataString;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public EncryptedObject()
    { }

    public EncryptedObject(EncryptionHandler handler, UnityEngine.Object obj)
    {
        dataString = EncryptObj(handler, obj, EncryptionHandler.EncryptionType.AES);
    }
    
    public EncryptedObject(EncryptionHandler handler, UnityEngine.Object obj, EncryptionHandler.EncryptionType encType)
    {
        dataString = EncryptObj(handler, obj, encType);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void EncryptNewObject(EncryptionHandler handler, UnityEngine.Object obj)
    {
        dataString = EncryptObj(handler, obj, EncryptionHandler.EncryptionType.AES);
    }
    
    public void EncryptNewObject(EncryptionHandler handler, UnityEngine.Object obj, EncryptionHandler.EncryptionType encType)
    {
        dataString = EncryptObj(handler, obj, encType);
    }

    private string EncryptObj(EncryptionHandler handler, UnityEngine.Object obj, EncryptionHandler.EncryptionType encType)
    {
        try
        {
            objectType = obj.GetType().ToString();

            string output;
            switch (encType)
            {
                default:
                case EncryptionHandler.EncryptionType.AES:
                    output = handler.AES.Encrypt(JsonUtility.ToJson(obj));
                    break;

                case EncryptionHandler.EncryptionType.DES:
                    output = handler.DES.Encrypt(JsonUtility.ToJson(obj));
                    break;

                case EncryptionHandler.EncryptionType.XOR:
                    output = handler.XOR.EncryptDecrypt(JsonUtility.ToJson(obj));
                    break;
            }
            return output;
        }
        catch
        {
            throw new Exception("ERROR: Cannot encrypt object \"" + obj.name + "\", as it is a non-serializable type! <" + obj.GetType().ToString() + ">");
        }
    }
}
