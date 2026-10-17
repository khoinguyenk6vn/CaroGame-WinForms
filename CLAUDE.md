# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Tổng quan

Đồ án môn "Lập trình trên môi trường Windows" của nhóm 4 sinh viên: game Caro (Gomoku) bàn 20x20, viết bằng C# WinForms + ASP.NET Core SignalR + Entity Framework Core + SQL Server.

Ba chế độ: PvE (đánh với bot), PvP Offline (2 người chung máy), PvP Online (xếp hạng). Có tài khoản, rank, bảng xếp hạng, lịch sử đấu.

Trao đổi với người dùng bằng tiếng Việt. Các quyết định trong file này đã được nhóm chốt: không tự thay đổi, chỗ nào thiếu hoặc mâu thuẫn thì hỏi lại. Nguyên tắc chung khi phải chọn: cách đơn giản nhất, dễ giải thích với giảng viên. Phần nào không nêu thì chưa làm.

## Trạng thái hiện tại

Ngày 2026-10-17: đã xong phần code của bước 0–6 và 8. Còn lại việc người dùng/nhóm phải tự làm: thử Cloudflare tunnel (bước 4, 7), triển khai máy host (bước 7), thêm control cho `frmRuleVote`, file âm thanh, Gmail App Password. Solution đã tách 3 tầng GUI → BLL → DAL (xem phần Kiến trúc). Giao diện các Form nằm trong `CaroGame.UI/Forms/` chia 3 thư mục `Account`, `Match`, `Menu`. App khởi động bằng `frmStart`.

- **Online (bước 6), BLL `Online/`:** `OnlineGameManager` (1 đối tượng duy nhất: hàng chờ ghép trận + danh sách trận, có lock), `OnlineMatch` (1 trận, có lock riêng; mọi sự kiện đi qua đây; các tin cần gửi gom vào outbox rồi `TakeNotifications()`), `IClientNotifier`; các class dữ liệu `OnlinePlayer`, `WaitingPlayer`, `MatchPhase`, `Notification` nằm ở BLL `Models/` (BLL không biết SignalR; Server cài đặt bằng `Hubs/SignalRNotifier`). Server có `GameTickService` (BackgroundService) gọi `OnlineGameManager.TickAsync()` mỗi giây: tăng thời gian chờ, ghép trận, chạy đồng hồ, đếm các hạn 15 giây (hỏi luật, Undo) và 30 giây (kết nối lại). Các hạn này đếm bằng số giây nguyên trong `Tick()`, không dùng DateTime.
- Trận Online tạm dừng (không ai đánh được, đồng hồ dừng) khi đang chờ trả lời Undo hoặc có người mất kết nối. Hết trận thì `OnlineGameManager` tính điểm (`ScoreCalculator`), lưu điểm và 2 dòng lịch sử, gửi `GameOver` cho từng người; người mất kết nối quá 30 giây bị giải phóng phiên sau khi xử xong.
- Tin server gửi client (tên sự kiện SignalR): `SearchTime`, `MatchFound`, `GameStarted`, `MoveMade`, `MoveUndone`, `ClockUpdate`, `UndoRequested`, `UndoAnswered`, `UndoTimeout`, `ChatMessage`, `OpponentDisconnected`, `OpponentReconnected`, `GameOver`, `MatchCanceled`. Tin không có dữ liệu vẫn gửi kèm 1 chuỗi (client đăng ký `On<string>`).
- Client Online: `frmGame` có 2 constructor (PvE/Offline nhận `GameMode`; Online nhận `MatchFoundDto`), Online dùng `OnlineMatchState` (BLL) và chỉ vẽ theo tin server gửi về. Mọi handler SignalR đưa qua `BeginInvoke` (không dùng `Invoke`, để không chặn luồng nhận tin khi đang mở hộp thoại Undo). `frmModeSelect` lặp: `frmMatchmaking` → `frmGame` → nếu `FindNewMatch` thì tìm tiếp. `ServerConnection` bật `WithAutomaticReconnect`, kết nối lại xong gửi `Reconnect(token)`; server trả false thì client thành Guest.
- **`frmRuleVote` vẫn trống** (chờ nhóm kéo `btnAgree`, `btnDecline`, `lblQuestion`, `lblCountdown`): hiện client không gửi phiếu, server hết 15 giây tự chơi luật thường. Khi có control thì `frmGame` (online) mở `frmRuleVote` lúc Load và gọi Hub `VoteRule(bool)`.
- **Bước 5:** `PlayerService` (Profile, lịch sử, BXH, lưu kết quả PvE/Offline), `AvatarService` (upload qua endpoint HTTP `POST /avatar`, header `X-Session-Token`, lưu ở `CaroGame.Server/wwwroot/avatars`, không commit), `RankCalculator`, `GameTexts` (đổi enum ra chữ tiếng Việt). DAL thêm `MatchHistory`, `MatchHistoryDAL`, `UserDAL.GetRankingAsync()` (đếm trận thắng Online bằng LINQ). Lịch sử/BXH hiện bằng `DataGridView.DataSource` = list DTO, tên cột lấy từ `[DisplayName]`.
- DAL dùng `IDbContextFactory<AppDbContext>`: mỗi hàm tự tạo 1 context rồi hủy, nên các class DAL/BLL đều đăng ký Singleton trong `Program.cs` của Server.
- Avatar mặc định: 20 ảnh `CaroGame.UI/Avatars/avatar_0..19.png` tải từ DiceBear style Thumbs (CC0, nhóm nên xem lại giấy phép trên trang DiceBear), copy ra thư mục chạy; `Services/AvatarLoader` (GUI) đọc ảnh mặc định, tải ảnh đã upload, upload ảnh.
- **Bước 8:** `Services/SoundManager` (GUI) dùng `MediaPlayer` của WPF (bật `<UseWPF>` trong csproj, không phải NuGet) để phát nhạc nền và tiếng đánh quân cùng lúc. File đặt ở `CaroGame.UI/Sounds/move.mp3`, `music.mp3` (chưa có, thiếu thì không phát). Bật `UseWPF` làm mất `using System.IO`/`System.Net.Http` tự động, file nào dùng `File`, `Path`, `HttpClient` phải tự thêm using. `frmInformation` đã có nội dung hướng dẫn (viết trong code).
- Giới hạn đã biết: tin gửi tới client trong lúc client đang mất kết nối sẽ bị mất (nước đi thì không, vì trận tạm dừng; chat thì có thể mất).

