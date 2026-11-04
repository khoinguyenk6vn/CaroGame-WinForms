-- =============================================================
-- Dữ liệu mẫu cho CSDL CaroGame (chạy SAU file CaroGame.sql)
-- 4 tài khoản demo đã xác minh email, mật khẩu đều là: 123456
--   giangvien  (1000 điểm, chưa đấu trận nào)
--   demo_an    (1056 điểm)
--   demo_binh  (1098 điểm)
--   demo_chi   (1040 điểm)
-- Mật khẩu lưu dạng hash BCrypt giống hệt lúc đăng ký trong game.
-- Email dùng tên miền example.com (tên miền dành riêng cho ví dụ, không gửi thư được).
-- Lịch sử: 6 trận Online (mỗi trận 2 dòng, điểm tính đúng công thức của ScoreCalculator)
-- và 4 trận PvE/Offline. Điểm trong bảng Users khớp với tổng điểm cộng/trừ trong lịch sử.
-- Script chỉ chèn khi chưa có tài khoản demo nào, chạy lại không bị trùng.
-- =============================================================

USE [CaroGame];
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF NOT EXISTS (SELECT * FROM [Users]
               WHERE [Username] IN (N'giangvien', N'demo_an', N'demo_binh', N'demo_chi'))
BEGIN
    INSERT INTO [Users] ([Username], [Email], [PasswordHash], [IsEmailVerified], [Score], [DefaultAvatarIndex], [AvatarPath], [CreatedAt])
    VALUES
        (N'giangvien', N'giangvien@example.com', N'$2a$11$d.GD8MJR/zetwUylKCIEbeOnypfZYKGrfFZZrDhrA5aTsSCyqzvBy', 1, 1000, 0,  NULL, '2026-10-18T08:00:00'),
        (N'demo_an',   N'demo_an@example.com',   N'$2a$11$IUjckD1CFd3LAQBIZST4neXulIOvCozQn7hgYSdwD5wnSd5H3QCJW', 1, 1056, 3,  NULL, '2026-10-18T08:05:00'),
        (N'demo_binh', N'demo_binh@example.com', N'$2a$11$KnMtJMG2iP6fC/BUcMLCze6XCwHGUBGeJ2BHbclzATJT/pi9Z.yQe', 1, 1098, 7,  NULL, '2026-10-18T08:10:00'),
        (N'demo_chi',  N'demo_chi@example.com',  N'$2a$11$v9PFOb2OEYnT1u3g4dqAH.KUlYZr4.IE3sbIs2DmN7Hs/A1F9URn2', 1, 1040, 12, NULL, '2026-10-18T08:15:00');

    DECLARE @an INT   = (SELECT [Id] FROM [Users] WHERE [Username] = N'demo_an');
    DECLARE @binh INT = (SELECT [Id] FROM [Users] WHERE [Username] = N'demo_binh');
    DECLARE @chi INT  = (SELECT [Id] FROM [Users] WHERE [Username] = N'demo_chi');

    -- Trận Online: mỗi trận 2 dòng (người thắng và người thua).
    -- Điểm không bao giờ dưới 1000 nên người đang 1000 mà thua thì ghi 0.
    INSERT INTO [MatchHistories] ([UserId], [OpponentId], [Mode], [Result], [EndReason], [BlockBothEndsRule], [MoveCount], [ScoreChange], [PlayedAt])
    VALUES
        -- Trận 1: an thắng binh, 21 nước (<= 25) nên +50 / -50 (binh đang 1000 nên ghi 0)
        (@an,   @binh, N'PvPOnline', N'Win',  N'FiveInRow',  1, 21,  50, '2026-10-18T20:05:00'),
        (@binh, @an,   N'PvPOnline', N'Lose', N'FiveInRow',  1, 21,   0, '2026-10-18T20:05:00'),
        -- Trận 2: an thắng chi, 37 nước: 50 - (37 - 25) / 2 = 44
        (@an,   @chi,  N'PvPOnline', N'Win',  N'FiveInRow',  0, 37,  44, '2026-10-18T20:20:00'),
        (@chi,  @an,   N'PvPOnline', N'Lose', N'FiveInRow',  0, 37,   0, '2026-10-18T20:20:00'),
        -- Trận 3: chi hết giờ, binh thắng, 19 nước: +50 / -50
        (@binh, @chi,  N'PvPOnline', N'Win',  N'Timeout',    1, 19,  50, '2026-10-19T19:30:00'),
        (@chi,  @binh, N'PvPOnline', N'Lose', N'Timeout',    1, 19,   0, '2026-10-19T19:30:00'),
        -- Trận 4: chi mất kết nối quá 30 giây (-100), an thắng, 15 nước: +50
        (@an,   @chi,  N'PvPOnline', N'Win',  N'Disconnect', 1, 15,  50, '2026-10-19T19:50:00'),
        (@chi,  @an,   N'PvPOnline', N'Lose', N'Disconnect', 1, 15,   0, '2026-10-19T19:50:00'),
        -- Trận 5: chi thắng an, 45 nước: 50 - (45 - 25) / 2 = 40
        (@chi,  @an,   N'PvPOnline', N'Win',  N'FiveInRow',  0, 45,  40, '2026-10-20T21:10:00'),
        (@an,   @chi,  N'PvPOnline', N'Lose', N'FiveInRow',  0, 45, -40, '2026-10-20T21:10:00'),
        -- Trận 6: binh thắng an, 29 nước: 50 - (29 - 25) / 2 = 48
        (@binh, @an,   N'PvPOnline', N'Win',  N'FiveInRow',  1, 29,  48, '2026-10-20T21:30:00'),
        (@an,   @binh, N'PvPOnline', N'Lose', N'FiveInRow',  1, 29, -48, '2026-10-20T21:30:00');

    -- Trận PvE và Offline: 1 dòng, không có đối thủ và không có điểm.
    -- PvE và Offline luôn áp dụng luật chặn 2 đầu.
    INSERT INTO [MatchHistories] ([UserId], [OpponentId], [Mode], [Result], [EndReason], [BlockBothEndsRule], [MoveCount], [ScoreChange], [PlayedAt])
    VALUES
        (@an,   NULL, N'PvE',        N'Win',  N'FiveInRow', 1, 31, NULL, '2026-10-18T19:40:00'),
        (@an,   NULL, N'PvE',        N'Lose', N'FiveInRow', 1, 26, NULL, '2026-10-19T19:00:00'),
        (@binh, NULL, N'PvPOffline', N'Win',  N'Timeout',   1, 40, NULL, '2026-10-19T20:10:00'),
        (@chi,  NULL, N'PvE',        N'Lose', N'Forfeit',   1, 10, NULL, '2026-10-20T20:45:00');
END;

COMMIT;
GO
