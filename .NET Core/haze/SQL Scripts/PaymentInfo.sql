CREATE TABLE PaymentInfos (
    Id int PRIMARY KEY,
	CreditCardNumber INT NOT NULL,
    BillingAddress VARCHAR(50) NOT NULL,
    ShippingAddress VARCHAR(50) NOT NULL,
    ExpiryDate Date NOT NULL,
    OwnerId INT NOT NULL,
    FOREIGN KEY (OwnerId) REFERENCES Users(Id)
);