- **Thư mục DAL/BLL (sắp xếp ngày 2026-10-17 theo mẫu Buoi7 của môn, namespace = đường dẫn thư mục, chỉ chuyển file và sửa using/namespace, không đổi logic):**
  - DAL: gốc `AppDbContext.cs` (namespace `CaroGame.DAL`); `Entities/` (`User`, `VerificationCode`, `MatchHistory`, namespace `CaroGame.DAL.Entities`); `DataAccess/` (`UserDAL`, `VerificationCodeDAL`, `MatchHistoryDAL`, namespace `CaroGame.DAL.DataAccess`); `Models/` (`UserRanking`: kết quả truy vấn, không phải bảng; namespace `CaroGame.DAL.Models`); `Migrations/`.
  - **Ngoại lệ:** `AppDbContext` và `Entities` GIỮ namespace cũ vì snapshot Migration tham chiếu chuỗi tên đầy đủ như `"CaroGame.DAL.Entities.User"`; đổi namespace thì EF coi là model thay đổi và đòi tạo Migration mới. Không dùng tên thư mục `Repositories` (dễ hiểu nhầm là Generic Repository).
  - BLL: `Game/` (luật chơi và các class có xử lý: `Board`, `GameRules`, `GameClock`, `LocalMatch`, `CaroBot`, `ScoreCalculator`, `RankCalculator`, `GameTexts`, `OnlineMatchState`); `Online/` (`OnlineGameManager`, `OnlineMatch`, `IClientNotifier`); `Services/` (`AccountService`, `PlayerService`, `AvatarService`, `EmailService`, `BotChatService`, `SessionStore`, `Session`); `Models/` (enum và class chỉ chứa dữ liệu: `Stone`, `GameMode`, `EndReason`, `MatchResult`, `Move`, `MatchPhase`, `OnlinePlayer`, `WaitingPlayer`, `Notification`, `OnlineUser`; namespace `CaroGame.BLL.Models`); `Settings/` (`EmailSettings`, `AvatarSettings`, `ClientSettings` + `LlmSettings`, `SoundSettings`; namespace `CaroGame.BLL.Settings`); `Dtos/`.
  - Class có hàm xử lý thì không đưa vào `Models/`. `ClientSettings` (có `Load()` đọc file) và `SoundSettings` (có hàm bật/tắt) nằm ở `Settings/` vì là cấu hình.
  - Dùng kiểu ở thư mục khác thì phải `using` namespace tương ứng (ví dụ `Board.cs` dùng `Stone` nên có `using CaroGame.BLL.Models;`). DAL không được `using` gì của BLL (entity có thuộc tính tên `EndReason` nhưng là chuỗi, không phải enum của BLL).
