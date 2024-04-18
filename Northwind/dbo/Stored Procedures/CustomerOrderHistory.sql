CREATE PROCEDURE CustomerOrderHistory(@CustomerID varchar(5))
as
begin 
SELECT
      ProductName,
      SUM(Quantity) as TOTAL
   FROM Products P
      JOIN [Order Details] OD ON P.ProductID = OD.ProductID
      JOIN Orders O ON OD.OrderID = O.OrderID
      JOIN Customers C ON O.CustomerID = C.CustomerID
   WHERE C.CustomerID = @CustomerID
   GROUP BY ProductName;
END 

