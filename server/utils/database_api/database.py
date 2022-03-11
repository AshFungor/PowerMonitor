import psycopg2


class Database:
    def __init__(self, dbname, user, password, host):
        self.name = dbname
        self.user = user
        self.password = password
        self.host = host

    @property
    def connection(self):
        return psycopg2.connect(
            dbname=self.name,
            user=self.user,
            password=self.password,
            host=self.host
        )

    def execute(self, sql, parameters=None, fetchone=False, fetchall=False, commit=False):
        parameters = tuple() if parameters is None else parameters
        connection = self.connection
        with connection.cursor() as cursor:
            data = None
            cursor.execute(sql, parameters)
            if commit:
                connection.commit()
            if fetchone:
                data = cursor.fetchone()
            if fetchall:
                data = cursor.fetchall()

            return data

    def _create_table_users(self):
        sql = """
        CREATE TABLE IF NOT EXISTS
            users (
                id SERIAL PRIMARY KEY,
                login VARCHAR(255) UNIQUE NOT NULL,
                password VARCHAR(255) NOT NULL,
                is_admin BOOL NOT NULL
            )
        ;
        """
        self.execute(sql, commit=True)

    def _create_table_complexes(self):
        sql = """
        CREATE TABLE IF NOT EXISTS
            complexes (
                id SERIAL PRIMARY KEY,
                serial_number INTEGER NOT NULL,
                user_login VARCHAR(255) NOT NULL
            )
        ;
        """
        self.execute(sql, commit=True)

    def _create_table_telemetry(self):
        sql = """
        CREATE TABLE IF NOT EXISTS
            telemetry(
                id SERIAL PRIMARY KEY,
                serial_number INTEGER,
                start timestamp without time zone NOT NULL,
                finish timestamp without time zone NOT NULL,
                reactive_power_by_phase_A FLOAT,
                reactive_power_by_phase_B FLOAT,
                reactive_power_by_phase_C FLOAT,
                active_power_by_phase_A FLOAT,
                active_power_by_phase_B FLOAT,
                active_power_by_phase_C FLOAT,
                voltage_by_phase_A FLOAT,
                voltage_by_phase_B FLOAT,
                voltage_by_phase_C FLOAT,
                cos_by_phase_A FLOAT,
                cos_by_phase_B FLOAT,
                cos_by_phase_C FLOAT,
                reactive_power_by_phase_A_off FLOAT,
                reactive_power_by_phase_B_off FLOAT,
                reactive_power_by_phase_C_off FLOAT,
                active_power_by_phase_A_off FLOAT,
                active_power_by_phase_B_off FLOAT,
                active_power_by_phase_C_off FLOAT,
                voltage_by_phase_A_off FLOAT,
                voltage_by_phase_B_off FLOAT,
                voltage_by_phase_C_off FLOAT,
                cos_by_phase_A_off FLOAT,
                cos_by_phase_B_off FLOAT,
                cos_by_phase_C_off FLOAT,
                number_of_enabled_blocks INTEGER
            )
        ;
        """
        self.execute(sql, commit=True)

    def initiate(self):
        self._create_table_users()
        self._create_table_complexes()
        self._create_table_telemetry()

    def add_user(self, user):
        sql = """
        INSERT INTO users(login, password, is_admin) VALUES(%s, %s, %s)
        """
        self.execute(sql, parameters=(user.login, user.password, user.is_admin), commit=True)

    def add_telemetry(self, telemetry):
        sql = f"""
        INSERT INTO telemetry({', '.join(telemetry.parameter_names)})
        VALUES({', '.join(["%s"] * len(telemetry.parameter_names))})
        """
        self.execute(sql, parameters=telemetry.parameters, commit=True)

    def add_complex(self, complex):
        sql = """
        INSERT INTO complexes(serial_number, user_login) VALUES(%s, %s)
        """
        self.execute(sql, parameters=(complex.serial_number, complex.user_login), commit=True)

    def select_user_by_login(self, login):
        sql = """
        SELECT * FROM users
        WHERE login = %s
        """
        return self.execute(sql, parameters=(login,), fetchone=True)

    def select_all_users(self):
        sql = "SELECT * FROM users"
        return self.execute(sql, fetchall=True)
