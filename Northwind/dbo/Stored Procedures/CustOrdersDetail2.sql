CREATE PROCEDURE CustOrdersDetail2(@AtOrderID int)
AS
BEGIN
   SELECT P.ProductName, ROUND(OD.UnitPrice, 2) AS 'Unit Price', OD.Quantity, ROUND(OD.Discount * 100, 2) AS 'Discount', ROUND(OD.Quantity * (1 - OD.Discount) * OD.UnitPrice, 2) AS ExtendedPrice
   FROM Products P, [Order Details] OD
   WHERE P.ProductID = OD.ProductID
   AND OrderID = @AtOrderID;
END
