using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProyectoPrograAvanzada.Models;

namespace ProyectoPrograAvanzada.Data;

public partial class DbAlquilerVehiculosContext : DbContext
{
    public DbAlquilerVehiculosContext()
    {
    }

    public DbAlquilerVehiculosContext(DbContextOptions<DbAlquilerVehiculosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAlquilere> TAlquileres { get; set; }

    public virtual DbSet<TAlquileresDetalle> TAlquileresDetalles { get; set; }

    public virtual DbSet<TBitacora> TBitacoras { get; set; }

    public virtual DbSet<TCliente> TClientes { get; set; }

    public virtual DbSet<TEmpleado> TEmpleados { get; set; }

    public virtual DbSet<TRecibo> TRecibos { get; set; }

    public virtual DbSet<TRole> TRoles { get; set; }

    public virtual DbSet<TSucursale> TSucursales { get; set; }

    public virtual DbSet<TVehiculo> TVehiculos { get; set; }

    public virtual DbSet<TVehiculosTipo> TVehiculosTipos { get; set; }

    public virtual DbSet<VwAlquileresDetalleCompleto> VwAlquileresDetalleCompletos { get; set; }

    public virtual DbSet<VwVehiculosDisponible> VwVehiculosDisponibles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAlquilere>(entity =>
        {
            entity.HasKey(e => e.IdAlquiler).HasName("PK_Alquileres");

            entity.ToTable("T_Alquileres", "SC_AlquilerVehiculos");

            entity.HasIndex(e => e.IdCliente, "IX_alquileres_cliente");

            entity.HasIndex(e => e.Estado, "IX_alquileres_estado");

            entity.Property(e => e.IdAlquiler).HasColumnName("id_alquiler");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("iva");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.TAlquileres)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Alquileres_Clientes");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.TAlquileres)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Alquileres_Empleados");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.TAlquileres)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Alquileres_Sucursales");
        });

        modelBuilder.Entity<TAlquileresDetalle>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK_AlquileresDetalles");

            entity.ToTable("T_AlquileresDetalles", "SC_AlquilerVehiculos");

            entity.HasIndex(e => e.IdAlquiler, "IX_alqdet_alquiler");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdAlquiler).HasColumnName("id_alquiler");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.TarifaDiaria)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("tarifa_diaria");

            entity.HasOne(d => d.IdAlquilerNavigation).WithMany(p => p.TAlquileresDetalles)
                .HasForeignKey(d => d.IdAlquiler)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlqDet_Alquileres");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.TAlquileresDetalles)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlqDet_Vehiculos");
        });

        modelBuilder.Entity<TBitacora>(entity =>
        {
            entity.HasKey(e => e.IdBitacora).HasName("PK_Bitacora");

            entity.ToTable("T_Bitacora", "SC_AlquilerVehiculos");

            entity.Property(e => e.IdBitacora).HasColumnName("id_bitacora");
            entity.Property(e => e.ClavePrimaria)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("clave_primaria");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Operacion)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("operacion");
            entity.Property(e => e.Tabla)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tabla");
            entity.Property(e => e.UsuarioSql)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasDefaultValueSql("(suser_sname())")
                .HasColumnName("usuario_sql");
            entity.Property(e => e.ValoresAntes).HasColumnName("valores_antes");
            entity.Property(e => e.ValoresDespues).HasColumnName("valores_despues");
        });

        modelBuilder.Entity<TCliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK_Clientes");

            entity.ToTable("T_Clientes", "SC_AlquilerVehiculos");

            entity.HasIndex(e => e.Cedula, "IX_clientes_cedula");

            entity.HasIndex(e => e.Cedula, "UQ__T_Client__415B7BE56F6F98EF").IsUnique();

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Cedula)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.Direccion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<TEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PK_Empleados");

            entity.ToTable("T_Empleados", "SC_AlquilerVehiculos");

            entity.HasIndex(e => e.Correo, "IX_empleados_correo");

            entity.HasIndex(e => e.IdSucursal, "IX_empleados_sucursal");

            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.ContraseñaHash)
                .HasMaxLength(256)
                .HasColumnName("contrasena_hash");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Puesto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("puesto");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.TEmpleados)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Roles");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.TEmpleados)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleados_Sucursales");
        });

        modelBuilder.Entity<TRecibo>(entity =>
        {
            entity.HasKey(e => e.IdRecibo).HasName("PK_Recibos");

            entity.ToTable("T_Recibos", "SC_AlquilerVehiculos");

            entity.Property(e => e.IdRecibo).HasColumnName("id_recibo");
            entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
            entity.Property(e => e.Metodo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("metodo");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");

            entity.HasMany(d => d.IdAlquilers).WithMany(p => p.IdRecibos)
                .UsingEntity<Dictionary<string, object>>(
                    "TAlquileresRecibo",
                    r => r.HasOne<TAlquilere>().WithMany()
                        .HasForeignKey("IdAlquiler")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AlqRec_Alquileres"),
                    l => l.HasOne<TRecibo>().WithMany()
                        .HasForeignKey("IdRecibo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AlqRec_Recibos"),
                    j =>
                    {
                        j.HasKey("IdRecibo", "IdAlquiler").HasName("PK_AlquileresRecibos");
                        j.ToTable("T_AlquileresRecibos", "SC_AlquilerVehiculos");
                        j.HasIndex(new[] { "IdAlquiler" }, "IX_alqrec_alquiler");
                        j.IndexerProperty<int>("IdRecibo").HasColumnName("id_recibo");
                        j.IndexerProperty<int>("IdAlquiler").HasColumnName("id_alquiler");
                    });
        });

        modelBuilder.Entity<TRole>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK_Roles");

            entity.ToTable("T_Roles", "SC_AlquilerVehiculos");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<TSucursale>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("PK_Sucursales");

            entity.ToTable("T_Sucursales", "SC_AlquilerVehiculos");

            entity.HasIndex(e => e.Nombre, "IX_sucursales_nombre");

            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Direccion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<TVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdVehiculo).HasName("PK_Vehiculos");

            entity.ToTable("T_Vehiculos", "SC_AlquilerVehiculos");

            entity.HasIndex(e => e.Placa, "IX_vehiculos_placa");

            entity.HasIndex(e => e.IdSucursal, "IX_vehiculos_sucursal");

            entity.HasIndex(e => e.IdTipo, "IX_vehiculos_tipo");

            entity.HasIndex(e => e.Placa, "UQ__T_Vehicu__0C0574256E6CA1EC").IsUnique();

            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modelo");
            entity.Property(e => e.Placa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("placa");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.TVehiculos)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculos_Sucursales");

            entity.HasOne(d => d.IdTipoNavigation).WithMany(p => p.TVehiculos)
                .HasForeignKey(d => d.IdTipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculos_Tipos");
        });

        modelBuilder.Entity<TVehiculosTipo>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK_VehiculosTipos");

            entity.ToTable("T_VehiculosTipos", "SC_AlquilerVehiculos");

            entity.HasIndex(e => e.Descripcion, "IX_vehTipos_desc");

            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.TarifaDiaria)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("tarifa_diaria");
        });

        modelBuilder.Entity<VwAlquileresDetalleCompleto>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_AlquileresDetalleCompleto", "SC_AlquilerVehiculos");

            entity.Property(e => e.Cliente)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cliente");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdAlquiler).HasColumnName("id_alquiler");
            entity.Property(e => e.Placa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("placa");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");
        });

        modelBuilder.Entity<VwVehiculosDisponible>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_VehiculosDisponibles", "SC_AlquilerVehiculos");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modelo");
            entity.Property(e => e.Placa)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("placa");
            entity.Property(e => e.Sucursal)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("sucursal");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
