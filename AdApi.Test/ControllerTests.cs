using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdAPI.Controllers;
using AdAPI.Models;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace AdAPI.Tests
{
    [TestClass]
    public class ControllerTests
    {
        private Mock<Storage> _storageMock;
        private PlatformsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _storageMock = new Mock<Storage>();
            _controller = new PlatformsController(_storageMock.Object);
        }

        [TestMethod]
        [Description("Должен возвращать ошибку при загрузке null файла")]
        public void LoadPlatforms_NullFile()
        {
            var result = _controller.LoadPlatforms(null);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [Description("Должен возвращать ошибку при загрузке пустого файла")]
        public void LoadPlatforms_EmptyFile()
        {
            var emptyFile = new Mock<IFormFile>();
            emptyFile.Setup(f => f.Length).Returns(0);

            var result = _controller.LoadPlatforms(emptyFile.Object);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [Description("Должен возвращать ошибку при загрузке файла не txt формата")]
        public void LoadPlatforms_NonTxtFile()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("image.jpg");

            var result = _controller.LoadPlatforms(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [Description("Должен успешно загружать корректный txt файл")]
        public void LoadPlatforms_ValidTxtFile()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("data.txt");
            fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>()));

            var result = _controller.LoadPlatforms(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Данные загружены!", okResult.Value);
        }

        [TestMethod]
        [Description("Должен возвращать ошибку при поиске с null локацией")]
        public void SearchPlatforms_NullLocation()
        {
            var result = _controller.SearchPlatforms(null);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [Description("Должен возвращать ошибку при поиске с пустой локацией")]
        public void SearchPlatforms_EmptyLocation()
        {
            var result = _controller.SearchPlatforms("");

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [Description("Должен возвращать ошибку при некорректном формате локации")]
        public void SearchPlatforms_InvalidLocation()
        {
            var result1 = _controller.SearchPlatforms("ru");
            var result2 = _controller.SearchPlatforms("/ru test");
            var result3 = _controller.SearchPlatforms(" /ru");

            Assert.IsInstanceOfType(result1, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(result2, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(result3, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [Description("Должен возвращать платформы по корректной локации")]
        public void SearchPlatforms_ValidLocation()
        {
            var expectedPlatforms = new List<string> { "Яндекс.Директ", "Google.Ads" };
            _storageMock.Setup(s => s.FindPlatforms("/ru")).Returns(expectedPlatforms);

            var result = _controller.SearchPlatforms("/ru") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            CollectionAssert.AreEqual(expectedPlatforms, result.Value as List<string>);
        }

        [TestMethod]
        [Description("Должен возвращать платформы по вложенной локации")]
        public void SearchPlatforms_SubPlatforms()
        {
            var expectedPlatforms = new List<string> { "Яндекс.Директ" };
            _storageMock.Setup(s => s.FindPlatforms("/ru/moscow")).Returns(expectedPlatforms);

            var result = _controller.SearchPlatforms("/ru/moscow") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            CollectionAssert.AreEqual(expectedPlatforms, result.Value as List<string>);
        }

        [TestMethod]
        [Description("Должен возвращать ошибку сервера при исключении в хранилище")]
        public void SearchPlatforms_StorageThrowsException()
        {
            _storageMock.Setup(s => s.FindPlatforms("/ru"))
                       .Throws(new Exception("Database error"));

            var result = _controller.SearchPlatforms("/ru") as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.IsTrue(result.Value.ToString().Contains("Ошибка поиска"));
        }

        [TestMethod]
        [Description("Должен возвращать пустой список при отсутствии платформ")]
        public void SearchPlatforms_NoPlatformsFound()
        {
            var emptyList = new List<string>();
            _storageMock.Setup(s => s.FindPlatforms("/fr")).Returns(emptyList);

            var result = _controller.SearchPlatforms("/fr") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var platforms = result.Value as List<string>;
            Assert.AreEqual(0, platforms.Count);
        }

        [TestMethod]
        [Description("Должен создавать и удалять временный файл при загрузке")]
        public void LoadPlatforms_CreatesAndDeletesTempFile()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("data.txt");
            fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>()))
                   .Callback<Stream>(stream =>
                   {
                       var data = Encoding.UTF8.GetBytes("test content");
                       stream.Write(data, 0, data.Length);
                   });

            var result = _controller.LoadPlatforms(fileMock.Object);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}