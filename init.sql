CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Coolers" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Link" text NOT NULL,
    "Tdp" integer NOT NULL,
    "IntelSockets" text[] NOT NULL,
    "AmdSockets" text[] NOT NULL,
    "FanCount" integer NOT NULL,
    "Radius" integer NOT NULL,
    "Thickness" integer NOT NULL,
    "Speed" integer NOT NULL,
    "Height" integer NOT NULL,
    "Type" integer NOT NULL,
    CONSTRAINT "PK_Coolers" PRIMARY KEY ("Id")
);

CREATE TABLE "Cpus" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Link" text NOT NULL,
    "Socket" text NOT NULL,
    "Tdp" integer NOT NULL,
    "RamGen" text NOT NULL,
    "RamFrequency" integer NOT NULL,
    "Packing" integer NOT NULL,
    "Image" bytea,
    "MinPrice" integer NOT NULL,
    "MaxPrice" integer NOT NULL,
    CONSTRAINT "PK_Cpus" PRIMARY KEY ("Id")
);

CREATE TABLE "Gpus" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Link" text NOT NULL,
    "Tdp" integer NOT NULL,
    "Length" integer NOT NULL,
    "Image" bytea,
    "MaxPrice" integer NOT NULL,
    "MinPrice" integer NOT NULL,
    CONSTRAINT "PK_Gpus" PRIMARY KEY ("Id")
);

CREATE TABLE "InternalDrives" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Link" text NOT NULL,
    "Capacity" integer NOT NULL,
    "Type" text NOT NULL,
    "Format" text NOT NULL,
    "Port" text NOT NULL,
    "Image" bytea,
    "MinPrice" integer NOT NULL,
    "MaxPrice" integer NOT NULL,
    CONSTRAINT "PK_InternalDrives" PRIMARY KEY ("Id")
);

CREATE TABLE "Motherboards" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Link" text NOT NULL,
    "Socket" text NOT NULL,
    "FormFactor" integer NOT NULL,
    "Chipset" text NOT NULL,
    "RamGeneration" text NOT NULL,
    "RamSlots" integer NOT NULL,
    "RamFrequency" integer NOT NULL,
    CONSTRAINT "PK_Motherboards" PRIMARY KEY ("Id")
);

CREATE TABLE "Psus" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Link" text NOT NULL,
    "Power" integer NOT NULL,
    "Level" integer NOT NULL,
    "PowerConnector" text NOT NULL,
    "FormFactor" integer NOT NULL,
    "Modularity" integer NOT NULL,
    "Image" bytea,
    "MinPrice" integer NOT NULL,
    "MaxPrice" integer NOT NULL,
    CONSTRAINT "PK_Psus" PRIMARY KEY ("Id")
);

CREATE TABLE "Rams" (
    "Id" uuid NOT NULL,
    "Model" text NOT NULL,
    "Submodel" text,
    "Link" text NOT NULL,
    "Amount" integer NOT NULL,
    "Sticks" integer NOT NULL,
    "Voltage" real NOT NULL,
    "Generation" text NOT NULL,
    "Frequency" integer NOT NULL,
    "Image" bytea,
    "MinPrice" integer NOT NULL,
    "MaxPrice" integer NOT NULL,
    CONSTRAINT "PK_Rams" PRIMARY KEY ("Id")
);

CREATE TABLE "PsuConnector" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Count" integer NOT NULL,
    "PsuId" uuid,
    CONSTRAINT "PK_PsuConnector" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PsuConnector_Psus_PsuId" FOREIGN KEY ("PsuId") REFERENCES "Psus" ("Id")
);

