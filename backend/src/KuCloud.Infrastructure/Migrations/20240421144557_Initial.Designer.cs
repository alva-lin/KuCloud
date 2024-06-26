﻿// <auto-generated />
using System;
using KuCloud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KuCloud.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240421144557_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KuCloud.Core.Domains.StorageAggregate.FolderNesting", b =>
                {
                    b.Property<long>("AncestorId")
                        .HasColumnType("bigint")
                        .HasColumnName("ancestor_id");

                    b.Property<long>("DescendantId")
                        .HasColumnType("bigint")
                        .HasColumnName("descendant_id");

                    b.Property<int>("Depth")
                        .HasColumnType("integer")
                        .HasColumnName("depth");

                    b.HasKey("AncestorId", "DescendantId")
                        .HasName("pk_folder_nesting");

                    b.HasIndex("DescendantId")
                        .HasDatabaseName("ix_folder_nesting_descendant_id");

                    b.ToTable("folder_nesting", (string)null);
                });

            modelBuilder.Entity("KuCloud.Core.Domains.StorageAggregate.StorageNode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id")
                        .HasColumnOrder(1);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name")
                        .HasColumnOrder(11);

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint")
                        .HasColumnName("parent_id")
                        .HasColumnOrder(12);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type")
                        .HasColumnOrder(10);

                    b.HasKey("Id")
                        .HasName("pk_storage_nodes");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("ix_storage_nodes_parent_id");

                    b.ToTable("storage_nodes", (string)null);

                    b.HasDiscriminator<string>("Type");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("KuCloud.Core.Domains.StorageAggregate.FileNode", b =>
                {
                    b.HasBaseType("KuCloud.Core.Domains.StorageAggregate.StorageNode");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("content_type")
                        .HasColumnOrder(20);

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("size")
                        .HasColumnOrder(21);

                    b.HasDiscriminator().HasValue("File");
                });

            modelBuilder.Entity("KuCloud.Core.Domains.StorageAggregate.Folder", b =>
                {
                    b.HasBaseType("KuCloud.Core.Domains.StorageAggregate.StorageNode");

                    b.HasDiscriminator().HasValue("Folder");
                });

            modelBuilder.Entity("KuCloud.Core.Domains.StorageAggregate.FolderNesting", b =>
                {
                    b.HasOne("KuCloud.Core.Domains.StorageAggregate.Folder", "Ancestor")
                        .WithMany("DescendantRelations")
                        .HasForeignKey("AncestorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_folder_nesting_folders_ancestor_id");

                    b.HasOne("KuCloud.Core.Domains.StorageAggregate.Folder", "Descendant")
                        .WithMany("AncestorRelations")
                        .HasForeignKey("DescendantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_folder_nesting_folders_descendant_id");

                    b.Navigation("Ancestor");

                    b.Navigation("Descendant");
                });

            modelBuilder.Entity("KuCloud.Core.Domains.StorageAggregate.StorageNode", b =>
                {
                    b.HasOne("KuCloud.Core.Domains.StorageAggregate.Folder", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("fk_storage_nodes_folders_parent_id");

                    b.OwnsOne("KuCloud.Core.AuditInfo", "AuditInfo", b1 =>
                        {
                            b1.Property<long>("StorageNodeId")
                                .HasColumnType("bigint");

                            b1.Property<DateTime>("CreationTime")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Creator")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)");

                            b1.Property<string>("CreatorId")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("character varying(100)");

                            b1.Property<DateTime?>("DeletionTime")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<bool>("IsDelete")
                                .HasColumnType("boolean");

                            b1.Property<DateTime?>("ModifiedTime")
                                .HasColumnType("timestamp with time zone");

                            b1.HasKey("StorageNodeId");

                            b1.ToTable("storage_nodes");

                            b1.ToJson("audit_info");

                            b1.WithOwner()
                                .HasForeignKey("StorageNodeId")
                                .HasConstraintName("fk_storage_nodes_storage_nodes_id");
                        });

                    b.Navigation("AuditInfo")
                        .IsRequired();

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("KuCloud.Core.Domains.StorageAggregate.Folder", b =>
                {
                    b.Navigation("AncestorRelations");

                    b.Navigation("Children");

                    b.Navigation("DescendantRelations");
                });
#pragma warning restore 612, 618
        }
    }
}
