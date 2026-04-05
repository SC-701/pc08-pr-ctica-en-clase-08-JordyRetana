CREATE PROCEDURE SP_SubCategoria_Insertar
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(100),
    @IdCategoria UNIQUEIDENTIFIER
AS
BEGIN
    INSERT INTO SubCategorias (Id, Nombre, IdCategoria)
    VALUES (@Id, @Nombre, @IdCategoria);
END;
GO
