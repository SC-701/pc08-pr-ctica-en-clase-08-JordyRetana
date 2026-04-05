CREATE PROCEDURE SP_Categoria_Listar
AS
BEGIN
    SELECT Id, Nombre
    FROM Categorias;
END;
GO
