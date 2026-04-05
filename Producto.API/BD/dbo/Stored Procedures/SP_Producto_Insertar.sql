CREATE PROCEDURE SP_Producto_Insertar
    @Id UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(150),
    @Descripcion NVARCHAR(300),
    @Precio DECIMAL(18,2),
    @Stock INT,
    @CodigoBarras NVARCHAR(50),
    @IdSubCategoria UNIQUEIDENTIFIER
AS
BEGIN
    INSERT INTO Producto (
        Id,
        Nombre,
        Descripcion,
        Precio,
        Stock,
        CodigoBarras,
        IdSubCategoria
    )
    VALUES (
        @Id,
        @Nombre,
        @Descripcion,
        @Precio,
        @Stock,
        @CodigoBarras,
        @IdSubCategoria
    );
END;
GO
