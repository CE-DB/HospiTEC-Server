CREATE DATABASE hospitec;

CREATE USER hospitec WITH ENCRYPTED PASSWORD '3H_=Tps!22VdrS2G';

GRANT hospitec TO dbmaster;

GRANT CONNECT ON DATABASE hospitec TO hospitec;

CREATE SCHEMA doctor AUTHORIZATION hospitec;
CREATE SCHEMA admin AUTHORIZATION hospitec;

GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA doctor, admin TO hospitec;

GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA admin, doctor TO hospitec;

CREATE TABLE admin.Person (
  Identification VARCHAR(12),
  First_Name VARCHAR(15) NOT NULL,
  Last_Name VARCHAR(15) NOT NULL,
  Birth_Date DATE NOT NULL,
  Phone_Number VARCHAR(8) NOT NULL,
  Province VARCHAR(20) NOT NULL,
  Canton VARCHAR(40) NOT NULL,
  Exact_Address VARCHAR(500) NOT NULL,
  PRIMARY KEY(Identification)
);

CREATE TABLE admin.Role (
  Name VARCHAR(15),
  PRIMARY KEY(Name)
);

CREATE TABLE admin.Patient (
  Identification VARCHAR(12),
  Patient_Password VARCHAR(100) NOT NULL,
  PRIMARY KEY(Identification),
  FOREIGN KEY (Identification) REFERENCES admin.Person(Identification)
);

CREATE TABLE admin.Staff (
  Name VARCHAR(15),
  Identification VARCHAR(12),
  Admission_Date Date NOT NULL,
  Staff_Password VARCHAR(100) NOT NULL,
  FOREIGN KEY (Name) REFERENCES admin.Role (Name),
  PRIMARY KEY (Name, Identification)
);

CREATE TABLE doctor.Medical_Procedures (
  Name VARCHAR(50) NOT NULL,
  Recovering_Days SMALLINT NOT NULL CHECK (Recovering_Days > 0),
  PRIMARY KEY(Name)
);

CREATE TABLE admin.Reservation (
  Identification VARCHAR(12),
  Check_In_Date DATE,
  Check_Out_Date DATE /*Store Procedure*/,
  PRIMARY KEY(Identification, Check_In_Date),
  FOREIGN KEY (Identification) REFERENCES admin.Patient (Identification)
);

CREATE TABLE doctor.Medical_Room (
  ID_Room INT CHECK ( ID_Room > 0 ),
  Floor_Number SMALLINT NOT NULL,
  Name VARCHAR(100) NOT NULL,
  Capacity SMALLINT NOT NULL CHECK ( Capacity >= 0 ),
  Care_Type VARCHAR(50) NOT NULL,
  PRIMARY KEY (ID_Room)
);

CREATE TABLE doctor.Bed (
  ID_Bed INT CHECK ( ID_Bed > 0 ),
  Is_ICU BOOLEAN NOT NULL,
  ID_Room INT NOT NULL,
  PRIMARY KEY (ID_Bed),
  FOREIGN KEY (ID_Room) REFERENCES doctor.Medical_Room (ID_Room)
);

CREATE TABLE admin.Reservation_Bed (
  Identification VARCHAR(12),
  ID_Bed INT,
  Check_In_Date DATE,
  PRIMARY KEY (Identification, ID_Bed, Check_In_Date),
  FOREIGN KEY (ID_Bed) REFERENCES doctor.Bed (ID_Bed),
  FOREIGN KEY (Identification, Check_In_Date) REFERENCES admin.Reservation (Identification, Check_In_Date)
);

CREATE TABLE doctor.Clinic_Record (
  Identification VARCHAR(12),
  Pathology_Name VARCHAR(100),
  Diagnostic_Date DATE,
  Treatment VARCHAR(1000),
  PRIMARY KEY (Identification, Pathology_Name, Diagnostic_Date),
  FOREIGN KEY (Identification) REFERENCES admin.Patient(Identification)
);

CREATE TABLE doctor.Medical_Equipment (
  Serial_Number VARCHAR(50),
  Name VARCHAR(50) NOT NULL,
  Stock INT NOT NULL CHECK ( Stock >= 0 ),
  Provider VARCHAR(50) NOT NULL,
  PRIMARY KEY (Serial_Number)
);

CREATE TABLE doctor.Medical_Equipment_Bed (
  ID_Bed INT,
  Serial_Number VARCHAR(50) NOT NULL,
  PRIMARY KEY (ID_Bed, Serial_Number),
  FOREIGN KEY (ID_Bed) REFERENCES doctor.Bed (ID_Bed),
  FOREIGN KEY (Serial_Number) REFERENCES doctor.Medical_Equipment (Serial_Number)
);

