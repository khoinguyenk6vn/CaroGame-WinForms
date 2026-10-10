# CaroGame-WinForms

Đồ án môn **Lập trình trên môi trường Windows**: game Caro (Gomoku) bàn 20x20 cho Windows, viết bằng C# WinForms, ASP.NET Core SignalR, Entity Framework Core và SQL Server.

## Mục lục

- [Tổng quan](#tổng-quan)
- [Kiến trúc](#kiến-trúc)
- [Cài đặt và chạy](#cài-đặt-và-chạy)
- [Tính năng theo màn hình](#tính-năng-theo-màn-hình)
- [Luật chơi](#luật-chơi)
- [Điểm rank và ghép trận](#điểm-rank-và-ghép-trận)
- [Dữ liệu](#dữ-liệu)
- [Phân công](#phân-công)
- [Roadmap](#roadmap)
- [Quy ước làm việc](#quy-ước-làm-việc)

## Tổng quan

Game có 3 chế độ:

| Chế độ | Mô tả | Cần server | Cần đăng nhập |
|---|---|---|---|
| PvE | Đánh với bot | Không | Không |
| PvP Offline | 2 người chơi chung 1 máy | Không | Không |
| PvP Online | Đấu xếp hạng qua mạng | Có | Có |

Ngoài ra có tài khoản (đăng ký, xác minh email, quên mật khẩu), hệ thống điểm và bậc rank, bảng xếp hạng chung, lịch sử đấu, avatar, và chat trong trận.

**Trạng thái hiện tại:** đã xong bước 0 của roadmap. Solution có đủ 3 project nhưng đều còn là template trống, chưa có logic game.

## Kiến trúc

```
WinForms client  ⇄  Game Server (ASP.NET Core + SignalR)  ⇄  SQL Server
```

Solution `CaroGame-WinForms.slnx` gồm 3 project, cùng dùng .NET 10 (LTS):

| Project | Loại | Vai trò |
|---|---|---|
| `CaroGame-WinForms` | Windows Forms App | Client. Toàn bộ giao diện. PvE và PvP Offline chạy hoàn toàn ở đây. Kết nối server bằng SignalR Client. Không truy cập DB, không chứa connection string. |
| `CaroGame.Server` | ASP.NET Core + SignalR | Xử lý tài khoản, gửi mã email, ghép trận, xác nhận nước đi, đồng hồ, Undo và chat Online, tính điểm, bảng xếp hạng, lịch sử, upload avatar. Chứa toàn bộ phần EF Core. |
| `CaroGame.Shared` | Class Library | Dùng chung cho Client và Server: luật chơi (kiểm tra thắng, chặn 2 đầu, nước đi hợp lệ) và các class dữ liệu truyền qua SignalR (DTO). |

### Phần EF Core (trong Server)

- Code First, class `AppDbContext : DbContext`.
- Entity classes và Migrations nối với SQL Server thật.
- Truy vấn bằng LINQ to Entities.
- Mọi thao tác CSDL dùng async/await.

### Giao tiếp Client và Server

- Đăng ký, đăng nhập, ghép trận, nước đi, chat đi qua **SignalR Hub**.
- Upload avatar là 1 endpoint **HTTP POST** riêng, gửi kèm session token.
- Không dùng JWT. Sau khi đăng nhập qua Hub, server sinh một session token ngẫu nhiên (GUID), lưu trong RAM và gắn userId với ConnectionId. Khi SignalR tự kết nối lại, client gửi token để server gắn lại.
- Một tài khoản chỉ đăng nhập được ở một máy. Máy thứ hai bị từ chối với thông báo "Tài khoản đang đăng nhập ở nơi khác". Phiên được giải phóng ngay khi mất kết nối nếu đang ngoài trận; nếu đang trong trận thì sau khi hết 30 giây chờ và đã xử thua.
- Server là nơi quyết định duy nhất cho trận Online. Mỗi trận là một object có lock, mọi sự kiện đi qua một chỗ xử lý.
- Trận đang chơi chỉ giữ trong RAM. Server khởi động lại thì các trận đó bị hủy, không tính điểm.

### Triển khai

- Server host trên máy một thành viên, public qua Cloudflare named tunnel ở một subdomain của `vernfy.com` (ví dụ `caro.vernfy.com`).
- Khi phát triển, mỗi người dùng SQL Server LocalDB trên máy mình và tạo DB bằng Migration. Máy host demo dùng SQL Server Express.
- Phương án dự phòng khi demo: chạy server và DB ngay trên laptop demo, client trỏ về localhost.

## Cài đặt và chạy

### Yêu cầu

- Windows.
- .NET 10 SDK (cả nhóm cài cùng phiên bản).
- Visual Studio bản mở được file `.slnx`. Nếu mở lỗi thì cập nhật Visual Studio.
- SQL Server LocalDB (cần từ bước 4 của roadmap).

### File cấu hình riêng của từng máy

Hai file dưới đây chứa thông tin bí mật nên đã nằm trong `.gitignore`. Sau khi clone, mỗi người tự sao chép từ file mẫu rồi điền:

| File cần tạo | Sao chép từ | Nội dung |
|---|---|---|
| `CaroGame.Server/appsettings.Local.json` | `CaroGame.Server/appsettings.Local.example.json` | Connection string, địa chỉ Gmail và App Password dùng để gửi mã |
| `CaroGame-WinForms/clientsettings.json` | `CaroGame-WinForms/clientsettings.example.json` | Địa chỉ server, base URL, tên model và API key của LLM |

**Không commit hai file này lên git.**

### Lệnh

Chạy từ thư mục gốc của repo:

```powershell
# Build cả solution
dotnet build CaroGame-WinForms.slnx

# Chạy client
dotnet run --project CaroGame-WinForms/CaroGame-WinForms.csproj

# Chạy server (http://localhost:5023)
dotnet run --project CaroGame.Server/CaroGame.Server.csproj
```

## Tính năng theo màn hình

### Màn hình mở app

- Tiêu đề "Caro", 2 nút: **Chơi ngay** và **Đăng nhập**.
- Góc trên phải có 2 icon:
  - **Information:** form hướng dẫn cho người mới, nội dung cuộn dọc.
  - **Settings:** chỉ gồm bật/tắt âm thanh và nhạc nền.
- Không có đổi ngôn ngữ.

### Chơi ngay (Guest)

- Vào thẳng màn chọn chế độ, không nhập tên.
- Tên tự sinh dạng `Guest12345`. Không kết nối server, không lưu gì (kể cả file cục bộ).
- Chơi được PvE và PvP Offline, chat được với AI.
- Chọn PvP Online hoặc bảng xếp hạng thì được yêu cầu đăng nhập.
- Bấm avatar thì hiện tên Guest và nút "Đăng nhập để lưu lịch sử".

### Đăng ký, đăng nhập, quên mật khẩu

- **Đăng ký:** username, email, mật khẩu, nhập lại mật khẩu.
  - Username: 4–20 ký tự, chỉ gồm chữ không dấu, số và dấu `_`. Kiểm tra trùng không phân biệt hoa thường. Không đổi được sau khi tạo.
  - Email: chấp nhận địa chỉ bất kỳ.
  - Mật khẩu: 6–50 ký tự, không bắt buộc ký tự đặc biệt. Lưu dạng hash BCrypt, không lưu mật khẩu gốc.
- **Mã xác minh 6 chữ số** (dùng chung cho đăng ký và quên mật khẩu):
  - Hết hạn sau 5 phút.
  - Có nút "Gửi lại mã", phải chờ 60 giây giữa 2 lần gửi.
  - Nhập sai 5 lần thì mã bị hủy, phải gửi lại.
  - Gửi bằng Gmail SMTP với App Password.
- **Tài khoản chưa xác minh:** vẫn lưu vào DB với `IsEmailVerified = false`. Đăng nhập bằng tài khoản này thì hệ thống gửi mã mới và chuyển sang form nhập mã, chưa cho vào game.
- **Đăng nhập:** username, mật khẩu, link "Quên mật khẩu". Không có ghi nhớ đăng nhập.
- **Quên mật khẩu:** nhập email, nhận mã, nhập đúng mã thì đặt mật khẩu mới.
- Hai form đăng ký và đăng nhập chuyển qua lại được, có nút back.

### Màn chọn chế độ

- 3 lựa chọn: PvE, PvP Online, PvP Offline.
- Góc trên phải là avatar người chơi, bấm để mở Profile.
- Có lối vào bảng xếp hạng chung.

### Profile

- Hiện avatar, tên, điểm và bậc rank.
- Nút **Lịch sử đấu**, nút **Đăng xuất**.
- **Avatar:**
  - Mặc định là 1 trong khoảng 20 ảnh PNG lấy từ DiceBear (style giấy phép CC0), đóng gói sẵn trong client. Lúc đăng ký server chọn ngẫu nhiên 1 ảnh và lưu số thứ tự vào DB, nên avatar cố định theo user. Guest thì ngẫu nhiên mỗi lần mở app.
  - User được upload ảnh từ máy: chỉ `.png` hoặc `.jpg`, tối đa 1MB. Server lưu file, DB chỉ lưu đường dẫn. Upload ảnh mới thì file cũ bị xóa.

### Lịch sử đấu

- Trận PvP Online: hiện chế độ và số điểm cộng/trừ.
- Trận PvE và PvP Offline: chỉ hiện chế độ, không có điểm.
- Với user đã đăng nhập, client gửi kết quả trận PvE/Offline lên server sau mỗi trận. Mất kết nối thì bỏ qua, không lưu.
- Chỉ lưu kết quả, không lưu danh sách nước đi, không có xem lại trận.

### Bảng xếp hạng

- Hiện top 50. Dòng cuối luôn là thứ hạng của chính mình.
- Bằng điểm thì xếp theo số trận thắng Online nhiều hơn, rồi tới username theo thứ tự chữ cái.

### Màn trận đấu

- **Bên trái:** bàn cờ 20x20.
- **Góc trên trái:** nút Settings trong trận, gồm Undo và FF (đầu hàng).
- **Bên phải, từ trên xuống:** 2 thẻ người chơi (avatar, tên, đồng hồ đếm ngược), rồi chat box (danh sách tin nhắn và ô nhập).
- **Hết trận:** hộp thoại kết quả hiện Thắng/Thua/Hòa, lý do kết thúc, điểm thay đổi (chỉ Online), cùng các nút "Chơi lại" (PvE/Offline) hoặc "Tìm trận mới" (Online), và "Về menu".

### Chat trong trận

| Chế độ | Cách hoạt động |
|---|---|
| PvP Online | Chat realtime giữa 2 người, đi qua server. Tối đa 200 ký tự, không lưu DB, không lọc từ ngữ. |
| PvE | Chat với AI. Client gọi trực tiếp một LLM miễn phí. Lỗi hoặc không có mạng thì bot trả lời bằng câu soạn sẵn, game vẫn chơi bình thường. |
| PvP Offline | Vẫn hiện chat box, tin nhắn chỉ hiển thị, không có phản hồi. |

Chi tiết phần chat với AI:

- Mặc định dùng Google Gemini API (model Flash-Lite, free tier), dự phòng là Groq.
- Gọi theo kiểu API tương thích OpenAI. Base URL, tên model và API key nằm trong file cấu hình, nên đổi nhà cung cấp chỉ cần sửa cấu hình, không sửa code.
- Chỉ gửi nội dung chat và tình hình trận đấu. Không gửi email hay thông tin cá nhân của người chơi.
- Câu trả lời của bot giới hạn 1–2 câu.
- Gặp lỗi 429 (hết lượt), timeout hoặc mất mạng thì dùng câu soạn sẵn.
- API key nằm ở client nên có thể bị lộ. Nhóm chấp nhận rủi ro này vì đây là key miễn phí, tạo riêng cho game.

### Âm thanh

Chưa có file âm thanh và nhạc nền. Chức năng được thiết kế sẵn chỗ phát (bật/tắt trong Settings), file sẽ thêm sau. Thiếu file thì game vẫn chạy bình thường, không báo lỗi.

## Luật chơi

### Thắng thua

- X luôn đi trước.
  - PvE: người chơi cầm X.
  - PvP Offline: người chơi 1 cầm X. Người chơi 2 tên "Người chơi 2", dùng avatar mặc định.
  - PvP Online: server chọn ngẫu nhiên ai cầm X.
- Từ 5 quân liên tiếp trở lên (ngang, dọc, chéo) là thắng.
- **Hòa:** bàn cờ đầy. Không có chức năng xin hòa.

### Luật chặn 2 đầu

- Hàng quân bị quân đối thủ nằm sát ngay cả 2 đầu thì **không** tính thắng.
- Mép bàn cờ **không** tính là bị chặn.
- PvE và PvP Offline: luôn áp dụng, không hỏi, không có nút bật/tắt.
- PvP Online: trước trận hỏi cả 2 người, hạn trả lời 15 giây, không trả lời tính là không đồng ý. Chỉ cần 1 người không đồng ý thì trận đó chơi luật thường (hàng 5 bị chặn 2 đầu vẫn thắng). Không hủy trận, không ghép lại. Đồng hồ chưa chạy trong lúc hỏi.

### Đồng hồ

- Mỗi người có tổng 180 giây cho cả trận (kiểu đồng hồ cờ vua), chỉ chạy trong lượt của mình.
- Áp dụng ở cả 3 chế độ. Ở PvE, bot không bị tính giờ.
- Đồng hồ của người đi trước (X) chạy ngay khi trận bắt đầu.
- Hết giờ là thua.

### Undo

| Chế độ | Cách hoạt động |
|---|---|
| PvE | Lùi 2 nước (nước của bot và nước của mình), không cần hỏi. |
| PvP Offline | Lùi 1 nước, trả lượt lại cho người vừa đi, không cần hỏi. |
| PvP Online | Phải được đối thủ đồng ý. Lùi 1 nước. |

Chi tiết Undo ở PvP Online:

- Chỉ xin được ngay sau nước của mình, trước khi đối thủ đi tiếp. Đối thủ đã đi rồi thì nút Undo bị khóa.
- Lời xin có hạn 15 giây. Không trả lời thì tự tính là từ chối.
- Trong lúc chờ trả lời, đồng hồ của cả 2 tạm dừng.
- Undo không hoàn lại thời gian đã dùng.
- Bị từ chối 3 lần trong cùng 1 trận thì không được xin Undo nữa trong trận đó.
- Không giới hạn số lần Undo được chấp nhận.

### FF (đầu hàng) và thoát trận

- PvE: người chơi đầu hàng, xử thua, không có điểm.
- PvP Offline: người đang tới lượt đầu hàng.
- PvP Online: bị trừ 100 điểm.
- Không có nút thoát riêng. Thoát trận chính là FF.

### Bot (PvE)

- Một mức độ khó duy nhất, không chia mức. Chỉ cần đánh đúng luật, không cần mạnh.
- Nước đi do thuật toán đơn giản trên client tính, bằng cách chấm điểm từng ô trống.
- Không dùng LLM để chọn nước đi. LLM chỉ dùng cho chat.

## Điểm rank và ghép trận

Chỉ áp dụng cho PvP Online. Nhóm gọi là "Elo" nhưng đây là hệ điểm cố định theo số nước, không dùng công thức Elo chuẩn (không phụ thuộc điểm đối thủ).

### Cách tính điểm

- Điểm khởi đầu: **1000**. Điểm không bao giờ xuống dưới 1000.
- n = số quân đang có trên bàn lúc trận kết thúc (Undo làm giảm n).

| Kết quả | n ≤ 25 | n > 25 |
|---|---|---|
| Thắng | +50 | +(50 − floor((n − 25) / 2)), thấp nhất +35 |
| Thua (kể cả hết giờ) | −50 | −(50 − floor((n − 25) / 2)), trừ ít nhất 25 |
| Hòa | 0 | 0 |
| FF (đầu hàng, thoát trận) | −100 | −100 |
| Mất kết nối quá 30 giây | −100 | −100 |

Ví dụ: trận kết thúc ở n = 35 thì người thắng +45, người thua −45. Thắng chạm mức thấp nhất +35 từ n = 55. Thua chạm mức −25 từ n = 75.

Các trường hợp đặc biệt:

- Đối thủ FF, hết giờ hoặc mất kết nối: người thắng được cộng theo công thức n như thắng thường.
- Lịch sử ghi đúng số điểm thực tế thay đổi. Người đang 1000 điểm mà thua thì ghi 0.
- Đối thủ thoát lúc đang hỏi luật hoặc trước nước đi đầu tiên: hủy trận, không ai bị trừ hay được cộng điểm, người còn lại quay về màn tìm trận.

### Mất kết nối giữa trận

- Có 30 giây để kết nối lại. Trong thời gian đó đồng hồ của cả 2 tạm dừng.
- Chỉ hỗ trợ kết nối lại tự động của SignalR. Tắt hẳn app thì không vào lại được trận.
- Quá 30 giây thì xử thua như FF (−100).

### Bậc rank

9 hạng, mỗi hạng 3 bậc I → II → III (I thấp nhất), mỗi bậc rộng 100 điểm.

| Hạng | Bậc I | Bậc II | Bậc III |
|---|---|---|---|
| Sắt | 1000 | 1100 | 1200 |
| Đồng | 1300 | 1400 | 1500 |
| Bạc | 1600 | 1700 | 1800 |
| Vàng | 1900 | 2000 | 2100 |
| Bạch Kim | 2200 | 2300 | 2400 |
| Kim Cương | 2500 | 2600 | 2700 |
| Lục Bảo | 2800 | 2900 | 3000 |
| Trái Đất | 3100 | 3200 | 3300 |
| Mặt Trời | 3400 | 3500 | 3600 trở lên |

Số trong bảng là điểm bắt đầu của bậc. Mặt Trời III không có giới hạn trên.

### Ghép trận

- Chỉ có ghép trận tự động, không có lobby hay tạo phòng.
- Ban đầu tìm đối thủ trong khoảng ±100 điểm. Cứ mỗi 10 giây mở rộng thêm ±100.
- Sau 60 giây thì ghép với bất kỳ ai đang chờ.
- Có nút hủy tìm trận.

## Dữ liệu

Mức khái niệm, chưa phải schema cuối cùng.

| Bảng | Thông tin lưu |
|---|---|
| User | Id, username, email, mật khẩu (hash), `IsEmailVerified`, điểm, số thứ tự avatar mặc định, đường dẫn avatar upload, `CreatedAt` |
| Lịch sử trận | Người chơi, đối thủ, chế độ, kết quả, lý do kết thúc, cờ có áp dụng luật chặn 2 đầu, số nước, điểm thay đổi (chỉ Online), thời gian |
| Mã xác minh email | Mã, mục đích (đăng ký / quên mật khẩu), hạn dùng |

- Lý do kết thúc gồm: `FiveInRow`, `Timeout`, `Forfeit`, `Disconnect`, `Draw`.
- Trận PvP Offline: đối thủ để null, hiển thị "Người chơi 2".
- Số trận thắng/thua không lưu thành cột riêng, tính bằng LINQ từ bảng lịch sử.
- Guest không lưu gì.

## Phân công

### Theo mảng

Mỗi người phụ trách kéo giao diện các Form trong mảng của mình bằng WinForms Designer, và nắm phần logic tương ứng để giải thích được khi bảo vệ.

| Thành viên | Mảng | Form phụ trách | Logic liên quan |
|---|---|---|---|
| TV1 | Trận đấu | `frmGame`, `frmResult`, `frmUndoRequest`, `frmRuleVote`, `frmMatchmaking` | Luật chơi, bàn cờ, đồng hồ, Undo, FF, bot, chat, ghép trận, trận Online |
| TV2 | Tài khoản | `frmRegister`, `frmLogin`, `frmVerifyCode`, `frmForgotPassword`, `frmResetPassword` | Đăng ký, đăng nhập, mã email, quên mật khẩu, session, EF Core và Migration |
| TV3 | Menu và hồ sơ | `frmStart`, `frmInformation`, `frmSettings`, `frmModeSelect`, `frmProfile`, `frmHistory`, `frmLeaderboard` | Luồng Guest, Profile, avatar, lịch sử, bảng xếp hạng, điểm rank, âm thanh |

`frmStart` chính là `Form1` hiện có trong project. TV1, TV2, TV3 là tên tạm, nhóm thay bằng tên thật.

### Danh sách Form và control

Tên Form và tên control dưới đây là tên đề xuất, theo tiền tố `btn`, `txt`, `lbl`, `pnl`, `pic`, `lst`, `chk`, `lnk`, `dgv`, `rtb`.

| Form | Người | Control chính |
|---|---|---|
| `frmGame` | TV1 | `pnlBoard`, `btnGameSettings`, `pnlGameSettings` (chứa `btnUndo`, `btnSurrender`), `picPlayer1Avatar`, `lblPlayer1Name`, `lblPlayer1Timer`, `picPlayer2Avatar`, `lblPlayer2Name`, `lblPlayer2Timer`, `lstChat`, `txtChatInput`, `btnSendChat`, `lblTurn`, `lblRule`, `lblConnectionStatus` |
| `frmResult` | TV1 | `lblResult`, `lblReason`, `lblScoreChange`, `btnPlayAgain`, `btnBackToMenu` |
| `frmUndoRequest` | TV1 | `lblMessage`, `lblCountdown`, `btnAccept`, `btnDecline` |
| `frmRuleVote` | TV1 | `lblQuestion`, `lblCountdown`, `btnAgree`, `btnDecline` |
| `frmMatchmaking` | TV1 | `lblStatus`, `lblWaitTime`, `btnCancel` |
| `frmRegister` | TV2 | `txtUsername`, `txtEmail`, `txtPassword`, `txtConfirmPassword`, `btnContinue`, `lnkLogin`, `btnBack`, `lblError` |
| `frmLogin` | TV2 | `txtUsername`, `txtPassword`, `lnkForgotPassword`, `btnContinue`, `lnkRegister`, `btnBack`, `lblError` |
| `frmVerifyCode` | TV2 | `lblMessage`, `txtCode`, `btnConfirm`, `btnResend`, `lblResendCountdown`, `lblError`, `btnBack` |
| `frmForgotPassword` | TV2 | `txtEmail`, `btnSendCode`, `btnBack`, `lblError` |
| `frmResetPassword` | TV2 | `txtNewPassword`, `txtConfirmPassword`, `btnConfirm`, `lblError` |
| `frmStart` | TV3 | `lblTitle`, `btnPlayNow`, `btnLogin`, `picInfo`, `picSettings` |
| `frmInformation` | TV3 | `rtbGuide` (chỉ đọc, cuộn dọc), `btnClose` |
| `frmSettings` | TV3 | `chkSound`, `chkMusic`, `btnClose` |
| `frmModeSelect` | TV3 | `btnPvE`, `btnPvPOnline`, `btnPvPOffline`, `picAvatar`, `btnLeaderboard` |
| `frmProfile` | TV3 | `picAvatar`, `lblUsername`, `lblScore`, `lblRank`, `btnChangeAvatar`, `btnHistory`, `btnLogout`, `btnLoginToSave`, `btnBack` |
| `frmHistory` | TV3 | `dgvHistory` (cột: thời gian, chế độ, đối thủ, kết quả, lý do, số nước, điểm), `btnBack` |
| `frmLeaderboard` | TV3 | `dgvLeaderboard` (cột: hạng, avatar, tên, điểm, bậc rank), `btnBack` |

Ghi chú:

- `pnlBoard` chỉ là 1 Panel trống. Bàn cờ 20x20 được vẽ và xử lý click bằng code.
- `btnPlayAgain` dùng chung một nút, code đổi chữ thành "Chơi lại" hoặc "Tìm trận mới" theo chế độ.
- `btnLoginToSave` chỉ hiện với Guest. Khi đó điểm, rank, lịch sử, đổi avatar và đăng xuất bị ẩn.
- `lblRule` cho biết trận đang áp dụng luật chặn 2 đầu hay không.
- Dòng "thứ hạng của mình" là dòng cuối của `dgvLeaderboard`, không cần control riêng.

### Việc hạ tầng

Các việc dưới đây chưa có người nhận, nhóm điền tên vào.

| Việc | Cần trước bước | Người phụ trách |
|---|---|---|
| Tạo API key Gemini trong Google AI Studio (và key Groq dự phòng) | 2 | Chưa phân công |
| Cài SQL Server LocalDB trên từng máy dev | 4 | Mỗi người tự làm |
| Tạo Gmail gửi mã, bật xác minh 2 bước, tạo App Password | 4 | Chưa phân công |
| Dựng Cloudflare named tunnel cho subdomain của `vernfy.com` | 4 | Chưa phân công |
| Tải khoảng 20 ảnh PNG DiceBear (style CC0) | 5 | Chưa phân công |
| Cài SQL Server Express trên máy host demo | 7 | Chưa phân công |
| Viết nội dung hướng dẫn cho form Information | 3 | Chưa phân công |
| Tìm file âm thanh và nhạc nền | 8 | Chưa phân công |

## Roadmap

Nguyên tắc sắp xếp: có thứ chơi được sớm nhất, và thử đường triển khai sớm vì đó là rủi ro lớn nhất khi demo. Bước 1 đến 3 không cần server, nên Form của bước 4 và 5 có thể được kéo song song trong lúc đó.

### Bước 0: Chuẩn bị solution (đã xong)

- [x] Thêm project `CaroGame.Server` và `CaroGame.Shared` vào solution.
- [x] Client và Server cùng tham chiếu `CaroGame.Shared`.
- [x] File cấu hình mẫu cho Client và Server.
- [x] `.gitignore` chặn các file cấu hình chứa bí mật.

**Kết quả:** cả nhóm clone về và build được.

### Bước 1: Luật chơi và PvP Offline

- [ ] Luật chơi trong `CaroGame.Shared`: nước đi hợp lệ, kiểm tra thắng, luật chặn 2 đầu, kiểm tra hòa.
- [ ] Vẽ bàn cờ 20x20 trên `pnlBoard` và xử lý click.
- [ ] Đồng hồ 180 giây mỗi người.
- [ ] Undo lùi 1 nước.
- [ ] FF và hộp thoại kết quả.

**Form cần có trước:** `frmGame`, `frmResult` (TV1).
**Kết quả:** chơi được 2 người chung máy từ đầu tới cuối trận.

### Bước 2: PvE

- [ ] Bot chấm điểm từng ô trống.
- [ ] Undo lùi 2 nước.
- [ ] Chat với bot bằng câu soạn sẵn.
- [ ] Đọc `clientsettings.json` và gọi LLM, có xử lý lỗi 429, timeout, mất mạng.

**Form cần có trước:** không thêm.
**Việc hạ tầng:** API key Gemini.
**Kết quả:** chơi được với bot, chat được với AI.

### Bước 3: Khung màn hình

- [ ] Màn mở app, luồng Chơi ngay với tên Guest tự sinh.
- [ ] Màn chọn chế độ, chuyển vào trận PvE và Offline.
- [ ] Form hướng dẫn và form Settings (âm thanh để chỗ trống).

**Form cần có trước:** `frmStart`, `frmModeSelect`, `frmInformation`, `frmSettings` (TV3).
**Kết quả:** đi được hết luồng của Guest.

### Bước 4: Server nền và tài khoản

- [ ] Cài gói EF Core, tạo `AppDbContext`, entity, Migration đầu tiên trên LocalDB.
- [ ] SignalR Hub và kết nối từ client.
- [ ] Đăng ký, gửi mã email, xác minh.
- [ ] Đăng nhập, session token, chặn đăng nhập 2 máy.
- [ ] Quên mật khẩu.
- [ ] Thử Cloudflare tunnel với server thật.

**Form cần có trước:** `frmRegister`, `frmLogin`, `frmVerifyCode`, `frmForgotPassword`, `frmResetPassword` (TV2).
**Việc hạ tầng:** LocalDB, Gmail App Password, Cloudflare tunnel.
**Kết quả:** đăng ký và đăng nhập được từ một máy khác mạng.

### Bước 5: Profile và dữ liệu người chơi

- [ ] Profile, đăng xuất, giao diện Profile của Guest.
- [ ] Avatar mặc định và upload avatar.
- [ ] Gửi kết quả PvE/Offline lên server, màn lịch sử đấu.
- [ ] Bảng xếp hạng top 50 và thứ hạng của mình.

**Form cần có trước:** `frmProfile`, `frmHistory`, `frmLeaderboard` (TV3).
**Việc hạ tầng:** bộ ảnh DiceBear.
**Kết quả:** xong toàn bộ phần tài khoản.

### Bước 6: PvP Online

Làm lần lượt từng mục, mục trước chạy ổn mới sang mục sau.

- [ ] Ghép trận tự động theo điểm, hủy tìm trận.
- [ ] Hỏi luật chặn 2 đầu trước trận.
- [ ] Nước đi do server xác nhận, đồng hồ chạy trên server.
- [ ] Chat realtime.
- [ ] Undo có xin phép, hạn 15 giây, giới hạn 3 lần bị từ chối.
- [ ] FF, hết giờ, tính điểm rank, lưu lịch sử.
- [ ] Mất kết nối và kết nối lại trong 30 giây.

**Form cần có trước:** `frmMatchmaking`, `frmRuleVote`, `frmUndoRequest` (TV1).
**Kết quả:** chơi được trận xếp hạng giữa 2 máy.

### Bước 7: Triển khai

- [ ] Cài SQL Server Express và chạy Migration trên máy host.
- [ ] Chạy server sau Cloudflare tunnel ở subdomain cố định.
- [ ] Thử 2 máy ở 2 mạng khác nhau.
- [ ] Tập phương án dự phòng: server và DB chạy trên laptop demo, client trỏ localhost.

**Kết quả:** sẵn sàng demo.

### Bước 8: Hoàn thiện

- [ ] Âm thanh và nhạc nền.
- [ ] Sửa lỗi, rà lại toàn bộ luồng.
- [ ] Ôn các câu hỏi thầy có thể hỏi cho từng phần.

## Quy ước làm việc

### Giao diện

- Toàn bộ Form được thiết kế bằng WinForms Designer. Không tạo control bằng code, trừ bàn cờ.
- Mỗi Form chỉ 1 người sửa, để tránh xung đột file `.Designer.cs` và `.resx` trên git.
- Thiếu control thì bổ sung trong Designer trước, rồi mới viết logic dùng tới nó.

### Code

- Ưu tiên dễ đọc hơn ngắn gọn. Mỗi hàm ngắn, làm đúng 1 việc.
- Tên biến và hàm bằng tiếng Anh rõ nghĩa. Comment tiếng Việt ở chỗ logic quan trọng.
- Không dùng Generic Repository, Unit of Work tự viết, CQRS/MediatR, AutoMapper hay nhiều tầng abstraction. Các class Service dùng thẳng `AppDbContext`.
- Logic game (kiểm tra thắng, chặn 2 đầu, đồng hồ, bot, tính điểm) tách ra class riêng. Form chỉ nhận sự kiện và hiển thị.
- Sự kiện từ SignalR chạy trên luồng khác với luồng giao diện, nên mọi cập nhật giao diện phải đi qua `Invoke`.

### Bí mật

- Connection string, Gmail App Password và API key chỉ nằm trong `appsettings.Local.json` và `clientsettings.json`.
- Hai file này đã có trong `.gitignore`. Kiểm tra `git status` trước khi commit để chắc chúng không bị thêm vào.
