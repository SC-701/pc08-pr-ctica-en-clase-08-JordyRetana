CREATE PROCEDURE SP_Categoria_Insertar
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(100)
AS
BEGIN
    INSERT INTO Categorias (Id, Nombre)
    VALUES (@Id, @Nombre);
END;
GO
