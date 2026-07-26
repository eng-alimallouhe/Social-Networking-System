# 1. مرحلة البناء (Build Stage)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف الحل (Solution) أولاً (إن وجد) أو نسخ ملفات الـ csproj لكل طبقة لضمان استغلال الـ caching
# استبدل SNS.sln باسم ملف الـ Solution الحقيقي عندك، أو انسخ ملفات الـ csproj يدوياً
COPY ["SNS.slnx", "./"]
COPY ["SNS.API/SNS.API.csproj", "SNS.API/"]
COPY ["SNS.Infrastructure/SNS.Infrastructure.csproj", "SNS.Infrastructure/"]
COPY ["SNS.Application/SNS.Application.csproj", "SNS.Application/"]
COPY ["SNS.Domain/SNS.Domain.csproj", "SNS.Domain/"]
COPY ["SNS.Shared/SNS.Shared.csproj", "SNS.Shared/"]

# استعادة الحزم (Restore)
RUN dotnet restore "SNS.API/SNS.API.csproj"

# نسخ باقي كود المشروع بالكامل
COPY . .

# الانتقال لمجلد الـ API وبناء المشروع (Publish)
WORKDIR "/src/SNS.API"
RUN dotnet publish "SNS.API.csproj" -c Release -o /app/publish

# 2. مرحلة التشغيل النهائية (Runtime Stage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# تحديد ملف الـ DLL الخاص بطبقة الـ API لتشغيله
ENTRYPOINT ["dotnet", "SNS.API.dll"]