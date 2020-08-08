CREATE DATABASE hospitec;

CREATE USER hospitec WITH ENCRYPTED PASSWORD '3H_=Tps!22VdrS2G';

GRANT hospitec TO dbmaster;

GRANT CONNECT ON DATABASE hospitec TO hospitec;

CREATE SCHEMA doctor AUTHORIZATION hospitec;
CREATE SCHEMA admin AUTHORIZATION hospitec;

GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA doctor TO hospitec;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA admin TO hospitec;

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
  FOREIGN KEY (Identification) REFERENCES admin.Person(Identification) ON UPDATE CASCADE
);

CREATE TABLE admin.Staff (
  Name VARCHAR(15),
  Identification VARCHAR(12),
  Admission_Date Date NOT NULL,
  Staff_Password VARCHAR(100) NOT NULL,
  FOREIGN KEY (Name) REFERENCES admin.Role (Name),
  FOREIGN KEY (Identification) REFERENCES admin.Person (Identification) ON UPDATE CASCADE,
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
  ID_Bed INT,
  PRIMARY KEY(Identification, Check_In_Date),
  FOREIGN KEY (Identification) REFERENCES admin.Patient (Identification) ON UPDATE CASCADE,
  FOREIGN KEY (ID_Bed) REFERENCES doctor.Bed (ID_Bed) ON UPDATE CASCADE
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
  ID_Room INT,
  PRIMARY KEY (ID_Bed),
  FOREIGN KEY (ID_Room) REFERENCES doctor.Medical_Room (ID_Room) ON UPDATE CASCADE
);

CREATE TABLE doctor.Clinic_Record (
  Identification VARCHAR(12),
  Pathology_Name VARCHAR(30),
  Diagnostic_Date DATE,
  Treatment VARCHAR(1000),
  PRIMARY KEY (Identification, Pathology_Name, Diagnostic_Date),
  FOREIGN KEY (Identification) REFERENCES admin.Patient(Identification) ON UPDATE CASCADE
);

CREATE TABLE doctor.Medical_Equipment (
  Serial_Number VARCHAR(50),
  Name VARCHAR(50) NOT NULL,
  Stock INT NOT NULL CHECK ( Stock >= 0 ),
  Provider VARCHAR(200) NOT NULL,
  PRIMARY KEY (Serial_Number)
);

CREATE TABLE doctor.Medical_Equipment_Bed (
  ID_Bed INT,
  Serial_Number VARCHAR(50) NOT NULL,
  PRIMARY KEY (ID_Bed, Serial_Number),
  FOREIGN KEY (ID_Bed) REFERENCES doctor.Bed (ID_Bed) ON UPDATE CASCADE ON DELETE CASCADE,
  FOREIGN KEY (Serial_Number) REFERENCES doctor.Medical_Equipment (Serial_Number) ON UPDATE CASCADE
);

CREATE TABLE doctor.Medical_Procedure_Record(
  Identification varchar(12),
  Pathology_Name VARCHAR(30),
  Diagnostic_Date DATE,
  Procedure_Name VARCHAR(50),
  Operation_Execution_Date DATE,
  PRIMARY KEY (Identification, Pathology_Name, Procedure_Name, Diagnostic_Date, Operation_Execution_Date),
  FOREIGN KEY (Identification, Pathology_Name, Diagnostic_Date) REFERENCES doctor.Clinic_Record (Identification, Pathology_Name, Diagnostic_Date) ON UPDATE CASCADE,
  FOREIGN KEY (Procedure_Name) REFERENCES doctor.Medical_Procedures (Name)
);

