1. Запустите Visual Studio
2. Откройте файл AdAPI.csproj
3. Запустите проект(F5)
4. Откройте командную строку и перейдите в папку с проектом cd C:\путь\к\папке\AdAPI\AdAPI
5. Введите curl -X POST -F "file=@sample.txt" http://localhost:5000/api/platforms/load
6. Откройте в браузере, например http://localhost:5000/api/platforms/search?location=/ru/svrd/revda

Либо:

1. Откройте командную строку и перейдите в папку с проектом cd C:\путь\к\папке\AdAPI\AdAPI
2. Восстановите зависимости - dotnet restore и запустите приложение - dotnet run
3. Откройте командную строку (не закрывайте предыдущую) и перейдите в папку с проектом cd C:\путь\к\папке\AdAPI\AdAPI
4. Введите curl -X POST -F "file=@sample.txt" http://localhost:5000/api/platforms/load
5. Откройте в браузере, например http://localhost:5000/api/platforms/search?location=/ru/svrd/revda

Для проверки тестов откройте "Обозреватель тестов" в IDE.
Или перейдите в папку с тестами через командную строку cd C:\путь\к\папке\AdAPI\AdApi.Test и запустите тесты - dotnet test

ВАЖНО! 
Обязательно должен быть установлен .NET SDK 6.0.400 или выше
Проверить версию и само наличие можно через коммандую строку - dotnet --version

Скачать можно:
с сайта https://dotnet.microsoft.com/download/dotnet;
через Visual Studio;
через коммандную строку curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh | bash /dev/stdin --version latest