- **BLL `Game/`:** `Board` (bàn cờ + danh sách nước để Undo), `GameRules` (nước hợp lệ, thắng, chặn 2 đầu), `GameClock` (đồng hồ 180 giây, đếm bằng số giây nguyên, mỗi giây gọi `TickOneSecond()`), `LocalMatch` (một trận PvE/Offline: lượt, thắng/hòa, hết giờ, Undo, FF, tên 2 người chơi), `CaroBot` (bot chấm điểm ô trống). Các enum `Stone`, `GameMode`, `EndReason` nằm ở `Models/`.
- **BLL `Services/`:** `AccountService` (đăng ký, đăng nhập, mã xác minh, đổi mật khẩu; gọi `UserDAL`/`VerificationCodeDAL`), `EmailService` (Gmail SMTP, cấu hình ở `Settings/EmailSettings`; để trống Gmail thì in mã ra cửa sổ server), `SessionStore` (dùng `Models/OnlineUser`; static, danh sách người đang online trong RAM, có lock), `BotChatService` (gọi LLM kiểu OpenAI, lỗi gì cũng trả câu soạn sẵn), `Settings/ClientSettings` (đọc `clientsettings.json` cạnh file .exe), `Session` (static, người đang chơi trên máy client, sinh tên Guest), `Settings/SoundSettings` (static, bật/tắt âm thanh và nhạc nền).
- **BLL `Dtos/`:** `AccountResult`, `LoginResult`, `CodePurpose` (truyền qua SignalR giữa Server và GUI).
- **DAL:** `AppDbContext`, `Entities/User`, `Entities/VerificationCode` (cột `Purpose` lưu dạng chữ "Register"/"ResetPassword"), `Entities/MatchHistory`, `DataAccess/UserDAL`, `DataAccess/VerificationCodeDAL`, `DataAccess/MatchHistoryDAL`, `Models/UserRanking`, `Migrations/` (`InitialCreate`, `AddMatchHistory`).
- **Server:** `Program.cs` (đăng ký DbContext, các class DAL/BLL), `Hubs/GameHub.cs` (map ở `/gamehub`, chỉ chuyển yêu cầu cho `AccountService`).
- **GUI:** các Form và `Services/ServerConnection.cs` (static, 1 `HubConnection` cho cả app). Các Form tài khoản gọi Hub bằng `InvokeAsync<T>("TênHàm", ...)` trong try/catch, lỗi kết nối thì hiện "Không kết nối được server." ở `lblError`. Kiểm tra dữ liệu nhập (kể cả "nhập lại mật khẩu có khớp không") đều do BLL ở server làm.
- `frmGame` (PvE, Offline) và `frmResult` đã có logic. `frmGame` dùng `Player1Timer` của Designer (1 giây/lần, chạy suốt trận) để gọi `match.TickClock()`; `Player2Timer` không dùng. Lý do kết thúc tạm hiện trên thanh tiêu đề `frmResult` cho tới khi có `lblReason`.
- Luồng màn hình: `frmStart` (Chơi ngay → Guest; Đăng nhập; `toolStripButton1` là icon Settings, `toolStripButton2` là icon Information) → `frmModeSelect` (menu chính; đóng menu chính là thoát app; bấm `picAvatar` mở `frmProfile`; đăng xuất trong Profile thì `LoggedOut = true` và quay về `frmStart`). Guest bấm PvP Online hoặc BXH thì mở `frmLogin`.
- `frmSettings` có 2 nút `btnSound`, `btnMusic` (đã đổi từ CheckBox ngày 2026-10-17, nhóm cho phép Claude sửa Designer lần này): bấm là bật/tắt và đổi chữ "Âm thanh: Bật/Tắt", "Nhạc nền: Bật/Tắt", không làm gì thêm.
- Form tài khoản mở nhau bằng `ShowDialog(this)` chồng lên nhau (không Hide form đang là dialog). `frmVerifyCode(email, purpose)` dùng chung cho đăng ký và quên mật khẩu, trả mã đã nhập đúng qua `VerifiedCode`. `frmForgotPassword.btnContinue` không dùng (chỉ dùng `btnSendCode`).
- Email và Username đều unique (cần cho quên mật khẩu theo email). Username so trùng không phân biệt hoa thường nhờ collation mặc định của SQL Server.
- Gửi mã lỗi (ví dụ sai Gmail/App Password): đăng ký thì xóa user vừa tạo (`UserDAL.DeleteAsync`) để đăng ký lại được; đăng nhập tài khoản chưa xác minh mà gửi lỗi và không còn mã hợp lệ thì báo lỗi, không mở form nhập mã; mã cũ chỉ bị xóa sau khi gửi mã mới thành công; lý do thật in ra cửa sổ server dòng `[Loi gui mail]`. Lưu ý: `SmtpClient` báo sai mật khẩu Gmail thành "connection was closed" chứ không phải 535, muốn biết chắc phải thử đăng nhập SMTP trực tiếp.
- Quên mật khẩu: `CheckResetCode` chỉ kiểm tra mã (không xóa), `ResetPassword` kiểm tra mã lần nữa rồi mới đổi mật khẩu và xóa mã.
- Ngoài trận, mất kết nối là server xóa phiên ngay (đúng luật đã chốt); khi SignalR tự kết nối lại, server trả `Reconnect` = false và client thành Guest. Trong trận thì giữ phiên 30 giây.
- Các TextBox ở Form tài khoản đang có chữ gợi ý nằm trong `Text` ("Username", "Password"...), người chơi phải tự xóa. Nhóm nên chuyển sang thuộc tính `PlaceholderText` trong Designer.
- Namespace khớp thư mục: `CaroGame.UI.Forms.Account`, `CaroGame.UI.Forms.Match`, `CaroGame.UI.Forms.Menu`, `CaroGame.UI.Services`, `CaroGame.UI.Properties`. Form ở thư mục khác phải `using` namespace tương ứng (ví dụ `frmModeSelect` dùng `CaroGame.UI.Forms.Match` để mở `frmGame`). Tên resource nhúng trong .exe đi theo namespace (`CaroGame.UI.Forms.Menu.frmStart.resources`, `CaroGame.UI.Properties.Resources`).
- `Resources/Nham_Coffe.jpg` (khai báo trong `Properties/Resources.resx`) là ảnh avatar tạm nhóm gán trong Designer cho `frmGame`, `frmModeSelect`, `frmProfile`. Nhóm quyết định giữ nguyên, không xóa.
- Form còn thiếu control: `frmRuleVote` đang trống (thiếu cả nút, đang chặn phần bỏ phiếu); `frmUndoRequest` thiếu label đếm ngược 15 giây; `frmResult` thiếu `lblReason` (lý do đang tạm hiện trên thanh tiêu đề). Các nút của `frmResult` và `frmUndoRequest` được gắn sự kiện Click trong constructor.

