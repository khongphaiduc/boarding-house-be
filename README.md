# BoardingHouse

Code base ASP.NET Core Web API .NET 8 theo Clean Architecture, dùng EF Core 8 và SQL Server.

## Cấu trúc và chiều phụ thuộc

~~~text
BoardingHouse.sln
├── .env.example                  # Mẫu cấu hình để commit
├── .gitignore                    # Bỏ qua secret, build output và file IDE
├── .gitattributes                # Chuẩn hóa line ending
├── .github/workflows/ci.yml       # CI build/test trên GitHub
├── BoardingHouse.Domain/
│   └── Entities/                  # 16 entity nghiệp vụ có hậu tố Entity
├── BoardingHouse.Application/
│   ├── Abstractions/Persistence/  # Interface repository, unit of work
│   ├── Common/                   # Phân trang, exception nghiệp vụ
│   ├── Rooms/                    # DTO, interface và application service mẫu
│   └── DependencyInjection.cs
├── BoardingHouse.Infrastructure/
│   ├── Models/                   # Model database: Room, User, Contract, ...
│   ├── Mappings/                 # Model database → Domain entity
│   ├── Persistence/
│   │   ├── BoardingHouseDbContext.cs
│   │   └── Repositories/         # EF Core implementation
│   └── DependencyInjection.cs
├── BoardingHouse/                # Project BoardingHouse.API
│   ├── Controllers/
│   ├── ExceptionHandling/
│   ├── Program.cs                # Composition root và middleware
│   └── BoardingHouse.http        # Request mẫu
└── tests/BoardingHouse.Tests/     # Application, DI và architecture tests
~~~

- Domain không tham chiếu project khác hoặc EF Core.
- Application tham chiếu Domain, định nghĩa use case và abstraction truy cập dữ liệu.
- Infrastructure tham chiếu Application và Domain, triển khai SQL Server/repository.
- API tham chiếu Application và Infrastructure để đăng ký DI; controller chỉ gọi interface Application.

Domain có 16 class riêng như RoomEntity, UserEntity và ContractEntity; chúng giữ các field nghiệp vụ và khóa liên kết bằng ID. Các model database như Room, User và Contract nằm trong Infrastructure/Models và giữ navigation property/mapping EF. DbContext chỉ ánh xạ model database; Application chỉ dùng entity của Domain.

Luồng mẫu: RoomsController → IRoomService/RoomService → IRoomRepository → RoomRepository → BoardingHouseDbContext → SQL Server. Repository chuyển model Room thành RoomEntity qua projection RoomMapping, sau đó Application chuyển RoomEntity thành RoomDto. IUnitOfWork dùng chung DbContext trong một request; use case ghi dữ liệu gọi SaveChangesAsync sau khi hoàn thành các thao tác.

## Clone và chạy local

Yêu cầu: SDK .NET hỗ trợ net8.0, runtime ASP.NET Core 8, SQL Server có schema BoardingHouseDB hiện có. Chạy các lệnh từ thư mục chứa BoardingHouse.sln.

1. Với checkout mới, tạo .env từ mẫu và chỉnh chuỗi kết nối SQL Server:

~~~powershell
Copy-Item .env.example .env
~~~

2. Restore, build, test và chạy API:

~~~powershell
dotnet restore BoardingHouse.sln
dotnet build BoardingHouse.sln -c Release
dotnet test BoardingHouse.sln -c Release --no-build --no-restore
dotnet run --project BoardingHouse/BoardingHouse.API.csproj --no-launch-profile
~~~

Health: http://localhost:5154/health — 200, text/plain, body Healthy. Endpoint này kiểm tra tiến trình API, không kiểm tra kết nối SQL Server. Request mẫu có trong BoardingHouse/BoardingHouse.http.

## Cấu hình .env

.env nằm ở thư mục solution và được nạp trước khi tạo WebApplicationBuilder. Khi chạy từ thư mục API bằng Visual Studio hoặc dotnet run, ứng dụng cũng tìm .env ở thư mục solution cha. Khi chạy bản publish, đặt .env vào working directory hoặc cạnh file DLL, hoặc truyền biến môi trường trực tiếp.

