namespace AdAPI.Models
{
    public class Platform
    {
        public string Name { get; set; }    // Название площадки (Яндекс.Директ)
        public List<string> Locations { get; set; }  // Список локаций (/ru, /ru/msk)
    }
}
