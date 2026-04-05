CREATE PROCEDURE SP_SubCategoria_Listar
AS
BEGIN
    SELECT
        sc.Id,
        sc.Nombre,
        c.Nombre AS Categoria
    FROM SubCategorias sc
    INNER JOIN Categorias c ON sc.IdCategoria = c.Id;
END;
GO
