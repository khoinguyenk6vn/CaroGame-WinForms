# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Tổng quan

Đồ án môn "Lập trình trên môi trường Windows" của nhóm 4 sinh viên: game Caro (Gomoku) bàn 20x20, viết bằng C# WinForms + ASP.NET Core SignalR + Entity Framework Core + SQL Server.

Ba chế độ: PvE (đánh với bot), PvP Offline (2 người chung máy), PvP Online (xếp hạng). Có tài khoản, rank, bảng xếp hạng, lịch sử đấu.

Trao đổi với người dùng bằng tiếng Việt. Các quyết định trong file này đã được nhóm chốt: không tự thay đổi, chỗ nào thiếu hoặc mâu thuẫn thì hỏi lại. Nguyên tắc chung khi phải chọn: cách đơn giản nhất, dễ giải thích với giảng viên. Phần nào không nêu thì chưa làm.

## Trạng thái hiện tại

Đã xong bước 0 của lộ trình: solution có đủ 3 project nhưng đều còn là template trống (`Form1` trống, Server chỉ có endpoint "Hello World", Shared chưa có class nào). Chưa có logic game, chưa cài gói NuGet nào (SignalR Client, EF Core, BCrypt sẽ thêm khi tới bước cần dùng).

## Lệnh

Chạy từ thư mục gốc (cần .NET 10 SDK, chỉ Windows).

```powershell
dotnet build CaroGame-WinForms.slnx
dotnet run --project CaroGame-WinForms/CaroGame-WinForms.csproj   # client
dotnet run --project CaroGame.Server/CaroGame.Server.csproj       # server, http://localhost:5023
```

Chưa có project test và linter.

File cấu hình riêng của từng máy (nằm trong `.gitignore`, mỗi thành viên tự sao chép từ file mẫu rồi điền):

- `CaroGame.Server/appsettings.Local.json` từ `appsettings.Local.example.json`: connection string, Gmail và App Password. `Program.cs` của Server nạp file này bằng `AddJsonFile(..., optional: true)`.
- `CaroGame-WinForms/clientsettings.json` từ `clientsettings.example.json`: địa chỉ server, base URL, model và API key của LLM. File được copy ra thư mục output khi build; code đọc file này chưa viết.

## Kiến trúc

```
WinForms client  ⇄  Game Server (ASP.NET Core + SignalR)  ⇄  SQL Server
```

Solution `CaroGame-WinForms.slnx` (định dạng XML, không phải `.sln`), cả 3 project dùng .NET 10:

- **`CaroGame-WinForms`** (đã có, đóng vai trò Client). KHÔNG đổi tên project; root namespace là `CaroGame_WinForms`. Chứa toàn bộ giao diện. PvE và PvP Offline chạy hoàn toàn ở đây. Kết nối server bằng SignalR Client. KHÔNG truy cập DB, KHÔNG chứa connection string.
- **`CaroGame.Server`** (sẽ thêm). ASP.NET Core + SignalR. Chứa toàn bộ EF Core: Code First, `AppDbContext : DbContext`, Migrations, LINQ to Entities, mọi thao tác DB dùng async/await. Là nơi quyết định duy nhất cho trận Online.
- **`CaroGame.Shared`** (sẽ thêm). Class library dùng chung: luật chơi (kiểm tra thắng, chặn 2 đầu, nước đi hợp lệ) và DTO truyền qua SignalR.

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

- **Mở app:** tiêu đề "Caro", nút Chơi ngay và Đăng nhập, icon Information (hướng dẫn cuộn dọc) và Settings (chỉ bật/tắt âm thanh và nhạc nền). Không có đổi ngôn ngữ.
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
- **Đồng hồ:** mỗi người tổng 180 giây cho cả trận, chỉ chạy trong lượt mình, ở cả 3 chế độ. Bot không bị tính giờ. Đồng hồ của X chạy ngay khi trận bắt đầu. Hết giờ là thua.
- **Hòa:** bàn cờ đầy. Không có xin hòa.
- **Undo:**
  - PvE: lùi 2 nước (bot + mình), không hỏi.
  - Offline: lùi 1 nước, trả lượt cho người vừa đi, không hỏi.
  - Online: chỉ xin được ngay sau nước của mình, trước khi đối thủ đi; lùi 1 nước. Lời xin có hạn 15 giây, đồng hồ cả 2 dừng trong lúc chờ, không trả lời tính là từ chối. Bị từ chối 3 lần trong 1 trận thì hết quyền xin. Không giới hạn số lần được chấp nhận. Không hoàn lại thời gian đã dùng.
- **FF:** PvE là người chơi đầu hàng; Offline là người đang tới lượt đầu hàng. Không có nút thoát riêng: thoát trận chính là FF (Online bị −100).
- **Bot:** một mức độ khó, thuật toán chấm điểm từng ô trống chạy trên client. KHÔNG dùng LLM để chọn nước đi.
- **Chat:**
  - Online: realtime qua server, tối đa 200 ký tự, không lưu DB, không lọc từ ngữ.
  - PvE (kể cả Guest): client gọi trực tiếp LLM qua API tương thích OpenAI. Mặc định Google Gemini Flash-Lite free tier, dự phòng Groq; base URL, model và API key nằm trong file cấu hình để đổi nhà cung cấp không sửa code. Chỉ gửi nội dung chat và tình hình trận, KHÔNG gửi email hay thông tin cá nhân. Câu trả lời giới hạn 1–2 câu. Lỗi 429, timeout hoặc mất mạng thì dùng câu soạn sẵn.
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