## Lệnh

Chạy từ thư mục gốc (cần .NET 10 SDK, chỉ Windows).

```powershell
dotnet build CaroGame.slnx
dotnet run --project CaroGame.UI/CaroGame.UI.csproj   # client (GUI)
dotnet run --project CaroGame.Server/CaroGame.Server.csproj       # server, http://localhost:5023

# Migration nằm trong DAL, startup project là Server (để đọc connection string)
# Cần cài 1 lần: dotnet tool install --global dotnet-ef --version 10.0.13
dotnet ef migrations add <TenMigration> --project CaroGame.DAL --startup-project CaroGame.Server
dotnet ef database update --project CaroGame.DAL --startup-project CaroGame.Server
```

Chưa có project test và linter.

File cấu hình riêng của từng máy (nằm trong `.gitignore`, mỗi thành viên tự sao chép từ file mẫu rồi điền):

- `CaroGame.Server/appsettings.Local.json` từ `appsettings.Local.example.json`: connection string, Gmail và App Password. `Program.cs` của Server nạp file này bằng `AddJsonFile(..., optional: true)`.
- `CaroGame.UI/clientsettings.json` từ `clientsettings.example.json`: địa chỉ server, base URL, model và API key của LLM. File được copy ra thư mục output khi build, đọc bằng `ClientSettings.Load()`. API key thật chỉ được để trong file này, KHÔNG để trong file `.example.json` (file mẫu được commit; đã 2 lần key thật bị dán nhầm vào file mẫu, luôn kiểm tra trước khi commit).

## Kiến trúc

Solution `CaroGame.slnx` (định dạng XML, không phải `.sln`), cả 4 project dùng .NET 10. Mô hình 3 tầng do nhóm chốt ngày 2026-10-17, vẫn giữ Server SignalR cho tài khoản và Online:

```
CaroGame.UI (GUI) ──tham chiếu──► CaroGame.BLL ──tham chiếu──► CaroGame.DAL ──EF Core──► SQL Server
        │                                    ▲
        └──── SignalR ────► CaroGame.Server ─┘ (tham chiếu BLL)
```

