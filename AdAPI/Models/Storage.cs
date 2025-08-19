namespace AdAPI.Models
{
    public class Storage
    {
        //Хранилище платформ в памяти
        private List<Platform> _platforms = new List<Platform>();
        public List<Platform> GetAllPlatforms() => _platforms;
        //Загрузка данных из файла в хранилище
        public void LoadFromFile(string filePath)
        {
            _platforms = new List<Platform>();
            // Построчное чтение файла
            foreach (var line in File.ReadLines(filePath))
            {
                try
                {
                    // Пропуск пустых строк
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    // Разделение строки на название платформы и локации
                    var parts = line.Split(':', 2); 
                    if (parts.Length != 2) continue;

                    // Обрабатываем локации
                    var locations = parts[1].Split(',')
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToList();
                    // Пропуск платформ без локаций
                    if (locations.Count == 0) continue;
                    // Добавление новой платформы в хранилище
                    _platforms.Add(new Platform{Name = parts[0].Trim(),Locations = locations});
                }
                catch
                {
                    continue;
                }
            }
        }

        // Поиск платформ по указанной локации
        public virtual List<string> FindPlatforms(string location)
        {
            return _platforms
                // Фильтрация
                .Where(p => p.Locations.Any(l => location.StartsWith(l) || l.StartsWith(location)))
                // Сортировка по глубине вложенности локации
                .OrderBy(p => p.Locations.Min(l => l.Count(c => c == '/')))
                .Select(p => p.Name)
                .ToList();
        }
    }
}
