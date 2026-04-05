CREATE PROCEDURE SP_Producto_Actualizar
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(150),
    @Descripcion NVARCHAR(300),
    @Precio DECIMAL(18,2),
    @Stock INT,
    @CodigoBarras NVARCHAR(50),
    @IdSubCategoria UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE Producto
    SET
        Nombre = @Nombre,
        Descripcion = @Descripcion,
        Precio = @Precio,
        Stock = @Stock,
        CodigoBarras = @CodigoBarras,
        IdSubCategoria = @IdSubCategoria
    WHERE Id = @Id;
END;
GO