- **User:** Id, username, email, mật khẩu hash, `IsEmailVerified`, điểm, số thứ tự avatar mặc định, đường dẫn avatar upload, `CreatedAt`. Số trận thắng/thua KHÔNG lưu cột riêng, tính bằng LINQ từ lịch sử.
- **Lịch sử trận:** người chơi, đối thủ (trận Offline để null, hiển thị "Người chơi 2"), chế độ, kết quả, lý do kết thúc (`FiveInRow`, `Timeout`, `Forfeit`, `Disconnect`, `Draw`), cờ có áp dụng luật chặn 2 đầu, số nước, điểm thay đổi (chỉ Online), thời gian. Không lưu danh sách nước đi, không có xem lại trận.
- **Mã xác minh email:** mã, mục đích (đăng ký / quên mật khẩu), hạn dùng.

## Cách làm việc với nhóm

- Nhóm tự thiết kế toàn bộ Form bằng WinForms Designer. Claude chỉ viết logic.
- Trước khi viết logic cho một Form, đọc file `.cs` và `.Designer.cs` của Form đó để lấy đúng tên control. KHÔNG đoán tên control.
- KHÔNG sửa `.Designer.cs`. KHÔNG tự tạo control bằng code. Ngoại lệ duy nhất: bàn cờ 20x20, nhóm đặt 1 Panel, Claude vẽ bàn cờ và xử lý click bằng code.
- Thiếu Form hoặc control thì DỪNG và báo nhóm, liệt kê: Form nào, loại control, tên đề xuất (tiền tố `btn`, `txt`, `lbl`, `pnl`, `pic`, `lst`...), dùng để làm gì. Chờ nhóm kéo xong mới viết tiếp.
- Mỗi Form chỉ 1 người sửa, để tránh xung đột `.Designer.cs` và `.resx` trên git.
- Phần nhóm có thể chưa biết làm (cài SQL Server, chạy Migration, tạo Gmail App Password, Cloudflare Tunnel) thì hướng dẫn từng bước.
- Làm từng phần nhỏ: mỗi lần một Form hoặc một chức năng.
- Sau mỗi phần code, giải thích ngắn: phần này làm gì, luồng chạy từ lúc bấm nút tới kết quả, và 2–3 câu thầy có thể hỏi kèm cách trả lời.

## Chia Form

Việc kéo Form chia cho 3 người (TV1, TV2, TV3; chưa gắn tên thật). Tên Form và control là đề xuất, luôn đọc `.Designer.cs` để lấy tên thật.

- **TV1 (trận đấu):** `frmGame`, `frmResult`, `frmUndoRequest`, `frmRuleVote`, `frmMatchmaking`.
- **TV2 (tài khoản):** `frmRegister`, `frmLogin`, `frmVerifyCode`, `frmForgotPassword`, `frmResetPassword`.
- **TV3 (menu và hồ sơ):** `frmStart` (là `Form1` hiện có), `frmInformation`, `frmSettings`, `frmModeSelect`, `frmProfile`, `frmHistory`, `frmLeaderboard`.

## Lộ trình

0. (Xong) Thêm 2 project vào solution, file cấu hình mẫu, `.gitignore` cho secrets.
1. Luật chơi trong Shared, bàn cờ, PvP Offline: thắng, chặn 2 đầu, đồng hồ, Undo, FF. Cần `frmGame`, `frmResult`.
2. PvE: bot, Undo 2 nước, chat câu soạn sẵn rồi tới LLM.
3. Khung màn hình: mở app, Guest, chọn chế độ, hướng dẫn, Settings.
4. Server nền: EF Core, Migration, Hub, đăng ký, đăng nhập, mã email, quên mật khẩu. Thử luôn Cloudflare tunnel.
5. Profile, avatar, lịch sử, bảng xếp hạng.
6. Online, lần lượt: ghép trận, hỏi luật, nước đi và đồng hồ trên server, chat, Undo, FF và tính điểm, kết nối lại 30 giây.
7. Triển khai trên máy host, thử 2 máy khác mạng, tập phương án localhost.
8. Âm thanh, sửa lỗi, ôn câu hỏi bảo vệ.

## Phong cách code

- Ưu tiên dễ đọc hơn ngắn gọn hay "thông minh". Mỗi hàm ngắn, làm đúng 1 việc.
- Tên biến và hàm bằng tiếng Anh rõ nghĩa. Comment tiếng Việt ở chỗ logic quan trọng.
- KHÔNG dùng Generic Repository, Unit of Work tự viết, CQRS/MediatR, AutoMapper hay nhiều tầng abstraction. Các class Service dùng thẳng `AppDbContext`.
- Logic game (kiểm tra thắng, chặn 2 đầu, đồng hồ, bot, tính điểm) tách ra class riêng. Form chỉ nhận sự kiện và hiển thị.
- Sự kiện từ SignalR chạy trên luồng khác: cập nhật giao diện phải đi qua `Invoke`, và giải thích lý do cho nhóm.
