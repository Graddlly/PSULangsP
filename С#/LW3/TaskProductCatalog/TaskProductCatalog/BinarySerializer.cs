namespace TaskProductCatalog;

using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;

/// <summary>
/// Класс для сериализации/десериализации списка продуктов в бинарный файл
/// </summary>
public class BinarySerializer
{
    /// <summary>
    /// Сериализует объект в XML и сохраняет в бинарный файл
    /// </summary>
    /// <param name="obj">Сериализуемый объект</param>
    /// <param name="filePath">Путь к файлу</param>
    public static void Serialize<T>(T obj, string filePath)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            string xmlData;
            
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, obj);
                xmlData = stringWriter.ToString();
            }
            
            var xmlBytes = Encoding.UTF8.GetBytes(xmlData);
            
            byte[] compressedBytes;
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    gzipStream.Write(xmlBytes, 0, xmlBytes.Length);
                compressedBytes = memoryStream.ToArray();
            }
            
            File.WriteAllBytes(filePath, compressedBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сериализации: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Десериализует объект из бинарного файла с XML-данными
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <returns>Десериализованный объект</returns>
    public static T Deserialize<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return default(T)!;
            
        try
        {
            var compressedBytes = File.ReadAllBytes(filePath);
            
            string xmlData;
            using (var memoryStream = new MemoryStream(compressedBytes))
                using (var decompressedStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                        gzipStream.CopyTo(decompressedStream);
                    xmlData = Encoding.UTF8.GetString(decompressedStream.ToArray());
                }
            
            var serializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xmlData))
            {
                return (T)serializer.Deserialize(stringReader)!;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при десериализации: {ex.Message}");
            return default(T)!;
        }
    }
}