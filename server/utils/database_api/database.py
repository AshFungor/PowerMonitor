from werkzeug.security import generate_password_hash
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

    def execute(self, sql, parameters, fetchone, fetchall, commit):
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

    def create_table_users(self):
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

    def create_table_complexes(self):
        sql = """
        CREATE TABLE IF NOT EXISTS
            complexes (
                id SERIAL PRIMARY KEY,
                serial_number INTEGER NOT NULL,
                user_id INTEGER NOT NULL
            )
        ;
        """
        self.execute(sql, commit=True)

    def create_table_telemetry(self):
        sql = """
        CREATE TABLE IF NOT EXISTS
            telemetry (
                id SERIAL PRIMARY KEY,
                serial_number INTEGER NOT NULL,
                path_to_file VARCHAR(255) NOT NULL,
                period VARCHAR(255) NOT NULL
            )
        ;
        """
        self.execute(sql, commit=True)

    def initiate(self):
        self.create_table_users()
        self.create_table_complexes()
        self.create_table_telemetry()

    def add_user(self, login, password, is_admin):
        password = generate_password_hash(password)
        sql = """
        INSERT INTO users(login, password, is_admin) VALUES(%s, %s, %s)
        """
        self.execute(sql, parameters=(login, password, is_admin), commit=True)

    def add_telemetry(self, serial_number, path_to_file, period):
        sql = """
                INSERT INTO telemetry(serial_number, path_to_file, period) VALUES(%s, %s, %s)
                """
        self.execute(sql, parameters=(serial_number, path_to_file, period), commit=True)

    def add_complex(self, serial_number, user_id):
        sql = """
                INSERT INTO complexes(serial_number, users) VALUES(%s, %s)
                """
        self.execute(sql, parameters=(serial_number, user_id), commit=True)

    def select_user_by_login(self, login):
        sql = """
        SELECT * FROM users
        WHERE login = %s
        """
        return self.execute(sql, parameters=(login,), fetchone=True)

    def select_all_users(self):
        sql = "SELECT * FROM users"
        return self.execute(sql, fetchall=True)