- **`CaroGame.DAL`** (class library): nơi DUY NHẤT giao tiếp CSDL, bằng EF Core (Code First, `AppDbContext : DbContext`, entity, Migrations, LINQ to Entities, mọi thao tác async/await). Mỗi bảng có 1 class DAL riêng trong thư mục `DataAccess/` (`UserDAL`, `VerificationCodeDAL`, `MatchHistoryDAL`), KHÔNG dùng Generic Repository. Entity ở `Entities/`, kết quả truy vấn không phải bảng ở `Models/`. `AppDbContext` và `Entities` giữ namespace cũ (`CaroGame.DAL`, `CaroGame.DAL.Entities`) để không phải tạo Migration mới. **Mọi gói NuGet của solution chỉ cài ở DAL**; các project khác dùng qua tham chiếu bắc cầu.
- **`CaroGame.BLL`** (class library, tham chiếu DAL): chứa TOÀN BỘ logic nghiệp vụ: luật chơi, bot, đồng hồ, tính điểm, tài khoản, mã xác minh, phiên đăng nhập, chat bot, DTO. Thư mục: `Game/`, `Online/`, `Services/` (class có xử lý), `Models/` (enum và class chỉ chứa dữ liệu), `Settings/` (cấu hình), `Dtos/` (truyền qua SignalR). KHÔNG dùng `AppDbContext`, `DbSet`, LINQ trên CSDL: muốn đọc/ghi dữ liệu thì gọi class DAL.
- **`CaroGame.UI`** (GUI, tham chiếu BLL): project giao diện, chứa toàn bộ Form. Ngày 2026-10-17 nhóm đổi tên từ `CaroGame-WinForms` (namespace cũ `CaroGame_WinForms`) thành `CaroGame.UI`, root namespace `CaroGame.UI`; solution đổi thành `CaroGame.slnx`. Tên repo trên GitHub vẫn là `CaroGame-WinForms`. Chỉ nhận thao tác và hiển thị: KHÔNG chứa logic nghiệp vụ, KHÔNG truy vấn CSDL, KHÔNG dùng class DAL, KHÔNG chứa connection string. PvE và PvP Offline chạy hoàn toàn ở client (gọi `LocalMatch` của BLL). Tài khoản, BXH, Online gọi Server qua SignalR.
- **`CaroGame.Server`** (ASP.NET Core + SignalR, tham chiếu BLL): Hub chỉ nhận yêu cầu rồi chuyển cho BLL, không chứa nghiệp vụ. `Program.cs` chỉ khai báo kết nối CSDL và đăng ký class, không truy vấn. Là nơi quyết định duy nhất cho trận Online.
- **KHÔNG dùng ADO.NET** trong toàn repo: không `SqlConnection`, `SqlCommand`, `SqlDataAdapter`, `SqlDataReader`, `DataTable`, `DataSet`, `FromSqlRaw`, `ExecuteSqlRaw`... (EF Core bên dưới vẫn dùng driver SQL Server, đó là việc của thư viện, không phải code của nhóm).

Giao tiếp:

- Đăng ký, đăng nhập, ghép trận, nước đi, chat đi qua SignalR Hub. Upload avatar là 1 endpoint HTTP POST riêng, gửi kèm session token.
- Không dùng JWT. Đăng nhập qua Hub, server sinh session token (GUID) lưu trong RAM và gắn userId với ConnectionId. Khi SignalR tự kết nối lại, client gửi token để server gắn lại.
- Tài khoản đang online thì từ chối đăng nhập ở máy khác ("Tài khoản đang đăng nhập ở nơi khác"). Phiên được giải phóng ngay khi mất kết nối nếu đang ngoài trận; nếu đang trong trận thì giải phóng sau khi hết 30 giây chờ và đã xử FF.
- Trận Online chỉ giữ trong RAM server: mỗi trận là một object có lock, mọi sự kiện đi qua một chỗ xử lý. Server khởi động lại thì trận đang chơi bị hủy, không tính điểm.

Triển khai và cấu hình:

- Server host trên máy một thành viên, public qua Cloudflare named tunnel ở subdomain của `vernfy.com` (ví dụ `caro.vernfy.com`). Dự phòng khi demo: chạy server + DB trên laptop demo, client trỏ localhost.
- Dev: mỗi người dùng SQL Server LocalDB, tạo DB bằng Migration. Máy host demo dùng SQL Server Express.
- Địa chỉ server nằm trong file cấu hình của client, đổi được mà không build lại.
- Secrets (connection string, Gmail + App Password, API key LLM) đặt trong file cấu hình riêng, thêm vào `.gitignore`, kèm file mẫu `*.example` để thành viên tự điền. KHÔNG commit secrets.

## Luồng màn hình