CREATE TABLE "Offer" (
    "Id" uuid NOT NULL,
    "ShopName" text NOT NULL,
    "Price" numeric NOT NULL,
    "City" text,
    "CpuId" uuid,
    "GpuId" uuid,
    "InternalDriveId" uuid,
    "PsuId" uuid,
    "RamId" uuid,
    CONSTRAINT "PK_Offer" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Offer_Cpus_CpuId" FOREIGN KEY ("CpuId") REFERENCES "Cpus" ("Id"),
    CONSTRAINT "FK_Offer_Gpus_GpuId" FOREIGN KEY ("GpuId") REFERENCES "Gpus" ("Id"),
    CONSTRAINT "FK_Offer_InternalDrives_InternalDriveId" FOREIGN KEY ("InternalDriveId") REFERENCES "InternalDrives" ("Id"),
    CONSTRAINT "FK_Offer_Psus_PsuId" FOREIGN KEY ("PsuId") REFERENCES "Psus" ("Id"),
    CONSTRAINT "FK_Offer_Rams_RamId" FOREIGN KEY ("RamId") REFERENCES "Rams" ("Id")
);

CREATE INDEX "IX_Offer_CpuId" ON "Offer" ("CpuId");

CREATE INDEX "IX_Offer_GpuId" ON "Offer" ("GpuId");

CREATE INDEX "IX_Offer_InternalDriveId" ON "Offer" ("InternalDriveId");

CREATE INDEX "IX_Offer_PsuId" ON "Offer" ("PsuId");

CREATE INDEX "IX_Offer_RamId" ON "Offer" ("RamId");

CREATE INDEX "IX_PsuConnector_PsuId" ON "PsuConnector" ("PsuId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260605144158_InitialCreate', '10.0.8');

COMMIT;

START TRANSACTION;
ALTER TABLE "Rams" DROP COLUMN "Image";

ALTER TABLE "Psus" DROP COLUMN "Image";

ALTER TABLE "InternalDrives" DROP COLUMN "Image";

ALTER TABLE "Gpus" DROP COLUMN "Image";

ALTER TABLE "Cpus" DROP COLUMN "Image";

ALTER TABLE "Rams" ADD "ImageUrl" text;

ALTER TABLE "Psus" ADD "ImageUrl" text;

ALTER TABLE "Offer" ADD "CoolerId" uuid;

ALTER TABLE "Offer" ADD "MotherboardRenamedForOmnissiahId" uuid;

ALTER TABLE "Motherboards" ADD "HasM2Slot" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Motherboards" ADD "ImageUrl" text;

ALTER TABLE "Motherboards" ADD "MaxPrice" integer NOT NULL DEFAULT 0;

ALTER TABLE "Motherboards" ADD "MinPrice" integer NOT NULL DEFAULT 0;

ALTER TABLE "InternalDrives" ADD "ImageUrl" text;

ALTER TABLE "Gpus" ADD "ImageUrl" text;

ALTER TABLE "Cpus" ADD "ImageUrl" text;

ALTER TABLE "Coolers" ADD "ImageUrl" text;

ALTER TABLE "Coolers" ADD "MaxPrice" integer NOT NULL DEFAULT 0;

ALTER TABLE "Coolers" ADD "MinPrice" integer NOT NULL DEFAULT 0;

CREATE TABLE "CpuBenchmarks" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Score" integer NOT NULL,
    CONSTRAINT "PK_CpuBenchmarks" PRIMARY KEY ("Id")
);

CREATE TABLE "GpuBenchmarks" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Score" integer NOT NULL,
    CONSTRAINT "PK_GpuBenchmarks" PRIMARY KEY ("Id")
);

CREATE INDEX "IX_Offer_CoolerId" ON "Offer" ("CoolerId");

CREATE INDEX "IX_Offer_MotherboardRenamedForOmnissiahId" ON "Offer" ("MotherboardRenamedForOmnissiahId");

ALTER TABLE "Offer" ADD CONSTRAINT "FK_Offer_Coolers_CoolerId" FOREIGN KEY ("CoolerId") REFERENCES "Coolers" ("Id");

ALTER TABLE "Offer" ADD CONSTRAINT "FK_Offer_Motherboards_MotherboardRenamedForOmnissiahId" FOREIGN KEY ("MotherboardRenamedForOmnissiahId") REFERENCES "Motherboards" ("Id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260608073018_AddBenchmarks', '10.0.8');

COMMIT;

