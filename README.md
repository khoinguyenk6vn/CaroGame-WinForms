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
- [Triển khai lên máy host](#triển-khai-lên-máy-host)
- [Câu hỏi bảo vệ](#câu-hỏi-bảo-vệ)
- [Quy ước làm việc](#quy-ước-làm-việc)

## Tổng quan

Game có 3 chế độ:

| Chế độ | Mô tả | Cần server | Cần đăng nhập |
|---|---|---|---|
| PvE | Đánh với bot | Không | Không |
| PvP Offline | 2 người chơi chung 1 máy | Không | Không |
| PvP Online | Đấu xếp hạng qua mạng | Có | Có |

Ngoài ra có tài khoản (đăng ký, xác minh email, quên mật khẩu), hệ thống điểm và bậc rank, bảng xếp hạng chung, lịch sử đấu, avatar, và chat trong trận.

**Trạng thái hiện tại:** đã xong phần code của toàn bộ roadmap; server thật chạy qua `https://caro.vernfy.com` và đã chơi Online được. Còn lại: thêm file âm thanh, sửa lỗi vặt. Hướng dẫn chạy server xem [Triển khai lên máy host](#triển-khai-lên-máy-host).

## Kiến trúc

Solution theo mô hình 3 tầng **GUI → BLL → DAL → CSDL**, cộng thêm 1 server SignalR để các máy nói chuyện với nhau:

```
CaroGame.UI (GUI) ──tham chiếu──► CaroGame.BLL ──tham chiếu──► CaroGame.DAL ──EF Core──► SQL Server
        │                                    ▲
        └──── SignalR ────► CaroGame.Server ─┘ (tham chiếu BLL)
```

Solution `CaroGame.slnx` gồm 4 project, cùng dùng .NET 10 (LTS):

| Project | Loại | Vai trò |
|---|---|---|
| `CaroGame.DAL` | Class Library | Tầng truy cập dữ liệu. Nơi **duy nhất** giao tiếp với CSDL, bằng EF Core. Chứa `AppDbContext`, entity, Migrations và các class `UserDAL`, `VerificationCodeDAL`... Mọi gói NuGet của solution chỉ cài ở đây. |
| `CaroGame.BLL` | Class Library | Tầng nghiệp vụ, tham chiếu DAL. Chứa **toàn bộ** logic nghiệp vụ: luật chơi, bot, đồng hồ, tài khoản, mã xác minh, phiên đăng nhập, chat với bot, các class dữ liệu truyền qua SignalR (DTO). Không truy vấn CSDL trực tiếp, chỉ gọi DAL. |
| `CaroGame.UI` | Windows Forms App | Tầng giao diện (GUI), tham chiếu BLL. Chỉ nhận thao tác và hiển thị, **không** chứa logic nghiệp vụ, **không** truy vấn CSDL. PvE và PvP Offline gọi thẳng BLL trên máy; tài khoản, BXH, Online gọi server qua SignalR. |
| `CaroGame.Server` | ASP.NET Core + SignalR | Tham chiếu BLL. Hub chỉ nhận yêu cầu từ các máy rồi chuyển cho BLL xử lý. Là nơi duy nhất giữ connection string và mật khẩu Gmail. |

Các gói NuGet (EF Core, BCrypt, SignalR Client) chỉ cài ở DAL. BLL, GUI và Server dùng được các gói đó nhờ **tham chiếu bắc cầu**: project tham chiếu tới một project khác thì dùng được cả các gói NuGet của project đó.

### Phần EF Core (trong DAL)

- Code First, class `AppDbContext : DbContext`.
- Entity classes và Migrations nối với SQL Server thật.
- Truy vấn bằng LINQ to Entities, chỉ viết trong các class DAL.
- Mọi thao tác CSDL dùng async/await.
- **Không dùng ADO.NET** (`SqlConnection`, `SqlCommand`, `DataTable`, `FromSqlRaw`...) ở bất kỳ đâu trong repo.

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
| `CaroGame.UI/clientsettings.json` | `CaroGame.UI/clientsettings.example.json` | Địa chỉ server, base URL, tên model và API key của LLM |

**Không commit hai file này lên git.** API key, mật khẩu thật chỉ được điền vào 2 file trên, không điền vào file `.example.json` (file mẫu được commit).

Nếu để trống `Address` và `AppPassword` trong `appsettings.Local.json` thì server không gửi mail mà in mã xác minh ra cửa sổ server, tiện cho việc code thử trên máy mình.

### Tạo CSDL (làm 1 lần trên mỗi máy)

```powershell
# Cài công cụ Migration của EF Core
dotnet tool install --global dotnet-ef --version 10.0.13

# Tạo database CaroGame trên LocalDB từ các Migration có sẵn
dotnet ef database update --project CaroGame.DAL --startup-project CaroGame.Server
```

Khi có Migration mới (sau khi pull code), chạy lại lệnh `dotnet ef database update` ở trên để cập nhật CSDL.

Migration nằm trong project `CaroGame.DAL`. `--startup-project CaroGame.Server` để lệnh đọc được connection string trong `appsettings.Local.json` của Server.

### Lệnh

Chạy từ thư mục gốc của repo:

```powershell
# Build cả solution
dotnet build CaroGame.slnx

# Chạy client
dotnet run --project CaroGame.UI/CaroGame.UI.csproj

# Chạy server (http://localhost:5023)
dotnet run --project CaroGame.Server/CaroGame.Server.csproj
```

## Tính năng theo màn hình

### Màn hình mở app

- Tiêu đề "Caro", 2 nút: **Chơi ngay** và **Đăng nhập**.
- Góc trên phải có 2 icon:
  - **Information:** form hướng dẫn cho người mới, nội dung cuộn dọc.
  - **Settings:** chỉ gồm 2 nút bấm bật/tắt âm thanh và nhạc nền. Chữ trên nút cho biết đang bật hay tắt.
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

### Phím tắt

| Màn hình | Phím | Tác dụng |
|---|---|---|
| Màn mở app | Enter | Chơi ngay |
| Đăng nhập, Đăng ký | Enter / Esc | Tiếp tục / Quay lại |
| Nhập mã, Đặt mật khẩu mới | Enter / Esc | Xác nhận / Quay lại |
| Quên mật khẩu | Enter / Esc | Gửi mã / Quay lại |
| Chọn chế độ | 1 / 2 / 3 / 4 | PvE / PvP Offline / PvP Online / Bảng xếp hạng |
| Profile, Lịch sử, Bảng xếp hạng | Esc | Quay lại |
| Hướng dẫn, Cài đặt | Esc | Đóng |
| Tìm trận | Esc | Dừng tìm trận |
| Trận đấu | Enter | Gửi tin nhắn chat |
| Trận đấu | Ctrl+Z | Undo |
| Trận đấu | Ctrl+Q | Đầu hàng (FF) |
| Kết quả trận | Enter / Esc | Chơi lại hoặc Tìm trận mới / Về menu |
| Đối thủ xin Undo | Enter / Esc | Đồng ý / Từ chối |

Cửa sổ nào cũng phóng to được (nút Maximize); khi phóng to, toàn bộ giao diện giãn theo cho tràn màn hình.

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
- Áp dụng ở PvP Offline và PvP Online. PvE không tính giờ: đồng hồ vẫn hiện trên màn hình nhưng đứng yên ở 03:00.
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

TV1, TV2, TV3 là tên tạm, nhóm thay bằng tên thật.

### Danh sách Form và control

Tên Form và tên control dưới đây là tên đề xuất, theo tiền tố `btn`, `txt`, `lbl`, `pnl`, `pic`, `lst`, `chk`, `lnk`, `dgv`, `rtb`.

| Form | Người | Control chính |
|---|---|---|
| `frmGame` | TV1 | `pnlBoard`, `btnGameSettings`, `pnlGameSettings` (chứa `btnUndo`, `btnSurrender`), `picPlayer1Avatar`, `lblPlayer1Name`, `lblPlayer1Timer`, `picPlayer2Avatar`, `lblPlayer2Name`, `lblPlayer2Timer`, `lstChat`, `txtChatInput`, `btnSendChat`, `lblTurn`, `lblRule`, `lblConnectionStatus` |
| `frmResult` | TV1 | `lblResult`, `lblReason`, `lblScoreChange`, `btnPlayAgain`, `btnBackToMenu` |
| `frmUndoRequest` | TV1 | `lblMessage`, `lblCountdown`, `btnAccept`, `btnDecline` |
| `frmRuleVote` | TV1 | Không dùng nữa: hỏi luật chặn 2 đầu bằng MessageBox (nhóm chốt ngày 18/10/2026) |
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
| Tải khoảng 20 ảnh PNG DiceBear (style CC0) | 5 | Đã xong (style Thumbs, trong `CaroGame.UI/Avatars`; nên xem lại giấy phép trên trang DiceBear) |
| Cài SQL Server Express trên máy host demo | 7 | Chưa phân công |
| Viết nội dung hướng dẫn cho form Information | 3 | Đã xong (viết trong `frmInformation.cs`, nhóm có thể sửa lại) |
| Tìm file âm thanh và nhạc nền | 8 | Chưa phân công |

## Roadmap

Nguyên tắc sắp xếp: có thứ chơi được sớm nhất, và thử đường triển khai sớm vì đó là rủi ro lớn nhất khi demo. Bước 1 đến 3 không cần server, nên Form của bước 4 và 5 có thể được kéo song song trong lúc đó.

### Bước 0: Chuẩn bị solution (đã xong)

- [x] Thêm project `CaroGame.Server`, `CaroGame.BLL`, `CaroGame.DAL` vào solution (tách 3 tầng ngày 17/10/2026).
- [x] GUI tham chiếu BLL, BLL tham chiếu DAL, Server tham chiếu BLL.
- [x] File cấu hình mẫu cho Client và Server.
- [x] `.gitignore` chặn các file cấu hình chứa bí mật.

**Kết quả:** cả nhóm clone về và build được.

### Bước 1: Luật chơi và PvP Offline (đã xong)

- [x] Luật chơi trong `CaroGame.BLL`: nước đi hợp lệ, kiểm tra thắng, luật chặn 2 đầu, kiểm tra hòa.
- [x] Vẽ bàn cờ 20x20 trên `pnlBoard` và xử lý click.
- [x] Đồng hồ 180 giây mỗi người.
- [x] Undo lùi 1 nước.
- [x] FF và hộp thoại kết quả.

**Form cần có trước:** `frmGame`, `frmResult` (TV1).
**Kết quả:** chơi được 2 người chung máy từ đầu tới cuối trận.

### Bước 2: PvE (đã xong)

- [x] Bot chấm điểm từng ô trống.
- [x] Undo lùi 2 nước.
- [x] Chat với bot bằng câu soạn sẵn.
- [x] Đọc `clientsettings.json` và gọi LLM, có xử lý lỗi 429, timeout, mất mạng.

**Form cần có trước:** không thêm.
**Việc hạ tầng:** API key Gemini.
**Kết quả:** chơi được với bot, chat được với AI.

### Bước 3: Khung màn hình (đã xong)

- [x] Màn mở app, luồng Chơi ngay với tên Guest tự sinh.
- [x] Màn chọn chế độ, chuyển vào trận PvE và Offline.
- [x] Form hướng dẫn và form Settings (âm thanh để chỗ trống).

**Form cần có trước:** `frmStart`, `frmModeSelect`, `frmInformation`, `frmSettings` (TV3).
**Kết quả:** đi được hết luồng của Guest.

### Bước 4: Server nền và tài khoản

- [x] Cài gói EF Core, tạo `AppDbContext`, entity, Migration đầu tiên trên LocalDB.
- [x] SignalR Hub và kết nối từ client.
- [x] Đăng ký, gửi mã email, xác minh.
- [x] Đăng nhập, session token, chặn đăng nhập 2 máy.
- [x] Quên mật khẩu.
- [ ] Thử Cloudflare tunnel với server thật.

**Form cần có trước:** `frmRegister`, `frmLogin`, `frmVerifyCode`, `frmForgotPassword`, `frmResetPassword` (TV2).
**Việc hạ tầng:** LocalDB, Gmail App Password, Cloudflare tunnel.
**Kết quả:** đăng ký và đăng nhập được từ một máy khác mạng.

### Bước 5: Profile và dữ liệu người chơi (đã xong)

- [x] Profile, đăng xuất, giao diện Profile của Guest.
- [x] Avatar mặc định và upload avatar.
- [x] Gửi kết quả PvE/Offline lên server, màn lịch sử đấu.
- [x] Bảng xếp hạng top 50 và thứ hạng của mình.

**Form cần có trước:** `frmProfile`, `frmHistory`, `frmLeaderboard` (TV3).
**Việc hạ tầng:** bộ ảnh DiceBear.
**Kết quả:** xong toàn bộ phần tài khoản.

### Bước 6: PvP Online (đã xong, trừ giao diện bỏ phiếu luật)

- [x] Ghép trận tự động theo điểm, hủy tìm trận.
- [x] Hỏi luật chặn 2 đầu trước trận: hỏi bằng MessageBox Có/Không khi vào trận, cần cả 2 người đồng ý mới bật luật; không trả lời trong 15 giây thì chơi luật thường (nhóm chốt bỏ `frmRuleVote`).
- [x] Nước đi do server xác nhận, đồng hồ chạy trên server.
- [x] Chat realtime.
- [x] Undo có xin phép, hạn 15 giây, giới hạn 3 lần bị từ chối.
- [x] FF, hết giờ, tính điểm rank, lưu lịch sử.
- [x] Mất kết nối và kết nối lại trong 30 giây.

**Form cần có trước:** `frmMatchmaking`, `frmRuleVote`, `frmUndoRequest` (TV1).
**Kết quả:** chơi được trận xếp hạng giữa 2 máy.

### Bước 7: Triển khai

- [ ] Cài SQL Server Express và chạy Migration trên máy host.
- [ ] Chạy server sau Cloudflare tunnel ở subdomain cố định.
- [ ] Thử 2 máy ở 2 mạng khác nhau.
- [ ] Tập phương án dự phòng: server và DB chạy trên laptop demo, client trỏ localhost.

**Kết quả:** sẵn sàng demo.

Hướng dẫn chi tiết ở mục [Triển khai lên máy host](#triển-khai-lên-máy-host).

### Bước 8: Hoàn thiện

- [x] Âm thanh và nhạc nền: code đã xong. **Còn chờ file**: đặt `move.mp3` (tiếng đánh quân) và `music.mp3` (nhạc nền) vào thư mục `CaroGame.UI/Sounds/`. Thiếu file thì game vẫn chạy, chỉ không có tiếng.
- [x] Nội dung form hướng dẫn (Information).
- [x] Rà lại toàn bộ luồng: đã kiểm thử tự động luật chơi, bot, tài khoản và 2 người chơi Online qua server thật.
- [ ] Bấm thử toàn bộ giao diện trên máy thật (cả nhóm).
- [x] Câu hỏi thầy có thể hỏi: xem mục [Câu hỏi bảo vệ](#câu-hỏi-bảo-vệ).

## Triển khai lên máy host

Bước 7 của roadmap. Làm trên máy của thành viên giữ server; máy này phải bật suốt lúc mọi người chơi Online.

### A. Chuẩn bị (chỉ làm 1 lần)

**A1. Phần mềm và code**

1. Cài **.NET 10 SDK** và **Git**.
2. Lấy code về: `git clone https://github.com/khoinguyenk6vn/CaroGame-WinForms.git` (đã có thì `git pull`).
3. Cài công cụ Migration: `dotnet tool install --global dotnet-ef --version 10.0.13`

**A2. SQL Server Express và CSDL**

1. Tải **SQL Server Express** (miễn phí) từ trang Microsoft, chọn kiểu cài **Basic**. Tên instance thường là `SQLEXPRESS`.
2. Sao chép `CaroGame.Server/appsettings.Local.example.json` thành `CaroGame.Server/appsettings.Local.json`, sửa dòng `Default`:
   ```json
   "Default": "Server=.\\SQLEXPRESS;Database=CaroGame;Trusted_Connection=True;TrustServerCertificate=True"
   ```
3. Tạo CSDL: `dotnet ef database update --project CaroGame.DAL --startup-project CaroGame.Server`

**A3. Gmail gửi mã xác minh**

1. Đăng nhập **đúng** Gmail dùng để gửi mã (nếu trình duyệt mở nhiều tài khoản Google, kiểm tra ảnh đại diện góc phải).
2. Vào **myaccount.google.com → Bảo mật**, bật **Xác minh 2 bước**.
3. Vào **myaccount.google.com/apppasswords**, tạo mật khẩu tên `Caro`. Chép 16 chữ cái, **viết liền, bỏ dấu cách**.
4. Điền vào `appsettings.Local.json`: `"Address": "gmail-cua-nhom@gmail.com"`, `"AppPassword": "16chucaivietlien"`.

**A4. Cloudflare tunnel ra `caro.vernfy.com`**

Điều kiện: tên miền `vernfy.com` đang ở trạng thái **Active** trong trang quản lý Cloudflare của nhóm.

1. Cài: `winget install --id Cloudflare.cloudflared`, đóng và mở lại PowerShell, kiểm tra bằng `cloudflared --version`.
2. Đăng nhập: `cloudflared tunnel login` (trình duyệt mở ra, chọn `vernfy.com`).
3. Tạo tunnel: `cloudflared tunnel create caro`
4. Gắn tên miền: `cloudflared tunnel route dns caro caro.vernfy.com` (nếu báo bản ghi đã có: vào Cloudflare → DNS, xóa bản ghi `caro` cũ rồi chạy lại).

**A5. Không cho máy host ngủ**

Mở PowerShell bằng quyền Administrator: `powercfg /change standby-timeout-ac 0`. Cắm sạc. Laptop thì vào **Control Panel → Power Options → Choose what closing the lid does**, chọn **Do nothing**.

### B. Mỗi lần mở server cho mọi người chơi

1. **Cửa sổ PowerShell 1, chạy server** (để mở suốt):
   ```powershell
   dotnet run --project CaroGame.Server/CaroGame.Server.csproj --launch-profile http
   ```
2. **Cửa sổ PowerShell 2, chạy tunnel** (để mở suốt):
   ```powershell
   cloudflared tunnel run --url http://localhost:5023 caro
   ```
3. Mở `https://caro.vernfy.com` trên trình duyệt, thấy chữ "Caro server dang chay" là xong.

Mở địa chỉ đó bằng trình duyệt **chỉ để kiểm tra** server còn sống. Game là ứng dụng Windows, người chơi phải chạy file `CaroGame.UI.exe` (mục C).

### C. Đóng gói game gửi người chơi

1. Sửa `CaroGame.UI/clientsettings.json`: `"ServerUrl": "https://caro.vernfy.com"`.
2. Đóng gói (bản chạy được trên máy không cài .NET):
   ```powershell
   dotnet publish CaroGame.UI/CaroGame.UI.csproj -c Release -r win-x64 --self-contained true -o publish\CaroGame
   ```
3. Nén thành zip (dùng cách này thay cho Compress-Archive, vì Windows Defender hay khóa file mới tạo):
   ```powershell
   Add-Type -AssemblyName System.IO.Compression.FileSystem
   Remove-Item publish\CaroGame.zip -ErrorAction SilentlyContinue
   [IO.Compression.ZipFile]::CreateFromDirectory("$PWD\publish\CaroGame", "$PWD\publish\CaroGame.zip")
   ```
4. Gửi `publish\CaroGame.zip` cho người chơi. Họ giải nén, chạy `CaroGame.UI.exe`. Nếu Windows hiện "Windows protected your PC" thì bấm **More info → Run anyway** (file chưa được ký số, không phải virus).

File zip chứa **API key Gemini** trong `clientsettings.json`: chỉ gửi riêng trong nhóm, không đưa lên chỗ công khai. Thư mục `publish` đã nằm trong `.gitignore`.

### D. Khi có code mới

1. Ở cửa sổ server bấm **Ctrl+C** để tắt server. Tunnel để nguyên.
2. `git pull`
3. Nếu có Migration mới: `dotnet ef database update --project CaroGame.DAL --startup-project CaroGame.Server`
4. Chạy lại server như mục B1.
5. Nếu code **giao diện hoặc BLL** thay đổi: đóng gói lại như mục C và gửi zip mới cho người chơi. Chỉ sửa phía server thì người chơi không cần tải lại.

### E. Thử bằng 2 máy ở 2 mạng khác nhau

Ví dụ 1 máy dùng wifi, 1 máy phát 4G từ điện thoại:

1. Đăng ký 2 tài khoản khác nhau; mã xác minh phải về Gmail (xem cả thư mục Spam).
2. Cả 2 bấm PvP Online. Khi vào trận, cả 2 trả lời câu hỏi luật chặn 2 đầu, đánh hết 1 trận; điểm, lịch sử, bảng xếp hạng phải cập nhật.
3. Thử Undo, chat, đầu hàng, đổi avatar, tắt wifi 1 máy khoảng 10 giây rồi bật lại (trận phải chơi tiếp được).

### F. Phương án dự phòng (mạng trường chặn hoặc tunnel lỗi)

Chạy server và SQL Server ngay trên laptop demo, để `"ServerUrl": "http://localhost:5023"` trong `clientsettings.json`, mở 2 cửa sổ game trên cùng laptop, đăng nhập 2 tài khoản khác nhau để demo Online.

### G. Lỗi thường gặp

| Hiện tượng | Cách xử lý |
|---|---|
| Game báo "Không kết nối được server." | Kiểm tra 2 cửa sổ server và tunnel còn chạy; mở `https://caro.vernfy.com` trên máy host |
| Không nhận được mã xác minh | Xem cửa sổ server: dòng `[Loi gui mail]` là sai Gmail/App Password, làm lại mục A3 rồi chạy lại server |
| "Tài khoản đang đăng nhập ở nơi khác" | Tài khoản đang mở ở máy khác; đóng game ở máy đó hoặc dùng tài khoản khác |
| Bấm PvP Online chờ mãi | Cần 2 người khác tài khoản cùng bấm PvP Online |
| Bot chat chỉ nói câu có sẵn | Xem dòng "(Bot đang dùng câu soạn sẵn vì ...)" trong khung chat; thường là thiếu `clientsettings.json` hoặc hết lượt gọi miễn phí |
| Mọi người rớt mạng giữa chừng | Máy host đã ngủ hoặc tắt cửa sổ server; làm lại mục A5 và B |

## Câu hỏi bảo vệ

| Câu hỏi | Trả lời ngắn |
|---|---|
| Vì sao chia 3 tầng GUI, BLL, DAL? | Mỗi tầng một việc: GUI nhận thao tác và hiển thị, BLL chứa nghiệp vụ, DAL là nơi duy nhất đọc/ghi CSDL. Sửa một tầng ít ảnh hưởng tầng khác. |
| BLL và GUI không cài NuGet sao dùng được EF Core, BCrypt, SignalR? | Tham chiếu bắc cầu: GUI tham chiếu BLL, BLL tham chiếu DAL, nên gói cài ở DAL dùng được ở các project phía trên. |
| Sao không dùng ADO.NET? | EF Core sinh câu SQL từ LINQ, không phải viết SQL bằng tay và tránh SQL Injection. Code của nhóm không có `SqlConnection`, `SqlCommand` nào. |
| Luật chặn 2 đầu kiểm tra thế nào? | Từ quân vừa đặt, đếm quân cùng màu liên tiếp về 2 phía theo 4 hướng. Đủ 5 thì xét 2 ô sát 2 đầu: chỉ khi cả 2 ô đều là quân đối thủ mới không tính thắng. Ô ngoài bàn không phải quân đối thủ nên mép bàn không tính là chặn. |
| Bot chọn nước đi thế nào? | Thắng được thì đánh, đối thủ sắp thắng thì chặn, còn lại chấm điểm từng ô trống theo độ dài hàng và số đầu còn trống của cả mình lẫn đối thủ, chọn ô điểm cao nhất. |
| Sao mật khẩu lưu hash? | BCrypt hash 1 chiều có muối ngẫu nhiên, lộ CSDL cũng không lấy lại được mật khẩu gốc. Đăng nhập thì `BCrypt.Verify` so mật khẩu nhập với hash. |
| Ai quyết định nước đi Online hợp lệ? | Server. Client chỉ gửi ô muốn đánh, server kiểm tra lượt, ô trống, luật rồi mới gửi `MoveMade` cho cả 2 máy; client chỉ vẽ theo tin server gửi. |
| Sao cập nhật giao diện từ SignalR phải dùng BeginInvoke? | Tin SignalR đến trên luồng khác luồng giao diện, mà control chỉ được sửa trên luồng giao diện. Dùng BeginInvoke (không chờ) để không chặn luồng nhận tin, ví dụ đang mở hộp thoại Undo vẫn nhận được tin hết giờ để tự đóng. |
| `lock` trong server để làm gì? | Nhiều người gọi server cùng lúc trên nhiều luồng. `lock` bắt các thao tác trên cùng 1 trận (hoặc danh sách người online) chạy lần lượt, không ghi đè lẫn nhau. |
| Đồng hồ Online chạy ở đâu? | Ở server: `GameTickService` mỗi giây gọi `TickAsync`, trừ 1 giây của người đang tới lượt rồi gửi `ClockUpdate` cho 2 máy. Client không tự đếm nên không gian lận được. |
| Mất kết nối giữa trận thì sao? | Server giữ phiên 30 giây và tạm dừng trận. SignalR tự kết nối lại, client gửi token để server gắn lại. Quá 30 giây thì xử thua như đầu hàng (−100) và giải phóng phiên. |
| Tính điểm rank thế nào? | n là số quân trên bàn. Thắng: n ≤ 25 được 50, sau đó giảm dần, thấp nhất 35. Thua: tương tự, trừ ít nhất 25. Đầu hàng hoặc mất kết nối bị trừ 100. Điểm không xuống dưới 1000. |
| Sao `await Task.Delay(1000)` mà không dùng `Thread.Sleep`? | `Thread.Sleep` làm đứng luồng giao diện, cửa sổ bị treo. `await Task.Delay` chờ mà giao diện vẫn chạy. |

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