- **Mở app:** tiêu đề "Caro", nút Chơi ngay và Đăng nhập, icon Information (hướng dẫn cuộn dọc) và Settings (chỉ bật/tắt âm thanh và nhạc nền bằng 2 nút bấm). Không có đổi ngôn ngữ.
- **Guest:** tên tự sinh `Guest12345`, không lưu gì (kể cả file cục bộ), không kết nối server. Chơi được PvE và Offline. Chọn PvP Online hoặc bảng xếp hạng thì yêu cầu đăng nhập. Bấm avatar hiện tên Guest và nút "Đăng nhập để lưu lịch sử".
- **Đăng ký:** username, email (bất kỳ), mật khẩu, nhập lại mật khẩu. Username 4–20 ký tự gồm chữ không dấu, số, `_`, kiểm tra trùng không phân biệt hoa thường, không đổi được. Mật khẩu 6–50 ký tự, lưu hash BCrypt.
- **Mã email 6 số** (đăng ký và quên mật khẩu như nhau): hết hạn 5 phút, gửi lại phải chờ 60 giây, sai 5 lần thì mã bị hủy. Gửi bằng Gmail SMTP với App Password.
- **Tài khoản chưa xác minh:** lưu DB với `IsEmailVerified = false`. Đăng nhập thì gửi mã mới và chuyển sang form nhập mã, chưa cho vào game.
- **Quên mật khẩu:** nhập email, nhập mã, đặt mật khẩu mới.
- **Chọn chế độ:** PvE, PvP Online, PvP Offline, avatar góc trên phải mở Profile, lối vào bảng xếp hạng.
- **Profile:** avatar, tên, điểm, bậc rank, nút Lịch sử đấu, nút Đăng xuất. Không có ghi nhớ đăng nhập.
- **Lịch sử:** trận Online hiện chế độ + điểm cộng/trừ; PvE/Offline chỉ hiện chế độ. User đã đăng nhập thì client gửi kết quả PvE/Offline lên server sau mỗi trận; mất kết nối thì bỏ qua.
- **Bảng xếp hạng:** top 50, dòng cuối luôn là thứ hạng của chính mình. Bằng điểm thì xếp theo số trận thắng Online, rồi username theo chữ cái.
- **Avatar:** mặc định là 1 trong khoảng 20 ảnh PNG DiceBear (style CC0) đóng gói trong resources của client; lúc đăng ký chọn ngẫu nhiên và lưu số thứ tự vào DB. Guest ngẫu nhiên mỗi lần mở app. Upload chỉ `.png`/`.jpg`, tối đa 1MB; server lưu file, DB lưu đường dẫn, upload mới thì xóa file cũ.
- **Trận đấu:** bàn cờ bên trái; nút Settings trong trận (Undo, FF) góc trên trái; bên phải là 2 thẻ người chơi (avatar, tên, đồng hồ) và chat box.
- **Hết trận:** hộp thoại kết quả gồm Thắng/Thua/Hòa, lý do kết thúc, điểm thay đổi (chỉ Online), nút "Chơi lại" (PvE/Offline) hoặc "Tìm trận mới" (Online), và "Về menu".
- **Âm thanh:** chưa có file. Thiết kế sẵn chỗ phát, thiếu file thì game vẫn chạy bình thường, không báo lỗi. Làm sau cùng.

## Luật chơi

- X luôn đi trước. PvE: người chơi cầm X. Offline: người chơi 1 cầm X, người chơi 2 tên "Người chơi 2" với avatar mặc định. Online: server chọn ngẫu nhiên.
- Từ 5 quân liên tiếp trở lên (ngang, dọc, chéo) là thắng.
- **Chặn 2 đầu:** hàng bị quân đối thủ nằm sát ngay cả 2 đầu thì không tính thắng. Mép bàn cờ KHÔNG tính là chặn. PvE và Offline luôn áp dụng. Online hỏi cả 2 người trước trận (hạn 15 giây, không trả lời là không đồng ý, đồng hồ chưa chạy); chỉ cần 1 người không đồng ý thì chơi luật thường.
- **Đồng hồ:** mỗi người tổng 180 giây cho cả trận, chỉ chạy trong lượt mình, áp dụng ở PvP Offline và PvP Online. PvE không tính giờ (nhóm đổi ngày 2026-10-16): đồng hồ vẫn hiện nhưng đứng yên. Đồng hồ của X chạy ngay khi trận bắt đầu. Hết giờ là thua.
- **Hòa:** bàn cờ đầy. Không có xin hòa.
- **Undo:**
  - PvE: lùi 2 nước (bot + mình), không hỏi.
  - Offline: lùi 1 nước, trả lượt cho người vừa đi, không hỏi.
  - Online: chỉ xin được ngay sau nước của mình, trước khi đối thủ đi; lùi 1 nước. Lời xin có hạn 15 giây, đồng hồ cả 2 dừng trong lúc chờ, không trả lời tính là từ chối. Bị từ chối 3 lần trong 1 trận thì hết quyền xin. Không giới hạn số lần được chấp nhận. Không hoàn lại thời gian đã dùng.
- **FF:** PvE là người chơi đầu hàng; Offline là người đang tới lượt đầu hàng. Không có nút thoát riêng: thoát trận chính là FF (Online bị −100).
- **Bot:** một mức độ khó, thuật toán chấm điểm từng ô trống chạy trên client. KHÔNG dùng LLM để chọn nước đi. Bot chờ khoảng 1 giây rồi mới đánh.
- **Chat:**
  - Online: realtime qua server, tối đa 200 ký tự, không lưu DB, không lọc từ ngữ.
  - PvE (kể cả Guest): client gọi trực tiếp LLM qua API tương thích OpenAI. Mặc định Google Gemini `gemini-3.5-flash-lite` free tier với `"ReasoningEffort": "minimal"` (trả lời khoảng 1 giây; đo ngày 2026-10-17: `gemini-3.1-flash-lite` mất 3–19 giây và hay lỗi 503 quá tải), dự phòng Groq (để trống `ReasoningEffort`); base URL, model và API key nằm trong file cấu hình để đổi nhà cung cấp không sửa code. Chỉ gửi nội dung chat và tình hình trận, KHÔNG gửi email hay thông tin cá nhân. Câu trả lời giới hạn 1–2 câu. Lỗi 429, 503, timeout (12 giây) hoặc mất mạng thì thử lại 1 lần rồi dùng câu soạn sẵn; `BotChatService.LastError` giữ lý do bằng tiếng Việt và `frmGame` hiện 1 dòng "(Bot đang dùng câu soạn sẵn vì ...)" trong khung chat (mỗi lý do 1 lần). `max_tokens` = 1024 vì model có thinking tốn token trước khi trả lời. Lưu ý: `clientsettings.json` bị git bỏ qua nên chuyển nhánh có đổi tên thư mục project thì file không đi theo, game sẽ báo "chưa có API key".
  - Offline: chỉ hiển thị tin nhắn, không có phản hồi.

