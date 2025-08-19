using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdAPI.Models;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AdAPI.Tests
{
    [TestClass]
    public class StorageTests
    {
        private string _testFile;

        [TestInitialize]
        public void Init() => _testFile = CreateTestFile();

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFile))
                File.Delete(_testFile);
        }

        [TestMethod]
        [Description("Должен загружать данные из файла")]
        public void LoadFromFile_ValidFile()
        {
            var storage = new Storage();

            storage.LoadFromFile(_testFile);

            var platforms = storage.GetAllPlatforms();
            Assert.AreEqual(2, platforms.Count);
            Assert.IsTrue(platforms.Any(p => p.Name == "Яндекс.Директ"));
            Assert.IsTrue(platforms.Any(p => p.Name == "Google Ads"));
        }

        [TestMethod]
        [Description("Должен находить площадки по вложенным локациям")]
        public void FindPlatforms_ReturnsMatches()
        {
            var storage = new Storage();
            storage.LoadFromFile(_testFile);

            var result = storage.FindPlatforms("/ru/msk");

            Assert.IsTrue(result.Contains("Яндекс.Директ"));
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        [Description("Должен возвращать пустой список для несуществующей локации")]
        public void FindPlatforms_ReturnsEmpty()
        {
            var storage = new Storage();
            storage.LoadFromFile(_testFile);

            var result = storage.FindPlatforms("/fr");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [Description("Должен корректно обрабатывать пустой файл")]
        public void LoadFromFile_FileEmpty()
        {
            var emptyFile = Path.GetTempFileName();
            File.WriteAllText(emptyFile, "");
            var storage = new Storage();

            storage.LoadFromFile(emptyFile);

            Assert.AreEqual(0, storage.GetAllPlatforms().Count);

            File.Delete(emptyFile);
        }

        [TestMethod]
        [Description("Должен игнорировать некорректные строки в файле")]
        public void LoadFromFile_InvalidLines()
        {
            var invalidFile = Path.GetTempFileName();
            File.WriteAllText(invalidFile, "Яндекс.Директ:/ru\nInvalidLineWithoutColon\n:OnlyColon\n   \nGoogle Ads:/us");
            var storage = new Storage();

            storage.LoadFromFile(invalidFile);

            var platforms = storage.GetAllPlatforms();
            Assert.AreEqual(3, platforms.Count);

            Assert.IsTrue(platforms.Any(p => p.Name == "Яндекс.Директ"));
            Assert.IsTrue(platforms.Any(p => p.Name == "Google Ads"));

            Assert.IsTrue(platforms.Any(p => string.IsNullOrEmpty(p.Name) && p.Locations.Contains("OnlyColon")));

            File.Delete(invalidFile);
        }

        [TestMethod]
        [Description("Должен находить платформы по точному совпадению локации")]
        public void FindPlatforms_ReturnsPlatform()
        {
            var storage = new Storage();
            storage.LoadFromFile(_testFile);

            var result = storage.FindPlatforms("/ru");

            Assert.IsTrue(result.Contains("Яндекс.Директ"));
        }

        [TestMethod]
        [Description("Должен сортировать результаты по количеству слешей в локации")]
        public void FindPlatforms_SlashSort()
        {
            var testFile = Path.GetTempFileName();
            File.WriteAllText(testFile, "Platform1:/ru\nPlatform2:/ru/msk\nPlatform3:/ru/msk/center");
            var storage = new Storage();
            storage.LoadFromFile(testFile);

            var result = storage.FindPlatforms("/ru/msk/center");

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Platform1", result[0]); // наименьшее количество слешей
            Assert.AreEqual("Platform2", result[1]);
            Assert.AreEqual("Platform3", result[2]); // наибольшее количество слешей

            File.Delete(testFile);
        }

        [TestMethod]
        [Description("Должен обрабатывать платформы без локаций")]
        public void LoadFromFile_NoLocations()
        {
            var testFile = Path.GetTempFileName();
            File.WriteAllText(testFile, "PlatformWithLocations:/ru,/us\nPlatformWithoutLocations:\nAnotherPlatform:/en");
            var storage = new Storage();

            storage.LoadFromFile(testFile);

            var platforms = storage.GetAllPlatforms();
            Assert.AreEqual(2, platforms.Count);
            Assert.IsTrue(platforms.Any(p => p.Name == "PlatformWithLocations"));
            Assert.IsTrue(platforms.Any(p => p.Name == "AnotherPlatform"));
            Assert.IsFalse(platforms.Any(p => p.Name == "PlatformWithoutLocations"));

            File.Delete(testFile);
        }

        [TestMethod]
        [Description("Должен корректно обрабатывать пробелы в локациях")]
        public void LoadFromFile_SpacesInLocations()
        {
            var testFile = Path.GetTempFileName();
            File.WriteAllText(testFile, "TestPlatform: /ru , /us/ny , /en ");
            var storage = new Storage();

            storage.LoadFromFile(testFile);

            var platforms = storage.GetAllPlatforms();
            var platform = platforms.First();
            CollectionAssert.AreEqual(new List<string> { "/ru", "/us/ny", "/en" }, platform.Locations);

            File.Delete(testFile);
        }

        [TestMethod]
        [Description("Должен обрабатывать строки только с пробелами")]
        public void LoadFromFile_WhitespaceOnlyLines()
        {
            var testFile = Path.GetTempFileName();
            File.WriteAllText(testFile, "  \n\t\nValidPlatform:/ru\n   ");
            var storage = new Storage();

            storage.LoadFromFile(testFile);

            Assert.AreEqual(1, storage.GetAllPlatforms().Count);

            File.Delete(testFile);
        }

        [TestMethod]
        [Description("Должен находить платформы по частичному совпадению локации")]
        public void FindPlatforms_PartialLocations()
        {
            var storage = new Storage();
            storage.LoadFromFile(_testFile);

            var result = storage.FindPlatforms("/ru/msk/south");

            Assert.IsTrue(result.Contains("Яндекс.Директ"));
        }

        private string CreateTestFile()
        {
            var path = Path.GetTempFileName();
            File.WriteAllText(path, "Яндекс.Директ:/ru,/ru/msk\nGoogle Ads:/us");
            return path;
        }
    }
}