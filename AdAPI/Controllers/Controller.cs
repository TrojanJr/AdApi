using System.IO;
using AdAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase  
    {
        private readonly Storage _storage;

        public PlatformsController(Storage storage) 
        {
            _storage = storage;
        }

        [HttpPost("load")]
        public IActionResult LoadPlatforms(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return BadRequest("Ошибка: не выбран файл");
            }

            if (!file.FileName.EndsWith(".txt"))
            {
                return BadRequest("Ошибка: нужен .txt файл");
            }

            string tempFilePath = null;
            try
            {
                // Создание временного файла для обработки
                tempFilePath = Path.GetTempFileName();
                // Копирование содержимого загруженного файла во временный файл
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                _storage.LoadFromFile(tempFilePath);

                return Ok("Данные загружены!");
            }
            catch (Exception ex)
            {
                // Обработка непредвиденных ошибок при работе с файлом
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
            finally
            {
                // Очистка: удаление временного файла в любом случае
                if (tempFilePath != null && System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }
        }

        [HttpGet("search")]
        public IActionResult SearchPlatforms(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Ошибка: укажите параметр location");
            }
            // Проверка формата локации (должна начинаться с / и без пробелов)
            if (!location.StartsWith("/") || location.Contains(" "))
            {
                return BadRequest("Ошибка: локация должна начинаться с / и без пробелов");
            }

            try
            {
                // Поиск платформ по указанной локации в хранилище
                var platforms = _storage.FindPlatforms(location);
                return Ok(platforms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка поиска: {ex.Message}");
            }
        }
    }
}