CREATE TABLE doctor.Medical_Procedure_Reservation (
  Identification VARCHAR(12),
  Check_In_Date DATE,
  Name VARCHAR(50),
  PRIMARY KEY (Identification, Check_In_Date, Name),
  FOREIGN KEY (Identification, Check_In_Date) REFERENCES admin.reservation (Identification, Check_In_Date) ON UPDATE CASCADE,
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

CREATE OR REPLACE FUNCTION delete_medical_equipment()
    RETURNS TRIGGER
    LANGUAGE plpgsql
AS $$
BEGIN

    DELETE FROM doctor.medical_equipment_bed
    WHERE serial_number = OLD.serial_number;

    RETURN OLD;
END
$$;

CREATE TRIGGER equipment_deleter
    BEFORE DELETE
    ON doctor.medical_equipment
    FOR EACH ROW
    EXECUTE PROCEDURE delete_medical_equipment();

CREATE OR REPLACE PROCEDURE delete_beds_procedures_reserved(
    patientId varchar(12),
    checkInDate Date = null
)
    LANGUAGE plpgsql
AS $$
BEGIN

    IF patientId IS NULL

    then
        RAISE EXCEPTION 'Patient id is null'
      USING HINT = 'You must provide a valid patient id';

    end if;

    IF checkInDate IS NULL

    THEN
        IF EXISTS(SELECT *
                    FROM doctor.Medical_Procedure_Reservation AS d
                    WHERE Identification = patientId)
        THEN
            DELETE FROM doctor.Medical_Procedure_Reservation
            WHERE Identification = patientId;
        end if;

        DELETE FROM admin.reservation
        WHERE Identification = patientId;

    else
        IF EXISTS(SELECT *
                    FROM doctor.Medical_Procedure_Reservation AS d
                    WHERE Identification = patientId
                    AND check_in_date = checkInDate)
        THEN
            DELETE FROM doctor.Medical_Procedure_Reservation
            WHERE Identification = patientId
            AND check_in_date = checkInDate;
        end if;

        DELETE FROM admin.reservation
        WHERE Identification = patientId
        AND check_in_date = checkInDate;
    end if;
END
$$;

CREATE OR REPLACE PROCEDURE delete_patients_records_reservation(
    patientId varchar(12)
)
    LANGUAGE plpgsql
AS $$
BEGIN

    IF patientId IS NULL

    then
        RAISE EXCEPTION 'Patient id is null'
      USING HINT = 'You must provide a valid patient id';

    end if;

    IF EXISTS(SELECT *
                FROM doctor.clinic_record AS d
                WHERE d.Identification = patientId)
    THEN

        DELETE FROM doctor.clinic_record
        WHERE Identification = patientId;
    end if;

    IF EXISTS(SELECT *
                FROM admin.reservation AS d
                WHERE d.Identification = patientId)
    THEN

        CALL delete_beds_procedures_reserved(patientId);

    end if;

    DELETE FROM admin.patient
    WHERE identification = patientId;

END
$$;

CREATE OR REPLACE PROCEDURE delete_medical_room(id INT)
LANGUAGE plpgsql
AS $$

BEGIN

/*
Updates ID_ROOM atribute in bed as NULL
if it matches with @Id
*/
UPDATE doctor.Bed
SET
  ID_ROOM = NULL
WHERE
  ID_ROOM = id;

/*
Deletes Medical_ROOM with corresponding @Id
*/
DELETE FROM doctor.medical_room
WHERE ID_ROOM = Id;

END $$;

CREATE OR REPLACE PROCEDURE create_procedure_for_reservation(
    patientId varchar(12),
    checkInDate Date,
    icu bool,
    procedureName varchar(50)
)
    LANGUAGE plpgsql
AS $$
declare

    days int;

BEGIN
    CREATE TEMP TABLE dates(
        startDate Date,
        endDate Date
    );

    CREATE TEMP TABLE beds(
        id INT
    );

    INSERT INTO dates(startDate, endDate)
    SELECT check_in_date, check_out_date
    from admin.reservation
    WHERE identification = patientId
    AND check_in_date = checkInDate;

    IF (select COUNT(*) FROM dates) < 1 THEN
        RAISE EXCEPTION 'No reservation match with data provided, please insert a reservation first p:% pr:%', patientId, procedureName;
    end if;

    IF (select COUNT(*)
        FROM doctor.medical_procedures
        WHERE name = procedureName) < 1 THEN
        RAISE EXCEPTION 'No procedure match with data provided.';
    end if;

    IF (select endDate from dates) IS NULL THEN
        days = (select recovering_days
                from doctor.medical_procedures
                where name = procedureName);
    else
        days = (SELECT date_part('day',
            (select endDate from dates)::timestamp - (select startDate from dates)::timestamp))
            + (select recovering_days
                from doctor.medical_procedures
                where name = procedureName);
    end if;
    delete from dates;

    insert into dates (startDate, endDate)
    values (checkInDate, checkInDate + days);

    IF (SELECT id_bed
        FROM admin.reservation
        WHERE identification = patientId
        AND check_in_date = checkInDate) IS NULL
        OR
        (SELECT COUNT(*)
            FROM admin.reservation
            WHERE ((check_in_date, check_out_date)
                    OVERLAPS
                  ((SELECT endDate from dates)+1, (SELECT check_out_date
                                                    FROM admin.reservation
                                                    WHERE identification = patientId
                                                    AND check_in_date = checkInDate)-1))) > 0 THEN

        insert into beds(id)
        select id_bed
        from doctor.bed
        where id_bed
                  not in (SELECT id_bed
                            FROM admin.reservation
                            WHERE ((check_in_date, check_out_date)
                                    OVERLAPS
                                  ((SELECT startDate from dates)-1, (SELECT endDate from dates)+1))
                            AND reservation.id_bed IS NOT NULL)
        AND is_icu = icu;

    ELSE

        INSERT INTO beds(id)
        SELECT id_bed
        FROM admin.reservation
        WHERE identification = patientId
        AND check_in_date = checkInDate;

    end if;

    IF (SELECT COUNT(*) FROM beds) < 1 THEN
        RAISE EXCEPTION 'No beds available between % and %',
            (SELECT startDate from dates),
            (SELECT endDate from dates);
    end if;

    INSERT INTO doctor.medical_procedure_reservation
        (identification, check_in_date, name)
    VALUES
    (patientId, checkInDate, procedureName);

    UPDATE admin.reservation
    SET id_bed = (SELECT id FROM beds LIMIT 1),
        check_out_date = (SELECT endDate FROM dates)
    WHERE identification = patientId
    AND check_in_date = checkInDate;

    DROP TABLE dates;
    DROP TABLE beds;
END
$$;

CREATE OR REPLACE PROCEDURE delete_procedure_for_reservation(
    patientId varchar(12),
    checkInDate Date,
    procedureName varchar(50)
)
    LANGUAGE plpgsql
AS $$
BEGIN

    IF (SELECT COUNT(*)
        from admin.reservation
        WHERE identification = patientId
        AND check_in_date = checkInDate) < 1 THEN
        RAISE EXCEPTION 'No reservation match with data provided';
    end if;

    IF (select COUNT(*)
        FROM doctor.medical_procedures
        WHERE name = procedureName) < 1 THEN
        RAISE EXCEPTION 'No procedure match with data provided.';
    end if;

    DELETE FROM doctor.medical_procedure_reservation
    WHERE check_in_date = checkInDate
    AND identification = patientId
    AND name = procedureName;

    UPDATE admin.reservation
    SET check_out_date = check_out_date -
        (SELECT recovering_days
        FROM doctor.medical_procedures
        WHERE name = procedureName LIMIT 1)
    WHERE identification = patientId
    AND check_in_date = checkInDate;

    UPDATE admin.reservation
    SET check_out_date = NULL,
        id_bed = NULL
    WHERE check_in_date = check_out_date;
END
$$;

CREATE OR REPLACE PROCEDURE update_reserved_check_in_date(
    patientId varchar(12),
    oldCheckInDate Date,
    newCheckInDate Date,
    icu bool
)
    LANGUAGE plpgsql
AS $$
declare

    days int;

BEGIN
    CREATE TEMP TABLE beds(
        id INT
    );

    days = (SELECT (SELECT date_part('day',
            check_out_date::timestamp - check_in_date::timestamp))
            FROM admin.reservation
            WHERE identification = patientId
            AND check_in_date = oldCheckInDate);

    insert into beds(id)
        select id_bed
        from doctor.bed
        where id_bed
                  not in (SELECT id_bed
                            FROM admin.reservation
                            WHERE ((check_in_date, check_out_date)
                                    OVERLAPS
                                  (newCheckInDate-1, newCheckInDate+days+1))
                            AND reservation.id_bed IS NOT NULL
                            AND identification != patientId
                            AND check_in_date != oldCheckInDate)
        AND is_icu = icu;

    IF (SELECT COUNT(*) FROM beds) < 1 THEN
        RAISE EXCEPTION 'No beds available between % and %',
            newCheckInDate,
            newCheckInDate+days;
    end if;

    UPDATE admin.reservation
    SET check_in_date = newCheckInDate,
        check_out_date = newCheckInDate + days,
        id_bed = (SELECT id FROM beds LIMIT 1)
    WHERE identification = patientId
    AND check_in_date = oldCheckInDate;

END
$$;

SELECT person.identification
FROM admin.person FULL OUTER JOIN ADMIN.Patient ON person.identification = patient.identification

call update_reserved_check_in_date(
    'N7854923',
    '2020-08-07',
    '2020-08-20',
    false);

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


