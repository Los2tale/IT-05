CREATE TABLE queues (
    id INT IDENTITY(1,1) PRIMARY KEY,
    queue_number NVARCHAR(100) NOT NULL,
);
GO

WITH LettersCTE AS (
    -- Anchor member: Start with 'A'
    SELECT 65 AS AsciiValue
    UNION ALL
    -- Recursive member: Add the next letter until 'Z' (ASCII 90)
    SELECT AsciiValue + 1
    FROM LettersCTE
    WHERE AsciiValue < 90
),
NumbersCTE AS (
    -- Anchor member: Start with 0
    SELECT 0 AS Number
    UNION ALL
    -- Recursive member: Add the next number until 9
    SELECT Number + 1
    FROM NumbersCTE
    WHERE Number < 9
),
AlphaNumericValues AS (
    SELECT
        -- Convert ASCII value to character and concatenate with number
        CAST(CHAR(L.AsciiValue) AS VARCHAR(1)) + CAST(N.Number AS VARCHAR(1)) AS SequenceValue
    FROM
        LettersCTE L
    CROSS JOIN
        NumbersCTE N
)
-- Insert the generated values into the table
INSERT INTO dbo.queues (queue_number)
SELECT SequenceValue
FROM AlphaNumericValues
ORDER BY SequenceValue
OPTION (MAXRECURSION 0);


CREATE TABLE tickets (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    queue_id INT NOT NULL,
    status NVARCHAR(20) DEFAULT 'PENDING',
    created_at DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT FK_Tickets_Queues FOREIGN KEY (queue_id) 
        REFERENCES queues(id) ON DELETE CASCADE,
    
    CONSTRAINT CHK_TicketStatus CHECK (status IN ('PENDING', 'CLEAR'))
);
GO


CREATE INDEX IX_Tickets_ActiveQueue 
ON tickets (queue_id, created_at) 
WHERE status = 'PENDING';
GO