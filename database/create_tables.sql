CREATE TABLE CurrencyRates (
    Id INT IDENTITY PRIMARY KEY,
    CurrencyCode NVARCHAR(3) NOT NULL,
    CurrencyName NVARCHAR(50),
    Rate DECIMAL(10,4) NOT NULL,
    RateDate DATE NOT NULL
);


ALTER TABLE CurrencyRates
ADD CONSTRAINT UQ_Currency_Date UNIQUE (CurrencyCode, RateDate);


CREATE INDEX IX_Currency_Date
ON CurrencyRates (CurrencyCode, RateDate);