## Điểm rank (chỉ PvP Online)

Nhóm gọi là "Elo" nhưng là hệ điểm cố định theo số nước, không phụ thuộc điểm đối thủ.

- Khởi đầu 1000, không bao giờ dưới 1000. Lịch sử ghi số điểm thực tế thay đổi (đang 1000 mà thua thì ghi 0).
- n = số quân trên bàn lúc kết thúc (undo làm giảm n).
- Thắng: n ≤ 25 được +50; n > 25 được `max(35, 50 − floor((n − 25) / 2))`. Thắng do đối thủ FF, hết giờ, mất kết nối cũng tính theo công thức này.
- Thua (kể cả hết giờ): n ≤ 25 bị −50; n > 25 bị trừ `max(25, 50 − floor((n − 25) / 2))`.
- Hòa: 0. FF hoặc mất kết nối quá 30 giây: −100.
- Đối thủ thoát lúc đang hỏi luật hoặc trước nước đi đầu tiên: hủy trận, không ai đổi điểm, người còn lại về màn tìm trận.
- **Mất kết nối giữa trận:** có 30 giây, đồng hồ cả 2 tạm dừng. Chỉ hỗ trợ kết nối lại tự động của SignalR; tắt hẳn app thì không vào lại được, hết 30 giây xử như FF.
- **Bậc rank:** 9 hạng từ thấp lên cao là Sắt, Đồng, Bạc, Vàng, Bạch Kim, Kim Cương, Lục Bảo, Trái Đất, Mặt Trời. Mỗi hạng 3 bậc I → II → III (I thấp nhất), mỗi bậc 100 điểm, bắt đầu từ Sắt I = 1000. Mặt Trời III từ 3600 trở lên, không giới hạn trên.
- **Ghép trận:** chỉ auto matchmaking. Ban đầu tìm trong ±100 điểm, mỗi 10 giây mở rộng thêm ±100, sau 60 giây ghép với bất kỳ ai đang chờ. Có nút hủy tìm trận.

## Dữ liệu

- **User:** Id, username, email, mật khẩu hash, `IsEmailVerified`, điểm, số thứ tự avatar mặc định, đường dẫn avatar upload, `CreatedAt`. Số trận thắng/thua KHÔNG lưu cột riêng, tính bằng LINQ từ lịch sử (truy vấn đó viết ở DAL).
- **Lịch sử trận:** người chơi, đối thủ (trận Offline để null, hiển thị "Người chơi 2"), chế độ, kết quả, lý do kết thúc (`FiveInRow`, `Timeout`, `Forfeit`, `Disconnect`, `Draw`), cờ có áp dụng luật chặn 2 đầu, số nước, điểm thay đổi (chỉ Online), thời gian. Không lưu danh sách nước đi, không có xem lại trận.
- **Mã xác minh email:** mã, mục đích (đăng ký / quên mật khẩu), hạn dùng.

## Cách làm việc với nhóm

- Nhóm tự thiết kế toàn bộ Form bằng WinForms Designer. Claude chỉ viết logic.
- Trước khi viết logic cho một Form, đọc file `.cs` và `.Designer.cs` của Form đó để lấy đúng tên control. KHÔNG đoán tên control.
- KHÔNG sửa `.Designer.cs`. KHÔNG tự tạo control bằng code. Ngoại lệ: bàn cờ 20x20 (nhóm đặt 1 Panel, Claude vẽ bàn cờ và xử lý click bằng code); và khi nhóm cho phép rõ ràng cho từng lần (đã có 2 lần: đổi CheckBox thành Button trong `frmSettings`; đổi namespace trong mọi `.Designer.cs` khi đổi tên project thành `CaroGame.UI`).
- Thiếu Form hoặc control tương tác (nút, ô nhập, checkbox...) thì DỪNG và báo nhóm, liệt kê: Form nào, loại control, tên đề xuất (tiền tố `btn`, `txt`, `lbl`, `pnl`, `pic`, `lst`...), dùng để làm gì. Chờ nhóm kéo xong mới viết tiếp.
- Thiếu label chỉ để hiển thị (đếm ngược, lý do kết thúc...) thì KHÔNG chặn: viết logic trước (ví dụ hết 15 giây vẫn tự tính là từ chối), báo nhóm label còn thiếu, nối phần hiển thị khi nhóm kéo xong.
- Trong mỗi bước của lộ trình, làm phần tương tác (click, tick, text-change) trước, phần hiển thị thông báo sau.
- Sự kiện chưa được gắn trong Designer thì gắn trong constructor của file `.cs` (`btnX.Click += btnX_Click;`), không sửa `.Designer.cs`.
- Mỗi Form chỉ 1 người sửa, để tránh xung đột `.Designer.cs` và `.resx` trên git.
- Phần nhóm có thể chưa biết làm (cài SQL Server, chạy Migration, tạo Gmail App Password, Cloudflare Tunnel) thì hướng dẫn từng bước.
- Làm từng phần nhỏ: mỗi lần một Form hoặc một chức năng.
- Sau mỗi phần code, giải thích ngắn: phần này làm gì, luồng chạy từ lúc bấm nút tới kết quả, và 2–3 câu thầy có thể hỏi kèm cách trả lời.
- Chỗ nào bất ổn hoặc chưa chắc thì hỏi lại người dùng trước khi làm (người dùng yêu cầu ngày 2026-10-17).

