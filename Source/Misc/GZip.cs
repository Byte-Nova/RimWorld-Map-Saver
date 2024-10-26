using System.IO;
using System.IO.Compression;

public static class GZip
{
    public static byte[] Compress(byte[] bytes)
    {
        using MemoryStream memoryStream = new MemoryStream();
        using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            gzipStream.Write(bytes, 0, bytes.Length);
        }

        return memoryStream.ToArray();
    }

    public static byte[] Decompress(byte[] bytes)
    {
        using MemoryStream memoryStream = new MemoryStream(bytes);
        using MemoryStream outputStream = new MemoryStream();
        using (GZipStream decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            decompressStream.CopyTo(outputStream);
        }

        return outputStream.ToArray();
    }
}