Biến môi trường đã được server/process đặt sẽ được ưu tiên hơn giá trị trong .env. Tham số dòng lệnh được ASP.NET Core áp dụng sau cùng cho các key hỗ trợ. appsettings.json chỉ giữ cấu hình mặc định không chứa credential; không dùng appsettings.Local.json nữa.

| Key | Ý nghĩa |
| --- | --- |
| ASPNETCORE_ENVIRONMENT | Development hoặc Production |
| ASPNETCORE_URLS | URL mà API lắng nghe, ví dụ http://localhost:5154 |
| ConnectionStrings__DefaultConnection | Chuỗi kết nối SQL Server; bắt buộc |
| Cors__AllowedOrigins__0 | Origin frontend thứ nhất |
| Cors__AllowedOrigins__1 | Origin frontend thứ hai; bỏ dòng nếu không dùng |
| AllowedHosts | Host được chấp nhận; local có thể dùng * |
| Logging__LogLevel__Default | Mức log mặc định |

Dấu __ được ASP.NET Core chuyển thành dấu : cho key cấu hình phân cấp. CORS dùng các origin cụ thể, không có dấu / ở cuối; thêm origin bằng index 2, 3, ... Nếu chỉ có một frontend thì bỏ các dòng index còn lại.

.env.example dùng Windows LocalDB và có comment mẫu SQL Server authentication với placeholder <user>/<password>. File .env local hiện có giữ kết nối của workspace; không commit file này. Dùng dấu nháy đơn cho chuỗi kết nối để giữ nguyên ký tự đặc biệt và dấu backslash. Khởi động lại API sau khi sửa .env.

Lệnh --no-launch-profile cho phép .env điều khiển môi trường và URL. Nếu chọn profile http/https trong Visual Studio, các giá trị trong launchSettings.json của profile đó được ưu tiên.

Ví dụ truyền cấu hình trực tiếp từ PowerShell:

~~~powershell
$env:ConnectionStrings__DefaultConnection = 'Server=localhost,1433;Database=BoardingHouseDB;User Id=<user>;Password=<password>;TrustServerCertificate=True;'
dotnet run --project BoardingHouse/BoardingHouse.API.csproj --no-launch-profile
~~~

## Đưa base code lên Git

.env.example, source code, solution, project, cấu hình mặc định và CI được commit. .env/.env.*, cấu hình local, bin/obj, cache, log, chứng chỉ/private key và file IDE cá nhân bị .gitignore bỏ qua. .gitattributes chuẩn hóa line ending khi Git lưu file.

Với thư mục chưa có Git repository:

~~~powershell
git init
git branch -M main
git add .
git status --short
git commit -m "chore: initialize Clean Architecture base"
git remote add origin <repository-url>
git push -u origin main
~~~

Thay <repository-url> bằng URL repository của bạn. Khi kiểm tra git status, .env và file có credential local không xuất hiện. Không dùng git add -f với các file secret.

.github/workflows/ci.yml tự restore, build Release và chạy tests khi push/pull request trên GitHub; CI không cần .env hoặc SQL Server thật vì tests dùng dữ liệu giả và kiểm tra EF model/query translation. Chưa chạy workflow trên GitHub trong workspace này.

## Publish

~~~powershell
dotnet publish BoardingHouse/BoardingHouse.API.csproj -c Release -o artifacts/publish
~~~

.env và cấu hình local không được đóng gói trong output/publish. Khi triển khai, truyền ConnectionStrings__DefaultConnection, ASPNETCORE_ENVIRONMENT=Production và các Cors__AllowedOrigins__<index> của frontend thật qua biến môi trường/secret store. Nếu dùng file .env trên server, tạo file riêng cho server sau khi publish; không sao chép .env Development của workspace.

## API mẫu

