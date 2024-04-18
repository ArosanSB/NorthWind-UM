CREATE PROCEDURE TOP10ExpProducts
as
begin 
SELECT
		TOP 10
      P.ProductName,
	  P.UnitPrice
   FROM Products P
   order by UnitPrice DESC
END 