CREATE TABLE doctor.Medical_Procedure_Record(
  Identification varchar(12),
  Pathology_Name VARCHAR(100),
  Diagnostic_Date DATE,
  Procedure_Name VARCHAR(50),
  Operation_Execution_Date DATE,
  PRIMARY KEY (Identification, Pathology_Name, Procedure_Name, Diagnostic_Date, Operation_Execution_Date),
  FOREIGN KEY (Identification, Pathology_Name, Diagnostic_Date) REFERENCES doctor.Clinic_Record (Identification, Pathology_Name, Diagnostic_Date),
  FOREIGN KEY (Procedure_Name) REFERENCES doctor.Medical_Procedures (Name)
);

CREATE TABLE doctor.Medical_Procedure_Reservation (
  Identification VARCHAR(12),
  Check_In_Date DATE,
  Name VARCHAR(50),
  PRIMARY KEY (Identification, Check_In_Date, Name),
  FOREIGN KEY (Identification, Check_In_Date) REFERENCES admin.reservation (Identification, Check_In_Date),
  FOREIGN KEY (Name) REFERENCES doctor.medical_procedures (Name)
);

CREATE OR REPLACE FUNCTION delete_record_procedures()
    RETURNS TRIGGER
    LANGUAGE plpgsql
AS $$
BEGIN

    IF EXISTS(SELECT *
                FROM doctor.medical_procedure_record AS d
                WHERE d.Identification = OLD.Identification
                AND d.Diagnostic_Date = OLD.Diagnostic_Date
                AND d.Pathology_Name = OLD.Pathology_Name)
    THEN

        DELETE FROM doctor.medical_procedure_record
        WHERE Identification = OLD.Identification
        AND Diagnostic_Date = OLD.Diagnostic_Date
        AND Pathology_Name = OLD.Pathology_Name;
    end if;

    RETURN OLD;
END
$$;

CREATE TRIGGER record_procedures_deleter
    BEFORE DELETE
    ON doctor.clinic_record
    FOR EACH ROW
    EXECUTE PROCEDURE delete_record_procedures();

CREATE OR REPLACE FUNCTION delete_beds_procedures_reserved()
    RETURNS TRIGGER
    LANGUAGE plpgsql
AS $$
BEGIN

    IF EXISTS(SELECT *
                FROM admin.reservation_bed AS d
                WHERE d.Identification = OLD.Identification
                AND d.check_in_date = OLD.check_in_date)
    THEN

        DELETE FROM admin.reservation_bed
        WHERE Identification = OLD.Identification
        AND check_in_date = OLD.check_in_date;
    end if;

    IF EXISTS(SELECT *
                FROM doctor.Medical_Procedure_Reservation AS d
                WHERE d.Identification = OLD.Identification
                AND d.check_in_date = OLD.check_in_date)
    THEN
        DELETE FROM doctor.Medical_Procedure_Reservation
        WHERE Identification = OLD.Identification
        AND check_in_date = OLD.check_in_date;
    end if;

    RETURN OLD;
END
$$;

CREATE TRIGGER bed_procedure_reserved_deleter
    BEFORE DELETE
    ON admin.reservation
    FOR EACH ROW
    EXECUTE PROCEDURE delete_beds_procedures_reserved();


CREATE OR REPLACE FUNCTION delete_patients_records_reservation()
    RETURNS TRIGGER
    LANGUAGE plpgsql
AS $$
BEGIN

    IF EXISTS(SELECT *
                FROM doctor.clinic_record AS d
                WHERE d.Identification = OLD.Identification)
    THEN

        DELETE FROM doctor.clinic_record
        WHERE Identification = OLD.Identification;
    end if;

    IF EXISTS(SELECT *
                FROM admin.reservation AS d
                WHERE d.Identification = OLD.Identification)
    THEN

        DELETE FROM admin.reservation
        WHERE Identification = OLD.Identification;
    end if;

    RETURN OLD;
END
$$;

CREATE TRIGGER patients_deleter
    BEFORE DELETE
    ON admin.patient
    FOR EACH ROW
    EXECUTE PROCEDURE delete_patients_records_reservation();

INSERT INTO doctor.medical_equipment(serial_number, name, stock, provider)
VALUES
       ('10', 'luces quirurgicas', 76, 'Empresa A'),
       ('20', 'ultrasonidos', 34, 'Empresa B'),
       ('30', 'esterilizadores', 65, 'Empresa C'),
       ('40', 'desfibriladores', 2, 'Empresa A'),
       ('50', 'monitores', 98, 'Empresa B'),
       ('60', 'respiradores artificiales', 76, 'Empresa C'),
       ('70', 'electrocardiografos', 23, 'Empresa A');

INSERT INTO doctor.medical_procedures(name, recovering_days)
VALUES
       ('apendicetomia', 28),
       ('biopsa de mama', 14),
       ('cirugia de cataratas', 10),
       ('cesarea', 20),
       ('histerectomia', 40),
       ('cirugia para lumbagia', 50),
       ('mastectomia', 30),
       ('amigdalectomia', 22);