## Chia Form

Việc kéo Form chia cho 3 người (TV1, TV2, TV3; chưa gắn tên thật). Tên Form và control là đề xuất, luôn đọc `.Designer.cs` để lấy tên thật.

- **TV1 (trận đấu):** `frmGame`, `frmResult`, `frmUndoRequest`, `frmRuleVote`, `frmMatchmaking`.
- **TV2 (tài khoản):** `frmRegister`, `frmLogin`, `frmVerifyCode`, `frmForgotPassword`, `frmResetPassword`.
- **TV3 (menu và hồ sơ):** `frmStart`, `frmInformation`, `frmSettings`, `frmModeSelect`, `frmProfile`, `frmHistory`, `frmLeaderboard`.

## Lộ trình

0. (Xong) Thêm các project vào solution, file cấu hình mẫu, `.gitignore` cho secrets.
1. (Xong) Luật chơi, bàn cờ, PvP Offline: thắng, chặn 2 đầu, đồng hồ, Undo, FF. Cần `frmGame`, `frmResult`.
2. (Xong) PvE: bot, Undo 2 nước, chat câu soạn sẵn rồi tới LLM.
3. (Xong) Khung màn hình: mở app, Guest, chọn chế độ, hướng dẫn, Settings.
4. (Xong, trừ thử Cloudflare tunnel) Server nền: EF Core, Migration, Hub, đăng ký, đăng nhập, mã email, quên mật khẩu. Thử luôn Cloudflare tunnel.
5. (Xong) Profile, avatar, lịch sử, bảng xếp hạng.
6. (Xong, trừ giao diện bỏ phiếu luật vì `frmRuleVote` chưa có control) Online: ghép trận, hỏi luật, nước đi và đồng hồ trên server, chat, Undo, FF và tính điểm, kết nối lại 30 giây.
7. (Người dùng tự làm, hướng dẫn trong README) Triển khai trên máy host, thử 2 máy khác mạng, tập phương án localhost.
8. (Xong phần code, chờ file âm thanh) Âm thanh, sửa lỗi, ôn câu hỏi bảo vệ (README có mục câu hỏi bảo vệ).

## Phong cách code

- Code chỉ làm đúng chức năng được yêu cầu, KHÔNG làm thêm (không thêm hiệu ứng, hộp thoại xác nhận, phím tắt, xử lý cho chế độ chưa làm tới...). Lý do: thầy có thể hỏi lại hoặc bắt nhóm tự viết lại đoạn code đó, nên code phải đủ cơ bản để sinh viên tự viết lại được.
- Dùng cú pháp và thư viện cơ bản: class thường thay vì `record`, không dùng Reflection, không dùng mẹo nâng cao khi có cách đơn giản hơn.
- Ưu tiên dễ đọc hơn ngắn gọn hay "thông minh". Mỗi hàm ngắn, làm đúng 1 việc.
- Tên biến và hàm bằng tiếng Anh rõ nghĩa. Comment tiếng Việt ở chỗ logic quan trọng.
- KHÔNG dùng Generic Repository, Unit of Work tự viết, CQRS/MediatR, AutoMapper hay nhiều tầng abstraction ngoài 3 tầng GUI/BLL/DAL. Class Service ở BLL gọi thẳng class DAL; class DAL dùng thẳng `AppDbContext`.
- Logic game (kiểm tra thắng, chặn 2 đầu, đồng hồ, bot, tính điểm) và mọi nghiệp vụ khác nằm ở BLL. Form chỉ nhận sự kiện và hiển thị.
- Sự kiện từ SignalR chạy trên luồng khác: cập nhật giao diện phải đi qua `Invoke`, và giải thích lý do cho nhóm.
- Khi sửa file bằng PowerShell 5.1 phải đọc/ghi bằng `[IO.File]::ReadAllText` / `WriteAllText` với UTF-8; `Get-Content -Raw` đọc sai tiếng Việt.
