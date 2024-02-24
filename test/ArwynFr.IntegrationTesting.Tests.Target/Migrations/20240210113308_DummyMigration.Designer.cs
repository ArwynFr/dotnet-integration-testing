﻿// <auto-generated />
using ArwynFr.IntegrationTesting.Tests.Target;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ArwynFr.IntegrationTesting.Tests.Target.Migrations
{
    [DbContext(typeof(MigrationDbContext))]
    [Migration("20240210113308_DummyMigration")]
    partial class DummyMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("ArwynFr.IntegrationTesting.Tests.Target.DummyEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Entities");
                });
#pragma warning restore 612, 618
        }
    }
}