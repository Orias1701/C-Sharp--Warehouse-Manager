# TEST

## I. STRUCTURE

```text
Test/
├── artifacts/                  # Centralized build outputs (bin/obj)
├── src/
│   │         
│   ├── App/                    # APPLICATION LAYER
│   │   ├── Contract/           # Data Transfer Objects (DTOs), Requests, Responses
│   │   ├── Functions/          # Business logic services implementation
│   │   ├── Interface/          # Service interfaces for DI
│   │   ├── Mappings/           # AutoMapper profiles
│   │   └── Rules/              # FluentValidation classes (Business Rules)
│   │
│   ├── Base/                   # INFRASTRUCTURE LAYER
│   │   ├── Api/                # External API clients (Mail, SMS, etc.)
│   │   ├── Database/           # DbContext & Fluent API Configurations
│   │   ├── Migrations/         # EF Core DB Migration files
│   │   └── Repository/         # EF Core Repositories implementation
│   │
│   ├── Client/                 # PRESENTATION LAYER (No Drag-Drop)
│   │   ├── Components/         # Hand-coded custom UI controls
│   │   ├── Layout/             # Base Forms, Menus, Shells
│   │   ├── Views/              # Concrete Forms (Hand-coded .cs files)
│   │   └── Vm/                 # ViewModels (UI state & command logic)
│   │
│   └── Core/                   # DOMAIN LAYER
│       ├── Class/              # Database Entities (POCO)
│       ├── Constant/           # Enums, Global constants
│       ├── Error/              # Custom domain exceptions
│       └── Interface/          # Repository & UnitOfWork interfaces
│
├── tests/                      # Unit & Integration Tests
├── .env                        # Environment variables (Secrets)
├── .gitignore                  # Git exclusion rules
├── Directory.Build.props       # bin/obj redirection
├── ROADMAP.md                  # Project progress
└── TestSolution.sln
```

## II. USAGE

### 1. Chuẩn bị môi trường (Prerequisites)
Để có trải nghiệm code C# tốt nhất trên VS Code, hãy cài đặt:
* **.NET SDK (8.0/9.0):** [Tải về tại đây](https://dotnet.microsoft.com/download). Kiểm tra: `dotnet --version`.
* **VS Code Extensions:**
    * `C# Dev Kit`: Cung cấp trình quản lý Solution chuyên nghiệp.
    * `C#`: Hỗ trợ IntelliSense và Debugging.
    * `MySQL`: (Tùy chọn) Để quản lý DB ngay trong VS Code.
* **EF Core Tools:** Cài đặt toàn cục để chạy migration:
    ```bash
    dotnet tool install --global dotnet-ef
    ```

### 2. Cấu hình dự án
1. Mở thư mục gốc `Test` bằng VS Code.
2. Mở file `.env` và cập nhật thông tin MySQL:
```text
DB_CONNECTION=server=Localhost;database=Test_db;user=root;password=LongK@170105
```

### 3. Thao tác qua Terminal (CLI)
Mở Terminal trong VS Code (`Ctrl + \`) và sử dụng các lệnh:
* **Khôi phục thư viện:** `dotnet restore`
* **Biên dịch:** `dotnet build`
* **Cập nhật Database:** 
```text
dotnet ef database update --project src/Base/Base.csproj
```
* **Chạy ứng dụng:**
```text
dotnet run --project src/Client/Client.csproj
```

### 4. Debug trong VS Code
1. Nhấn **F5** hoặc chuyển sang tab **Run and Debug**.
2. Nếu lần đầu chạy, chọn môi trường là `.NET 5+ and .NET Core`.
3. Chọn project khởi chạy là `Client`.