Hai endpoint phòng hiện là public, chưa yêu cầu token. Đây là use case đọc mẫu để mở rộng nghiệp vụ; JWT và phân quyền chưa được triển khai trong bộ khung này. Request GET không có body; dùng Accept: application/json. JSON trả về dùng camelCase và DTO, không serialize entity/navigation property.

### GET http://localhost:5154/api/rooms?pageNumber=1&pageSize=20

pageNumber mặc định 1, tối thiểu 1. pageSize mặc định 20, từ 1 đến 100. Offset phân trang phải nằm trong Int32. Sắp xếp theo roomNumber, sau đó id. Trả về danh sách tất cả trạng thái phòng.

200 application/json, ví dụ minh họa:

~~~json
{
  "items": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "floorId": "22222222-2222-2222-2222-222222222222",
      "roomNumber": "A101",
      "area": 25,
      "basePrice": 3000000,
      "maxOccupants": 2,
      "description": "Phòng có ban công",
      "status": "Available"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1
}
~~~

Danh sách trống trả 200 với items: [], totalCount: 0, totalPages: 0.

### GET http://localhost:5154/api/rooms/11111111-1111-1111-1111-111111111111

200 application/json trả về một object phòng có các field giống phần tử trong items. Không có phòng trả 404.

### Lỗi

Phân trang không hợp lệ: 400 application/problem+json. Ví dụ:

~~~json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation failed",
  "status": 400,
  "detail": "pageNumber must be >= 1, pageSize must be between 1 and 100, and the pagination offset must fit in Int32.",
  "instance": "/api/rooms",
  "traceId": "<request-trace-id>"
}
~~~

404 có title Resource not found và detail Room '<id>' was not found. Lỗi SQL Server hoặc lỗi ngoài dự kiến trả 500 với thông báo chung; stack trace được ghi vào log console/debug. Query không parse được (ví dụ pageNumber=abc) trả 400 ValidationProblemDetails của ASP.NET Core, có field errors.

## Database và EF tooling

Model/mapping được giữ từ database-first hiện có; thay đổi kiến trúc không tạo hoặc sửa schema. Startup không tự tạo database hoặc áp migration. Chạy API đọc phòng cần database đã có các bảng tương ứng.

Local EF CLI được pin cùng phiên bản EF Core:

~~~powershell
dotnet tool restore
dotnet build BoardingHouse.sln
dotnet ef dbcontext info --project BoardingHouse.Infrastructure --startup-project BoardingHouse --no-build
~~~

Nếu chuyển database hiện có sang migration-first, xác nhận schema và tạo migration baseline trước khi áp dụng migration. Không áp một migration tạo toàn bộ schema lên database đã tồn tại.

## Mở rộng feature

1. Thêm hoặc cập nhật entity/rule ở Domain; tên class và file phải có hậu tố Entity.
2. Thêm DTO, service/interface và repository abstraction ở Application.
3. Thêm model database ở Infrastructure/Models, cấu hình EF trong DbContext, và mapping model ↔ domain entity trong Infrastructure/Mappings. Triển khai repository, đăng ký trong AddInfrastructure.
4. Thêm controller gọi application interface và đăng ký service trong AddApplication.
5. Với thao tác ghi, dùng cùng scoped DbContext và IUnitOfWork; tránh business logic hoặc EF query trực tiếp trong controller.
6. Thêm test cho rule/use case có hành vi cần bảo vệ.

Test hiện có kiểm tra phân trang, mapping DTO, cancellation token, phòng không tồn tại, chiều phụ thuộc các tầng, DI, hậu tố Entity và việc EF chỉ dùng 16 model database trong Infrastructure và projection Room → RoomEntity được dịch thành SQL. Tests không kết nối SQL Server thật.

Tài liệu tham chiếu: [Microsoft — Common web application architectures](https://learn.microsoft.com/en-us/dotnet/standard/modern-web-apps-azure-architecture/common-web-application-architectures), [DotNetEnv](https://www.nuget.org/packages/DotNetEnv/3.1.1), [GitHub Actions setup-dotnet](https://github.com/actions/setup-dotnet).
