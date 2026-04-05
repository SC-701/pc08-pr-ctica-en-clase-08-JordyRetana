CREATE VIEW VW_ProductoResponse
AS
SELECT
    p.Id,
    p.Nombre,
    p.Descripcion,
    p.Precio,
    p.Stock,
    p.CodigoBarras,
    sc.Nombre AS SubCategoria,
    c.Nombre AS Categoria
FROM Producto p
INNER JOIN SubCategorias sc ON p.IdSubCategoria = sc.Id
INNER JOIN Categorias c ON sc.IdCategoria = c.Id;
GO
